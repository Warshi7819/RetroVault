using System;
using System.Collections.Generic;
using System.Text;

namespace RetroVault
{
    public class VaultSettingsConfig
    {
        public List<string> Categories { get; set; }
        public List<string> Systems { get; set; }
        public string VaultPath { get; set; }
        public List<string> Currencies { get; set; }
        public string MediaLibraryPath { get; set; }
        public List<string> Regions { get; set; }
        public List<string> Complete { get; set; }
        public string RESTAPI { get; set; }
        public string ThumbnailURL { get; set; }
        public string GeminiModel { get; set; }

        public bool AutoAIOnPaste { get; set; }
        public bool AutoOpenImgFolderOnSave { get; set; }

        public string DefaultStorageRef { get; set; }
    }
}
