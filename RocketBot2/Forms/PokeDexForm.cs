using PoGo.NecroBot.Logic.State;
using POGOProtos.Enums;
using RocketBot2.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketBot2.Forms
{
    public partial class PokeDexForm : System.Windows.Forms.Form
    {
        private ISession session;

        public PokeDexForm(Session session)
        {
            InitializeComponent();
            this.session = session;
            DefaultValuesAsync().ConfigureAwait(false);
        }

        private async Task DefaultValuesAsync()
        {
            var x = await this.session.Inventory.GetPokeDexItems();
            Dictionary<PokemonId, int> see = new Dictionary<PokemonId, int>();
            Dictionary<PokemonId, int> cath = new Dictionary<PokemonId, int>();

            foreach (var item in x)
            {
                var entry = item.InventoryItemData.PokedexEntry;
                see.Add(entry.PokemonId, entry.TimesEncountered);
                cath.Add(entry.PokemonId, entry.TimesCaptured);
            }

            foreach (PokemonId e in Enum.GetValues(typeof(PokemonId)))
            {
                if (e == PokemonId.Missingno || (int)e > 251) continue;
                Image img = ResourceHelper.SetImageSize(ResourceHelper.GetPokemonImageById((int)e), 48, 48);
                int vu = 0;
                int cap = 0;
                if (see.ContainsKey(e)) vu = see[e];
                if (cath.ContainsKey(e)) cap = cath[e];
                if (!see.ContainsKey(e) && !cath.ContainsKey(e)) img = ResourceHelper.ConvertToBlack(img);
                if (see.ContainsKey(e) && !cath.ContainsKey(e)) img = ResourceHelper.ConvertToBlackAndWhite(img);
                var Pok = new ItemBox(vu, cap, img);
                flpPokeDex.Controls.Add(Pok);
            }
        }
    }
}


