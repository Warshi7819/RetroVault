using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroVault.Shared;
using RetroVault.Shared.Models;
using RetroVaultWebApp.Data;

namespace RetroVaultWebApp.Pages
{
    [Authorize]
    public class CreateEditModel : PageModel
    {
        private readonly VaultApiClient _api;
        private readonly RetroVaultWebDbContext _db;

        public CreateEditModel(VaultApiClient api, RetroVaultWebDbContext db)
        {
            _api = api;
            _db = db;
        }

        public bool IsEdit => ItemId.HasValue;
        public int? ItemId { get; set; }

        [BindProperty]
        public VaultItem Item { get; set; } = new();

        [BindProperty]
        public IFormFile? ThumbnailFile { get; set; }

        public string? ErrorMessage { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<string> Systems { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadDropdownListsAsync(null, null);

            if (id.HasValue)
            {
                var existing = await _api.GetVaultItemAsync(id.Value);
                if (existing == null)
                    return NotFound();

                Item = existing;
                ItemId = id;

                await LoadDropdownListsAsync(Item.System, Item.Category);
            }
            else
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var user = await _db.Users.FindAsync(userId);
                var preferredCurrency = user?.PreferredCurrency ?? "NOK";

                Item = new VaultItem
                {
                    Sold = "No",
                    Currency = preferredCurrency,
                    Year = 0,
                    PurchasePrice = 0,
                    SalePrice = 0,
                    PriceChartingLoosePrice = 0,
                    PriceChartingCompletePrice = 0
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Clear();
            ValidateItem();

            if (!ModelState.IsValid)
            {
                await LoadDropdownListsAsync(Item.System, Item.Category);
                return Page();
            }

            int savedId;
            bool isEdit = Item.Id > 0;

            if (isEdit)
            {
                var success = await _api.UpdateVaultItemAsync(Item.Id, Item);
                if (!success)
                {
                    ErrorMessage = "Failed to update item.";
                    return Page();
                }
                savedId = Item.Id;
            }
            else
            {
                VaultItem? created;
                try
                {
                    created = await _api.CreateVaultItemAsync(Item);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to create item: {ex.Message}";
                    return Page();
                }

                if (created == null)
                {
                    ErrorMessage = "Failed to create item.";
                    return Page();
                }
                savedId = created.Id;
            }

            if (ThumbnailFile != null && ThumbnailFile.Length > 0)
            {
                using var stream = ThumbnailFile.OpenReadStream();
                var thumbOk = await _api.UploadThumbnailAsync(savedId, stream, ThumbnailFile.FileName);
                if (!thumbOk)
                {
                    TempData["Error"] = "Item saved, but thumbnail upload failed.";
                }
            }

            return RedirectToPage("Details", new { id = savedId });
        }

        private void ValidateItem()
        {
            if (string.IsNullOrWhiteSpace(Item.Name))
                ModelState.AddModelError("Item.Name", "Name is required.");
            else if (Item.Name.Length > 200)
                ModelState.AddModelError("Item.Name", "Name must be 200 characters or fewer.");

            if (Item.Description == null)
                Item.Description = string.Empty;

            if (string.IsNullOrWhiteSpace(Item.Category))
                ModelState.AddModelError("Item.Category", "Category is required.");
            else if (Item.Category.Length > 100)
                ModelState.AddModelError("Item.Category", "Category must be 100 characters or fewer.");

            if (string.IsNullOrWhiteSpace(Item.System))
                ModelState.AddModelError("Item.System", "System is required.");
            else if (Item.System.Length > 100)
                ModelState.AddModelError("Item.System", "System must be 100 characters or fewer.");

            if (Item.Region == null)
                Item.Region = string.Empty;
            else if (Item.Region.Length > 100)
                ModelState.AddModelError("Item.Region", "Region must be 100 characters or fewer.");

            if (Item.Developer == null)
                Item.Developer = string.Empty;
            else if (Item.Developer.Length > 200)
                ModelState.AddModelError("Item.Developer", "Developer must be 200 characters or fewer.");

            if (Item.Publisher == null)
                Item.Publisher = string.Empty;
            else if (Item.Publisher.Length > 200)
                ModelState.AddModelError("Item.Publisher", "Publisher must be 200 characters or fewer.");

            if (Item.Year < 0 || Item.Year > 2099)
                ModelState.AddModelError("Item.Year", "Year must be between 0 and 2099.");

            if (Item.AcquiredDate == null)
                Item.AcquiredDate = string.Empty;
            else if (Item.AcquiredDate.Length > 50)
                ModelState.AddModelError("Item.AcquiredDate", "Acquired date must be 50 characters or fewer.");

            if (Item.AcquiredFrom == null)
                Item.AcquiredFrom = string.Empty;
            else if (Item.AcquiredFrom.Length > 200)
                ModelState.AddModelError("Item.AcquiredFrom", "Acquired from must be 200 characters or fewer.");

            if (Item.Completeness == null)
                Item.Completeness = string.Empty;
            else if (Item.Completeness.Length > 50)
                ModelState.AddModelError("Item.Completeness", "Completeness must be 50 characters or fewer.");

            if (Item.StorageLocation == null)
                Item.StorageLocation = string.Empty;
            else if (Item.StorageLocation.Length > 200)
                ModelState.AddModelError("Item.StorageLocation", "Storage location must be 200 characters or fewer.");

            if (Item.PurchasePrice < 0)
                ModelState.AddModelError("Item.PurchasePrice", "Purchase price must be 0 or greater.");

            if (string.IsNullOrWhiteSpace(Item.Currency))
                ModelState.AddModelError("Item.Currency", "Currency is required.");
            else if (Item.Currency.Length > 10)
                ModelState.AddModelError("Item.Currency", "Currency must be 10 characters or fewer.");

            if (string.IsNullOrWhiteSpace(Item.Sold))
                Item.Sold = "No";
            else if (Item.Sold != "Yes" && Item.Sold != "No")
                ModelState.AddModelError("Item.Sold", "Sold must be 'Yes' or 'No'.");

            if (Item.SalePrice < 0)
                ModelState.AddModelError("Item.SalePrice", "Sale price must be 0 or greater.");

            if (Item.PriceChartingURL == null)
                Item.PriceChartingURL = string.Empty;
            else if (Item.PriceChartingURL.Length > 500)
                ModelState.AddModelError("Item.PriceChartingURL", "PriceCharting URL must be 500 characters or fewer.");

            if (Item.PriceChartingLastUpdated == null)
                Item.PriceChartingLastUpdated = string.Empty;

            if (ThumbnailFile != null)
            {
                var allowedTypes = new[] { "image/png", "image/jpeg", "image/gif", "image/webp", "image/bmp" };
                if (!allowedTypes.Contains(ThumbnailFile.ContentType.ToLower()))
                {
                    ModelState.AddModelError("ThumbnailFile", "Thumbnail must be an image (PNG, JPEG, GIF, WebP, or BMP).");
                }
                if (ThumbnailFile.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("ThumbnailFile", "Thumbnail must be 10 MB or smaller.");
                }
            }
        }

        private async Task LoadDropdownListsAsync(string? currentSystem, string? currentCategory)
        {
            Categories = await _db.ListItems
                .Where(l => l.ListType == "Category")
                .OrderBy(l => l.SortOrder).ThenBy(l => l.Name)
                .Select(l => l.Name)
                .ToListAsync();

            Systems = await _db.ListItems
                .Where(l => l.ListType == "System")
                .OrderBy(l => l.SortOrder).ThenBy(l => l.Name)
                .Select(l => l.Name)
                .ToListAsync();

            if (!string.IsNullOrEmpty(currentCategory) && !Categories.Contains(currentCategory))
                Categories.Add(currentCategory);

            if (!string.IsNullOrEmpty(currentSystem) && !Systems.Contains(currentSystem))
                Systems.Add(currentSystem);
        }
    }
}
