namespace PokemonGo.RocketAPI.Window
{
    partial class MainForm
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
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.todoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startBotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pokemonToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.statsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.Color.Black;
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.ForeColor = System.Drawing.Color.White;
            this.logTextBox.Location = new System.Drawing.Point(0, 25);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.Size = new System.Drawing.Size(905, 430);
            this.logTextBox.TabIndex = 0;
            this.logTextBox.Text = "";
            this.logTextBox.TextChanged += new System.EventHandler(this.logTextBox_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 455);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(905, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.statusStrip1_ItemClicked);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(43, 17);
            this.statusLabel.Text = "Status";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.todoToolStripMenuItem,
            this.startBotToolStripMenuItem,
            this.pokemonToolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(905, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // todoToolStripMenuItem
            // 
            this.todoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.todoToolStripMenuItem.Name = "todoToolStripMenuItem";
            this.todoToolStripMenuItem.Size = new System.Drawing.Size(40, 21);
            this.todoToolStripMenuItem.Text = "Bot";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // startBotToolStripMenuItem
            // 
            this.startBotToolStripMenuItem.Name = "startBotToolStripMenuItem";
            this.startBotToolStripMenuItem.Size = new System.Drawing.Size(71, 21);
            this.startBotToolStripMenuItem.Text = "Start bot";
            this.startBotToolStripMenuItem.Click += new System.EventHandler(this.startBotToolStripMenuItem_Click);
            // 
            // pokemonToolStripMenuItem2
            // 
            this.pokemonToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAllToolStripMenuItem2,
            this.statsToolStripMenuItem});
            this.pokemonToolStripMenuItem2.Name = "pokemonToolStripMenuItem2";
            this.pokemonToolStripMenuItem2.Size = new System.Drawing.Size(55, 21);
            this.pokemonToolStripMenuItem2.Text = "Player";
            // 
            // showAllToolStripMenuItem2
            // 
            this.showAllToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAllToolStripMenuItem3});
            this.showAllToolStripMenuItem2.Name = "showAllToolStripMenuItem2";
            this.showAllToolStripMenuItem2.Size = new System.Drawing.Size(131, 22);
            this.showAllToolStripMenuItem2.Text = "Pokemon";
            // 
            // showAllToolStripMenuItem3
            // 
            this.showAllToolStripMenuItem3.Name = "showAllToolStripMenuItem3";
            this.showAllToolStripMenuItem3.Size = new System.Drawing.Size(105, 22);
            this.showAllToolStripMenuItem3.Text = "Mine";
            this.showAllToolStripMenuItem3.Click += new System.EventHandler(this.showAllToolStripMenuItem3_Click);
            // 
            // statsToolStripMenuItem
            // 
            this.statsToolStripMenuItem.Name = "statsToolStripMenuItem";
            this.statsToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.statsToolStripMenuItem.Text = "Stats";
            this.statsToolStripMenuItem.Click += new System.EventHandler(this.statsToolStripMenuItem_Click);
            // 
            // showAllToolStripMenuItem
            // 
            this.showAllToolStripMenuItem.Name = "showAllToolStripMenuItem";
            this.showAllToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // showAllToolStripMenuItem1
            // 
            this.showAllToolStripMenuItem1.Name = "showAllToolStripMenuItem1";
            this.showAllToolStripMenuItem1.Size = new System.Drawing.Size(32, 19);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 477);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "PokemonGo Rocket API";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox logTextBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem todoToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startBotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pokemonToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem statsToolStripMenuItem;
    }
}

