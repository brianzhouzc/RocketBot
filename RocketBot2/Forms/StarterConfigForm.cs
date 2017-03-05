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
            this.Session = _session;
            this.txtLanguage.Text = this.Session.LogicSettings.TranslationLanguageCode;
            this.txtLat.Text = settings.LocationConfig.DefaultLatitude.ToString();
            this.txtLng.Text = settings.LocationConfig.DefaultLongitude.ToString();
            this.settings = settings;
            this.txtWebsocketPort.Text = settings.WebsocketsConfig.WebSocketPort.ToString();
            this.chkAllowWebsocket.Checked = settings.WebsocketsConfig.UseWebsocket;

            this.chkSnipeDex.Checked = this.Session.LogicSettings.SnipePokemonNotInPokedex;
            this.chkEnableSnipe.Checked = this.Session.LogicSettings.DataSharingConfig.AutoSnipe;
            this.txtMinIV.Text = this.Session.LogicSettings.MinIVForAutoSnipe.ToString();
            this.txtMinLevel.Text = this.Session.LogicSettings.MinLevelForAutoSnipe.ToString();
            this.elevationService = elevationService;
            this.configFile = configFile;
        }

        private void wizardPage2_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            this.settings.Auth.AuthConfig.AuthType = comboBox1.Text == "ptc" ? AuthType.Ptc : AuthType.Google;
            this.settings.Auth.AuthConfig.Username = txtUsername.Text;
            this.settings.Auth.AuthConfig.Password = txtPassword.Text;
        }

        private void SelectLanguagePage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            
        }

        private void wizardPage4_Click(object sender, EventArgs e)
        {
        }

        private void WalkinSpeedPage_Click(object sender, EventArgs e)
        {
        
        }

        private void chkAllowVariant_Click(object sender, EventArgs e)
        {
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
        }

        private void WebSocketPage_Click(object sender, EventArgs e)
        {
          
        }

        private void wizardControl1_Finished(object sender, EventArgs e)
        {
            GlobalSettings.SaveFiles(settings, this.configFile);
            new Session(settings,new ClientSettings(settings, elevationService), new LogicSettings(settings), elevationService);
            Logger.Write(Session.Translation.GetTranslation(TranslationString.FirstStartSetupCompleted), LogLevel.Info);
        }

        private void SelectLanguagePage_Click(object sender, EventArgs e)
        {
            this.Session = new Session(settings,
                new ClientSettings(settings, elevationService),
                new LogicSettings(settings),
                elevationService
            );

            this.settings.ConsoleConfig.TranslationLanguageCode = this.txtLanguage.Text;
        }

        private void wizardControl1_Cancelling(object sender, CancelEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            GlobalSettings.SaveFiles(settings, this.configFile);
            new Session(settings,new ClientSettings(settings, elevationService), new LogicSettings(settings), elevationService);
            Logger.Write(Session.Translation.GetTranslation(TranslationString.FirstStartSetupCompleted), LogLevel.Info);

            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://necrobot2.com/config.json/2017/01/07/how-to-config-auto-snipe/");
        }

        private void SnipePage_Initialize(object sender, WizardPageInitEventArgs e)
        {
           
        }

        private void LocationPage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            this.settings.LocationConfig.DefaultLatitude = Convert.ToDouble(txtLat.Text);
            this.settings.LocationConfig.DefaultLongitude = Convert.ToDouble(txtLng.Text);

        }

        private void WalkinSpeedPage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            this.settings.LocationConfig.WalkingSpeedInKilometerPerHour = Convert.ToDouble(txtSpeed.Text);
            this.settings.LocationConfig.WalkingSpeedVariant = Convert.ToDouble(txtSpeed.Text);
            this.settings.LocationConfig.UseWalkingSpeedVariant = chkAllowVariant.Checked;

            this.settings.GoogleWalkConfig.UseGoogleWalk = chkEnableGoogle.Checked;
            this.settings.GoogleWalkConfig.GoogleAPIKey = txtGoogleKey.Text;
            this.settings.GoogleWalkConfig.UseGoogleWalk = chkAllowYourwalk.Checked;

            this.settings.MapzenWalkConfig.UseMapzenWalk = chkMazen.Checked;
            this.settings.MapzenWalkConfig.MapzenTurnByTurnApiKey = txtMapzenKey.Text;
        }

        private void WebSocketPage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            this.settings.WebsocketsConfig.UseWebsocket = chkAllowWebsocket.Checked;
            this.settings.WebsocketsConfig.WebSocketPort = Convert.ToInt32(txtWebsocketPort.Text);
        }

        private void SnipePage_Commit(object sender, WizardPageConfirmEventArgs e)
        {
            this.settings.SnipeConfig.MinLevelForAutoSnipe = int.Parse(txtMinLevel.Text);
            this.settings.SnipeConfig.MinIVForAutoSnipe = int.Parse(txtMinIV.Text);
            this.settings.DataSharingConfig.AutoSnipe = chkEnableSnipe.Checked;
            this.settings.SnipeConfig.SnipePokemonNotInPokedex = chkSnipeDex.Checked;

        }

      
    }
}