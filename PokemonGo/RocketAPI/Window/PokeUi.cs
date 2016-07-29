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
    public partial class PokeUi : Form
    {
        private static Client client;
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
            EnabledButton(false);

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
                await client.SetServer();
                var profile = await client.GetProfile();
                var inventory = await client.GetInventory();
                var pokemons =
                    inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.Pokemon)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .OrderByDescending(key => key.Cp);
                var families = inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.PokemonFamily)
                    .Where(p => p != null && (int)p?.FamilyId > 0)
                    .OrderByDescending(p => (int)p.FamilyId);

            
                //listView1.ShowItemToolTips = true;

                //put data into gridview
                this.dataGridView1.AutoGenerateColumns = false;
                this.dataGridView1.Columns.Add("Image", "Image");
                this.dataGridView1.Columns.Add("Name", "Name");
                this.dataGridView1.Columns.Add("CP", "CP");
                this.dataGridView1.Columns.Add("Candy","Candy");
                this.dataGridView1.Columns.Add("IV", "IV");

                foreach (var pokemon in pokemons)
                {
                    Bitmap pokemonImage = null;
                    await Task.Run(() =>
                    {
                        pokemonImage = GetPokemonImage((int)pokemon.PokemonId);
                    });

                    var currentCandy = families
                        .Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId)
                        .Select(f => f.Candy)
                        .First();
                    var currIv = Math.Round(Perfect(pokemon));
                    
           
                    var pokemonId2 = pokemon.PokemonId;
                    var pokemonName = pokemon.Id;

                    var row = new DataGridViewRow();
                    row.Tag = pokemon;
                    row.HeaderCell.Value = row.Index + 1;
                    row.Cells.Add(new DataGridViewImageCell { Value = pokemonImage  });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = pokemon.PokemonId });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = pokemon.Cp });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = currentCandy });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = currIv + "%" });
                    this.dataGridView1.Rows.Add(row);
                }

                this.Text = "PokeUi " + pokemons.Count<PokemonData>() + "/" + profile.Profile.PokeStorage;
                EnabledButton(true);

            }
            catch (TaskCanceledException) { Execute(); }
            catch (UriFormatException) { Execute(); }
            catch (ArgumentOutOfRangeException) { Execute(); }
            catch (ArgumentNullException) { Execute(); }
            catch (NullReferenceException) { Execute(); }
            catch (Exception ex) { Execute(); }
        }

        private void EnabledButton(bool enabled)
        {
            btnReload.Enabled = enabled;
            btnEvolve.Enabled = enabled;
            btnTransfer.Enabled = enabled;
            btnUpgrade.Enabled = enabled;
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

            var imageSize = ClientSettings.ImageSize;

            if ((imageSize > 96) || (imageSize < 1)) // no bigger than orig size and no smaller than 1x1
                imageSize = 50;

            PictureBox picbox = new PictureBox();
            picbox.Image = Image.FromFile(location);
            Bitmap bitmapRemote = new Bitmap(picbox.Image, imageSize, imageSize);
            return bitmapRemote;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Rows.Clear();
            Execute();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public static float Perfect(PokemonData poke)
        {
            return ((float)(poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f)) * 100.0f;
        }

        /*private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.dataGridView1.
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }*/

        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)this.dataGridView1.SelectedRows[0].Tag;


            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(Perfect(pokemon)) + "% perfect", "Are you sure you want to transfer?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var transfer = await client.TransferPokemon(pokemon.Id);
            }
            this.dataGridView1.Rows.Remove(this.dataGridView1.SelectedRows[0]);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            IEnumerable<DataGridViewRow> selectedItems = null;

            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                selectedItems = this.dataGridView1.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.OwningRow).Distinct();
            }
            else
            {
                selectedItems = this.dataGridView1.SelectedRows.Cast<DataGridViewRow>();
            }

            foreach (DataGridViewRow selectedItem in selectedItems)
            {
                await evolvePokemon((PokemonData)selectedItem.Tag);
            }

            this.dataGridView1.DataSource = null;
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Rows.Clear();
            Execute();
        }

        private async void button3_Click(object sender, EventArgs e)
        {

            IEnumerable<DataGridViewRow> selectedItems = null;

            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                 selectedItems = this.dataGridView1.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.OwningRow).Distinct();
            }else
            {
                 selectedItems = this.dataGridView1.SelectedRows.Cast<DataGridViewRow>();
            }

            foreach (DataGridViewRow selectedItem in selectedItems)
            {
                await transferPokemon((PokemonData)selectedItem.Tag);
            }

            this.dataGridView1.DataSource = null;
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Rows.Clear();
            Execute();
        }

        private static async Task evolvePokemon(PokemonData pokemon)
        {
            try
            {
                var evolvePokemonResponse = await client.EvolvePokemon(pokemon.Id);
                string message = "";
                string caption = "";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                if (evolvePokemonResponse.Result == 1)
                {
                    message = $"{pokemon.PokemonId} successfully evolved into {evolvePokemonResponse.EvolvedPokemon.PokemonType}\n{evolvePokemonResponse.ExpAwarded} experience awarded\n{evolvePokemonResponse.CandyAwarded} candy awarded";
                    caption = $"{pokemon.PokemonId} evolved into {evolvePokemonResponse.EvolvedPokemon.PokemonType}";
                }
                else
                {
                    message = $"{pokemon.PokemonId} could not be evolved";
                    caption = $"Evolve {pokemon.PokemonId} failed";
                }

                result = MessageBox.Show(message, caption, buttons, MessageBoxIcon.Information);
            }
            catch (TaskCanceledException) { await evolvePokemon(pokemon); }
            catch (UriFormatException) { await evolvePokemon(pokemon); }
            catch (ArgumentOutOfRangeException) { await evolvePokemon(pokemon); }
            catch (ArgumentNullException) { await evolvePokemon(pokemon); }
            catch (NullReferenceException) { await evolvePokemon(pokemon); }
            catch (Exception ex) { await evolvePokemon(pokemon); }
        }

        private static async Task transferPokemon(PokemonData pokemon)
        {
            try
            {
                var transferPokemonResponse = await client.TransferPokemon(pokemon.Id);
                string message = "";
                string caption = "";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                if (transferPokemonResponse.Status == 1)
                {
                    message = $"{pokemon.PokemonId} was transferred\n{transferPokemonResponse.CandyAwarded} candy awarded";
                    caption = $"{pokemon.PokemonId} transferred";
                }
                else
                {
                    message = $"{pokemon.PokemonId} could not be transferred";
                    caption = $"Transfer {pokemon.PokemonId} failed";
                }

                result = MessageBox.Show(message, caption, buttons, MessageBoxIcon.Information);
            }
            catch (TaskCanceledException) { await transferPokemon(pokemon); }
            catch (UriFormatException) { await transferPokemon(pokemon); }
            catch (ArgumentOutOfRangeException) { await transferPokemon(pokemon); }
            catch (ArgumentNullException) { await transferPokemon(pokemon); }
            catch (NullReferenceException) { await transferPokemon(pokemon); }
            catch (Exception ex) { await transferPokemon(pokemon); }
        }

        private async void btnUpgrade_Click(object sender, EventArgs e)
        {
            IEnumerable<DataGridViewRow> selectedItems = null;

            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                selectedItems = this.dataGridView1.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.OwningRow).Distinct();
            }
            else
            {
                selectedItems = this.dataGridView1.SelectedRows.Cast<DataGridViewRow>();
            }

            foreach (DataGridViewRow selectedItem in selectedItems)
            {
                await PowerUp((PokemonData)selectedItem.Tag);
            }

            this.dataGridView1.DataSource = null;
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Rows.Clear();
            Execute();
        }

        private static async Task PowerUp(PokemonData pokemon)
        {
            try
            {
                var evolvePokemonResponse = await client.PowerUp(pokemon.Id);
                string message = "";
                string caption = "";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                if (evolvePokemonResponse.Result == 1)
                {
                    message = $"{pokemon.PokemonId} successfully upgraded.";
                    caption = $"{pokemon.PokemonId} upgraded";
                }
                else
                {
                    message = $"{pokemon.PokemonId} could not be upgraded";
                    caption = $"Upgrade {pokemon.PokemonId} failed";
                }

                result = MessageBox.Show(message, caption, buttons, MessageBoxIcon.Information);
            }
            catch (TaskCanceledException) { await PowerUp(pokemon); }
            catch (UriFormatException) { await PowerUp(pokemon); }
            catch (ArgumentOutOfRangeException) { await PowerUp(pokemon); }
            catch (ArgumentNullException) { await PowerUp(pokemon); }
            catch (NullReferenceException) { await PowerUp(pokemon); }
            catch (Exception ex) { await PowerUp(pokemon); }
        }

    }
}
