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
        public override string Description => "Shows your profile. ";
        public override bool StopProcess => true;

        public ProfileCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> Callback)
        {
            if (cmd.ToLower() == Command)
            {
                string answerTextmessage = "";

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
                Callback(answerTextmessage);
                return true;
            }
            return false;
        }
    }
}