namespace RocketBot2.Forms
{
    partial class EggsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EggsForm));
            this.flpEggs = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flpEggs
            // 
            this.flpEggs.AutoScroll = true;
            this.flpEggs.BackColor = System.Drawing.Color.Honeydew;
            this.flpEggs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpEggs.Location = new System.Drawing.Point(0, 0);
            this.flpEggs.Name = "flpEggs";
            this.flpEggs.Size = new System.Drawing.Size(468, 354);
            this.flpEggs.TabIndex = 0;
            // 
            // EggsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 354);
            this.Controls.Add(this.flpEggs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EggsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Eggs...";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.FlowLayoutPanel flpEggs;
    }
}