namespace RetroVault.Shared.Models
{
    public class VaultItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string System { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int Year { get; set; } = -1;
        public string AcquiredDate { get; set; } = string.Empty;
        public string Completeness { get; set; } = string.Empty;
        public string AcquiredFrom { get; set; } = string.Empty;
        public string StorageLocation { get; set; } = string.Empty;
        public int PurchasePrice { get; set; } = 0;
        public string Currency { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
    }
}