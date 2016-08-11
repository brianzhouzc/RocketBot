using POGOProtos.Inventory.Item;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window {
    public partial class ItemForm : Form {
        public ItemForm(ItemData item) {
            InitializeComponent();

            pb.Image = (Image)Properties.Resources.ResourceManager.GetObject(item.ItemId.ToString());
            numCount.Maximum = item.Count;

            if (item.ItemId == ItemId.ItemLuckyEgg || item.ItemId == ItemId.ItemIncenseOrdinary) {
                btnRecycle.Text = "Use";
                //btnRecycle.Enabled = false;
                numCount.Visible = false;
            }
        }
    }
}
