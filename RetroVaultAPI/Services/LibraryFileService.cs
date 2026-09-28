using RetroVault.Shared.Models;

namespace RetroVaultAPI.Services
{
    public class LibraryFileService
    {
        public static readonly string[] Categories = VaultFile.Categories;

        private static readonly string[] InlineTypes =
        {
            ".pdf", ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".svg",
            ".mp3", ".wav", ".ogg", ".flac", ".m4a",
            ".mp4", ".webm", ".mkv", ".mov", ".avi"
        };

        private readonly string _root;

        public LibraryFileService()
        {
            _root = Path.Combine(Environment.CurrentDirectory, "Library");
            if (!Directory.Exists(_root))
            {
                Directory.CreateDirectory(_root);
            }
        }

        public string Root => _root;

        public static bool IsValidCategory(string? category)
        {
            return !string.IsNullOrWhiteSpace(category) &&
                   Categories.Contains(category, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsInlineType(string path)
        {
            var extension = Path.GetExtension(path);
            return InlineTypes.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        public static string GetCategoryName(string category)
        {
            return Categories.First(c => c.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        public string GetItemDirectory(int id)
        {
            return Path.Combine(_root, id.ToString());
        }

        public List<VaultFile> GetFiles(int id)
        {
            var files = new List<VaultFile>();
            var itemDir = GetItemDirectory(id);
            if (!Directory.Exists(itemDir))
            {
                return files;
            }

            foreach (var category in Categories)
            {
                var categoryDir = Path.Combine(itemDir, category);
                if (!Directory.Exists(categoryDir))
                {
                    continue;
                }

                foreach (var file in Directory.EnumerateFiles(categoryDir, "*", SearchOption.AllDirectories))
                {
                    var info = new FileInfo(file);
                    files.Add(new VaultFile
                    {
                        Category = category,
                        Path = Path.GetRelativePath(categoryDir, file).Replace(Path.DirectorySeparatorChar, '/'),
                        Name = info.Name,
                        SizeBytes = info.Length,
                        LastModifiedUtc = info.LastWriteTimeUtc
                    });
                }
            }

            return files
                .OrderBy(f => f.Category)
                .ThenBy(f => f.Path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public bool TryGetFilePath(int id, string category, string? relativePath, out string fullPath)
        {
            fullPath = string.Empty;

            if (!IsValidCategory(category) || string.IsNullOrWhiteSpace(relativePath))
            {
                return false;
            }

            var categoryDir = Path.GetFullPath(Path.Combine(GetItemDirectory(id), GetCategoryName(category)));
            var combined = Path.GetFullPath(Path.Combine(categoryDir, relativePath));

            var comparison = OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            if (!combined.StartsWith(categoryDir + Path.DirectorySeparatorChar, comparison))
            {
                return false;
            }

            fullPath = combined;
            return true;
        }

        public bool TryGetUploadPath(int id, string category, string? subfolder, string fileName, out string fullPath)
        {
            fullPath = string.Empty;

            if (!IsValidCategory(category))
            {
                return false;
            }

            var safeName = SanitizeName(fileName);
            if (safeName == null)
            {
                return false;
            }

            var targetDir = Path.Combine(GetItemDirectory(id), GetCategoryName(category));

            if (!TryResolveRelativePath(targetDir, subfolder, out targetDir))
            {
                return false;
            }

            fullPath = Path.Combine(targetDir, safeName);
            return true;
        }

        public List<VaultFolder> GetFolders(int id)
        {
            var folders = new List<VaultFolder>();
            var itemDir = GetItemDirectory(id);
            if (!Directory.Exists(itemDir))
            {
                return folders;
            }

            foreach (var category in Categories)
            {
                var categoryDir = Path.Combine(itemDir, category);
                if (!Directory.Exists(categoryDir))
                {
                    continue;
                }

                foreach (var dir in Directory.EnumerateDirectories(categoryDir, "*", SearchOption.AllDirectories))
                {
                    var relativePath = Path.GetRelativePath(categoryDir, dir).Replace(Path.DirectorySeparatorChar, '/');
                    folders.Add(new VaultFolder
                    {
                        Category = category,
                        Path = relativePath,
                        Name = Path.GetFileName(dir)
                    });
                }
            }

            return folders
                .OrderBy(f => f.Category)
                .ThenBy(f => f.Path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public bool FolderExists(int id, string category, string? subfolderPath)
        {
            if (!TryGetFolderDirectory(id, category, subfolderPath, out var directory))
            {
                return false;
            }

            return Directory.Exists(directory);
        }

        public bool CreateFolder(int id, string category, string? subfolderPath)
        {
            if (!TryGetFolderDirectory(id, category, subfolderPath, out var directory))
            {
                return false;
            }

            if (Directory.Exists(directory))
            {
                return false;
            }

            Directory.CreateDirectory(directory);
            return true;
        }

        public bool DeleteFolder(int id, string category, string? subfolderPath)
        {
            if (!TryGetFolderDirectory(id, category, subfolderPath, out var directory))
            {
                return false;
            }

            if (!Directory.Exists(directory))
            {
                return false;
            }

            Directory.Delete(directory, true);
            return true;
        }

        public bool TryGetFolderDirectory(int id, string category, string? subfolderPath, out string directory)
        {
            directory = string.Empty;

            if (!IsValidCategory(category) || string.IsNullOrWhiteSpace(subfolderPath))
            {
                return false;
            }

            var categoryDir = Path.Combine(GetItemDirectory(id), GetCategoryName(category));
            return TryResolveRelativePath(categoryDir, subfolderPath, out directory);
        }

        private static bool TryResolveRelativePath(string rootDir, string? relativePath, out string resolved)
        {
            resolved = rootDir;

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return true;
            }

            var target = rootDir;

            foreach (var segment in relativePath.Split('/', '\\'))
            {
                if (segment.Trim().Length == 0)
                {
                    continue;
                }

                var safeSegment = SanitizeName(segment.Trim());
                if (safeSegment == null)
                {
                    return false;
                }

                target = Path.Combine(target, safeSegment);
            }

            resolved = target;
            return true;
        }

        public async Task<VaultFile> SaveFileAsync(int id, string category, string? subfolder, string fileName, Stream content, long length)
        {
            TryGetUploadPath(id, category, subfolder, fileName, out var fullPath);

            var directory = Path.GetDirectoryName(fullPath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await content.CopyToAsync(stream);
            }

            var info = new FileInfo(fullPath);
            var categoryDir = Path.Combine(GetItemDirectory(id), GetCategoryName(category));

            return new VaultFile
            {
                Category = GetCategoryName(category),
                Path = Path.GetRelativePath(categoryDir, fullPath).Replace(Path.DirectorySeparatorChar, '/'),
                Name = info.Name,
                SizeBytes = info.Length,
                LastModifiedUtc = info.LastWriteTimeUtc
            };
        }

        public bool DeleteFile(int id, string category, string? relativePath)
        {
            if (!TryGetFilePath(id, category, relativePath, out var fullPath))
            {
                return false;
            }

            if (!File.Exists(fullPath))
            {
                return false;
            }

            File.Delete(fullPath);
            return true;
        }

        public void DeleteItemDirectory(int id)
        {
            var itemDir = GetItemDirectory(id);
            if (Directory.Exists(itemDir))
            {
                Directory.Delete(itemDir, true);
            }
        }

        private static string? SanitizeName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var fileName = Path.GetFileName(name.Trim());
            if (fileName.Length == 0 ||
                fileName == "." ||
                fileName == ".." ||
                fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return null;
            }

            return fileName;
        }
    }
}
