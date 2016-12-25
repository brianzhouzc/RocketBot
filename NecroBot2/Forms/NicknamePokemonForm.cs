using System;
using System.Windows.Forms;

namespace NecroBot2.Forms
{
    public partial class NicknamePokemonForm : Form
    {
        public NicknamePokemonForm()
        {
            InitializeComponent();
            txtNickname.Text = @"{IV}_{Name}";
            txtNickname.KeyDown += TxtNickname_KeyDown;
        }

        public NicknamePokemonForm(PokemonObject pokemon)
        {
            InitializeComponent();
            txtNickname.Text = pokemon.Nickname;
            txtNickname.KeyDown += TxtNickname_KeyDown;
        }

        private void TxtNickname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void closeRenameBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}