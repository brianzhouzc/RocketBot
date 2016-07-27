using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Enums;

namespace PokemonGo.RocketAPI.Window
{
    public partial class PokemonForm : Form
    {
        public PokemonForm()
        {
            InitializeComponent();
            ClientSettings = Settings.Instance;
        }

        private void PokemonForm_Load(object sender, EventArgs e)
        {
            Execute();
        }

        public static ISettings ClientSettings;

        private async void Execute()
        {
            var client = new Client(ClientSettings);

            try
            {
                switch (ClientSettings.AuthType)
                {
                    case AuthType.Ptc:
                        await client.DoPtcLogin(ClientSettings.PtcUsername, ClientSettings.PtcPassword);
                        break;
                    case AuthType.Google:
                        await client.DoGoogleLogin();
                        break;
                }

                await client.SetServer();
                var inventory = await client.GetInventory();
                var pokemons =
                    inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                        .Where(p => p != null && p?.PokemonId > 0).OrderByDescending(key => key.Cp);
                

                foreach (var pokemon in pokemons)
                {
                    ListViewItem lvi = new ListViewItem(pokemon.PokemonId.ToString());
                    listView1.Items.Add(lvi);
                    lvi.SubItems.Add(pokemon.Cp.ToString());
                    float iv = PerfectHelper.Perfect(pokemon);
                    lvi.SubItems.Add(Math.Round(iv, MidpointRounding.AwayFromZero).ToString());
                }
            }
            catch (TaskCanceledException) { ColoredConsoleWrite(Color.Red, "Task Canceled Exception - Restarting"); Execute(); }
            catch (UriFormatException) { ColoredConsoleWrite(Color.Red, "System URI Format Exception - Restarting"); Execute(); }
            catch (ArgumentOutOfRangeException) { ColoredConsoleWrite(Color.Red, "ArgumentOutOfRangeException - Restarting"); Execute(); }
            catch (ArgumentNullException) { ColoredConsoleWrite(Color.Red, "Argument Null Refference - Restarting"); Execute(); }
            catch (NullReferenceException) { ColoredConsoleWrite(Color.Red, "Null Refference - Restarting"); Execute(); }
            catch (Exception ex) { ColoredConsoleWrite(Color.Red, ex.ToString()); Execute(); }
        }

        private void ColoredConsoleWrite(Color red, string taskCanceledExceptionRestarting)
        {
            //throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            Execute();
        }

        private ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                lvwColumnSorter.Order = lvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Descending;
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();

        }
    }
}
