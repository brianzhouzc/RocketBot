using System;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;

namespace PokemonGo.RocketAPI.Window
{
    public partial class ItemBox : UserControl
    {
        public ItemBox(ItemData item)
        {
            InitializeComponent();

            item_ = item;

            pb.Image = (Image) Properties.Resources.ResourceManager.GetObject(item.ItemId.ToString());
            lbl.Text = item.Count.ToString();

            foreach (Control control in Controls)
            {
                control.MouseEnter += childMouseEnter;
                control.MouseLeave += childMouseLeave;
                control.MouseClick += childMouseClick;
            }

            if (item_.ItemId == ItemId.ItemIncubatorBasic || item_.ItemId == ItemId.ItemIncubatorBasicUnlimited)
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
    }
}