using System;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;
using System.Collections.Generic;
using POGOProtos.Inventory;

namespace PokemonGo.RocketAPI.Window
{
    public partial class ItemBox : UserControl
    {
        public DateTime expires = new DateTime(0);

        public ItemBox(ItemData item)
        {
            InitializeComponent();

            item_ = item;

            pb.Image = (Image) Properties.Resources.ResourceManager.GetObject(item.ItemId.ToString());
            lbl.Text = item.Count.ToString();
            lblTime.Parent = pb;

            foreach (Control control in Controls)
            {
                control.MouseEnter += childMouseEnter;
                control.MouseLeave += childMouseLeave;
                control.MouseClick += childMouseClick;
            }

            if (item_.ItemId == ItemId.ItemIncubatorBasic || item_.ItemId == ItemId.ItemIncubatorBasicUnlimited || item.Count < 1)
            {
                Enabled = false;
            }
        }

        public ItemData item_ { get; }

        public event EventHandler ItemClick;

        private void childMouseClick(object sender, MouseEventArgs e)
        {
            OnItemClick(item_, EventArgs.Empty);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            OnItemClick(item_, EventArgs.Empty);
        }

        private void childMouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void childMouseEnter(object sender, EventArgs e)
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
            var handler = ItemClick;
            if (handler != null)
            {
                handler(item, e);
            }
        }

        private void tmr_Tick(object sender, EventArgs e) {
            TimeSpan time = expires - DateTime.UtcNow;
            if (expires.Ticks == 0 || time.TotalSeconds < 0) {
                lblTime.Visible = false;
            } else {
                lblTime.Visible = true;
                lblTime.Text = $"{time.Minutes}m {Math.Abs(time.Seconds)}s";
            }
        }
    }
}