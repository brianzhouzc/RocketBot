using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Data;
using static System.Text.RegularExpressions.Regex;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class AllCommand : CommandMessage
    {
        private const string DefaultOrderBy = "cp";

        private string CommandParseRegex => "^(\\" + Command + ")(?>\\s+(?<orderBy>iv|cp))?\\s*";

        public override string Command => "/all";
        public override string Arguments => "[iv|cp]";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandAllDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandAllMsgHead;

        public AllCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> callback)
        {
            var commandMatch = Match(cmd, CommandParseRegex);

            if (!commandMatch.Success)
            {
                return false;
            }

            // Parse orderBy
            var orderBy = string.IsNullOrEmpty(commandMatch.Groups["orderBy"].Value)
                ? DefaultOrderBy
                : commandMatch.Groups["orderBy"].Value;

            var allPokemonCount = (await session.Inventory.GetPokemons()).Count();

            // Get all Pokemon and 'orderBy' -> will never be null
            var topPokemon = string.Equals("iv", orderBy)
                ? await session.Inventory.GetHighestsPerfect(allPokemonCount)
                : await session.Inventory.GetHighestsCp(allPokemonCount);

            var topPokemonList = topPokemon as IList<PokemonData> ?? topPokemon.ToList();

            var answerTextmessage = GetMsgHead(session, session.Profile.PlayerData.Username, topPokemonList.Count) + "\r\n\r\n";
            answerTextmessage = topPokemonList.Aggregate(answerTextmessage, (current, pokemon)
                => current + session.Translation.GetTranslation(
                       TranslationString.ShowPokeSkillTemplate,
                       pokemon.Cp,
                       PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00"),
                       session.Translation.GetPokemonMovesetTranslation(PokemonInfo.GetPokemonMove1(pokemon)),
                       session.Translation.GetPokemonMovesetTranslation(PokemonInfo.GetPokemonMove2(pokemon)),
                       session.Translation.GetPokemonTranslation(pokemon.PokemonId)
                   )
            );

            callback(answerTextmessage);
            return true;
        }
    }
}