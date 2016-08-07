using POGOProtos.Enums;
using POGOProtos.Map.Fort;
using PokemonGo.RocketAPI;
using System;

namespace PokemonGo.Bot.ViewModels
{
    public class GymViewModel : FortViewModel
    {
        public string Team { get; }
        public GymViewModel(FortData fort, Client client)
            :base(fort, client)
        {
            Team = Enum.GetName(typeof(TeamColor), fort.OwnedByTeam);
        }
    }
}