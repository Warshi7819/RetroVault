using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RetroVault
{
    public partial class ItemOverviewForm : Form
    {
        internal VaultItemCard item;
        public ItemOverviewForm(VaultItemCard card)
        {
            InitializeComponent();
            item = card;

            // Load icon
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", "RetroVault.ico");
            this.Icon = new Icon(iconPath);
        }
    }
}
