namespace RetroVault.Shared.Models
{
    public class VaultFile
    {
        public static readonly string[] Categories = { "Audio", "Documents", "Images", "Software", "Videos" };

        public string Category { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public DateTime LastModifiedUtc { get; set; }
    }
}
