using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Google.Protobuf;
using POGOLib.Net;
using POGOLib.Net.Authentication.Providers;
using POGOLib.Pokemon;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using PokemonGo.Bot.Messages;
using PokemonGo.Bot.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PokemonGo.Bot.ViewModels
{
    public class SessionViewModel : ViewModelBase
    {
        readonly MainViewModel main;
        Session session;
        PtcLoginProvider loginProvider;

        bool isLoggedIn;

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                if (IsLoggedIn != value)
                {
                    isLoggedIn = value;
                    RaisePropertyChanged();
                    Login.RaiseCanExecuteChanged();
                }
            }
        }
        #region Login

        AsyncRelayCommand login;

        public AsyncRelayCommand Login
        {
            get
            {
                if (login == null)
                    login = new AsyncRelayCommand(ExecuteLogin, CanExecuteLogin);

                return login;
            }
        }

        async Task ExecuteLogin()
        {
            MessengerInstance.Send(new Message("Logging in"));

            if (main.Settings.AuthType == AuthType.Google)
                MessengerInstance.Send(new Message(Colors.Red, "Google Login is not supported at the moment."));
            else
            {
                loginProvider = new PtcLoginProvider(main.Settings.Username, main.Settings.Password);
                session = await POGOLib.Net.Authentication.Login.GetSession(loginProvider, main.Player.Position.Latitude, main.Player.Position.Longitude);
                IsLoggedIn = await session.Startup();
                main.Settings.UpdateWith(session.GlobalSettings);
                var templates = await DownloadItemTemplates();
                main.Settings.UpdateWith(templates);
                main.Player.UpdateWith(session.Player.Data);

                // update after the settings have been downloaded
                session.Map.Update += Map_Update;
                session.Player.Inventory.Update += Inventory_Update;
                Map_Update(null, null);
                Inventory_Update(null, null);
            }

            if (IsLoggedIn)
                MessengerInstance.Send(new Message(Colors.Green, "Login successfull."));
            else
                MessengerInstance.Send(new Message(Colors.Red, "Login unsuccessfull."));
        }

        bool CanExecuteLogin() => !IsLoggedIn;

        #endregion Login


        public SessionViewModel(MainViewModel main)
        {
            this.main = main;
        }
        async void Inventory_Update(object sender, EventArgs e)
        {
            await main.Player.Inventory.UpdateWith(session.Player.Inventory.InventoryItems);
            main.Player.UpdateWith(session.Player.Data);
        }

        void Map_Update(object sender, EventArgs e)
        {
            main.Map.UpdateWith(session.Map.Cells);
        }

        public void SetPosition(Position3DViewModel position)
        {
            session?.Player?.SetCoordinates(position.Latitude, position.Longitude, position.Altitute);
        }

        #region Direct server calls
        internal Task<FortDetailsResponse> GetFortDetails(string id, double latitude, double longitude)
            => CallServer<FortDetailsMessage, FortDetailsResponse>(RequestType.FortDetails, new FortDetailsMessage { FortId = id, Latitude = latitude, Longitude = longitude });

        internal Task<SetFavoritePokemonResponse> SetFavoritePokemon(ulong pokemonId, bool isFavorite)
            => CallServer<SetFavoritePokemonMessage, SetFavoritePokemonResponse>(RequestType.SetFavoritePokemon, new SetFavoritePokemonMessage { IsFavorite = isFavorite, PokemonId = (long)pokemonId });

        internal Task<FortSearchResponse> SearchFort(string id, double latitude, double longitude)
            => CallServer<FortSearchMessage, FortSearchResponse>(RequestType.FortSearch, new FortSearchMessage { FortId = id, FortLatitude = latitude, FortLongitude = longitude, PlayerLatitude = main.Player.Position.Latitude, PlayerLongitude = main.Player.Position.Longitude });

        internal Task<EvolvePokemonResponse> EvolvePokemon(ulong pokemonId)
            => CallServer<EvolvePokemonMessage, EvolvePokemonResponse>(RequestType.EvolvePokemon, new EvolvePokemonMessage { PokemonId = pokemonId });

        internal Task<CatchPokemonResponse> CatchPokemon(ulong encounterId, string spawnPointId, ItemId itemPokeBall, double normalizedRecticleSize = 1.950, double spinModifier = 1, double normalizedHitPos = 1)
            => CallServer<CatchPokemonMessage, CatchPokemonResponse>(RequestType.CatchPokemon, new CatchPokemonMessage { EncounterId = encounterId, SpawnPointId = spawnPointId, Pokeball = itemPokeBall, HitPokemon = true, NormalizedHitPosition = normalizedHitPos, NormalizedReticleSize = normalizedRecticleSize, SpinModifier = spinModifier });

        internal Task UseCaptureItem(ulong encounterId, ItemId itemId, string spawnPointId)
            => CallServer<UseItemCaptureMessage, UseItemCaptureResponse>(RequestType.UseItemCapture, new UseItemCaptureMessage { EncounterId = encounterId, ItemId = itemId, SpawnPointId = spawnPointId });

        internal Task<ReleasePokemonResponse> TransferPokemon(ulong pokemonId)
            => CallServer<ReleasePokemonMessage, ReleasePokemonResponse>(RequestType.ReleasePokemon, new ReleasePokemonMessage { PokemonId = pokemonId });

        internal Task<UpgradePokemonResponse> UpgradePokemon(ulong pokemonId)
            => CallServer<UpgradePokemonMessage, UpgradePokemonResponse>(RequestType.UpgradePokemon, new UpgradePokemonMessage { PokemonId = pokemonId });


        internal Task<RecycleInventoryItemResponse> RecycleInventoryItem(ItemId itemType, int count)
            => CallServer<RecycleInventoryItemMessage, RecycleInventoryItemResponse>(RequestType.RecycleInventoryItem, new RecycleInventoryItemMessage { Count = count, ItemId = itemType });

        internal Task<EncounterResponse> EncounterPokemon(ulong encounterId, string spawnPointId)
            => CallServer<EncounterMessage, EncounterResponse>(RequestType.Encounter, new EncounterMessage { EncounterId = encounterId, SpawnPointId = spawnPointId, PlayerLatitude = main.Player.Position.Latitude, PlayerLongitude = main.Player.Position.Longitude });

        internal Task<DiskEncounterResponse> EncounterDiskPokemon(ulong encounterId, string fortId)
            => CallServer<DiskEncounterMessage, DiskEncounterResponse>(RequestType.DiskEncounter, new DiskEncounterMessage { EncounterId = encounterId, FortId = fortId, PlayerLatitude = main.Player.Position.Latitude, PlayerLongitude = main.Player.Position.Longitude });

        internal Task<UseItemEggIncubatorResponse> UseItemEggIncubator(string incubatorId, ulong eggId)
            => CallServer<UseItemEggIncubatorMessage, UseItemEggIncubatorResponse>(RequestType.UseItemEggIncubator, new UseItemEggIncubatorMessage { ItemId = incubatorId, PokemonId = eggId });

        async Task<TResult> CallServer<TMessage, TResult>(RequestType type, TMessage message)
            where TResult : IMessage<TResult>, new()
            where TMessage : IMessage<TMessage>
        {
            try
            {
                var response = await session.RpcClient.SendRemoteProcedureCall(new Request
                {
                    RequestType = type,
                    RequestMessage = message.ToByteString()
                });

                var parser = new MessageParser<TResult>(() => new TResult());
                return parser.ParseFrom(response);
            }
            catch(Exception e)
            {
                MessengerInstance.Send(new Message(Colors.Red, e.ToString()));
                return default(TResult);
            }
        }

        Task<DownloadItemTemplatesResponse> DownloadItemTemplates()
            => CallServer<DownloadItemTemplatesMessage, DownloadItemTemplatesResponse>(RequestType.DownloadItemTemplates, new DownloadItemTemplatesMessage());

        #endregion Direct server calls
    }
}