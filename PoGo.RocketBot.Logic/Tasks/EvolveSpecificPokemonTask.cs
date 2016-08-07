#region using directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoGo.RocketBot.Logic.Event;
using PoGo.RocketBot.Logic.PoGoUtils;
using PoGo.RocketBot.Logic.State;
using PoGo.RocketBot.Logic.Utils;
using PoGo.RocketBot.Logic.Logging;
using PoGo.RocketBot.Logic.Common;

#endregion

namespace PoGo.RocketBot.Logic.Tasks
{
    public class EvolveSpecificPokemonTask
    {
        public static async Task Execute(ISession session, ulong pokemonId)
        {
            var all = await session.Inventory.GetPokemons();
            var pokemons = all.OrderByDescending(x => x.Cp).ThenBy(n => n.StaminaMax);
            var pokemon = pokemons.FirstOrDefault(p => p.Id == pokemonId);

            if (pokemon == null) return;

            var evolveResponse = await session.Client.Inventory.EvolvePokemon(pokemon.Id);

            session.EventDispatcher.Send(new PokemonEvolveEvent
            {
                Id = pokemon.PokemonId,
                Exp = evolveResponse.ExperienceAwarded,
                Result = evolveResponse.Result
            });
            DelayingUtils.Delay(session.LogicSettings.DelayBetweenPlayerActions, 2000);
        }
    }
}