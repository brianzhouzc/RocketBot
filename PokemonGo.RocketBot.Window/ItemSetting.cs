using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;

namespace PokemonGo.RocketAPI.Window {
    public partial class ItemSetting : UserControl {

        private ItemData itemData_;

        public ItemSetting(ItemData itemData) {
            InitializeComponent();
            ItemData = itemData;

            count.ValueChanged += Count_ValueChanged;
        }

        private void Count_ValueChanged(object sender, EventArgs e) {
            ItemData.Count = Decimal.ToInt32(count.Value);
        }

        public ItemData ItemData 
        {
            get 
            {
                return itemData_;
            }
            set 
            {
                itemData_ = value;
                ItemId.Text = itemData_.ItemId.ToString().Remove(0, 4);
                count.Value = itemData_.Count;
            }
        }
    }
}
