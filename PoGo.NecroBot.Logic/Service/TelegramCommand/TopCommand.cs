using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Data;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class TopCommand : CommandMessage
    {
        public override string Command => "/top";
        public override string Description => "<cp/iv> <amount> - Shows you top Pokemons. ";
        public override bool StopProcess => true;

        public TopCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> Callback)
        {
            string[] messagetext = cmd.Split(' ');
            string answerTextmessage = "";

            if (messagetext[0].ToLower() == Command)
            {
                var times = 10;
                var sortby = "cp";

                if (messagetext.Length >= 2)
                {
                    sortby = messagetext[1];
                }
                if (messagetext.Length == 3)
                {
                    try
                    {
                        times = Convert.ToInt32(messagetext[2]);
                    }
                    catch (FormatException)
                    {
                        answerTextmessage =
                            session.Translation.GetTranslation(TranslationString.UsageHelp, "/top [cp/iv] [amount]");
                    }
                }
                else if (messagetext.Length > 3)
                {
                    answerTextmessage =
                        session.Translation.GetTranslation(TranslationString.UsageHelp, "/top [cp/iv] [amount]");
                }

                IEnumerable<PokemonData> topPokemons = null;
                if (sortby.Equals("iv"))
                {
                    topPokemons = await session.Inventory.GetHighestsPerfect(times);
                }
                else if (sortby.Equals("cp"))
                {
                    topPokemons = await session.Inventory.GetHighestsCp(times);
                }
                else
                {
                    answerTextmessage =
                        session.Translation.GetTranslation(TranslationString.UsageHelp, "/top [cp/iv] [amount]");
                }

                foreach (var pokemon in topPokemons)
                {
                    answerTextmessage += session.Translation.GetTranslation(TranslationString.ShowPokeSkillTemplate,
                        pokemon.Cp, PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00"),
                        session.Translation.GetPokemonMovesetTranslation(PokemonInfo.GetPokemonMove1(pokemon)),
                        session.Translation.GetPokemonMovesetTranslation(PokemonInfo.GetPokemonMove2(pokemon)),
                        session.Translation.GetPokemonTranslation(pokemon.PokemonId));

                    if (answerTextmessage.Length > 3800)
                    {
                        Callback(answerTextmessage);
                        answerTextmessage = "";
                    }
                }

                Callback(answerTextmessage);
                return true;
            }
            return false;
        }
    }
}