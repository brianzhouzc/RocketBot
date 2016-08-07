using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace PoGo.RocketBot.Window
{
    public partial class MainForm : Form
    {
        private static string _title = "RocketBot";
        public static MainForm Instance;

        public MainForm()
        {
            InitializeComponent();
            CenterToScreen();
            Instance = this;
            _title += " v" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        public void MainForm_Load(object sender, EventArgs e)
        {

        }

        public static void ColoredConsoleWrite(Color color, string text)
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke(new Action<Color, string>(ColoredConsoleWrite), color, text);
                return;
            }
            Instance.logTextBox.Select(Instance.logTextBox.Text.Length, 1); // Reset cursor to last
            Instance.logTextBox.SelectionColor = color;
            Instance.logTextBox.AppendText(text);

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