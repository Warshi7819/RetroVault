using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RetroVault
{
    public partial class BackupForm : Form
    {
        VaultSettingsConfig config;

        public BackupForm(VaultSettingsConfig conf, Point parentLoc)
        {
            InitializeComponent();
            config = conf;

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

            textBox1.ScrollBars = ScrollBars.Vertical;

            textBox1.Text += "Awaiting thy command";
        }

        private void backupButton_Click(object sender, EventArgs e)
        {
            // Disable backup button while we run. Remember to print progress!
            backupButton.Enabled = false;

            // Verify that folder to be backed up and the folder to back up to exists
            if(Directory.Exists(config.MediaLibraryPath) == false)
            {
                MessageBox.Show("The media library folder does not exist. Please check your settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                backupButton.Enabled = true;
                return;
            }

            if (Directory.Exists(textBox2.Text) == false)
            {
                MessageBox.Show("Please select backup output folder above.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                backupButton.Enabled = true;
                return;
            }


            // Backup media folder first
            textBox1.Text += Environment.NewLine + " * Starting backup of Media Folders";



            // Backup the database
            textBox1.Text += Environment.NewLine + " * Starting backup of DB and thumbnails";



            // El Finito
            textBox1.Text += Environment.NewLine + Environment.NewLine + "Backup completed successfully!";
            backupButton.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = dlg.SelectedPath;
            }

        }
    }
}
