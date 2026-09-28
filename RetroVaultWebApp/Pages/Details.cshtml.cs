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

        public class Crumb
        {
            public string Name { get; set; } = string.Empty;
            public string? Folder { get; set; }
            public bool IsCurrent { get; set; }
        }

        public VaultItem? Item { get; set; }

        public List<VaultFile> AllFiles { get; set; } = new();

        public List<VaultFolder> Subfolders { get; set; } = new();

        public List<VaultFile> Files { get; set; } = new();

        public List<Crumb> Crumbs { get; set; } = new();

        public bool IsRoot => string.IsNullOrEmpty(CurrentFolder);

        public string? CurrentFolder { get; set; }

        public string? CurrentCategory { get; set; }

        public string? CurrentSubfolder { get; set; }

        [BindProperty]
        public List<IFormFile>? UploadFiles { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Folder { get; set; }

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

            if (!TryNormalizeFolder(Folder, out var normalized))
                return NotFound();

            CurrentFolder = normalized;
            await LoadFilesAsync(id);

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

        public async Task<IActionResult> OnPostUploadFilesAsync(int id)
        {
            var item = await _api.GetVaultItemAsync(id);
            if (item == null)
                return NotFound();

            if (!TryNormalizeFolder(Folder, out var normalized))
                return NotFound();

            if (UploadFiles == null || UploadFiles.Count == 0)
            {
                TempData["Error"] = "Choose at least one file to upload.";
                return RedirectToDetails(id, normalized);
            }

            SplitFolder(normalized, out var category, out var subfolder);

            if (category == null)
            {
                TempData["Error"] = "Open a folder before uploading files.";
                return RedirectToDetails(id, normalized);
            }

            var failed = new List<string>();

            foreach (var file in UploadFiles)
            {
                if (file == null || file.Length == 0)
                    continue;

                using var stream = file.OpenReadStream();
                var saved = await _api.UploadFileAsync(id, category, stream, file.FileName, subfolder);

                if (saved == null)
                    failed.Add(file.FileName);
            }

            if (failed.Count > 0)
                TempData["Error"] = "Upload failed for: " + string.Join(", ", failed);
            else
                TempData["Success"] = $"Uploaded {UploadFiles.Count} file(s).";

            return RedirectToDetails(id, normalized);
        }

        public async Task<IActionResult> OnPostDeleteFileAsync(int id, string fileCategory, string filePath)
        {
            var deleted = await _api.DeleteFileAsync(id, fileCategory, filePath);

            if (!deleted)
                TempData["Error"] = $"Could not delete '{filePath}'.";
            else
                TempData["Success"] = $"Deleted '{filePath}'.";

            return RedirectToDetails(id);
        }

        public async Task<IActionResult> OnPostCreateFolderAsync(int id, string folderName)
        {
            var item = await _api.GetVaultItemAsync(id);
            if (item == null)
                return NotFound();

            if (!TryNormalizeFolder(Folder, out var normalized))
                return NotFound();

            SplitFolder(normalized, out var category, out var subfolder);

            if (category == null)
            {
                TempData["Error"] = "Open a folder before creating subfolders.";
                return RedirectToDetails(id, normalized);
            }

            var name = folderName?.Trim() ?? string.Empty;

            if (!IsValidFolderName(name))
            {
                TempData["Error"] = "Invalid folder name.";
                return RedirectToDetails(id, normalized);
            }

            var path = string.IsNullOrEmpty(subfolder) ? name : $"{subfolder}/{name}";
            var created = await _api.CreateFolderAsync(id, category, path);

            if (created == null)
                TempData["Error"] = $"Could not create folder '{name}'. It may already exist.";
            else
                TempData["Success"] = $"Created folder '{name}'.";

            return RedirectToDetails(id, normalized);
        }

        public async Task<IActionResult> OnPostDeleteFolderAsync(int id, string folderPath)
        {
            var item = await _api.GetVaultItemAsync(id);
            if (item == null)
                return NotFound();

            if (!TryNormalizeFolder(Folder, out var normalized))
                return NotFound();

            SplitFolder(normalized, out var category, out _);

            if (category == null || string.IsNullOrWhiteSpace(folderPath))
                return RedirectToDetails(id, normalized);

            var deleted = await _api.DeleteFolderAsync(id, category, folderPath.Trim().Trim('/'));

            if (!deleted)
                TempData["Error"] = $"Could not delete folder '{folderPath}'.";
            else
                TempData["Success"] = $"Deleted folder '{folderPath}' and its contents.";

            return RedirectToDetails(id, normalized);
        }

        public string GetFileUrl(VaultFile file)
        {
            var escapedPath = string.Join("/", file.Path.Split('/', '\\').Select(Uri.EscapeDataString));
            return $"/Files/Download/{Item!.Id}/{Uri.EscapeDataString(file.Category)}/{escapedPath}";
        }

        public int FileCountUnder(string folderPath)
        {
            var prefix = folderPath + "/";
            return AllFiles.Count(f => (f.Category + "/" + f.Path).StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        public static string FormatCount(int count, string singular, string plural)
        {
            return count == 1 ? $"1 {singular}" : $"{count} {plural}";
        }

        public static string FormatSize(long bytes)
        {
            if (bytes >= 1024L * 1024 * 1024)
                return $"{bytes / (1024.0 * 1024 * 1024):0.#} GB";
            if (bytes >= 1024L * 1024)
                return $"{bytes / (1024.0 * 1024):0.#} MB";
            if (bytes >= 1024L)
                return $"{bytes / 1024.0:0.#} KB";
            return $"{bytes} B";
        }

        public static string CategoryIcon(string category) => category switch
        {
            "Audio" => "bi-music-note-beamed",
            "Documents" => "bi-file-earmark-text",
            "Images" => "bi-image",
            "Software" => "bi-box-seam",
            "Videos" => "bi-film",
            _ => "bi-file"
        };

        private async Task LoadFilesAsync(int id)
        {
            AllFiles = await _api.GetFilesAsync(id);
            var allFolders = await _api.GetFoldersAsync(id);

            if (IsRoot)
            {
                Files = AllFiles;
                return;
            }

            SplitFolder(CurrentFolder, out var category, out var subfolder);

            CurrentCategory = category;
            CurrentSubfolder = subfolder;

            Files = AllFiles
                .Where(f => ParentOf($"{f.Category}/{f.Path}").Equals(CurrentFolder, StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            Subfolders = allFolders
                .Where(f => ParentOf($"{f.Category}/{f.Path}").Equals(CurrentFolder, StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            Crumbs = new List<Crumb> { new Crumb { Name = "Files", Folder = null, IsCurrent = false } };

            var segments = CurrentFolder!.Split('/');
            for (var i = 0; i < segments.Length; i++)
            {
                Crumbs.Add(new Crumb
                {
                    Name = segments[i],
                    Folder = string.Join("/", segments.Take(i + 1)),
                    IsCurrent = i == segments.Length - 1
                });
            }
        }

        private static string ParentOf(string fullPath)
        {
            var index = fullPath.LastIndexOf('/');
            return index < 0 ? string.Empty : fullPath[..index];
        }

        private IActionResult RedirectToDetails(int id, string? folder = null)
        {
            return RedirectToPage("Details", new
            {
                id,
                folder,
                name = Name,
                system = System,
                category = Category,
                pageNumber = PageNumber,
                search = true
            });
        }

        private static bool TryNormalizeFolder(string? folder, out string? normalized)
        {
            normalized = null;

            if (string.IsNullOrWhiteSpace(folder))
                return true;

            var segments = folder.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
                return true;

            var category = VaultFile.Categories
                .FirstOrDefault(c => c.Equals(segments[0].Trim(), StringComparison.OrdinalIgnoreCase));

            if (category == null)
                return false;

            var parts = new List<string> { category };

            for (var i = 1; i < segments.Length; i++)
            {
                var segment = segments[i].Trim();
                if (!IsValidFolderName(segment))
                    return false;

                parts.Add(segment);
            }

            normalized = string.Join("/", parts);
            return true;
        }

        private static void SplitFolder(string? folder, out string? category, out string? subfolder)
        {
            category = null;
            subfolder = null;

            if (string.IsNullOrEmpty(folder))
                return;

            var index = folder.IndexOf('/');
            if (index < 0)
            {
                category = folder;
                return;
            }

            category = folder[..index];
            subfolder = folder[(index + 1)..];
        }

        private static bool IsValidFolderName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) &&
                   name != "." &&
                   name != ".." &&
                   name.IndexOfAny(new[] { '/', '\\' }) < 0 &&
                   name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }
    }
}
