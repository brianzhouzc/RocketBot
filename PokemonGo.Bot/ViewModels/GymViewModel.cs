using POGOProtos.Enums;
using POGOProtos.Map.Fort;
using PokemonGo.Bot.MVVMLightUtils;
using System;

namespace PokemonGo.Bot.ViewModels
{
    public class GymViewModel : FortViewModel, IUpdateable<GymViewModel>
    {
        string team;
        public string Team
        {
            get { return team; }
            set { if (Team != value) { team = value; RaisePropertyChanged(); } }
        }



        public GymViewModel(FortData fort, SessionViewModel session)
            : base(fort, session)
        {
            Team = Enum.GetName(typeof(TeamColor), fort.OwnedByTeam);
        }

        public void UpdateWith(GymViewModel other)
        {
            if (!Equals(other))
                throw new ArgumentException($"Expected a gym with Id {Id} but got {Id}.", nameof(other));

            Team = other.Team;
        }
    }
}