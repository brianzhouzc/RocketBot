namespace PokemonGo.RocketAPI.Window {
    partial class NicknamePokemonForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.pnl = new System.Windows.Forms.Panel();
            this.txtNickname = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNickname = new System.Windows.Forms.Button();
            this.pnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl
            // 
            this.pnl.BackColor = System.Drawing.SystemColors.Window;
            this.pnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl.Controls.Add(this.txtNickname);
            this.pnl.Controls.Add(this.label1);
            this.pnl.Controls.Add(this.btnNickname);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(401, 144);
            this.pnl.TabIndex = 0;
            // 
            // txtNickname
            // 
            this.txtNickname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNickname.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNickname.Location = new System.Drawing.Point(16, 44);
            this.txtNickname.Name = "txtNickname";
            this.txtNickname.Size = new System.Drawing.Size(372, 35);
            this.txtNickname.TabIndex = 0;
            this.txtNickname.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label1.Location = new System.Drawing.Point(11, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(377, 33);
            this.label1.TabIndex = 6;
            this.label1.Text = "Nickname Pokemon";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnNickname
            // 
            this.btnNickname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNickname.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnNickname.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNickname.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNickname.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.btnNickname.Location = new System.Drawing.Point(133, 95);
            this.btnNickname.Name = "btnNickname";
            this.btnNickname.Size = new System.Drawing.Size(140, 35);
            this.btnNickname.TabIndex = 1;
            this.btnNickname.Text = "Rename";
            this.btnNickname.UseVisualStyleBackColor = true;
            // 
            // NicknamePokemonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 144);
            this.Controls.Add(this.pnl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NicknamePokemonForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NicknamePokemonForm";
            this.pnl.ResumeLayout(false);
            this.pnl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.Button btnNickname;
        public System.Windows.Forms.TextBox txtNickname;
        private System.Windows.Forms.Label label1;
    }
}