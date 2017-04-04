using PoGo.NecroBot.Logic.Model;
using PoGo.NecroBot.Logic.State;
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

        public PokemonPropertiesForm(ISession session, PokemonObject pokemon)
        {
            InitializeComponent();

            pbPokemon.Image = ResourceHelper.GetImageSize(pokemon.Icon, pbPokemon.Size.Height, pbPokemon.Size.Width);
            var pokName = string.IsNullOrEmpty(pokemon.Nickname) ? session.Translation.GetPokemonTranslation(pokemon.PokemonId) : pokemon.Nickname;
            lbName.Text = $"{pokName} Level: {pokemon.GetLv}";
            Text = $"Properties of {lbName.Text}";
            lbTypes.Text = $"Types\n\r {pokemon.Types}";
            lbSex.Text = $"Sex: {pokemon.Sex}";
            lbShiny.Text = pokemon.Shiny ? "Shiny: Yes" : "Shiny: No";
            lbMove1.Text = $"Move1\n\r {pokemon.Move1}";
            lbMove2.Text = $"Move2\n\r {pokemon.Move2}";
            lbHP.Text = $"HP: {pokemon.HP}/{pokemon.MaxHP}";
            lbCp.Text = $"CP: {pokemon.Cp}";
            lbIV.Text = $"IV: {pokemon.GetIV * 100}%";
            lbCaughtloc.Text = $"Caught Location: {pokemon.CaughtLocation}";
            lbCountry.Text = $"Country: {pokemon.GeoLocation}";
            lbStamina.Text = $"Stamina: {pokemon.IndividualStamina}";
            lbAtk.Text = $"Attack: {pokemon.IndividualAttack}";
            lbDefense.Text = $"Defense: {pokemon.IndividualDefense}";
            lbCandy.Text = $"{pokemon.Candy} candy {pokemon.FamilyId}\n\r{pokemon.CandyToEvolve} candy required to evolve";
            lbCaughtTime.Text = $"Caught Time: {pokemon.CaughtTime}";
            lbKg.Text = $"Weight {pokemon.WeightKg}kg";
            lbHeight.Text = $"Height {pokemon.HeightM}m";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
