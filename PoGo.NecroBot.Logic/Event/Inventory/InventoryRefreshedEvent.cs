using POGOProtos.Inventory;
using POGOProtos.Networking.Responses;
using POGOProtos.Settings.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Event.Inventory
{
    public class InventoryRefreshedEvent :IEvent
    {
        public GetInventoryResponse Inventory { get; set; }

        public List<PokemonSettings> PokemonSettings { get; set; }

        public List<Candy> Candies { get; set; }

        public InventoryRefreshedEvent(GetInventoryResponse e, List<PokemonSettings> settings, List<Candy> candy)
        {
            this.Candies = candy;
            this.PokemonSettings = settings;
            this.Inventory = e;
        }
    }
}
