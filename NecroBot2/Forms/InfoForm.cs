using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NecroBot2.Forms
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            this.Text = "Necrobot 2 - " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.mypogosnipers.com/?donate");
        }

        private void link_click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(((LinkLabel)sender).Text);

        }
        private int count = 60;
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (count-- <= 0)
            {
                this.Close();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Necrobot-Private/NecroBot/releases");
        }
    }
}
