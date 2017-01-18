using System;
using System.Reflection;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class StatusCommand : CommandMessage
    {
        public override string Command => "/status";
        public override string Description => "Shows you the Status of the Bot.";
        public override bool StopProcess => true;

        public StatusCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        #pragma warning disable 1998 // added to get rid of compiler warning. Remove this if async code is used below.

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<string> callback)
        #pragma warning restore 1998
        {
            if (cmd.ToLower() == Command)
            {
                var necroBotVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
                var necroBotStatistics = session.RuntimeStatistics;
                var necrobotStats = necroBotStatistics.GetCurrentInfo(session.Inventory);

                var answerCatchLimit = "diabled";
                var answerPokestopLimit = "disabled";

                if (session.LogicSettings.UseCatchLimit)
                {
                    answerCatchLimit = string.Format(
                        "{0}/{1}",
                        session.Stats.GetNumPokemonsInLast24Hours(),
                        session.LogicSettings.CatchPokemonLimit
                    );
                }

                if (session.LogicSettings.UsePokeStopLimit)
                {
                    answerPokestopLimit = string.Format(
                        "{0}/{1}",
                        session.Stats.GetNumPokestopsInLast24Hours(),
                        session.LogicSettings.PokeStopLimit
                    );
                }

                var answerTextmessage = string.Format(
                    "Bot: Necrobot2 v{0}\n" +
                    "Account: {1}\n" +
                    "Runtime: {2}\n" +
                    "Level: {3}\n" +
                    "Advance in: {4}h {5}m | {6} EP\n" +
                    "XP / h: {7:n0}\n" +
                    "Poke / h: {8:n0}\n" +
                    "Stardust / h: {9:n0}\n" +
                    "Poke Sent: {10}\n" +
                    "Poke Evolved: {11}\n" +
                    "Recycled: {12}\n" +
                    "Pokestop limit: {13}\n" +
                    "Catch limit: {14}",
                    necroBotVersion,
                    session.Profile.PlayerData.Username,
                    necroBotStatistics.FormatRuntime(),
                    necrobotStats.Level,
                    necrobotStats.HoursUntilLvl,
                    necrobotStats.MinutesUntilLevel,
                    necrobotStats.LevelupXp - necrobotStats.CurrentXp,
                    necroBotStatistics.TotalExperience / necroBotStatistics.GetRuntime(),
                    necroBotStatistics.TotalPokemons / necroBotStatistics.GetRuntime(),
                    necroBotStatistics.TotalStardust / necroBotStatistics.GetRuntime(),
                    necroBotStatistics.TotalPokemonTransferred,
                    necroBotStatistics.TotalPokemonEvolved,
                    necroBotStatistics.TotalItemsRemoved,
                    answerCatchLimit,
                    answerPokestopLimit
                );

                callback(answerTextmessage);
                return true;
            }
            return false;
        }
    }
}