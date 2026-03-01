namespace RetroVaultWebApp.Config
{
    public class VaultOptions
    {
        public List<string> Systems { get; set; } = new();
        public List<string> Categories { get; set; } = new();

        public Dictionary<string, string> User { get; set; } = new();
        public string BaseServerUrl { get; set; } = string.Empty;
    }
}