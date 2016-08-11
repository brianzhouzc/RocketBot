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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.clbTransfer = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.clbCatch = new System.Windows.Forms.CheckedListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.clbEvolve = new System.Windows.Forms.CheckedListBox();
            this.cbSelectAllCatch = new System.Windows.Forms.CheckBox();
            this.cbSelectAllTransfer = new System.Windows.Forms.CheckBox();
            this.cbSelectAllEvolve = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            this.FindAdressButton.Click += new System.EventHandler(this.FindAdressButton_Click_1);
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
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(597, 458);
            this.tabControl.TabIndex = 26;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.trackBar);
            this.tabPage1.Controls.Add(this.AdressBox);
            this.tabPage1.Controls.Add(this.FindAdressButton);
            this.tabPage1.Controls.Add(this.gMapControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(589, 430);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Location";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(589, 430);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Pokemon";
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
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckedListBox clbTransfer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox clbCatch;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckedListBox clbEvolve;
        private System.Windows.Forms.CheckBox cbSelectAllEvolve;
        private System.Windows.Forms.CheckBox cbSelectAllCatch;
        private System.Windows.Forms.CheckBox cbSelectAllTransfer;
    }
}
