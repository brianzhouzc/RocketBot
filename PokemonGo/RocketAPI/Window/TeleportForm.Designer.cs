namespace PokemonGo.RocketAPI.Window
{
    partial class TeleportForm
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
            this.latLabel = new System.Windows.Forms.Label();
            this.longiLabel = new System.Windows.Forms.Label();
            this.longitudeText = new System.Windows.Forms.TextBox();
            this.latitudeText = new System.Windows.Forms.TextBox();
            this.teleportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // latLabel
            // 
            this.latLabel.AutoSize = true;
            this.latLabel.Location = new System.Drawing.Point(8, 15);
            this.latLabel.Name = "latLabel";
            this.latLabel.Size = new System.Drawing.Size(59, 12);
            this.latLabel.TabIndex = 15;
            this.latLabel.Text = "Latitude:";
            // 
            // longiLabel
            // 
            this.longiLabel.AutoSize = true;
            this.longiLabel.Location = new System.Drawing.Point(8, 39);
            this.longiLabel.Name = "longiLabel";
            this.longiLabel.Size = new System.Drawing.Size(65, 12);
            this.longiLabel.TabIndex = 16;
            this.longiLabel.Text = "Longitude:";
            // 
            // longitudeText
            // 
            this.longitudeText.Location = new System.Drawing.Point(84, 36);
            this.longitudeText.Name = "longitudeText";
            this.longitudeText.Size = new System.Drawing.Size(100, 21);
            this.longitudeText.TabIndex = 18;
            // 
            // latitudeText
            // 
            this.latitudeText.Location = new System.Drawing.Point(84, 12);
            this.latitudeText.Name = "latitudeText";
            this.latitudeText.Size = new System.Drawing.Size(100, 21);
            this.latitudeText.TabIndex = 17;
            // 
            // teleportButton
            // 
            this.teleportButton.Location = new System.Drawing.Point(194, 12);
            this.teleportButton.Name = "teleportButton";
            this.teleportButton.Size = new System.Drawing.Size(106, 45);
            this.teleportButton.TabIndex = 19;
            this.teleportButton.Text = "Teleport!";
            this.teleportButton.UseVisualStyleBackColor = true;
            this.teleportButton.Click += new System.EventHandler(this.teleportButton_Click);
            // 
            // TeleportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 65);
            this.Controls.Add(this.teleportButton);
            this.Controls.Add(this.latLabel);
            this.Controls.Add(this.longiLabel);
            this.Controls.Add(this.longitudeText);
            this.Controls.Add(this.latitudeText);
            this.Name = "TeleportForm";
            this.Text = "TeleportForm";
            this.Load += new System.EventHandler(this.TeleportForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label latLabel;
        private System.Windows.Forms.Label longiLabel;
        private System.Windows.Forms.TextBox longitudeText;
        private System.Windows.Forms.TextBox latitudeText;
        private System.Windows.Forms.Button teleportButton;
    }
}