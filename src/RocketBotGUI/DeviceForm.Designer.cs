namespace PokemonGo.RocketAPI.Window
{
    partial class DeviceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceForm));
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
            this.label6 = new System.Windows.Forms.Label();
            this.DeviceBrandTb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.AndroidBootloaderTb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.AndroidBoardNameTb = new System.Windows.Forms.TextBox();
            this.BoardName = new System.Windows.Forms.Label();
            this.DeviceIdTb = new System.Windows.Forms.TextBox();
            this.deviceIdlb = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RandomIDBtn
            // 
            this.RandomIDBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.RandomIDBtn.Location = new System.Drawing.Point(322, 106);
            this.RandomIDBtn.Name = "RandomIDBtn";
            this.RandomIDBtn.Size = new System.Drawing.Size(87, 25);
            this.RandomIDBtn.TabIndex = 35;
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
            this.deviceTypeCb.Location = new System.Drawing.Point(166, 80);
            this.deviceTypeCb.Name = "deviceTypeCb";
            this.deviceTypeCb.Size = new System.Drawing.Size(150, 23);
            this.deviceTypeCb.TabIndex = 7;
            this.deviceTypeCb.SelectedIndexChanged += new System.EventHandler(this.deviceTypeCb_SelectedIndexChanged);
            // 
            // RandomDeviceBtn
            // 
            this.RandomDeviceBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.RandomDeviceBtn.Location = new System.Drawing.Point(12, 489);
            this.RandomDeviceBtn.Name = "RandomDeviceBtn";
            this.RandomDeviceBtn.Size = new System.Drawing.Size(402, 64);
            this.RandomDeviceBtn.TabIndex = 34;
            this.RandomDeviceBtn.Text = "I am feeling RICH";
            this.RandomDeviceBtn.UseVisualStyleBackColor = true;
            this.RandomDeviceBtn.Click += new System.EventHandler(this.RandomDeviceBtn_Click);
            // 
            // FirmwareFingerprintTb
            // 
            this.FirmwareFingerprintTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FirmwareFingerprintTb.Location = new System.Drawing.Point(166, 459);
            this.FirmwareFingerprintTb.Name = "FirmwareFingerprintTb";
            this.FirmwareFingerprintTb.Size = new System.Drawing.Size(243, 21);
            this.FirmwareFingerprintTb.TabIndex = 32;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label14.Location = new System.Drawing.Point(15, 463);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(121, 15);
            this.label14.TabIndex = 19;
            this.label14.Text = "Firmware Fingerprint";
            // 
            // FirmwareTypeTb
            // 
            this.FirmwareTypeTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FirmwareTypeTb.Location = new System.Drawing.Point(166, 430);
            this.FirmwareTypeTb.Name = "FirmwareTypeTb";
            this.FirmwareTypeTb.Size = new System.Drawing.Size(243, 21);
            this.FirmwareTypeTb.TabIndex = 28;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label13.Location = new System.Drawing.Point(15, 433);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(88, 15);
            this.label13.TabIndex = 21;
            this.label13.Text = "Firmware Type";
            // 
            // FirmwareTagsTb
            // 
            this.FirmwareTagsTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FirmwareTagsTb.Location = new System.Drawing.Point(166, 401);
            this.FirmwareTagsTb.Name = "FirmwareTagsTb";
            this.FirmwareTagsTb.Size = new System.Drawing.Size(243, 21);
            this.FirmwareTagsTb.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label12.Location = new System.Drawing.Point(15, 404);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 15);
            this.label12.TabIndex = 20;
            this.label12.Text = "Firmware Tags";
            // 
            // FirmwareBrandTb
            // 
            this.FirmwareBrandTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FirmwareBrandTb.Location = new System.Drawing.Point(166, 372);
            this.FirmwareBrandTb.Name = "FirmwareBrandTb";
            this.FirmwareBrandTb.Size = new System.Drawing.Size(243, 21);
            this.FirmwareBrandTb.TabIndex = 22;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label11.Location = new System.Drawing.Point(15, 375);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(95, 15);
            this.label11.TabIndex = 18;
            this.label11.Text = "Firmware Brand";
            // 
            // HardwareModelTb
            // 
            this.HardwareModelTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.HardwareModelTb.Location = new System.Drawing.Point(166, 342);
            this.HardwareModelTb.Name = "HardwareModelTb";
            this.HardwareModelTb.Size = new System.Drawing.Size(243, 21);
            this.HardwareModelTb.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label10.Location = new System.Drawing.Point(15, 346);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(99, 15);
            this.label10.TabIndex = 16;
            this.label10.Text = "Hardware Model";
            // 
            // HardwareManufacturerTb
            // 
            this.HardwareManufacturerTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.HardwareManufacturerTb.Location = new System.Drawing.Point(166, 313);
            this.HardwareManufacturerTb.Name = "HardwareManufacturerTb";
            this.HardwareManufacturerTb.Size = new System.Drawing.Size(243, 21);
            this.HardwareManufacturerTb.TabIndex = 30;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label9.Location = new System.Drawing.Point(15, 316);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 15);
            this.label9.TabIndex = 17;
            this.label9.Text = "Hardware Manu facturer";
            // 
            // DeviceModelBootTb
            // 
            this.DeviceModelBootTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceModelBootTb.Location = new System.Drawing.Point(166, 284);
            this.DeviceModelBootTb.Name = "DeviceModelBootTb";
            this.DeviceModelBootTb.Size = new System.Drawing.Size(243, 21);
            this.DeviceModelBootTb.TabIndex = 33;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label8.Location = new System.Drawing.Point(15, 287);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 15);
            this.label8.TabIndex = 14;
            this.label8.Text = "Device Model Boot";
            // 
            // DeviceModelIdentifierTb
            // 
            this.DeviceModelIdentifierTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceModelIdentifierTb.Location = new System.Drawing.Point(166, 255);
            this.DeviceModelIdentifierTb.Name = "DeviceModelIdentifierTb";
            this.DeviceModelIdentifierTb.Size = new System.Drawing.Size(243, 21);
            this.DeviceModelIdentifierTb.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label7.Location = new System.Drawing.Point(15, 258);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(132, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "Device Model Identifier";
            // 
            // DeviceModelTb
            // 
            this.DeviceModelTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceModelTb.Location = new System.Drawing.Point(166, 225);
            this.DeviceModelTb.Name = "DeviceModelTb";
            this.DeviceModelTb.Size = new System.Drawing.Size(243, 21);
            this.DeviceModelTb.TabIndex = 25;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label6.Location = new System.Drawing.Point(15, 229);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "Device Model";
            // 
            // DeviceBrandTb
            // 
            this.DeviceBrandTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceBrandTb.Location = new System.Drawing.Point(166, 196);
            this.DeviceBrandTb.Name = "DeviceBrandTb";
            this.DeviceBrandTb.Size = new System.Drawing.Size(243, 21);
            this.DeviceBrandTb.TabIndex = 27;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label5.Location = new System.Drawing.Point(15, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 15);
            this.label5.TabIndex = 11;
            this.label5.Text = "Device Brand";
            // 
            // AndroidBootloaderTb
            // 
            this.AndroidBootloaderTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.AndroidBootloaderTb.Location = new System.Drawing.Point(166, 167);
            this.AndroidBootloaderTb.Name = "AndroidBootloaderTb";
            this.AndroidBootloaderTb.Size = new System.Drawing.Size(243, 21);
            this.AndroidBootloaderTb.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label4.Location = new System.Drawing.Point(15, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Android Boot loader";
            // 
            // AndroidBoardNameTb
            // 
            this.AndroidBoardNameTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.AndroidBoardNameTb.Location = new System.Drawing.Point(166, 138);
            this.AndroidBoardNameTb.Name = "AndroidBoardNameTb";
            this.AndroidBoardNameTb.Size = new System.Drawing.Size(243, 21);
            this.AndroidBoardNameTb.TabIndex = 31;
            // 
            // BoardName
            // 
            this.BoardName.AutoSize = true;
            this.BoardName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.BoardName.Location = new System.Drawing.Point(15, 141);
            this.BoardName.Name = "BoardName";
            this.BoardName.Size = new System.Drawing.Size(122, 15);
            this.BoardName.TabIndex = 9;
            this.BoardName.Text = "Android Board Name";
            // 
            // DeviceIdTb
            // 
            this.DeviceIdTb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DeviceIdTb.Location = new System.Drawing.Point(166, 108);
            this.DeviceIdTb.Name = "DeviceIdTb";
            this.DeviceIdTb.Size = new System.Drawing.Size(150, 21);
            this.DeviceIdTb.TabIndex = 8;
            // 
            // deviceIdlb
            // 
            this.deviceIdlb.AutoSize = true;
            this.deviceIdlb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.deviceIdlb.Location = new System.Drawing.Point(15, 112);
            this.deviceIdlb.Name = "deviceIdlb";
            this.deviceIdlb.Size = new System.Drawing.Size(59, 15);
            this.deviceIdlb.TabIndex = 15;
            this.deviceIdlb.Text = "Device ID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label1.Location = new System.Drawing.Point(15, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Device Type:";
            // 
            // SaveBtn
            // 
            this.SaveBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.SaveBtn.Location = new System.Drawing.Point(12, 559);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(402, 64);
            this.SaveBtn.TabIndex = 36;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label2.Location = new System.Drawing.Point(9, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 15);
            this.label2.TabIndex = 37;
            this.label2.Text = "For your account safety.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label3.Location = new System.Drawing.Point(9, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 18);
            this.label3.TabIndex = 38;
            this.label3.Text = "Important:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label15.Location = new System.Drawing.Point(9, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(312, 15);
            this.label15.TabIndex = 39;
            this.label15.Text = "Please do not change your account infomation too often.";
            // 
            // DeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 630);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.RandomIDBtn);
            this.Controls.Add(this.deviceTypeCb);
            this.Controls.Add(this.RandomDeviceBtn);
            this.Controls.Add(this.FirmwareFingerprintTb);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.FirmwareTypeTb);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.FirmwareTagsTb);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.FirmwareBrandTb);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.HardwareModelTb);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.HardwareManufacturerTb);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.DeviceModelBootTb);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.DeviceModelIdentifierTb);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.DeviceModelTb);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.DeviceBrandTb);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.AndroidBootloaderTb);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.AndroidBoardNameTb);
            this.Controls.Add(this.BoardName);
            this.Controls.Add(this.DeviceIdTb);
            this.Controls.Add(this.deviceIdlb);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DeviceSetting";
            this.Load += new System.EventHandler(this.DeviceForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox DeviceBrandTb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox AndroidBootloaderTb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox AndroidBoardNameTb;
        private System.Windows.Forms.Label BoardName;
        private System.Windows.Forms.TextBox DeviceIdTb;
        private System.Windows.Forms.Label deviceIdlb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label15;
    }
}