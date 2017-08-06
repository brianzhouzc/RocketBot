using PoGo.NecroBot.Logic.State;
using POGOProtos.Data;
using POGOProtos.Inventory;
using RocketBot2.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace RocketBot2.Forms
{
    public partial class EggsForm : System.Windows.Forms.Form
    {
        public static ISession Session;
        
        public EggsForm() { InitializeComponent(); }

        public EggsForm(Session session)
        {
            InitializeComponent();
            Session = session;
            var inventory = Session.Inventory.GetCachedInventory().Result;
            var eggsListViewModel = new ItemsList();
            eggsListViewModel.InventoryRefreshed(inventory);
            AddControls(eggsListViewModel);
        }

        public void AddControls(ItemsList eggsListViewModel)
        {
            foreach ( var item in eggsListViewModel.Eggs)
            {
                var pic = new ItemBox(item, Session);
                flpEggs.Controls.Add(pic);
            }

            foreach (var item in eggsListViewModel.Incubators)
            {
                var pic = new ItemBox(item);
                flpEggs.Controls.Add(pic);
            }
        }
    }

    public  class Eggs
    {
        Dictionary<double, Image> icons = new Dictionary<double, Image>()
        {
            {2.00, ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_2"), 48, 48)},
            {5.00, ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_5"), 48, 48)},
            {10.00, ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_10"), 48, 48)}
        };

        private PokemonData egg;

        public ulong Id { get; set; }
        public double TotalKM { get; set; }
        public double KM { get; set; }

        public bool Hatchable { get; set; }
        public Image Icon => icons[TotalKM];

        public Eggs(PokemonData egg)
        {
            Id = egg.Id;
            TotalKM = egg.EggKmWalkedTarget;
            KM = egg.EggKmWalkedStart;
            this.egg = egg;
        }
    }

    public  class ItemsList
    {
        public ObservableCollection<Eggs> Eggs { get; set; }
        public ObservableCollection<Incubators> Incubators { get; set; }
        public ItemsList()
        {
            Eggs = new ObservableCollection<Eggs>();
            Incubators = new ObservableCollection<Incubators>();
        }

        public void InventoryRefreshed(IEnumerable<InventoryItem> inventory)
        {
            var eggs = inventory
                .Select(x => x.InventoryItemData?.PokemonData)
                .Where(x => x != null && x.IsEgg)
                .ToList();

            var incubators = inventory
                    .Where(x => x.InventoryItemData.EggIncubators != null)
                    .SelectMany(i => i.InventoryItemData.EggIncubators.EggIncubator)
                    .Where(i => i != null);

            foreach (var incu in incubators)
            {
                AddOrUpdateIncubator(incu);
            }

            foreach (var egg in eggs)
            {
                var incu = Incubators.FirstOrDefault(x => x.PokemonId == egg.Id);
                AddOrUpdate(egg, incu);
            }
        }

        public void AddOrUpdateIncubator(EggIncubator incu)
        {
            var incuModel = new Incubators(incu);
            Incubators.Add(incuModel);
        }

        public void AddOrUpdate(PokemonData egg, Incubators incu = null)
        {
            var eggModel = new Eggs(egg)
            {
                Hatchable = incu == null,
            };
            if (!eggModel.Hatchable && incu != null)
                eggModel.KM = incu.KM;
                Eggs.Add(eggModel);
        }
    }

    public class Incubators
    {
        Dictionary<double, Image> icons = new Dictionary<double, Image>()
        {
            {2.00, ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_2_incubator"), 48, 48)},
            {5.00, ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_5_incubator"), 48, 48)},
            {10.00, ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_10_incubator"), 48, 48)}
        };

        Dictionary<double, Image> iconsUnlimited = new Dictionary<double, Image>()
        {
            {2.00, ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_2_incubator_unlimited"), 48, 48)},
            {5.00, ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_5_incubator_unlimited"), 48, 48)},
            {10.00, ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_10_incubator_unlimited"), 48, 48)}
        };

        public static float kmWalked;

        public async Task GetPlayerStats()
        {
            var playerStats = (await EggsForm.Session.Inventory.GetPlayerStats().ConfigureAwait(false)).FirstOrDefault();
            kmWalked = playerStats.KmWalked;
        }

        public Incubators(EggIncubator incu)
        {
            GetPlayerStats().ConfigureAwait(false);
            Id = incu.Id;
            InUse = incu.PokemonId > 0;
            KM =  kmWalked - incu.StartKmWalked;
            TotalKM = incu.TargetKmWalked - incu.StartKmWalked;
            PokemonId = incu.PokemonId;
            UsesRemaining = incu.UsesRemaining;
            IsUnlimited = incu.ItemId == POGOProtos.Inventory.Item.ItemId.ItemIncubatorBasicUnlimited;
        }

        public Image Icon (bool unlimited)
        {
            try
            {
                if (unlimited) return iconsUnlimited[TotalKM];
                return icons[TotalKM];
            }
            catch
            {
                if (unlimited) return ResourceHelper.SetImageSize((Image)Resources.ItemIdDB.ResourceManager.GetObject("_901"), 48, 48);
                return ResourceHelper.SetImageSize((Image)Resources.ItemIdDB.ResourceManager.GetObject("_902"), 48, 48);
            }
        }

        public double Availbility => InUse ? 0 : 1;

        public string Id { get; set; }
        public bool InUse { get; set; }
        public bool IsUnlimited { get; set; }
        public ulong PokemonId { get; set; }
        public int UsesRemaining { get; set; }
        public double KM { get; set; }
        public double TotalKM { get; set; }
    }
}
