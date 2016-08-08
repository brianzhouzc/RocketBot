﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Map.Fort;
using PokemonGo.RocketAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.Bot.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        readonly Client client;

        public AsyncRelayCommand GetMapObjects { get; }
        public AsyncRelayCommand<Position3DViewModel> SetPosition { get; }

        public ObservableCollection<PokestopViewModel> Pokestops { get; } = new ObservableCollection<PokestopViewModel>();
        public ObservableCollection<GymViewModel> Gyms { get; } = new ObservableCollection<GymViewModel>();
        public ObservableCollection<WildPokemonViewModel> WildPokemon { get; } = new ObservableCollection<WildPokemonViewModel>();
        public ObservableCollection<MapPokemonViewModel> CatchablePokemon { get; } = new ObservableCollection<MapPokemonViewModel>();

        public MapViewModel(Client client)
        {
            this.client = client;

            GetMapObjects = new AsyncRelayCommand(async () =>
            {
                var mapResponse = await client.Map.GetMapObjects();
                Pokestops.UpdateWith(mapResponse.Item1.MapCells.SelectMany(m => m.Forts).Where(f => f.Type == FortType.Checkpoint).Select(f => new PokestopViewModel(f, client)));
                Gyms.UpdateWith(mapResponse.Item1.MapCells.SelectMany(m => m.Forts).Where(f => f.Type == FortType.Gym).Select(f => new GymViewModel(f, client)));
                WildPokemon.UpdateWith(mapResponse.Item1.MapCells.SelectMany(m => m.WildPokemons).Select(p => new WildPokemonViewModel(p)));
                CatchablePokemon.UpdateWith(mapResponse.Item1.MapCells.SelectMany(m => m.CatchablePokemons).Select(p => new MapPokemonViewModel(p)));
            });

            SetPosition = new AsyncRelayCommand<Position3DViewModel>(async pos =>
            {
                await client.Player.UpdatePlayerLocation(pos.Latitude, pos.Longitude, pos.Altitute);
                await GetMapObjects.ExecuteAsync();
            });
        }
    }
}