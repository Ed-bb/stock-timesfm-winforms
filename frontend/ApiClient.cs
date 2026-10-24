using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BwetForecasterWinForms
{
    public class ApiClient
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public ApiClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<HistoryResult> GetHistoryAsync(string ticker, string period)
        {
            var url = $"{_baseUrl}/api/historical?ticker={Uri.EscapeDataString(ticker)}&period={Uri.EscapeDataString(period)}";
            var response = await _httpClient.GetAsync(url);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = JObject.Parse(responseJson)["error"]?.ToString();
                throw new Exception(error ?? "History request failed");
            }

            return JsonConvert.DeserializeObject<HistoryResult>(responseJson);
        }

        public async Task<ForecastResult> GetForecastAsync(string ticker, int horizon = 30)
        {
            try
            {
                var requestBody = new
                {
                    ticker = ticker.ToUpper(),
                    horizon = horizon,
                    period = "2y"
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/forecast", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var error = JObject.Parse(responseJson)["error"]?.ToString();
                    throw new Exception(error ?? "API request failed");
                }

                var result = JsonConvert.DeserializeObject<ForecastResult>(responseJson);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting forecast: {ex.Message}", ex);
            }
        }

        public async Task<Dictionary<string, ForecastResult>> GetForecastBatchAsync(List<string> tickers, int horizon = 30)
        {
            try
            {
                var requestBody = new
                {
                    tickers = tickers.ConvertAll(t => t.ToUpper()),
                    horizon = horizon
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/forecast-batch", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Batch forecast failed");

                var data = JObject.Parse(responseJson)["forecasts"];
                var results = new Dictionary<string, ForecastResult>();

                foreach (var item in data.Children<JProperty>())
                {
                    var forecast = JsonConvert.DeserializeObject<ForecastResult>(item.Value.ToString());
                    results[item.Name] = forecast;
                }

                return results;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting batch forecast: {ex.Message}", ex);
            }
        }
    }

    public class HistoryResult
    {
        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("dates")]
        public List<string> Dates { get; set; }

        [JsonProperty("prices")]
        public List<decimal> Prices { get; set; }
    }

    public class ForecastResult
    {
        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("current_price")]
        public decimal CurrentPrice { get; set; }

        [JsonProperty("forecast")]
        public List<decimal> Forecast { get; set; }

        [JsonProperty("quantiles")]
        public List<List<decimal>> Quantiles { get; set; }

        [JsonProperty("dates")]
        public List<string> Dates { get; set; }

        [JsonProperty("change_percent")]
        public decimal ChangePercent { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
