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
using System;
using System.Net;
using System.IO;

namespace PokemonGo.RocketAPI.Window
{
    public partial class PokeUi : Form
    {
        public PokeUi()
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
            button1.Enabled = false;

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
                //
                await client.SetServer();
                var inventory = await client.GetInventory();
                var pokemons =
                    inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.Pokemon)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .OrderByDescending(key => key.Cp);
                var families = inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.PokemonFamily)
                    .Where(p => p != null && (int) p?.FamilyId > 0)
                    .OrderByDescending(p => (int)p.FamilyId);
                    
                    
                
                
                var imageList = new ImageList { ImageSize = new Size(50, 50) };
                listView1.ShowItemToolTips = true;
                

                foreach (var pokemon in pokemons)
                {

                    var pokemonImage = GetPokemonImage((int)pokemon.PokemonId);
                    imageList.Images.Add(pokemon.PokemonId.ToString(),pokemonImage);

                    listView1.LargeImageList = imageList;
                    var listViewItem = new ListViewItem();
                    listViewItem.SubItems.Add("Cp: " + pokemon.Cp);


                    var currentCandy = families
                        .Where(i => (int) i.FamilyId <= (int) pokemon.PokemonId)
                        .Select(f=>f.Candy)
                        .First();
                    var currIv = Math.Round(Perfect(pokemon));
                    //listViewItem.SubItems.Add();
                    listViewItem.ImageKey = pokemon.PokemonId.ToString();
                    listViewItem.Text = string.Format("{0}\n{1} CP", pokemon.PokemonId, pokemon.Cp);
                    listViewItem.ToolTipText = currentCandy+" Candy\n"+currIv+"% IV";


                    this.listView1.Items.Add(listViewItem);


                }
                button1.Enabled = true;

            }
            catch (TaskCanceledException){ Execute(); }
            catch (UriFormatException) { Execute(); }
            catch (ArgumentOutOfRangeException) {  Execute(); }
            catch (ArgumentNullException) { Execute(); }
            catch (NullReferenceException) { Execute(); }
            catch (Exception ex) {  Execute(); }
        }
        private static Bitmap GetPokemonImage(int pokemonId)
        {
            var Sprites = AppDomain.CurrentDomain.BaseDirectory + "Sprites\\";
            string location = Sprites + pokemonId + ".png";
            if (!Directory.Exists(Sprites))
                Directory.CreateDirectory(Sprites);
            if (!File.Exists(location))
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("http://pokeapi.co/media/sprites/pokemon/" + pokemonId + ".png", @location);
            }
            PictureBox picbox = new PictureBox();
            picbox.Image = Image.FromFile(location);
            Bitmap bitmapRemote = (Bitmap)picbox.Image;
            return bitmapRemote;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.listView1.Clear();
            Execute();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (listView1.SelectedItems.Count == 0) return;
            //label1.Text = listView1.SelectedItems[0]?.SubItems[1].Text;
            //label2.Text = listView1.SelectedItems[0]?.SubItems[2].Text;
        }
        public static float Perfect(PokemonData poke)
        {
            return ((float)(poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f)) * 100.0f;
        }
    }
}
