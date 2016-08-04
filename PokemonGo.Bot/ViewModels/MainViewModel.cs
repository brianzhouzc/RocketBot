﻿using GalaSoft.MvvmLight;
using PokemonGo.Bot.TransferPokemonAlgorithms;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Bot;

namespace PokemonGo.Bot.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            var settings = Settings.Instance;
            var client = new Client(settings);
            var transferPokemonAlgorithmFactory = new TransferPokemonAlgorithmFactory(settings);
            var inventory = new InventoryViewModel(client, transferPokemonAlgorithmFactory);
            Map = new MapViewModel(client);
            Player = new PlayerViewModel(client, inventory, Map, settings);
            Bot = new BotViewModel(client, Player, Map, settings);
            Console = new ConsoleViewModel();
        }
        public MapViewModel Map { get; }
        public PlayerViewModel Player { get; }
        public BotViewModel Bot { get; }
        public ConsoleViewModel Console { get; }
    }
}