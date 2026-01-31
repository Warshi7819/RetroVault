using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RetroVault
{
    public partial class ConfigForm : Form
    {
        VaultSettingsConfig config;

        public ConfigForm(VaultSettingsConfig conf)
        {
            InitializeComponent();
            config = conf;

            if (string.IsNullOrWhiteSpace(config.MediaLibraryPath))
            {
                label1.Text = "No path selected...";
            }
            else
            {
                label1.Text = config.MediaLibraryPath;
            }
            // Load icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", "RetroVault.ico");
            this.Icon = new Icon(iconPath);
        }

        public VaultSettingsConfig getVaultSettingsConfig()
        {
            return config;
        }

        private void selectVaultbutton_Click(object sender, EventArgs e)
        {
            // Select folder to store data
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    label1.Text = fbd.SelectedPath;
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Update config
            var mediaPath = label1.Text;
            if(string.IsNullOrWhiteSpace(mediaPath))
            {
                MessageBox.Show("Please select a valid media library path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(!System.IO.Directory.Exists(mediaPath))
            {
                MessageBox.Show("The selected media library path does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            config.MediaLibraryPath = label1.Text;

            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
