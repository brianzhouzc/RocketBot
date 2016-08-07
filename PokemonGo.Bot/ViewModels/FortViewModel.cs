using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Map.Fort;
using PokemonGo.RocketAPI;

namespace PokemonGo.Bot.ViewModels
{
    public class FortViewModel : ViewModelBase
    {
        protected readonly Client client;

        public Position2DViewModel Position { get; }

        public string Id { get; }

        public FortViewModel(FortData fort, Client client)
        {
            Position = new Position2DViewModel(fort.Latitude, fort.Longitude);
            Id = fort.Id;
            this.client = client;
        }
    }
}