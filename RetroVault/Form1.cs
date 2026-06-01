using Microsoft.Extensions.Configuration;
using System.Text.Json;
using RetroVault.Shared;
using RetroVault.Shared.Models;

namespace RetroVault
{
    public partial class Form1 : Form
    {
        // Initial search values
        string searchTerm = "";
        string selectedSystem = "All";
        string selectedCategory = "All";
        string vaultPath = "";
        int currentPage = 1;

        // Configuration
        VaultSettingsConfig vaultSettingsConfig;

        // REST API
        VaultApiClient api;

        // If the form has been initialized
        bool initializedForm = false;

        public Form1()
        {
            InitializeComponent();
            LoadConfig();

            // Center form horizontally (x), but not vertically (y)
            if (Screen.PrimaryScreen != null)
            {
                this.StartPosition = FormStartPosition.Manual;
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                int centerX = (workingArea.Width - this.Width) / 2;
                int customY = 100;
                this.Location = new Point(centerX, customY);
            }

            // Load icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", "RetroVault.ico");
            this.Icon = new Icon(iconPath);

            InitializeVaultPanel();

            this.api = new VaultApiClient(new HttpClient
            {
                BaseAddress = new Uri(this.vaultSettingsConfig.RESTAPI)
            });

            // set initialized to true after loading config
            initializedForm = true;
        }

        private void LoadConfig()
        {
            // Load configuration from appsettings.json
            var config = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory) // Set the base path
                        .AddJsonFile("config.json", optional: false, reloadOnChange: true) // Add the JSON file provider
                        .Build(); // Build the configuration
            vaultSettingsConfig = new VaultSettingsConfig();
            config.Bind(vaultSettingsConfig);

            // Set vault path from config
            vaultPath = vaultSettingsConfig.VaultPath;
        }

        private void InitializeVaultPanel()
        {
            vaultPanel.AutoScroll = true;
            vaultPanel.WrapContents = false;
            vaultPanel.FlowDirection = FlowDirection.TopDown;
            vaultPanel.SizeChanged += vaultPanel_SizeChanged;
            Controls.Add(vaultPanel);


            foreach (string system in vaultSettingsConfig.Systems)
            {
                systemComboBox.Items.Add(system);
            }
            foreach (string category in vaultSettingsConfig.Categories)
            {
                catComboBox.Items.Add(category);
            }

            systemComboBox.SelectedIndex = 0; // Select "All" by default
            catComboBox.SelectedIndex = 0; // Select "All" by default

            // Disable prev and next button at startup
            buttonNext.Enabled = false;
            buttonPrev.Enabled = false;
        }


        public void SaveConfig(VaultSettingsConfig config)
        {
            var configFileName = "config.json";
            var confPath = Path.Combine(AppContext.BaseDirectory, configFileName);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(configFileName, json);
        }


        private void vaultPanel_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control c in vaultPanel.Controls)
            {
                c.Width = vaultPanel.ClientSize.Width - c.Margin.Horizontal;
            }
        }

        private async void Card_Clicked(object? sender, EventArgs e)
        {
            if (sender is VaultItemCard card)
            {
                await updateVaultItemHelper(card.GetVaultItem());
            }
        }

        private async Task updateVaultItemHelper(VaultItem vaultItem)
        {
            NewEditItemForm editForm = new NewEditItemForm(vaultItem, vaultSettingsConfig, this.Location);
            editForm.ShowDialog();

            if (editForm.DialogResult == DialogResult.OK)
            {
                if (editForm.isDeleteRequested())
                {
                    // see if we should remove thumbnail image
                    var thumbPath = "thumbnails/" + "item_" + editForm.getVaultItem().Id.ToString() + ".png";
                    if (System.IO.File.Exists(thumbPath))
                    {
                        System.IO.File.Delete(thumbPath);
                    }

                    // TODO: See if we should delete the corresponding media folder and contents as well.
                    // Or maybe just let it sit.. as we may be deleting media files that are not easily recovered. 

                    // Delete vault item from DB using the api
                    await api.DeleteVaultItemAsync(editForm.getVaultItem().Id);
                }
                else
                {
                    // Update existing vault item using the api
                    await updateVaultItem(editForm.getVaultItem());
                    // Check if thumbnail is updated, if so upload
                    if (editForm.isThumbnailUpdated())
                    {
                        var thumbPath = "thumbnails/" + "item_" + editForm.getVaultItem().Id.ToString() + ".png";
                        await api.UploadThumbnail(editForm.getVaultItem().Id, thumbPath);
                    }
                }

                // Refresh current search results
                await DoSearchAsync();
            }
        }


        private async void newButton_Click(object sender, EventArgs e)
        {
            // I needed to change this so that the vault item is created and 
            // stored to the DB before we open the form. This is because we need the ID of the vault item
            // to create the media folder. 

            // Once the item is created, this will be just like any other update. 
            // Possible downside is that if a user cancels out a blank item has been created in the DB. 
            // The user has to then explicitly delete it to be removed. But I guess that is better as we are
            // optimizing the common route with less clicks this way

            newButton.Enabled = false;

            var item = new VaultItem();
            item.Name = "New Item"; // Name must be set or item won't be created

            // Essential to get the new item returned from API and use that one 
            // going forward as this is the one containing the correct ID.
            var newItem = await createVaultItem(item);
            await updateVaultItemHelper(newItem);

            newButton.Enabled = true;
        }

        private async Task updateVaultItem(VaultItem vaultItem)
        {
            var result = await api.UpdateVaultItemAsync(vaultItem.Id, vaultItem);
        }

        private async Task<VaultItem?> createVaultItem(VaultItem vaultItem)
        {
            return await api.CreateVaultItemAsync(vaultItem);
        }


        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            // update search term
            searchTerm = searchBox.Text;

            this.labelNumSearchResults.Text = "Results: Unknown, Issue Search";
            this.currentPage = 1; // reset to first page when changing search term
            this.buttonNext.Enabled = false;
            this.buttonPrev.Enabled = false;
        }

        private async void systemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Implement system filtering logic here
            selectedSystem = systemComboBox.SelectedItem.ToString() ?? "All";

            if (initializedForm)
            {
                this.currentPage = 1; // reset to first page when changing system filter
                await DoSearchAsync();
            }
        }

        private async void catComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Implement category filtering logic here
            selectedCategory = catComboBox.SelectedItem.ToString() ?? "All";
            if (initializedForm)
            {
                this.currentPage = 1; // reset to first page when changing category filter
                await DoSearchAsync();
            }
        }
        private async void searchButton_ClickAsync(object sender, EventArgs e)
        {
            this.currentPage = 1; // reset to first page when performing new search
            await DoSearchAsync();
        }

        private async Task DoSearchAsync()
        {
            // Execute search and filtering based on searchTerm, selectedSystem, and selectedCategory
            // Clear existing items
            vaultPanel.Controls.Clear();

            // Search
            var results = await api.SearchVaultItemsAsync(name: searchBox.Text,
                                                          category: this.selectedCategory,
                                                          system: this.selectedSystem,
                                                          pageSize: 5,
                                                          page: this.currentPage);

            // Get total count and calculate pagination 
            if (results.TotalCount > 0)
            {
                this.labelNumSearchResults.Text = "Results: " + results.TotalCount + " (Page " + this.currentPage + " of " + results.TotalPages + ")";
            }
            else
            {
                this.labelNumSearchResults.Text = "Results: 0";
            }

            if (this.currentPage > 1)
            {
                // Make previous available
                this.buttonPrev.Enabled = true;
            }
            else
            {
                // Make previous unavailable
                this.buttonPrev.Enabled = false;
            }

            if (this.currentPage < results.TotalPages)
            {
                // Make next available
                this.buttonNext.Enabled = true;
            }
            else 
            {
                // Make next unavailable
                this.buttonNext.Enabled = false;
            }

            foreach (VaultItem item in results.Items)
            {
                var card = new VaultItemCard(item, this.vaultSettingsConfig.ThumbnailURL)
                {
                    Margin = new Padding(0, 0, 0, 10),
                    Height = 200
                };

                // Add event handler for CardClicked event
                card.CardClicked += Card_Clicked;
                // initial width
                card.Width = vaultPanel.ClientSize.Width - card.Margin.Horizontal;
                vaultPanel.Controls.Add(card);
            }
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            // Open configuration/settings logic here
            ConfigForm configForm = new ConfigForm(vaultSettingsConfig, this.Location);
            configForm.ShowDialog();

            if (configForm.DialogResult == DialogResult.OK)
            {
                // Reload config
                SaveConfig(configForm.getVaultSettingsConfig());
            }
        }

        private void searchBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // optional: prevents ding sound }
                searchButton_ClickAsync(sender, e);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            this.currentPage--;
            this.buttonPrev.Enabled = false;
            this.buttonNext.Enabled = false;
            _ = DoSearchAsync();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.currentPage++;
            this.buttonPrev.Enabled = false;
            this.buttonNext.Enabled = false;
            _ = DoSearchAsync();
        }
    }
}
