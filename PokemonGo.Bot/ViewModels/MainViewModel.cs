using GalaSoft.MvvmLight;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using PokemonGo.Bot.Utils;
using System.Collections.Generic;

namespace PokemonGo.Bot.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        const string settingsFile = "settings.json";

        public MainViewModel()
        {
            Settings = Settings.LoadFromFile(settingsFile);
            Session = new SessionViewModel(this);
            var transferPokemonAlgorithmFactory = new TransferPokemonAlgorithmFactory(Settings);
            var inventory = new InventoryViewModel(Session, transferPokemonAlgorithmFactory);
            Map = new MapViewModel(Session, Settings);
            Player = new PlayerViewModel(inventory, Map, Settings, Session);
            Map.Player = Player;
            inventory.Player = Player;
            Bot = new BotViewModel(Player, Map);
            Console = new ConsoleViewModel();
            Players = new[] { Player };
        }

        public MapViewModel Map { get; }
        public PlayerViewModel Player { get; }
        public BotViewModel Bot { get; }
        public ConsoleViewModel Console { get; }
        public IEnumerable<PlayerViewModel> Players { get; }
        public Settings Settings { get; }
        public SessionViewModel Session { get; }
    }
}