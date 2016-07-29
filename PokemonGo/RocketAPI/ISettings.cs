#region

using PokemonGo.RocketAPI.Enums;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#endregion

namespace PokemonGo.RocketAPI
{
    public interface ISettings
    {
        AuthType AuthType { get; }
        double DefaultLatitude { get; set; }
        double DefaultLongitude { get; set; }
        string LevelOutput { get; }
        int LevelTimeInterval { get; }
        string GoogleRefreshToken { get; set; }
        string PtcPassword { get; }
        string PtcUsername { get; }
        string Email { get;  }
        string Password { get; }
        bool EvolveAllGivenPokemons { get; }
        string TransferType { get; }
        int TransferCPThreshold { get; }
        int TransferIVThreshold { get; }
        int TravelSpeed { get; }
        int ImageSize { get; }
        bool Recycler { get; }
        ICollection<KeyValuePair<AllEnum.ItemId, int>> ItemRecycleFilter { get; }
        int RecycleItemsInterval { get; }
        string Language { get; }
        string RazzBerryMode { get; }
        double RazzBerrySetting { get; }
        bool CatchPokemon { get; }
    }
}
