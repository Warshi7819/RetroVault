using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RetroVault.Shared;
using RetroVault.Shared.Models;
using RetroVaultWebApp.Services;

namespace RetroVaultWebApp.Pages.Settings;

[Authorize]
public class BackupModel : PageModel
{
    private readonly VaultApiClient _api;
    private readonly IWebHostEnvironment _env;
    private readonly ThumbnailService _thumbnails;
    private readonly LibraryProxyService _proxy;

    public BackupModel(VaultApiClient api, IWebHostEnvironment env, ThumbnailService thumbnails, LibraryProxyService proxy)
    {
        _api = api;
        _env = env;
        _thumbnails = thumbnails;
        _proxy = proxy;
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

        var libraryFileCount = 0;
        foreach (var item in allItems)
        {
            libraryFileCount += (await _api.GetFilesAsync(item.Id)).Count;
        }

        var backup = new
        {
            backup = new
            {
                timestamp = DateTime.UtcNow,
                itemCount = allItems.Count,
                thumbnailCount,
                libraryFileCount
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

        var thumbnails = allItems
            .Where(i => !string.IsNullOrEmpty(i.Thumbnail))
            .Select(i => i.Id)
            .ToList();

        return new JsonResult(thumbnails);
    }

    public async Task<IActionResult> OnGetThumbnailAsync(int id)
    {
        var filePath = await _thumbnails.EnsureThumbnailAsync(id, forceRefresh: true);

        if (filePath == "/images/no-thumb.png")
        {
            return NotFound();
        }

        var thumbnailsDir = Path.Combine(_env.WebRootPath, "images", "thumbnails");
        var localPath = Path.Combine(thumbnailsDir, $"{id}.png");

        return PhysicalFile(localPath, "image/png");
    }

    public async Task<IActionResult> OnGetFilesAsync()
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

        var files = new List<object>();

        foreach (var item in allItems)
        {
            foreach (var file in await _api.GetFilesAsync(item.Id))
            {
                files.Add(new
                {
                    itemId = item.Id,
                    category = file.Category,
                    path = file.Path,
                    name = file.Name,
                    sizeBytes = file.SizeBytes,
                    lastModifiedUtc = file.LastModifiedUtc
                });
            }
        }

        return new JsonResult(files);
    }

    public async Task<IActionResult> OnGetFileAsync(int id, string category, string path)
    {
        await _proxy.ProxyFileAsync(HttpContext, id, category, path);
        return new EmptyResult();
    }
}
