using System;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace BwetForecasterWinForms
{
    public partial class MainForm : Form
    {
        private readonly ApiClient _apiClient;
        private readonly string _defaultTicker;
        private readonly int _horizon;
        private readonly System.Windows.Forms.Timer _refreshTimer;
        private ForecastResult _forecast;
        private HistoryResult _history;

        public MainForm()
        {
            InitializeComponent();

            var apiUrl = Environment.GetEnvironmentVariable("services__backend__http__0")
                ?? ConfigurationManager.AppSettings["ApiBaseUrl"]
                ?? "http://127.0.0.1:5000";
            _defaultTicker = (ConfigurationManager.AppSettings["DefaultTicker"] ?? "BWET").ToUpperInvariant();
            _horizon = int.TryParse(ConfigurationManager.AppSettings["ForecastHorizon"], out var h) ? h : 30;
            int refreshMs = int.TryParse(ConfigurationManager.AppSettings["RefreshInterval"], out var r) ? r : 300000;

            _apiClient = new ApiClient(apiUrl);
            tickerBox.Text = _defaultTicker;

            forecastButton.Click += async (s, e) => await RunForecastAsync(tickerBox.Text);
            rangeBox.SelectedIndexChanged += async (s, e) => await OnRangeChangedAsync();
            resetZoomButton.Click += (s, e) =>
            {
                plotView.Model?.ResetAllAxes();
                plotView.InvalidatePlot(false);
            };
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
                var forecastTask = _apiClient.GetForecastAsync(ticker, _horizon);
                var historyTask = _apiClient.GetHistoryAsync(ticker, SelectedPeriod());

                _forecast = await forecastTask;
                try
                {
                    _history = await historyTask;
                }
                catch
                {
                    _history = null;
                }

                tickerBox.Text = _forecast.Ticker ?? ticker;
                ShowForecast(_forecast);
                SetStatus(_history == null ? "Ready (price history unavailable)" : "Ready", Color.LimeGreen);
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

            ShowChart(f);
        }

        private void ShowChart(ForecastResult f)
        {
            var model = new PlotModel
            {
                Title = $"{f.Ticker}: price history and {f.Forecast.Count}-day forecast"
            };
            model.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.TopLeft,
                LegendPlacement = LegendPlacement.Inside
            });

            var anchorDate = _history != null && _history.Dates.Count > 0
                ? DateTime.Parse(_history.Dates[_history.Dates.Count - 1])
                : DateTime.Parse(f.Dates[0]).AddDays(-1);
            double anchorX = DateTimeAxis.ToDouble(anchorDate);

            if (_history != null && _history.Dates.Count > 0)
            {
                var history = new LineSeries
                {
                    Title = "Close",
                    Color = OxyColors.SteelBlue,
                    StrokeThickness = 2,
                    TrackerFormatString = "{0}\n{2:MMM dd, yyyy}\n{4:C2}"
                };
                for (int i = 0; i < _history.Dates.Count; i++)
                    history.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Parse(_history.Dates[i])), (double)_history.Prices[i]));
                model.Series.Add(history);
            }

            // Both forecast lines and the band start at the last close so they join the history line
            var band = new AreaSeries
            {
                Title = "10-90% range",
                Fill = OxyColor.FromAColor(55, OxyColors.DarkOrange),
                Color = OxyColor.FromAColor(140, OxyColors.DarkOrange),
                Color2 = OxyColor.FromAColor(140, OxyColors.DarkOrange),
                StrokeThickness = 1,
                TrackerFormatString = "{0}\n{2:MMM dd, yyyy}\n{4:C2}"
            };
            var median = new LineSeries
            {
                Title = "Forecast",
                Color = OxyColors.Crimson,
                StrokeThickness = 2.5,
                TrackerFormatString = "{0}\n{2:MMM dd, yyyy}\n{4:C2}"
            };

            band.Points.Add(new DataPoint(anchorX, (double)f.CurrentPrice));
            band.Points2.Add(new DataPoint(anchorX, (double)f.CurrentPrice));
            median.Points.Add(new DataPoint(anchorX, (double)f.CurrentPrice));

            for (int i = 0; i < f.Forecast.Count; i++)
            {
                double x = DateTimeAxis.ToDouble(DateTime.Parse(f.Dates[i]));
                median.Points.Add(new DataPoint(x, (double)f.Forecast[i]));
                band.Points2.Add(new DataPoint(x, (double)f.Quantiles[i][0]));
                band.Points.Add(new DataPoint(x, (double)f.Quantiles[i][8]));
            }

            model.Series.Add(band);
            model.Series.Add(median);

            model.Annotations.Add(new LineAnnotation
            {
                Type = LineAnnotationType.Vertical,
                X = anchorX,
                LineStyle = LineStyle.Dash,
                Color = OxyColors.Gray,
                Text = "Forecast start",
                TextOrientation = AnnotationTextOrientation.Horizontal
            });

            model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = SelectedPeriodDays() > 180 ? "MMM yy" : "MMM dd",
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray
            });
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Price",
                StringFormat = "C2",
                MinimumPadding = 0.05,
                MaximumPadding = 0.05,
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray
            });
            plotView.Model = model;
        }

        private static readonly System.Collections.Generic.Dictionary<string, (string Period, int Days)> Ranges =
            new System.Collections.Generic.Dictionary<string, (string, int)>
            {
                ["1M"] = ("1mo", 30),
                ["3M"] = ("3mo", 90),
                ["6M"] = ("6mo", 180),
                ["1Y"] = ("1y", 365),
                ["2Y"] = ("2y", 730)
            };

        private string SelectedPeriod() => Ranges[(string)rangeBox.SelectedItem].Period;

        private int SelectedPeriodDays() => Ranges[(string)rangeBox.SelectedItem].Days;

        private async Task OnRangeChangedAsync()
        {
            if (_forecast == null)
                return;

            try
            {
                _history = await _apiClient.GetHistoryAsync(_forecast.Ticker, SelectedPeriod());
                ShowChart(_forecast);
            }
            catch (Exception ex)
            {
                SetStatus("Error: " + ex.Message, Color.Red);
            }
        }

        private void SetStatus(string message, Color color)
        {
            statusLabel.Text = message;
            statusLabel.ForeColor = color;
        }
    }
}
