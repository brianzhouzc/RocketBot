using AllEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace PokemonGo.RocketAPI.Window
{
    public partial class PokemonLists : Form
    {
        public PokemonLists()
        {
            InitializeComponent();
            List<PokemonId> userUnwanted = JsonConvert.DeserializeObject<List<PokemonId>>(Properties.Settings.Default["userUnwanted"].ToString());
            List<PokemonId> userSkipped = JsonConvert.DeserializeObject<List<PokemonId>>(Properties.Settings.Default["userSkipped"].ToString());
            pokeList.AutoGenerateColumns = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("Pokeman", typeof(PokemonId));
            dt.Columns.Add("Keep Strongest", typeof(bool));
            dt.Columns.Add("Don't Catch", typeof(bool));
            if (userUnwanted == null) userUnwanted = new List<PokemonId>();
            if (userSkipped == null) userSkipped = new List<PokemonId>();
            foreach (PokemonId pId in Enum.GetValues(typeof(PokemonId)))
            {
                dt.Rows.Add(pId, userUnwanted.Contains(pId), userSkipped.Contains(pId));
            }
            pokeList.DataSource = dt;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {

            List<PokemonId> userUnwanted = new List<PokemonId>();
            List<PokemonId> userSkipped = new List<PokemonId>();
            foreach (DataGridViewRow row in pokeList.Rows)
            {
                if ((bool)row.Cells[1].Value == true)
                    userUnwanted.Add((PokemonId)row.Cells[0].Value);
                if ((bool)row.Cells[2].Value == true)
                    userSkipped.Add((PokemonId)row.Cells[0].Value);
            }
            Properties.Settings.Default["userUnwanted"] = JsonConvert.SerializeObject(userUnwanted);
            Properties.Settings.Default["userSkipped"] = JsonConvert.SerializeObject(userSkipped);
            MessageBox.Show(Properties.Settings.Default["userUnwanted"].ToString() + (char)10 + Properties.Settings.Default["userSkipped"].ToString());
            Close();
        }
    }
}
