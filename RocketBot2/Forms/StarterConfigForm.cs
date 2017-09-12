using System;
using System.ComponentModel;
using System.Windows.Forms;
using AeroWizard;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.Service.Elevation;
using PoGo.NecroBot.Logic.State;
using PokemonGo.RocketAPI.Enums;
using System.Diagnostics;

namespace RocketBot2.Forms
{
    public partial class StarterConfigForm : Form
    {
        private GlobalSettings settings;
        private IElevationService elevationService;
        private string configFile;

        public ISession Session { get; set; }

        public StarterConfigForm()
        {
            InitializeComponent();
        }

        public StarterConfigForm(Session _session)
        {
            InitializeComponent();
        }

        public StarterConfigForm(Session _session, GlobalSettings settings,
            IElevationService elevationService, string configFile) : this(_session)
        {
            Session = _session;
            txtLanguage.Text = Session.LogicSettings.TranslationLanguageCode;
            txtLat.Text = settings.LocationConfig.DefaultLatitude.ToString();
            txtLng.Text = settings.LocationConfig.DefaultLongitude.ToString();
            this.settings = settings;
            txtWebsocketPort.Text = settings.WebsocketsConfig.WebSocketPort.ToString();
            chkAllowWebsocket.Checked = settings.WebsocketsConfig.UseWebsocket;

            chkSnipeDex.Checked = Session.LogicSettings.SnipePokemonNotInPokedex;
            chkEnableSnipe.Checked = Session.LogicSettings.DataSharingConfig.AutoSnipe;
            txtMinIV.Text = Session.LogicSettings.MinIVForAutoSnipe.ToString();
            txtMinLevel.Text = Session.LogicSettings.MinLevelForAutoSnipe.ToString();
            this.elevationService = elevationService;
            this.configFile = configFile;
        }

        private void WizardPage2_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            settings.Auth.CurrentAuthConfig.AuthType = comboBox1.Text == "ptc" ? AuthType.Ptc : AuthType.Google;
            settings.Auth.CurrentAuthConfig.Username = txtUsername.Text;
            settings.Auth.CurrentAuthConfig.Password = txtPassword.Text;
        }

        private void SelectLanguagePage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            
        }

        private void WizardPage4_Click(object sender, EventArgs e)
        {
        }

        private void WalkinSpeedPage_Click(object sender, EventArgs e)
        {
        
        }

        private void ChkAllowVariant_Click(object sender, EventArgs e)
        {
        }

        private void CheckBox1_Click(object sender, EventArgs e)
        {
        }

        private void WebSocketPage_Click(object sender, EventArgs e)
        {
          
        }

        private void WizardControl1_Finished(object sender, EventArgs e)
        {
            GlobalSettings.SaveFiles(settings, configFile);
            new Session(settings,new ClientSettings(settings, elevationService), new LogicSettings(settings), elevationService);
            Logger.Write(Session.Translation.GetTranslation(TranslationString.FirstStartSetupCompleted), LogLevel.Info);
        }

        private void SelectLanguagePage_Click(object sender, EventArgs e)
        {
            Session = new Session(settings,
                new ClientSettings(settings, elevationService),
                new LogicSettings(settings),
                elevationService
            );

            settings.ConsoleConfig.TranslationLanguageCode = txtLanguage.Text;
        }

        private void WizardControl1_Cancelling(object sender, CancelEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            GlobalSettings.SaveFiles(settings, configFile);
            new Session(settings,new ClientSettings(settings, elevationService), new LogicSettings(settings), elevationService);
            Logger.Write(Session.Translation.GetTranslation(TranslationString.FirstStartSetupCompleted), LogLevel.Info);

            Close();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://necrobot2.com/config.json/2017/01/07/how-to-config-auto-snipe/");
        }

        private void SnipePage_Initialize(object sender, WizardPageInitEventArgs e)
        {
           
        }

        private void LocationPage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            settings.LocationConfig.DefaultLatitude = Convert.ToDouble(txtLat.Text);
            settings.LocationConfig.DefaultLongitude = Convert.ToDouble(txtLng.Text);

        }

        private void WalkinSpeedPage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            settings.LocationConfig.WalkingSpeedInKilometerPerHour = Convert.ToDouble(txtSpeed.Text);
            settings.LocationConfig.WalkingSpeedVariant = Convert.ToDouble(txtSpeed.Text);
            settings.LocationConfig.UseWalkingSpeedVariant = chkAllowVariant.Checked;

            settings.GoogleWalkConfig.UseGoogleWalk = chkEnableGoogle.Checked;
            settings.GoogleWalkConfig.GoogleAPIKey = txtGoogleKey.Text;
            settings.GoogleWalkConfig.UseGoogleWalk = chkAllowYourwalk.Checked;

            settings.MapzenWalkConfig.UseMapzenWalk = chkMazen.Checked;
            settings.MapzenWalkConfig.MapzenTurnByTurnApiKey = txtMapzenKey.Text;
        }

        private void WebSocketPage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            settings.WebsocketsConfig.UseWebsocket = chkAllowWebsocket.Checked;
            settings.WebsocketsConfig.WebSocketPort = Convert.ToInt32(txtWebsocketPort.Text);
        }

        private void SnipePage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            settings.SnipeConfig.MinLevelForAutoSnipe = int.Parse(txtMinLevel.Text);
            settings.SnipeConfig.MinIVForAutoSnipe = int.Parse(txtMinIV.Text);
            settings.DataSharingConfig.AutoSnipe = chkEnableSnipe.Checked;
            settings.SnipeConfig.SnipePokemonNotInPokedex = chkSnipeDex.Checked;

        }

      
    }
}