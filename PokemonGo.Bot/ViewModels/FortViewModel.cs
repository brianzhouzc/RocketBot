using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using POGOProtos.Map.Fort;

namespace PokemonGo.Bot.ViewModels
{
    public class FortViewModel : ViewModelBase
    {
        protected readonly SessionViewModel session;

        public Position2DViewModel Position { get; }

        public string Id { get; }
        public AsyncRelayCommand Details { get; }

        public FortViewModel(FortData fort, SessionViewModel session)
        {
            Position = new Position2DViewModel(fort.Latitude, fort.Longitude);
            Id = fort.Id;
            this.session = session;

            Details = new AsyncRelayCommand(async () =>
            {
                var fortInfo = await session.GetFortDetails(Id, Position.Latitude, Position.Longitude);
            });
        }

        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object obj) => Equals(obj as FortViewModel);
        public bool Equals(FortViewModel other) => Id == other?.Id;
    }
}