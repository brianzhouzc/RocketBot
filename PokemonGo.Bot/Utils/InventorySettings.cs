namespace PokemonGo.Bot.Utils
{
    public class InventorySettings
    {
        public InventorySettings()
        {
        }

        public InventorySettings(POGOProtos.Settings.InventorySettings inventorySettings)
        {
            BaseBagItems = inventorySettings.BaseBagItems;
            BaseEggs = inventorySettings.BaseEggs;
            BasePokemon = inventorySettings.BasePokemon;
            MaxBagItems = inventorySettings.MaxBagItems;
            MaxPokemon = inventorySettings.MaxPokemon;
        }

        public int BaseBagItems { get; }
        public int BaseEggs { get; }
        public int BasePokemon { get; }
        public int MaxBagItems { get; }
        public int MaxPokemon { get; }
    }
}