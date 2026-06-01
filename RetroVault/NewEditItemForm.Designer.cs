namespace RetroVault
{
    partial class NewEditItemForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            saveButton = new Button();
            nameLabel = new Label();
            nameTextBox = new TextBox();
            categoryComboBox = new ComboBox();
            systemLabel = new Label();
            categoryLabel = new Label();
            descLabel = new Label();
            descTextBox = new TextBox();
            yearLabel = new Label();
            yearTextBox = new TextBox();
            publisherLabel = new Label();
            developerLabel = new Label();
            systemComboBox = new ComboBox();
            publisherTextBox = new TextBox();
            devTextBox = new TextBox();
            priceLabel = new Label();
            priceTextBox = new TextBox();
            currencyComboBox = new ComboBox();
            storageLabel = new Label();
            storageTextBox = new TextBox();
            regionLabel = new Label();
            regionTextBox = new TextBox();
            completeLabel = new Label();
            completeTextBox = new TextBox();
            acquiredFromLabel = new Label();
            acquiredFromTextBox = new TextBox();
            acquiredDateLabel = new Label();
            acquiredDateTextBox = new TextBox();
            openMediaFolderButton = new Button();
            deleteButton = new Button();
            cancelButton = new Button();
            setThumbnailButton = new Button();
            button1 = new Button();
            soldCheckBox = new CheckBox();
            salePriceLabel = new Label();
            salePriceTextBox = new TextBox();
            saleCurrencyLabel = new Label();
            SuspendLayout();
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.Location = new Point(812, 1074);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(150, 46);
            saveButton.TabIndex = 15;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(14, 17);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(83, 32);
            nameLabel.TabIndex = 1;
            nameLabel.Text = "Name:";
            // 
            // nameTextBox
            // 
            nameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            nameTextBox.Location = new Point(135, 10);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(790, 39);
            nameTextBox.TabIndex = 1;
            // 
            // categoryComboBox
            // 
            categoryComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            categoryComboBox.FormattingEnabled = true;
            categoryComboBox.Location = new Point(135, 146);
            categoryComboBox.Name = "categoryComboBox";
            categoryComboBox.Size = new Size(312, 40);
            categoryComboBox.TabIndex = 4;
            // 
            // systemLabel
            // 
            systemLabel.AutoSize = true;
            systemLabel.Location = new Point(14, 82);
            systemLabel.Name = "systemLabel";
            systemLabel.Size = new Size(95, 32);
            systemLabel.TabIndex = 5;
            systemLabel.Text = "System:";
            // 
            // categoryLabel
            // 
            categoryLabel.AutoSize = true;
            categoryLabel.Location = new Point(14, 146);
            categoryLabel.Name = "categoryLabel";
            categoryLabel.Size = new Size(115, 32);
            categoryLabel.TabIndex = 6;
            categoryLabel.Text = "Category:";
            // 
            // descLabel
            // 
            descLabel.AutoSize = true;
            descLabel.Location = new Point(14, 413);
            descLabel.Name = "descLabel";
            descLabel.Size = new Size(70, 32);
            descLabel.TabIndex = 7;
            descLabel.Text = "Desc:";
            // 
            // descTextBox
            // 
            descTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            descTextBox.Location = new Point(135, 410);
            descTextBox.Multiline = true;
            descTextBox.Name = "descTextBox";
            descTextBox.Size = new Size(790, 340);
            descTextBox.TabIndex = 11;
            // 
            // yearLabel
            // 
            yearLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            yearLabel.AutoSize = true;
            yearLabel.Location = new Point(481, 83);
            yearLabel.Name = "yearLabel";
            yearLabel.Size = new Size(63, 32);
            yearLabel.TabIndex = 9;
            yearLabel.Text = "Year:";
            // 
            // yearTextBox
            // 
            yearTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            yearTextBox.Location = new Point(558, 79);
            yearTextBox.Name = "yearTextBox";
            yearTextBox.Size = new Size(200, 39);
            yearTextBox.TabIndex = 3;
            // 
            // publisherLabel
            // 
            publisherLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            publisherLabel.AutoSize = true;
            publisherLabel.Location = new Point(12, 774);
            publisherLabel.Name = "publisherLabel";
            publisherLabel.Size = new Size(117, 32);
            publisherLabel.TabIndex = 11;
            publisherLabel.Text = "Publisher:";
            // 
            // developerLabel
            // 
            developerLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            developerLabel.AutoSize = true;
            developerLabel.Location = new Point(14, 830);
            developerLabel.Name = "developerLabel";
            developerLabel.Size = new Size(129, 32);
            developerLabel.TabIndex = 12;
            developerLabel.Text = "Developer:";
            // 
            // systemComboBox
            // 
            systemComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            systemComboBox.FormattingEnabled = true;
            systemComboBox.Location = new Point(135, 79);
            systemComboBox.Name = "systemComboBox";
            systemComboBox.Size = new Size(312, 40);
            systemComboBox.TabIndex = 2;
            // 
            // publisherTextBox
            // 
            publisherTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            publisherTextBox.Location = new Point(156, 774);
            publisherTextBox.Name = "publisherTextBox";
            publisherTextBox.Size = new Size(769, 39);
            publisherTextBox.TabIndex = 12;
            // 
            // devTextBox
            // 
            devTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            devTextBox.Location = new Point(156, 827);
            devTextBox.Name = "devTextBox";
            devTextBox.Size = new Size(769, 39);
            devTextBox.TabIndex = 13;
            // 
            // priceLabel
            // 
            priceLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            priceLabel.AutoSize = true;
            priceLabel.Location = new Point(481, 149);
            priceLabel.Name = "priceLabel";
            priceLabel.Size = new Size(70, 32);
            priceLabel.TabIndex = 19;
            priceLabel.Text = "Price:";
            // 
            // priceTextBox
            // 
            priceTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            priceTextBox.Location = new Point(558, 143);
            priceTextBox.Name = "priceTextBox";
            priceTextBox.Size = new Size(200, 39);
            priceTextBox.TabIndex = 5;
            // 
            // currencyComboBox
            // 
            currencyComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            currencyComboBox.FormattingEnabled = true;
            currencyComboBox.Location = new Point(785, 143);
            currencyComboBox.Name = "currencyComboBox";
            currencyComboBox.Size = new Size(140, 40);
            currencyComboBox.TabIndex = 6;
            currencyComboBox.SelectedIndexChanged += currencyComboBox_SelectedIndexChanged;
            // 
            // storageLabel
            // 
            storageLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            storageLabel.AutoSize = true;
            storageLabel.Location = new Point(14, 893);
            storageLabel.Name = "storageLabel";
            storageLabel.Size = new Size(136, 32);
            storageLabel.TabIndex = 22;
            storageLabel.Text = "Storage ref:";
            // 
            // storageTextBox
            // 
            storageTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            storageTextBox.Location = new Point(156, 892);
            storageTextBox.Name = "storageTextBox";
            storageTextBox.Size = new Size(769, 39);
            storageTextBox.TabIndex = 14;
            // 
            // regionLabel
            // 
            regionLabel.AutoSize = true;
            regionLabel.Location = new Point(14, 218);
            regionLabel.Name = "regionLabel";
            regionLabel.Size = new Size(93, 32);
            regionLabel.TabIndex = 24;
            regionLabel.Text = "Region:";
            // 
            // regionTextBox
            // 
            regionTextBox.Location = new Point(135, 215);
            regionTextBox.Name = "regionTextBox";
            regionTextBox.Size = new Size(208, 39);
            regionTextBox.TabIndex = 7;
            // 
            // completeLabel
            // 
            completeLabel.AutoSize = true;
            completeLabel.Location = new Point(14, 279);
            completeLabel.Name = "completeLabel";
            completeLabel.Size = new Size(123, 32);
            completeLabel.TabIndex = 26;
            completeLabel.Text = "Complete:";
            // 
            // completeTextBox
            // 
            completeTextBox.Location = new Point(143, 276);
            completeTextBox.Name = "completeTextBox";
            completeTextBox.Size = new Size(200, 39);
            completeTextBox.TabIndex = 9;
            // 
            // acquiredFromLabel
            // 
            acquiredFromLabel.AutoSize = true;
            acquiredFromLabel.Location = new Point(368, 218);
            acquiredFromLabel.Name = "acquiredFromLabel";
            acquiredFromLabel.Size = new Size(176, 32);
            acquiredFromLabel.TabIndex = 28;
            acquiredFromLabel.Text = "Acquired From:";
            // 
            // acquiredFromTextBox
            // 
            acquiredFromTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            acquiredFromTextBox.Location = new Point(558, 215);
            acquiredFromTextBox.Name = "acquiredFromTextBox";
            acquiredFromTextBox.Size = new Size(367, 39);
            acquiredFromTextBox.TabIndex = 8;
            // 
            // acquiredDateLabel
            // 
            acquiredDateLabel.AutoSize = true;
            acquiredDateLabel.Location = new Point(368, 283);
            acquiredDateLabel.Name = "acquiredDateLabel";
            acquiredDateLabel.Size = new Size(171, 32);
            acquiredDateLabel.TabIndex = 30;
            acquiredDateLabel.Text = "Acquired Date:";
            // 
            // acquiredDateTextBox
            // 
            acquiredDateTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            acquiredDateTextBox.Location = new Point(558, 279);
            acquiredDateTextBox.Name = "acquiredDateTextBox";
            acquiredDateTextBox.Size = new Size(367, 39);
            acquiredDateTextBox.TabIndex = 10;
            // 
            // openMediaFolderButton
            // 
            openMediaFolderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            openMediaFolderButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            openMediaFolderButton.Location = new Point(14, 1038);
            openMediaFolderButton.Name = "openMediaFolderButton";
            openMediaFolderButton.Size = new Size(212, 82);
            openMediaFolderButton.TabIndex = 31;
            openMediaFolderButton.Text = "Media Folder";
            openMediaFolderButton.UseVisualStyleBackColor = true;
            openMediaFolderButton.Click += button1_Click;
            // 
            // deleteButton
            // 
            deleteButton.Anchor = AnchorStyles.Bottom;
            deleteButton.Location = new Point(389, 1074);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(150, 46);
            deleteButton.TabIndex = 32;
            deleteButton.Text = "Delete Item";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new Point(656, 1074);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(150, 46);
            cancelButton.TabIndex = 33;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // setThumbnailButton
            // 
            setThumbnailButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            setThumbnailButton.Location = new Point(14, 975);
            setThumbnailButton.Name = "setThumbnailButton";
            setThumbnailButton.Size = new Size(212, 46);
            setThumbnailButton.TabIndex = 34;
            setThumbnailButton.Text = "Set Thumbnail";
            setThumbnailButton.UseVisualStyleBackColor = true;
            setThumbnailButton.Click += setThumbnailButton_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Enabled = false;
            button1.Location = new Point(731, 975);
            button1.Name = "button1";
            button1.Size = new Size(231, 46);
            button1.TabIndex = 35;
            button1.Text = "Info From Gemini";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // soldCheckBox
            // 
            soldCheckBox.AutoSize = true;
            soldCheckBox.Location = new Point(143, 346);
            soldCheckBox.Name = "soldCheckBox";
            soldCheckBox.Size = new Size(148, 36);
            soldCheckBox.TabIndex = 37;
            soldCheckBox.Text = "Item Sold";
            soldCheckBox.UseVisualStyleBackColor = true;
            soldCheckBox.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // salePriceLabel
            // 
            salePriceLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            salePriceLabel.AutoSize = true;
            salePriceLabel.Location = new Point(430, 350);
            salePriceLabel.Name = "salePriceLabel";
            salePriceLabel.Size = new Size(121, 32);
            salePriceLabel.TabIndex = 38;
            salePriceLabel.Text = "Sale Price:";
            // 
            // salePriceTextBox
            // 
            salePriceTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            salePriceTextBox.Location = new Point(557, 347);
            salePriceTextBox.Name = "salePriceTextBox";
            salePriceTextBox.Size = new Size(200, 39);
            salePriceTextBox.TabIndex = 39;
            // 
            // saleCurrencyLabel
            // 
            saleCurrencyLabel.AutoSize = true;
            saleCurrencyLabel.Location = new Point(785, 350);
            saleCurrencyLabel.Name = "saleCurrencyLabel";
            saleCurrencyLabel.Size = new Size(105, 32);
            saleCurrencyLabel.TabIndex = 40;
            saleCurrencyLabel.Text = "currency";
            // 
            // NewEditItemForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(974, 1132);
            Controls.Add(saleCurrencyLabel);
            Controls.Add(salePriceTextBox);
            Controls.Add(salePriceLabel);
            Controls.Add(soldCheckBox);
            Controls.Add(button1);
            Controls.Add(setThumbnailButton);
            Controls.Add(cancelButton);
            Controls.Add(deleteButton);
            Controls.Add(openMediaFolderButton);
            Controls.Add(acquiredDateTextBox);
            Controls.Add(acquiredDateLabel);
            Controls.Add(acquiredFromTextBox);
            Controls.Add(acquiredFromLabel);
            Controls.Add(completeTextBox);
            Controls.Add(completeLabel);
            Controls.Add(regionTextBox);
            Controls.Add(regionLabel);
            Controls.Add(storageTextBox);
            Controls.Add(storageLabel);
            Controls.Add(currencyComboBox);
            Controls.Add(priceTextBox);
            Controls.Add(priceLabel);
            Controls.Add(devTextBox);
            Controls.Add(publisherTextBox);
            Controls.Add(systemComboBox);
            Controls.Add(developerLabel);
            Controls.Add(publisherLabel);
            Controls.Add(yearTextBox);
            Controls.Add(yearLabel);
            Controls.Add(descTextBox);
            Controls.Add(descLabel);
            Controls.Add(categoryLabel);
            Controls.Add(systemLabel);
            Controls.Add(categoryComboBox);
            Controls.Add(nameTextBox);
            Controls.Add(nameLabel);
            Controls.Add(saveButton);
            Name = "NewEditItemForm";
            Text = "New / Edit Item";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button saveButton;
        private Label nameLabel;
        private TextBox nameTextBox;
        private ComboBox systemComboBox;
        private ComboBox categoryComboBox;
        private Label systemLabel;
        private Label categoryLabel;
        private Label descLabel;
        private TextBox descTextBox;
        private Label yearLabel;
        private TextBox yearTextBox;
        private Label publisherLabel;
        private Label developerLabel;
        private TextBox publisherTextBox;
        private TextBox devTextBox;
        private Label priceLabel;
        private TextBox priceTextBox;
        private ComboBox currencyComboBox;
        private Label storageLabel;
        private TextBox storageTextBox;
        private Label regionLabel;
        private TextBox regionTextBox;
        private Label completeLabel;
        private TextBox completeTextBox;
        private Label acquiredFromLabel;
        private TextBox acquiredFromTextBox;
        private Label acquiredDateLabel;
        private TextBox acquiredDateTextBox;
        private Button openMediaFolderButton;
        private Button deleteButton;
        private Button cancelButton;
        private Button setThumbnailButton;
        private Button button1;
        private CheckBox soldCheckBox;
        private Label salePriceLabel;
        private TextBox salePriceTextBox;
        private Label saleCurrencyLabel;
    }
}