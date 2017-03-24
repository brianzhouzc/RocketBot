using GMap.NET;
using GMap.NET.MapProviders;
using PoGo.NecroBot.Logic.Model.Settings;
using POGOProtos.Enums;
using PokemonGo.RocketAPI.Enums;
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
        private readonly GlobalSettings _setting;
        private TabPage _tabAdvSettingTab;

        public SettingsForm(ref GlobalSettings settings)
        {
            InitializeComponent();
            _setting = settings;

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
            }
        }

        #region Advanced Setting Init

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            var languageList = GetLanguageList();
            var  languageIndex = languageList.IndexOf(_setting.ConsoleConfig.TranslationLanguageCode);
            cbLanguage.DataSource = languageList;
            cbLanguage.SelectedIndex = languageIndex == -1 ? 0 : languageIndex;

            _tabAdvSettingTab = tabAdvSetting;
            
            #endregion

            #region Login Type and info

            authTypeCb.Text = _setting.Auth.CurrentAuthConfig.AuthType.ToString();
            UserLoginBox.Text = _setting.Auth.CurrentAuthConfig.Username; 
            UserPasswordBox.Text = _setting.Auth.CurrentAuthConfig.Password;

            //google api
            if (_setting.GoogleWalkConfig.GoogleAPIKey != null)
            GoogleApiBox.Text = _setting.GoogleWalkConfig.GoogleAPIKey;

            //proxy
            useProxyCb.Checked = _setting.Auth.ProxyConfig.UseProxy;
            useProxyAuthCb.Checked = _setting.Auth.ProxyConfig.UseProxy && _setting.Auth.ProxyConfig.UseProxyAuthentication;
            ToggleProxyCtrls();

            #endregion

            #region Map Info

            //use google provider
            gMapCtrl.MapProvider = GoogleMapProvider.Instance;
            //get tiles from server only
            gMapCtrl.Manager.Mode = AccessMode.ServerOnly;
            //not use proxy
            GMapProvider.WebProxy = null;
            //center map on moscow
            gMapCtrl.Position = new PointLatLng(_setting.LocationConfig.DefaultLatitude, _setting.LocationConfig.DefaultLongitude);
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

            tbWalkingSpeed.Text = _setting.LocationConfig.WalkingSpeedInKilometerPerHour.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Device Info

            //by default, select one from Necro's device dictionary
            DeviceIdTb.Text = _setting.Auth.DeviceConfig.DeviceId;
            AndroidBoardNameTb.Text = _setting.Auth.DeviceConfig.AndroidBoardName;
            AndroidBootloaderTb.Text = _setting.Auth.DeviceConfig.AndroidBootloader;
            DeviceBrandTb.Text = _setting.Auth.DeviceConfig.DeviceBrand;
            DeviceModelTb.Text = _setting.Auth.DeviceConfig.DeviceModel;
            DeviceModelIdentifierTb.Text = _setting.Auth.DeviceConfig.DeviceModelIdentifier;
            DeviceModelBootTb.Text = _setting.Auth.DeviceConfig.DeviceModelBoot;
            HardwareManufacturerTb.Text = _setting.Auth.DeviceConfig.HardwareManufacturer;
            HardwareModelTb.Text = _setting.Auth.DeviceConfig.HardwareModel;
            FirmwareBrandTb.Text = _setting.Auth.DeviceConfig.FirmwareBrand;
            FirmwareTagsTb.Text = _setting.Auth.DeviceConfig.FirmwareTags;
            FirmwareTypeTb.Text = _setting.Auth.DeviceConfig.FirmwareType;
            FirmwareFingerprintTb.Text = _setting.Auth.DeviceConfig.FirmwareFingerprint;
            deviceTypeCb.SelectedIndex = _setting.Auth.DeviceConfig.DeviceBrand.ToLower() == "apple" ? 0 : 1;

            #endregion

            #region Pokemon Info

            #region Catch

            cbCatchPoke.Checked = _setting.PokemonConfig.CatchPokemon;
            cbUseEggIncubators.Checked = _setting.PokemonConfig.UseEggIncubators;


            tbMaxPokeballsPerPokemon.Text = _setting.PokemonConfig.MaxPokeballsPerPokemon.ToString();
            cbAutoFavoritePokemon.Checked = _setting.PokemonConfig.AutoFavoritePokemon;
            tbFavoriteMinIvPercentage.Text = _setting.PokemonConfig.FavoriteMinIvPercentage.ToString(CultureInfo.InvariantCulture);

            tBMaxBerriesToUsePerPokemon.Text = _setting.ItemUseFilters.FirstOrDefault().Value.MaxItemsUsePerPokemon.ToString();
            tbUseBerriesMinCp.Text = _setting.ItemUseFilters.FirstOrDefault().Value.UseItemMinCP.ToString();
            tbUseBerriesMinIv.Text = _setting.ItemUseFilters.FirstOrDefault().Value.UseItemMinIV.ToString(CultureInfo.InvariantCulture);
            tbUseBerriesBelowCatchProbability.Text =_setting.ItemUseFilters.FirstOrDefault().Value.CatchProbability.ToString(CultureInfo.InvariantCulture);
            cbUseBerriesOperator.SelectedIndex = _setting.ItemUseFilters.FirstOrDefault().Value.Operator == "and" ? 0 : 1;
           
            tbUseGreatBallAboveCp.Text = _setting.PokemonConfig.UseGreatBallAboveCp.ToString();
            tbUseUltraBallAboveCp.Text = _setting.PokemonConfig.UseUltraBallAboveCp.ToString();
            tbUseMasterBallAboveCp.Text = _setting.PokemonConfig.UseMasterBallAboveCp.ToString();
            tbUseGreatBallAboveIv.Text = _setting.PokemonConfig.UseGreatBallAboveIv.ToString(CultureInfo.InvariantCulture);
            tbUseUltraBallAboveIv.Text = _setting.PokemonConfig.UseUltraBallAboveIv.ToString(CultureInfo.InvariantCulture);
            tbUseGreatBallBelowCatchProbability.Text =
                _setting.PokemonConfig.UseGreatBallBelowCatchProbability.ToString(CultureInfo.InvariantCulture);
            tbUseUltraBallBelowCatchProbability.Text =
                _setting.PokemonConfig.UseUltraBallBelowCatchProbability.ToString(CultureInfo.InvariantCulture);
            tbUseMasterBallBelowCatchProbability.Text =
                _setting.PokemonConfig.UseMasterBallBelowCatchProbability.ToString(CultureInfo.InvariantCulture);

            foreach (var poke in _setting.PokemonsToIgnore)
            {
                clbIgnore.SetItemChecked(clbIgnore.FindStringExact(poke.ToString()), true);
            }

            #endregion

            #region Transfer

            cbPrioritizeIvOverCp.Checked = _setting.PokemonConfig.PrioritizeIvOverCp;
            tbKeepMinCp.Text = _setting.PokemonConfig.KeepMinCp.ToString();
            tbKeepMinIV.Text = _setting.PokemonConfig.KeepMinIvPercentage.ToString(CultureInfo.InvariantCulture);
            tbKeepMinLvl.Text = _setting.PokemonConfig.KeepMinLvl.ToString();
            cbKeepMinOperator.SelectedIndex = _setting.PokemonConfig.KeepMinOperator.ToLowerInvariant() == "and" ? 0 : 1;
            cbTransferWeakPokemon.Checked = _setting.PokemonConfig.TransferWeakPokemon;
            cbTransferDuplicatePokemon.Checked = _setting.PokemonConfig.TransferDuplicatePokemon;
            cbTransferDuplicatePokemonOnCapture.Checked = _setting.PokemonConfig.TransferDuplicatePokemonOnCapture;

            tbKeepMinDuplicatePokemon.Text = _setting.PokemonConfig.KeepMinDuplicatePokemon.ToString();
            cbUseKeepMinLvl.Checked = _setting.PokemonConfig.UseKeepMinLvl;
            foreach (var poke in _setting.PokemonsNotToTransfer)
            {
                clbTransfer.SetItemChecked(clbTransfer.FindStringExact(poke.ToString()), true);
            }

            #endregion

            #region Powerup

            //focuse to use filter list
            _setting.PokemonConfig.UseLevelUpList = true;

            cbAutoPowerUp.Checked = _setting.PokemonConfig.AutomaticallyLevelUpPokemon;
            cbPowerUpFav.Checked = _setting.PokemonConfig.OnlyUpgradeFavorites;
            cbPowerUpType.SelectedIndex = _setting.PokemonConfig.LevelUpByCPorIv == "iv" ? 0 : 1;
            cbPowerUpCondiction.SelectedIndex = _setting.PokemonConfig.UpgradePokemonMinimumStatsOperator == "and" ? 0 : 1;
            cbPowerUpMinStarDust.Text = _setting.PokemonConfig.GetMinStarDustForLevelUp.ToString();
            tbPowerUpMinIV.Text = _setting.PokemonConfig.UpgradePokemonIvMinimum.ToString(CultureInfo.InvariantCulture);
            tbPowerUpMinCP.Text = _setting.PokemonConfig.UpgradePokemonCpMinimum.ToString(CultureInfo.InvariantCulture);
            foreach (var poke in _setting.PokemonsToLevelUp)
            {
                clbPowerUp.SetItemChecked(clbPowerUp.FindStringExact(poke.ToString()), true);
            }
            if (_setting.PokemonConfig.LevelUpByCPorIv == "iv")
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

            cbEvoAllAboveIV.Checked = _setting.PokemonConfig.EvolveAllPokemonAboveIv;
            tbEvoAboveIV.Text = _setting.PokemonConfig.EvolveAboveIvValue.ToString(CultureInfo.InvariantCulture);
            cbEvolveAllPokemonWithEnoughCandy.Checked = _setting.PokemonConfig.EvolveAllPokemonWithEnoughCandy;
            cbKeepPokemonsThatCanEvolve.Checked = _setting.PokemonConfig.KeepPokemonsThatCanEvolve;
            tbEvolveKeptPokemonsAtStorageUsagePercentage.Text =
                _setting.PokemonConfig.EvolveKeptPokemonsAtStorageUsagePercentage.ToString(CultureInfo.InvariantCulture);
            cbUseLuckyEggsWhileEvolving.Checked = _setting.PokemonConfig.UseLuckyEggsWhileEvolving;
            tbUseLuckyEggsMinPokemonAmount.Text = _setting.PokemonConfig.UseLuckyEggsMinPokemonAmount.ToString();
            //TODO:
            /*
            foreach (var poke in _setting.PokemonEvolveFilter)
            {
                clbEvolve.SetItemChecked(clbEvolve.FindStringExact(poke.ToString()), true);
            }
            */
            #endregion

            #endregion

            #region Item Info

            cbUseLuckyEggConstantly.Checked = _setting.PokemonConfig.UseLuckyEggConstantly;
            cbUseIncenseConstantly.Checked = _setting.PokemonConfig.UseIncenseConstantly;
            tbTotalAmountOfPokeballsToKeep.Text = _setting.RecycleConfig.TotalAmountOfPokeballsToKeep.ToString();
            tbTotalAmountOfPotionsToKeep.Text = _setting.RecycleConfig.TotalAmountOfPotionsToKeep.ToString();
            tbTotalAmountOfRevivesToKeep.Text = _setting.RecycleConfig.TotalAmountOfRevivesToKeep.ToString();
            tbTotalAmountOfBerriesToKeep.Text = _setting.RecycleConfig.TotalAmountOfBerriesToKeep.ToString();
            cbVerboseRecycling.Checked = _setting.RecycleConfig.VerboseRecycling;
            tbRecycleInventoryAtUsagePercentage.Text =
                _setting.RecycleConfig.RecycleInventoryAtUsagePercentage.ToString(CultureInfo.InvariantCulture);

            #endregion

            #region Advance Settings

            cbDisableHumanWalking.Checked = _setting.LocationConfig.DisableHumanWalking;
            cbUseWalkingSpeedVariant.Checked = _setting.LocationConfig.UseWalkingSpeedVariant;
            tbWalkingSpeedVariantInKilometerPerHour.Text = _setting.LocationConfig.WalkingSpeedVariant.ToString(CultureInfo.InvariantCulture);
            cbShowWalkingSpeed.Checked = _setting.LocationConfig.ShowVariantWalking;
            tbMaxSpawnLocationOffset.Text = _setting.LocationConfig.MaxSpawnLocationOffset.ToString();
            tbMaxTravelDistanceInMeters.Text = _setting.LocationConfig.MaxTravelDistanceInMeters.ToString();
            tbDelayBetweenPlayerActions.Text = _setting.PlayerConfig.DelayBetweenPlayerActions.ToString();
            tbDelayBetweenPokemonCatch.Text = _setting.PokemonConfig.DelayBetweenPokemonCatch.ToString();
            cbRandomizeRecycle.Checked = _setting.RecycleConfig.RandomizeRecycle;
            tbRandomRecycleValue.Text = _setting.RecycleConfig.RandomRecycleValue.ToString();
            cbEnableHumanizedThrows.Checked = _setting.CustomCatchConfig.EnableHumanizedThrows;
            tbNiceThrowChance.Text = _setting.CustomCatchConfig.NiceThrowChance.ToString();
            tbGreatThrowChance.Text = _setting.CustomCatchConfig.GreatThrowChance.ToString();
            tbExcellentThrowChance.Text = _setting.CustomCatchConfig.ExcellentThrowChance.ToString();
            tbCurveThrowChance.Text = _setting.CustomCatchConfig.CurveThrowChance.ToString();
            tbForceGreatThrowOverIv.Text = _setting.CustomCatchConfig.ForceGreatThrowOverIv.ToString(CultureInfo.InvariantCulture);
            tbForceExcellentThrowOverIv.Text = _setting.CustomCatchConfig.ForceExcellentThrowOverIv.ToString(CultureInfo.InvariantCulture);
            tbForceGreatThrowOverCp.Text = _setting.CustomCatchConfig.ForceGreatThrowOverCp.ToString();
            tbForceExcellentThrowOverCp.Text = _setting.CustomCatchConfig.ForceExcellentThrowOverCp.ToString();
            cbAutoSniper.Checked = _setting.DataSharingConfig.AutoSnipe;
            cbEnableGyms.Checked = _setting.GymConfig.Enable;
            tbDataServiceIdentification.Text = _setting.DataSharingConfig.DataServiceIdentification;
        }
            #endregion

        #region Help button for API key

        protected override void OnLoad(EventArgs e)
        {
            var btn = new Button {Size = new Size(25, GoogleApiBox.ClientSize.Height + 2)};
            btn.Location = new Point(GoogleApiBox.ClientSize.Width - btn.Width, -1);
            btn.Cursor = Cursors.Default;
            btn.Image = ResourceHelper.GetImage("question");
            btn.Click += googleapihep_click;
            GoogleApiBox.Controls.Add(btn);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            ConsoleHelper.SendMessage(GoogleApiBox.Handle, 0xd3, (IntPtr) 2, (IntPtr) (btn.Width << 16));
            base.OnLoad(e);
        }

        private void googleapihep_click(object sender, EventArgs e)
        {
            Process.Start("https://developers.google.com/maps/documentation/directions/get-api-key");
        }

        #endregion

        #region private methods

        private static int ConvertStringToInt(string input)
        {
            var output = 0;
            int.TryParse(input, out output);
            return output;
        }

        private static float ConvertStringToFloat(string input)
        {
            float output = 0;
            float.TryParse(input, out output);
            return output;
        }

        private static double ConvertStringToDouble(string input)
        {
            double output = 0;
            double.TryParse(input, out output);
            return output;
        }

        private static List<PokemonId> ConvertClbToList(CheckedListBox input)
        {
            return input.CheckedItems.Cast<PokemonId>().ToList();
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
                proxyHostTb.Text = _setting.Auth.ProxyConfig.UseProxyHost;
                proxyPortTb.Enabled = true;
                proxyPortTb.Text = _setting.Auth.ProxyConfig.UseProxyPort;
                useProxyAuthCb.Enabled = true;
            }
            else
            {
                proxyHostTb.Enabled = false;
                proxyHostTb.Text = _setting.Auth.ProxyConfig.UseProxyHost = "";
                proxyPortTb.Enabled = false;
                proxyPortTb.Text = _setting.Auth.ProxyConfig.UseProxyPort = "";
                useProxyAuthCb.Enabled = false;
            }
            if (useProxyAuthCb.Checked)
            {
                proxyUserTb.Enabled = true;
                proxyUserTb.Text = _setting.Auth.ProxyConfig.UseProxyUsername;
                proxyPwTb.Enabled = true;
                proxyPwTb.Text = _setting.Auth.ProxyConfig.UseProxyPassword;
            }
            else
            {
                proxyUserTb.Enabled = false;
                proxyUserTb.Text = _setting.Auth.ProxyConfig.UseProxyUsername = "";
                proxyPwTb.Enabled = false;
                proxyPwTb.Text = _setting.Auth.ProxyConfig.UseProxyPassword = "";
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

        private void saveBtn_Click(object sender, EventArgs e)
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

                var lastPosFile = Path.Combine(_setting.ProfileConfigPath, "LastPos.ini");
                if (File.Exists(lastPosFile))
                {
                    File.Delete(lastPosFile);
                }
                _setting.Auth.CurrentAuthConfig.AuthType = authTypeCb.Text == @"Google" ? AuthType.Google : AuthType.Ptc;
                _setting.Auth.CurrentAuthConfig.Username = UserLoginBox.Text;
                _setting.Auth.CurrentAuthConfig.Password = UserPasswordBox.Text;
                _setting.GoogleWalkConfig.GoogleAPIKey = GoogleApiBox.Text == "" ? null : GoogleApiBox.Text;
                _setting.Auth.ProxyConfig.UseProxy = useProxyCb.Checked == true ? true : false;
                _setting.Auth.ProxyConfig.UseProxyHost = proxyHostTb.Text == "" ? null : proxyHostTb.Text;
                _setting.Auth.ProxyConfig.UseProxyPort = proxyPortTb.Text == "" ? null : proxyPortTb.Text;
                _setting.Auth.ProxyConfig.UseProxyAuthentication = useProxyAuthCb.Checked == true ? true : false;
                _setting.Auth.ProxyConfig.UseProxyUsername = proxyUserTb.Text == "" ? null : proxyUserTb.Text;
                _setting.Auth.ProxyConfig.UseProxyPassword = proxyPwTb.Text == "" ? null : proxyPwTb.Text;
                _setting.Auth.DeviceConfig.DevicePackageName = "custom";
                _setting.Auth.DeviceConfig.DeviceId = DeviceIdTb.Text == "" ? null : DeviceIdTb.Text;
                _setting.Auth.DeviceConfig.AndroidBoardName = AndroidBoardNameTb.Text == "" ? null : AndroidBoardNameTb.Text;
                _setting.Auth.DeviceConfig.AndroidBootloader = AndroidBootloaderTb.Text == "" ? null : AndroidBootloaderTb.Text;
                _setting.Auth.DeviceConfig.DeviceBrand = DeviceBrandTb.Text == "" ? null : DeviceBrandTb.Text;
                _setting.Auth.DeviceConfig.DeviceModel = DeviceModelTb.Text == "" ? null : DeviceModelTb.Text;
                _setting.Auth.DeviceConfig.DeviceModelIdentifier = DeviceModelIdentifierTb.Text == "" ? null : DeviceModelIdentifierTb.Text;
                _setting.Auth.DeviceConfig.DeviceModelBoot = DeviceModelBootTb.Text == "" ? null : DeviceModelBootTb.Text;
                _setting.Auth.DeviceConfig.HardwareManufacturer = HardwareManufacturerTb.Text == "" ? null : HardwareManufacturerTb.Text;
                _setting.Auth.DeviceConfig.HardwareModel = HardwareModelTb.Text == "" ? null : HardwareModelTb.Text;
                _setting.Auth.DeviceConfig.FirmwareBrand = FirmwareBrandTb.Text == "" ? null : FirmwareBrandTb.Text;
                _setting.Auth.DeviceConfig.FirmwareTags = FirmwareTagsTb.Text == "" ? null : FirmwareTagsTb.Text;
                _setting.Auth.DeviceConfig.FirmwareType = FirmwareTypeTb.Text == "" ? null: FirmwareTypeTb.Text;
                _setting.Auth.DeviceConfig.FirmwareFingerprint = FirmwareFingerprintTb.Text == "" ? null : FirmwareFingerprintTb.Text;
                _setting.ConsoleConfig.TranslationLanguageCode = cbLanguage.Text;
                _setting.Auth.Save(AuthFilePath);
                
                #endregion

                #region RocketBot2.Form Settings


                #region Location

                _setting.LocationConfig.DefaultLatitude = ConvertStringToDouble(tbLatitude.Text);
                _setting.LocationConfig.DefaultLongitude = ConvertStringToDouble(tbLongitude.Text);
                _setting.LocationConfig.WalkingSpeedInKilometerPerHour = ConvertStringToDouble(tbWalkingSpeed.Text);

                #endregion

                #region Pokemon

                #region Catch

                _setting.PokemonConfig.CatchPokemon = cbCatchPoke.Checked;
                _setting.PokemonConfig.UseEggIncubators = cbUseEggIncubators.Checked;
                _setting.PokemonConfig.MaxPokeballsPerPokemon = ConvertStringToInt(tbMaxPokeballsPerPokemon.Text);
                _setting.PokemonsToIgnore = ConvertClbToList(clbIgnore);
                _setting.PokemonConfig.AutoFavoritePokemon = cbAutoFavoritePokemon.Checked;
                _setting.PokemonConfig.FavoriteMinIvPercentage = ConvertStringToFloat(tbFavoriteMinIvPercentage.Text);

                _setting.ItemUseFilters.FirstOrDefault().Value.MaxItemsUsePerPokemon = ConvertStringToInt(tBMaxBerriesToUsePerPokemon.Text);
                _setting.ItemUseFilters.FirstOrDefault().Value.UseItemMinCP = ConvertStringToInt(tbUseBerriesMinCp.Text);
                _setting.ItemUseFilters.FirstOrDefault().Value.UseItemMinIV = ConvertStringToInt(tbUseBerriesMinIv.Text);
                _setting.ItemUseFilters.FirstOrDefault().Value.CatchProbability = ConvertStringToDouble(tbUseBerriesBelowCatchProbability.Text);
                _setting.ItemUseFilters.FirstOrDefault().Value.Operator = cbUseBerriesOperator.SelectedIndex == 0 ? "and" : "or";

                _setting.PokemonConfig.UseGreatBallAboveCp = ConvertStringToInt(tbUseGreatBallAboveCp.Text);
                _setting.PokemonConfig.UseUltraBallAboveCp = ConvertStringToInt(tbUseUltraBallAboveCp.Text);
                _setting.PokemonConfig.UseMasterBallAboveCp = ConvertStringToInt(tbUseMasterBallAboveCp.Text);
                _setting.PokemonConfig.UseGreatBallAboveIv = ConvertStringToDouble(tbUseGreatBallAboveIv.Text);
                _setting.PokemonConfig.UseUltraBallAboveIv = ConvertStringToDouble(tbUseUltraBallAboveIv.Text);
                _setting.PokemonConfig.UseGreatBallBelowCatchProbability =
                    ConvertStringToDouble(tbUseGreatBallBelowCatchProbability.Text);
                _setting.PokemonConfig.UseUltraBallBelowCatchProbability =
                    ConvertStringToDouble(tbUseUltraBallBelowCatchProbability.Text);
                _setting.PokemonConfig.UseMasterBallBelowCatchProbability =
                    ConvertStringToDouble(tbUseMasterBallBelowCatchProbability.Text);

                #endregion

                #region Transfer

                _setting.PokemonConfig.PrioritizeIvOverCp = cbPrioritizeIvOverCp.Checked;
                _setting.PokemonConfig.KeepMinCp = ConvertStringToInt(tbKeepMinCp.Text);
                _setting.PokemonConfig.KeepMinIvPercentage = ConvertStringToFloat(tbKeepMinIV.Text);
                _setting.PokemonConfig.KeepMinLvl = ConvertStringToInt(tbKeepMinLvl.Text);
                _setting.PokemonConfig.KeepMinOperator = cbKeepMinOperator.SelectedIndex == 0 ? "and" : "or";
                _setting.PokemonConfig.TransferWeakPokemon = cbTransferWeakPokemon.Checked;
                _setting.PokemonConfig.TransferDuplicatePokemon = cbTransferDuplicatePokemon.Checked;
                _setting.PokemonConfig.TransferDuplicatePokemonOnCapture = cbTransferDuplicatePokemonOnCapture.Checked;

                _setting.PokemonConfig.KeepMinDuplicatePokemon = ConvertStringToInt(tbKeepMinDuplicatePokemon.Text);
                _setting.PokemonConfig.UseKeepMinLvl = cbUseKeepMinLvl.Checked;
                _setting.PokemonsNotToTransfer = ConvertClbToList(clbTransfer);

                #endregion

                #region PowerUp

                _setting.PokemonConfig.UseLevelUpList = true;

                _setting.PokemonConfig.AutomaticallyLevelUpPokemon = cbAutoPowerUp.Checked;
                _setting.PokemonConfig.OnlyUpgradeFavorites = cbPowerUpFav.Checked;
                _setting.PokemonConfig.LevelUpByCPorIv = cbPowerUpType.SelectedIndex == 0 ? "iv" : "cp";
                _setting.PokemonConfig.UpgradePokemonMinimumStatsOperator = cbPowerUpCondiction.SelectedIndex == 0 ? "and" : "or";
                _setting.PokemonConfig.GetMinStarDustForLevelUp = ConvertStringToInt(cbPowerUpMinStarDust.Text);
                _setting.PokemonConfig.UpgradePokemonIvMinimum = ConvertStringToFloat(tbPowerUpMinIV.Text);
                _setting.PokemonConfig.UpgradePokemonCpMinimum = ConvertStringToFloat(tbPowerUpMinCP.Text);
                _setting.PokemonsToLevelUp = ConvertClbToList(clbPowerUp);

                #endregion

                #region Evo

                _setting.PokemonConfig.EvolveAllPokemonAboveIv = cbEvoAllAboveIV.Checked;
                _setting.PokemonConfig.EvolveAboveIvValue = ConvertStringToFloat(tbEvoAboveIV.Text);
                _setting.PokemonConfig.EvolveAllPokemonWithEnoughCandy = cbEvolveAllPokemonWithEnoughCandy.Checked;
                _setting.PokemonConfig.KeepPokemonsThatCanEvolve = cbKeepPokemonsThatCanEvolve.Checked;
                _setting.PokemonConfig.UseLuckyEggsWhileEvolving = cbUseLuckyEggsWhileEvolving.Checked;
                _setting.PokemonConfig.EvolveKeptPokemonsAtStorageUsagePercentage =
                    ConvertStringToDouble(tbEvolveKeptPokemonsAtStorageUsagePercentage.Text);
                _setting.PokemonConfig.UseLuckyEggsMinPokemonAmount = ConvertStringToInt(tbUseLuckyEggsMinPokemonAmount.Text);
                //TODO:
                //_setting.PokemonEvolveFilter = ConvertClbToList(clbEvolve);

                #endregion

                #endregion

                #region Item

                _setting.PokemonConfig.UseLuckyEggConstantly = cbUseLuckyEggConstantly.Checked;
                _setting.PokemonConfig.UseIncenseConstantly = cbUseIncenseConstantly.Checked;
                _setting.RecycleConfig.TotalAmountOfPokeballsToKeep = ConvertStringToInt(tbTotalAmountOfPokeballsToKeep.Text);
                _setting.RecycleConfig.TotalAmountOfPotionsToKeep = ConvertStringToInt(tbTotalAmountOfPotionsToKeep.Text);
                _setting.RecycleConfig.TotalAmountOfRevivesToKeep = ConvertStringToInt(tbTotalAmountOfRevivesToKeep.Text);
                _setting.RecycleConfig.TotalAmountOfBerriesToKeep = ConvertStringToInt(tbTotalAmountOfBerriesToKeep.Text);
                _setting.RecycleConfig.VerboseRecycling = cbVerboseRecycling.Checked;
                _setting.RecycleConfig.RecycleInventoryAtUsagePercentage =
                    ConvertStringToDouble(tbRecycleInventoryAtUsagePercentage.Text);

                #endregion

                #region Advanced Settings

                _setting.LocationConfig.DisableHumanWalking = cbDisableHumanWalking.Checked;
                _setting.LocationConfig.UseWalkingSpeedVariant = cbUseWalkingSpeedVariant.Checked;
                _setting.LocationConfig.WalkingSpeedVariant = ConvertStringToDouble(tbWalkingSpeedVariantInKilometerPerHour.Text);
                _setting.LocationConfig.ShowVariantWalking = cbShowWalkingSpeed.Checked;
                _setting.LocationConfig.MaxSpawnLocationOffset = ConvertStringToInt(tbMaxSpawnLocationOffset.Text);
                _setting.LocationConfig.MaxTravelDistanceInMeters = ConvertStringToInt(tbMaxTravelDistanceInMeters.Text);
                _setting.PlayerConfig.DelayBetweenPlayerActions = ConvertStringToInt(tbDelayBetweenPlayerActions.Text);
                _setting.PokemonConfig.DelayBetweenPokemonCatch = ConvertStringToInt(tbDelayBetweenPokemonCatch.Text);
                _setting.RecycleConfig.RandomizeRecycle = cbRandomizeRecycle.Checked;
                _setting.RecycleConfig.RandomRecycleValue = ConvertStringToInt(tbRandomRecycleValue.Text);
                _setting.CustomCatchConfig.EnableHumanizedThrows = cbEnableHumanizedThrows.Checked;
                _setting.CustomCatchConfig.NiceThrowChance = ConvertStringToInt(tbNiceThrowChance.Text);
                _setting.CustomCatchConfig.GreatThrowChance = ConvertStringToInt(tbGreatThrowChance.Text);
                _setting.CustomCatchConfig.ExcellentThrowChance = ConvertStringToInt(tbExcellentThrowChance.Text);
                _setting.CustomCatchConfig.CurveThrowChance = ConvertStringToInt(tbCurveThrowChance.Text);
                _setting.CustomCatchConfig.ForceGreatThrowOverIv = ConvertStringToDouble(tbForceGreatThrowOverIv.Text);
                _setting.CustomCatchConfig.ForceExcellentThrowOverIv = ConvertStringToDouble(tbForceExcellentThrowOverIv.Text);
                _setting.CustomCatchConfig.ForceGreatThrowOverCp = ConvertStringToInt(tbForceGreatThrowOverCp.Text);
                _setting.CustomCatchConfig.ForceExcellentThrowOverCp = ConvertStringToInt(tbForceExcellentThrowOverCp.Text);
                _setting.GymConfig.Enable = cbEnableGyms.Checked;
                _setting.DataSharingConfig.AutoSnipe = cbAutoSniper.Checked;
                _setting.DataSharingConfig.DataServiceIdentification = tbDataServiceIdentification.Text;

                #endregion

                _setting.Save(ConfigFilePath, true);
                Close();

                #endregion
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            gMapCtrl.Zoom = trackBar.Value;
        }

        private void gMapCtrl_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateLocationInfo();
        }

        private void gMapCtrl_OnMapZoomChanged()
        {
            UpdateLocationInfo();
        }

        private void ResetLocationBtn_Click(object sender, EventArgs e)
        {
            gMapCtrl.Zoom = trackBar.Value = DefaultZoomLevel;
            UpdateMapLocation(_setting.LocationConfig.DefaultLatitude, _setting.LocationConfig.DefaultLongitude);
        }

        private void gMapCtrl_MouseClick(object sender, MouseEventArgs e)
        {
            var localCoordinates = e.Location;
            gMapCtrl.Position = gMapCtrl.FromLocalToLatLng(localCoordinates.X, localCoordinates.Y);
        }

        private void latitudeText_Leave(object sender, EventArgs e)
        {
            UpdateMapLocation(ConvertStringToDouble(tbLatitude.Text), ConvertStringToDouble(tbLongitude.Text));
        }

        private void longitudeText_Leave(object sender, EventArgs e)
        {
            UpdateMapLocation(ConvertStringToDouble(tbLatitude.Text), ConvertStringToDouble(tbLongitude.Text));
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

        private void useProxyCb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleProxyCtrls();
        }

        private void useProxyAuthCb_CheckedChanged(object sender, EventArgs e)
        {
            ToggleProxyCtrls();
        }

        private void deviceTypeCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PopulateDevice(deviceTypeCb.SelectedIndex);
        }

        private void cbPowerUpAll_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbPowerUp, cbPowerUpAll.Checked);
        }

        private void cbPowerUpType_SelectionChangeCommitted(object sender, EventArgs e)
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

        private void cbSelectAllEvolve_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbEvolve, cbEvolveAll.Checked);
        }

        private void cbSelectAllCatch_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbIgnore, cbIgnoreAll.Checked);
        }

        private void cbSelectAllTransfer_CheckedChanged(object sender, EventArgs e)
        {
            ListSelectAllHandler(clbTransfer, cbNotTransferAll.Checked);
        }
        #endregion
    }
}