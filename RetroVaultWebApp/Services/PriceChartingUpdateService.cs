using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using RetroVault.Shared;
using RetroVault.Shared.Models;
using RetroVaultWebApp.Config;
using System.Threading.Channels;

namespace RetroVaultWebApp.Services
{
    public class PriceChartingUpdateService : BackgroundService
    {
        public const string ApiClientName = "PriceChartingApi";

        private readonly IHttpClientFactory _httpFactory;
        private readonly IOptions<VaultOptions> _options;
        private readonly ExchangeRateService _exchangeRate;
        private readonly Channel<(string Currency, bool Force)> _triggerChannel = Channel.CreateUnbounded<(string, bool)>();

        public bool IsRunning { get; private set; }
        public int TotalItems { get; private set; }
        public int ProcessedCount { get; private set; }
        public string CurrentItemName { get; private set; } = string.Empty;
        public string? StatusMessage { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public PriceChartingUpdateService(IHttpClientFactory httpFactory, IOptions<VaultOptions> options,
            ExchangeRateService exchangeRate)
        {
            _httpFactory = httpFactory;
            _options = options;
            _exchangeRate = exchangeRate;
        }

        public void TriggerUpdate(string preferredCurrency, bool forceUpdate)
        {
            _triggerChannel.Writer.TryWrite((preferredCurrency, forceUpdate));
        }

        public async Task<(string? LoosePrice, string? CompletePrice)> ScrapePriceChartingUrlAsync(string url)
        {
            using var http = _httpFactory.CreateClient("PriceChartingScrape");
            var html = await http.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var priceTable = doc.DocumentNode.SelectSingleNode(
                "//table[@id='price_data' or contains(@class,'price-data') or contains(@class,'js-price-data')]");

            if (priceTable == null)
                return (null, null);

            var rows = priceTable.SelectNodes(".//tr");
            if (rows == null || rows.Count < 2)
                return (null, null);

            var valueRow = rows[1];
            var cells = valueRow.SelectNodes("./td");
            if (cells == null || cells.Count < 2)
                return (null, null);

            var looseSpan = cells[0].SelectSingleNode(".//span[contains(@class,'price')]");
            var completeSpan = cells[1].SelectSingleNode(".//span[contains(@class,'price')]");

            var loosePrice = looseSpan?.InnerText.Trim();
            var completePrice = completeSpan?.InnerText.Trim();

            return (loosePrice, completePrice);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var (preferredCurrency, forceUpdate) = await _triggerChannel.Reader.ReadAsync(stoppingToken);

                IsRunning = true;
                CompletedAt = null;
                ErrorMessage = null;
                StatusMessage = "Starting...";
                ProcessedCount = 0;
                TotalItems = 0;
                CurrentItemName = string.Empty;

                try
                {
                    StatusMessage = $"Fetching USD → {preferredCurrency} exchange rate...";
                    var usdRate = await _exchangeRate.GetUsdRateAsync(preferredCurrency);

                    using var apiHttp = _httpFactory.CreateClient(ApiClientName);
                    var api = new VaultApiClient(apiHttp);

                    var allItems = new List<VaultItem>();
                    var res = await api.SearchVaultItemsAsync("", "", "", 1, 100);
                    allItems.AddRange(res.Items);
                    for (int page = 2; page <= res.TotalPages; page++)
                    {
                        res = await api.SearchVaultItemsAsync("", "", "", page, 100);
                        allItems.AddRange(res.Items);
                    }

                    var cutoff = DateTime.UtcNow.AddDays(-7);
                    var toUpdate = allItems.Where(i =>
                        !string.IsNullOrWhiteSpace(i.PriceChartingURL) &&
                        (forceUpdate ||
                         string.IsNullOrWhiteSpace(i.PriceChartingLastUpdated) ||
                         !DateTime.TryParse(i.PriceChartingLastUpdated, out var last) ||
                         last < cutoff))
                        .ToList();

                    TotalItems = toUpdate.Count;
                    StatusMessage = $"Found {TotalItems} items to update. Rate: 1 USD = {usdRate} {preferredCurrency}";

                    foreach (var item in toUpdate)
                    {
                        stoppingToken.ThrowIfCancellationRequested();

                        CurrentItemName = $"{item.Name} ({item.System})";
                        StatusMessage = $"Processing {ProcessedCount + 1} of {TotalItems}...";

                        try
                        {
                            var (loose, complete) = await ScrapePriceChartingUrlAsync(item.PriceChartingURL);

                            if (loose != null && ParsePrice(loose) is decimal looseUsd)
                                item.PriceChartingLoosePrice = (int)Math.Round(looseUsd * usdRate);
                            if (complete != null && ParsePrice(complete) is decimal completeUsd)
                                item.PriceChartingCompletePrice = (int)Math.Round(completeUsd * usdRate);

                            item.Currency = preferredCurrency;
                            item.PriceChartingLastUpdated = DateTime.UtcNow.ToString("yyyy-MM-dd");
                            await api.UpdateVaultItemAsync(item.Id, item);
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage = $"Error on '{item.Name}': {ex.Message}";
                        }

                        ProcessedCount++;

                        if (ProcessedCount < TotalItems)
                            await Task.Delay(TimeSpan.FromSeconds(_options.Value.PriceChartingUpdateSeconds), stoppingToken);
                    }

                    StatusMessage = $"Completed {ProcessedCount} items.";
                    CompletedAt = DateTime.UtcNow;
                }
                catch (OperationCanceledException)
                {
                    StatusMessage = "Cancelled (app shutting down).";
                    CompletedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Fatal error: {ex.Message}";
                }
                finally
                {
                    IsRunning = false;
                }
            }
        }

        private static decimal? ParsePrice(string price)
        {
            var cleaned = price.Replace("$", "").Replace(",", "").Trim();
            if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var val))
                return val;
            return null;
        }
    }
}
