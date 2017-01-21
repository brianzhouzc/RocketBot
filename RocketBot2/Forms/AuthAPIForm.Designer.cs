namespace RocketBot2.Forms
{
    partial class AuthAPIForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAPIKey = new System.Windows.Forms.TextBox();
            this.radLegacy = new System.Windows.Forms.RadioButton();
            this.lnkBuy = new System.Windows.Forms.LinkLabel();
            this.radHashServer = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(61, 226);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please select your API method";
            // 
            // txtAPIKey
            // 
            this.txtAPIKey.Location = new System.Drawing.Point(12, 115);
            this.txtAPIKey.Name = "txtAPIKey";
            this.txtAPIKey.Size = new System.Drawing.Size(228, 20);
            this.txtAPIKey.TabIndex = 2;
            // 
            // radLegacy
            // 
            this.radLegacy.AutoSize = true;
            this.radLegacy.Location = new System.Drawing.Point(15, 40);
            this.radLegacy.Name = "radLegacy";
            this.radLegacy.Size = new System.Drawing.Size(154, 17);
            this.radLegacy.TabIndex = 3;
            this.radLegacy.TabStop = true;
            this.radLegacy.Text = "Legacy 0.45 API - FREE (High risk)";
            this.radLegacy.UseVisualStyleBackColor = true;
            // 
            // lnkBuy
            // 
            this.lnkBuy.AutoSize = true;
            this.lnkBuy.Location = new System.Drawing.Point(246, 118);
            this.lnkBuy.Name = "lnkBuy";
            this.lnkBuy.Size = new System.Drawing.Size(25, 13);
            this.lnkBuy.TabIndex = 4;
            this.lnkBuy.TabStop = true;
            this.lnkBuy.Text = "Buy";
            this.lnkBuy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBuy_LinkClicked);
            // 
            // radHashServer
            // 
            this.radHashServer.AutoSize = true;
            this.radHashServer.Location = new System.Drawing.Point(15, 74);
            this.radHashServer.Name = "radHashServer";
            this.radHashServer.Size = new System.Drawing.Size(225, 17);
            this.radHashServer.TabIndex = 5;
            this.radHashServer.TabStop = true;
            this.radHashServer.Text = "Pogodev hash server api - Latest API 0.51";
            this.radHashServer.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "API Key";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 149);
            this.label3.MaximumSize = new System.Drawing.Size(270, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(252, 52);
            this.label3.TabIndex = 7;
            this.label3.Text = "We don\'t provide key, you have to buy it  from Pogodev. RPM = Request per minute," +
    " it depend on how fast your config setup.  150RPM will sufficient enought for 2-" +
    "3 normal bot. ";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(142, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // AuthAPIForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.radHashServer);
            this.Controls.Add(this.lnkBuy);
            this.Controls.Add(this.radLegacy);
            this.Controls.Add(this.txtAPIKey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Name = "AuthAPIForm";
            this.ShowInTaskbar = false;
            this.Text = "APIConfig";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAPIKey;
        private System.Windows.Forms.RadioButton radLegacy;
        private System.Windows.Forms.LinkLabel lnkBuy;
        private System.Windows.Forms.RadioButton radHashServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancel;
    }
}