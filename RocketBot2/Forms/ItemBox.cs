using System;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;
using RocketBot2.Helpers;
using PoGo.NecroBot.Logic.Tasks;
using PoGo.NecroBot.Logic.State;
using System.Globalization;
using POGOProtos.Inventory;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using System.Linq;

namespace RocketBot2.Forms
{
    public partial class ItemBox : UserControl
    {
        public DateTime expires = new DateTime(0);
        public ItemData Item_ { get; }
        public bool DisableTimer = false;
        public static ISession Session;
        public static Incubators _incubator;
        public Control Box;

        public ItemBox() { }

        public ItemBox(int see, int cath, Image pic)
        {
            InitializeComponent();
            DisableTimer = true;
            lbl.Font = new System.Drawing.Font("Segoe UI", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTime.Font = new System.Drawing.Font("Segoe UI", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            pb.Image = pic;
            lblTime.Text = $"Seen: {see}";
            lblTime.Visible = true;
            lbl.Text = $"Catch: {cath}";
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
            await UseIncubatorsTask.Execute(Session, Session.CancellationTokenSource.Token, egg.Id, _incubator.Id);
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