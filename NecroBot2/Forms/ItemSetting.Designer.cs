namespace NecroBot2 {
    partial class ItemSetting {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.count = new System.Windows.Forms.NumericUpDown();
            this.ItemId = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.count)).BeginInit();
            this.SuspendLayout();
            // 
            // count
            // 
            this.count.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.count.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.count.Location = new System.Drawing.Point(132, 0);
            this.count.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.count.Name = "count";
            this.count.Size = new System.Drawing.Size(75, 21);
            this.count.TabIndex = 0;
            this.count.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ItemId
            // 
            this.ItemId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemId.Location = new System.Drawing.Point(-1, -1);
            this.ItemId.Name = "ItemId";
            this.ItemId.Size = new System.Drawing.Size(127, 18);
            this.ItemId.TabIndex = 1;
            this.ItemId.Text = "Pokeball";
            this.ItemId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ItemSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ItemId);
            this.Controls.Add(this.count);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ItemSetting";
            this.Size = new System.Drawing.Size(210, 19);
            ((System.ComponentModel.ISupportInitialize)(this.count)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown count;
        private System.Windows.Forms.Label ItemId;
    }
}
