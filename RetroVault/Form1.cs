using Microsoft.Extensions.Configuration;
using System.IO;

namespace RetroVault
{
    public partial class Form1 : Form
    {
        string searchTerm = "";
        string selectedSystem = "All";
        string selectedCategory = "All";



        public Form1()
        {
            InitializeComponent();
            // Load icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", "RetroVault.ico");
            this.Icon = new Icon(iconPath);

            InitializeVaultPanel();
            LoadVaultItems();
        }

        private void InitializeVaultPanel()
        {
            vaultPanel.AutoScroll = true;
            vaultPanel.WrapContents = false;
            vaultPanel.FlowDirection = FlowDirection.TopDown;
            vaultPanel.SizeChanged += vaultPanel_SizeChanged;
            Controls.Add(vaultPanel);

            var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Add the JSON file provider
                        .Build(); // Build the configuration
            var vaultSettings = new VaultSettingsConfig();
            config.GetSection("VaultSettings").Bind(vaultSettings);

            foreach(string system in vaultSettings.Systems)
            {
                systemComboBox.Items.Add(system);
            }
            foreach (string category in vaultSettings.Categories)
            {
                catComboBox.Items.Add(category);
            }
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
            vaultItem.year = 1983;
            vaultItem.vaultID = 1;
            vaultItem.thumbnailImage = "thumbnails/c64.png";
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

        private void Card_Clicked(object? sender, EventArgs e)
        {
            if (sender is VaultItemCard card)
            {
                MessageBox.Show($"Card clicked: {card.getId()}");
            }
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            // Implement search filtering logic here
            searchTerm = searchBox.Text;
        }

        private void systemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Implement system filtering logic here
            selectedSystem = systemComboBox.SelectedItem.ToString() ?? "All";
        }

        private void catComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Implement category filtering logic here
            selectedCategory = catComboBox.SelectedItem.ToString() ?? "All";
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            // Execute search and filtering based on searchTerm, selectedSystem, and selectedCategory
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // New item logic here
        }
    }
}
