"""Configuration for TimesFM Flask API"""
import os
from dotenv import load_dotenv

load_dotenv()

# Flask Configuration
FLASK_HOST = os.getenv("FLASK_HOST", "127.0.0.1")
FLASK_PORT = int(os.getenv("FLASK_PORT", "5000"))
FLASK_DEBUG = os.getenv("FLASK_DEBUG", "false").lower() == "true"

# Default stock ticker
DEFAULT_TICKER = "BWET"

# TimesFM Configuration
TIMESFM_CONFIG = {
    "checkpoint_path": "google/timesfm-3.0-pytorch",
    "per_core_batch_size": 32,
    "device": os.getenv("TIMESFM_DEVICE", "cuda"),
}

# Forecasting Parameters
FORECAST_HORIZON = int(os.getenv("FORECAST_HORIZON", "30"))
HISTORICAL_PERIOD = os.getenv("HISTORICAL_PERIOD", "2y")
MIN_DATA_POINTS = 60
