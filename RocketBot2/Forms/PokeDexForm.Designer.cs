namespace RocketBot2.Forms
{
    partial class PokeDexForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokeDexForm));
            this.flpPokeDex = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flpPokeDex
            // 
            this.flpPokeDex.AutoScroll = true;
            this.flpPokeDex.BackColor = System.Drawing.Color.Lavender;
            this.flpPokeDex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpPokeDex.Location = new System.Drawing.Point(0, 0);
            this.flpPokeDex.Name = "flpPokeDex";
            this.flpPokeDex.Size = new System.Drawing.Size(1190, 560);
            this.flpPokeDex.TabIndex = 0;
            // 
            // PokeDexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1190, 560);
            this.Controls.Add(this.flpPokeDex);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PokeDexForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PokéDex";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpPokeDex;
    }
}