using RetroVaultAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RetroVault
{
    public partial class NewEditItemForm : Form
    {
        internal VaultItem vaultItem;
        internal VaultSettingsConfig config;
        internal Boolean isEditMode = true;
        public NewEditItemForm(VaultItem item, VaultSettingsConfig conf)
        {
            InitializeComponent();
            vaultItem = item;
            config = conf;
            if (vaultItem == null)
            {
                isEditMode = false;
                this.Text = "Add New Item";
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
                priceTextBox.Text = vaultItem.PurchasePrice.ToString("C");
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
    }
}
