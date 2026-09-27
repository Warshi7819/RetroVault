using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using RetroVault.Shared;
using RetroVault.Shared.Models;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Data;
using RetroVaultWebApp.Services;

namespace RetroVaultWebApp.Pages.Setup
{
    [Authorize]
    public class PriceChartingModel : PageModel
    {
        private readonly VaultApiClient _api;
        private readonly PriceChartingUpdateService _updateService;
        private readonly ExchangeRateService _exchangeRate;
        private readonly RetroVaultWebDbContext _db;
        private readonly int _updateSeconds;

        public PriceChartingModel(VaultApiClient api, PriceChartingUpdateService updateService,
            ExchangeRateService exchangeRate, IOptions<VaultOptions> options, RetroVaultWebDbContext db)
        {
            _api = api;
            _updateService = updateService;
            _exchangeRate = exchangeRate;
            _updateSeconds = options.Value.PriceChartingUpdateSeconds;
            _db = db;
        }

        public int UpdateIntervalSeconds => _updateSeconds;
        public string PreferredCurrency { get; set; } = "NOK";
        public List<VaultItem> MissingItems { get; set; } = new();

        [BindProperty]
        public string TestUrl { get; set; } = "https://www.pricecharting.com/game/nes/super-mario-bros";

        public string? TestLoosePrice { get; set; }
        public string? TestCompletePrice { get; set; }
        public string? TestLoosePriceConverted { get; set; }
        public string? TestCompletePriceConverted { get; set; }
        public bool TestResultAvailable { get; set; }

        [BindProperty]
        public bool ForceUpdate { get; set; }

        public async Task OnGetAsync()
        {
            await LoadUserDataAsync();
            await LoadMissingItemsAsync();
        }

        public async Task<IActionResult> OnPostTriggerUpdateAsync()
        {
            await LoadUserDataAsync();
            _updateService.TriggerUpdate(PreferredCurrency, ForceUpdate);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostTestUrlAsync()
        {
            await LoadUserDataAsync();
            await LoadMissingItemsAsync();

            if (string.IsNullOrWhiteSpace(TestUrl))
                return Page();

            try
            {
                var (loose, complete) = await _updateService.ScrapePriceChartingUrlAsync(TestUrl);
                TestLoosePrice = loose;
                TestCompletePrice = complete;

                if (PreferredCurrency != "USD")
                {
                    var rate = await _exchangeRate.GetUsdRateAsync(PreferredCurrency);
                    if (loose != null)
                        TestLoosePriceConverted = ConvertPrice(loose, rate);
                    if (complete != null)
                        TestCompletePriceConverted = ConvertPrice(complete, rate);
                }

                TestResultAvailable = true;
            }
            catch (Exception ex)
            {
                TestLoosePrice = $"Error: {ex.Message}";
                TestCompletePrice = null;
                TestResultAvailable = true;
            }

            return Page();
        }

        private static string ConvertPrice(string usdPrice, decimal rate)
        {
            var cleaned = usdPrice.Replace("$", "").Replace(",", "").Trim();
            if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var val))
            {
                return $"${val:F2} → {(val * rate):N0}";
            }
            return usdPrice;
        }

        public IActionResult OnGetProgress()
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            return Content(System.Text.Json.JsonSerializer.Serialize(new
            {
                _updateService.IsRunning,
                _updateService.TotalItems,
                _updateService.ProcessedCount,
                _updateService.CurrentItemName,
                _updateService.StatusMessage,
                _updateService.ErrorMessage,
                CompletedAt = _updateService.CompletedAt?.ToString("yyyy-MM-dd HH:mm:ss")
            }, options), "application/json");
        }

        private async Task LoadUserDataAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _db.Users.FindAsync(userId);
            PreferredCurrency = user?.PreferredCurrency ?? "NOK";
        }

        private async Task LoadMissingItemsAsync()
        {
            var res = await _api.SearchVaultItemsAsync("", "", "", 1, 1000);
            MissingItems = res.Items
                .Where(i => string.IsNullOrWhiteSpace(i.PriceChartingURL))
                .ToList();
        }
    }
}
