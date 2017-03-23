using System;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;
using RocketBot2.Helpers;

namespace RocketBot2.Forms
{
    public partial class ItemBox : UserControl
    {
        public DateTime expires = new DateTime(0);
        public ItemData item_ { get; }


        public ItemBox(ItemData item)
        {
            InitializeComponent();

            pb.Image = ResourceHelper.ItemPicture(item);
            lbl.Text = item.Count.ToString();
            lblTime.Parent = pb;

            item_ = item;

            foreach (Control control in Controls)
            {
                control.MouseEnter += childMouseEnter;
                control.MouseLeave += childMouseLeave;
                control.MouseClick += childMouseClick;
            }

            if (item.ItemId == ItemId.ItemIncubatorBasic 
                || item.ItemId == ItemId.ItemIncubatorBasicUnlimited 
              /*|| item.ItemId == ItemId.ItemDragonScale
                || item.ItemId == ItemId.ItemKingsRock
                || item.ItemId == ItemId.ItemMetalCoat
                || item.ItemId == ItemId.ItemSunStone
                || item.ItemId == ItemId.ItemUpGrade*/
                || item.Count < 1)
            {
                Enabled = false;
            }
        }

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

        private void tmr_Tick(object sender, EventArgs e)
        {
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