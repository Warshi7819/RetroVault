using System.Text.Json;

namespace RetroVaultWebApp.Services
{
    public class ExchangeRateService
    {
        private readonly IHttpClientFactory _httpFactory;

        public ExchangeRateService(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        public async Task<decimal> GetUsdRateAsync(string targetCurrency)
        {
            if (targetCurrency.Equals("USD", StringComparison.OrdinalIgnoreCase))
                return 1m;

            var url = $"https://api.frankfurter.app/latest?from=USD&to={targetCurrency.ToUpperInvariant()}";
            using var http = _httpFactory.CreateClient();
            var response = await http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("rates", out var rates) &&
                rates.TryGetProperty(targetCurrency.ToUpperInvariant(), out var rate))
            {
                return rate.GetDecimal();
            }

            throw new InvalidOperationException($"Rate for {targetCurrency} not found in API response.");
        }
    }
}
