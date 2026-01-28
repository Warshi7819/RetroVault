using RetroVaultAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace RetroVault
{
    public partial class NewEditItemForm : Form
    {
        internal VaultItem vaultItem;
        internal VaultSettingsConfig config;
        internal Boolean deleteItem = false;
        public NewEditItemForm(VaultItem item, VaultSettingsConfig conf)
        {
            InitializeComponent();
            vaultItem = item;
            config = conf;
            if (vaultItem == null)
            {
                this.Text = "Add New Item";
                this.deleteButton.Enabled = false;
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
                priceTextBox.Text = vaultItem.PurchasePrice.ToString();
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
                "Delete item?",
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
                MessageBox.Show("Operation Cancelled!", "Ok", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
