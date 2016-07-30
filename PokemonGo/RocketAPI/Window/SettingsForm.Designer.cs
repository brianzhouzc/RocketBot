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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FindAdressButton = new System.Windows.Forms.Button();
            this.AdressBox = new System.Windows.Forms.TextBox();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.language = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TravelSpeedBox = new System.Windows.Forms.TextBox();
            this.CatchPokemonBox = new System.Windows.Forms.CheckBox();
            this.CatchPokemonText = new System.Windows.Forms.Label();
            this.transferIVThresText = new System.Windows.Forms.TextBox();
            this.TravelSpeedText = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // authTypeLabel
            // 
            resources.ApplyResources(this.authTypeLabel, "authTypeLabel");
            this.authTypeLabel.Name = "authTypeLabel";
            this.authTypeLabel.Click += new System.EventHandler(this.authTypeLabel_Click);
            // 
            // authTypeCb
            // 
            resources.ApplyResources(this.authTypeCb, "authTypeCb");
            this.authTypeCb.FormattingEnabled = true;
            this.authTypeCb.Items.AddRange(new object[] {
            resources.GetString("authTypeCb.Items"),
            resources.GetString("authTypeCb.Items1")});
            this.authTypeCb.Name = "authTypeCb";
            this.authTypeCb.SelectedIndexChanged += new System.EventHandler(this.authTypeCb_SelectedIndexChanged);
            // 
            // UserLabel
            // 
            resources.ApplyResources(this.UserLabel, "UserLabel");
            this.UserLabel.Name = "UserLabel";
            // 
            // PasswordLabel
            // 
            resources.ApplyResources(this.PasswordLabel, "PasswordLabel");
            this.PasswordLabel.Name = "PasswordLabel";
            // 
            // latLabel
            // 
            resources.ApplyResources(this.latLabel, "latLabel");
            this.latLabel.Name = "latLabel";
            // 
            // longiLabel
            // 
            resources.ApplyResources(this.longiLabel, "longiLabel");
            this.longiLabel.Name = "longiLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // UserLoginBox
            // 
            resources.ApplyResources(this.UserLoginBox, "UserLoginBox");
            this.UserLoginBox.Name = "UserLoginBox";
            // 
            // UserPasswordBox
            // 
            resources.ApplyResources(this.UserPasswordBox, "UserPasswordBox");
            this.UserPasswordBox.Name = "UserPasswordBox";
            // 
            // latitudeText
            // 
            resources.ApplyResources(this.latitudeText, "latitudeText");
            this.latitudeText.Name = "latitudeText";
            this.latitudeText.ReadOnly = true;
            // 
            // longitudeText
            // 
            resources.ApplyResources(this.longitudeText, "longitudeText");
            this.longitudeText.Name = "longitudeText";
            this.longitudeText.ReadOnly = true;
            // 
            // razzmodeCb
            // 
            resources.ApplyResources(this.razzmodeCb, "razzmodeCb");
            this.razzmodeCb.FormattingEnabled = true;
            this.razzmodeCb.Items.AddRange(new object[] {
            resources.GetString("razzmodeCb.Items"),
            resources.GetString("razzmodeCb.Items1")});
            this.razzmodeCb.Name = "razzmodeCb";
            // 
            // razzSettingText
            // 
            resources.ApplyResources(this.razzSettingText, "razzSettingText");
            this.razzSettingText.Name = "razzSettingText";
            // 
            // transferTypeCb
            // 
            resources.ApplyResources(this.transferTypeCb, "transferTypeCb");
            this.transferTypeCb.FormattingEnabled = true;
            this.transferTypeCb.Items.AddRange(new object[] {
            resources.GetString("transferTypeCb.Items"),
            resources.GetString("transferTypeCb.Items1"),
            resources.GetString("transferTypeCb.Items2"),
            resources.GetString("transferTypeCb.Items3"),
            resources.GetString("transferTypeCb.Items4"),
            resources.GetString("transferTypeCb.Items5"),
            resources.GetString("transferTypeCb.Items6")});
            this.transferTypeCb.Name = "transferTypeCb";
            this.transferTypeCb.SelectedIndexChanged += new System.EventHandler(this.transferTypeCb_SelectedIndexChanged);
            // 
            // transferCpThresText
            // 
            resources.ApplyResources(this.transferCpThresText, "transferCpThresText");
            this.transferCpThresText.Name = "transferCpThresText";
            this.transferCpThresText.TextChanged += new System.EventHandler(this.transferCpThresText_TextChanged);
            // 
            // evolveAllChk
            // 
            resources.ApplyResources(this.evolveAllChk, "evolveAllChk");
            this.evolveAllChk.Name = "evolveAllChk";
            this.evolveAllChk.UseVisualStyleBackColor = true;
            this.evolveAllChk.CheckedChanged += new System.EventHandler(this.evolveAllChk_CheckedChanged);
            // 
            // saveBtn
            // 
            resources.ApplyResources(this.saveBtn, "saveBtn");
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // gMapControl1
            // 
            resources.ApplyResources(this.gMapControl1, "gMapControl1");
            this.gMapControl1.BackColor = System.Drawing.SystemColors.Info;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
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
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            this.gMapControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseClick);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.FindAdressButton);
            this.groupBox1.Controls.Add(this.AdressBox);
            this.groupBox1.Controls.Add(this.trackBar);
            this.groupBox1.Controls.Add(this.gMapControl1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // FindAdressButton
            // 
            resources.ApplyResources(this.FindAdressButton, "FindAdressButton");
            this.FindAdressButton.Name = "FindAdressButton";
            this.FindAdressButton.UseVisualStyleBackColor = true;
            this.FindAdressButton.Click += new System.EventHandler(this.FindAdressButton_Click_1);
            // 
            // AdressBox
            // 
            resources.ApplyResources(this.AdressBox, "AdressBox");
            this.AdressBox.Name = "AdressBox";
            // 
            // trackBar
            // 
            resources.ApplyResources(this.trackBar, "trackBar");
            this.trackBar.BackColor = System.Drawing.SystemColors.Info;
            this.trackBar.Name = "trackBar";
            this.trackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.language);
            this.panel1.Controls.Add(this.label7);
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
            this.panel1.Name = "panel1";
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // language
            // 
            resources.ApplyResources(this.language, "language");
            this.language.FormattingEnabled = true;
            this.language.Items.AddRange(new object[] {
            resources.GetString("language.Items"),
            resources.GetString("language.Items1"),
            resources.GetString("language.Items2"),
            resources.GetString("language.Items3")});
            this.language.Name = "language";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // TravelSpeedBox
            // 
            resources.ApplyResources(this.TravelSpeedBox, "TravelSpeedBox");
            this.TravelSpeedBox.Name = "TravelSpeedBox";
            this.TravelSpeedBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // CatchPokemonBox
            // 
            resources.ApplyResources(this.CatchPokemonBox, "CatchPokemonBox");
            this.CatchPokemonBox.Name = "CatchPokemonBox";
            this.CatchPokemonBox.UseVisualStyleBackColor = true;
            this.CatchPokemonBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // CatchPokemonText
            // 
            resources.ApplyResources(this.CatchPokemonText, "CatchPokemonText");
            this.CatchPokemonText.Name = "CatchPokemonText";
            this.CatchPokemonText.Click += new System.EventHandler(this.label7_Click);
            // 
            // transferIVThresText
            // 
            resources.ApplyResources(this.transferIVThresText, "transferIVThresText");
            this.transferIVThresText.Name = "transferIVThresText";
            this.transferIVThresText.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // TravelSpeedText
            // 
            resources.ApplyResources(this.TravelSpeedText, "TravelSpeedText");
            this.TravelSpeedText.Name = "TravelSpeedText";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "SettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar trackBar;
        private CueTextBox textBoxAddress;
        private System.Windows.Forms.TextBox TravelSpeedBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label TravelSpeedText;
        private System.Windows.Forms.TextBox transferIVThresText;
        private System.Windows.Forms.TextBox AdressBox;
        private System.Windows.Forms.Button FindAdressButton;
        private System.Windows.Forms.CheckBox CatchPokemonBox;
        private System.Windows.Forms.Label CatchPokemonText;
        private System.Windows.Forms.ComboBox language;
        private System.Windows.Forms.Label label7;
    }
}
