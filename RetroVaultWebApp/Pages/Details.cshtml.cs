using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroVaultAPI.Client;
using RetroVaultAPI.Models;

namespace RetroVaultWebApp.Pages
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly VaultApiClient _api;

        public DetailsModel(VaultApiClient api)
        {
            _api = api;
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

            return Page();
        }
    }
}