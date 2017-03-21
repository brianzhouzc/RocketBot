using POGOProtos.Data;
using RocketBot2.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RocketBot2.Forms
{
    public partial class EvoleToPokemon : Form
    {
        public EvoleToPokemon()
        {
            InitializeComponent();
        }

        private void EvoleToPokemon_Load(object sender, EventArgs e)
        {
            var _point = new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y);
            Top = _point.Y;
            Left = MainForm.Instance.Location.X + MainForm.Instance.Width - Width;
        }
    }
}
