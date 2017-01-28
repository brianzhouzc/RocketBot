namespace RocketBot2.Forms
{
    partial class StarterConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StarterConfigForm));
            this.wizardControl1 = new AeroWizard.WizardControl();
            this.SelectLanguagePage = new AeroWizard.WizardPage();
            this.txtLanguage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AccountPage = new AeroWizard.WizardPage();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.LocationPage = new AeroWizard.WizardPage();
            this.txtLng = new System.Windows.Forms.TextBox();
            this.txtLat = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.WalkinSpeedPage = new AeroWizard.WizardPage();
            this.chkAllowVariant = new System.Windows.Forms.CheckBox();
            this.txtVariant = new System.Windows.Forms.TextBox();
            this.txtSpeed = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.WebSocketPage = new AeroWizard.WizardPage();
            this.txtWebsocketPort = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.chkAllowWebsocket = new System.Windows.Forms.CheckBox();
            this.wizardPage1 = new AeroWizard.WizardPage();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).BeginInit();
            this.SelectLanguagePage.SuspendLayout();
            this.AccountPage.SuspendLayout();
            this.LocationPage.SuspendLayout();
            this.WalkinSpeedPage.SuspendLayout();
            this.WebSocketPage.SuspendLayout();
            this.wizardPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizardControl1
            // 
            this.wizardControl1.BackColor = System.Drawing.Color.White;
            this.wizardControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardControl1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wizardControl1.Location = new System.Drawing.Point(0, 0);
            this.wizardControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.wizardControl1.Name = "wizardControl1";
            this.wizardControl1.Pages.Add(this.SelectLanguagePage);
            this.wizardControl1.Pages.Add(this.AccountPage);
            this.wizardControl1.Pages.Add(this.LocationPage);
            this.wizardControl1.Pages.Add(this.WalkinSpeedPage);
            this.wizardControl1.Pages.Add(this.WebSocketPage);
            this.wizardControl1.Pages.Add(this.wizardPage1);
            this.wizardControl1.Size = new System.Drawing.Size(732, 423);
            this.wizardControl1.TabIndex = 0;
            this.wizardControl1.Text = "Initial first time config";
            this.wizardControl1.Title = "Initial first time config";
            this.wizardControl1.Cancelling += new System.ComponentModel.CancelEventHandler(this.wizardControl1_Cancelling);
            this.wizardControl1.Finished += new System.EventHandler(this.wizardControl1_Finished);
            // 
            // SelectLanguagePage
            // 
            this.SelectLanguagePage.Controls.Add(this.txtLanguage);
            this.SelectLanguagePage.Controls.Add(this.label1);
            this.SelectLanguagePage.Name = "SelectLanguagePage";
            this.SelectLanguagePage.NextPage = this.AccountPage;
            this.SelectLanguagePage.Size = new System.Drawing.Size(685, 236);
            this.SelectLanguagePage.TabIndex = 0;
            this.SelectLanguagePage.Text = "Welcome to RocketBot2";
            this.SelectLanguagePage.Commit += new System.EventHandler<AeroWizard.WizardPageConfirmEventArgs>(this.SelectLanguagePage_Commit);
            this.SelectLanguagePage.Click += new System.EventHandler(this.SelectLanguagePage_Click);
            // 
            // txtLanguage
            // 
            this.txtLanguage.Location = new System.Drawing.Point(296, 14);
            this.txtLanguage.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtLanguage.Name = "txtLanguage";
            this.txtLanguage.Size = new System.Drawing.Size(132, 27);
            this.txtLanguage.TabIndex = 1;
            this.txtLanguage.Text = "en";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select your prefer language";
            // 
            // AccountPage
            // 
            this.AccountPage.Controls.Add(this.txtPassword);
            this.AccountPage.Controls.Add(this.txtUsername);
            this.AccountPage.Controls.Add(this.label4);
            this.AccountPage.Controls.Add(this.label3);
            this.AccountPage.Controls.Add(this.label2);
            this.AccountPage.Controls.Add(this.comboBox1);
            this.AccountPage.Name = "AccountPage";
            this.AccountPage.Size = new System.Drawing.Size(669, 234);
            this.AccountPage.TabIndex = 1;
            this.AccountPage.Text = "Account setup";
            this.AccountPage.Commit += new System.EventHandler<AeroWizard.WizardPageConfirmEventArgs>(this.wizardPage2_Commit);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(179, 126);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(215, 27);
            this.txtPassword.TabIndex = 5;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(179, 78);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(215, 27);
            this.txtUsername.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 126);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 81);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Account Type";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "google",
            "ptc"});
            this.comboBox1.Location = new System.Drawing.Point(179, 25);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(215, 28);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.Text = "ptc";
            // 
            // LocationPage
            // 
            this.LocationPage.Controls.Add(this.txtLng);
            this.LocationPage.Controls.Add(this.txtLat);
            this.LocationPage.Controls.Add(this.label5);
            this.LocationPage.Controls.Add(this.label6);
            this.LocationPage.Name = "LocationPage";
            this.LocationPage.Size = new System.Drawing.Size(669, 234);
            this.LocationPage.TabIndex = 3;
            this.LocationPage.Text = "Start location";
            this.LocationPage.Click += new System.EventHandler(this.wizardPage4_Click);
            // 
            // txtLng
            // 
            this.txtLng.Location = new System.Drawing.Point(167, 65);
            this.txtLng.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtLng.Name = "txtLng";
            this.txtLng.Size = new System.Drawing.Size(215, 27);
            this.txtLng.TabIndex = 13;
            // 
            // txtLat
            // 
            this.txtLat.Location = new System.Drawing.Point(167, 21);
            this.txtLat.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtLat.Name = "txtLat";
            this.txtLat.Size = new System.Drawing.Size(215, 27);
            this.txtLat.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 65);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "Longitude";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 21);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "Latitude";
            // 
            // WalkinSpeedPage
            // 
            this.WalkinSpeedPage.Controls.Add(this.chkAllowVariant);
            this.WalkinSpeedPage.Controls.Add(this.txtVariant);
            this.WalkinSpeedPage.Controls.Add(this.txtSpeed);
            this.WalkinSpeedPage.Controls.Add(this.label7);
            this.WalkinSpeedPage.Controls.Add(this.label8);
            this.WalkinSpeedPage.Name = "WalkinSpeedPage";
            this.WalkinSpeedPage.Size = new System.Drawing.Size(669, 234);
            this.WalkinSpeedPage.TabIndex = 4;
            this.WalkinSpeedPage.Text = "Walking setting";
            this.WalkinSpeedPage.Click += new System.EventHandler(this.WalkinSpeedPage_Click);
            // 
            // chkAllowVariant
            // 
            this.chkAllowVariant.AutoSize = true;
            this.chkAllowVariant.Checked = true;
            this.chkAllowVariant.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAllowVariant.Location = new System.Drawing.Point(47, 117);
            this.chkAllowVariant.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkAllowVariant.Name = "chkAllowVariant";
            this.chkAllowVariant.Size = new System.Drawing.Size(196, 24);
            this.chkAllowVariant.TabIndex = 18;
            this.chkAllowVariant.Text = "Allow walk speed variant";
            this.chkAllowVariant.UseVisualStyleBackColor = true;
            this.chkAllowVariant.Click += new System.EventHandler(this.chkAllowVariant_Click);
            // 
            // txtVariant
            // 
            this.txtVariant.Location = new System.Drawing.Point(181, 73);
            this.txtVariant.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtVariant.Name = "txtVariant";
            this.txtVariant.Size = new System.Drawing.Size(215, 27);
            this.txtVariant.TabIndex = 17;
            this.txtVariant.Text = "0";
            // 
            // txtSpeed
            // 
            this.txtSpeed.Location = new System.Drawing.Point(181, 28);
            this.txtSpeed.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.Size = new System.Drawing.Size(215, 27);
            this.txtSpeed.TabIndex = 16;
            this.txtSpeed.Text = "30";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(43, 73);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "Speed variant";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(39, 28);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 20);
            this.label8.TabIndex = 14;
            this.label8.Text = "Walk Speed";
            // 
            // WebSocketPage
            // 
            this.WebSocketPage.Controls.Add(this.txtWebsocketPort);
            this.WebSocketPage.Controls.Add(this.label9);
            this.WebSocketPage.Controls.Add(this.chkAllowWebsocket);
            this.WebSocketPage.Name = "WebSocketPage";
            this.WebSocketPage.NextPage = this.wizardPage1;
            this.WebSocketPage.Size = new System.Drawing.Size(669, 234);
            this.WebSocketPage.TabIndex = 2;
            this.WebSocketPage.Text = "Websocket";
            this.WebSocketPage.Click += new System.EventHandler(this.WebSocketPage_Click);
            // 
            // txtWebsocketPort
            // 
            this.txtWebsocketPort.Location = new System.Drawing.Point(203, 89);
            this.txtWebsocketPort.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtWebsocketPort.Name = "txtWebsocketPort";
            this.txtWebsocketPort.Size = new System.Drawing.Size(215, 27);
            this.txtWebsocketPort.TabIndex = 18;
            this.txtWebsocketPort.Text = "30";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(60, 89);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(117, 20);
            this.label9.TabIndex = 17;
            this.label9.Text = "Web Socket Port";
            // 
            // chkAllowWebsocket
            // 
            this.chkAllowWebsocket.AutoSize = true;
            this.chkAllowWebsocket.Location = new System.Drawing.Point(64, 36);
            this.chkAllowWebsocket.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkAllowWebsocket.Name = "chkAllowWebsocket";
            this.chkAllowWebsocket.Size = new System.Drawing.Size(264, 24);
            this.chkAllowWebsocket.TabIndex = 0;
            this.chkAllowWebsocket.Text = "Allow websocket (use for Web GUI)";
            this.chkAllowWebsocket.UseVisualStyleBackColor = true;
            this.chkAllowWebsocket.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // wizardPage1
            // 
            this.wizardPage1.AllowBack = false;
            this.wizardPage1.AllowCancel = false;
            this.wizardPage1.AllowNext = false;
            this.wizardPage1.Controls.Add(this.button1);
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.ShowCancel = false;
            this.wizardPage1.ShowNext = false;
            this.wizardPage1.Size = new System.Drawing.Size(669, 234);
            this.wizardPage1.TabIndex = 5;
            this.wizardPage1.Text = "You are all set!";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(224, 87);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(192, 64);
            this.button1.TabIndex = 0;
            this.button1.Text = "START BOT";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // StarterConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 423);
            this.ControlBox = false;
            this.Controls.Add(this.wizardControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "StarterConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Initial Necrobot2 Config";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).EndInit();
            this.SelectLanguagePage.ResumeLayout(false);
            this.SelectLanguagePage.PerformLayout();
            this.AccountPage.ResumeLayout(false);
            this.AccountPage.PerformLayout();
            this.LocationPage.ResumeLayout(false);
            this.LocationPage.PerformLayout();
            this.WalkinSpeedPage.ResumeLayout(false);
            this.WalkinSpeedPage.PerformLayout();
            this.WebSocketPage.ResumeLayout(false);
            this.WebSocketPage.PerformLayout();
            this.wizardPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AeroWizard.WizardControl wizardControl1;
        private AeroWizard.WizardPage SelectLanguagePage;
        private System.Windows.Forms.Label label1;
        private AeroWizard.WizardPage AccountPage;
        private AeroWizard.WizardPage WebSocketPage;
        private System.Windows.Forms.TextBox txtLanguage;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private AeroWizard.WizardPage LocationPage;
        private System.Windows.Forms.TextBox txtLng;
        private System.Windows.Forms.TextBox txtLat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private AeroWizard.WizardPage WalkinSpeedPage;
        private System.Windows.Forms.TextBox txtVariant;
        private System.Windows.Forms.TextBox txtSpeed;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtWebsocketPort;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkAllowWebsocket;
        private System.Windows.Forms.CheckBox chkAllowVariant;
        private AeroWizard.WizardPage wizardPage1;
        private System.Windows.Forms.Button button1;
    }
}