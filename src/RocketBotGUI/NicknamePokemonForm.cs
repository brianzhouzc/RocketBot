using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window {
    public partial class NicknamePokemonForm : Form {
        public NicknamePokemonForm(PokemonObject pokemon) {
            InitializeComponent();
            txtNickname.Text = pokemon.Nickname;
            txtNickname.KeyDown += TxtNickname_KeyDown;
        }

        private void TxtNickname_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
