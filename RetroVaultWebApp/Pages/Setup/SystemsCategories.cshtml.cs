using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroVaultWebApp.Data;
using RetroVaultWebApp.Models;

namespace RetroVaultWebApp.Pages.Setup
{
    [Authorize]
    public class SystemsCategoriesModel : PageModel
    {
        private readonly RetroVaultWebDbContext _db;

        public SystemsCategoriesModel(RetroVaultWebDbContext db)
        {
            _db = db;
        }

        public List<ListItem> Categories { get; set; } = new();
        public List<ListItem> Systems { get; set; } = new();

        [BindProperty]
        public string NewCategoryName { get; set; } = string.Empty;

        [BindProperty]
        public string NewSystemName { get; set; } = string.Empty;

        [BindProperty]
        public int RenameItemId { get; set; }

        [BindProperty]
        public string RenameItemName { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await LoadListsAsync();
        }

        public async Task<IActionResult> OnPostAddCategoryAsync()
        {
            if (!string.IsNullOrWhiteSpace(NewCategoryName))
            {
                _db.ListItems.Add(new ListItem
                {
                    ListType = "Category",
                    Name = NewCategoryName.Trim(),
                    SortOrder = await _db.ListItems.CountAsync(l => l.ListType == "Category")
                });
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddSystemAsync()
        {
            if (!string.IsNullOrWhiteSpace(NewSystemName))
            {
                _db.ListItems.Add(new ListItem
                {
                    ListType = "System",
                    Name = NewSystemName.Trim(),
                    SortOrder = await _db.ListItems.CountAsync(l => l.ListType == "System")
                });
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRenameAsync()
        {
            var item = await _db.ListItems.FindAsync(RenameItemId);
            if (item != null && !string.IsNullOrWhiteSpace(RenameItemName))
            {
                item.Name = RenameItemName.Trim();
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await _db.ListItems.FindAsync(id);
            if (item != null)
            {
                _db.ListItems.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        private async Task LoadListsAsync()
        {
            Categories = await _db.ListItems
                .Where(l => l.ListType == "Category")
                .OrderBy(l => l.SortOrder).ThenBy(l => l.Name)
                .ToListAsync();

            Systems = await _db.ListItems
                .Where(l => l.ListType == "System")
                .OrderBy(l => l.SortOrder).ThenBy(l => l.Name)
                .ToListAsync();
        }
    }
}
