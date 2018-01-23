using System;
using System.Diagnostics;
using System.Windows.Forms;
using PoGo.NecroBot.Logic.Model.Settings;
using PokemonGo.RocketAPI;

namespace RocketBot2.Forms
{
    public partial class AuthAPIForm : Form
    {
        public APIConfig Config
        {
            get
            {
                return new APIConfig()
                {
                    UsePogoDevAPI = radHashServer.Checked,
                    UseCustomAPI = radCustomHash.Checked,
                    AuthAPIKey = txtAPIKey.Text.Trim(),
                    UrlHashServices = txtCustomHash.Text.Trim()
                };
            }
            set
            {
                radHashServer.Checked = value.UsePogoDevAPI;
                radCustomHash.Checked = value.UseCustomAPI;
                txtCustomHash.Enabled = value.UseCustomAPI;
            }
        }

        private bool forceInput;

        public AuthAPIForm(bool forceInput)
        {
            InitializeComponent();

            if (forceInput)
            {
                this.forceInput = forceInput;
                ControlBox = false;
                btnCancel.Visible = false;
            }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                if (forceInput)
                {
                    myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                }

                return myCp;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (radHashServer.Checked && string.IsNullOrEmpty(txtAPIKey.Text))
            {
                MessageBox.Show("Please enter a valid API Key", "Missing API Key", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (radCustomHash.Checked && string.IsNullOrEmpty(txtAPIKey.Text))
            {
                MessageBox.Show("Please enter a valid API Key", "Missing API Key", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (radCustomHash.Checked && string.IsNullOrEmpty(txtCustomHash.Text))
            {
                MessageBox.Show("Please enter a valid Hash URL", "Missing Hash URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!radHashServer.Checked && !radCustomHash.Checked)
            {
                MessageBox.Show("Please select an API method", "Config error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void TxtCustomHash_TextChanged(object sender, EventArgs e)
        {
            if (txtCustomHash.Text.Contains(Constants.ApiEndPoint))
            {
                txtCustomHash.Text.Replace(Constants.ApiEndPoint, "");
            }
        }

        private void RadHashServer_Click(object sender, EventArgs e)
        {
            if (radHashServer.Checked || radCustomHash.Checked)
            {
                txtAPIKey.Enabled = true;
                if (radCustomHash.Checked)
                {
                    txtCustomHash.Enabled = true;
                }
            }
            else if (!radHashServer.Checked || !radCustomHash.Checked)
            {
                txtAPIKey.Enabled = false;
                if (!radCustomHash.Checked)
                {
                    txtCustomHash.Enabled = false;
                }
            }
        }

        private void RadCustomHash_Click(object sender, EventArgs e)
        {
            if (radHashServer.Checked || radCustomHash.Checked)
            {
                txtAPIKey.Enabled = true;
                if (radCustomHash.Checked)
                {
                    txtCustomHash.Enabled = true;
                }
            }
            else if (!radHashServer.Checked || !radCustomHash.Checked)
            {
                txtAPIKey.Enabled = false;
                if (!radCustomHash.Checked)
                {
                    txtCustomHash.Enabled = false;
                }
            }
        }
    }
}