using System;
using System.Drawing;
using System.Windows.Forms;

namespace RocketBot.Window
{
    internal partial class SettingsForm : Form
    {
        private readonly ToolTip toolTip1 = new ToolTip();

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
        }


        private void saveBtn_Click(object sender, EventArgs e)
        {
        }


        private void authTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (authTypeCb.Text == "google")
            {
                UserLabel.Text = "Email:";
            }
            else
            {
                UserLabel.Text = "Username:";
            }
        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            var localCoordinates = e.Location;
            gMapControl1.Position = gMapControl1.FromLocalToLatLng(localCoordinates.X, localCoordinates.Y);

            if (e.Clicks >= 2)
            {
                gMapControl1.Zoom += 5;
            }

            var X = Math.Round(gMapControl1.Position.Lng, 6);
            var Y = Math.Round(gMapControl1.Position.Lat, 6);
            var longitude = X.ToString();
            var latitude = Y.ToString();
            latitudeText.Text = latitude;
            longitudeText.Text = longitude;
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            gMapControl1.Zoom = trackBar.Value;
        }

        private void FindAdressButton_Click(object sender, EventArgs e)
        {
            gMapControl1.SetPositionByKeywords(AdressBox.Text);
            gMapControl1.Zoom = 15;
        }

        private void authTypeLabel_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void transferCpThresText_TextChanged(object sender, EventArgs e)
        {
        }

        private void transferTypeCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (transferTypeCb.Text == "CP")
            {
                CPThresholdLabel.Visible = true;
                transferCpThresText.Visible = true;
            }
            else
            {
                CPThresholdLabel.Visible = false;
                transferCpThresText.Visible = false;
            }

            if (transferTypeCb.Text == "IV")
            {
                IVThresholdLabel.Visible = true;
                transferIVThresText.Visible = true;
            }
            else
            {
                IVThresholdLabel.Visible = false;
                transferIVThresText.Visible = false;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {
        }

        private void FindAdressButton_Click_1(object sender, EventArgs e)
        {
            gMapControl1.SetPositionByKeywords(AdressBox.Text);
            gMapControl1.Zoom = 15;
            var X = Math.Round(gMapControl1.Position.Lng, 6);
            var Y = Math.Round(gMapControl1.Position.Lat, 6);
            var longitude = X.ToString();
            var latitude = Y.ToString();
            latitudeText.Text = latitude;
            longitudeText.Text = longitude;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void evolveAllChk_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void TravelSpeedBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void razzSettingText_KeyPress(object sender, KeyPressEventArgs e)
        {
            var ch = e.KeyChar;
            if (!char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void AdressBox_Leave(object sender, EventArgs e)
        {
            if (AdressBox.Text.Length == 0)
            {
                AdressBox.Text = "Enter an address or a coordinate";
                AdressBox.ForeColor = SystemColors.GrayText;
            }
        }

        private void AdressBox_Enter(object sender, EventArgs e)
        {
            if (AdressBox.Text == "Enter an address or a coordinate")
            {
                AdressBox.Text = "";
                AdressBox.ForeColor = SystemColors.WindowText;
            }
        }

        private void AdressBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void transferTypeCb_DropDownClosed(object sender, EventArgs e)
        {
            toolTip1.Hide(transferTypeCb);
        }

        private void transferTypeCb_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            } // added this line thanks to Andrew's comment
            string tip;
            switch (transferTypeCb.GetItemText(transferTypeCb.Items[e.Index]))
            {
                case "Leave Strongest":
                    tip = "N/A";
                    break;
                case "All":
                    tip = "Transfer all pokemons";
                    break;
                case "CP Duplicate":
                    tip = "Only keeping the highest CP pokemon of each type";
                    break;
                case "IV Duplicate":
                    tip = "Only keeping the highest IV pokemon of each type";
                    break;
                case "CP/IV Duplicate":
                    tip = "Keeping one highest pokemon CP and one highest IV pokemon of each type";
                    break;
                case "CP":
                    tip = "Only keeping pokemons that have a CP higher than the threshold";
                    break;
                case "IV":
                    tip = "Only keeping pokemons that have a IV higher than the threshold";
                    break;
                default:
                    tip = "Not transfering any pokemons";
                    break;
            }
            e.DrawBackground();
            using (var br = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(transferTypeCb.GetItemText(transferTypeCb.Items[e.Index]), e.Font, br, e.Bounds);
            }
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                toolTip1.Show(tip, transferTypeCb, e.Bounds.Right, e.Bounds.Bottom);
            }
            e.DrawFocusRectangle();
        }
    }
}