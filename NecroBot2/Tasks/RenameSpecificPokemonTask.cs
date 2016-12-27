using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using POGOProtos.Data;
using POGOProtos.Networking.Responses;

namespace NecroBot2.Tasks
{
    public class RenameSpecificPokemonTask
    {
        public static async Task Execute(Session session, PokemonData pokemon, string newNickname)
        {
            //var pkm = pokemon;
            var response = await session.Client.Inventory.NicknamePokemon(pokemon.Id, newNickname);
            Logger.Write(response.Result == NicknamePokemonResponse.Types.Result.Success
                ? $"Successfully renamed {pokemon.PokemonId} to \"{newNickname}\""
                : $"Failed renaming {pokemon.PokemonId} to \"{newNickname}\"");
            DelayingUtils.Delay(session.LogicSettings.DelayBetweenPlayerActions, 0);
            //session.EventDispatcher.Send(new NoticeEvent
            //{
            //    Message =
            //        session.Translation.GetTranslation(TranslationString.PokemonRename,
            //            session.Translation.GetPokemonTranslation(pokemon.PokemonId),
            //            pokemon.PokemonId, pkm.Nickname, newNickname)
            //});
        }
    }
}