using System;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class ProfileCommand : CommandMessage
    {
        public override string Command => "/profile";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandProfileDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandProfileMsgHead;

        public ProfileCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> callback)
        {
            if (cmd.ToLower() == Command)
            {
                var answerTextmessage = GetMsgHead(session, session.Profile.PlayerData.Username) + "\r\n\r\n";

                var stats = session.Inventory.GetPlayerStats().Result;
                var stat = stats.FirstOrDefault();

                var myPokemons2 = await session.Inventory.GetPokemons();
                if (stat != null)
                    answerTextmessage += session.Translation.GetTranslation(
                        TranslationString.ProfileStatsTemplateString, stat.Level, session.Profile.PlayerData.Username,
                        stat.Experience, stat.NextLevelXp, stat.PokemonsCaptured, stat.PokemonDeployed,
                        stat.PokeStopVisits, stat.EggsHatched, stat.Evolutions, stat.UniquePokedexEntries,
                        stat.KmWalked,
                        myPokemons2.ToList().Count, session.Profile.PlayerData.MaxPokemonStorage);
                callback(answerTextmessage);
                return true;
            }
            return false;
        }
    }
}