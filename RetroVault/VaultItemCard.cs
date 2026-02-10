using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using RetroVaultAPI.Models; 

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
        private VaultItem vaultItem;

        public VaultItemCard(VaultItem item)
        {
            InitializeComponent();
            this.vaultItem = item;
            lineOne = "Name: " + item.Name;
            lineTwo = "System: " + item.System;
            lineThree = "Category: " + item.Category;
            lineFour = "Year: " + item.Year.ToString();
            id = item.Id.ToString();

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


            var thumbPath = "thumbnails/" +"item_"+ vaultItem.Id.ToString() + ".png";
            if(!System.IO.File.Exists(thumbPath))
            {
                thumbPath = "thumbnails/detective.png";
            }

            imageBox.Image = LoadImageUnlocked(thumbPath);

            layout.Controls.Add(textPanel, 0, 0);
            layout.Controls.Add(imageBox, 1, 0);

            this.Padding = new Padding(10);
            this.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(layout);
        }


        // Load image without locking the file
        // This prevents file locks that can causes issues when trying to overwrite
        // or delete the image file later when e.g. deleting the item or updating the thumbnail
        public static Image LoadImageUnlocked(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            using (var ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }


        public VaultItem GetVaultItem()
        {
            return vaultItem;
        }
    }
}
