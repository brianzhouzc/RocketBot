#region using directives

using System;
using System.Threading;
using System.Threading.Tasks;
using PoGo.RocketBot.Logic.Common;
using PoGo.RocketBot.Logic.Event;
using PoGo.RocketBot.Logic.PoGoUtils;
using PoGo.RocketBot.Logic.State;

#endregion

namespace PoGo.RocketBot.Logic.Tasks
{
    public class FavoritePokemonTask
    {
        public static async Task Execute(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pokemons = await session.Inventory.GetPokemons();

            foreach (var pokemon in pokemons)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var perfection = Math.Round(PokemonInfo.CalculatePokemonPerfection(pokemon));

                if (session.LogicSettings.AutoFavoritePokemon && perfection >= session.LogicSettings.FavoriteMinIvPercentage && pokemon.Favorite!=1)
                {
                    await session.Client.Inventory.SetFavoritePokemon(pokemon.Id, true);

                    session.EventDispatcher.Send(new NoticeEvent
                    {
                        Message =
                            session.Translation.GetTranslation(TranslationString.PokemonFavorite, perfection, session.Translation.GetPokemonTranslation(pokemon.PokemonId), pokemon.Cp)
                    });
                }
            }
        }
    }
}