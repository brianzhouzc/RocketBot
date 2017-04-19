using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Data;
using POGOProtos.Inventory;
using RocketBot2.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketBot2.Forms
{
    

    public partial class EggsForm : System.Windows.Forms.Form
    {
        private ISession session;
        public EggsForm() { InitializeComponent(); }

        public EggsForm(Session session)
        {
            InitializeComponent();
            this.session = session;
            var inventory = session.Inventory.GetCachedInventory().Result;
            var eggsListViewModel = new EggsListViewModel();
            eggsListViewModel.InventoryRefreshed(inventory);
            AddControls(eggsListViewModel);
        }

        public void AddControls(EggsListViewModel eggsListViewModel)
        {
            foreach ( var item in eggsListViewModel.Eggs)
            {
                var pic = new ItemBox(item);
                flpEggs.Controls.Add(pic);
            }

            foreach (var item in eggsListViewModel.Incubators)
            {
                var pic = new ItemBox(item);
                flpEggs.Controls.Add(pic);
            }
        }
    }

    public  class EggViewModel
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
        public EggViewModel() { }
        public EggViewModel(PokemonData egg)
        {
            Id = egg.Id;
             TotalKM = egg.EggKmWalkedTarget;
            KM = egg.EggKmWalkedStart;
            this.egg = egg;
        }
        public void UpdateWith(EggViewModel e)
        {
            KM = e.KM;
        }
    }

    //--------------------------------------------------

    public  class EggsListViewModel
    {
        public ObservableCollection<EggViewModel> Eggs { get; set; }
        public ObservableCollection<IncubatorViewModel> Incubators { get; set; }
        public EggsListViewModel()
        {
            Eggs = new ObservableCollection<EggViewModel>();
            Incubators = new ObservableCollection<IncubatorViewModel>();
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
            var incuModel = new IncubatorViewModel(incu);
            var existing = Incubators.FirstOrDefault(x => x.Id == incu.Id);
            if (existing != null)
            {
                existing.UpdateWith(incuModel);
            }
            else
            {
                Incubators.Add(incuModel);
            }
        }

        public void AddOrUpdate(PokemonData egg, IncubatorViewModel incu = null)
        {
            var eggModel = new EggViewModel(egg)
            {
                Hatchable = incu == null
            };
            var existing = Eggs.FirstOrDefault(x => x.Id == eggModel.Id);
            if (existing != null)
            {
                // Do not update, it overwrites OnEggIncubatorStatus Status updates
                // existing.UpdateWith(eggModel);
            }
            else
            {
                Eggs.Add(eggModel);
            }
        }

        public void OnEggIncubatorStatus(EggIncubatorStatusEvent e)
        {
            var egg = Eggs.FirstOrDefault(t => t.Id == e.PokemonId);
            var incu = Incubators.FirstOrDefault(t => t.Id == e.IncubatorId);

            if (egg == null) return;

            egg.Hatchable = false;
            incu.InUse = true;
            egg.KM = e.KmWalked;
        }
    }

    //--------------------------------------------------

    public class IncubatorViewModel
    {

        public IncubatorViewModel(EggIncubator incu)
        {

            Id = incu.Id;
            InUse = incu.PokemonId > 0;
            KM = incu.StartKmWalked;
            TotalKM = incu.TargetKmWalked;
            PokemonId = incu.PokemonId;
            UsesRemaining = incu.UsesRemaining;
            IsUnlimited = incu.ItemId == POGOProtos.Inventory.Item.ItemId.ItemIncubatorBasicUnlimited;
        }
        public IncubatorViewModel() { }
        
        public Image Icon => ResourceHelper.SetImageSize((Image)Resources.EggDB.ResourceManager.GetObject("egg_incubator"), 48, 48);
        public double Availbility => InUse ? 0 : 1;

        public string Id { get; set; }
        public bool InUse { get; set; }
        public bool IsUnlimited { get; set; }
        public ulong PokemonId { get; set; }
        public int UsesRemaining { get; set; }
        public double KM { get; set; }
        public double TotalKM { get; set; }
        public int Count { get; set; }

        public void UpdateWith(IncubatorViewModel incuModel)
        {
            InUse = incuModel.PokemonId > 0;
            PokemonId = incuModel.PokemonId;
            UsesRemaining = incuModel.UsesRemaining;
            KM = incuModel.KM;
            TotalKM = incuModel.TotalKM;
        }
    }
}
