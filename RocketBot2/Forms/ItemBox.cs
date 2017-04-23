using System;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;
using RocketBot2.Helpers;
using PoGo.NecroBot.Logic.Tasks;
using PoGo.NecroBot.Logic.State;

namespace RocketBot2.Forms
{
    public partial class ItemBox : UserControl
    {
        public DateTime expires = new DateTime(0);
        public ItemData Item_ { get; }
        public bool DisableTimer = false;
        public static ISession Session;
        public Incubators incubator;

        public ItemBox(int see, int cath, Image pic)
        {
            InitializeComponent();
            DisableTimer = true;
            lbl.Font = new System.Drawing.Font("Segoe UI", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblTime.Font = new System.Drawing.Font("Segoe UI", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            pb.Image = pic;
            lblTime.Text = $"See: {see}";
            lblTime.Visible = true;
            lbl.Text = $"Catch: {cath}";
            lblTime.Parent = pb;
        }

        public ItemBox(Eggs item, ISession session)
        {
            InitializeComponent();
            Session = session;
            DisableTimer = true;
            lbl.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            pb.Image = item.Icon;
            lblTime.Text = $"{item.TotalKM:0.0}Km";
            lblTime.Visible = true;
            lbl.Text = $"{item.KM:0.0}Km";
            lblTime.Parent = pb;
            
            foreach (Control control in Controls)
            {
                control.MouseEnter += ChildMouseEnter;
                control.MouseLeave += ChildMouseLeave;
                control.MouseClick += HatchEgg_Click;
            }
        }

        public ItemBox(Incubators item)
        {
            InitializeComponent();
            DisableTimer = true;
            lbl.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            pb.Image = item.Icon(item.IsUnlimited);
            lblTime.Text = $"{item.TotalKM - item.KM:0.0}Km";
            lblTime.Visible = true;
            lbl.Text = $"{item.TotalKM / 1000:0.0}Km";
            lblTime.Parent = pb;
 
            foreach (Control control in Controls)
            {
                control.MouseEnter += ChildMouseEnter;
                control.MouseLeave += ChildMouseLeave;
                //TODO: Review click
                //control.MouseClick += SetIncubator_Click;
            }
        }

        private void SetIncubator_Click(object sender, MouseEventArgs e)
        {
            var incu = ((Incubators)sender);
            if (incu.InUse)
            {
                MessageBox.Show("Incubator in use choice an other", "Hatch Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            ((ItemBox)sender).BackColor = Color.LightGreen;
            incubator = incu;
        }

        private async void HatchEgg_Click(object sender, MouseEventArgs e)
        {
            if (incubator == null)
            {
                MessageBox.Show("Please select an incubator to hatch eggs", "Hatch Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var eggs = ((Eggs)sender);
            await UseIncubatorsTask.Execute(Session, Session.CancellationTokenSource.Token, eggs.Id, incubator.Id);
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