using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using RetroVault.Shared;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Reporting;
using RetroVaultWebApp.Services;
using System.Reflection;
using System.Xml.Linq;

namespace RetroVaultWebApp.Pages
{
    [Authorize]
    public class ReportsModel : PageModel
    {
        private readonly VaultApiClient _api;
        
        public ReportsModel(VaultApiClient api, IOptions<VaultOptions> options)
        {
            _api = api;
        }

        public int TotalItems = 0;
        public int CountedItems = 0;
        public Dictionary<string, int> TotalCostPerCurrency = new Dictionary<string, int>();
        public Dictionary<string, CategoryInfo> CatInf = new Dictionary<string, CategoryInfo>();
        public Dictionary<string, SystemInfo> SysInf = new Dictionary<string, SystemInfo>();
        public IEnumerable<KeyValuePair<string, PublisherInfo>> Top10Publishers;
        public IEnumerable<KeyValuePair<string, DeveloperInfo>> Top10Developers;

        public async Task OnGetAsync()
        {
            // We process every item in the DB. This happens on the server but it's still
            // a pretty heavy operation. But works for several thousand items so good enough for now. 
            // What mad man/woman has more than a few thousand retro items in their vault?

            // Get every retro item, 10 items per page request (which is the default).
            var res = await _api.SearchVaultItemsAsync("", "", "", 1);
            var totalPages = res.TotalPages;
            TotalItems = res.TotalCount;
            CountedItems = 0;

            Dictionary<string, PublisherInfo> pubInf = new Dictionary<string, PublisherInfo>();
            Dictionary<string, DeveloperInfo> devInf = new Dictionary<string, DeveloperInfo>();

            for (int pageNum = 1; pageNum <= totalPages; pageNum++)
            {
                if (pageNum > 1) 
                {
                    // Page 1 is already fetched, but for subsequent pages we need
                    // to fetch the new items.
                    res = await _api.SearchVaultItemsAsync("", "", "", pageNum);
                }


                foreach (var item in res.Items)
                {
                    if (!SysInf.ContainsKey(item.System))
                    {
                        SysInf[item.System] = new SystemInfo();
                    }
                    SysInf[item.System].TotalCost += item.PurchasePrice;
                    SysInf[item.System].ItemCount += 1;
                    if (item.Category.Equals("Games"))
                    {
                        SysInf[item.System].GameCount += 1;
                    }

                    if (!CatInf.ContainsKey(item.Category))
                    {
                        CatInf[item.Category] = new CategoryInfo();
                    }

                    CatInf[item.Category].TotalCost += item.PurchasePrice;
                    CatInf[item.Category].ItemCount += 1;

                    if(item.Publisher.Trim() != "")
                    {
                        if (!pubInf.ContainsKey(item.Publisher))
                        {
                            pubInf[item.Publisher] = new PublisherInfo();
                        }

                        pubInf[item.Publisher].ItemCount += 1;
                    }

                    if(item.Developer.Trim() != "")
                    {
                        if (!devInf.ContainsKey(item.Developer))
                        {
                            devInf[item.Developer] = new DeveloperInfo();
                        }
                        devInf[item.Developer].ItemCount += 1;
                    }

                    if (item.PurchasePrice > 0)
                    {
                        if (!string.IsNullOrEmpty(item.Currency))
                        {
                            if (!TotalCostPerCurrency.ContainsKey(item.Currency))
                            { 
                                TotalCostPerCurrency[item.Currency] = 0;
                            }
                            TotalCostPerCurrency[item.Currency] += item.PurchasePrice;
                        }
                    }

                    CountedItems += 1;
                }
            }

            Top10Publishers = pubInf.OrderByDescending(x => x.Value.ItemCount).Take(10);
            Top10Developers = devInf.OrderByDescending(x => x.Value.ItemCount).Take(10);
        }
    }
}
