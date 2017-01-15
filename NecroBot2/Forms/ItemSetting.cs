using System;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;

namespace NecroBot2
{
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
