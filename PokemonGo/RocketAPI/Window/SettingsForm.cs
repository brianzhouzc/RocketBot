using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window
{
    partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            authTypeCb.Text = Settings.Instance.AuthType.ToString();
            ptcUserText.Text = Settings.Instance.PtcUsername;
            ptcPassText.Text = Settings.Instance.PtcPassword;
            latitudeText.Text = Settings.Instance.DefaultLatitude.ToString();
            longitudeText.Text = Settings.Instance.DefaultLongitude.ToString();
            razzmodeCb.Text = Settings.Instance.RazzBerryMode;
            razzSettingText.Text = Settings.Instance.RazzBerrySetting.ToString();
            transferTypeCb.Text = Settings.Instance.TransferType;
            transferCpThresText.Text = Settings.Instance.TransferCPThreshold.ToString();
            evolveAllChk.Checked = Settings.Instance.EvolveAllGivenPokemons;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Settings.Instance.SetSetting(authTypeCb.Text , "AuthType");
            Settings.Instance.SetSetting(ptcUserText.Text , "PtcUsername");
            Settings.Instance.SetSetting(ptcPassText.Text , "PtcPassword");
            Settings.Instance.SetSetting(latitudeText.Text , "DefaultLatitude");
            Settings.Instance.SetSetting(longitudeText.Text , "DefaultLongitude");
            Settings.Instance.SetSetting(razzmodeCb.Text , "RazzBerryMode");
            Settings.Instance.SetSetting(razzSettingText.Text , "RazzBerrySetting");
            Settings.Instance.SetSetting(transferTypeCb.Text , "TransferType");
            Settings.Instance.SetSetting(transferCpThresText.Text , "TransferCPThreshold");
            Settings.Instance.SetSetting(evolveAllChk.Checked ? "true" : "false", "EvolveAllGivenPokemons");
            Settings.Instance.Reload();
            Close();
        }

        private void authTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (authTypeCb.Text == "Google")
            {
                ptcUserText.Visible = false;
                ptcPassText.Visible = false;
                ptcUserLabel.Visible = false;
                ptcPasswordLabel.Visible = false;
            }
            else
            {
                ptcUserText.Visible = true;
                ptcPassText.Visible = true;
                ptcUserLabel.Visible = true;
                ptcPasswordLabel.Visible = true;

            }
        }
    }
}
