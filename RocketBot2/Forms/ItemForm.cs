using System.Windows.Forms;
using POGOProtos.Inventory.Item;
using RocketBot2.Helpers;

namespace RocketBot2.Forms
{
    public partial class ItemForm : Form
    {
        public ItemForm(ItemData item)
        {
            InitializeComponent();

            pb.Image = ResourceHelper.GetImageSize(ResourceHelper.ItemPicture(item), pb.Size.Height, pb.Size.Width);
            numCount.Maximum = item.Count;

            if (item.ItemId == ItemId.ItemLuckyEgg || item.ItemId == ItemId.ItemIncenseOrdinary)
            {
                btnRecycle.Text = "Use";
                //btnRecycle.Enabled = false;
                numCount.Visible = false;
            }
        }
    }
}