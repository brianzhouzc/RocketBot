using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketBot.Logic;
using PokemonGo.RocketBot.Logic.Common;
using PokemonGo.RocketBot.Logic.Event;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.PoGoUtils;
using PokemonGo.RocketBot.Logic.Service;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Utils;
using PokemonGo.RocketBot.Window.Plugin;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using static System.Reflection.Assembly;

namespace PokemonGo.RocketBot.Window
{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);
        private static readonly string subPath = "";
        private static Session session;

        public MainForm()
        {
            InitializeComponent();
            InitializePokemonForm();
            Instance = this;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Logger.SetLogger(new ConsoleLogger(LogLevel.LevelUp), subPath);
            CheckVersion();
            if (BoolNeedsSetup())
            {
                startStopBotToolStripMenuItem.Enabled = false;
                Logger.Write("First time here? Go to settings to set your basic info.");
            }
        }

        private async Task StartBot()
        {
            var strCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            var culture = CultureInfo.CreateSpecificCulture("en");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;

            var logger = new ConsoleLogger(LogLevel.LevelUp);
            Logger.SetLogger(logger, subPath);

            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), subPath);
            var profileConfigPath = Path.Combine(profilePath, "config");
            var configFile = Path.Combine(profileConfigPath, "config.json");

            GlobalSettings settings;
            var boolNeedsSetup = false;

            if (File.Exists(configFile))
            {
                if (!VersionCheckState.IsLatest())
                    settings = GlobalSettings.Load(subPath, true);
                else
                    settings = GlobalSettings.Load(subPath);
            }
            else
            {
                settings = new GlobalSettings();
                settings.ProfilePath = profilePath;
                settings.ProfileConfigPath = profileConfigPath;
                settings.GeneralConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config");
                settings.TranslationLanguageCode = strCulture;

                boolNeedsSetup = true;
            }


            session = new Session(new ClientSettings(settings), new LogicSettings(settings));

            if (boolNeedsSetup)
            {
                menuStrip1.ShowItemToolTips = true;
                startStopBotToolStripMenuItem.ToolTipText = "Please goto settings and enter your basic info";
                return;
                /** if (GlobalSettings.PromptForSetup(session.Translation) && !settings.isGui)
                {
                    session = GlobalSettings.SetupSettings(session, settings, configFile);

                    if (!settings.isGui)
                    {
                        var fileName = Assembly.GetExecutingAssembly().Location;
                        System.Diagnostics.Process.Start(fileName);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    GlobalSettings.Load(subPath);

                    Logger.Write("Press a Key to continue...",
                        LogLevel.Warning);
                    Console.ReadKey();
                    return;
                } **/
            }
            session.Client.ApiFailure = new ApiFailureStrategy(session);

            var machine = new StateMachine();
            var stats = new Statistics();

            var strVersion = GetExecutingAssembly().GetName().Version.ToString(3);

            stats.DirtyEvent +=
                () =>
                    Text = $"[RocketBot v{strVersion}] " +
                           stats.GetTemplatedStats(
                               session.Translation.GetTranslation(TranslationString.StatsTemplateString),
                               session.Translation.GetTranslation(TranslationString.StatsXpTemplateString));

            var aggregator = new StatisticsAggregator(stats);
            var listener = new ConsoleEventListener();

            session.EventDispatcher.EventReceived += evt => listener.Listen(evt, session);
            session.EventDispatcher.EventReceived += evt => aggregator.Listen(evt, session);
            if (settings.UseWebsocket)
            {
                var websocket = new WebSocketInterface(settings.WebSocketPort, session);
                session.EventDispatcher.EventReceived += evt => websocket.Listen(evt, session);
            }

            var plugins = new PluginManager(new PluginInitializerInfo
            {
                Logger = logger,
                Session = session,
                Settings = settings,
                Statistics = stats
            });
            plugins.InitPlugins();
            machine.SetFailureState(new LoginState());
            Logger.SetLoggerContext(session);
            session.Navigation.UpdatePositionEvent +=
                (lat, lng) => session.EventDispatcher.Send(new UpdatePositionEvent {Latitude = lat, Longitude = lng});
            session.Navigation.UpdatePositionEvent += Navigation_UpdatePositionEvent;
            machine.AsyncStart(new VersionCheckState(), session);

            if (settings.UseTelegramAPI)
            {
                session.Telegram = new TelegramService(settings.TelegramAPIKey, session);
            }

            settings.checkProxy();

            QuitEvent.WaitOne();
        }

        private static void Navigation_UpdatePositionEvent(double lat, double lng)
        {
            SaveLocationToDisk(lat, lng);
        }

        private static void SaveLocationToDisk(double lat, double lng)
        {
            var coordsPath = Path.Combine(Directory.GetCurrentDirectory(), subPath, "Config", "LastPos.ini");

            File.WriteAllText(coordsPath, $"{lat}:{lng}");
        }

        private static void UnhandledExceptionEventHandler(object obj, UnhandledExceptionEventArgs args)
        {
            Logger.Write("Exception caught, writing LogBuffer.", force: true);
            throw new Exception();
        }

        private void startStopBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => StartBot());
        }

        public static void ColoredConsoleWrite(Color color, string text)
        {
            if (text.Length <= 0)
                return;
            var message = text;
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<Color, string>(ColoredConsoleWrite), color, message);
                return;
            }
            Instance.logTextBox.Select(Instance.logTextBox.Text.Length, 1); // Reset cursor to last
            Instance.logTextBox.SelectionColor = color;
            Instance.logTextBox.AppendText(message);
        }

        private static bool BoolNeedsSetup()
        {
            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), subPath);
            var profileConfigPath = Path.Combine(profilePath, "config");
            var configFile = Path.Combine(profileConfigPath, "config.json");
            return !File.Exists(configFile);
        }

        public void CheckVersion()
        {
            try
            {
                var match =
                    new Regex(
                        @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                        .Match(DownloadServerVersion());

                if (!match.Success) return;
                var gitVersion =
                    new Version(
                        string.Format(
                            "{0}.{1}.{2}.{3}",
                            match.Groups[1],
                            match.Groups[2],
                            match.Groups[3],
                            match.Groups[4]));
                // makes sense to display your version and say what the current one is on github
                Logger.Write("Your version is " + GetExecutingAssembly().GetName().Version);
                Logger.Write("Github version is " + gitVersion);
                Logger.Write("You can find it at www.GitHub.com/TheUnnameOrganization/RocketBot/releases");
            }
            catch (Exception)
            {
                Logger.Write("Unable to check for updates now...", LogLevel.Error);
            }
        }

        private static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/TheUnnameOrganization/RocketBot/Beta-Build/src/RocketBotGUI/Properties/AssemblyInfo.cs");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ReloadPokemonList();
        }

        #region POKEMON LIST

        private IEnumerable<Candy> families;

        private void InitializePokemonForm()
        {
            //olvPokemonList.ButtonClick += PokemonListButton_Click;

            pkmnName.ImageGetter = delegate(object rowObject)
            {
                var pokemon = rowObject as PokemonObject;

                var key = pokemon.PokemonId.ToString();
                if (!olvPokemonList.SmallImageList.Images.ContainsKey(key))
                {
                    var img = GetPokemonImage((int) pokemon.PokemonId);
                    olvPokemonList.SmallImageList.Images.Add(key, img);
                }
                return key;
            };

            olvPokemonList.FormatRow += delegate(object sender, FormatRowEventArgs e)
            {
                var pok = e.Model as PokemonObject;
                if (olvPokemonList.Objects.Cast<PokemonObject>()
                    .Select(i => i.PokemonId)
                    .Where(p => p == pok.PokemonId)
                    .Count() > 1)
                    e.Item.BackColor = Color.LightGreen;

                foreach (OLVListSubItem sub in e.Item.SubItems)
                {
                    if (sub.Text.Equals("Evolve") && !pok.CanEvolve)
                    {
                        sub.CellPadding = new Rectangle(100, 100, 0, 0);
                    }
                }
            };

            cmsPokemonList.Opening += delegate(object sender, CancelEventArgs e)
            {
                e.Cancel = false;
                cmsPokemonList.Items.Clear();

                var pokemons = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o.PokemonData);
                var canAllEvolve =
                    olvPokemonList.SelectedObjects.Cast<PokemonObject>()
                        .Select(o => o)
                        .Where(o => o.CanEvolve == false)
                        .Count() == 0;
                var count = pokemons.Count();

                if (count < 1)
                {
                    e.Cancel = true;
                }

                /** var pokemonObject = olvPokemonList.SelectedObjects.Cast<PokemonObject>().Select(o => o).First();

                 var item = new ToolStripMenuItem();
                 var separator = new ToolStripSeparator();
                 item.Text = "Transfer " + count + " pokemon";
                 item.Click += delegate { TransferPokemon(pokemons); };
                 cmsPokemonList.Items.Add(item);

                 item = new ToolStripMenuItem();
                 item.Text = "Rename";
                 item.Click += delegate
                 {
                     using (var form = count == 1 ? new NicknamePokemonForm(pokemonObject) : new NicknamePokemonForm())
                     {
                         if (form.ShowDialog() == DialogResult.OK)
                         {
                             NicknamePokemon(pokemons, form.txtNickname.Text);
                         }
                     }
                 };
                 cmsPokemonList.Items.Add(item);

                 if (canAllEvolve)
                 {
                     item = new ToolStripMenuItem();
                     item.Text = "Evolve " + count + " pokemon";
                     item.Click += delegate { EvolvePokemon(pokemons); };
                     cmsPokemonList.Items.Add(item);
                 }

                 if (count == 1)
                 {
                     item = new ToolStripMenuItem();
                     item.Text = "PowerUp";
                     item.Click += delegate { PowerUpPokemon(pokemons); };
                     cmsPokemonList.Items.Add(item);

                     cmsPokemonList.Items.Add(separator);

                     item = new ToolStripMenuItem();
                     item.Text = "Transfer Clean Up (Keep highest IV)";
                     item.Click += delegate { CleanUpTransferPokemon(pokemonObject, "IV"); };
                     cmsPokemonList.Items.Add(item);

                     item = new ToolStripMenuItem();
                     item.Text = "Transfer Clean Up (Keep highest CP)";
                     item.Click += delegate { CleanUpTransferPokemon(pokemonObject, "CP"); };
                     cmsPokemonList.Items.Add(item);

                     item = new ToolStripMenuItem();
                     item.Text = "Evolve Clean Up (Highest IV)";
                     item.Click += delegate { CleanUpEvolvePokemon(pokemonObject, "IV"); };
                     cmsPokemonList.Items.Add(item);

                     item = new ToolStripMenuItem();
                     item.Text = "Evolve Clean Up (Highest CP)";
                     item.Click += delegate { CleanUpEvolvePokemon(pokemonObject, "CP"); };
                     cmsPokemonList.Items.Add(item);

                     cmsPokemonList.Items.Add(separator); 
                 }**/
            };
        }

        private Image GetPokemonImage(int pokemonId)
        {
            return (Image) Properties.Resources.ResourceManager.GetObject("Pokemon_" + pokemonId);
        }

        private async Task ReloadPokemonList()
        {
            SetState(false);

            try
            {
                var itemTemplates = await session.Client.Download.GetItemTemplates();
                var inventory = await session.Inventory.GetCachedInventory();
                var profile = await session.Client.Player.GetPlayer();
                var appliedItems = new Dictionary<ItemId, DateTime>();
                var inventoryAppliedItems =
                    await session.Inventory.GetAppliedItems();

                foreach (var aItems in inventoryAppliedItems)
                {
                    if (aItems != null && aItems.Item != null)
                    {
                        foreach (var item in aItems.Item)
                        {
                            appliedItems.Add(item.ItemId, Utils.FromUnixTimeUtc(item.ExpireMs));
                        }
                    }
                }

                PokemonObject.Initilize(itemTemplates);

                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i?.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                        .OrderByDescending(key => key.Cp)
                        .OrderBy(key => key.PokemonId);
                families = inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData.Candy)
                    .Where(p => p != null && p.FamilyId > 0)
                    .OrderByDescending(p => p.FamilyId);

                var pokemonObjects = new List<PokemonObject>();
                foreach (var pokemon in pokemons)
                {
                    var pokemonObject = new PokemonObject(pokemon);
                    var family =
                        families.Where(i => (int) i.FamilyId <= (int) pokemon.PokemonId)
                            .First();
                    pokemonObject.Candy = family.Candy_;
                    pokemonObjects.Add(pokemonObject);
                }

                var prevTopItem = olvPokemonList.TopItemIndex;
                olvPokemonList.SetObjects(pokemonObjects);
                olvPokemonList.TopItemIndex = prevTopItem;

                var pokemoncount =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .Count();
                var eggcount =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.IsEgg == true)
                        .Count();
                lblPokemonList.Text = pokemoncount + eggcount + " / " + profile.PlayerData.MaxPokemonStorage + " (" +
                                      pokemoncount + " pokemon, " + eggcount + " eggs)";

                var items =
                    inventory.InventoryDelta.InventoryItems
                        .Select(i => i.InventoryItemData?.Item)
                        .Where(i => i != null)
                        .OrderBy(i => i.ItemId);
                var itemscount =
                    inventory.InventoryDelta.InventoryItems
                        .Select(i => i.InventoryItemData?.Item)
                        .Where(i => i != null)
                        .Sum(i => i.Count) + 1;

                flpItems.Controls.Clear();
                /** foreach (var item in items)
                 {
                     var box = new ItemBox(item);
                     if (appliedItems.ContainsKey(item.ItemId))
                         box.expires = appliedItems[item.ItemId];
                     box.ItemClick += ItemBox_ItemClick;
                     flpItems.Controls.Add(box);
                 } **/

                lblInventory.Text = itemscount + " / " + profile.PlayerData.MaxItemStorage;
            }
            catch (GoogleException)
            {
                ColoredConsoleWrite(Color.Red, "Please check your google login information again");
            }
            catch (LoginFailedException)
            {
                ColoredConsoleWrite(Color.Red, "Login failed, please check your ptc login information again");
            }
            catch (AccessTokenExpiredException ex)
            {
                ColoredConsoleWrite(Color.Red, ex.Message);
            }
            catch (Exception ex)
            {
                ColoredConsoleWrite(Color.Red, ex.ToString());
            }

            SetState(true);
        }

        private void SetState(bool state)
        {
            btnRefresh.Enabled = state;
            olvPokemonList.Enabled = state;
            flpItems.Enabled = state;
        }

        #endregion POKEMON LIST
    }
}