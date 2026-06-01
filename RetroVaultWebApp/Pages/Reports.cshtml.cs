using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using RetroVault.Shared;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Services;
using System.Xml.Linq;

namespace RetroVaultWebApp.Pages
{
    public class ReportsModel : PageModel
    {
        private readonly VaultApiClient _api;
        
        public ReportsModel(VaultApiClient api, IOptions<VaultOptions> options,
                  ThumbnailService thumbs)
        {
            _api = api;
        }

        public int TotalItems = 0;
        public int CountedItems = 0;

        public async Task OnGetAsync()
        {
            // We process every item in the DB. This happens on the server but it's still
            // a pretty heavy operation. But works for several thousand items so good enough for now. 
            // What mad man/woman has more than a few thousand retro items in their vault?

            // Get set of systems
            var systems = await _api.GetSystemsAsync();

            // Get set of categories
            var categories = await _api.GetCategoriesAsync();

            // Get every retro item, 10 items per page request (which is the default).
            var res = await _api.SearchVaultItemsAsync("", "", "", 0);
            TotalItems = res.TotalCount;

            for (int pageNum = 1; pageNum <= res.TotalPages; pageNum++)
            {
                foreach (var item in res.Items)
                {
                    CountedItems++;
                }
                
                res = await _api.SearchVaultItemsAsync("", "", "", pageNum);
            }
        }
    }
}
