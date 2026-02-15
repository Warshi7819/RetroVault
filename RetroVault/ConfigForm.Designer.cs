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
            // ConfigForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 590);
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
    }
}