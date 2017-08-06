using System;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;
using RocketBot2.Helpers;
using PoGo.NecroBot.Logic.Tasks;
using PoGo.NecroBot.Logic.State;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;
using POGOProtos.Data;
using PoGo.NecroBot.Logic.Logging;

namespace RocketBot2.Forms
{
    public partial class ItemBox : UserControl
    {
        public DateTime expires = new DateTime(0);
        private ItemData Item_ { get; }
        private bool DisableTimer = false;
        private static ISession Session;
        private static Incubators _incubator;
        private Control Box;
 
        public ItemBox() { }

        public ItemBox(ISession session, ItemData item, PokemonData pokemonData, Image picture)
        {
            InitializeComponent();
            Session = session;
            DisableTimer = true;
            lbl.Font = new System.Drawing.Font("Segoe UI", 6.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTime.Font = new System.Drawing.Font("Segoe UI", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            pb.Image = picture;
            lblTime.Text = $"{Session.Translation.GetPokemonTranslation(pokemonData.PokemonId)}";
            lblTime.Visible = true;
            var PokemonSettings = Session.Inventory.GetPokemonSettings().Result.FirstOrDefault(x => x.PokemonId == pokemonData.PokemonId);
 
            switch (item.ItemId)
            {
                case ItemId.ItemRareCandy:
                    lbl.Text = $"{PokemonSettings.FamilyId.ToString().Replace("Family", null).Replace("Male","(M)").Replace("Female","(F)")}\n\rCandy: {Session.Inventory.GetCandyCount(pokemonData.PokemonId).Result}";
                    break;
                case ItemId.ItemMoveRerollFastAttack:
                    lbl.Text = $"{Session.Translation.GetPokemonMovesetTranslation(pokemonData.Move1)}\n\r{Session.Translation.GetPokemonMovesetTranslation(pokemonData.Move2)}";
                    break;
                case ItemId.ItemMoveRerollSpecialAttack:
                    lbl.Text = $"{Session.Translation.GetPokemonMovesetTranslation(pokemonData.Move1)}\n\r{Session.Translation.GetPokemonMovesetTranslation(pokemonData.Move2)}";
                    break;
                default:
                    lbl.Text = null;
                    break;
            }

            lblTime.Parent = pb;

            foreach (Control control in Controls)
            {
                control.MouseEnter += ChildMouseEnter;
                control.MouseLeave += ChildMouseLeave;
                control.MouseClick += delegate { UseItem(session, item, pokemonData); };
                Box = control;
            }
        }

        private async void UseItem(ISession session, ItemData item, PokemonData pokemon)
        {
            switch (item.ItemId)
            {
                case ItemId.ItemRareCandy:
                    await Task.Run(async () => { await UseRareCandyTask.Execute(session, item, pokemon).ConfigureAwait(false); });
                    break;
                case ItemId.ItemMoveRerollFastAttack:
                    string text = $"{session.Translation.GetPokemonTranslation(pokemon.PokemonId)} will forget move1: {session.Translation.GetPokemonMovesetTranslation(pokemon.Move1)}\n\rDo you want to continue?";
                    DialogResult result = MessageBox.Show(text, $"Use {item.ItemId}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    switch (result)
                    {
                        case DialogResult.Yes:
                            await Task.Run(async () => { await UseItemMoveRerollTask.Execute(session, item, pokemon).ConfigureAwait(false); });
                            break;
                    }
                    break;
                case ItemId.ItemMoveRerollSpecialAttack:
                    string text2 = $"{session.Translation.GetPokemonTranslation(pokemon.PokemonId)} will forget move2: {session.Translation.GetPokemonMovesetTranslation(pokemon.Move2)}\n\rDo you want to continue?";
                    DialogResult result2 = MessageBox.Show(text2, $"Use {item.ItemId}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    switch (result2)
                    {
                        case DialogResult.Yes:
                            await Task.Run(async () => { await UseItemMoveRerollTask.Execute(session, item, pokemon).ConfigureAwait(false); });
                            break;
                    }
                    break;
                default:
                    Logger.Write($"{item.ItemId} Can not be used for the moment, the bot still does not completely generate this process.", LogLevel.Warning);
                    break;
            }
            PokeDexForm.ActiveForm.Close();
        }

        public ItemBox(int see, int cath, Image pic, string name)
        {
            InitializeComponent();
            DisableTimer = true;
            lbl.Font = new System.Drawing.Font("Segoe UI", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTime.Font = new System.Drawing.Font("Segoe UI", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            pb.Image = pic;
            lblTime.Text = $"{name}";
            lblTime.Visible = true;
            lbl.Text = $"Seen: {see}\n\rCatch: {cath}";
            lblTime.Parent = pb;
        }

        public ItemBox(Eggs egg, ISession session)
        {
            InitializeComponent();
            Session = session;
            DisableTimer = true;
            lbl.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            pb.Image = egg.Icon;
            lblTime.Parent = pb;
            lblTime.Visible = true;
            lbl.Text = String.Format(CultureInfo.InvariantCulture, "{0:0.0}Km", egg.KM);
            if (!egg.Hatchable)
            {
                lblTime.Text = String.Format(CultureInfo.InvariantCulture, "Walked", egg.TotalKM);
                Enabled = false;
                return;
            }
            lblTime.Text = String.Format(CultureInfo.InvariantCulture, "{0:0.0}Km", egg.TotalKM);

            foreach (Control control in Controls)
            {
                control.MouseEnter += ChildMouseEnter;
                control.MouseLeave += ChildMouseLeave;
                control.MouseClick += delegate { HatchEgg_Click(egg); };
            }
        }

        public ItemBox(Incubators item)
        {
            InitializeComponent();
            DisableTimer = true;
            lbl.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            pb.Image = item.Icon(item.IsUnlimited);
            lblTime.Text = String.Format(CultureInfo.InvariantCulture, "{0:0.0}Km", item.TotalKM);
            lblTime.Visible = true;
            lbl.Text = item.InUse ? String.Format(CultureInfo.InvariantCulture,"{0:0.0}Km", item.KM) : "0.0Km";
            lblTime.Parent = pb;

            if (item.InUse)
            {
                Enabled = false;
                return;
            }
            lblTime.Text = "Empty";
            lbl.Text = item.IsUnlimited ? "(∞)" : $"{item.UsesRemaining} times";

            foreach (Control control in Controls)
            {
                control.MouseEnter += ChildMouseEnter;
                control.MouseLeave += ChildMouseLeave;
                control.MouseClick += delegate { SetIncubator_Click(item); } ;
                Box = control;
            }
        }

        private void SetIncubator_Click(Incubators Incubator)
        {
            if (Box.BackColor == Color.LightGreen)
            {
                Box.BackColor = Color.Transparent; //SystemColors.Control;
                _incubator = null;
                return;
            }

            if (Incubator.InUse)
            {
                MessageBox.Show("Incubator in use choice an other", "Incubator Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            _incubator = Incubator;
            Box.BackColor = Color.LightGreen;
        }

        private async void HatchEgg_Click(Eggs egg)
        {
            if (_incubator == null)
            {
                MessageBox.Show("Please select an incubator to hatch eggs", "Hatch Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            await UseIncubatorsTask.Execute(Session, Session.CancellationTokenSource.Token, egg.Id, _incubator.Id).ConfigureAwait(false);
            EggsForm.ActiveForm.Close();
        }

        public ItemBox(ItemData item)
        {
            InitializeComponent();
 
            pb.Image = ResourceHelper.SetImageSize(ResourceHelper.ItemPicture(item), pb.Size.Height, pb.Size.Width);
            lbl.Text = item.Count.ToString();
            lblTime.Parent = pb;

            Item_ = item;

            foreach (Control control in Controls)
            {
                control.MouseEnter += ChildMouseEnter;
                control.MouseLeave += ChildMouseLeave;
                control.MouseClick += ChildMouseClick;
            }

            if (item.Count < 1)
            {
                Enabled = false;
            }
        }

        public event EventHandler ItemClick;

        private void ChildMouseClick(object sender, MouseEventArgs e)
        {
            OnItemClick(Item_, EventArgs.Empty);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            OnItemClick(Item_, EventArgs.Empty);
        }

        private void ChildMouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void ChildMouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            BackColor = Color.Transparent;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            BackColor = Color.LightGreen;
        }

        protected virtual void OnItemClick(ItemData item, EventArgs e)
        {
            ItemClick?.Invoke(item, e);
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            if (DisableTimer) return;
            var time = expires - DateTime.UtcNow;
            if (expires.Ticks == 0 || time.TotalSeconds < 0)
            {
                lblTime.Visible = false;
            }
            else
            {
                Enabled = false;
                lblTime.Visible = true;
                lblTime.Text = $"{time.Minutes}m {Math.Abs(time.Seconds)}s";
            }
        }
    }
}