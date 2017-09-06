using GMap.NET;
using GMap.NET.MapProviders;
using PoGo.NecroBot.Logic.Interfaces.Configuration;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Enums;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using RocketBot2.Forms.advSettings;
using RocketBot2.Helpers;
using RocketBot2.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RocketBot2.Forms
{
    internal partial class SettingsForm : System.Windows.Forms.Form
    {
        private const int DefaultZoomLevel = 15;

        private static readonly string ConfigFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        private static readonly string AuthFilePath = Path.Combine(ConfigFolderPath, "auth.json");
        private static readonly string ConfigFilePath = Path.Combine(ConfigFolderPath, "config.json");
        private static readonly string LanguagePath = Path.Combine(ConfigFolderPath, "Translations");
        private readonly DeviceHelper _deviceHelper;
        private readonly List<DeviceInfo> _deviceInfos;
        private readonly GlobalSettings _settings;
        private readonly ISession _session;
        private List<AuthConfig> accounts;

        public List<AuthConfig> Accounts
        {
            get { return accounts; }
        }

        public SettingsForm(ref GlobalSettings settings, ISession session)
        {
            InitializeComponent();
            _settings = settings;
            _session = session;

            _deviceHelper = new DeviceHelper();
            _deviceInfos = _deviceHelper.DeviceBucket;

            foreach (
                var pokemon in
                    Enum.GetValues(typeof(PokemonId)).Cast<PokemonId>().Where(id => id != PokemonId.Missingno))
            {
                clbIgnore.Items.Add(pokemon);
                clbTransfer.Items.Add(pokemon);
                clbPowerUp.Items.Add(pokemon);
                clbEvolve.Items.Add(pokemon);
                clbSnipePokemonFilter.Items.Add(pokemon);
            }

            var logicSettings = new LogicSettings(settings);

            accounts = new List<AuthConfig>();
            accounts.AddRange(logicSettings.Bots);

            if (accounts.Count > 1)
            {
                lvAccounts.Visible = true;
                foreach (var acc in accounts)
                {
                    lvAccounts.Items.Add($"{acc.AuthType}").SubItems.Add($"{acc.Username}");
                }
            }
            else
            {
                lvAccounts.Visible = false;
            }

            StreamReader auth = new StreamReader(AuthFilePath);
            Auth.LoadJsonToTreeView(auth.ReadToEnd(), "Auth");
            //JsonTreeView.ExpandAll();
            auth.Close();

            StreamReader config = new StreamReader(ConfigFilePath);
            Config.LoadJsonToTreeView(config.ReadToEnd(), "Config");
            //JsonTreeView.ExpandAll();
            config.Close();
        }

        #region Advanced Setting Init

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            var languageList = GetLanguageList();
            var  languageIndex = languageList.IndexOf(_settings.ConsoleConfig.TranslationLanguageCode);
            cbLanguage.DataSource = languageList;
            cbLanguage.SelectedIndex = languageIndex == -1 ? 0 : languageIndex;

            #endregion

            #region Login Type and info
            authTypeCb.Text = _settings.Auth.CurrentAuthConfig.AuthType.ToString();
            UserLoginBox.Text = _settings.Auth.CurrentAuthConfig.Username; 
            UserPasswordBox.Text = _settings.Auth.CurrentAuthConfig.Password;

            //google api
            if (_settings.GoogleWalkConfig.GoogleAPIKey != null)
            GoogleApiBox.Text = _settings.GoogleWalkConfig.GoogleAPIKey;

            //proxy
            useProxyCb.Checked = _settings.Auth.ProxyConfig.UseProxy;
            useProxyAuthCb.Checked = _settings.Auth.ProxyConfig.UseProxy && _settings.Auth.ProxyConfig.UseProxyAuthentication;
            ToggleProxyCtrls();

            //apiconfig
            cbUsePogoDevAPI.Checked = _settings.Auth.APIConfig.UsePogoDevAPI;
            tbAuthAPIKey.Text = _settings.Auth.APIConfig.AuthAPIKey;
            cbUseLegacyAPI.Checked = _settings.Auth.APIConfig.UseLegacyAPI;
            cbDiplayHashServerLog.Checked = _settings.Auth.APIConfig.DiplayHashServerLog;

            cbEnablePushBulletNotification.Checked = _settings.NotificationConfig.EnablePushBulletNotification;
            tbPushBulletAPIKey.Text = _settings.NotificationConfig.PushBulletApiKey;

            #endregion

            #region Map Info

            //use google provider
            gMapCtrl.MapProvider = GoogleMapProvider.Instance;
            //get tiles from server only
            gMapCtrl.Manager.Mode = AccessMode.ServerOnly;
            //not use proxy
            GMapProvider.WebProxy = null;
            //center map on moscow
            gMapCtrl.Position = new PointLatLng(_session.Client.CurrentLatitude, _session.Client.CurrentLongitude);
            //zoom min/max; default both = 2
            gMapCtrl.DragButton = MouseButtons.Left;
            gMapCtrl.CenterPen = new Pen(Color.Red, 2);
            gMapCtrl.MinZoom = trackBar.Maximum = 2;
            gMapCtrl.MaxZoom = trackBar.Maximum = 18;
            trackBar.Value = DefaultZoomLevel;
            //set zoom
            gMapCtrl.Zoom = trackBar.Value;
            //disable map focus
            gMapCtrl.DisableFocusOnMouseEnter = true;

            tbWalkingSpeed.Text = _settings.LocationConfig.WalkingSpeedInKilometerPerHour.ToString(CultureInfo.InvariantCulture);
            cbStartFromLastPosition.Checked = _settings.LocationConfig.StartFromLastPosition;

            #endregion

            #region Device Info

            //by default, select one from Necro's device dictionary
            DeviceIdTb.Text = _settings.Auth.DeviceConfig.DeviceId;
            AndroidBoardNameTb.Text = _settings.Auth.DeviceConfig.AndroidBoardName;
            AndroidBootloaderTb.Text = _settings.Auth.DeviceConfig.AndroidBootloader;
            DeviceBrandTb.Text = _settings.Auth.DeviceConfig.DeviceBrand;
            DeviceModelTb.Text = _settings.Auth.DeviceConfig.DeviceModel;
            DeviceModelIdentifierTb.Text = _settings.Auth.DeviceConfig.DeviceModelIdentifier;
            DeviceModelBootTb.Text = _settings.Auth.DeviceConfig.DeviceModelBoot;
            HardwareManufacturerTb.Text = _settings.Auth.DeviceConfig.HardwareManufacturer;
            HardwareModelTb.Text = _settings.Auth.DeviceConfig.HardwareModel;
            FirmwareBrandTb.Text = _settings.Auth.DeviceConfig.FirmwareBrand;
            FirmwareTagsTb.Text = _settings.Auth.DeviceConfig.FirmwareTags;
            FirmwareTypeTb.Text = _settings.Auth.DeviceConfig.FirmwareType;
            FirmwareFingerprintTb.Text = _settings.Auth.DeviceConfig.FirmwareFingerprint;
            deviceTypeCb.SelectedIndex = _settings.Auth.DeviceConfig.DeviceBrand.ToLower() == "apple" ? 0 : 1;

            #endregion

            #region Pokemon Info

            #region Catch

            cbCatchPoke.Checked = _settings.PokemonConfig.CatchPokemon;
            cbUseEggIncubators.Checked = _settings.PokemonConfig.UseEggIncubators;
            cbUseLimitedEggIncubators.Checked = _settings.PokemonConfig.UseLimitedEggIncubators;
            cbAutoFavoriteShinyOnCatch.Checked = _settings.PokemonConfig.AutoFavoriteShinyOnCatch;

            tbMaxPokeballsPerPokemon.Text = _settings.PokemonConfig.MaxPokeballsPerPokemon.ToString();
            cbAutoFavoritePokemon.Checked = _settings.PokemonConfig.AutoFavoritePokemon;
            tbFavoriteMinIvPercentage.Text = _settings.PokemonConfig.FavoriteMinIvPercentage.ToString(CultureInfo.InvariantCulture);

            tBMaxBerriesToUsePerPokemon.Text = _settings.ItemUseFilters.FirstOrDefault().Value.MaxItemsUsePerPokemon.ToString();
            tbUseBerriesMinCp.Text = _settings.ItemUseFilters.FirstOrDefault().Value.UseItemMinCP.ToString();
            tbUseBerriesMinIv.Text = _settings.ItemUseFilters.FirstOrDefault().Value.UseItemMinIV.ToString(CultureInfo.InvariantCulture);
            tbUseBerriesBelowCatchProbability.Text =_settings.ItemUseFilters.FirstOrDefault().Value.CatchProbability.ToString(CultureInfo.InvariantCulture);
            cbUseBerriesOperator.SelectedIndex = _settings.ItemUseFilters.FirstOrDefault().Value.Operator == "and" ? 0 : 1;
           
            tbUseGreatBallAboveCp.Text = _settings.PokemonConfig.UseGreatBallAboveCp.ToString();
            tbUseUltraBallAboveCp.Text = _settings.PokemonConfig.UseUltraBallAboveCp.ToString();
            tbUseMasterBallAboveCp.Text = _settings.PokemonConfig.UseMasterBallAboveCp.ToString();
            tbUseGreatBallAboveIv.Text = _settings.PokemonConfig.UseGreatBallAboveIv.ToString(CultureInfo.InvariantCulture);
            tbUseUltraBallAboveIv.Text = _settings.PokemonConfig.UseUltraBallAboveIv.ToString(CultureInfo.InvariantCulture);
            tbUseGreatBallBelowCatchProbability.Text =
                _settings.PokemonConfig.UseGreatBallBelowCatchProbability.ToString(CultureInfo.InvariantCulture);
            tbUseUltraBallBelowCatchProbability.Text =
                _settings.PokemonConfig.UseUltraBallBelowCatchProbability.ToString(CultureInfo.InvariantCulture);
            tbUseMasterBallBelowCatchProbability.Text =
                _settings.PokemonConfig.UseMasterBallBelowCatchProbability.ToString(CultureInfo.InvariantCulture);

            foreach (var poke in _settings.PokemonsToIgnore)
            {
                clbIgnore.SetItemChecked(clbIgnore.FindStringExact(poke.ToString()), true);
            }

            foreach (var poke in _settings.SnipePokemonFilter)
            {
                clbSnipePokemonFilter.SetItemChecked(clbSnipePokemonFilter.FindStringExact(poke.Key.ToString()), true);
            }

            #endregion

            #region Transfer

            cbPrioritizeIvOverCp.Checked = _settings.PokemonConfig.PrioritizeIvOverCp;
            tbKeepMinCp.Text = _settings.PokemonConfig.KeepMinCp.ToString();
            tbKeepMinIV.Text = _settings.PokemonConfig.KeepMinIvPercentage.ToString(CultureInfo.InvariantCulture);
            tbKeepMinLvl.Text = _settings.PokemonConfig.KeepMinLvl.ToString();
            cbKeepMinOperator.SelectedIndex = _settings.PokemonConfig.KeepMinOperator.ToLowerInvariant() == "and" ? 0 : 1;
            cbTransferWeakPokemon.Checked = _settings.PokemonConfig.TransferWeakPokemon;
            cbTransferDuplicatePokemon.Checked = _settings.PokemonConfig.TransferDuplicatePokemon;
            cbTransferDuplicatePokemonOnCapture.Checked = _settings.PokemonConfig.TransferDuplicatePokemonOnCapture;

            tbKeepMinDuplicatePokemon.Text = _settings.PokemonConfig.KeepMinDuplicatePokemon.ToString();
            cbUseKeepMinLvl.Checked = _settings.PokemonConfig.UseKeepMinLvl;
            foreach (var poke in _settings.PokemonsNotToTransfer)
            {
                clbTransfer.SetItemChecked(clbTransfer.FindStringExact(poke.ToString()), true);
            }

            #endregion

            #region Powerup

            //focuse to use filter list
            _settings.PokemonConfig.UseLevelUpList = true;

            cbAutoPowerUp.Checked = _settings.PokemonConfig.AutomaticallyLevelUpPokemon;
            cbPowerUpFav.Checked = _settings.PokemonConfig.OnlyUpgradeFavorites;
            cbPowerUpType.SelectedIndex = _settings.PokemonConfig.LevelUpByCPorIv == "iv" ? 0 : 1;
            cbPowerUpCondiction.SelectedIndex = _settings.PokemonConfig.UpgradePokemonMinimumStatsOperator == "and" ? 0 : 1;
            cbPowerUpMinStarDust.Text = _settings.PokemonConfig.GetMinStarDustForLevelUp.ToString();
            tbPowerUpMinIV.Text = _settings.PokemonConfig.UpgradePokemonIvMinimum.ToString(CultureInfo.InvariantCulture);
            tbPowerUpMinCP.Text = _settings.PokemonConfig.UpgradePokemonCpMinimum.ToString(CultureInfo.InvariantCulture);
            foreach (var poke in _settings.PokemonsToLevelUp)
            {
                clbPowerUp.SetItemChecked(clbPowerUp.FindStringExact(poke.ToString()), true);
            }
            if (_settings.PokemonConfig.LevelUpByCPorIv == "iv")
            {
                label31.Visible = true;
                tbPowerUpMinIV.Visible = true;
                label30.Visible = false;
                tbPowerUpMinCP.Visible = false;
            }
            else
            {
                label31.Visible = false;
                tbPowerUpMinIV.Visible = false;
                label30.Visible = true;
                tbPowerUpMinCP.Visible = true;
            }

            #endregion

            #region Evo

            cbEvoAllAboveIV.Checked = _settings.PokemonConfig.EvolveAllPokemonAboveIv;
            tbEvoAboveIV.Text = _settings.PokemonConfig.EvolveAboveIvValue.ToString(CultureInfo.InvariantCulture);
            cbEvolveAllPokemonWithEnoughCandy.Checked = _settings.PokemonConfig.EvolveAllPokemonWithEnoughCandy;
            cbKeepPokemonsThatCanEvolve.Checked = _settings.PokemonConfig.KeepPokemonsThatCanEvolve;
            tbEvolveKeptPokemonsAtStorageUsagePercentage.Text =
                _settings.PokemonConfig.EvolveKeptPokemonsAtStorageUsagePercentage.ToString(CultureInfo.InvariantCulture);
            cbUseLuckyEggsWhileEvolving.Checked = _settings.PokemonConfig.UseLuckyEggsWhileEvolving;
            tbUseLuckyEggsMinPokemonAmount.Text = _settings.PokemonConfig.UseLuckyEggsMinPokemonAmount.ToString();
            
            foreach (var poke in _settings.PokemonEvolveFilter)
            {
                clbEvolve.SetItemChecked(clbEvolve.FindStringExact(poke.Key.ToString()), true);
            }

            #endregion

            #endregion

            #region Item Info

            cbUseLuckyEggConstantly.Checked = _settings.PokemonConfig.UseLuckyEggConstantly;
            cbUseIncenseConstantly.Checked = _settings.PokemonConfig.UseIncenseConstantly;
            tbTotalAmountOfPokeballsToKeep.Text = _settings.RecycleConfig.TotalAmountOfPokeballsToKeep.ToString();
            tbTotalAmountOfPotionsToKeep.Text = _settings.RecycleConfig.TotalAmountOfPotionsToKeep.ToString();
            tbTotalAmountOfRevivesToKeep.Text = _settings.RecycleConfig.TotalAmountOfRevivesToKeep.ToString();
            tbTotalAmountOfBerriesToKeep.Text = _settings.RecycleConfig.TotalAmountOfBerriesToKeep.ToString();
            cbVerboseRecycling.Checked = _settings.RecycleConfig.VerboseRecycling;
            tbRecycleInventoryAtUsagePercentage.Text =
                _settings.RecycleConfig.RecycleInventoryAtUsagePercentage.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Advance Settings

            cbDisableHumanWalking.Checked = _settings.LocationConfig.DisableHumanWalking;
            cbUseWalkingSpeedVariant.Checked = _settings.LocationConfig.UseWalkingSpeedVariant;
            tbWalkingSpeedVariantInKilometerPerHour.Text = _settings.LocationConfig.WalkingSpeedVariant.ToString(CultureInfo.InvariantCulture);
            cbShowWalkingSpeed.Checked = _settings.LocationConfig.ShowVariantWalking;
            tbMaxSpawnLocationOffset.Text = _settings.LocationConfig.MaxSpawnLocationOffset.ToString();
            tbMaxTravelDistanceInMeters.Text = _settings.LocationConfig.MaxTravelDistanceInMeters.ToString();
            tbDelayBetweenPlayerActions.Text = _settings.PlayerConfig.DelayBetweenPlayerActions.ToString();
            tbDelayBetweenPokemonCatch.Text = _settings.PokemonConfig.DelayBetweenPokemonCatch.ToString();
            cbRandomizeRecycle.Checked = _settings.RecycleConfig.RandomizeRecycle;
            tbRandomRecycleValue.Text = _settings.RecycleConfig.RandomRecycleValue.ToString();
            cbEnableHumanizedThrows.Checked = _settings.CustomCatchConfig.EnableHumanizedThrows;
            tbNiceThrowChance.Text = _settings.CustomCatchConfig.NiceThrowChance.ToString();
            tbGreatThrowChance.Text = _settings.CustomCatchConfig.GreatThrowChance.ToString();
            tbExcellentThrowChance.Text = _settings.CustomCatchConfig.ExcellentThrowChance.ToString();
            tbCurveThrowChance.Text = _settings.CustomCatchConfig.CurveThrowChance.ToString();
            tbForceGreatThrowOverIv.Text = _settings.CustomCatchConfig.ForceGreatThrowOverIv.ToString(CultureInfo.InvariantCulture);
            tbForceExcellentThrowOverIv.Text = _settings.CustomCatchConfig.ForceExcellentThrowOverIv.ToString(CultureInfo.InvariantCulture);
            tbForceGreatThrowOverCp.Text = _settings.CustomCatchConfig.ForceGreatThrowOverCp.ToString();
            tbForceExcellentThrowOverCp.Text = _settings.CustomCatchConfig.ForceExcellentThrowOverCp.ToString();
            cbAutoSniper.Checked = _settings.DataSharingConfig.AutoSnipe;
            tbDataServiceIdentification.Text = _settings.DataSharingConfig.DataServiceIdentification;
            cbEnableSyncData.Checked = _settings.DataSharingConfig.EnableSyncData;
            cbEnableGyms.Checked = _settings.GymConfig.Enable;
            cBoxTeamColor.Text = _settings.GymConfig.DefaultTeam;
            cbUseHumanlikeDelays.Checked = _settings.HumanlikeDelays.UseHumanlikeDelays;
        }
        #endregion

        #region Help button for API key

        private void cbUseEggIncubators_CheckedChanged(object sender, EventArgs e)
        {
            cbUseLimitedEggIncubators.Enabled = cbUseEggIncubators.Checked;
        }

        private void cbCatchPoke_CheckedChanged(object sender, EventArgs e)
        {
            gbCatchPokemon.Enabled = cbCatchPoke.Checked;
        }

        protected override void OnLoad(EventArgs e)
        {
            var btn = new Button {Size = new Size(25, GoogleApiBox.ClientSize.Height + 2)};
            btn.Location = new Point(GoogleApiBox.ClientSize.Width - btn.Width, -1);
            btn.Cursor = Cursors.Default;
            btn.Image = ResourceHelper.GetImage("question");
            btn.Click += Googleapihep_click;
            GoogleApiBox.Controls.Add(btn);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            ConsoleHelper.SendMessage(GoogleApiBox.Handle, 0xd3, (IntPtr) 2, (IntPtr) (btn.Width << 16));
            base.OnLoad(e);
        }

        private void Googleapihep_click(object sender, EventArgs e)
        {
            Process.Start("https://developers.google.com/maps/documentation/directions/get-api-key");
        }

        #endregion

        #region private methods
        private static float ConvertStringToFloat(string input)
        {
#pragma warning disable IDE0018 // Inline variable declaration - Build.Bat Error Happens if We Do
            float output;
            float.TryParse(input, out output);
            return output;
#pragma warning restore IDE0018 // Inline variable declaration - Build.Bat Error Happens if We Do
        }
        private static List<PokemonId> ConvertClbToList(CheckedListBox input)
        {
            return input.CheckedItems.Cast<PokemonId>().ToList();
        }

        private static Dictionary<PokemonId, EvolveFilter> EvolveFilterConvertClbDictionary(CheckedListBox input)
        {
            var x = input.CheckedItems.Cast<PokemonId>().ToList();
            var results = new Dictionary<PokemonId, EvolveFilter>();
            foreach (var i in x)
            {
                var y = new EvolveFilter();
                results.Add(i, y);
            }
            return results;
        }

        private static Dictionary<PokemonId, SnipeFilter> SnipeFilterConvertClbDictionary(CheckedListBox input)
        {
            var x = input.CheckedItems.Cast<PokemonId>().ToList();
            var results = new Dictionary<PokemonId, SnipeFilter>();
            foreach (var i in x)
            {
                var y = new SnipeFilter();
                results.Add(i, y);
            }
            return results;
        }

        /// <summary>
        ///     Get languale list from Translations folder and populate it to combo box
        /// </summary>
        private List<string> GetLanguageList()
        {
            var languages = new List<string> {"en"};
            var langFiles = Directory.GetFiles(LanguagePath, "*.json", SearchOption.TopDirectoryOnly);
            languages.AddRange(langFiles.Select(
                langFileName => Path.GetFileNameWithoutExtension(langFileName)?.Replace("translation.", ""))
                .Where(langCode => langCode != "en"));
            return languages;
        }

        /// <summary>
        ///     Update location lat and lon to textboxes
        /// </summary>
        private void UpdateLocationInfo()
        {
            //not rounding it, need to have correct position to prevent map drifting
            tbLatitude.Text = gMapCtrl.Position.Lat.ToString(CultureInfo.InvariantCulture);
            tbLongitude.Text = gMapCtrl.Position.Lng.ToString(CultureInfo.InvariantCulture);
            //update trackbar
            trackBar.Value = (int) Math.Round(gMapCtrl.Zoom);
        }

        /// <summary>
        ///     Update map location base on giving lng and lat
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        private void UpdateMapLocation(double lat, double lng)
        {
            gMapCtrl.Position = new PointLatLng(lat, lng);
        }

        private void ToggleProxyCtrls()
        {
            if (useProxyCb.Checked)
            {
                proxyHostTb.Enabled = true;
                proxyHostTb.Text = _settings.Auth.ProxyConfig.UseProxyHost;
                proxyPortTb.Enabled = true;
                proxyPortTb.Text = _settings.Auth.ProxyConfig.UseProxyPort;
                useProxyAuthCb.Enabled = true;
            }
            else
            {
                proxyHostTb.Enabled = false;
                proxyHostTb.Text = _settings.Auth.ProxyConfig.UseProxyHost = "";
                proxyPortTb.Enabled = false;
                proxyPortTb.Text = _settings.Auth.ProxyConfig.UseProxyPort = "";
                useProxyAuthCb.Enabled = false;
            }
            if (useProxyAuthCb.Checked)
            {
                proxyUserTb.Enabled = true;
                proxyUserTb.Text = _settings.Auth.ProxyConfig.UseProxyUsername;
                proxyPwTb.Enabled = true;
                proxyPwTb.Text = _settings.Auth.ProxyConfig.UseProxyPassword;
            }
            else
            {
                proxyUserTb.Enabled = false;
                proxyUserTb.Text = _settings.Auth.ProxyConfig.UseProxyUsername = "";
                proxyPwTb.Enabled = false;
                proxyPwTb.Text = _settings.Auth.ProxyConfig.UseProxyPassword = "";
            }
        }

        private void PopulateDevice(int tabIndex = -1)
        {
            deviceTypeCb.SelectedIndex = tabIndex == -1 ? _deviceHelper.GetRandomIndex(2) : tabIndex;
            var candidateDevices = deviceTypeCb.SelectedIndex == 0
                ? _deviceInfos.Where(d => d.DeviceBrand.ToLower() == "apple").ToList()
                : _deviceInfos.Where(d => d.DeviceBrand.ToLower() != "apple").ToList();
            var selectIndex = _deviceHelper.GetRandomIndex(candidateDevices.Count);

            DeviceIdTb.Text = candidateDevices[selectIndex].DeviceId == "8525f5d8201f78b5"
                ? _deviceHelper.RandomString(16, "0123456789abcdef")
                : candidateDevices[selectIndex].DeviceId;
            AndroidBoardNameTb.Text = candidateDevices[selectIndex].AndroidBoardName;
            AndroidBootloaderTb.Text = candidateDevices[selectIndex].AndroidBootloader;
            DeviceBrandTb.Text = candidateDevices[selectIndex].DeviceBrand;
            DeviceModelTb.Text = candidateDevices[selectIndex].DeviceModel;
            DeviceModelIdentifierTb.Text = candidateDevices[selectIndex].DeviceModelIdentifier;
            DeviceModelBootTb.Text = candidateDevices[selectIndex].DeviceModelBoot;
            HardwareManufacturerTb.Text = candidateDevices[selectIndex].HardwareManufacturer;
            HardwareModelTb.Text = candidateDevices[selectIndex].HardwareModel;
            FirmwareBrandTb.Text = candidateDevices[selectIndex].FirmwareBrand;
            FirmwareTagsTb.Text = candidateDevices[selectIndex].FirmwareTags;
            FirmwareTypeTb.Text = candidateDevices[selectIndex].FirmwareType;
            FirmwareFingerprintTb.Text = candidateDevices[selectIndex].FirmwareFingerprint;
        }

        private static void ListSelectAllHandler(CheckedListBox targetList, bool setToValue)
        {
            for (var index = 0; index < targetList.Items.Count; index++)
            {
                targetList.SetItemChecked(index, setToValue);
            }
        }

        #endregion

        #region Events

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (UserLoginBox.Text.Length == 0 || UserPasswordBox.Text.Length == 0)
            {
                MessageBox.Show(
                    @"You haven't complete entering your basic information yet." + Environment.NewLine +
                    @"Either Username, Password is empty. Please complete them before saving.",
                    @"Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                #region Auth Settings

                var lastPosFile = Path.Combine(_settings.ProfileConfigPath, "LastPos.ini");
                if (File.Exists(lastPosFile))
                {
                    File.Delete(lastPosFile);
                }
                _settings.Auth.CurrentAuthConfig.AuthType = authTypeCb.Text == @"Google" ? AuthType.Google : AuthType.Ptc;
                _settings.Auth.CurrentAuthConfig.Username = UserLoginBox.Text;
                _settings.Auth.CurrentAuthConfig.Password = UserPasswordBox.Text;
                _settings.GoogleWalkConfig.GoogleAPIKey = GoogleApiBox.Text == "" ? null : GoogleApiBox.Text;
                _settings.Auth.ProxyConfig.UseProxy = useProxyCb.Checked == true ? true : false;
                _settings.Auth.ProxyConfig.UseProxyHost = proxyHostTb.Text == "" ? null : proxyHostTb.Text;
                _settings.Auth.ProxyConfig.UseProxyPort = proxyPortTb.Text == "" ? null : proxyPortTb.Text;
                _settings.Auth.ProxyConfig.UseProxyAuthentication = useProxyAuthCb.Checked == true ? true : false;
                _settings.Auth.ProxyConfig.UseProxyUsername = proxyUserTb.Text == "" ? null : proxyUserTb.Text;
                _settings.Auth.ProxyConfig.UseProxyPassword = proxyPwTb.Text == "" ? null : proxyPwTb.Text;
                _settings.Auth.DeviceConfig.DevicePackageName = "custom";
                _settings.Auth.DeviceConfig.DeviceId = DeviceIdTb.Text == "" ? null : DeviceIdTb.Text;
                _settings.Auth.DeviceConfig.AndroidBoardName = AndroidBoardNameTb.Text == "" ? null : AndroidBoardNameTb.Text;
                _settings.Auth.DeviceConfig.AndroidBootloader = AndroidBootloaderTb.Text == "" ? null : AndroidBootloaderTb.Text;
                _settings.Auth.DeviceConfig.DeviceBrand = DeviceBrandTb.Text == "" ? null : DeviceBrandTb.Text;
                _settings.Auth.DeviceConfig.DeviceModel = DeviceModelTb.Text == "" ? null : DeviceModelTb.Text;
                _settings.Auth.DeviceConfig.DeviceModelIdentifier = DeviceModelIdentifierTb.Text == "" ? null : DeviceModelIdentifierTb.Text;
                _settings.Auth.DeviceConfig.DeviceModelBoot = DeviceModelBootTb.Text == "" ? null : DeviceModelBootTb.Text;
                _settings.Auth.DeviceConfig.HardwareManufacturer = HardwareManufacturerTb.Text == "" ? null : HardwareManufacturerTb.Text;
                _settings.Auth.DeviceConfig.HardwareModel = HardwareModelTb.Text == "" ? null : HardwareModelTb.Text;
                _settings.Auth.DeviceConfig.FirmwareBrand = FirmwareBrandTb.Text == "" ? null : FirmwareBrandTb.Text;
                _settings.Auth.DeviceConfig.FirmwareTags = FirmwareTagsTb.Text == "" ? null : FirmwareTagsTb.Text;
                _settings.Auth.DeviceConfig.FirmwareType = FirmwareTypeTb.Text == "" ? null : FirmwareTypeTb.Text;
                _settings.Auth.DeviceConfig.FirmwareFingerprint = FirmwareFingerprintTb.Text == "" ? null : FirmwareFingerprintTb.Text;
                _settings.ConsoleConfig.TranslationLanguageCode = cbLanguage.Text;
                _settings.Auth.APIConfig.UsePogoDevAPI = cbUsePogoDevAPI.Checked;
                _settings.Auth.APIConfig.AuthAPIKey = tbAuthAPIKey.Text;
                _settings.Auth.APIConfig.UseLegacyAPI = cbUseLegacyAPI.Checked;
                _settings.Auth.APIConfig.DiplayHashServerLog = cbDiplayHashServerLog.Checked;
                _settings.Auth.Save(AuthFilePath);
                #endregion

                #region RocketBot2.Form Settings


                #region Location

                _settings.LocationConfig.DefaultLatitude = Convert.ToDouble(tbLatitude.Text);
                _settings.LocationConfig.DefaultLongitude = Convert.ToDouble(tbLongitude.Text);
                _settings.LocationConfig.WalkingSpeedInKilometerPerHour = Convert.ToDouble(tbWalkingSpeed.Text);
                _settings.LocationConfig.StartFromLastPosition = cbStartFromLastPosition.Checked;

                #endregion

                #region Pokemon

                #region Catch

                _settings.PokemonConfig.CatchPokemon = cbCatchPoke.Checked;
                _settings.PokemonConfig.UseEggIncubators = cbUseEggIncubators.Checked;
                _settings.PokemonConfig.MaxPokeballsPerPokemon = Convert.ToInt32(tbMaxPokeballsPerPokemon.Text);
                _settings.PokemonsToIgnore = ConvertClbToList(clbIgnore);
                _settings.PokemonConfig.AutoFavoritePokemon = cbAutoFavoritePokemon.Checked;
                _settings.PokemonConfig.FavoriteMinIvPercentage = ConvertStringToFloat(tbFavoriteMinIvPercentage.Text);

                _settings.ItemUseFilters.FirstOrDefault().Value.MaxItemsUsePerPokemon = Convert.ToInt32(tBMaxBerriesToUsePerPokemon.Text);
                _settings.ItemUseFilters.FirstOrDefault().Value.UseItemMinCP = Convert.ToInt32(tbUseBerriesMinCp.Text);
                _settings.ItemUseFilters.FirstOrDefault().Value.UseItemMinIV = Convert.ToInt32(tbUseBerriesMinIv.Text);
                _settings.ItemUseFilters.FirstOrDefault().Value.CatchProbability = Convert.ToDouble(tbUseBerriesBelowCatchProbability.Text);
                _settings.ItemUseFilters.FirstOrDefault().Value.Operator = cbUseBerriesOperator.SelectedIndex == 0 ? "and" : "or";

                _settings.PokemonConfig.UseGreatBallAboveCp = Convert.ToInt32(tbUseGreatBallAboveCp.Text);
                _settings.PokemonConfig.UseUltraBallAboveCp = Convert.ToInt32(tbUseUltraBallAboveCp.Text);
                _settings.PokemonConfig.UseMasterBallAboveCp = Convert.ToInt32(tbUseMasterBallAboveCp.Text);
                _settings.PokemonConfig.UseGreatBallAboveIv = Convert.ToDouble(tbUseGreatBallAboveIv.Text);
                _settings.PokemonConfig.UseUltraBallAboveIv = Convert.ToDouble(tbUseUltraBallAboveIv.Text);
                _settings.PokemonConfig.UseGreatBallBelowCatchProbability =
                    Convert.ToDouble(tbUseGreatBallBelowCatchProbability.Text);
                _settings.PokemonConfig.UseUltraBallBelowCatchProbability =
                    Convert.ToDouble(tbUseUltraBallBelowCatchProbability.Text);
                _settings.PokemonConfig.UseMasterBallBelowCatchProbability =
                    Convert.ToDouble(tbUseMasterBallBelowCatchProbability.Text);
                _settings.PokemonConfig.UseLimitedEggIncubators = cbUseLimitedEggIncubators.Checked;
                _settings.PokemonConfig.AutoFavoriteShinyOnCatch = cbAutoFavoriteShinyOnCatch.Checked;
                _settings.SnipePokemonFilter = SnipeFilterConvertClbDictionary(clbSnipePokemonFilter);

                #endregion

                #region Transfer

                _settings.PokemonConfig.PrioritizeIvOverCp = cbPrioritizeIvOverCp.Checked;
                _settings.PokemonConfig.KeepMinCp = Convert.ToInt32(tbKeepMinCp.Text);
                _settings.PokemonConfig.KeepMinIvPercentage = ConvertStringToFloat(tbKeepMinIV.Text);
                _settings.PokemonConfig.KeepMinLvl = Convert.ToInt32(tbKeepMinLvl.Text);
                _settings.PokemonConfig.KeepMinOperator = cbKeepMinOperator.SelectedIndex == 0 ? "and" : "or";
                _settings.PokemonConfig.TransferWeakPokemon = cbTransferWeakPokemon.Checked;
                _settings.PokemonConfig.TransferDuplicatePokemon = cbTransferDuplicatePokemon.Checked;
                _settings.PokemonConfig.TransferDuplicatePokemonOnCapture = cbTransferDuplicatePokemonOnCapture.Checked;

                _settings.PokemonConfig.KeepMinDuplicatePokemon = Convert.ToInt32(tbKeepMinDuplicatePokemon.Text);
                _settings.PokemonConfig.UseKeepMinLvl = cbUseKeepMinLvl.Checked;
                _settings.PokemonsNotToTransfer = ConvertClbToList(clbTransfer);

                #endregion

                #region PowerUp

                _settings.PokemonConfig.UseLevelUpList = true;

                _settings.PokemonConfig.AutomaticallyLevelUpPokemon = cbAutoPowerUp.Checked;
                _settings.PokemonConfig.OnlyUpgradeFavorites = cbPowerUpFav.Checked;
                _settings.PokemonConfig.LevelUpByCPorIv = cbPowerUpType.SelectedIndex == 0 ? "iv" : "cp";
                _settings.PokemonConfig.UpgradePokemonMinimumStatsOperator = cbPowerUpCondiction.SelectedIndex == 0 ? "and" : "or";
                _settings.PokemonConfig.GetMinStarDustForLevelUp = Convert.ToInt32(cbPowerUpMinStarDust.Text);
                _settings.PokemonConfig.UpgradePokemonIvMinimum = ConvertStringToFloat(tbPowerUpMinIV.Text);
                _settings.PokemonConfig.UpgradePokemonCpMinimum = ConvertStringToFloat(tbPowerUpMinCP.Text);
                _settings.PokemonsToLevelUp = ConvertClbToList(clbPowerUp);

                #endregion

                #region Evo

                _settings.PokemonConfig.EvolveAllPokemonAboveIv = cbEvoAllAboveIV.Checked;
                _settings.PokemonConfig.EvolveAboveIvValue = ConvertStringToFloat(tbEvoAboveIV.Text);
                _settings.PokemonConfig.EvolveAllPokemonWithEnoughCandy = cbEvolveAllPokemonWithEnoughCandy.Checked;
                _settings.PokemonConfig.KeepPokemonsThatCanEvolve = cbKeepPokemonsThatCanEvolve.Checked;
                _settings.PokemonConfig.UseLuckyEggsWhileEvolving = cbUseLuckyEggsWhileEvolving.Checked;
                _settings.PokemonConfig.EvolveKeptPokemonsAtStorageUsagePercentage =
                    Convert.ToDouble(tbEvolveKeptPokemonsAtStorageUsagePercentage.Text);
                _settings.PokemonConfig.UseLuckyEggsMinPokemonAmount = Convert.ToInt32(tbUseLuckyEggsMinPokemonAmount.Text);
                _settings.PokemonEvolveFilter = EvolveFilterConvertClbDictionary(clbEvolve);

                #endregion

                #endregion

                #region Item

                _settings.PokemonConfig.UseLuckyEggConstantly = cbUseLuckyEggConstantly.Checked;
                _settings.PokemonConfig.UseIncenseConstantly = cbUseIncenseConstantly.Checked;
                _settings.RecycleConfig.TotalAmountOfPokeballsToKeep = Convert.ToInt32(tbTotalAmountOfPokeballsToKeep.Text);
                _settings.RecycleConfig.TotalAmountOfPotionsToKeep = Convert.ToInt32(tbTotalAmountOfPotionsToKeep.Text);
                _settings.RecycleConfig.TotalAmountOfRevivesToKeep = Convert.ToInt32(tbTotalAmountOfRevivesToKeep.Text);
                _settings.RecycleConfig.TotalAmountOfBerriesToKeep = Convert.ToInt32(tbTotalAmountOfBerriesToKeep.Text);
                _settings.RecycleConfig.VerboseRecycling = cbVerboseRecycling.Checked;
                _settings.RecycleConfig.RecycleInventoryAtUsagePercentage =
                    Convert.ToDouble(tbRecycleInventoryAtUsagePercentage.Text);

                #endregion

                #region Advanced Settings

                _settings.LocationConfig.DisableHumanWalking = cbDisableHumanWalking.Checked;
                _settings.LocationConfig.UseWalkingSpeedVariant = cbUseWalkingSpeedVariant.Checked;
                _settings.LocationConfig.WalkingSpeedVariant = Convert.ToDouble(tbWalkingSpeedVariantInKilometerPerHour.Text);
                _settings.LocationConfig.ShowVariantWalking = cbShowWalkingSpeed.Checked;
                _settings.LocationConfig.MaxSpawnLocationOffset = Convert.ToInt32(tbMaxSpawnLocationOffset.Text);
                _settings.LocationConfig.MaxTravelDistanceInMeters = Convert.ToInt32(tbMaxTravelDistanceInMeters.Text);
                _settings.PlayerConfig.DelayBetweenPlayerActions = Convert.ToInt32(tbDelayBetweenPlayerActions.Text);
                _settings.PokemonConfig.DelayBetweenPokemonCatch = Convert.ToInt32(tbDelayBetweenPokemonCatch.Text);
                _settings.RecycleConfig.RandomizeRecycle = cbRandomizeRecycle.Checked;
                _settings.RecycleConfig.RandomRecycleValue = Convert.ToInt32(tbRandomRecycleValue.Text);
                _settings.CustomCatchConfig.EnableHumanizedThrows = cbEnableHumanizedThrows.Checked;
                _settings.CustomCatchConfig.NiceThrowChance = Convert.ToInt32(tbNiceThrowChance.Text);
                _settings.CustomCatchConfig.GreatThrowChance = Convert.ToInt32(tbGreatThrowChance.Text);
                _settings.CustomCatchConfig.ExcellentThrowChance = Convert.ToInt32(tbExcellentThrowChance.Text);
                _settings.CustomCatchConfig.CurveThrowChance = Convert.ToInt32(tbCurveThrowChance.Text);
                _settings.CustomCatchConfig.ForceGreatThrowOverIv = Convert.ToInt32(tbForceGreatThrowOverIv.Text);
                _settings.CustomCatchConfig.ForceExcellentThrowOverIv = Convert.ToInt32(tbForceExcellentThrowOverIv.Text);
                _settings.CustomCatchConfig.ForceGreatThrowOverCp = Convert.ToInt32(tbForceGreatThrowOverCp.Text);
                _settings.CustomCatchConfig.ForceExcellentThrowOverCp = Convert.ToInt32(tbForceExcellentThrowOverCp.Text);
                _settings.GymConfig.Enable = cbEnableGyms.Checked;
                _settings.GymConfig.DefaultTeam = cBoxTeamColor.Text;
                _settings.DataSharingConfig.AutoSnipe = cbAutoSniper.Checked;
                _settings.DataSharingConfig.DataServiceIdentification = tbDataServiceIdentification.Text;
                _settings.DataSharingConfig.EnableSyncData = cbEnableSyncData.Checked;
                _settings.HumanlikeDelays.UseHumanlikeDelays = cbUseHumanlikeDelays.Checked;

                //Settings added by TheWizard
                _settings.NotificationConfig.EnablePushBulletNotification = cbEnablePushBulletNotification.Checked;
                _settings.NotificationConfig.PushBulletApiKey = tbPushBulletAPIKey.Text;

                #endregion

                _settings.Save(ConfigFilePath);
                Close();

                #endregion
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            gMapCtrl.Zoom = trackBar.Value;
        }

        private void GMapCtrl_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateLocationInfo();
        }

        private void GMapCtrl_OnMapZoomChanged()
        {
            UpdateLocationInfo();
        }

        private void ResetLocationBtn_Click(object sender, EventArgs e)
        {
            gMapCtrl.Zoom = trackBar.Value = DefaultZoomLevel;
            UpdateMapLocation(_settings.LocationConfig.DefaultLatitude, _settings.LocationConfig.DefaultLongitude);
        }

        private void GMapCtrl_MouseClick(object sender, MouseEventArgs e)
        {
            var localCoordinates = e.Location;
            gMapCtrl.Position = gMapCtrl.FromLocalToLatLng(localCoordinates.X, localCoordinates.Y);
        }

        private void LatitudeText_Leave(object sender, EventArgs e)
        {
            UpdateMapLocation(Convert.ToDouble(tbLatitude.Text), Convert.ToDouble(tbLongitude.Text));
        }

        private void LongitudeText_Leave(object sender, EventArgs e)
        {
            UpdateMapLocation(Convert.ToDouble(tbLatitude.Text), Convert.ToDouble(tbLongitude.Text));
        }

        private void AdressBox_Enter(object sender, EventArgs e)
        {
            if (AdressBox.Text != @"Enter an address or a coordinate")
            {
                return;
            }
            AdressBox.Text = "";
        }

        private void AdressBox_Leave(object sender, EventArgs e)
        {
            if (AdressBox.Text != string.Empty)
            {
                return;
            }
            AdressBox.Text = @"Enter an address or a coordinate";
        }

        private void AdressBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) Keys.Enter)
            {
                return;
            }
            gMapCtrl.SetPositionByKeywords(AdressBox.Text);
            gMapCtrl.Zoom = DefaultZoomLevel;
            UpdateLocationInfo();
        }

        private void FindAdressBtn_Click(object sender, EventArgs e)
        {
            gMapCtrl.SetPositionByKeywords(AdressBox.Text);
            gMapCtrl.Zoom = DefaultZoomLevel;
            UpdateLocationInfo();
        }

        private void RandomIDBtn_Click(object sender, EventArgs e)
        {
            DeviceIdTb.Text = _deviceHelper.RandomString(16, "0123456789abcdef");
        }

        private void RandomDeviceBtn_Click(object sender, EventArgs e)
        {
            PopulateDevice();
        }

        private void UseProxyCb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleProxyCtrls();
        }

        private void UseProxyAuthCb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleProxyCtrls();
        }

        private void DeviceTypeCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PopulateDevice(deviceTypeCb.SelectedIndex);
        }

        private void CbPowerUpAll_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbPowerUp, cbPowerUpAll.Checked);
        }

        private void CbPowerUpType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbPowerUpType.Text.ToLowerInvariant() == "iv")
            {
                label31.Visible = true;
                tbPowerUpMinIV.Visible = true;
                label30.Visible = false;
                tbPowerUpMinCP.Visible = false;
            }
            else
            {
                label31.Visible = false;
                tbPowerUpMinIV.Visible = false;
                label30.Visible = true;
                tbPowerUpMinCP.Visible = true;
            }
        }

        private void CbSelectAllEvolve_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbEvolve, cbEvolveAll.Checked);
        }

        private void CbSelectAllSnipePokemonFilter_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbSnipePokemonFilter, cbSnipePokemonFilterAll.Checked);
        }

        private void CbSelectAllCatch_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbIgnore, cbIgnoreAll.Checked);
        }

        private void CbSelectAllTransfer_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbTransfer, cbNotTransferAll.Checked);
        }

        private void CbUsePogoDevAPI_CheckedChanged(object sender, EventArgs e)
        {
            cbUseLegacyAPI.Checked = !cbUsePogoDevAPI.Checked;
        }

        private void CbUseLegacyAPI_CheckedChanged(object sender, EventArgs e)
        {
            cbUsePogoDevAPI.Checked = !cbUseLegacyAPI.Checked;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            gMapCtrl.Dispose();
        }

        private void CbEnablePushBulletNotification_CheckedChanged(object sender, EventArgs e)
        {
            _settings.NotificationConfig.EnablePushBulletNotification = cbEnablePushBulletNotification.Checked;
        }
        #endregion
    }
}
