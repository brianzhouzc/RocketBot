#region using directives

using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Model;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;

#endregion

namespace RocketBot2.Logic.Tasks
{
    public class EvolveSpecificPokemonTask
    {
        public static async Task Execute(ISession session, ulong pokemonId)
        {
            using (var blocker = new BlockableScope(session, BotActions.Envolve))
            {
                if (!await blocker.WaitToRun()) return;

                var all = session.Inventory.GetPokemons();
                var pokemons = all.OrderByDescending(x => x.Cp).ThenBy(n => n.StaminaMax);
                var pokemon = pokemons.FirstOrDefault(p => p.Id == pokemonId);

                if (pokemon == null) return;

                if (!await session.Inventory.CanEvolvePokemon(pokemon))
                    return;

                var evolveResponse = await session.Client.Inventory.EvolvePokemon(pokemon.Id);

                session.EventDispatcher.Send(new PokemonEvolveEvent
                {
                    OriginalId = pokemonId,
                    Id = pokemon.PokemonId,
                    Exp = evolveResponse.ExperienceAwarded,
                    UniqueId = pokemon.Id,
                    Result = evolveResponse.Result,
                    EvolvedPokemon = evolveResponse.EvolvedPokemonData
                });

                DelayingUtils.Delay(session.LogicSettings.EvolveActionDelay, 0);
            }
        }
    }
}