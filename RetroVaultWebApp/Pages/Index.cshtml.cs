using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using RetroVault.Shared;
using RetroVault.Shared.Models;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Services;

namespace RetroVaultWebApp.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly VaultApiClient _api;
        private readonly VaultOptions _options;
        private readonly ThumbnailService _thumbs;

        public IndexModel(VaultApiClient api, IOptions<VaultOptions> options, 
                          ThumbnailService thumbs) 
        { 
            _api = api; 
            _options = options.Value; 
            _thumbs = thumbs; 
        }

        // Search inputs
        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? System { get; set; } = "All";

        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; } = "All";

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)] 
        public bool Search { get; set; }

        public List<string> Systems => _options.Systems; 
        public List<string> Categories => _options.Categories;

        public PagedResult<VaultItem>? Results { get; set; }

        public async Task OnGetAsync()
        {
            if (!Search)
            {
                // No search criteria and not returning from details page, so just show empty results
                return;
            }

            Results = await _api.SearchVaultItemsAsync(Name, System, Category, PageNumber);

            foreach (var item in Results.Items)
            {
                await _thumbs.EnsureThumbnailAsync(item.Id);
            }
        }
    }
}