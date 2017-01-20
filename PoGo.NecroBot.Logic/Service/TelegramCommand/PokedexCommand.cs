using System;
using System.Linq;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Enums;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class PokedexCommand : CommandMessage
    {
        public override string Command => "/pokedex";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandPokedexDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandPokedexMsgHead;

        public PokedexCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        #pragma warning disable 1998  // added to get rid of compiler warning. Remove this if async code is used below.
        public override async Task<bool> OnCommand(ISession session,string cmd, Action<string> callback)
        #pragma warning restore 1998
        {
            if (cmd.ToLower() == Command)
            {
                var pokedex = session.Inventory.GetPokeDexItems().Result;
                var pokedexSort = pokedex.OrderBy(x => x.InventoryItemData.PokedexEntry.PokemonId);
                var answerTextmessage = GetMsgHead(session, session.Profile.PlayerData.Username) + "\r\n\r\n";

                answerTextmessage += session.Translation.GetTranslation(TranslationString.PokedexCatchedTelegram);
                foreach (var pokedexItem in pokedexSort)
                {
                    answerTextmessage +=
                        session.Translation.GetTranslation(TranslationString.PokedexPokemonCatchedTelegram,
                            Convert.ToInt32(pokedexItem.InventoryItemData.PokedexEntry.PokemonId),
                            session.Translation.GetPokemonTranslation(
                                pokedexItem.InventoryItemData.PokedexEntry.PokemonId),
                            pokedexItem.InventoryItemData.PokedexEntry.TimesCaptured,
                            pokedexItem.InventoryItemData.PokedexEntry.TimesEncountered);

                    if (answerTextmessage.Length > 3800)
                    {
                        callback(answerTextmessage);
                        answerTextmessage = "";
                    }
                }

                var pokemonsToCapture =
                    Enum.GetValues(typeof(PokemonId))
                        .Cast<PokemonId>()
                        .Except(pokedex.Select(x => x.InventoryItemData.PokedexEntry.PokemonId));

                callback(answerTextmessage);
                answerTextmessage = "";

                answerTextmessage += session.Translation.GetTranslation(TranslationString.PokedexNeededTelegram);

                foreach (var pokedexItem in pokemonsToCapture)
                {
                    if (Convert.ToInt32(pokedexItem) > 0)
                    {
                        answerTextmessage +=
                            session.Translation.GetTranslation(TranslationString.PokedexPokemonNeededTelegram,
                                Convert.ToInt32(pokedexItem), session.Translation.GetPokemonTranslation(pokedexItem));

                        if (answerTextmessage.Length > 3800)
                        {
                            callback(answerTextmessage);
                            answerTextmessage = "";
                        }
                    }
                }
                callback(answerTextmessage);
                return true;
            }
            return false;
        }
    }
}