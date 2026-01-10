using System;
using System.Collections.Generic;
using System.Text;

namespace RetroVault
{
    public class VaultItem
    {
        public VaultItem()
        {
            Name = "";
            System = "";
            Category = "";
            Description = "";
            Developer = "";
            Publisher = "";
            Year = -1;
            VaultID = -1;
            PhysicalLocation = "";
            Thumbnail = "";
            ImageFolder = "";
            VideoFolder = "";
            DocumentationFolder = "";
            Price = "";
            Currencty = "";
        }

        public string Name { get; set; }
        public string System { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }
        public int VaultID { get; set; }
        public string PhysicalLocation { get; set; }
        public string Thumbnail { get; set; }
        public string ImageFolder { get; set; }
        public string VideoFolder { get; set; }
        public string DocumentationFolder { get; set; }
        public string Price { get; set; }   
        public string Currencty { get; set; }

    }
}
