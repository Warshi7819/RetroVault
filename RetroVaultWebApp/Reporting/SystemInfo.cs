namespace RetroVaultWebApp.Reporting
{
    public class SystemInfo
    {
        public int ItemCount { get; set; } = 0;
        public int TotalCost { get; set; } = 0;

        public int AveragePrice
        {
            get
            {
                if (ItemCount == 0) return 0;
                return TotalCost / ItemCount;
            }
        }
    }
}
