namespace RetroVaultWebApp.Services
{
    public class ThumbnailService
    {
        private readonly HttpClient _internalApi;
        private readonly IWebHostEnvironment _env;

        // Cache duration for thumbnails (1 hour)
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

        public ThumbnailService(HttpClient internalApi, IWebHostEnvironment env)
        {
            _internalApi = internalApi;
            _env = env;
        }

        public async Task<string> EnsureThumbnailAsync(int itemId)
        {
            string folder = Path.Combine(_env.WebRootPath, "images", "thumbnails");
            Directory.CreateDirectory(folder);

            string localPath = Path.Combine(folder, $"{itemId}.png");

            bool needsRefresh = true;

            if (File.Exists(localPath))
            {
                DateTime lastWrite = File.GetLastWriteTimeUtc(localPath);

                if (DateTime.UtcNow - lastWrite < _cacheDuration)
                {
                    needsRefresh = false;
                }
            }

            if (needsRefresh)
            {
                var response = await _internalApi.GetAsync($"thumbnails/{itemId}.png");

                if (!response.IsSuccessStatusCode)
                    return "/images/no-thumb.png";

                var bytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(localPath, bytes);

                // Explicitly update timestamp to be sure it reflects the current time
                File.SetLastWriteTimeUtc(localPath, DateTime.UtcNow);
            }

            return $"/images/thumbnails/{itemId}.png";
        }
    }
}
