using RetroVaultAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace RetroVault
{
    public partial class NewEditItemForm : Form
    {
        internal VaultItem vaultItem;
        internal VaultSettingsConfig config;
        internal Boolean deleteItem = false;
        public NewEditItemForm(VaultItem item, VaultSettingsConfig conf)
        {
            InitializeComponent();
            vaultItem = item;
            config = conf;
            if (vaultItem == null)
            {
                this.Text = "Add New Item";
                this.deleteButton.Enabled = false;
                this.openMediaFolderButton.Enabled = false;
                this.setThumbnailButton.Enabled = false;
            }
            else
            {
                this.Text = "Edit Item";
            }

            // Load icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", "RetroVault.ico");
            this.Icon = new Icon(iconPath);

            // Load categories and systems
            foreach (string system in config.Systems)
            {
                systemComboBox.Items.Add(system);
            }
            foreach (string category in config.Categories)
            {
                categoryComboBox.Items.Add(category);
            }
            foreach (string currency in config.Currencies)
            {
                currencyComboBox.Items.Add(currency);
            }


            //populate fields if editing an existing item
            if (vaultItem != null)
            {
                nameTextBox.Text = vaultItem.Name;
                systemComboBox.SelectedItem = vaultItem.System;
                yearTextBox.Text = vaultItem.Year.ToString();
                categoryComboBox.SelectedItem = vaultItem.Category;
                priceTextBox.Text = vaultItem.PurchasePrice.ToString();
                currencyComboBox.SelectedItem = vaultItem.Currency;
                regionTextBox.Text = vaultItem.Region;
                acquiredFromTextBox.Text = vaultItem.AcquiredFrom;
                completeTextBox.Text = vaultItem.Completeness;
                acquiredDateTextBox.Text = vaultItem.AcquiredDate;
                descTextBox.Text = vaultItem.Description;
                publisherTextBox.Text = vaultItem.Publisher;
                devTextBox.Text = vaultItem.Developer;
                storageTextBox.Text = vaultItem.StorageLocation;

            }
        }

        public VaultItem getVaultItem()
        {
            return vaultItem;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Update the vaultItem with the form data
            if (vaultItem == null)
            {
                vaultItem = new VaultItem();
            }
            vaultItem.Name = nameTextBox.Text;
            vaultItem.System = systemComboBox.SelectedItem?.ToString() ?? "";
            vaultItem.Year = int.TryParse(yearTextBox.Text, out int year) ? year : 0;
            vaultItem.Category = categoryComboBox.SelectedItem?.ToString() ?? "";
            vaultItem.PurchasePrice = int.TryParse(priceTextBox.Text, out int price) ? price : 0;
            vaultItem.Currency = currencyComboBox.SelectedItem?.ToString() ?? "";
            vaultItem.Region = regionTextBox.Text;
            vaultItem.AcquiredFrom = acquiredFromTextBox.Text;
            vaultItem.Completeness = completeTextBox.Text;
            vaultItem.AcquiredDate = acquiredDateTextBox.Text;
            vaultItem.Description = descTextBox.Text;
            vaultItem.Publisher = publisherTextBox.Text;
            vaultItem.Developer = devTextBox.Text;
            vaultItem.StorageLocation = storageTextBox.Text;

            DialogResult = DialogResult.OK;
            this.Close();
        }

        public Boolean isDeleteRequested()
        {
            return deleteItem;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Display the message box
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this item?",
                "Delete Request",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );

            // Check the result and take action
            if (result == DialogResult.OK)
            {
                // User says OK to delete!
                deleteItem = true;
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (result == DialogResult.Cancel)
            {
                // User canceled the deletion
                MessageBox.Show("Operation Cancelled!", "Delete Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void setThumbnailButton_Click(object sender, EventArgs e)
        {
            // Open file dialog to select an image of type PNG, JPG, BMP, GIF, TIFF
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(config.MediaLibraryPath != null && config.MediaLibraryPath.Trim() != "")
            {
                openFileDialog.InitialDirectory = Path.Combine(config.MediaLibraryPath, vaultItem.Id.ToString());
            }   
            openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedImagePath = openFileDialog.FileName;
                string thumbnailsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thumbnails");
                if (!Directory.Exists(thumbnailsDir))
                {
                    Directory.CreateDirectory(thumbnailsDir);
                }
                string thumbnailPath = Path.Combine(thumbnailsDir, "item_" + vaultItem.Id.ToString() + ".png");
                // Create thumbnail
                createThumbnail(selectedImagePath, thumbnailPath, 300);

                MessageBox.Show("Thumbnail set successfully!", "Thumbnail", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // Create a thumbnail image with a target height while maintaining aspect ratio
        // input image can be of type PNG, JPG, BMP, GIF, TIFF as supported by Image.FromFile.
        // Saves the thumbnail as a PNG file
        public static void createThumbnail(string inputPath, string outputPath, int targetHeight = 300)
        {
            using (Image original = Image.FromFile(inputPath))
            {
                // Maintain aspect ratio
                double scale = (double)targetHeight / original.Height;
                int targetWidth = (int)(original.Width * scale);

                using (Bitmap resized = new Bitmap(targetWidth, targetHeight))
                {
                    resized.SetResolution(original.HorizontalResolution, original.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(resized))
                    {
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        g.DrawImage(original, 0, 0, targetWidth, targetHeight);


                    }
                    if (File.Exists(outputPath))
                    {
                        File.Delete(outputPath);
                    }

                    resized.Save(outputPath, ImageFormat.Png);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Based on configured path to media library
            // Create a folder structure like:
            // <MediaLibraryPath>\<ID>\Images\
            // <MediaLibraryPath>\<ID>\Videos\
            // <MediaLibraryPath>\<ID>\Documents\
            // <MediaLibraryPath>\<ID>\Audio\
            // <MediaLibraryPath>\<ID>\Software\
            string mediaLibraryPath = config.MediaLibraryPath;

            if(mediaLibraryPath == null || mediaLibraryPath.Trim() == "")
            {
                MessageBox.Show("Media Library Path is not configured. Please set it in the Settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(Directory.Exists(mediaLibraryPath) == false)
            {
                MessageBox.Show("Media Library Path does not exist. Please check the path in the Settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(Directory.Exists(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString())) == false)
            {
                // Create directories when item directory is initialized. 
                // If user deletes some of these later on then be it, they won't be automatically recreated unless
                // the entire item directory is missing.
                Directory.CreateDirectory(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString()));
                Directory.CreateDirectory(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString(), "Images"));
                Directory.CreateDirectory(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString(), "Videos"));
                Directory.CreateDirectory(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString(), "Documents"));
                Directory.CreateDirectory(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString(), "Audio"));
                Directory.CreateDirectory(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString(), "Software"));
            }

            OpenFolder(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString()));
        }

        

        // Try to keep it as non-windows as possible I guess...
        public static void OpenFolder(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", path);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", path);
            }
        }
    }
}
