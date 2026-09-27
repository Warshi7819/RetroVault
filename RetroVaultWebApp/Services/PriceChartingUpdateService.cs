using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using RetroVault.Shared;
using RetroVault.Shared.Models;
using RetroVaultWebApp.Config;

namespace RetroVaultWebApp.Services
{
    public class PriceChartingUpdateService
    {
        public const string ApiClientName = "PriceChartingApi";

        private readonly IHttpClientFactory _httpFactory;
        private readonly IOptions<VaultOptions> _options;

        public bool IsRunning { get; private set; }
        public int TotalItems { get; private set; }
        public int ProcessedCount { get; private set; }
        public string CurrentItemName { get; private set; } = string.Empty;
        public string? StatusMessage { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public PriceChartingUpdateService(IHttpClientFactory httpFactory, IOptions<VaultOptions> options)
        {
            _httpFactory = httpFactory;
            _options = options;
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

        public async Task StartUpdateAsync()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            CompletedAt = null;
            ErrorMessage = null;
            StatusMessage = "Starting...";
            ProcessedCount = 0;
            TotalItems = 0;
            CurrentItemName = string.Empty;

            _ = Task.Run(async () =>
            {
                try
                {
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
                        (string.IsNullOrWhiteSpace(i.PriceChartingLastUpdated) ||
                         !DateTime.TryParse(i.PriceChartingLastUpdated, out var last) ||
                         last < cutoff))
                        .ToList();

                    TotalItems = toUpdate.Count;
                    StatusMessage = $"Found {TotalItems} items to update.";

                    foreach (var item in toUpdate)
                    {
                        CurrentItemName = $"{item.Name} ({item.System})";
                        StatusMessage = $"Processing {ProcessedCount + 1} of {TotalItems}...";

                        try
                        {
                            var (loose, complete) = await ScrapePriceChartingUrlAsync(item.PriceChartingURL);

                            if (loose != null && ParsePrice(loose) is int looseVal)
                                item.PriceChartingLoosePrice = looseVal;
                            if (complete != null && ParsePrice(complete) is int completeVal)
                                item.PriceChartingCompletePrice = completeVal;

                            item.PriceChartingLastUpdated = DateTime.UtcNow.ToString("yyyy-MM-dd");
                            await api.UpdateVaultItemAsync(item.Id, item);
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage = $"Error on '{item.Name}': {ex.Message}";
                        }

                        ProcessedCount++;

                        if (ProcessedCount < TotalItems)
                            await Task.Delay(TimeSpan.FromSeconds(_options.Value.PriceChartingUpdateSeconds));
                    }

                    StatusMessage = $"Completed {ProcessedCount} items.";
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
            });
        }

        private static int? ParsePrice(string price)
        {
            var cleaned = price.Replace("$", "").Replace(",", "").Trim();
            if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var val))
                return (int)Math.Round(val);
            return null;
        }
    }
}
