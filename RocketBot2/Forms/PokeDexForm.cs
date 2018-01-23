using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using RocketBot2.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace RocketBot2.Forms
{
    public partial class PokeDexForm : System.Windows.Forms.Form
    {
        private ISession session;

        public PokeDexForm(Session session, ItemData item = null)
        {
            InitializeComponent();
            this.session = session;
            if (item != null)
                LoadPokemons(item);
            else
                DefaultValuesAsync().ConfigureAwait(false);
        }

        private void LoadPokemons(ItemData item)
        {
            var pokemons =
                  session.Inventory.GetPokemons().Result
                  .Where(p => p != null && p.PokemonId > 0)
                  .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                  .ThenByDescending(key => key.Cp)
                  .OrderBy(key => key.PokemonId);

            List<PokemonId> list = new List<PokemonId>();
 
            foreach (var pokemon in pokemons)
            {
                if (!list.Contains(pokemon.PokemonId))
                {
                    PokemonObject _pokemon = new PokemonObject(session, pokemon);
                    if (_pokemon.Slashed) continue;
                    list.Add(pokemon.PokemonId);
                    Image img = ResourceHelper.SetImageSize(_pokemon.Icon, 48, 48);
                    var Pok = new ItemBox(session, item, pokemon, img);
                    flpPokeDex.Controls.Add(Pok);
                }
            }
            PokeDexForm.ActiveForm.Refresh();
        }

        private async Task DefaultValuesAsync()
        {
            var x = await this.session.Inventory.GetPokeDexItems();
            Dictionary<PokemonId, int> seen = new Dictionary<PokemonId, int>();
            Dictionary<PokemonId, int> cath = new Dictionary<PokemonId, int>();

            foreach (var item in x)
            {
                var entry = item.InventoryItemData.PokedexEntry;
                seen.Add(entry.PokemonId, entry.TimesEncountered);
                cath.Add(entry.PokemonId, entry.TimesCaptured);
            }

            foreach (PokemonId e in Enum.GetValues(typeof(PokemonId)))
            {
                if (e == PokemonId.Missingno || (int)e > 251) continue;
                Image img = ResourceHelper.SetImageSize(ResourceHelper.GetPokemonImageById((int)e), 48, 48);
                int vu = 0;
                int cap = 0;
                string name = session.Translation.GetPokemonTranslation(e);
                if (seen.ContainsKey(e)) vu = seen[e];
                if (cath.ContainsKey(e)) cap = cath[e];
                if (vu == 0 && cap == 0) img = ResourceHelper.ConvertToBlack(img);
                if (vu > 0 && cap == 0) img = ResourceHelper.ConvertToBlackAndWhite(img);
                var Pok = new ItemBox(vu, cap, img, name);
                flpPokeDex.Controls.Add(Pok);
            }
            PokeDexForm.ActiveForm.Refresh();
        }
    }
}


