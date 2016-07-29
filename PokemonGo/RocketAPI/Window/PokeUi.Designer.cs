namespace PokemonGo.RocketAPI.Window
{
    partial class PokeUi
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokeUi));
            this.btnReload = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEvolve = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReload.Location = new System.Drawing.Point(12, 463);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(295, 26);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.button1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(117, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(116, 22);
            this.toolStripMenuItem1.Text = "Transfer";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // btnEvolve
            // 
            this.btnEvolve.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEvolve.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEvolve.Location = new System.Drawing.Point(313, 463);
            this.btnEvolve.Name = "btnEvolve";
            this.btnEvolve.Size = new System.Drawing.Size(295, 26);
            this.btnEvolve.TabIndex = 4;
            this.btnEvolve.Text = "Evolve Selected";
            this.btnEvolve.UseVisualStyleBackColor = true;
            this.btnEvolve.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransfer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnTransfer.Location = new System.Drawing.Point(313, 495);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(295, 26);
            this.btnTransfer.TabIndex = 5;
            this.btnTransfer.Text = "Transfer Selected";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpgrade.Location = new System.Drawing.Point(12, 495);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(295, 26);
            this.btnUpgrade.TabIndex = 6;
            this.btnUpgrade.Text = "Upgrade Selected";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(596, 445);
            this.dataGridView1.TabIndex = 7;
            // 
            // PokeUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 540);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.btnEvolve);
            this.Controls.Add(this.btnReload);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PokeUi";
            this.Text = "PokeUi";
            this.Load += new System.EventHandler(this.PokeUi_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Button btnEvolve;
        private System.Windows.Forms.Button btnTransfer;
		private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}