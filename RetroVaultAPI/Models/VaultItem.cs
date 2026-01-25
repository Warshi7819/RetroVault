namespace RetroVaultAPI.Models
{
    public class VaultItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string System { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int Year { get; set; } = -1;
        public string PhysicalLocation { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string ImageFolder { get; set; } = string.Empty;
        public string VideoFolder { get; set; } = string.Empty;
        public string DocumentationFolder { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Currencty { get; set; } = string.Empty;
    }
}