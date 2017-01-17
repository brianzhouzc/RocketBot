using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Inventory.Item;
using System;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class ItemsCommand : CommandMessage
    {
        public override string Command   =>  "/items";
        public override string Description  =>  "Shows your items.";
        public override bool StopProcess => true;
        
        public ItemsCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        public override async Task<bool> OnCommand(ISession session,string cmd, Action<string> Callback)
        {
            if(cmd.ToLower() == Command)
            {
                var answerTextmessage = "";
                var inventory = session.Inventory;
                answerTextmessage += session.Translation.GetTranslation(TranslationString.CurrentPokeballInv,
                    await inventory.GetItemAmountByType(ItemId.ItemPokeBall),
                    await inventory.GetItemAmountByType(ItemId.ItemGreatBall),
                    await inventory.GetItemAmountByType(ItemId.ItemUltraBall),
                    await inventory.GetItemAmountByType(ItemId.ItemMasterBall));
                answerTextmessage += "\n";
                answerTextmessage += session.Translation.GetTranslation(TranslationString.CurrentPotionInv,
                    await inventory.GetItemAmountByType(ItemId.ItemPotion),
                    await inventory.GetItemAmountByType(ItemId.ItemSuperPotion),
                    await inventory.GetItemAmountByType(ItemId.ItemHyperPotion),
                    await inventory.GetItemAmountByType(ItemId.ItemMaxPotion));
                answerTextmessage += "\n";
                answerTextmessage += session.Translation.GetTranslation(TranslationString.CurrentReviveInv,
                    await inventory.GetItemAmountByType(ItemId.ItemRevive),
                    await inventory.GetItemAmountByType(ItemId.ItemMaxRevive));
                answerTextmessage += "\n";
                answerTextmessage += session.Translation.GetTranslation(TranslationString.CurrentMiscItemInv,
                    await session.Inventory.GetItemAmountByType(ItemId.ItemRazzBerry) +
                    await session.Inventory.GetItemAmountByType(ItemId.ItemBlukBerry) +
                    await session.Inventory.GetItemAmountByType(ItemId.ItemNanabBerry) +
                    await session.Inventory.GetItemAmountByType(ItemId.ItemWeparBerry) +
                    await session.Inventory.GetItemAmountByType(ItemId.ItemPinapBerry),
                    await session.Inventory.GetItemAmountByType(ItemId.ItemIncenseOrdinary) +
                    await session.Inventory.GetItemAmountByType(ItemId.ItemIncenseSpicy) +
                    await session.Inventory.GetItemAmountByType(ItemId.ItemIncenseCool) +
                    await session.Inventory.GetItemAmountByType(ItemId.ItemIncenseFloral),
                    await session.Inventory.GetItemAmountByType(ItemId.ItemLuckyEgg),
                    await session.Inventory.GetItemAmountByType(ItemId.ItemTroyDisk));
                Callback(answerTextmessage);
                return true;

            }
            return false;
        }
    }
}
