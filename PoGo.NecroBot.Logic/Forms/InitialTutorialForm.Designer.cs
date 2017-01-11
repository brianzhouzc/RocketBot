namespace PoGo.NecroBot.Logic.Forms
{
    partial class InitialTutorialForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitialTutorialForm));
            this.wizardControl1 = new AeroWizard.WizardControl();
            this.Step1 = new AeroWizard.WizardPage();
            this.rdoFemale = new System.Windows.Forms.RadioButton();
            this.rdoMale = new System.Windows.Forms.RadioButton();
            this.Step1_1 = new AeroWizard.WizardPage();
            this.label2 = new System.Windows.Forms.Label();
            this.Step2 = new AeroWizard.WizardPage();
            this.rdoCharmander = new System.Windows.Forms.RadioButton();
            this.rdoSquirle = new System.Windows.Forms.RadioButton();
            this.rdoBulbasaur = new System.Windows.Forms.RadioButton();
            this.Step2_2 = new AeroWizard.WizardPage();
            this.label3 = new System.Windows.Forms.Label();
            this.Step3 = new AeroWizard.WizardPage();
            this.lblNameError = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNick = new System.Windows.Forms.TextBox();
            this.Step3_3 = new AeroWizard.WizardPage();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).BeginInit();
            this.Step1.SuspendLayout();
            this.Step1_1.SuspendLayout();
            this.Step2.SuspendLayout();
            this.Step2_2.SuspendLayout();
            this.Step3.SuspendLayout();
            this.Step3_3.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizardControl1
            // 
            this.wizardControl1.BackColor = System.Drawing.Color.White;
            this.wizardControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardControl1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wizardControl1.Location = new System.Drawing.Point(0, 0);
            this.wizardControl1.Name = "wizardControl1";
            this.wizardControl1.Pages.Add(this.Step1);
            this.wizardControl1.Pages.Add(this.Step1_1);
            this.wizardControl1.Pages.Add(this.Step2);
            this.wizardControl1.Pages.Add(this.Step2_2);
            this.wizardControl1.Pages.Add(this.Step3);
            this.wizardControl1.Pages.Add(this.Step3_3);
            this.wizardControl1.Size = new System.Drawing.Size(517, 270);
            this.wizardControl1.TabIndex = 0;
            this.wizardControl1.Text = "Settin";
            this.wizardControl1.Title = "Beginner Tutorial";
            // 
            // Step1
            // 
            this.Step1.Controls.Add(this.rdoFemale);
            this.Step1.Controls.Add(this.rdoMale);
            this.Step1.Name = "Step1";
            this.Step1.Size = new System.Drawing.Size(470, 116);
            this.Step1.TabIndex = 0;
            this.Step1.Text = "Select  Gender";
            this.Step1.Initialize += new System.EventHandler<AeroWizard.WizardPageInitEventArgs>(this.wizardPage1_Initialize);
            // 
            // rdoFemale
            // 
            this.rdoFemale.AutoSize = true;
            this.rdoFemale.Location = new System.Drawing.Point(193, 25);
            this.rdoFemale.Name = "rdoFemale";
            this.rdoFemale.Size = new System.Drawing.Size(63, 19);
            this.rdoFemale.TabIndex = 1;
            this.rdoFemale.Text = "Female";
            this.rdoFemale.UseVisualStyleBackColor = true;
            this.rdoFemale.UseWaitCursor = true;
            // 
            // rdoMale
            // 
            this.rdoMale.AutoSize = true;
            this.rdoMale.Checked = true;
            this.rdoMale.Location = new System.Drawing.Point(26, 24);
            this.rdoMale.Name = "rdoMale";
            this.rdoMale.Size = new System.Drawing.Size(51, 19);
            this.rdoMale.TabIndex = 0;
            this.rdoMale.TabStop = true;
            this.rdoMale.Text = "Male";
            this.rdoMale.UseVisualStyleBackColor = true;
            this.rdoMale.UseWaitCursor = true;
            // 
            // Step1_1
            // 
            this.Step1_1.AllowBack = false;
            this.Step1_1.AllowNext = false;
            this.Step1_1.Controls.Add(this.label2);
            this.Step1_1.Name = "Step1_1";
            this.Step1_1.ShowNext = false;
            this.Step1_1.Size = new System.Drawing.Size(470, 116);
            this.Step1_1.TabIndex = 3;
            this.Step1_1.Text = "Waiting Gender ";
            this.Step1_1.Initialize += new System.EventHandler<AeroWizard.WizardPageInitEventArgs>(this.wizardPage4_Initialize);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(99, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Please wait........";
            // 
            // Step2
            // 
            this.Step2.AllowBack = false;
            this.Step2.AllowCancel = false;
            this.Step2.Controls.Add(this.rdoCharmander);
            this.Step2.Controls.Add(this.rdoSquirle);
            this.Step2.Controls.Add(this.rdoBulbasaur);
            this.Step2.Name = "Step2";
            this.Step2.ShowCancel = false;
            this.Step2.Size = new System.Drawing.Size(470, 116);
            this.Step2.TabIndex = 1;
            this.Step2.Text = "First Catch";
            this.Step2.Initialize += new System.EventHandler<AeroWizard.WizardPageInitEventArgs>(this.wizardPage2_Initialize);
            // 
            // rdoCharmander
            // 
            this.rdoCharmander.AutoSize = true;
            this.rdoCharmander.Location = new System.Drawing.Point(163, 32);
            this.rdoCharmander.Name = "rdoCharmander";
            this.rdoCharmander.Size = new System.Drawing.Size(91, 19);
            this.rdoCharmander.TabIndex = 2;
            this.rdoCharmander.Text = "Charmander";
            this.rdoCharmander.UseVisualStyleBackColor = true;
            this.rdoCharmander.UseWaitCursor = true;
            // 
            // rdoSquirle
            // 
            this.rdoSquirle.AutoSize = true;
            this.rdoSquirle.Location = new System.Drawing.Point(300, 32);
            this.rdoSquirle.Name = "rdoSquirle";
            this.rdoSquirle.Size = new System.Drawing.Size(61, 19);
            this.rdoSquirle.TabIndex = 1;
            this.rdoSquirle.Text = "Squirle";
            this.rdoSquirle.UseVisualStyleBackColor = true;
            this.rdoSquirle.UseWaitCursor = true;
            // 
            // rdoBulbasaur
            // 
            this.rdoBulbasaur.AutoSize = true;
            this.rdoBulbasaur.Checked = true;
            this.rdoBulbasaur.Location = new System.Drawing.Point(24, 32);
            this.rdoBulbasaur.Name = "rdoBulbasaur";
            this.rdoBulbasaur.Size = new System.Drawing.Size(77, 19);
            this.rdoBulbasaur.TabIndex = 0;
            this.rdoBulbasaur.TabStop = true;
            this.rdoBulbasaur.Text = "Bulbasaur";
            this.rdoBulbasaur.UseVisualStyleBackColor = true;
            this.rdoBulbasaur.UseWaitCursor = true;
            // 
            // Step2_2
            // 
            this.Step2_2.AllowBack = false;
            this.Step2_2.AllowNext = false;
            this.Step2_2.Controls.Add(this.label3);
            this.Step2_2.Name = "Step2_2";
            this.Step2_2.Size = new System.Drawing.Size(470, 116);
            this.Step2_2.TabIndex = 2;
            this.Step2_2.Text = "Catching first pkm";
            this.Step2_2.Initialize += new System.EventHandler<AeroWizard.WizardPageInitEventArgs>(this.wizardPage3_Initialize);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(167, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Please wait........";
            // 
            // Step3
            // 
            this.Step3.Controls.Add(this.lblNameError);
            this.Step3.Controls.Add(this.label1);
            this.Step3.Controls.Add(this.txtNick);
            this.Step3.Name = "Step3";
            this.Step3.Size = new System.Drawing.Size(470, 116);
            this.Step3.TabIndex = 4;
            this.Step3.Text = "Select Nickname";
            this.Step3.Initialize += new System.EventHandler<AeroWizard.WizardPageInitEventArgs>(this.wizardPage5_Initialize);
            // 
            // lblNameError
            // 
            this.lblNameError.AutoSize = true;
            this.lblNameError.ForeColor = System.Drawing.Color.Red;
            this.lblNameError.Location = new System.Drawing.Point(77, 74);
            this.lblNameError.Name = "lblNameError";
            this.lblNameError.Size = new System.Drawing.Size(32, 15);
            this.lblNameError.TabIndex = 2;
            this.lblNameError.Text = "error";
            this.lblNameError.UseWaitCursor = true;
            this.lblNameError.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nickname";
            this.label1.UseWaitCursor = true;
            // 
            // txtNick
            // 
            this.txtNick.Location = new System.Drawing.Point(77, 31);
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(225, 23);
            this.txtNick.TabIndex = 0;
            this.txtNick.UseWaitCursor = true;
            // 
            // Step3_3
            // 
            this.Step3_3.AllowBack = false;
            this.Step3_3.Controls.Add(this.label4);
            this.Step3_3.Name = "Step3_3";
            this.Step3_3.Size = new System.Drawing.Size(470, 116);
            this.Step3_3.TabIndex = 5;
            this.Step3_3.Text = "Set nickname .....";
            this.Step3_3.Initialize += new System.EventHandler<AeroWizard.WizardPageInitEventArgs>(this.wizardPage6_Initialize);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(191, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Please wait........";
            // 
            // InitialTutorialForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 270);
            this.Controls.Add(this.wizardControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InitialTutorialForm";
            this.Text = "Initial Account Tutorial";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).EndInit();
            this.Step1.ResumeLayout(false);
            this.Step1.PerformLayout();
            this.Step1_1.ResumeLayout(false);
            this.Step1_1.PerformLayout();
            this.Step2.ResumeLayout(false);
            this.Step2.PerformLayout();
            this.Step2_2.ResumeLayout(false);
            this.Step2_2.PerformLayout();
            this.Step3.ResumeLayout(false);
            this.Step3.PerformLayout();
            this.Step3_3.ResumeLayout(false);
            this.Step3_3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AeroWizard.WizardControl wizardControl1;
        private AeroWizard.WizardPage Step1;
        private System.Windows.Forms.RadioButton rdoFemale;
        private System.Windows.Forms.RadioButton rdoMale;
        private AeroWizard.WizardPage Step2;
        private AeroWizard.WizardPage Step2_2;
        private AeroWizard.WizardPage Step1_1;
        private System.Windows.Forms.RadioButton rdoCharmander;
        private System.Windows.Forms.RadioButton rdoSquirle;
        private System.Windows.Forms.RadioButton rdoBulbasaur;
        private AeroWizard.WizardPage Step3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNick;
        private AeroWizard.WizardPage Step3_3;
        private System.Windows.Forms.Label lblNameError;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}