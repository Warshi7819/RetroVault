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

        public VaultItemCard()
        {
            InitializeComponent();

            // Disable automatic sizing to allow manual control
            this.AutoSize = false;
            BuildLayout();

            WireAllControls(this);
        }

        protected virtual void OnCardClicked(EventArgs e)
        {
            CardClicked?.Invoke(this, e);
        }

        public void setId(string id)
        {
            this.id = "VaultItemCard_" + id.ToString();
        }

        public string getId()
        {
            return this.id;
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

            textPanel.Controls.Add(new Label { Text = "Line 1", AutoSize = true });
            textPanel.Controls.Add(new Label { Text = "Line 2", AutoSize = true });
            textPanel.Controls.Add(new Label { Text = "Line 3", AutoSize = true });
            textPanel.Controls.Add(new Label { Text = "Line 4", AutoSize = true });

            var imageBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.DimGray // just to see it
            };

            layout.Controls.Add(textPanel, 0, 0);
            layout.Controls.Add(imageBox, 1, 0);

            this.Padding = new Padding(10);
            this.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(layout);
        }

    }
}
