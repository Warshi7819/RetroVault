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
            year = -1;
            vaultID = -1;
            physicalLocation = "";
            thumbnailImage = "";
            imageFolder = "";
            videoFolder = "";
        }

        public string Name { get; set; }
        public string System { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public int year { get; set; }
        public int vaultID { get; set; }
        public string physicalLocation { get; set; }
        public string thumbnailImage { get; set; }
        public string imageFolder { get; set; }
        public string videoFolder { get; set; }
    }
}
