using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace PoGo.RocketBot.Window
{
    public partial class MainForm : Form
    {
        private static string _title = "RocketBot";
        private static MainForm _instance;

        public MainForm()
        {
            InitializeComponent();
            CenterToScreen();
            _instance = this;
            _title += " v" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        public void MainForm_Load(object sender, EventArgs e)
        {
        }

        public static void ColoredConsoleWrite(Color color, string text)
        {
            if (_instance.InvokeRequired)
            {
                _instance.Invoke(new Action<Color, string>(ColoredConsoleWrite), color, text);
                return;
            }
            _instance.logTextBox.Select(_instance.logTextBox.Text.Length, 1); // Reset cursor to last
            _instance.logTextBox.SelectionColor = color;
            _instance.logTextBox.AppendText(text);

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