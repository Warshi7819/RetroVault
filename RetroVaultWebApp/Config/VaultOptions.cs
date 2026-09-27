namespace RetroVaultWebApp.Config
{
    public class VaultOptions
    {
        public string BaseServerUrl { get; set; } = string.Empty;
        public int PriceChartingUpdateSeconds { get; set; } = 15;
    }
}
