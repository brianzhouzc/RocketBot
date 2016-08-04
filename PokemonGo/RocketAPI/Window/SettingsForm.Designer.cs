namespace PokemonGo.RocketAPI.Window
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.authTypeLabel = new System.Windows.Forms.Label();
            this.authTypeCb = new System.Windows.Forms.ComboBox();
            this.UserLabel = new System.Windows.Forms.Label();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.latLabel = new System.Windows.Forms.Label();
            this.longiLabel = new System.Windows.Forms.Label();
            this.RazzberryModeLabel = new System.Windows.Forms.Label();
            this.TransferTypeLabel = new System.Windows.Forms.Label();
            this.EvolvePokemonLabel = new System.Windows.Forms.Label();
            this.CPThresholdLabel = new System.Windows.Forms.Label();
            this.RazzberrySettingLabel = new System.Windows.Forms.Label();
            this.UserLoginBox = new System.Windows.Forms.TextBox();
            this.UserPasswordBox = new System.Windows.Forms.TextBox();
            this.latitudeText = new System.Windows.Forms.TextBox();
            this.longitudeText = new System.Windows.Forms.TextBox();
            this.razzmodeCb = new System.Windows.Forms.ComboBox();
            this.razzSettingText = new System.Windows.Forms.TextBox();
            this.transferTypeCb = new System.Windows.Forms.ComboBox();
            this.transferCpThresText = new System.Windows.Forms.TextBox();
            this.evolveAllChk = new System.Windows.Forms.CheckBox();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.LocationGroupBox = new System.Windows.Forms.GroupBox();
            this.FindAdressButton = new System.Windows.Forms.Button();
            this.AdressBox = new System.Windows.Forms.TextBox();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.TravelSpeedBox = new System.Windows.Forms.TextBox();
            this.CatchPokemonBox = new System.Windows.Forms.CheckBox();
            this.CatchPokemonLabel = new System.Windows.Forms.Label();
            this.transferIVThresText = new System.Windows.Forms.TextBox();
            this.TravelSpeedLabel = new System.Windows.Forms.Label();
            this.IVThresholdLabel = new System.Windows.Forms.Label();
            this.MainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.LocationControlsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SettingsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.Spacer1Label = new System.Windows.Forms.Label();
            this.Spacer2Label = new System.Windows.Forms.Label();
            this.saveBtn = new System.Windows.Forms.Button();
            this.LocationGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.MainTableLayoutPanel.SuspendLayout();
            this.LocationControlsTableLayoutPanel.SuspendLayout();
            this.SettingsTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // authTypeLabel
            // 
            this.authTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.authTypeLabel.AutoSize = true;
            this.authTypeLabel.Location = new System.Drawing.Point(3, 0);
            this.authTypeLabel.Name = "authTypeLabel";
            this.authTypeLabel.Size = new System.Drawing.Size(102, 27);
            this.authTypeLabel.TabIndex = 0;
            this.authTypeLabel.Text = "Login Type:";
            this.authTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.authTypeLabel.Click += new System.EventHandler(this.authTypeLabel_Click);
            // 
            // authTypeCb
            // 
            this.authTypeCb.FormattingEnabled = true;
            this.authTypeCb.Items.AddRange(new object[] {
            "google",
            "Ptc"});
            this.authTypeCb.Location = new System.Drawing.Point(111, 3);
            this.authTypeCb.Name = "authTypeCb";
            this.authTypeCb.Size = new System.Drawing.Size(136, 21);
            this.authTypeCb.TabIndex = 1;
            this.authTypeCb.SelectedIndexChanged += new System.EventHandler(this.authTypeCb_SelectedIndexChanged);
            // 
            // UserLabel
            // 
            this.UserLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UserLabel.AutoSize = true;
            this.UserLabel.Location = new System.Drawing.Point(3, 27);
            this.UserLabel.Name = "UserLabel";
            this.UserLabel.Size = new System.Drawing.Size(102, 26);
            this.UserLabel.TabIndex = 2;
            this.UserLabel.Text = "Username:";
            this.UserLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(3, 53);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(102, 26);
            this.PasswordLabel.TabIndex = 3;
            this.PasswordLabel.Text = "Password:";
            this.PasswordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // latLabel
            // 
            this.latLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.latLabel.AutoSize = true;
            this.latLabel.Location = new System.Drawing.Point(3, 79);
            this.latLabel.Name = "latLabel";
            this.latLabel.Size = new System.Drawing.Size(102, 26);
            this.latLabel.TabIndex = 4;
            this.latLabel.Text = "Latitude:";
            this.latLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // longiLabel
            // 
            this.longiLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.longiLabel.AutoSize = true;
            this.longiLabel.Location = new System.Drawing.Point(3, 105);
            this.longiLabel.Name = "longiLabel";
            this.longiLabel.Size = new System.Drawing.Size(102, 26);
            this.longiLabel.TabIndex = 5;
            this.longiLabel.Text = "Longitude:";
            this.longiLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RazzberryModeLabel
            // 
            this.RazzberryModeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RazzberryModeLabel.AutoSize = true;
            this.RazzberryModeLabel.Location = new System.Drawing.Point(3, 144);
            this.RazzberryModeLabel.Name = "RazzberryModeLabel";
            this.RazzberryModeLabel.Size = new System.Drawing.Size(102, 27);
            this.RazzberryModeLabel.TabIndex = 6;
            this.RazzberryModeLabel.Text = "Razzberry Mode:";
            this.RazzberryModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TransferTypeLabel
            // 
            this.TransferTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TransferTypeLabel.AutoSize = true;
            this.TransferTypeLabel.Location = new System.Drawing.Point(3, 210);
            this.TransferTypeLabel.Name = "TransferTypeLabel";
            this.TransferTypeLabel.Size = new System.Drawing.Size(102, 27);
            this.TransferTypeLabel.TabIndex = 7;
            this.TransferTypeLabel.Text = "Transfer Type:";
            this.TransferTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // EvolvePokemonLabel
            // 
            this.EvolvePokemonLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EvolvePokemonLabel.AutoSize = true;
            this.EvolvePokemonLabel.Location = new System.Drawing.Point(3, 335);
            this.EvolvePokemonLabel.Name = "EvolvePokemonLabel";
            this.EvolvePokemonLabel.Size = new System.Drawing.Size(102, 20);
            this.EvolvePokemonLabel.TabIndex = 8;
            this.EvolvePokemonLabel.Text = "Evolve Pokemon:";
            this.EvolvePokemonLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EvolvePokemonLabel.Click += new System.EventHandler(this.label3_Click);
            // 
            // CPThresholdLabel
            // 
            this.CPThresholdLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CPThresholdLabel.AutoSize = true;
            this.CPThresholdLabel.Location = new System.Drawing.Point(3, 263);
            this.CPThresholdLabel.Name = "CPThresholdLabel";
            this.CPThresholdLabel.Size = new System.Drawing.Size(102, 26);
            this.CPThresholdLabel.TabIndex = 9;
            this.CPThresholdLabel.Text = "CP Threshold:";
            this.CPThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RazzberrySettingLabel
            // 
            this.RazzberrySettingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RazzberrySettingLabel.AutoSize = true;
            this.RazzberrySettingLabel.Location = new System.Drawing.Point(3, 171);
            this.RazzberrySettingLabel.Name = "RazzberrySettingLabel";
            this.RazzberrySettingLabel.Size = new System.Drawing.Size(102, 26);
            this.RazzberrySettingLabel.TabIndex = 10;
            this.RazzberrySettingLabel.Text = "Razzberry Setting:";
            this.RazzberrySettingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UserLoginBox
            // 
            this.UserLoginBox.Location = new System.Drawing.Point(111, 30);
            this.UserLoginBox.MinimumSize = new System.Drawing.Size(136, 20);
            this.UserLoginBox.Name = "UserLoginBox";
            this.UserLoginBox.Size = new System.Drawing.Size(136, 20);
            this.UserLoginBox.TabIndex = 11;
            // 
            // UserPasswordBox
            // 
            this.UserPasswordBox.Location = new System.Drawing.Point(111, 56);
            this.UserPasswordBox.Name = "UserPasswordBox";
            this.UserPasswordBox.Size = new System.Drawing.Size(136, 20);
            this.UserPasswordBox.TabIndex = 12;
            // 
            // latitudeText
            // 
            this.latitudeText.Location = new System.Drawing.Point(111, 82);
            this.latitudeText.Name = "latitudeText";
            this.latitudeText.ReadOnly = true;
            this.latitudeText.Size = new System.Drawing.Size(100, 20);
            this.latitudeText.TabIndex = 13;
            // 
            // longitudeText
            // 
            this.longitudeText.Location = new System.Drawing.Point(111, 108);
            this.longitudeText.Name = "longitudeText";
            this.longitudeText.ReadOnly = true;
            this.longitudeText.Size = new System.Drawing.Size(100, 20);
            this.longitudeText.TabIndex = 14;
            // 
            // razzmodeCb
            // 
            this.razzmodeCb.FormattingEnabled = true;
            this.razzmodeCb.Items.AddRange(new object[] {
            "probability",
            "cp"});
            this.razzmodeCb.Location = new System.Drawing.Point(111, 147);
            this.razzmodeCb.Name = "razzmodeCb";
            this.razzmodeCb.Size = new System.Drawing.Size(100, 21);
            this.razzmodeCb.TabIndex = 15;
            // 
            // razzSettingText
            // 
            this.razzSettingText.Location = new System.Drawing.Point(111, 174);
            this.razzSettingText.Name = "razzSettingText";
            this.razzSettingText.Size = new System.Drawing.Size(100, 20);
            this.razzSettingText.TabIndex = 16;
            this.razzSettingText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.razzSettingText_KeyPress);
            // 
            // transferTypeCb
            // 
            this.transferTypeCb.FormattingEnabled = true;
            this.transferTypeCb.Items.AddRange(new object[] {
            "None",
            "CP/IV Duplicate",
            "CP Duplicate",
            "IV Duplicate",
            "CP",
            "IV",
            "Leave Strongest",
            "All"});
            this.transferTypeCb.Location = new System.Drawing.Point(111, 213);
            this.transferTypeCb.Name = "transferTypeCb";
            this.transferTypeCb.Size = new System.Drawing.Size(100, 21);
            this.transferTypeCb.TabIndex = 17;
            this.transferTypeCb.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.transferTypeCb_DrawItem);
            this.transferTypeCb.SelectedIndexChanged += new System.EventHandler(this.transferTypeCb_SelectedIndexChanged);
            this.transferTypeCb.DropDownClosed += new System.EventHandler(this.transferTypeCb_DropDownClosed);
            // 
            // transferCpThresText
            // 
            this.transferCpThresText.Location = new System.Drawing.Point(111, 266);
            this.transferCpThresText.Name = "transferCpThresText";
            this.transferCpThresText.Size = new System.Drawing.Size(100, 20);
            this.transferCpThresText.TabIndex = 18;
            this.transferCpThresText.TextChanged += new System.EventHandler(this.transferCpThresText_TextChanged);
            // 
            // evolveAllChk
            // 
            this.evolveAllChk.AutoSize = true;
            this.evolveAllChk.Location = new System.Drawing.Point(111, 338);
            this.evolveAllChk.Name = "evolveAllChk";
            this.evolveAllChk.Size = new System.Drawing.Size(15, 14);
            this.evolveAllChk.TabIndex = 19;
            this.evolveAllChk.UseVisualStyleBackColor = true;
            this.evolveAllChk.CheckedChanged += new System.EventHandler(this.evolveAllChk_CheckedChanged);
            // 
            // gMapControl1
            // 
            this.gMapControl1.BackColor = System.Drawing.SystemColors.Info;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.LocationControlsTableLayoutPanel.SetColumnSpan(this.gMapControl1, 2);
            this.gMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(3, 3);
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
            this.gMapControl1.Size = new System.Drawing.Size(414, 354);
            this.gMapControl1.TabIndex = 22;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            this.gMapControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseClick);
            // 
            // LocationGroupBox
            // 
            this.LocationGroupBox.Controls.Add(this.trackBar);
            this.LocationGroupBox.Controls.Add(this.LocationControlsTableLayoutPanel);
            this.LocationGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LocationGroupBox.Location = new System.Drawing.Point(275, 27);
            this.LocationGroupBox.Name = "LocationGroupBox";
            this.LocationGroupBox.Size = new System.Drawing.Size(426, 411);
            this.LocationGroupBox.TabIndex = 25;
            this.LocationGroupBox.TabStop = false;
            this.LocationGroupBox.Text = "Location";
            // 
            // FindAdressButton
            // 
            this.FindAdressButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FindAdressButton.Location = new System.Drawing.Point(298, 363);
            this.FindAdressButton.Name = "FindAdressButton";
            this.FindAdressButton.Size = new System.Drawing.Size(119, 26);
            this.FindAdressButton.TabIndex = 25;
            this.FindAdressButton.Text = "Find Location";
            this.FindAdressButton.UseVisualStyleBackColor = true;
            this.FindAdressButton.Click += new System.EventHandler(this.FindAdressButton_Click_1);
            // 
            // AdressBox
            // 
            this.AdressBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AdressBox.ForeColor = System.Drawing.Color.Gray;
            this.AdressBox.Location = new System.Drawing.Point(3, 363);
            this.AdressBox.Name = "AdressBox";
            this.AdressBox.Size = new System.Drawing.Size(289, 20);
            this.AdressBox.TabIndex = 25;
            this.AdressBox.Text = "Enter an address or a coordinate";
            this.AdressBox.TextChanged += new System.EventHandler(this.AdressBox_TextChanged);
            this.AdressBox.Enter += new System.EventHandler(this.AdressBox_Enter);
            this.AdressBox.Leave += new System.EventHandler(this.AdressBox_Leave);
            // 
            // trackBar
            // 
            this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar.BackColor = System.Drawing.SystemColors.Info;
            this.trackBar.Location = new System.Drawing.Point(375, 19);
            this.trackBar.Name = "trackBar";
            this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar.Size = new System.Drawing.Size(45, 104);
            this.trackBar.TabIndex = 25;
            this.trackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // TravelSpeedBox
            // 
            this.TravelSpeedBox.Location = new System.Drawing.Point(111, 292);
            this.TravelSpeedBox.Name = "TravelSpeedBox";
            this.TravelSpeedBox.Size = new System.Drawing.Size(100, 20);
            this.TravelSpeedBox.TabIndex = 22;
            this.TravelSpeedBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.TravelSpeedBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TravelSpeedBox_KeyPress);
            // 
            // CatchPokemonBox
            // 
            this.CatchPokemonBox.AutoSize = true;
            this.CatchPokemonBox.Location = new System.Drawing.Point(111, 318);
            this.CatchPokemonBox.Name = "CatchPokemonBox";
            this.CatchPokemonBox.Size = new System.Drawing.Size(15, 14);
            this.CatchPokemonBox.TabIndex = 26;
            this.CatchPokemonBox.UseVisualStyleBackColor = true;
            this.CatchPokemonBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // CatchPokemonLabel
            // 
            this.CatchPokemonLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CatchPokemonLabel.AutoSize = true;
            this.CatchPokemonLabel.Location = new System.Drawing.Point(3, 315);
            this.CatchPokemonLabel.Name = "CatchPokemonLabel";
            this.CatchPokemonLabel.Size = new System.Drawing.Size(102, 20);
            this.CatchPokemonLabel.TabIndex = 25;
            this.CatchPokemonLabel.Text = "Catch Pokemon:";
            this.CatchPokemonLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CatchPokemonLabel.Click += new System.EventHandler(this.label7_Click);
            // 
            // transferIVThresText
            // 
            this.transferIVThresText.Location = new System.Drawing.Point(111, 240);
            this.transferIVThresText.Name = "transferIVThresText";
            this.transferIVThresText.Size = new System.Drawing.Size(100, 20);
            this.transferIVThresText.TabIndex = 24;
            this.transferIVThresText.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // TravelSpeedLabel
            // 
            this.TravelSpeedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TravelSpeedLabel.AutoSize = true;
            this.TravelSpeedLabel.Location = new System.Drawing.Point(3, 289);
            this.TravelSpeedLabel.Name = "TravelSpeedLabel";
            this.TravelSpeedLabel.Size = new System.Drawing.Size(102, 26);
            this.TravelSpeedLabel.TabIndex = 23;
            this.TravelSpeedLabel.Text = "Travel Speed km/h:";
            this.TravelSpeedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // IVThresholdLabel
            // 
            this.IVThresholdLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IVThresholdLabel.AutoSize = true;
            this.IVThresholdLabel.Location = new System.Drawing.Point(3, 237);
            this.IVThresholdLabel.Name = "IVThresholdLabel";
            this.IVThresholdLabel.Size = new System.Drawing.Size(102, 26);
            this.IVThresholdLabel.TabIndex = 21;
            this.IVThresholdLabel.Text = "IV Threshold:";
            this.IVThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.IVThresholdLabel.Click += new System.EventHandler(this.label6_Click);
            // 
            // MainTableLayoutPanel
            // 
            this.MainTableLayoutPanel.ColumnCount = 2;
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.Controls.Add(this.SettingsTableLayoutPanel, 0, 0);
            this.MainTableLayoutPanel.Controls.Add(this.LocationGroupBox, 1, 0);
            this.MainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayoutPanel.Location = new System.Drawing.Point(9, 9);
            this.MainTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            this.MainTableLayoutPanel.RowCount = 1;
            this.MainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayoutPanel.Size = new System.Drawing.Size(704, 441);
            this.MainTableLayoutPanel.TabIndex = 26;
            // 
            // LocationControlsTableLayoutPanel
            // 
            this.LocationControlsTableLayoutPanel.ColumnCount = 2;
            this.LocationControlsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LocationControlsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.LocationControlsTableLayoutPanel.Controls.Add(this.gMapControl1, 0, 0);
            this.LocationControlsTableLayoutPanel.Controls.Add(this.FindAdressButton, 1, 1);
            this.LocationControlsTableLayoutPanel.Controls.Add(this.AdressBox, 0, 1);
            this.LocationControlsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LocationControlsTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.LocationControlsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.LocationControlsTableLayoutPanel.Name = "LocationControlsTableLayoutPanel";
            this.LocationControlsTableLayoutPanel.RowCount = 2;
            this.LocationControlsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LocationControlsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.LocationControlsTableLayoutPanel.Size = new System.Drawing.Size(420, 392);
            this.LocationControlsTableLayoutPanel.TabIndex = 26;
            // 
            // SettingsTableLayoutPanel
            // 
            this.SettingsTableLayoutPanel.ColumnCount = 2;
            this.SettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.SettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.SettingsTableLayoutPanel.Controls.Add(this.evolveAllChk, 1, 15);
            this.SettingsTableLayoutPanel.Controls.Add(this.EvolvePokemonLabel, 0, 15);
            this.SettingsTableLayoutPanel.Controls.Add(this.CatchPokemonBox, 1, 14);
            this.SettingsTableLayoutPanel.Controls.Add(this.TravelSpeedBox, 1, 12);
            this.SettingsTableLayoutPanel.Controls.Add(this.CatchPokemonLabel, 0, 14);
            this.SettingsTableLayoutPanel.Controls.Add(this.authTypeLabel, 0, 0);
            this.SettingsTableLayoutPanel.Controls.Add(this.authTypeCb, 1, 0);
            this.SettingsTableLayoutPanel.Controls.Add(this.UserLabel, 0, 1);
            this.SettingsTableLayoutPanel.Controls.Add(this.TravelSpeedLabel, 0, 12);
            this.SettingsTableLayoutPanel.Controls.Add(this.transferIVThresText, 1, 10);
            this.SettingsTableLayoutPanel.Controls.Add(this.UserLoginBox, 1, 1);
            this.SettingsTableLayoutPanel.Controls.Add(this.PasswordLabel, 0, 2);
            this.SettingsTableLayoutPanel.Controls.Add(this.transferCpThresText, 1, 11);
            this.SettingsTableLayoutPanel.Controls.Add(this.IVThresholdLabel, 0, 10);
            this.SettingsTableLayoutPanel.Controls.Add(this.UserPasswordBox, 1, 2);
            this.SettingsTableLayoutPanel.Controls.Add(this.CPThresholdLabel, 0, 11);
            this.SettingsTableLayoutPanel.Controls.Add(this.latLabel, 0, 3);
            this.SettingsTableLayoutPanel.Controls.Add(this.latitudeText, 1, 3);
            this.SettingsTableLayoutPanel.Controls.Add(this.longiLabel, 0, 4);
            this.SettingsTableLayoutPanel.Controls.Add(this.transferTypeCb, 1, 9);
            this.SettingsTableLayoutPanel.Controls.Add(this.RazzberryModeLabel, 0, 6);
            this.SettingsTableLayoutPanel.Controls.Add(this.TransferTypeLabel, 0, 9);
            this.SettingsTableLayoutPanel.Controls.Add(this.longitudeText, 1, 4);
            this.SettingsTableLayoutPanel.Controls.Add(this.razzSettingText, 1, 7);
            this.SettingsTableLayoutPanel.Controls.Add(this.razzmodeCb, 1, 6);
            this.SettingsTableLayoutPanel.Controls.Add(this.RazzberrySettingLabel, 0, 7);
            this.SettingsTableLayoutPanel.Controls.Add(this.Spacer1Label, 0, 5);
            this.SettingsTableLayoutPanel.Controls.Add(this.Spacer2Label, 0, 8);
            this.SettingsTableLayoutPanel.Controls.Add(this.saveBtn, 0, 17);
            this.SettingsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.SettingsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SettingsTableLayoutPanel.Name = "SettingsTableLayoutPanel";
            this.SettingsTableLayoutPanel.RowCount = 18;
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SettingsTableLayoutPanel.Size = new System.Drawing.Size(250, 441);
            this.SettingsTableLayoutPanel.TabIndex = 26;
            // 
            // Spacer1Label
            // 
            this.Spacer1Label.AutoSize = true;
            this.Spacer1Label.Location = new System.Drawing.Point(3, 131);
            this.Spacer1Label.Name = "Spacer1Label";
            this.Spacer1Label.Size = new System.Drawing.Size(10, 13);
            this.Spacer1Label.TabIndex = 5;
            this.Spacer1Label.Text = " ";
            // 
            // Spacer2Label
            // 
            this.Spacer2Label.AutoSize = true;
            this.Spacer2Label.Location = new System.Drawing.Point(3, 197);
            this.Spacer2Label.Name = "Spacer2Label";
            this.Spacer2Label.Size = new System.Drawing.Size(10, 13);
            this.Spacer2Label.TabIndex = 5;
            this.Spacer2Label.Text = " ";
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingsTableLayoutPanel.SetColumnSpan(this.saveBtn, 2);
            this.saveBtn.Location = new System.Drawing.Point(3, 358);
            this.saveBtn.MinimumSize = new System.Drawing.Size(218, 39);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(244, 39);
            this.saveBtn.TabIndex = 20;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 459);
            this.Controls.Add(this.MainTableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(630, 359);
            this.Name = "SettingsForm";
            this.Padding = new System.Windows.Forms.Padding(9, 9, 9, 9);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.LocationGroupBox.ResumeLayout(false);
            this.LocationGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.MainTableLayoutPanel.ResumeLayout(false);
            this.LocationControlsTableLayoutPanel.ResumeLayout(false);
            this.LocationControlsTableLayoutPanel.PerformLayout();
            this.SettingsTableLayoutPanel.ResumeLayout(false);
            this.SettingsTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label authTypeLabel;
        private System.Windows.Forms.ComboBox authTypeCb;
        private System.Windows.Forms.Label UserLabel;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.Label latLabel;
        private System.Windows.Forms.Label longiLabel;
        private System.Windows.Forms.Label RazzberryModeLabel;
        private System.Windows.Forms.Label TransferTypeLabel;
        private System.Windows.Forms.Label EvolvePokemonLabel;
        private System.Windows.Forms.Label CPThresholdLabel;
        private System.Windows.Forms.Label RazzberrySettingLabel;
        private System.Windows.Forms.TextBox UserLoginBox;
        private System.Windows.Forms.TextBox UserPasswordBox;
        private System.Windows.Forms.TextBox latitudeText;
        private System.Windows.Forms.TextBox longitudeText;
        private System.Windows.Forms.ComboBox razzmodeCb;
        private System.Windows.Forms.TextBox razzSettingText;
        private System.Windows.Forms.ComboBox transferTypeCb;
        private System.Windows.Forms.TextBox transferCpThresText;
        private System.Windows.Forms.CheckBox evolveAllChk;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.GroupBox LocationGroupBox;
        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.TextBox TravelSpeedBox;
        private System.Windows.Forms.Label IVThresholdLabel;
        private System.Windows.Forms.Label TravelSpeedLabel;
        private System.Windows.Forms.TextBox transferIVThresText;
        private System.Windows.Forms.TextBox AdressBox;
        private System.Windows.Forms.Button FindAdressButton;
        private System.Windows.Forms.CheckBox CatchPokemonBox;
        private System.Windows.Forms.Label CatchPokemonLabel;
        private System.Windows.Forms.TableLayoutPanel LocationControlsTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel MainTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel SettingsTableLayoutPanel;
        private System.Windows.Forms.Label Spacer1Label;
        private System.Windows.Forms.Label Spacer2Label;
        private System.Windows.Forms.Button saveBtn;
    }
}
