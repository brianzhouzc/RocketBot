using System;
using System.Reflection;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    // TODO I18N
    public class StatusCommand : CommandMessage
    {
        public override string Command => "/status";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandStatusDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandStatusMsgHead;

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
                        "{0} / {1}",
                        session.Stats.GetNumPokemonsInLast24Hours(),
                        session.LogicSettings.CatchPokemonLimit
                    );
                }

                if (session.LogicSettings.UsePokeStopLimit)
                {
                    answerPokestopLimit = string.Format(
                        "{0} / {1}",
                        session.Stats.GetNumPokestopsInLast24Hours(),
                        session.LogicSettings.PokeStopLimit
                    );
                }

                var answerTextmessage = GetMsgHead(session, session.Profile.PlayerData.Username) + "\r\n\r\n";

                answerTextmessage += session.Translation.GetTranslation(
                    TranslationString.TelegramCommandStatusMsgBody,
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