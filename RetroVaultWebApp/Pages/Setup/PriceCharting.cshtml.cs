using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using RetroVault.Shared;
using RetroVault.Shared.Models;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Services;

namespace RetroVaultWebApp.Pages.Setup
{
    [Authorize]
    public class PriceChartingModel : PageModel
    {
        private readonly VaultApiClient _api;
        private readonly PriceChartingUpdateService _updateService;
        private readonly int _updateSeconds;

        public PriceChartingModel(VaultApiClient api, PriceChartingUpdateService updateService,
            IOptions<VaultOptions> options)
        {
            _api = api;
            _updateService = updateService;
            _updateSeconds = options.Value.PriceChartingUpdateSeconds;
        }

        public int UpdateIntervalSeconds => _updateSeconds;
        public List<VaultItem> MissingItems { get; set; } = new();

        [BindProperty]
        public string TestUrl { get; set; } = "https://www.pricecharting.com/game/nes/super-mario-bros";

        public string? TestLoosePrice { get; set; }
        public string? TestCompletePrice { get; set; }
        public bool TestResultAvailable { get; set; }

        public async Task OnGetAsync()
        {
            await LoadMissingItemsAsync();
        }

        public async Task<IActionResult> OnPostTriggerUpdateAsync()
        {
            await _updateService.StartUpdateAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostTestUrlAsync()
        {
            await LoadMissingItemsAsync();

            if (string.IsNullOrWhiteSpace(TestUrl))
                return Page();

            try
            {
                var (loose, complete) = await _updateService.ScrapePriceChartingUrlAsync(TestUrl);
                TestLoosePrice = loose;
                TestCompletePrice = complete;
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

        private async Task LoadMissingItemsAsync()
        {
            var res = await _api.SearchVaultItemsAsync("", "", "", 1, 1000);
            MissingItems = res.Items
                .Where(i => string.IsNullOrWhiteSpace(i.PriceChartingURL))
                .ToList();
        }
    }
}
