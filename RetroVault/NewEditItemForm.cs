using Google.GenAI;
using Google.GenAI.Types;
using RetroVault.Shared.Models;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RetroVault
{
    public partial class NewEditItemForm : Form
    {
        internal VaultItem vaultItem;
        internal VaultSettingsConfig config;
        internal Boolean deleteItem = false;
        internal Boolean thumbnailUpdated = false;

        public string GeminiAPIEnvKey = "GOOGLE_API_KEY";
        Client client;
        internal string? imagePath = null;
        internal string? apiKey = null;

        string geminiModel = "";
        string geminiPrompt = "";


        public NewEditItemForm(VaultItem item, VaultSettingsConfig conf, Point parentLoc)
        {
            InitializeComponent();
            vaultItem = item;
            config = conf;

            // Init AI Search
            this.apiKey = System.Environment.GetEnvironmentVariable(GeminiAPIEnvKey);
            if (apiKey != null)
            {
                client = new Client(apiKey: apiKey);
                this.geminiModel = config.GeminiModel;
                this.geminiPrompt = $"""
                I've sent you a picture. Based on your findings looking at it I need you to reply with structured data that I can programmatically parse. That means data on a strict key:value format. 
                If you can't find the info from the picture alone please use your wealth of knowledge to try to fill in the blanks.

                An example reply I can handle follows:
                name:Donkey Kong
                system:Famicom
                year:1982
                publisher:Nintendo
                developer:Nintendo

                Given the image I want you to give me the following info in the format described above:
                * Name
                * System
                * Year
                * Publisher
                * Developer
                """;
            }


            this.Text = "Edit Item";

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



            // Load categories and systems
            foreach (string system in config.Systems)
            {
                systemComboBox.Items.Add(system);
            }
            foreach (string category in config.Categories)
            {
                categoryComboBox.Items.Add(category);
                if (category == "Games")
                {
                    // As the majority of items will likely be games,
                    // set it as the default to reduce number of clicks
                    categoryComboBox.SelectedItem = category;
                }
            }
            foreach (string currency in config.Currencies)
            {
                currencyComboBox.Items.Add(currency);
            }
            // Set default currency to first one, again to reduce the number of clicks
            // Users should specify the most used currency first in the config
            currencyComboBox.SelectedIndex = 0;
            saleCurrencyLabel.Text = currencyComboBox.SelectedItem.ToString();

            // Set default storage location if configured, otherwise it will be blank.
            // The less input per item, the better!
            this.storageTextBox.Text = config.DefaultStorageRef;

            //populate fields if editing an existing item
            if (vaultItem != null)
            {
                nameTextBox.Text = vaultItem.Name;
                systemComboBox.SelectedItem = vaultItem.System;
                yearTextBox.Text = vaultItem.Year.ToString();
                categoryComboBox.SelectedItem = vaultItem.Category;
                priceTextBox.Text = vaultItem.PurchasePrice.ToString();
                currencyComboBox.SelectedItem = vaultItem.Currency;
                saleCurrencyLabel.Text = vaultItem.Currency.ToString();
                regionTextBox.Text = vaultItem.Region;
                acquiredFromTextBox.Text = vaultItem.AcquiredFrom;
                completeTextBox.Text = vaultItem.Completeness;
                acquiredDateTextBox.Text = vaultItem.AcquiredDate;
                descTextBox.Text = vaultItem.Description;
                publisherTextBox.Text = vaultItem.Publisher;
                devTextBox.Text = vaultItem.Developer;
                storageTextBox.Text = vaultItem.StorageLocation;

                if (vaultItem.Sold != null && vaultItem.Sold.ToLower() == "yes")
                {
                    soldCheckBox.Checked = true;
                    salePriceTextBox.Text = vaultItem.SalePrice.ToString();
                }
                else 
                {
                    salePriceTextBox.Text = "0";
                }

            }


            // Create media folders on new item creation. 
            createMediaFolders();
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
            if (soldCheckBox.Checked)
            {   
                vaultItem.Sold = "Yes";
                vaultItem.SalePrice = int.TryParse(salePriceTextBox.Text, out int salePrice) ? salePrice : 0;
            }
            else
            {
                vaultItem.Sold = "No";
                vaultItem.SalePrice = 0;
            }

            // Check if we should open the image folder before closing the dialog, based on user settings.
            if (config.AutoOpenImgFolderOnSave)
            {
                OpenFolder(Path.Combine(config.MediaLibraryPath, vaultItem.Id.ToString(), "Images"));
            }

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
            if (config.MediaLibraryPath != null && config.MediaLibraryPath.Trim() != "")
            {
                openFileDialog.InitialDirectory = Path.Combine(config.MediaLibraryPath, vaultItem.Id.ToString());
            }
            openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedImagePath = openFileDialog.FileName;


                // Create thumbnail
                createThumbnail(selectedImagePath, 300);

                // Set update flag
                thumbnailUpdated = true;
            }
        }


        // Create a thumbnail image with a target height while maintaining aspect ratio
        // input image can be of type PNG, JPG, BMP, GIF, TIFF as supported by Image.FromFile.
        // Saves the thumbnail as a PNG file
        internal void createThumbnail(string inputPath, int targetHeight = 300)
        {

            string thumbnailsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thumbnails");
            if (!Directory.Exists(thumbnailsDir))
            {
                Directory.CreateDirectory(thumbnailsDir);
            }
            string thumbnailPath = Path.Combine(thumbnailsDir, "item_" + vaultItem.Id.ToString() + ".png");

            using (System.Drawing.Image original = System.Drawing.Image.FromFile(inputPath))
            {
                // Set input image as the one to use by Gemini for analysis
                this.imagePath = inputPath;
                this.button1.Enabled = true;


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
                    if (System.IO.File.Exists(thumbnailPath))
                    {
                        System.IO.File.Delete(thumbnailPath);
                    }

                    resized.Save(thumbnailPath, ImageFormat.Png);
                }
            }
        }

        private void createMediaFolders()
        {
            // Based on configured path to media library
            // Create a folder structure like:
            // <MediaLibraryPath>\<ID>\Images\
            // <MediaLibraryPath>\<ID>\Videos\
            // <MediaLibraryPath>\<ID>\Documents\
            // <MediaLibraryPath>\<ID>\Audio\
            // <MediaLibraryPath>\<ID>\Software\
            string mediaLibraryPath = config.MediaLibraryPath;

            if (mediaLibraryPath == null || mediaLibraryPath.Trim() == "")
            {
                return;
            }
            if (Directory.Exists(mediaLibraryPath) == false)
            {
                return;
            }

            if (Directory.Exists(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString())) == false)
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

        }


        private void button1_Click(object sender, EventArgs e)
        {
            string mediaLibraryPath = config.MediaLibraryPath;
            if (mediaLibraryPath == null || mediaLibraryPath.Trim() == "")
            {
                MessageBox.Show("Media Library Path is not configured. Please set it in the Settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Directory.Exists(mediaLibraryPath) == false)
            {
                MessageBox.Show("Media Library Path does not exist. Please check the path in the Settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Directory.Exists(Path.Combine(mediaLibraryPath, vaultItem.Id.ToString())) == false)
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

            // Open the folder!
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

        internal bool isThumbnailUpdated()
        {
            return thumbnailUpdated;
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            // Get information from Google Gemini - using api key in your environment variable: GOOGLE_API_KEY
            if (this.apiKey == null)
            {
                MessageBox.Show($"API key not found in environment variable {GeminiAPIEnvKey}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await HandleAISearch();

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                // If clipboard has an image and Auto AI Search is enabled
                if (Clipboard.ContainsImage() && config.AutoAIOnPaste)
                {
                    // Save clipboard image 
                    string frontImagePath = Path.Combine(config.MediaLibraryPath, vaultItem.Id.ToString(), "Images", "Front.png");
                    System.Drawing.Image img = Clipboard.GetImage();
                    img.Save(frontImagePath, System.Drawing.Imaging.ImageFormat.Png);

                    // Create thumbnail
                    createThumbnail(frontImagePath, 300);

                    // Set update flag on thumbnail to ensure it is saved to server
                    thumbnailUpdated = true;

                    // Fire off AI Search
                    _ = HandleAISearch();
                    return true; // swallow the key/ "mark" as handled
                }

                // Otherwise just pass the ctrl-v onwards to allow other controls to intercept it as normal
                return false;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }



        private async Task HandleAISearch()
        {
            try
            {
                string output = await SendTextAndImageAsync(this.geminiModel, this.geminiPrompt, this.imagePath);

                string[] lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { ':' }, 2);

                    if (parts.Length == 2)
                    {
                        string key = parts[0];
                        string value = parts[1];

                        if (key.Trim().Equals("name", StringComparison.OrdinalIgnoreCase))
                        {
                            nameTextBox.Text = value.Trim();
                        }
                        else if (key.Trim().Equals("system", StringComparison.OrdinalIgnoreCase))
                        {
                            int index = systemComboBox.FindString(value.Trim());
                            if (index != -1)
                            {
                                systemComboBox.SelectedIndex = index;
                            }
                        }
                        else if (key.Trim().Equals("year", StringComparison.OrdinalIgnoreCase))
                        {
                            yearTextBox.Text = value.Trim();
                        }
                        else if (key.Trim().Equals("publisher", StringComparison.OrdinalIgnoreCase))
                        {
                            publisherTextBox.Text = value.Trim();
                        }
                        else if (key.Trim().Equals("developer", StringComparison.OrdinalIgnoreCase))
                        {
                            devTextBox.Text = value.Trim();
                        }
                    }
                }

                // Show result in dialog
                // MessageBox.Show(output, "Gemini Response Parsed Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Show full error in dialog
                MessageBox.Show(ex.ToString(), "Gemini Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<string> SendTextAndImageAsync(string model, string prompt, string imagePath)
        {
            // Load image bytes
            byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

            // Create Gemini client
            var client = new Client(apiKey: this.apiKey);

            // Build the content with both text and image
            var content = new Content
            {
                Parts = new List<Part>
                {
                    new Part { Text = prompt },

                    new Part
                    {
                        InlineData = new Blob
                        {
                            MimeType = "image/png",
                            Data = imageBytes
                        }
                    }
                }
            };


            // Send request async
            var response = await client.Models.GenerateContentAsync(
                model: model,
                contents: content
            );

            // Extract text
            return response.Candidates[0].Content.Parts[0].Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void currencyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.saleCurrencyLabel.Text = currencyComboBox.SelectedItem.ToString();
        }
    }
}
