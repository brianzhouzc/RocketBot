using System.Drawing;
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.UserLoginBox = new System.Windows.Forms.TextBox();
            this.UserPasswordBox = new System.Windows.Forms.TextBox();
            this.latitudeText = new System.Windows.Forms.TextBox();
            this.longitudeText = new System.Windows.Forms.TextBox();
            this.razzmodeCb = new System.Windows.Forms.ComboBox();
            this.razzSettingText = new System.Windows.Forms.TextBox();
            this.transferTypeCb = new System.Windows.Forms.ComboBox();
            this.transferCpThresText = new System.Windows.Forms.TextBox();
            this.evolveAllChk = new System.Windows.Forms.CheckBox();
            this.saveBtn = new System.Windows.Forms.Button();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.FindAdressButton = new System.Windows.Forms.Button();
            this.AdressBox = new System.Windows.Forms.TextBox();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TravelSpeedBox = new System.Windows.Forms.TextBox();
            this.CatchPokemonBox = new System.Windows.Forms.CheckBox();
            this.CatchPokemonText = new System.Windows.Forms.Label();
            this.transferIVThresText = new System.Windows.Forms.TextBox();
            this.TravelSpeedText = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabLocation = new System.Windows.Forms.TabPage();
            this.tabPokemon = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbSelectAllEvolve = new System.Windows.Forms.CheckBox();
            this.clbEvolve = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbSelectAllCatch = new System.Windows.Forms.CheckBox();
            this.clbCatch = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbSelectAllTransfer = new System.Windows.Forms.CheckBox();
            this.clbTransfer = new System.Windows.Forms.CheckedListBox();
            this.tabItems = new System.Windows.Forms.TabPage();
            this.tabDevice = new System.Windows.Forms.TabPage();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.RandomIDBtn = new System.Windows.Forms.Button();
            this.deviceTypeCb = new System.Windows.Forms.ComboBox();
            this.RandomDeviceBtn = new System.Windows.Forms.Button();
            this.FirmwareFingerprintTb = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.FirmwareTypeTb = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.FirmwareTagsTb = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.FirmwareBrandTb = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.HardwareModelTb = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.HardwareManufacturerTb = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.DeviceModelBootTb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.DeviceModelIdentifierTb = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.DeviceModelTb = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.DeviceBrandTb = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.AndroidBootloaderTb = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.AndroidBoardNameTb = new System.Windows.Forms.TextBox();
            this.BoardName = new System.Windows.Forms.Label();
            this.DeviceIdTb = new System.Windows.Forms.TextBox();
            this.deviceIdlb = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.flpItems = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabLocation.SuspendLayout();
            this.tabPokemon.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabItems.SuspendLayout();
            this.tabDevice.SuspendLayout();
            this.SuspendLayout();
            // 
            // authTypeLabel
            // 
            this.authTypeLabel.AutoSize = true;
            this.authTypeLabel.Location = new System.Drawing.Point(3, 9);
            this.authTypeLabel.Name = "authTypeLabel";
            this.authTypeLabel.Size = new System.Drawing.Size(70, 15);
            this.authTypeLabel.TabIndex = 0;
            this.authTypeLabel.Text = "Login Type:";
            // 
            // authTypeCb
            // 
            this.authTypeCb.FormattingEnabled = true;
            this.authTypeCb.Items.AddRange(new object[] {
            "google",
            "Ptc"});
            this.authTypeCb.Location = new System.Drawing.Point(96, 5);
            this.authTypeCb.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.authTypeCb.Name = "authTypeCb";
            this.authTypeCb.Size = new System.Drawing.Size(158, 23);
            this.authTypeCb.TabIndex = 1;
            this.authTypeCb.SelectedIndexChanged += new System.EventHandler(this.authTypeCb_SelectedIndexChanged);
            // 
            // UserLabel
            // 
            this.UserLabel.AutoSize = true;
            this.UserLabel.Location = new System.Drawing.Point(3, 41);
            this.UserLabel.Name = "UserLabel";
            this.UserLabel.Size = new System.Drawing.Size(68, 15);
            this.UserLabel.TabIndex = 2;
            this.UserLabel.Text = "Username:";
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(3, 71);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(64, 15);
            this.PasswordLabel.TabIndex = 3;
            this.PasswordLabel.Text = "Password:";
            // 
            // latLabel
            // 
            this.latLabel.AutoSize = true;
            this.latLabel.Location = new System.Drawing.Point(3, 101);
            this.latLabel.Name = "latLabel";
            this.latLabel.Size = new System.Drawing.Size(54, 15);
            this.latLabel.TabIndex = 4;
            this.latLabel.Text = "Latitude:";
            // 
            // longiLabel
            // 
            this.longiLabel.AutoSize = true;
            this.longiLabel.Location = new System.Drawing.Point(3, 131);
            this.longiLabel.Name = "longiLabel";
            this.longiLabel.Size = new System.Drawing.Size(65, 15);
            this.longiLabel.TabIndex = 5;
            this.longiLabel.Text = "Longitude:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Razzberry Mode:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 222);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Transfer Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 370);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Evolve Pokemon:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 254);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "CP Threshold:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 192);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Razzberry Setting:";
            // 
            // UserLoginBox
            // 
            this.UserLoginBox.Location = new System.Drawing.Point(96, 39);
            this.UserLoginBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.UserLoginBox.Name = "UserLoginBox";
            this.UserLoginBox.Size = new System.Drawing.Size(158, 21);
            this.UserLoginBox.TabIndex = 11;
            // 
            // UserPasswordBox
            // 
            this.UserPasswordBox.Location = new System.Drawing.Point(96, 71);
            this.UserPasswordBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.UserPasswordBox.Name = "UserPasswordBox";
            this.UserPasswordBox.PasswordChar = '*';
            this.UserPasswordBox.Size = new System.Drawing.Size(158, 21);
            this.UserPasswordBox.TabIndex = 12;
            // 
            // latitudeText
            // 
            this.latitudeText.Location = new System.Drawing.Point(138, 99);
            this.latitudeText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.latitudeText.Name = "latitudeText";
            this.latitudeText.ReadOnly = true;
            this.latitudeText.Size = new System.Drawing.Size(116, 21);
            this.latitudeText.TabIndex = 13;
            // 
            // longitudeText
            // 
            this.longitudeText.Location = new System.Drawing.Point(138, 129);
            this.longitudeText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.longitudeText.Name = "longitudeText";
            this.longitudeText.ReadOnly = true;
            this.longitudeText.Size = new System.Drawing.Size(116, 21);
            this.longitudeText.TabIndex = 14;
            // 
            // razzmodeCb
            // 
            this.razzmodeCb.FormattingEnabled = true;
            this.razzmodeCb.Items.AddRange(new object[] {
            "probability",
            "cp"});
            this.razzmodeCb.Location = new System.Drawing.Point(138, 159);
            this.razzmodeCb.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.razzmodeCb.Name = "razzmodeCb";
            this.razzmodeCb.Size = new System.Drawing.Size(116, 23);
            this.razzmodeCb.TabIndex = 15;
            // 
            // razzSettingText
            // 
            this.razzSettingText.Location = new System.Drawing.Point(138, 190);
            this.razzSettingText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.razzSettingText.Name = "razzSettingText";
            this.razzSettingText.Size = new System.Drawing.Size(116, 21);
            this.razzSettingText.TabIndex = 16;
            this.razzSettingText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.razzSettingText_KeyPress);
            // 
            // transferTypeCb
            // 
            this.transferTypeCb.FormattingEnabled = true;
            this.transferTypeCb.Items.AddRange(new object[] {
            "None",
            "CP",
            "IV",
            "Leave Strongest",
            "Duplicate",
            "IV Duplicate",
            "All"});
            this.transferTypeCb.Location = new System.Drawing.Point(138, 220);
            this.transferTypeCb.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.transferTypeCb.Name = "transferTypeCb";
            this.transferTypeCb.Size = new System.Drawing.Size(116, 23);
            this.transferTypeCb.TabIndex = 17;
            this.transferTypeCb.SelectedIndexChanged += new System.EventHandler(this.transferTypeCb_SelectedIndexChanged);
            // 
            // transferCpThresText
            // 
            this.transferCpThresText.Location = new System.Drawing.Point(138, 252);
            this.transferCpThresText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.transferCpThresText.Name = "transferCpThresText";
            this.transferCpThresText.Size = new System.Drawing.Size(116, 21);
            this.transferCpThresText.TabIndex = 18;
            // 
            // evolveAllChk
            // 
            this.evolveAllChk.AutoSize = true;
            this.evolveAllChk.Location = new System.Drawing.Point(138, 370);
            this.evolveAllChk.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.evolveAllChk.Name = "evolveAllChk";
            this.evolveAllChk.Size = new System.Drawing.Size(15, 14);
            this.evolveAllChk.TabIndex = 19;
            this.evolveAllChk.UseVisualStyleBackColor = true;
            // 
            // saveBtn
            // 
            this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBtn.Location = new System.Drawing.Point(6, 412);
            this.saveBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(248, 42);
            this.saveBtn.TabIndex = 20;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // gMapControl1
            // 
            this.gMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl1.BackColor = System.Drawing.SystemColors.Info;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(6, 7);
            this.gMapControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
            this.gMapControl1.Size = new System.Drawing.Size(577, 378);
            this.gMapControl1.TabIndex = 22;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseClick);
            // 
            // FindAdressButton
            // 
            this.FindAdressButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.FindAdressButton.Location = new System.Drawing.Point(464, 393);
            this.FindAdressButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.FindAdressButton.Name = "FindAdressButton";
            this.FindAdressButton.Size = new System.Drawing.Size(119, 30);
            this.FindAdressButton.TabIndex = 25;
            this.FindAdressButton.Text = "Find Location";
            this.FindAdressButton.UseVisualStyleBackColor = true;
            this.FindAdressButton.Click += new System.EventHandler(this.FindAdressButton_Click);
            // 
            // AdressBox
            // 
            this.AdressBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AdressBox.ForeColor = System.Drawing.Color.Gray;
            this.AdressBox.Location = new System.Drawing.Point(6, 397);
            this.AdressBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AdressBox.Name = "AdressBox";
            this.AdressBox.Size = new System.Drawing.Size(452, 23);
            this.AdressBox.TabIndex = 25;
            this.AdressBox.Text = "Enter an address or a coordinate";
            this.AdressBox.Enter += new System.EventHandler(this.AdressBox_Enter);
            this.AdressBox.Leave += new System.EventHandler(this.AdressBox_Leave);
            // 
            // trackBar
            // 
            this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar.BackColor = System.Drawing.SystemColors.Info;
            this.trackBar.Location = new System.Drawing.Point(536, 7);
            this.trackBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.trackBar.Name = "trackBar";
            this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar.Size = new System.Drawing.Size(45, 120);
            this.trackBar.TabIndex = 25;
            this.trackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.TravelSpeedBox);
            this.panel1.Controls.Add(this.CatchPokemonBox);
            this.panel1.Controls.Add(this.CatchPokemonText);
            this.panel1.Controls.Add(this.transferIVThresText);
            this.panel1.Controls.Add(this.TravelSpeedText);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.authTypeLabel);
            this.panel1.Controls.Add(this.authTypeCb);
            this.panel1.Controls.Add(this.UserLabel);
            this.panel1.Controls.Add(this.PasswordLabel);
            this.panel1.Controls.Add(this.saveBtn);
            this.panel1.Controls.Add(this.latLabel);
            this.panel1.Controls.Add(this.evolveAllChk);
            this.panel1.Controls.Add(this.longiLabel);
            this.panel1.Controls.Add(this.transferCpThresText);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.transferTypeCb);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.razzSettingText);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.razzmodeCb);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.longitudeText);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.latitudeText);
            this.panel1.Controls.Add(this.UserLoginBox);
            this.panel1.Controls.Add(this.UserPasswordBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(261, 458);
            this.panel1.TabIndex = 26;
            // 
            // TravelSpeedBox
            // 
            this.TravelSpeedBox.Location = new System.Drawing.Point(138, 314);
            this.TravelSpeedBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TravelSpeedBox.Name = "TravelSpeedBox";
            this.TravelSpeedBox.Size = new System.Drawing.Size(116, 21);
            this.TravelSpeedBox.TabIndex = 22;
            this.TravelSpeedBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TravelSpeedBox_KeyPress);
            // 
            // CatchPokemonBox
            // 
            this.CatchPokemonBox.AutoSize = true;
            this.CatchPokemonBox.Location = new System.Drawing.Point(138, 344);
            this.CatchPokemonBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CatchPokemonBox.Name = "CatchPokemonBox";
            this.CatchPokemonBox.Size = new System.Drawing.Size(15, 14);
            this.CatchPokemonBox.TabIndex = 26;
            this.CatchPokemonBox.UseVisualStyleBackColor = true;
            // 
            // CatchPokemonText
            // 
            this.CatchPokemonText.AutoSize = true;
            this.CatchPokemonText.Location = new System.Drawing.Point(3, 344);
            this.CatchPokemonText.Name = "CatchPokemonText";
            this.CatchPokemonText.Size = new System.Drawing.Size(97, 15);
            this.CatchPokemonText.TabIndex = 25;
            this.CatchPokemonText.Text = "Catch Pokemon:";
            // 
            // transferIVThresText
            // 
            this.transferIVThresText.Location = new System.Drawing.Point(138, 254);
            this.transferIVThresText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.transferIVThresText.Name = "transferIVThresText";
            this.transferIVThresText.Size = new System.Drawing.Size(116, 21);
            this.transferIVThresText.TabIndex = 24;
            // 
            // TravelSpeedText
            // 
            this.TravelSpeedText.AutoSize = true;
            this.TravelSpeedText.Location = new System.Drawing.Point(3, 318);
            this.TravelSpeedText.Name = "TravelSpeedText";
            this.TravelSpeedText.Size = new System.Drawing.Size(112, 15);
            this.TravelSpeedText.TabIndex = 23;
            this.TravelSpeedText.Text = "Travel Speed km/h:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 254);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 15);
            this.label6.TabIndex = 21;
            this.label6.Text = "IV Threshold:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(261, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(597, 458);
            this.panel2.TabIndex = 27;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabLocation);
            this.tabControl.Controls.Add(this.tabPokemon);
            this.tabControl.Controls.Add(this.tabItems);
            this.tabControl.Controls.Add(this.tabDevice);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(597, 458);
            this.tabControl.TabIndex = 26;
            // 
            // tabLocation
            // 
            this.tabLocation.BackColor = System.Drawing.SystemColors.Control;
            this.tabLocation.Controls.Add(this.trackBar);
            this.tabLocation.Controls.Add(this.AdressBox);
            this.tabLocation.Controls.Add(this.FindAdressButton);
            this.tabLocation.Controls.Add(this.gMapControl1);
            this.tabLocation.Location = new System.Drawing.Point(4, 24);
            this.tabLocation.Name = "tabLocation";
            this.tabLocation.Padding = new System.Windows.Forms.Padding(3);
            this.tabLocation.Size = new System.Drawing.Size(589, 430);
            this.tabLocation.TabIndex = 0;
            this.tabLocation.Text = "Location";
            // 
            // tabPokemon
            // 
            this.tabPokemon.BackColor = System.Drawing.SystemColors.Control;
            this.tabPokemon.Controls.Add(this.groupBox3);
            this.tabPokemon.Controls.Add(this.groupBox2);
            this.tabPokemon.Controls.Add(this.groupBox1);
            this.tabPokemon.Location = new System.Drawing.Point(4, 24);
            this.tabPokemon.Name = "tabPokemon";
            this.tabPokemon.Padding = new System.Windows.Forms.Padding(3);
            this.tabPokemon.Size = new System.Drawing.Size(589, 430);
            this.tabPokemon.TabIndex = 1;
            this.tabPokemon.Text = "Pokemon";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.cbSelectAllEvolve);
            this.groupBox3.Controls.Add(this.clbEvolve);
            this.groupBox3.Location = new System.Drawing.Point(394, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(188, 421);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Exclude Evolve";
            // 
            // cbSelectAllEvolve
            // 
            this.cbSelectAllEvolve.AutoSize = true;
            this.cbSelectAllEvolve.Location = new System.Drawing.Point(6, 22);
            this.cbSelectAllEvolve.Name = "cbSelectAllEvolve";
            this.cbSelectAllEvolve.Size = new System.Drawing.Size(74, 19);
            this.cbSelectAllEvolve.TabIndex = 1;
            this.cbSelectAllEvolve.Text = "Select All";
            this.cbSelectAllEvolve.UseVisualStyleBackColor = true;
            // 
            // clbEvolve
            // 
            this.clbEvolve.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbEvolve.CheckOnClick = true;
            this.clbEvolve.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clbEvolve.FormattingEnabled = true;
            this.clbEvolve.Location = new System.Drawing.Point(0, 41);
            this.clbEvolve.Name = "clbEvolve";
            this.clbEvolve.Size = new System.Drawing.Size(188, 364);
            this.clbEvolve.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.cbSelectAllCatch);
            this.groupBox2.Controls.Add(this.clbCatch);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(188, 421);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Exclude Catch";
            // 
            // cbSelectAllCatch
            // 
            this.cbSelectAllCatch.AutoSize = true;
            this.cbSelectAllCatch.Location = new System.Drawing.Point(6, 22);
            this.cbSelectAllCatch.Name = "cbSelectAllCatch";
            this.cbSelectAllCatch.Size = new System.Drawing.Size(74, 19);
            this.cbSelectAllCatch.TabIndex = 1;
            this.cbSelectAllCatch.Text = "Select All";
            this.cbSelectAllCatch.UseVisualStyleBackColor = true;
            // 
            // clbCatch
            // 
            this.clbCatch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbCatch.CheckOnClick = true;
            this.clbCatch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clbCatch.FormattingEnabled = true;
            this.clbCatch.Location = new System.Drawing.Point(0, 41);
            this.clbCatch.Name = "clbCatch";
            this.clbCatch.Size = new System.Drawing.Size(188, 364);
            this.clbCatch.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.cbSelectAllTransfer);
            this.groupBox1.Controls.Add(this.clbTransfer);
            this.groupBox1.Location = new System.Drawing.Point(200, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(188, 421);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exclude Transfer";
            // 
            // cbSelectAllTransfer
            // 
            this.cbSelectAllTransfer.AutoSize = true;
            this.cbSelectAllTransfer.Location = new System.Drawing.Point(6, 22);
            this.cbSelectAllTransfer.Name = "cbSelectAllTransfer";
            this.cbSelectAllTransfer.Size = new System.Drawing.Size(74, 19);
            this.cbSelectAllTransfer.TabIndex = 1;
            this.cbSelectAllTransfer.Text = "Select All";
            this.cbSelectAllTransfer.UseVisualStyleBackColor = true;
            // 
            // clbTransfer
            // 
            this.clbTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbTransfer.CheckOnClick = true;
            this.clbTransfer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clbTransfer.FormattingEnabled = true;
            this.clbTransfer.Location = new System.Drawing.Point(0, 41);
            this.clbTransfer.Name = "clbTransfer";
            this.clbTransfer.Size = new System.Drawing.Size(188, 364);
            this.clbTransfer.TabIndex = 0;
            // 
            // tabItems
            // 
            this.tabItems.BackColor = System.Drawing.SystemColors.Control;
            this.tabItems.Controls.Add(this.flpItems);
            this.tabItems.Location = new System.Drawing.Point(4, 24);
            this.tabItems.Name = "tabItems";
            this.tabItems.Padding = new System.Windows.Forms.Padding(3);
            this.tabItems.Size = new System.Drawing.Size(589, 430);
            this.tabItems.TabIndex = 2;
            this.tabItems.Text = "Items";
            // 
            // tabDevice
            // 
            this.tabDevice.Controls.Add(this.label22);
            this.tabDevice.Controls.Add(this.label20);
            this.tabDevice.Controls.Add(this.label21);
            this.tabDevice.Controls.Add(this.RandomIDBtn);
            this.tabDevice.Controls.Add(this.deviceTypeCb);
            this.tabDevice.Controls.Add(this.RandomDeviceBtn);
            this.tabDevice.Controls.Add(this.FirmwareFingerprintTb);
            this.tabDevice.Controls.Add(this.label14);
            this.tabDevice.Controls.Add(this.FirmwareTypeTb);
            this.tabDevice.Controls.Add(this.label13);
            this.tabDevice.Controls.Add(this.FirmwareTagsTb);
            this.tabDevice.Controls.Add(this.label12);
            this.tabDevice.Controls.Add(this.FirmwareBrandTb);
            this.tabDevice.Controls.Add(this.label11);
            this.tabDevice.Controls.Add(this.HardwareModelTb);
            this.tabDevice.Controls.Add(this.label10);
            this.tabDevice.Controls.Add(this.HardwareManufacturerTb);
            this.tabDevice.Controls.Add(this.label9);
            this.tabDevice.Controls.Add(this.DeviceModelBootTb);
            this.tabDevice.Controls.Add(this.label8);
            this.tabDevice.Controls.Add(this.DeviceModelIdentifierTb);
            this.tabDevice.Controls.Add(this.label7);
            this.tabDevice.Controls.Add(this.DeviceModelTb);
            this.tabDevice.Controls.Add(this.label15);
            this.tabDevice.Controls.Add(this.DeviceBrandTb);
            this.tabDevice.Controls.Add(this.label16);
            this.tabDevice.Controls.Add(this.AndroidBootloaderTb);
            this.tabDevice.Controls.Add(this.label17);
            this.tabDevice.Controls.Add(this.AndroidBoardNameTb);
            this.tabDevice.Controls.Add(this.BoardName);
            this.tabDevice.Controls.Add(this.DeviceIdTb);
            this.tabDevice.Controls.Add(this.deviceIdlb);
            this.tabDevice.Controls.Add(this.label18);
            this.tabDevice.Location = new System.Drawing.Point(4, 24);
            this.tabDevice.Name = "tabDevice";
            this.tabDevice.Size = new System.Drawing.Size(589, 430);
            this.tabDevice.TabIndex = 0;
            this.tabDevice.Text = "Device";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label22.Location = new System.Drawing.Point(416, 92);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(168, 90);
            this.label22.TabIndex = 69;
            this.label22.Text = "This setting change what the \r\nserver think you are using to \r\nplay Pokémon GO. I" +
    "ts a good \r\nidea to change your device to \r\nwhat phone you are using to \r\npreven" +
    "t ip ban.";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label20.Location = new System.Drawing.Point(416, 13);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(74, 18);
            this.label20.TabIndex = 67;
            this.label20.Text = "Important:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label21.Location = new System.Drawing.Point(416, 32);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(164, 45);
            this.label21.TabIndex = 66;
            this.label21.Text = "For your account safety.\r\nPlease do not change your \r\naccount infomation too ofte" +
    "n.\r\n";
            // 
            // RandomIDBtn
            // 
            this.RandomIDBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.RandomIDBtn.Location = new System.Drawing.Point(326, 39);
            this.RandomIDBtn.Name = "RandomIDBtn";
            this.RandomIDBtn.Size = new System.Drawing.Size(87, 25);
            this.RandomIDBtn.TabIndex = 65;
            this.RandomIDBtn.Text = "Get New ID";
            this.RandomIDBtn.UseVisualStyleBackColor = true;
            this.RandomIDBtn.Click += new System.EventHandler(this.RandomIDBtn_Click);
            // 
            // deviceTypeCb
            // 
            this.deviceTypeCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deviceTypeCb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.deviceTypeCb.FormattingEnabled = true;
            this.deviceTypeCb.Items.AddRange(new object[] {
            "Apple",
            "Android"});
            this.deviceTypeCb.Location = new System.Drawing.Point(170, 13);
            this.deviceTypeCb.Name = "deviceTypeCb";
            this.deviceTypeCb.Size = new System.Drawing.Size(150, 23);
            this.deviceTypeCb.TabIndex = 37;
            this.deviceTypeCb.SelectionChangeCommitted += new System.EventHandler(this.deviceTypeCb_SelectedIndexChanged);
            // 
            // RandomDeviceBtn
            // 
            this.RandomDeviceBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.RandomDeviceBtn.Location = new System.Drawing.Point(419, 334);
            this.RandomDeviceBtn.Name = "RandomDeviceBtn";
            this.RandomDeviceBtn.Size = new System.Drawing.Size(162, 79);
            this.RandomDeviceBtn.TabIndex = 64;
            this.RandomDeviceBtn.Text = "I am feeling RICH\r\n(Randomize)";
            this.RandomDeviceBtn.UseVisualStyleBackColor = true;
            this.RandomDeviceBtn.Click += new System.EventHandler(this.RandomDeviceBtn_Click);
            // 
            // FirmwareFingerprintTb
            // 
            this.FirmwareFingerprintTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FirmwareFingerprintTb.Location = new System.Drawing.Point(170, 392);
            this.FirmwareFingerprintTb.Name = "FirmwareFingerprintTb";
            this.FirmwareFingerprintTb.Size = new System.Drawing.Size(243, 21);
            this.FirmwareFingerprintTb.TabIndex = 62;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label14.Location = new System.Drawing.Point(19, 396);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(121, 15);
            this.label14.TabIndex = 49;
            this.label14.Text = "Firmware Fingerprint";
            // 
            // FirmwareTypeTb
            // 
            this.FirmwareTypeTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FirmwareTypeTb.Location = new System.Drawing.Point(170, 363);
            this.FirmwareTypeTb.Name = "FirmwareTypeTb";
            this.FirmwareTypeTb.Size = new System.Drawing.Size(243, 21);
            this.FirmwareTypeTb.TabIndex = 58;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label13.Location = new System.Drawing.Point(19, 366);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(88, 15);
            this.label13.TabIndex = 51;
            this.label13.Text = "Firmware Type";
            // 
            // FirmwareTagsTb
            // 
            this.FirmwareTagsTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FirmwareTagsTb.Location = new System.Drawing.Point(170, 334);
            this.FirmwareTagsTb.Name = "FirmwareTagsTb";
            this.FirmwareTagsTb.Size = new System.Drawing.Size(243, 21);
            this.FirmwareTagsTb.TabIndex = 54;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label12.Location = new System.Drawing.Point(19, 337);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 15);
            this.label12.TabIndex = 50;
            this.label12.Text = "Firmware Tags";
            // 
            // FirmwareBrandTb
            // 
            this.FirmwareBrandTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FirmwareBrandTb.Location = new System.Drawing.Point(170, 305);
            this.FirmwareBrandTb.Name = "FirmwareBrandTb";
            this.FirmwareBrandTb.Size = new System.Drawing.Size(243, 21);
            this.FirmwareBrandTb.TabIndex = 52;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label11.Location = new System.Drawing.Point(19, 308);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(95, 15);
            this.label11.TabIndex = 48;
            this.label11.Text = "Firmware Brand";
            // 
            // HardwareModelTb
            // 
            this.HardwareModelTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.HardwareModelTb.Location = new System.Drawing.Point(170, 275);
            this.HardwareModelTb.Name = "HardwareModelTb";
            this.HardwareModelTb.Size = new System.Drawing.Size(243, 21);
            this.HardwareModelTb.TabIndex = 56;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label10.Location = new System.Drawing.Point(19, 279);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(99, 15);
            this.label10.TabIndex = 46;
            this.label10.Text = "Hardware Model";
            // 
            // HardwareManufacturerTb
            // 
            this.HardwareManufacturerTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.HardwareManufacturerTb.Location = new System.Drawing.Point(170, 246);
            this.HardwareManufacturerTb.Name = "HardwareManufacturerTb";
            this.HardwareManufacturerTb.Size = new System.Drawing.Size(243, 21);
            this.HardwareManufacturerTb.TabIndex = 60;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label9.Location = new System.Drawing.Point(19, 249);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 15);
            this.label9.TabIndex = 47;
            this.label9.Text = "Hardware Manu facturer";
            // 
            // DeviceModelBootTb
            // 
            this.DeviceModelBootTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceModelBootTb.Location = new System.Drawing.Point(170, 217);
            this.DeviceModelBootTb.Name = "DeviceModelBootTb";
            this.DeviceModelBootTb.Size = new System.Drawing.Size(243, 21);
            this.DeviceModelBootTb.TabIndex = 63;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label8.Location = new System.Drawing.Point(19, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 15);
            this.label8.TabIndex = 44;
            this.label8.Text = "Device Model Boot";
            // 
            // DeviceModelIdentifierTb
            // 
            this.DeviceModelIdentifierTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceModelIdentifierTb.Location = new System.Drawing.Point(170, 188);
            this.DeviceModelIdentifierTb.Name = "DeviceModelIdentifierTb";
            this.DeviceModelIdentifierTb.Size = new System.Drawing.Size(243, 21);
            this.DeviceModelIdentifierTb.TabIndex = 53;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label7.Location = new System.Drawing.Point(19, 191);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(132, 15);
            this.label7.TabIndex = 43;
            this.label7.Text = "Device Model Identifier";
            // 
            // DeviceModelTb
            // 
            this.DeviceModelTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceModelTb.Location = new System.Drawing.Point(170, 158);
            this.DeviceModelTb.Name = "DeviceModelTb";
            this.DeviceModelTb.Size = new System.Drawing.Size(243, 21);
            this.DeviceModelTb.TabIndex = 55;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label15.Location = new System.Drawing.Point(19, 162);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(82, 15);
            this.label15.TabIndex = 42;
            this.label15.Text = "Device Model";
            // 
            // DeviceBrandTb
            // 
            this.DeviceBrandTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceBrandTb.Location = new System.Drawing.Point(170, 129);
            this.DeviceBrandTb.Name = "DeviceBrandTb";
            this.DeviceBrandTb.Size = new System.Drawing.Size(243, 21);
            this.DeviceBrandTb.TabIndex = 57;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label16.Location = new System.Drawing.Point(19, 132);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(80, 15);
            this.label16.TabIndex = 41;
            this.label16.Text = "Device Brand";
            // 
            // AndroidBootloaderTb
            // 
            this.AndroidBootloaderTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.AndroidBootloaderTb.Location = new System.Drawing.Point(170, 100);
            this.AndroidBootloaderTb.Name = "AndroidBootloaderTb";
            this.AndroidBootloaderTb.Size = new System.Drawing.Size(243, 21);
            this.AndroidBootloaderTb.TabIndex = 59;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label17.Location = new System.Drawing.Point(19, 103);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(115, 15);
            this.label17.TabIndex = 40;
            this.label17.Text = "Android Boot loader";
            // 
            // AndroidBoardNameTb
            // 
            this.AndroidBoardNameTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.AndroidBoardNameTb.Location = new System.Drawing.Point(170, 71);
            this.AndroidBoardNameTb.Name = "AndroidBoardNameTb";
            this.AndroidBoardNameTb.Size = new System.Drawing.Size(243, 21);
            this.AndroidBoardNameTb.TabIndex = 61;
            // 
            // BoardName
            // 
            this.BoardName.AutoSize = true;
            this.BoardName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.BoardName.Location = new System.Drawing.Point(19, 74);
            this.BoardName.Name = "BoardName";
            this.BoardName.Size = new System.Drawing.Size(122, 15);
            this.BoardName.TabIndex = 39;
            this.BoardName.Text = "Android Board Name";
            // 
            // DeviceIdTb
            // 
            this.DeviceIdTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceIdTb.Location = new System.Drawing.Point(170, 41);
            this.DeviceIdTb.Name = "DeviceIdTb";
            this.DeviceIdTb.Size = new System.Drawing.Size(150, 21);
            this.DeviceIdTb.TabIndex = 38;
            // 
            // deviceIdlb
            // 
            this.deviceIdlb.AutoSize = true;
            this.deviceIdlb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.deviceIdlb.Location = new System.Drawing.Point(19, 45);
            this.deviceIdlb.Name = "deviceIdlb";
            this.deviceIdlb.Size = new System.Drawing.Size(59, 15);
            this.deviceIdlb.TabIndex = 45;
            this.deviceIdlb.Text = "Device ID";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label18.Location = new System.Drawing.Point(19, 16);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(76, 15);
            this.label18.TabIndex = 36;
            this.label18.Text = "Device Type:";
            // 
            // flpItems
            // 
            this.flpItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpItems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpItems.Location = new System.Drawing.Point(3, 3);
            this.flpItems.Name = "flpItems";
            this.flpItems.Size = new System.Drawing.Size(583, 424);
            this.flpItems.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 458);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(874, 497);
            this.Name = "SettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabLocation.ResumeLayout(false);
            this.tabLocation.PerformLayout();
            this.tabPokemon.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabItems.ResumeLayout(false);
            this.tabDevice.ResumeLayout(false);
            this.tabDevice.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label authTypeLabel;
        private System.Windows.Forms.ComboBox authTypeCb;
        private System.Windows.Forms.Label UserLabel;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.Label latLabel;
        private System.Windows.Forms.Label longiLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox UserLoginBox;
        private System.Windows.Forms.TextBox UserPasswordBox;
        private System.Windows.Forms.TextBox latitudeText;
        private System.Windows.Forms.TextBox longitudeText;
        private System.Windows.Forms.ComboBox razzmodeCb;
        private System.Windows.Forms.TextBox razzSettingText;
        private System.Windows.Forms.ComboBox transferTypeCb;
        private System.Windows.Forms.TextBox transferCpThresText;
        private System.Windows.Forms.CheckBox evolveAllChk;
        private System.Windows.Forms.Button saveBtn;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.TextBox TravelSpeedBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label TravelSpeedText;
        private System.Windows.Forms.TextBox transferIVThresText;
        private System.Windows.Forms.TextBox AdressBox;
        private System.Windows.Forms.Button FindAdressButton;
        private System.Windows.Forms.CheckBox CatchPokemonBox;
        private System.Windows.Forms.Label CatchPokemonText;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabLocation;
        private System.Windows.Forms.TabPage tabPokemon;
        private System.Windows.Forms.CheckedListBox clbTransfer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox clbCatch;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckedListBox clbEvolve;
        private System.Windows.Forms.CheckBox cbSelectAllEvolve;
        private System.Windows.Forms.CheckBox cbSelectAllCatch;
        private System.Windows.Forms.CheckBox cbSelectAllTransfer;
        private System.Windows.Forms.TabPage tabDevice;
        private System.Windows.Forms.Button RandomIDBtn;
        private System.Windows.Forms.ComboBox deviceTypeCb;
        private System.Windows.Forms.Button RandomDeviceBtn;
        private System.Windows.Forms.TextBox FirmwareFingerprintTb;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox FirmwareTypeTb;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox FirmwareTagsTb;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox FirmwareBrandTb;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox HardwareModelTb;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox HardwareManufacturerTb;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox DeviceModelBootTb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox DeviceModelIdentifierTb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox DeviceModelTb;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox DeviceBrandTb;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox AndroidBootloaderTb;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox AndroidBoardNameTb;
        private System.Windows.Forms.Label BoardName;
        private System.Windows.Forms.TextBox DeviceIdTb;
        private System.Windows.Forms.Label deviceIdlb;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TabPage tabItems;
        private System.Windows.Forms.FlowLayoutPanel flpItems;
    }
}
