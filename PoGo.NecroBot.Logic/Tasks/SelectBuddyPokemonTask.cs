using PoGo.NecroBot.Logic.Event.Player;
using PoGo.NecroBot.Logic.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Tasks
{
    public class SelectBuddyPokemonTask
    {
        public static async Task Execute(ISession session, CancellationToken cancellationToken, ulong pokemonId)
        {
            var pokemon = (await session.Inventory.GetPokemons()).FirstOrDefault(x => x.Id == pokemonId);

            if (pokemon == null) return;

            var response = await session.Client.Player.SelectBuddy(pokemon.Id);

            if(response.Result == POGOProtos.Networking.Responses.SetBuddyPokemonResponse.Types.Result.Success)
            {
                session.EventDispatcher.Send(new BuddyUpdateEvent(response.UpdatedBuddy, pokemon));
            }
        }
    }
}

