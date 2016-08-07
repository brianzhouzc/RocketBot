using System;
using System.Windows.Forms;

namespace RocketBot.Window
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            CenterToScreen();
        }

        public void MainForm_Load(object sender, EventArgs e)
        {
        }

        public void btnRefreshPokemonList_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void forceUnbanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void useLuckyEggToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        public void startStopBotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}