using System;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;
using RocketBot2.Helpers;
using System.Globalization;

namespace RocketBot2.Forms
{
    public partial class ItemBox : UserControl
    {
        public DateTime expires = new DateTime(0);
        public ItemData Item_ { get; }
        public bool isEggs = false;

        public ItemBox(EggViewModel item)
        {
            InitializeComponent();
            isEggs = true;
            //item.UpdateWith(item);
            pb.Image = item.Icon;
            lbl.Text = $"{item.TotalKM}Km";
            lblTime.Visible = true;
            lblTime.Text = $"{item.KM / 1000:0.0}Km";
            lblTime.Parent = pb;
            /*
            Item_ = item;

            foreach (Control control in Controls)
            {
                control.MouseEnter += ChildMouseEnter;
                control.MouseLeave += ChildMouseLeave;
                control.MouseClick += ChildMouseClick;
            }*/
        }

        public ItemBox(IncubatorViewModel item)
        {
            InitializeComponent();
            isEggs = true;
            //item.UpdateWith(item);
            pb.Image = item.Icon(item.IsUnlimited);
            lbl.Text = $"{item.TotalKM - item.KM:0}Km";
            lblTime.Visible = true;
            lblTime.Text = $"{item.KM / 1000:0.00}Km";
            lblTime.Parent = pb;
            /*
            Item_ = item;

            foreach (Control control in Controls)
            {
                control.MouseEnter += ChildMouseEnter;
                control.MouseLeave += ChildMouseLeave;
                control.MouseClick += ChildMouseClick;
            }*/
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
            if (isEggs) return;
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