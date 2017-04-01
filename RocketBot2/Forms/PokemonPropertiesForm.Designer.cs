namespace RocketBot2.Forms
{
    partial class PokemonPropertiesForm
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
            this.pbPokemon = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.lbName = new System.Windows.Forms.Label();
            this.lbTypes = new System.Windows.Forms.Label();
            this.lbSex = new System.Windows.Forms.Label();
            this.lbShiny = new System.Windows.Forms.Label();
            this.lbMove1 = new System.Windows.Forms.Label();
            this.lbMove2 = new System.Windows.Forms.Label();
            this.lbHP = new System.Windows.Forms.Label();
            this.lbCp = new System.Windows.Forms.Label();
            this.lbIV = new System.Windows.Forms.Label();
            this.lbLevel = new System.Windows.Forms.Label();
            this.lbCaughtloc = new System.Windows.Forms.Label();
            this.lbCountry = new System.Windows.Forms.Label();
            this.lbStamina = new System.Windows.Forms.Label();
            this.lbAtk = new System.Windows.Forms.Label();
            this.lbDefense = new System.Windows.Forms.Label();
            this.lbCandy = new System.Windows.Forms.Label();
            this.lbCaughtTime = new System.Windows.Forms.Label();
            this.lbCantoEvo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbPokemon)).BeginInit();
            this.SuspendLayout();
            // 
            // pbPokemon
            // 
            this.pbPokemon.Location = new System.Drawing.Point(11, 8);
            this.pbPokemon.Name = "pbPokemon";
            this.pbPokemon.Size = new System.Drawing.Size(230, 230);
            this.pbPokemon.TabIndex = 0;
            this.pbPokemon.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(769, 290);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(103, 39);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("Segoe UI", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.ForeColor = System.Drawing.Color.Black;
            this.lbName.Location = new System.Drawing.Point(264, 8);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(103, 41);
            this.lbName.TabIndex = 2;
            this.lbName.Text = "Name";
            // 
            // lbTypes
            // 
            this.lbTypes.AutoSize = true;
            this.lbTypes.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTypes.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.lbTypes.Location = new System.Drawing.Point(267, 66);
            this.lbTypes.Name = "lbTypes";
            this.lbTypes.Size = new System.Drawing.Size(213, 28);
            this.lbTypes.TabIndex = 3;
            this.lbTypes.Text = "Types: xxxxxxx/xxxxxx";
            // 
            // lbSex
            // 
            this.lbSex.AutoSize = true;
            this.lbSex.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSex.ForeColor = System.Drawing.Color.DarkMagenta;
            this.lbSex.Location = new System.Drawing.Point(569, 18);
            this.lbSex.Name = "lbSex";
            this.lbSex.Size = new System.Drawing.Size(125, 28);
            this.lbSex.TabIndex = 4;
            this.lbSex.Text = "Sex: xxxxxxx";
            // 
            // lbShiny
            // 
            this.lbShiny.AutoSize = true;
            this.lbShiny.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbShiny.ForeColor = System.Drawing.Color.Crimson;
            this.lbShiny.Location = new System.Drawing.Point(769, 18);
            this.lbShiny.Name = "lbShiny";
            this.lbShiny.Size = new System.Drawing.Size(103, 28);
            this.lbShiny.TabIndex = 5;
            this.lbShiny.Text = "Shiny: xxx";
            // 
            // lbMove1
            // 
            this.lbMove1.AutoSize = true;
            this.lbMove1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMove1.ForeColor = System.Drawing.Color.DarkBlue;
            this.lbMove1.Location = new System.Drawing.Point(267, 107);
            this.lbMove1.Name = "lbMove1";
            this.lbMove1.Size = new System.Drawing.Size(142, 56);
            this.lbMove1.TabIndex = 6;
            this.lbMove1.Text = "Move1\r\nxxxxxxxxxxxxx";
            // 
            // lbMove2
            // 
            this.lbMove2.AutoSize = true;
            this.lbMove2.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMove2.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.lbMove2.Location = new System.Drawing.Point(569, 107);
            this.lbMove2.Name = "lbMove2";
            this.lbMove2.Size = new System.Drawing.Size(152, 56);
            this.lbMove2.TabIndex = 7;
            this.lbMove2.Text = "Move2\r\nxxxxxxxxxxxxxx";
            // 
            // lbHP
            // 
            this.lbHP.AutoSize = true;
            this.lbHP.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHP.ForeColor = System.Drawing.Color.Coral;
            this.lbHP.Location = new System.Drawing.Point(6, 244);
            this.lbHP.Name = "lbHP";
            this.lbHP.Size = new System.Drawing.Size(118, 28);
            this.lbHP.TabIndex = 8;
            this.lbHP.Text = "HP: xxx/xxx";
            // 
            // lbCp
            // 
            this.lbCp.AutoSize = true;
            this.lbCp.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCp.ForeColor = System.Drawing.Color.DarkCyan;
            this.lbCp.Location = new System.Drawing.Point(144, 244);
            this.lbCp.Name = "lbCp";
            this.lbCp.Size = new System.Drawing.Size(97, 28);
            this.lbCp.TabIndex = 9;
            this.lbCp.Text = "CP: xxxxx";
            // 
            // lbIV
            // 
            this.lbIV.AutoSize = true;
            this.lbIV.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIV.ForeColor = System.Drawing.Color.Crimson;
            this.lbIV.Location = new System.Drawing.Point(266, 244);
            this.lbIV.Name = "lbIV";
            this.lbIV.Size = new System.Drawing.Size(82, 28);
            this.lbIV.TabIndex = 10;
            this.lbIV.Text = "IV: xxxx";
            // 
            // lbLevel
            // 
            this.lbLevel.AutoSize = true;
            this.lbLevel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLevel.ForeColor = System.Drawing.Color.DarkMagenta;
            this.lbLevel.Location = new System.Drawing.Point(380, 244);
            this.lbLevel.Name = "lbLevel";
            this.lbLevel.Size = new System.Drawing.Size(100, 28);
            this.lbLevel.TabIndex = 11;
            this.lbLevel.Text = "Level: xxx";
            // 
            // lbCaughtloc
            // 
            this.lbCaughtloc.AutoSize = true;
            this.lbCaughtloc.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCaughtloc.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbCaughtloc.Location = new System.Drawing.Point(266, 172);
            this.lbCaughtloc.Name = "lbCaughtloc";
            this.lbCaughtloc.Size = new System.Drawing.Size(372, 28);
            this.lbCaughtloc.TabIndex = 12;
            this.lbCaughtloc.Text = "Caught Location: xx.xxxxxx, xxx.xxxxxxx";
            // 
            // lbCountry
            // 
            this.lbCountry.AutoSize = true;
            this.lbCountry.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCountry.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbCountry.Location = new System.Drawing.Point(267, 210);
            this.lbCountry.Name = "lbCountry";
            this.lbCountry.Size = new System.Drawing.Size(166, 28);
            this.lbCountry.TabIndex = 13;
            this.lbCountry.Text = "Country: xxxxxxx";
            // 
            // lbStamina
            // 
            this.lbStamina.AutoSize = true;
            this.lbStamina.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStamina.ForeColor = System.Drawing.Color.Red;
            this.lbStamina.Location = new System.Drawing.Point(499, 244);
            this.lbStamina.Name = "lbStamina";
            this.lbStamina.Size = new System.Drawing.Size(115, 28);
            this.lbStamina.TabIndex = 14;
            this.lbStamina.Text = "Stamina: xx";
            // 
            // lbAtk
            // 
            this.lbAtk.AutoSize = true;
            this.lbAtk.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAtk.ForeColor = System.Drawing.Color.Green;
            this.lbAtk.Location = new System.Drawing.Point(630, 244);
            this.lbAtk.Name = "lbAtk";
            this.lbAtk.Size = new System.Drawing.Size(101, 28);
            this.lbAtk.TabIndex = 15;
            this.lbAtk.Text = "Attack: xx";
            // 
            // lbDefense
            // 
            this.lbDefense.AutoSize = true;
            this.lbDefense.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDefense.ForeColor = System.Drawing.Color.SaddleBrown;
            this.lbDefense.Location = new System.Drawing.Point(754, 244);
            this.lbDefense.Name = "lbDefense";
            this.lbDefense.Size = new System.Drawing.Size(118, 28);
            this.lbDefense.TabIndex = 16;
            this.lbDefense.Text = "Defence: xx";
            // 
            // lbCandy
            // 
            this.lbCandy.AutoSize = true;
            this.lbCandy.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCandy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbCandy.Location = new System.Drawing.Point(723, 172);
            this.lbCandy.Name = "lbCandy";
            this.lbCandy.Size = new System.Drawing.Size(149, 28);
            this.lbCandy.TabIndex = 17;
            this.lbCandy.Text = "Candy: xxxxxxx";
            // 
            // lbCaughtTime
            // 
            this.lbCaughtTime.AutoSize = true;
            this.lbCaughtTime.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCaughtTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbCaughtTime.Location = new System.Drawing.Point(499, 66);
            this.lbCaughtTime.Name = "lbCaughtTime";
            this.lbCaughtTime.Size = new System.Drawing.Size(219, 28);
            this.lbCaughtTime.TabIndex = 18;
            this.lbCaughtTime.Text = "Caught Time: xx:xx,xxx";
            // 
            // lbCantoEvo
            // 
            this.lbCantoEvo.AutoSize = true;
            this.lbCantoEvo.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCantoEvo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbCantoEvo.Location = new System.Drawing.Point(630, 210);
            this.lbCantoEvo.Name = "lbCantoEvo";
            this.lbCantoEvo.Size = new System.Drawing.Size(198, 28);
            this.lbCantoEvo.TabIndex = 19;
            this.lbCantoEvo.Text = "Candy to Evolve: xxx";
            // 
            // PokemonPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 341);
            this.Controls.Add(this.lbCantoEvo);
            this.Controls.Add(this.lbCaughtTime);
            this.Controls.Add(this.lbCandy);
            this.Controls.Add(this.lbDefense);
            this.Controls.Add(this.lbAtk);
            this.Controls.Add(this.lbStamina);
            this.Controls.Add(this.lbCountry);
            this.Controls.Add(this.lbCaughtloc);
            this.Controls.Add(this.lbLevel);
            this.Controls.Add(this.lbIV);
            this.Controls.Add(this.lbCp);
            this.Controls.Add(this.lbHP);
            this.Controls.Add(this.lbMove2);
            this.Controls.Add(this.lbMove1);
            this.Controls.Add(this.lbShiny);
            this.Controls.Add(this.lbSex);
            this.Controls.Add(this.lbTypes);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pbPokemon);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "PokemonPropertiesForm";
            this.Text = "PokemonProperties";
            ((System.ComponentModel.ISupportInitialize)(this.pbPokemon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbPokemon;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbTypes;
        private System.Windows.Forms.Label lbSex;
        private System.Windows.Forms.Label lbShiny;
        private System.Windows.Forms.Label lbMove1;
        private System.Windows.Forms.Label lbMove2;
        private System.Windows.Forms.Label lbHP;
        private System.Windows.Forms.Label lbCp;
        private System.Windows.Forms.Label lbIV;
        private System.Windows.Forms.Label lbLevel;
        private System.Windows.Forms.Label lbCaughtloc;
        private System.Windows.Forms.Label lbCountry;
        private System.Windows.Forms.Label lbStamina;
        private System.Windows.Forms.Label lbAtk;
        private System.Windows.Forms.Label lbDefense;
        private System.Windows.Forms.Label lbCandy;
        private System.Windows.Forms.Label lbCaughtTime;
        private System.Windows.Forms.Label lbCantoEvo;
    }
}