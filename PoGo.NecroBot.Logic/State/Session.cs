#region using directives
using System.Linq;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Interfaces.Configuration;
using PoGo.NecroBot.Logic.Model.Settings;
using PoGo.NecroBot.Logic.Service;
using PokemonGo.RocketAPI;
using POGOProtos.Networking.Responses;
using PoGo.NecroBot.Logic.Service.Elevation;
using System.Collections.Generic;
using POGOProtos.Map.Fort;
using System;
using PokemonGo.RocketAPI.Extensions;
using PoGo.NecroBot.Logic.Model;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Caching;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Utils;
using System.IO;

#endregion

namespace PoGo.NecroBot.Logic.State
{
    public interface ISession
    {
        ISettings Settings { get; set; }
        Inventory Inventory { get; }
        Client Client { get; }
        GetPlayerResponse Profile { get; set; }
        Navigation Navigation { get; }
        ILogicSettings LogicSettings { get; set; }
        ITranslation Translation { get; }
        IEventDispatcher EventDispatcher { get; }
        TelegramService Telegram { get; set; }
        SessionStats Stats { get; }
        IElevationService ElevationService { get; set; }
        List<FortData> Forts { get; set; }
        List<FortData> VisibleForts { get; set; }
        bool ReInitSessionWithNextBot(AuthConfig authConfig = null, double lat = 0, double lng = 0, double att = 0);
        void AddForts(List<FortData> mapObjects);
        void AddVisibleForts(List<FortData> mapObjects);
        Task<bool> WaitUntilActionAccept(BotActions action, int timeout = 30000);
        List<BotActions> Actions { get; }
        CancellationTokenSource CancellationTokenSource { get; set; }
        MemoryCache Cache { get; set; }
        List<AuthConfig> Accounts { get; }
        DateTime LoggedTime { get; set; }
        DateTime CatchBlockTime { get; set; }

        void BlockCurrentBot(int expired = 15);
    }


    public class Session : ISession
    {
        public Session(ISettings settings, ILogicSettings logicSettings, IElevationService elevationService) : this(settings, logicSettings, elevationService, Common.Translation.Load(logicSettings))
        {
            LoggedTime = DateTime.Now;
        }
        public DateTime LoggedTime { get; set; }
        private List<AuthConfig> accounts;
        public List<BotActions> Actions { get { return this.botActions; } }
        public Session(ISettings settings, ILogicSettings logicSettings, IElevationService elevationService, ITranslation translation)
        {
            this.CancellationTokenSource = new CancellationTokenSource();
            this.Forts = new List<FortData>();
            this.VisibleForts = new List<FortData>();
            this.Cache = new MemoryCache("Necrobot2");
            this.accounts = new List<AuthConfig>();
            this.EventDispatcher = new EventDispatcher();
            this.LogicSettings = logicSettings;

            this.ElevationService = elevationService;

            this.Settings = settings;

            this.Translation = translation;
            this.Reset(settings, LogicSettings);
            this.Stats = new SessionStats(this);
            this.accounts.AddRange(logicSettings.Bots);
            if (!this.accounts.Any(x => (x.AuthType == PokemonGo.RocketAPI.Enums.AuthType.Ptc && x.PtcUsername == settings.PtcUsername) ||
                                        (x.AuthType == PokemonGo.RocketAPI.Enums.AuthType.Google && x.GoogleUsername == settings.GoogleUsername)
                                        ))
            {
                this.accounts.Add(new AuthConfig()
                {
                    AuthType = settings.AuthType,
                    GooglePassword = settings.GooglePassword,
                    GoogleUsername = settings.GoogleUsername,
                    PtcPassword = settings.PtcPassword,
                    PtcUsername = settings.PtcUsername
                });
            }
            if (File.Exists("runtime.log"))
            {
                var lines = File.ReadAllLines("runtime.log");
                foreach (var item in lines)
                {
                    var arr = item.Split(';');
                    var acc = this.accounts.FirstOrDefault(p => p.PtcUsername == arr[0] || p.GoogleUsername == arr[1]);
                    if (acc != null)
                    {
                        acc.RuntimeTotal = Convert.ToDouble(arr[1]);
                        
                    }
                }
            }
        }
        public List<FortData> Forts { get; set; }
        public List<FortData> VisibleForts { get; set; }
        public GlobalSettings GlobalSettings { get; set; }

        public ISettings Settings { get; set; }

        public Inventory Inventory { get; private set; }

        public Client Client { get; private set; }

        public GetPlayerResponse Profile { get; set; }
        public Navigation Navigation { get; private set; }

        public ILogicSettings LogicSettings { get; set; }

        public ITranslation Translation { get; }

        public IEventDispatcher EventDispatcher { get; }

        public TelegramService Telegram { get; set; }

        public SessionStats Stats { get; set; }

        public IElevationService ElevationService { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public MemoryCache Cache { get; set; }
        public List<AuthConfig> Accounts
        {
            get
            {
                return this.accounts;
            }
        }

        public DateTime CatchBlockTime { get; set; }
        private List<BotActions> botActions = new List<BotActions>();
        public void Reset(ISettings settings, ILogicSettings logicSettings)
        {
            Client = new Client(settings);
            // ferox wants us to set this manually
            Inventory = new Inventory(Client, logicSettings);
            Navigation = new Navigation(Client, logicSettings);
            Navigation.WalkStrategy.UpdatePositionEvent +=
                (lat, lng) => this.EventDispatcher.Send(new UpdatePositionEvent { Latitude = lat, Longitude = lng });
          
        }
        //TODO : Need add BotManager to manage all feature related to multibot, 
        public bool ReInitSessionWithNextBot(AuthConfig bot = null, double lat = 0, double lng = 0, double att = 0)
        {
            this.CatchBlockTime = DateTime.Now; //remove any block
            
            var currentAccount = this.accounts.FirstOrDefault(x => (x.AuthType == PokemonGo.RocketAPI.Enums.AuthType.Ptc && x.PtcUsername == this.Settings.PtcUsername) ||
                                        (x.AuthType == PokemonGo.RocketAPI.Enums.AuthType.Google && x.GoogleUsername == this.Settings.GoogleUsername));
            if (LoggedTime != DateTime.MinValue)
            {
                currentAccount.RuntimeTotal += (DateTime.Now - LoggedTime).TotalMinutes;
            }

            this.accounts = this.accounts.OrderByDescending(p => p.RuntimeTotal).ToList();
            var first = this.accounts.First();
            if(first.RuntimeTotal >= 100000)
            {
                first.RuntimeTotal = this.accounts.Min(p => p.RuntimeTotal);
            }

            var nextBot = bot != null ? bot : this.accounts.LastOrDefault(p => p != currentAccount && p.ReleaseBlockTime < DateTime.Now);
            if (nextBot != null)
            {
                Logger.Write($"Switching to {nextBot.GoogleUsername}{nextBot.PtcUsername}...");
                string body = "";

                File.Delete("runtime.log");
                List<string> logs = new List<string>();

                foreach (var item in this.Accounts)
                {
                    
                    int day = (int)item.RuntimeTotal / 1440;
                    int hour = (int)(item.RuntimeTotal - (day * 1400)) / 60;
                    int min = (int)(item.RuntimeTotal - (day * 1400) - hour * 60);

                    body = body + $"{item.GoogleUsername}{item.PtcUsername}     {day:00}:{hour:00}:{min:00}:00\r\n";
                    logs.Add($"{item.GoogleUsername}{item.PtcUsername};{item.RuntimeTotal}");
                }
                File.AppendAllLines("runtime.log", logs);
                PushNotificationClient.SendNotification(this,$"Account changed to {nextBot.GoogleUsername}{nextBot.PtcUsername}",body);

                this.Settings.AuthType = nextBot.AuthType;
                this.Settings.GooglePassword = nextBot.GooglePassword;
                this.Settings.GoogleUsername = nextBot.GoogleUsername;
                this.Settings.PtcPassword = nextBot.PtcPassword;
                this.Settings.PtcUsername = nextBot.PtcUsername;
                this.Settings.DefaultAltitude = att == 0 ? this.Client.CurrentAltitude : att;
                this.Settings.DefaultLatitude = lat == 0 ? this.Client.CurrentLatitude : lat;
                this.Settings.DefaultLongitude = lng == 0 ? this.Client.CurrentLongitude : lng;
                this.Stats = new SessionStats(this);
                this.Reset(this.Settings, this.LogicSettings);
                CancellationTokenSource.Cancel();
                this.CancellationTokenSource = new CancellationTokenSource();
                
                this.EventDispatcher.Send(new BotSwitchedEvent() {
                });

                if(this.LogicSettings.MultipleBotConfig.DisplayList)
                {
                    foreach (var item in this.accounts)
                    {
                        Logger.Write($"{item.PtcUsername}{item.GoogleUsername} \tRuntime : {item.RuntimeTotal:0.00} min ");
                    }
                }
            }
            return nextBot != null;

        }
        public void AddForts(List<FortData> data)
        {
            this.Forts.RemoveAll(p => data.Any(x => x.Id == p.Id && x.Type == FortType.Checkpoint));
            this.Forts.AddRange(data.Where(x => x.Type == FortType.Checkpoint));
            foreach (var item in data.Where(p => p.Type == FortType.Gym))
            {
                var exist = this.Forts.FirstOrDefault(x => x.Id == item.Id);
                if (exist != null && exist.CooldownCompleteTimestampMs > DateTime.UtcNow.ToUnixTime())
                {
                    continue;
                }
                else
                {
                    this.Forts.RemoveAll(x => x.Id == item.Id);
                    this.Forts.Add(item);
                }
            }
        }

        public void AddVisibleForts(List<FortData> mapObjects)
        {
            var notexist = mapObjects.Where(p => !this.VisibleForts.Any(x => x.Id == p.Id));
            this.VisibleForts.AddRange(notexist);
        }
        public async Task<bool> WaitUntilActionAccept(BotActions action, int timeout = 30000)
        {
            if (botActions.Count == 0) return true;
            var waitTimes = 0;
            while (true && waitTimes < timeout)
            {
                if (botActions.Count == 0) return true;
                ///implement logic of action dependent
                waitTimes += 1000;
                await Task.Delay(1000);
            }
            return false; //timedout
        }

        public void BlockCurrentBot(int expired = 60)
        {
            var currentAccount = this.accounts.FirstOrDefault(x => (x.AuthType == PokemonGo.RocketAPI.Enums.AuthType.Ptc && x.PtcUsername == this.Settings.PtcUsername) ||
                                       (x.AuthType == PokemonGo.RocketAPI.Enums.AuthType.Google && x.GoogleUsername == this.Settings.GoogleUsername));

            currentAccount.ReleaseBlockTime = DateTime.Now.AddMinutes(expired);
        }
    }
}