using PoGo.NecroBot.Logic.State;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using RocketBot2.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
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
            var i = new List<PokemonId>();
            
            foreach (var item in x)
            {
                var entry = item.InventoryItemData.PokedexEntry;
                if (entry.TimesCaptured > 0)
                i.Add(entry.PokemonId);
            }

            foreach (PokemonId e in Enum.GetValues(typeof(PokemonId)))
            {
                if (e == PokemonId.Missingno || (int)e > 251) continue;
                var Pok = new PictureBox();
                Image img = ResourceHelper.SetImageSize(ResourceHelper.GetPokemonImageById((int)e), Pok.Size.Height, Pok.Size.Width);
                
                Pok.Image = i.Contains(e) ? img : ResourceHelper.ConvertToBlack(img);
                //Pok.BackColor = i.Contains(e) ? Color.Transparent : Color.LightGray;
                flpPokeDex.Controls.Add(Pok);
            }
        }
    }
}


