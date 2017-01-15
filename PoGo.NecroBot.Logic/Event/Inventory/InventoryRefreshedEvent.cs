using POGOProtos.Inventory;
using POGOProtos.Networking.Responses;
using POGOProtos.Settings.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Data.Player;

namespace PoGo.NecroBot.Logic.Event.Inventory
{
    public class InventoryRefreshedEvent :IEvent
    {
        public IEnumerable<PlayerStats> PlayerStats { get; set; }

        public GetInventoryResponse Inventory { get; set; }

        public List<PokemonSettings> PokemonSettings { get; set; }

        public List<Candy> Candies { get; set; }

        public InventoryRefreshedEvent(GetInventoryResponse e, List<PokemonSettings> settings, List<Candy> candy)
        {
            this.Candies = candy;
            this.PokemonSettings = settings;
            this.Inventory = e;
        }

        public InventoryRefreshedEvent(GetInventoryResponse args, IEnumerable<PlayerStats> playerStats, List<PokemonSettings> pokemonSettings, List<Candy> candy)
        {
            this.Inventory = args;
            this.PlayerStats = playerStats;
            this.PokemonSettings = pokemonSettings;
            this.Candies = candy;
        }
    }
}
