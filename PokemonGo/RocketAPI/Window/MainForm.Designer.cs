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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.startBotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.todoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pokemonToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.useLuckyEggToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceUnbanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            resources.ApplyResources(this.logTextBox, "logTextBox");
            this.logTextBox.BackColor = System.Drawing.Color.Black;
            this.logTextBox.ForeColor = System.Drawing.Color.White;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.TextChanged += new System.EventHandler(this.logTextBox_TextChanged);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.Name = "statusLabel";
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startBotToolStripMenuItem,
            this.todoToolStripMenuItem,
            this.pokemonToolStripMenuItem2,
            this.useLuckyEggToolStripMenuItem,
            this.forceUnbanToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // startBotToolStripMenuItem
            // 
            resources.ApplyResources(this.startBotToolStripMenuItem, "startBotToolStripMenuItem");
            this.startBotToolStripMenuItem.Name = "startBotToolStripMenuItem";
            this.startBotToolStripMenuItem.Click += new System.EventHandler(this.startBotToolStripMenuItem_Click);
            // 
            // todoToolStripMenuItem
            // 
            resources.ApplyResources(this.todoToolStripMenuItem, "todoToolStripMenuItem");
            this.todoToolStripMenuItem.Name = "todoToolStripMenuItem";
            this.todoToolStripMenuItem.Click += new System.EventHandler(this.todoToolStripMenuItem_Click);
            // 
            // pokemonToolStripMenuItem2
            // 
            resources.ApplyResources(this.pokemonToolStripMenuItem2, "pokemonToolStripMenuItem2");
            this.pokemonToolStripMenuItem2.Name = "pokemonToolStripMenuItem2";
            this.pokemonToolStripMenuItem2.Click += new System.EventHandler(this.pokemonToolStripMenuItem2_Click);
            // 
            // useLuckyEggToolStripMenuItem
            // 
            resources.ApplyResources(this.useLuckyEggToolStripMenuItem, "useLuckyEggToolStripMenuItem");
            this.useLuckyEggToolStripMenuItem.Name = "useLuckyEggToolStripMenuItem";
            this.useLuckyEggToolStripMenuItem.Click += new System.EventHandler(this.useLuckyEggToolStripMenuItem_Click);
            // 
            // forceUnbanToolStripMenuItem
            // 
            resources.ApplyResources(this.forceUnbanToolStripMenuItem, "forceUnbanToolStripMenuItem");
            this.forceUnbanToolStripMenuItem.Name = "forceUnbanToolStripMenuItem";
            this.forceUnbanToolStripMenuItem.Click += new System.EventHandler(this.forceUnbanToolStripMenuItem_Click);
            // 
            // showAllToolStripMenuItem
            // 
            resources.ApplyResources(this.showAllToolStripMenuItem, "showAllToolStripMenuItem");
            this.showAllToolStripMenuItem.Name = "showAllToolStripMenuItem";
            // 
            // showAllToolStripMenuItem1
            // 
            resources.ApplyResources(this.showAllToolStripMenuItem1, "showAllToolStripMenuItem1");
            this.showAllToolStripMenuItem1.Name = "showAllToolStripMenuItem1";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
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
        private System.Windows.Forms.ToolStripMenuItem startBotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pokemonToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem useLuckyEggToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceUnbanToolStripMenuItem;
    }
}

