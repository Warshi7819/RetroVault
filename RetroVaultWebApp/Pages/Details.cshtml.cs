using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroVault.Shared;
using RetroVault.Shared.Models;
using RetroVaultWebApp.Services;

namespace RetroVaultWebApp.Pages
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly VaultApiClient _api;
        private readonly ThumbnailService _thumbs;

        public DetailsModel(VaultApiClient api, ThumbnailService thumbs)
        {
            _api = api;
            _thumbs = thumbs;
        }

        public VaultItem? Item { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? System { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool Search { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _api.GetVaultItemAsync(id);

            if (Item == null)
                return NotFound();

            await _thumbs.EnsureThumbnailAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _api.DeleteVaultItemAsync(id);
            return RedirectToPage("Index", new
            {
                name = Name,
                system = System,
                category = Category,
                pageNumber = PageNumber,
                search = true
            });
        }
    }
}