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

        public ConfigForm(VaultSettingsConfig conf, Point parentLoc)
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
            checkBox1.Checked = config.AutoAIOnPaste;
            checkBox2.Checked = config.AutoOpenImgFolderOnSave;
            textBox1.Text = config.DefaultStorageRef;

            // Load icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", "RetroVault.ico");
            this.Icon = new Icon(iconPath);

            // Position the form relative to the parent form
            int offsetX = 20;
            int offsetY = 20;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                parentLoc.X + offsetX,
                parentLoc.Y + offsetY
            );
        }

        public VaultSettingsConfig getVaultSettingsConfig()
        {
            return config;
        }

        private void selectMediaLibraryButton_Click(object sender, EventArgs e)
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
            if (string.IsNullOrWhiteSpace(mediaPath))
            {
                MessageBox.Show("Please select a valid media library path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!System.IO.Directory.Exists(mediaPath))
            {
                MessageBox.Show("The selected media library path does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            config.MediaLibraryPath = label1.Text;
            config.DefaultStorageRef = textBox1.Text;
            if (checkBox1.Checked)
            {
                config.AutoAIOnPaste = true;
            }
            else
            {
                config.AutoAIOnPaste = false;
            }

            if (checkBox2.Checked)
            {
                config.AutoOpenImgFolderOnSave = true;
            }
            else
            {
                config.AutoOpenImgFolderOnSave = false;
            }


            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Fire up the backup dialog
            BackupForm backupForm = new BackupForm(config, this.Location); 
            backupForm.ShowDialog();
        }
    }
}
