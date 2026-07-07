using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using RetroVault.Shared;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Reporting;
using RetroVaultWebApp.Services;

namespace RetroVaultWebApp.Pages
{
    public class PriceChartModel : PageModel
    {

        private readonly VaultApiClient _api;
        private readonly ThumbnailService _thumbs;
        public PriceChartModel(VaultApiClient api, IOptions<VaultOptions> options,
            ThumbnailService thumbs) 
        { 
            _api = api;
            _thumbs = thumbs;
        }

        public int TotalItems = 0;
        public int CountedItems = 0;
        public int NumCIBItems = 0;
        public int NumLooseItems = 0;
        public int NumItemsWithPriceData = 0;
        public int DiffCIB = 0;
        public int DiffLoose = 0;
        public Dictionary<string, int> CostData = new Dictionary<string, int>();
        public Dictionary<int, int> TopTenWinners = new Dictionary<int, int>();
        public Dictionary<int, int> TopTenLoosers = new Dictionary<int, int>();

        public async Task OnGetAsync()
        {
            // Get every retro item, 10 items per page request (which is the default).
            var res = await _api.SearchVaultItemsAsync("", "", "", 1);
            var totalPages = res.TotalPages;
            TotalItems = res.TotalCount;
            CountedItems = 0;
            NumCIBItems = 0;
            NumLooseItems = 0;
            NumItemsWithPriceData = 0;
            DiffCIB = 0;
            DiffLoose = 0;

            
            for (int pageNum = 1; pageNum <= totalPages; pageNum++)
            {
                if (pageNum > 1)
                {
                    // Page 1 is already fetched, but for subsequent pages we need
                    // to fetch the new items.
                    res = await _api.SearchVaultItemsAsync("", "", "", pageNum);
                }


                CostData = new Dictionary<string, int>();
                CostData["PriceChart: CIB"] = 0;
                CostData["My Collection: CIB"] = 0;
                CostData["PriceChart: Loose"] = 0;
                CostData["My Collection: Loose"] = 0;

                foreach (var item in res.Items)
                {
                    // Compare items but only if threre is price chart data
                    if(item.PriceChartingCompletePrice > 0 && item.PriceChartingLoosePrice > 0)
                    {
                        NumItemsWithPriceData += 1;

                        if (item.Completeness.Equals("CIB", StringComparison.OrdinalIgnoreCase))
                        {
                            NumCIBItems += 1;
                            CostData["PriceChart: CIB"] += item.PriceChartingCompletePrice;
                            CostData["My Collection: CIB"] += item.PurchasePrice;
                            AddPossibleWinnerLooser(item.Id, item.PriceChartingCompletePrice - item.PurchasePrice);
                        }
                        else
                        {
                            NumLooseItems += 1;
                            CostData["PriceChart: Loose"] += item.PriceChartingLoosePrice;
                            CostData["My Collection: Loose"] += item.PurchasePrice;
                            AddPossibleWinnerLooser(item.Id, item.PriceChartingLoosePrice - item.PurchasePrice);
                        }
                    }

                    CountedItems += 1;
                }

                DiffCIB += CostData["PriceChart: CIB"] - CostData["My Collection: CIB"];
                DiffLoose += CostData["PriceChart: Loose"] - CostData["My Collection: Loose"];

                // Sort the winners and losers dictionaries by value
                TopTenWinners = TopTenWinners.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                TopTenLoosers = TopTenLoosers.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);


                // Ensure that we prepare the thumbnails for the top ten winners and losers
                foreach (var item in TopTenLoosers)
                {
                    await _thumbs.EnsureThumbnailAsync(item.Key);
                }

                foreach(var item in TopTenWinners)
                {
                    await _thumbs.EnsureThumbnailAsync(item.Key);
                }
            }
        }

        private void AddPossibleWinnerLooser(int id, int diff)
        {
            TopTenWinners.Add(id, diff);
            TopTenLoosers.Add(id, diff);

            if (TopTenWinners.Count > 10)
            {
                var minDiff = TopTenWinners.OrderBy(x => x.Value).First();
                TopTenWinners.Remove(minDiff.Key);
            }

            if(TopTenLoosers.Count > 10)
            {
                var maxDiff = TopTenLoosers.OrderByDescending(x => x.Value).First();
                TopTenLoosers.Remove(maxDiff.Key);
            }

        }
    }
}

