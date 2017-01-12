using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoGo.NecroBot.Logic.Forms
{
    public partial class CaptchaSolveForm : Form
    {
        public CaptchaSolveForm(string url)
        {
            this.captchaUrl = url;
            InitializeComponent();
        }
        private string captchaUrl = "";
        private void CaptchaSolveForm_Load(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate(captchaUrl);

        }
    }
}
