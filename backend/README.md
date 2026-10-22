# TimesFM Flask Backend

## Installation

```bash
cd backend
python -m venv venv
venv\Scripts\activate
pip install -r requirements.txt
```

## Running

```bash
python forecast_api.py
```

Server runs on http://127.0.0.1:5000

## API Endpoints

- POST /api/forecast - Single stock forecast
- POST /api/forecast-batch - Multiple stocks
- GET /api/current-price?ticker=BWET - Current price
- GET /api/historical?ticker=BWET - Historical data
- GET /api/health - Health check
