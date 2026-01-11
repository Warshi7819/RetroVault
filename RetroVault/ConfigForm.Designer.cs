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
            selectVaultbutton = new Button();
            saveButton = new Button();
            cancelButton = new Button();
            label2 = new Label();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(295, 17);
            label1.Name = "label1";
            label1.Size = new Size(183, 32);
            label1.TabIndex = 0;
            label1.Text = "None selected...";
            // 
            // selectVaultbutton
            // 
            selectVaultbutton.Location = new Point(20, 10);
            selectVaultbutton.Name = "selectVaultbutton";
            selectVaultbutton.Size = new Size(252, 46);
            selectVaultbutton.TabIndex = 1;
            selectVaultbutton.Text = "Select Vault Folder";
            selectVaultbutton.UseVisualStyleBackColor = true;
            selectVaultbutton.Click += selectVaultbutton_Click;
            // 
            // saveButton
            // 
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
            cancelButton.Location = new Point(771, 532);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(150, 46);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 80);
            label2.Name = "label2";
            label2.Size = new Size(114, 32);
            label2.TabIndex = 4;
            label2.Text = "Currency:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(145, 77);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(200, 39);
            textBox1.TabIndex = 5;
            textBox1.Text = "NOK";
            // 
            // ConfigForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 590);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(cancelButton);
            Controls.Add(saveButton);
            Controls.Add(selectVaultbutton);
            Controls.Add(label1);
            Name = "ConfigForm";
            Text = "App Config";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button selectVaultbutton;
        private Button saveButton;
        private Button cancelButton;
        private Label label2;
        private TextBox textBox1;
    }
}