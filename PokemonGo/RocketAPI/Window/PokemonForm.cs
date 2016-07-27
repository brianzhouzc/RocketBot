using PokemonGo.RocketAPI.Enums;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window
{
    public partial class PokemonForm : Form
    {
        public PokemonForm()
        {
            InitializeComponent();
            ClientSettings = Settings.Instance;
        }

        private Locale localePokemonName = null;

        private void PokemonForm_Load(object sender, EventArgs e)
        {
            string locale = ClientSettings.Language;
            using (StreamReader sr = new StreamReader(string.Format("locales/pokemon.{0}.json", locale), Encoding.GetEncoding("UTF-8")))
            {
                localePokemonName = new Locale(Newtonsoft.Json.JsonConvert.DeserializeObject<LocaleData[]>(sr.ReadToEnd()));
            }

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
                    string pokemonName = Convert.ToString(pokemon.PokemonId);
                    var pokeName = localePokemonName.Get(((int)pokemon.PokemonId).ToString());
                    if (string.IsNullOrEmpty(pokeName) == false)
                    {
                        pokemonName = pokeName;
                    }

                    ListViewItem lvi = new ListViewItem(pokemonName);
                    listView1.Items.Add(lvi);
                    lvi.SubItems.Add(pokemon.Cp.ToString());
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

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //listView1.Sorting = listView1.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
