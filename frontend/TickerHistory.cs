using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BwetForecasterWinForms
{
    public class TickerHistory
    {
        private const int MaximumTickers = 15;
        private readonly string _filePath;

        public TickerHistory()
        {
            _filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BwetForecaster",
                "ticker-history.json");
        }

        public IReadOnlyList<string> GetRecentTickers()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return Array.Empty<string>();

                return (JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_filePath)) ?? new List<string>())
                    .Where(ticker => !string.IsNullOrWhiteSpace(ticker))
                    .Select(ticker => ticker.Trim().ToUpperInvariant())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(MaximumTickers)
                    .ToList();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        public void Add(string ticker)
        {
            ticker = (ticker ?? string.Empty).Trim().ToUpperInvariant();
            if (ticker.Length == 0)
                return;

            var tickers = GetRecentTickers()
                .Where(existing => !string.Equals(existing, ticker, StringComparison.OrdinalIgnoreCase))
                .ToList();
            tickers.Insert(0, ticker);
            Save(tickers.Take(MaximumTickers));
        }

        public void Clear()
        {
            Save(Array.Empty<string>());
        }

        private void Save(IEnumerable<string> tickers)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(tickers));
            }
            catch
            {
            }
        }
    }
}
