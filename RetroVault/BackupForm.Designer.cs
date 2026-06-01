namespace RetroVault
{
    partial class BackupForm
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
            backupButton = new Button();
            textBox1 = new TextBox();
            button1 = new Button();
            textBox2 = new TextBox();
            SuspendLayout();
            // 
            // backupButton
            // 
            backupButton.Location = new Point(622, 512);
            backupButton.Name = "backupButton";
            backupButton.Size = new Size(150, 46);
            backupButton.TabIndex = 0;
            backupButton.Text = "Backup";
            backupButton.UseVisualStyleBackColor = true;
            backupButton.Click += backupButton_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(24, 125);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(748, 381);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(24, 12);
            button1.Name = "button1";
            button1.Size = new Size(281, 46);
            button1.TabIndex = 2;
            button1.Text = "Select Output Folder";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(24, 64);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(748, 39);
            textBox2.TabIndex = 3;
            // 
            // BackupForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 570);
            Controls.Add(textBox2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(backupButton);
            Name = "BackupForm";
            Text = "BackupForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button backupButton;
        private TextBox textBox1;
        private Button button1;
        private TextBox textBox2;
    }
}