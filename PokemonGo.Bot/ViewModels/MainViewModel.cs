using GalaSoft.MvvmLight;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using PokemonGo.Bot.Utils;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;
using System.Collections.Generic;

namespace PokemonGo.Bot.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        const string settingsFile = "settings.json";
        public MainViewModel()
        {
            Settings = Settings.LoadFromFile(settingsFile);
            var client = new Client(Settings, new ApiFailureStrategy());
            Settings.Client = client;
            var transferPokemonAlgorithmFactory = new TransferPokemonAlgorithmFactory(Settings);
            var inventory = new InventoryViewModel(client, transferPokemonAlgorithmFactory);
            Map = new MapViewModel(client, Settings);
            Player = new PlayerViewModel(client, inventory, Map, Settings);
            Map.Player = Player;
            inventory.Player = Player;
            Bot = new BotViewModel(client, Player, Map);
            Console = new ConsoleViewModel();
            Players = new[] { Player };
        }
        public MapViewModel Map { get; }
        public PlayerViewModel Player { get; }
        public BotViewModel Bot { get; }
        public ConsoleViewModel Console { get; }
        public IEnumerable<PlayerViewModel> Players { get; }
        public Settings Settings { get; }
    }
}