using PoGo.NecroBot.Logic.Model;
using RocketBot2.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketBot2.Forms
{
    public partial class PokemonPropertiesForm : Form
    {
        public PokemonPropertiesForm()
        {
            InitializeComponent();
        }

        public PokemonPropertiesForm(PokemonObject pokemon)
        {
            InitializeComponent();

            pbPokemon.Image = ResourceHelper.GetImage(null, (int)pokemon.PokemonId, pokemon.Shiny, pbPokemon.Size.Height, pbPokemon.Size.Width);
            lbName.Text = string.IsNullOrEmpty(pokemon.Nickname) ? pokemon.PokemonId.ToString() : pokemon.Nickname;
            Text = $"Properties of {lbName.Text}";
            lbTypes.Text = $"Types \n\r {pokemon.Types}";
            lbSex.Text = $"Sex: {pokemon.Sex}";
            lbShiny.Text = pokemon.Shiny ? "Shiny: Yes" : "Shiny: No";
            lbMove1.Text = "Move1 \n\r {pokemon.Move1}";
            lbMove2.Text = $"Move2 \n\r {pokemon.Move2}";
            lbHP.Text = $"HP: {pokemon.HP}/{pokemon.MaxHP}";
            lbCp.Text = $"CP: {pokemon.Cp.ToString()}";
            lbIV.Text = $"IV: {pokemon.GetIV * 100}%";
            lbLevel.Text = $"Level: {pokemon.GetLv}";
            lbCaughtloc.Text = $"Caught Location: {pokemon.CaughtLocation}";
            lbCountry.Text = $"Country: {pokemon.GeoLocation}";
            lbStamina.Text = $"Stamina: {pokemon.IndividualStamina}";
            lbAtk.Text = $"Attack: {pokemon.IndividualAttack}";
            lbDefense.Text = $"Defense: {pokemon.IndividualDefense}";
            lbCandy.Text = $"Candy: {pokemon.Candy}";
            lbCaughtTime.Text = $"Caught Time: {pokemon.CaughtTime}";
            lbCantoEvo.Text = $"Candy To Evolve: {pokemon.CandyToEvolve}";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
