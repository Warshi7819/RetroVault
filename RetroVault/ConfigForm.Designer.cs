namespace RetroVault
{
    partial class ConfigForm
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
            label1 = new Label();
            selectMediaLibraryButton = new Button();
            saveButton = new Button();
            cancelButton = new Button();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            label2 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 59);
            label1.Name = "label1";
            label1.Size = new Size(183, 32);
            label1.TabIndex = 0;
            label1.Text = "None selected...";
            // 
            // selectMediaLibraryButton
            // 
            selectMediaLibraryButton.Location = new Point(20, 10);
            selectMediaLibraryButton.Name = "selectMediaLibraryButton";
            selectMediaLibraryButton.Size = new Size(252, 46);
            selectMediaLibraryButton.TabIndex = 1;
            selectMediaLibraryButton.Text = "Media Library Folder";
            selectMediaLibraryButton.UseVisualStyleBackColor = true;
            selectMediaLibraryButton.Click += selectMediaLibraryButton_Click;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.Location = new Point(615, 532);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(150, 46);
            saveButton.TabIndex = 2;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new Point(771, 532);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(150, 46);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(20, 227);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(786, 36);
            checkBox1.TabIndex = 4;
            checkBox1.Text = "Auto AI search (and set as thumbnail) on image paste (new/edit item)";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(20, 281);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(625, 36);
            checkBox2.TabIndex = 5;
            checkBox2.Text = "Auto open image folder when saving new/edited item";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(22, 161);
            label2.Name = "label2";
            label2.Size = new Size(219, 32);
            label2.TabIndex = 6;
            label2.Text = "Default storage ref:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(247, 154);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(312, 39);
            textBox1.TabIndex = 7;
            // 
            // button1
            // 
            button1.Location = new Point(20, 380);
            button1.Name = "button1";
            button1.Size = new Size(196, 46);
            button1.TabIndex = 8;
            button1.Text = "Backup Data";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // ConfigForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 590);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(cancelButton);
            Controls.Add(saveButton);
            Controls.Add(selectMediaLibraryButton);
            Controls.Add(label1);
            Name = "ConfigForm";
            Text = "App Config";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button selectMediaLibraryButton;
        private Button saveButton;
        private Button cancelButton;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Label label2;
        private TextBox textBox1;
        private Button button1;
    }
}