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
    public class ReportsModel : PageModel
    {
        private readonly VaultApiClient _api;
        
        public ReportsModel(VaultApiClient api, IOptions<VaultOptions> options,
                  ThumbnailService thumbs)
        {
            _api = api;
        }

        public int TotalItems = 0;
        public int CountedItems = 0;
        public int TotalCost = 0;
        public Dictionary<string, CategoryInfo> CatInf = new Dictionary<string, CategoryInfo>();
        public Dictionary<string, SystemInfo> SysInf = new Dictionary<string, SystemInfo>();
        public IEnumerable<KeyValuePair<string, PublisherInfo>> Top10Publishers;
        public IEnumerable<KeyValuePair<string, DeveloperInfo>> Top10Developers;

        public async Task OnGetAsync()
        {
            // We process every item in the DB. This happens on the server but it's still
            // a pretty heavy operation. But works for several thousand items so good enough for now. 
            // What mad man/woman has more than a few thousand retro items in their vault?

            // Get set of systems
            // var systems = await _api.GetSystemsAsync();

            // Get set of categories
            // var categories = await _api.GetCategoriesAsync();

            // Get every retro item, 10 items per page request (which is the default).
            var res = await _api.SearchVaultItemsAsync("", "", "", 0);
            TotalItems = res.TotalCount;
            Dictionary<string, PublisherInfo> pubInf = new Dictionary<string, PublisherInfo>();
            Dictionary<string, DeveloperInfo> devInf = new Dictionary<string, DeveloperInfo>();

            for (int pageNum = 1; pageNum <= res.TotalPages; pageNum++)
            {
                foreach (var item in res.Items)
                {
                    if (!SysInf.ContainsKey(item.System))
                    {
                        SysInf[item.System] = new SystemInfo();
                    }
                    SysInf[item.System].TotalCost += item.PurchasePrice;
                    SysInf[item.System].ItemCount += 1;

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


                    CountedItems++;
                    TotalCost += item.PurchasePrice;
                }

                Top10Publishers = pubInf.OrderByDescending(x => x.Value.ItemCount).Take(10);
                Top10Developers = devInf.OrderByDescending(x => x.Value.ItemCount).Take(10);    

                // Get next page of items
                res = await _api.SearchVaultItemsAsync("", "", "", pageNum);
            }
        }
    }
}
