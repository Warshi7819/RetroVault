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
            button1 = new Button();
            deleteButton = new Button();
            cancelButton = new Button();
            SuspendLayout();
            // 
            // saveButton
            // 
            saveButton.Location = new Point(812, 971);
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
            nameTextBox.Location = new Point(135, 10);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(790, 39);
            nameTextBox.TabIndex = 1;
            // 
            // categoryComboBox
            // 
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
            descLabel.Location = new Point(14, 351);
            descLabel.Name = "descLabel";
            descLabel.Size = new Size(70, 32);
            descLabel.TabIndex = 7;
            descLabel.Text = "Desc:";
            // 
            // descTextBox
            // 
            descTextBox.Location = new Point(135, 348);
            descTextBox.Multiline = true;
            descTextBox.Name = "descTextBox";
            descTextBox.Size = new Size(790, 281);
            descTextBox.TabIndex = 11;
            // 
            // yearLabel
            // 
            yearLabel.AutoSize = true;
            yearLabel.Location = new Point(481, 83);
            yearLabel.Name = "yearLabel";
            yearLabel.Size = new Size(63, 32);
            yearLabel.TabIndex = 9;
            yearLabel.Text = "Year:";
            // 
            // yearTextBox
            // 
            yearTextBox.Location = new Point(558, 79);
            yearTextBox.Name = "yearTextBox";
            yearTextBox.Size = new Size(200, 39);
            yearTextBox.TabIndex = 3;
            // 
            // publisherLabel
            // 
            publisherLabel.AutoSize = true;
            publisherLabel.Location = new Point(12, 656);
            publisherLabel.Name = "publisherLabel";
            publisherLabel.Size = new Size(117, 32);
            publisherLabel.TabIndex = 11;
            publisherLabel.Text = "Publisher:";
            // 
            // developerLabel
            // 
            developerLabel.AutoSize = true;
            developerLabel.Location = new Point(14, 712);
            developerLabel.Name = "developerLabel";
            developerLabel.Size = new Size(129, 32);
            developerLabel.TabIndex = 12;
            developerLabel.Text = "Developer:";
            // 
            // systemComboBox
            // 
            systemComboBox.FormattingEnabled = true;
            systemComboBox.Location = new Point(135, 79);
            systemComboBox.Name = "systemComboBox";
            systemComboBox.Size = new Size(312, 40);
            systemComboBox.TabIndex = 2;
            // 
            // publisherTextBox
            // 
            publisherTextBox.Location = new Point(156, 656);
            publisherTextBox.Name = "publisherTextBox";
            publisherTextBox.Size = new Size(769, 39);
            publisherTextBox.TabIndex = 12;
            // 
            // devTextBox
            // 
            devTextBox.Location = new Point(156, 709);
            devTextBox.Name = "devTextBox";
            devTextBox.Size = new Size(769, 39);
            devTextBox.TabIndex = 13;
            // 
            // priceLabel
            // 
            priceLabel.AutoSize = true;
            priceLabel.Location = new Point(481, 149);
            priceLabel.Name = "priceLabel";
            priceLabel.Size = new Size(70, 32);
            priceLabel.TabIndex = 19;
            priceLabel.Text = "Price:";
            // 
            // priceTextBox
            // 
            priceTextBox.Location = new Point(558, 143);
            priceTextBox.Name = "priceTextBox";
            priceTextBox.Size = new Size(200, 39);
            priceTextBox.TabIndex = 5;
            // 
            // currencyComboBox
            // 
            currencyComboBox.FormattingEnabled = true;
            currencyComboBox.Location = new Point(785, 143);
            currencyComboBox.Name = "currencyComboBox";
            currencyComboBox.Size = new Size(140, 40);
            currencyComboBox.TabIndex = 6;
            // 
            // storageLabel
            // 
            storageLabel.AutoSize = true;
            storageLabel.Location = new Point(14, 793);
            storageLabel.Name = "storageLabel";
            storageLabel.Size = new Size(136, 32);
            storageLabel.TabIndex = 22;
            storageLabel.Text = "Storage ref:";
            // 
            // storageTextBox
            // 
            storageTextBox.Location = new Point(156, 792);
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
            acquiredDateTextBox.Location = new Point(558, 279);
            acquiredDateTextBox.Name = "acquiredDateTextBox";
            acquiredDateTextBox.Size = new Size(367, 39);
            acquiredDateTextBox.TabIndex = 10;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(14, 935);
            button1.Name = "button1";
            button1.Size = new Size(212, 82);
            button1.TabIndex = 31;
            button1.Text = "Media Folder";
            button1.UseVisualStyleBackColor = true;
            // 
            // deleteButton
            // 
            deleteButton.Location = new Point(389, 971);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(150, 46);
            deleteButton.TabIndex = 32;
            deleteButton.Text = "Delete Item";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(656, 971);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(150, 46);
            cancelButton.TabIndex = 33;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // NewEditItemForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(974, 1029);
            Controls.Add(cancelButton);
            Controls.Add(deleteButton);
            Controls.Add(button1);
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
        private Button button1;
        private Button deleteButton;
        private Button cancelButton;
    }
}