using System;
using System.Collections.Generic;
using System.Text;

namespace RetroVault
{
    public class VaultSettingsConfig
    {
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Systems { get; set; } = new List<string>();
        public string VaultPath { get; set; } = string.Empty;
        public List<string> Currencies { get; set; } = new List<string>();
        public string MediaLibraryPath { get; set; } = string.Empty;
        public List<string> Regions { get; set; } = new List<string>();
        public List<string> Complete { get; set; } = new List<string>();
        public string RESTAPI { get; set; } = string.Empty;
        public string ThumbnailURL { get; set; } = string.Empty;
        public string GeminiModel { get; set; } = string.Empty;

        public bool AutoAIOnPaste { get; set; }
        public bool AutoOpenImgFolderOnSave { get; set; }

        public string DefaultStorageRef { get; set; } = string.Empty;
    }
}
