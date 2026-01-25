namespace RetroVault
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            systemComboBox = new ComboBox();
            label2 = new Label();
            catComboBox = new ComboBox();
            label3 = new Label();
            searchBox = new TextBox();
            searchButton = new Button();
            newButton = new Button();
            vaultPanel = new FlowLayoutPanel();
            configButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 23);
            label1.Name = "label1";
            label1.Size = new Size(85, 32);
            label1.TabIndex = 0;
            label1.Text = "Search";
            // 
            // systemComboBox
            // 
            systemComboBox.FormattingEnabled = true;
            systemComboBox.Location = new Point(562, 17);
            systemComboBox.Name = "systemComboBox";
            systemComboBox.Size = new Size(306, 40);
            systemComboBox.TabIndex = 1;
            systemComboBox.SelectedIndexChanged += systemComboBox_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(466, 20);
            label2.Name = "label2";
            label2.Size = new Size(90, 32);
            label2.TabIndex = 2;
            label2.Text = "System";
            // 
            // catComboBox
            // 
            catComboBox.FormattingEnabled = true;
            catComboBox.Location = new Point(1038, 17);
            catComboBox.Name = "catComboBox";
            catComboBox.Size = new Size(328, 40);
            catComboBox.TabIndex = 2;
            catComboBox.SelectedIndexChanged += catComboBox_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(922, 17);
            label3.Name = "label3";
            label3.Size = new Size(110, 32);
            label3.TabIndex = 4;
            label3.Text = "Category";
            // 
            // searchBox
            // 
            searchBox.Location = new Point(125, 20);
            searchBox.Name = "searchBox";
            searchBox.Size = new Size(302, 39);
            searchBox.TabIndex = 0;
            searchBox.TextChanged += searchBox_TextChanged;
            // 
            // searchButton
            // 
            searchButton.Location = new Point(43, 99);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(150, 46);
            searchButton.TabIndex = 6;
            searchButton.Text = "Search";
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += searchButton_ClickAsync;
            // 
            // newButton
            // 
            newButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            newButton.Location = new Point(1067, 99);
            newButton.Name = "newButton";
            newButton.Size = new Size(150, 46);
            newButton.TabIndex = 7;
            newButton.Text = "New Item";
            newButton.UseVisualStyleBackColor = true;
            newButton.Click += newButton_Click;
            // 
            // vaultPanel
            // 
            vaultPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            vaultPanel.AutoScroll = true;
            vaultPanel.FlowDirection = FlowDirection.TopDown;
            vaultPanel.Location = new Point(34, 170);
            vaultPanel.Name = "vaultPanel";
            vaultPanel.Size = new Size(1339, 989);
            vaultPanel.TabIndex = 8;
            // 
            // configButton
            // 
            configButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            configButton.Location = new Point(1223, 99);
            configButton.Name = "configButton";
            configButton.Size = new Size(150, 46);
            configButton.TabIndex = 9;
            configButton.Text = "Config";
            configButton.UseVisualStyleBackColor = true;
            configButton.Click += configButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1419, 1187);
            Controls.Add(configButton);
            Controls.Add(vaultPanel);
            Controls.Add(newButton);
            Controls.Add(searchButton);
            Controls.Add(searchBox);
            Controls.Add(label3);
            Controls.Add(catComboBox);
            Controls.Add(label2);
            Controls.Add(systemComboBox);
            Controls.Add(label1);
            Name = "Form1";
            Text = "TheRetroVault";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox systemComboBox;
        private Label label2;
        private ComboBox catComboBox;
        private Label label3;
        private TextBox searchBox;
        private Button searchButton;
        private Button newButton;
        private FlowLayoutPanel vaultPanel;
        private Button configButton;
    }
}
