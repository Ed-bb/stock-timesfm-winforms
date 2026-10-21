using System;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace BwetForecasterWinForms
{
    public partial class MainForm : Form
    {
        private readonly ApiClient _apiClient;
        private readonly string _defaultTicker;
        private readonly int _horizon;
        private readonly System.Windows.Forms.Timer _refreshTimer;

        public MainForm()
        {
            InitializeComponent();

            var apiUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://127.0.0.1:5000";
            _defaultTicker = (ConfigurationManager.AppSettings["DefaultTicker"] ?? "BWET").ToUpperInvariant();
            _horizon = int.TryParse(ConfigurationManager.AppSettings["ForecastHorizon"], out var h) ? h : 30;
            int refreshMs = int.TryParse(ConfigurationManager.AppSettings["RefreshInterval"], out var r) ? r : 300000;

            _apiClient = new ApiClient(apiUrl);
            tickerBox.Text = _defaultTicker;

            forecastButton.Click += async (s, e) => await RunForecastAsync(tickerBox.Text);
            Shown += async (s, e) => await RunForecastAsync(_defaultTicker);

            _refreshTimer = new System.Windows.Forms.Timer { Interval = refreshMs };
            _refreshTimer.Tick += async (s, e) => await RunForecastAsync(tickerBox.Text);
            _refreshTimer.Start();

            FormClosed += (s, e) => _refreshTimer.Dispose();
        }

        private async Task RunForecastAsync(string ticker)
        {
            ticker = (ticker ?? string.Empty).Trim().ToUpperInvariant();
            if (ticker.Length == 0)
            {
                SetStatus("Enter a ticker symbol", Color.Orange);
                return;
            }

            forecastButton.Enabled = false;
            SetStatus($"Loading {ticker}...", Color.Gold);

            try
            {
                var result = await _apiClient.GetForecastAsync(ticker, _horizon);
                tickerBox.Text = result.Ticker ?? ticker;
                ShowForecast(result);
                SetStatus("Ready", Color.LimeGreen);
            }
            catch (Exception ex)
            {
                SetStatus("Error: " + ex.Message, Color.Red);
            }
            finally
            {
                forecastButton.Enabled = true;
            }
        }

        private void ShowForecast(ForecastResult f)
        {
            var last = f.Forecast[f.Forecast.Count - 1];
            var lastQ = f.Quantiles[f.Quantiles.Count - 1];

            priceLabel.Text = $"Current Price\n{f.CurrentPrice.ToString("C2")}";
            forecastLabel.Text = $"Forecast (final day)\n{last.ToString("C2")}";
            changeLabel.Text = $"Expected Change\n{f.ChangePercent:+0.00;-0.00}%";
            ciLabel.Text = $"80% Interval (final day)\n{lastQ[0].ToString("C2")} - {lastQ[8].ToString("C2")}";

            detailsGrid.Rows.Clear();
            for (int i = 0; i < f.Forecast.Count; i++)
            {
                var date = DateTime.Parse(f.Dates[i]);
                detailsGrid.Rows.Add(
                    i + 1,
                    date.ToString("yyyy-MM-dd"),
                    f.Forecast[i].ToString("C2"),
                    f.Quantiles[i][0].ToString("C2"),
                    f.Quantiles[i][8].ToString("C2"));
            }

            var model = new PlotModel { Title = $"{f.Ticker} Price Forecast ({f.Forecast.Count} days)" };
            var median = new LineSeries { Title = "Forecast", Color = OxyColors.Red, StrokeThickness = 2 };
            var low = new LineSeries { Title = "10th %ile", Color = OxyColors.Orange, LineStyle = LineStyle.Dash };
            var high = new LineSeries { Title = "90th %ile", Color = OxyColors.Orange, LineStyle = LineStyle.Dash };

            for (int i = 0; i < f.Forecast.Count; i++)
            {
                double x = DateTimeAxis.ToDouble(DateTime.Parse(f.Dates[i]));
                median.Points.Add(new DataPoint(x, (double)f.Forecast[i]));
                low.Points.Add(new DataPoint(x, (double)f.Quantiles[i][0]));
                high.Points.Add(new DataPoint(x, (double)f.Quantiles[i][8]));
            }

            model.Series.Add(low);
            model.Series.Add(high);
            model.Series.Add(median);
            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "MM-dd" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Price ($)" });
            plotView.Model = model;
        }

        private void SetStatus(string message, Color color)
        {
            statusLabel.Text = message;
            statusLabel.ForeColor = color;
        }
    }
}
