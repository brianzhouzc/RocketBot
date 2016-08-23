using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketBot.Logic.State;
using PokemonGo.RocketBot.Logic.Event;
using PokemonGo.RocketBot.Logic.Common;
using POGOProtos.Data;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketBot.Logic.Logging;
using PokemonGo.RocketBot.Logic.Utils;

namespace PokemonGo.RocketBot.Logic.Tasks
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
