namespace RetroVault
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Load icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", "RetroVault.ico");
            this.Icon = new Icon(iconPath);

            InitializeVaultPanel();
            LoadVaultItems();
        }

        private void InitializeVaultPanel() { 
            //vaultPanel.Dock = DockStyle.Fill;
            vaultPanel.AutoScroll = true;
            vaultPanel.WrapContents = false;   
            vaultPanel.FlowDirection = FlowDirection.TopDown;   
            
            vaultPanel.SizeChanged += vaultPanel_SizeChanged; Controls.Add(vaultPanel); 
        }

        private void LoadVaultItems()
        {
            for (int i = 0; i < 10; i++)
            {
                var card = new VaultItemCard { 
                    Margin = new Padding(0, 0, 0, 10), 
                    Height = 200 
                };
                card.setId(i.ToString());

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
    }
}
