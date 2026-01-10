using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RetroVault
{
    public partial class VaultItemCard : UserControl
    {
        public event EventHandler CardClicked;
        private string id = "";

        private string lineOne = "";
        private string lineTwo = "";
        private string lineThree = "";
        private string lineFour = "";

        public VaultItemCard(VaultItem item)
        {
            InitializeComponent();
            lineOne = "Name: " + item.Name;
            lineTwo = "System: " + item.System;
            lineThree = "Category: " + item.Category;
            lineFour = "Year: " + item.year.ToString();
            id = item.vaultID.ToString();

            // Disable automatic sizing to allow manual control
            this.AutoSize = false;
            BuildLayout();

            WireAllControls(this);
        }

        public string getId()
        {
            return id;
        }

        protected virtual void OnCardClicked(EventArgs e)
        {
            CardClicked?.Invoke(this, e);
        }

        private void WireAllControls(Control parent) { 
            parent.Click += (s, e) => OnCardClicked(EventArgs.Empty); 
            foreach (Control child in parent.Controls) 
                WireAllControls(child); 
        }

        private void BuildLayout()
        {
            var layout = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            var textPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Fill,
                WrapContents = false
            };

            // Make the first label's text bold
            textPanel.Controls.Add(new Label { Text = lineOne, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) });
            textPanel.Controls.Add(new Label { Text = lineTwo, AutoSize = true });
            textPanel.Controls.Add(new Label { Text = lineThree, AutoSize = true });
            textPanel.Controls.Add(new Label { Text = lineFour, AutoSize = true });

            var imageBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray // just to see it
            };

            imageBox.Image = Image.FromFile("thumbnails/c64.png");

            layout.Controls.Add(textPanel, 0, 0);
            layout.Controls.Add(imageBox, 1, 0);

            this.Padding = new Padding(10);
            this.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(layout);
        }

    }
}
