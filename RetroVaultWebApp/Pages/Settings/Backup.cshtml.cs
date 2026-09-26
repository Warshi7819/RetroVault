using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroVault.Shared;
using RetroVault.Shared.Models;

namespace RetroVaultWebApp.Pages.Settings;

[Authorize]
public class BackupModel : PageModel
{
    private readonly VaultApiClient _api;
    private readonly IWebHostEnvironment _env;

    public BackupModel(VaultApiClient api, IWebHostEnvironment env)
    {
        _api = api;
        _env = env;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetItemsAsync()
    {
        var allItems = new List<VaultItem>();
        int page = 1;
        const int pageSize = 100;

        while (true)
        {
            var result = await _api.SearchVaultItemsAsync(null, null, null, page, pageSize);
            allItems.AddRange(result.Items);

            if (page >= result.TotalPages || !result.Items.Any())
                break;

            page++;
        }

        var thumbnailCount = 0;
        var thumbnailsDir = Path.Combine(_env.WebRootPath, "images", "thumbnails");
        if (Directory.Exists(thumbnailsDir))
        {
            thumbnailCount = Directory.GetFiles(thumbnailsDir, "*.png").Length;
        }

        var backup = new
        {
            backup = new
            {
                timestamp = DateTime.UtcNow,
                itemCount = allItems.Count,
                thumbnailCount
            },
            items = allItems
        };

        var json = JsonSerializer.Serialize(backup, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Response.Headers["X-Item-Count"] = allItems.Count.ToString();

        return Content(json, "application/json");
    }

    public async Task<IActionResult> OnGetThumbnailsAsync()
    {
        var thumbnailsDir = Path.Combine(_env.WebRootPath, "images", "thumbnails");
        if (!Directory.Exists(thumbnailsDir))
        {
            return new JsonResult(Array.Empty<string>());
        }

        var files = Directory.GetFiles(thumbnailsDir, "*.png")
            .Select(f => Path.GetFileName(f))
            .Where(f => f != null)
            .ToList();

        return new JsonResult(files);
    }

    public async Task<IActionResult> OnGetThumbnailAsync(int id)
    {
        var thumbnailsDir = Path.Combine(_env.WebRootPath, "images", "thumbnails");
        var filePath = Path.Combine(thumbnailsDir, $"{id}.png");

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        return PhysicalFile(filePath, "image/png");
    }
}
