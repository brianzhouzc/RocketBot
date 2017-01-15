#region using directives

using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;

#endregion

namespace PoGo.NecroBot.Logic.Logging
{
    public class LoggingStrings
    {
        public static string Attention;

        public static string Berry;

        public static string Debug;

        public static string Egg;

        public static string Error;

        public static string Evolved;

        public static string Farming;

        public static string Info;

        public static string Pkmn;

        public static string Pokestop;

        public static string Recycling;

        public static string Sniper;

        public static string Transferred;

        public static string Update;

        public static string New;

        public static string SoftBan;
        public static string Gym;

        public static string Service;
        public static void SetStrings(ISession session)
        {
            Attention =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryAttention) ?? "ATTENTION";

            Berry =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryBerry) ?? "BERRY";

            Debug =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryDebug) ?? "DEBUG";

            Egg =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryEgg) ?? "EGG";

            Error =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryError) ?? "ERROR";

            Evolved =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryEvolved) ?? "EVOLVED";

            Farming =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryFarming) ?? "FARMING";

            Info =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryInfo) ?? "INFO";

            Pkmn =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryPkmn) ?? "PKMN";

            Pokestop =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryPokestop) ?? "POKESTOP";

            Recycling =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryRecycling) ?? "RECYCLING";

            Sniper =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntrySniper) ?? "SNIPER";

            Transferred =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryTransfered) ?? "TRANSFERRED";

            Update =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryUpdate) ?? "UPDATE";

            New =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryNew) ?? "NEW";

            SoftBan =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntrySoftBan) ?? "SOFTBAN";

            Gym =
               session?.Translation.GetTranslation(
                   TranslationString.LogEntryGym) ?? "GYM";

            Service =
                session?.Translation.GetTranslation(
                    TranslationString.LogEntryService) ?? "SERVICE";
        }
    }
}