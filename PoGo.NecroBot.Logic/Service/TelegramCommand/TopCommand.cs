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
    public class TopCommand : CommandMessage
    {
        private const int DefaultCount = 10;
        private const string DefaultOrderBy = "cp";

        private string CommandParseRegex => "^(\\" + Command + ")(?>(?>\\s+(?<orderBy>iv|cp))|(?>\\s+(?<count>\\d+))){0,2}\\s*";

        public override string Command => "/top";
        public override string Arguments => "[iv|cp] [n]";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandTopDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandTopMsgHead;

        public TopCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override string GetDescription(ISession session) =>
            base.GetDescription(session, DefaultCount.ToString());

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> callback)
        {
            var commandMatch = Match(cmd, CommandParseRegex);

            if (!commandMatch.Success)
            {
                return false;
            }

            // Parse count
            int count;
            try
            {
                count = string.IsNullOrEmpty(commandMatch.Groups["count"].Value)
                    ? DefaultCount
                    : Convert.ToInt32(commandMatch.Groups["count"].Value);
            }
            catch (FormatException)
            {
                // Exception should not be thrown ...
                return false;
            }

            // Parse orderBy
            var orderBy = string.IsNullOrEmpty(commandMatch.Groups["orderBy"].Value)
                ? DefaultOrderBy
                : commandMatch.Groups["orderBy"].Value;

            // Get 'count' top Pokemon and 'orderBy' -> will never be null
            var topPokemon = string.Equals("iv", orderBy)
                ? await session.Inventory.GetHighestsPerfect(count)
                : await session.Inventory.GetHighestsCp(count);

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