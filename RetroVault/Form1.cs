using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using RetroVaultAPI.Models;
using System.IO;
using System.Threading.Tasks;

namespace RetroVault
{
    public partial class Form1 : Form
    {
        string searchTerm = "";
        string selectedSystem = "All";
        string selectedCategory = "All";
        string vaultPath = "";
        VaultSettingsConfig vaultSettingsConfig;
        VaultApiClient api;
        bool initializedForm = false;

        public Form1()
        {
            InitializeComponent();
            LoadConfig();

            // Center form horizontally (x), but not vertically (y)
            this.StartPosition = FormStartPosition.Manual;
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int centerX = (workingArea.Width - this.Width) / 2;
            int customY = 100;
            this.Location = new Point(centerX, customY);

            // Load icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", "RetroVault.ico");
            this.Icon = new Icon(iconPath);

            InitializeVaultPanel();

            this.api = new VaultApiClient(new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7251/api/")
            });

            // set initialized to true after loading config
            initializedForm = true;
        }

        private void LoadConfig()
        {
            // Load configuration from appsettings.json
            var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path
                        .AddJsonFile("config.json", optional: false, reloadOnChange: true) // Add the JSON file provider
                        .Build(); // Build the configuration
            vaultSettingsConfig = new VaultSettingsConfig();
            config.GetSection("VaultSettings").Bind(vaultSettingsConfig);
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
        }

        private void LoadVaultItems()
        {
            // Clear existing items
            vaultPanel.Controls.Clear();

            // Load new items - Dummy data for demonstration right now
            List<VaultItem> vaultItems = new List<VaultItem>();
            VaultItem vaultItem = new VaultItem();
            vaultItem.Name = "C64 - Breadbin";
            vaultItem.System = "Commodore 64";
            vaultItem.Category = "Hardware";
            vaultItem.Year = 1983;
            vaultItem.Id = 1;
            vaultItem.Thumbnail = "thumbnails/c64.png";
            vaultItems.Add(vaultItem);

            foreach (VaultItem item in vaultItems)
            {
                var card = new VaultItemCard(item)
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
                NewEditItemForm editForm = new NewEditItemForm(card.GetVaultItem(), vaultSettingsConfig);
                editForm.ShowDialog();

                if (editForm.DialogResult == DialogResult.OK)
                {
                    if (editForm.isDeleteRequested())
                    { 
                        // see if we should remove thumbnail image
                        var thumbPath = "thumbnails/" + "item_" + editForm.getVaultItem().Id.ToString() + ".png";
                        if(System.IO.File.Exists(thumbPath))        
                        {
                            System.IO.File.Delete(thumbPath);
                        }
                        
                        await api.DeleteVaultItemAsync(editForm.getVaultItem().Id);
                    }
                    else
                    {
                        // Update existing vault item using the api
                        await updateVaultItem(editForm.getVaultItem());
                    }
                   
                    // Refresh current search results
                    await DoSearchAsync();
                }

            }
        }

        private async void newButton_Click(object sender, EventArgs e)
        {
            // New item logic here
            NewEditItemForm newItemForm = new NewEditItemForm(null, vaultSettingsConfig);
            newItemForm.ShowDialog();


            if (newItemForm.DialogResult == DialogResult.OK)
            {
                // Update existing vault item using the api
                await createVaultItem(newItemForm.getVaultItem());

                // Refresh current search results
                await DoSearchAsync();
            }
        }

        private async Task updateVaultItem(VaultItem vaultItem)
        {
            var result = await api.UpdateVaultItemAsync(vaultItem.Id, vaultItem);
        }

        private async Task createVaultItem(VaultItem vaultItem)
        {   
            var result = await api.CreateVaultItemAsync(vaultItem);
        }


        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            // Implement search filtering logic here
            searchTerm = searchBox.Text;
        }

        private async void systemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Implement system filtering logic here
            selectedSystem = systemComboBox.SelectedItem.ToString() ?? "All";

            if (initializedForm)
            {
                await DoSearchAsync();
            }
        }

        private async void catComboBox_SelectedIndexChanged(object sender, EventArgs e)
        { 
            // Implement category filtering logic here
            selectedCategory = catComboBox.SelectedItem.ToString() ?? "All";
            if (initializedForm)
            {
                await DoSearchAsync();
            }
        }
        private async void searchButton_ClickAsync(object sender, EventArgs e)
        {
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
                                                          system: this.selectedSystem);

            foreach (VaultItem item in results)
            {
                var card = new VaultItemCard(item)
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
            ConfigForm configForm = new ConfigForm();
            configForm.ShowDialog();

        }

        private void searchBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // optional: prevents ding sound }
                searchButton_ClickAsync(sender, e);
            }
        }
    }
}
