using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;

namespace PokemonGo.RocketBot.Window.Forms
{
    public partial class ItemForm : Form
    {
        public ItemForm(ItemData item)
        {
            InitializeComponent();

            pb.Image = (Image) Properties.Resources.ResourceManager.GetObject(item.ItemId.ToString());
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