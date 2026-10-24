"""TimesFM Flask API Server"""
import sys

# Redirected Windows consoles default to cp1252, which can't print the emoji below
for stream in (sys.stdout, sys.stderr):
    if hasattr(stream, "reconfigure"):
        stream.reconfigure(encoding="utf-8")

from flask import Flask, jsonify, request
from flask_cors import CORS
import numpy as np
import pandas as pd
import yfinance as yf
from timesfm3 import TimesFM3Evaluator, ModelConfig
from datetime import datetime, timedelta
import traceback
from config import (
    FLASK_HOST, FLASK_PORT, FLASK_DEBUG, DEFAULT_TICKER,
    TIMESFM_CONFIG, FORECAST_HORIZON, HISTORICAL_PERIOD, MIN_DATA_POINTS
)

app = Flask(__name__)
CORS(app)

# Global forecaster instance
forecaster = None

def init_forecaster():
    """Initialize the TimesFM forecaster"""
    global forecaster
    try:
        print("🤖 Initializing TimesFM 3.0...")
        config = ModelConfig(
            checkpoint_path=TIMESFM_CONFIG["checkpoint_path"],
            per_core_batch_size=TIMESFM_CONFIG["per_core_batch_size"],
            device=TIMESFM_CONFIG["device"]
        )
        forecaster = TimesFM3Evaluator(config)
        print("✓ TimesFM loaded successfully")
        return True
    except Exception as e:
        print(f"✗ Error loading TimesFM: {e}")
        return False

@app.route("/api/health", methods=["GET"])
def health():
    """Health check endpoint"""
    return jsonify({
        "status": "ok",
        "forecaster": forecaster is not None,
        "timestamp": datetime.now().isoformat()
    })

def download(ticker, period):
    """Download history with flat (single-level) columns across yfinance versions"""
    data = yf.download(ticker, period=period, progress=False)
    if isinstance(data.columns, pd.MultiIndex):
        data.columns = data.columns.get_level_values(0)
    return data

@app.route("/api/forecast", methods=["POST"])
def forecast():
    """Generate forecast for a stock"""
    try:
        data = request.json
        ticker = data.get("ticker", DEFAULT_TICKER).upper()
        horizon = data.get("horizon", FORECAST_HORIZON)
        period = data.get("period", HISTORICAL_PERIOD)
        
        if not forecaster:
            return jsonify({"error": "Forecaster not initialized"}), 503
        
        # Fetch data
        stock_data = download(ticker, period)
        if stock_data.empty or len(stock_data) < MIN_DATA_POINTS:
            return jsonify({
                "error": f"Insufficient data for {ticker}. Need at least {MIN_DATA_POINTS} points."
            }), 400
        
        prices = stock_data['Close'].values.astype(np.float32)
        current_price = float(prices[-1])
        
        # Normalize to returns
        returns = np.diff(prices) / prices[:-1]
        
        # Forecast
        outputs = list(forecaster.predict_batch(
            [returns],
            horizon=horizon,
            return_quantiles=True
        ))
        
        forecast_returns = outputs[0].forecast
        quantiles_returns = outputs[0].quantiles
        
        # Convert back to prices
        forecast_prices = current_price * np.cumprod(1 + forecast_returns)
        quantiles_prices = np.zeros((horizon, 9))
        # Per-step deviations from the median are treated as independent and
        # accumulated in quadrature (random-walk), rather than compounding every
        # step at its extreme quantile, which hugely overstates the spread.
        median_idx = 4
        for i in range(9):
            dev = quantiles_returns[:, i] - quantiles_returns[:, median_idx]
            cum_dev = np.sign(dev) * np.sqrt(np.cumsum(dev ** 2))
            quantiles_prices[:, i] = forecast_prices * np.exp(cum_dev)
        
        # Generate dates
        forecast_dates = pd.bdate_range(
            start=pd.Timestamp(stock_data.index[-1]) + timedelta(days=1),
            periods=horizon
        )
        
        return jsonify({
            "ticker": ticker,
            "current_price": float(current_price),
            "forecast": forecast_prices.tolist(),
            "quantiles": quantiles_prices.tolist(),
            "dates": [d.isoformat() for d in forecast_dates],
            "change_percent": float(((forecast_prices[-1] - current_price) / current_price) * 100)
        })
    
    except Exception as e:
        print(f"Error in forecast: {traceback.format_exc()}")
        return jsonify({"error": str(e)}), 500

@app.route("/api/forecast-batch", methods=["POST"])
def forecast_batch():
    """Forecast multiple stocks"""
    try:
        data = request.json
        tickers = data.get("tickers", [DEFAULT_TICKER])
        horizon = data.get("horizon", FORECAST_HORIZON)
        
        results = {}
        for ticker in tickers:
            try:
                with app.test_client() as client:
                    resp = client.post(
                        "/api/forecast",
                        json={"ticker": ticker, "horizon": horizon}
                    )
                    if resp.status_code == 200:
                        results[ticker] = resp.get_json()
                    else:
                        results[ticker] = {"error": "Failed to forecast"}
            except Exception as e:
                results[ticker] = {"error": str(e)}
        
        return jsonify({"forecasts": results})
    
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route("/api/current-price", methods=["GET"])
def current_price():
    """Get current price for a ticker"""
    try:
        ticker = request.args.get("ticker", DEFAULT_TICKER).upper()
        data = download(ticker, "1d")
        
        if data.empty:
            return jsonify({"error": f"No data for {ticker}"}), 404
        
        price = float(data['Close'].iloc[-1])
        return jsonify({"ticker": ticker, "price": price})
    
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route("/api/historical", methods=["GET"])
def historical():
    """Get historical data for a ticker"""
    try:
        ticker = request.args.get("ticker", DEFAULT_TICKER).upper()
        period = request.args.get("period", HISTORICAL_PERIOD)
        
        data = download(ticker, period)
        if data.empty:
            return jsonify({"error": f"No data for {ticker}"}), 404
        
        return jsonify({
            "ticker": ticker,
            "dates": [d.isoformat() for d in data.index],
            "prices": data['Close'].tolist(),
            "period": period
        })
    
    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == "__main__":
    print("\n🚀 Starting TimesFM Flask API Server...")
    print(f"   Host: {FLASK_HOST}")
    print(f"   Port: {FLASK_PORT}")
    print(f"   Debug: {FLASK_DEBUG}\n")
    
    if init_forecaster():
        app.run(host=FLASK_HOST, port=FLASK_PORT, debug=FLASK_DEBUG, threaded=True)
    else:
        print("❌ Failed to initialize forecaster. Exiting.")
