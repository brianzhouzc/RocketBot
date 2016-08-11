using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Window
{
    public partial class NicknamePokemonForm : Form
    {
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
        }
    }
}