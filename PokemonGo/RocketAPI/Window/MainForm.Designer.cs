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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.startStopBotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.todoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useLuckyEggToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceUnbanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.pkmnName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pkmnCP = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pkmnAtkIV = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pkmnDefIV = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pkmnStaIV = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pkmnIV = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pkmnTransferButton = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pkmnPowerUpButton = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pkmnEvolveButton = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.largePokemonImageList = new System.Windows.Forms.ImageList(this.components);
            this.smallPokemonImageList = new System.Windows.Forms.ImageList(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Item = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.Color.Black;
            this.logTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logTextBox.ForeColor = System.Drawing.Color.White;
            this.logTextBox.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.logTextBox.Location = new System.Drawing.Point(0, 32);
            this.logTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(889, 449);
            this.logTextBox.TabIndex = 0;
            this.logTextBox.Text = "";
            this.logTextBox.TextChanged += new System.EventHandler(this.logTextBox_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 824);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1349, 25);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(49, 20);
            this.statusLabel.Text = "Status";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startStopBotToolStripMenuItem,
            this.todoToolStripMenuItem,
            this.useLuckyEggToolStripMenuItem,
            this.forceUnbanToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1349, 30);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // startStopBotToolStripMenuItem
            // 
            this.startStopBotToolStripMenuItem.Name = "startStopBotToolStripMenuItem";
            this.startStopBotToolStripMenuItem.Size = new System.Drawing.Size(96, 24);
            this.startStopBotToolStripMenuItem.Text = "▶ Start Bot";
            this.startStopBotToolStripMenuItem.Click += new System.EventHandler(this.startStopBotToolStripMenuItem_Click);
            // 
            // todoToolStripMenuItem
            // 
            this.todoToolStripMenuItem.Name = "todoToolStripMenuItem";
            this.todoToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.todoToolStripMenuItem.Text = "Settings";
            this.todoToolStripMenuItem.Click += new System.EventHandler(this.todoToolStripMenuItem_Click);
            // 
            // useLuckyEggToolStripMenuItem
            // 
            this.useLuckyEggToolStripMenuItem.Name = "useLuckyEggToolStripMenuItem";
            this.useLuckyEggToolStripMenuItem.Size = new System.Drawing.Size(115, 24);
            this.useLuckyEggToolStripMenuItem.Text = "Use Lucky Egg";
            this.useLuckyEggToolStripMenuItem.Click += new System.EventHandler(this.useLuckyEggToolStripMenuItem_Click);
            // 
            // forceUnbanToolStripMenuItem
            // 
            this.forceUnbanToolStripMenuItem.Name = "forceUnbanToolStripMenuItem";
            this.forceUnbanToolStripMenuItem.Size = new System.Drawing.Size(104, 24);
            this.forceUnbanToolStripMenuItem.Text = "Force Unban";
            this.forceUnbanToolStripMenuItem.Click += new System.EventHandler(this.forceUnbanToolStripMenuItem_Click);
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
            // gMapControl1
            // 
            this.gMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(899, 32);
            this.gMapControl1.Margin = new System.Windows.Forms.Padding(4);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 2;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(451, 451);
            this.gMapControl1.TabIndex = 23;
            this.gMapControl1.Zoom = 0D;
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.pkmnName);
            this.objectListView1.AllColumns.Add(this.pkmnCP);
            this.objectListView1.AllColumns.Add(this.pkmnAtkIV);
            this.objectListView1.AllColumns.Add(this.pkmnDefIV);
            this.objectListView1.AllColumns.Add(this.pkmnStaIV);
            this.objectListView1.AllColumns.Add(this.pkmnIV);
            this.objectListView1.AllColumns.Add(this.pkmnTransferButton);
            this.objectListView1.AllColumns.Add(this.pkmnPowerUpButton);
            this.objectListView1.AllColumns.Add(this.pkmnEvolveButton);
            this.objectListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectListView1.CellEditUseWholeCell = false;
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pkmnName,
            this.pkmnCP,
            this.pkmnAtkIV,
            this.pkmnDefIV,
            this.pkmnStaIV,
            this.pkmnIV,
            this.pkmnTransferButton,
            this.pkmnPowerUpButton,
            this.pkmnEvolveButton});
            this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.GridLines = true;
            this.objectListView1.LargeImageList = this.largePokemonImageList;
            this.objectListView1.Location = new System.Drawing.Point(0, 491);
            this.objectListView1.Margin = new System.Windows.Forms.Padding(4);
            this.objectListView1.MultiSelect = false;
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.RowHeight = 32;
            this.objectListView1.ShowGroups = false;
            this.objectListView1.Size = new System.Drawing.Size(889, 285);
            this.objectListView1.SmallImageList = this.smallPokemonImageList;
            this.objectListView1.TabIndex = 25;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            // 
            // pkmnName
            // 
            this.pkmnName.AspectName = "PokemonId";
            this.pkmnName.AspectToStringFormat = "";
            this.pkmnName.Text = "Name";
            this.pkmnName.Width = 120;
            // 
            // pkmnCP
            // 
            this.pkmnCP.AspectName = "Cp";
            this.pkmnCP.Text = "CP";
            this.pkmnCP.Width = 50;
            // 
            // pkmnAtkIV
            // 
            this.pkmnAtkIV.AspectName = "IndividualAttack";
            this.pkmnAtkIV.Text = "Atk IV";
            this.pkmnAtkIV.Width = 50;
            // 
            // pkmnDefIV
            // 
            this.pkmnDefIV.AspectName = "IndividualDefense";
            this.pkmnDefIV.Text = "Def IV";
            this.pkmnDefIV.Width = 50;
            // 
            // pkmnStaIV
            // 
            this.pkmnStaIV.AspectName = "IndividualStamina";
            this.pkmnStaIV.Text = "Sta IV";
            this.pkmnStaIV.Width = 50;
            // 
            // pkmnIV
            // 
            this.pkmnIV.AspectName = "GetIV";
            this.pkmnIV.AspectToStringFormat = "{0:P2}";
            this.pkmnIV.Text = "IV %";
            this.pkmnIV.Width = 50;
            // 
            // pkmnTransferButton
            // 
            this.pkmnTransferButton.AspectName = "Id";
            this.pkmnTransferButton.AspectToStringFormat = "Transfer";
            this.pkmnTransferButton.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.CellBounds;
            this.pkmnTransferButton.IsButton = true;
            this.pkmnTransferButton.Text = "";
            // 
            // pkmnPowerUpButton
            // 
            this.pkmnPowerUpButton.AspectName = "Id";
            this.pkmnPowerUpButton.AspectToStringFormat = "Power Up";
            this.pkmnPowerUpButton.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.CellBounds;
            this.pkmnPowerUpButton.IsButton = true;
            this.pkmnPowerUpButton.Text = "";
            // 
            // pkmnEvolveButton
            // 
            this.pkmnEvolveButton.AspectName = "Id";
            this.pkmnEvolveButton.AspectToStringFormat = "Evolve";
            this.pkmnEvolveButton.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.CellBounds;
            this.pkmnEvolveButton.IsButton = true;
            this.pkmnEvolveButton.Text = "";
            // 
            // largePokemonImageList
            // 
            this.largePokemonImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.largePokemonImageList.ImageSize = new System.Drawing.Size(96, 96);
            this.largePokemonImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // smallPokemonImageList
            // 
            this.smallPokemonImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.smallPokemonImageList.ImageSize = new System.Drawing.Size(32, 32);
            this.smallPokemonImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(0, 771);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(891, 35);
            this.button1.TabIndex = 26;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(899, 491);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(451, 315);
            this.tabControl1.TabIndex = 27;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(443, 284);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Item List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Item,
            this.Count});
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(443, 287);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // Item
            // 
            this.Item.HeaderText = "Item";
            this.Item.Name = "Item";
            this.Item.ReadOnly = true;
            // 
            // Count
            // 
            this.Count.HeaderText = "Count";
            this.Count.Name = "Count";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(443, 284);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = ":3";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1349, 849);
            this.Controls.Add(this.gMapControl1);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.objectListView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabControl1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PokemonGo Rocket API";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox logTextBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem todoToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripMenuItem startStopBotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem useLuckyEggToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forceUnbanToolStripMenuItem;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private BrightIdeasSoftware.ObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn pkmnName;
        private BrightIdeasSoftware.OLVColumn pkmnCP;
        private BrightIdeasSoftware.OLVColumn pkmnAtkIV;
        private BrightIdeasSoftware.OLVColumn pkmnDefIV;
        private BrightIdeasSoftware.OLVColumn pkmnStaIV;
        private BrightIdeasSoftware.OLVColumn pkmnIV;
        private BrightIdeasSoftware.OLVColumn pkmnTransferButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ImageList smallPokemonImageList;
        private System.Windows.Forms.ImageList largePokemonImageList;
        private BrightIdeasSoftware.OLVColumn pkmnPowerUpButton;
        private BrightIdeasSoftware.OLVColumn pkmnEvolveButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Item;
        private System.Windows.Forms.DataGridViewTextBoxColumn Count;
    }
}
