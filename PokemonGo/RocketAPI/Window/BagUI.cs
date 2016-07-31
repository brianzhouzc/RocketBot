using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Net;
using System.IO;

namespace PokemonGo.RocketAPI.Window
{
    public partial class BagUI : Form
    {
        private static Client client;
        public BagUI()
        {
            InitializeComponent();
            ClientSettings = Settings.Instance;
        }

        private void PokeUi_Load(object sender, EventArgs e)
        {
            Execute();
        }

        public static ISettings ClientSettings;

        private async void Execute()
        {
          

            client = new Client(ClientSettings);

            try
            {
                switch (ClientSettings.AuthType)
                {
                    case AuthType.Ptc:
                        await client.DoPtcLogin(ClientSettings.PtcUsername, ClientSettings.PtcPassword);
                        break;
                    case AuthType.Google:
                        await client.DoGoogleLogin(ClientSettings.Email, ClientSettings.Password);
                        break;
                }
                //
                await client.SetServer();
                var inventory = await client.GetInventory();
               




                

                


                
                PopulateItemList();
                


            }
            catch (TaskCanceledException) { Execute(); }
            catch (UriFormatException) { Execute(); }
            catch (ArgumentOutOfRangeException) { Execute(); }
            catch (ArgumentNullException) { Execute(); }
            catch (NullReferenceException) { Execute(); }
            catch (Exception ex) { Execute(); }
        }
        private async void PopulateItemList()
        {
            listView2.Items.Clear();
            var inventory = await client.GetInventory();
            var items = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Item)
                       .Where(p => p != null && p?.Count > 1).OrderByDescending(key => key.Item_);
            int counter = 0;
            foreach (Item item in items)
            {
                ListViewItem lvi = new ListViewItem(Convert.ToString((AllEnum.ItemId)item.Item_));
                lvi.Tag = item;
                listView2.Items.Add(lvi);
                counter += item.Count;
                lvi.SubItems.Add(item.Count.ToString());
            }
            listView2.Columns[0].Text = "Total Item's : " + counter.ToString() ;
        }
      

      

        private void button1_Click(object sender, EventArgs e)
        {
            this.listView2.Items.Clear();
            
            Execute();
        }

       

        private async void discard10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = (Item)listView2.SelectedItems[0].Tag;

            var transfer = await client.RecycleItem((AllEnum.ItemId)item.Item_, 10);
            PopulateItemList();
        }

        private async void discardAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = (Item)listView2.SelectedItems[0].Tag;
            var transfer = await client.RecycleItem((AllEnum.ItemId)item.Item_, item.Count);
            PopulateItemList();
        }



        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView2.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    var item = (Item)listView2.SelectedItems[0].Tag;
                    if (item.Count <= 10)
                        discard10ToolStripMenuItem.Enabled = false;
                    else
                        discard10ToolStripMenuItem.Enabled = true;
                    contextMenuStrip2.Show(Cursor.Position);
                }
            }
        }
    }
}