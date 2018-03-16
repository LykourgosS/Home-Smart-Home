using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Home_Smart_Home
{
    public partial class e_kettle : Form
    {
        menu_form menu;
        bool on = false;

        public e_kettle(menu_form f)
        {
            InitializeComponent();
            menu = f;
        }

        private void e_kettle_FormClosed(object sender, FormClosedEventArgs e)
        {
            menu.open_devices[2] = new Device(new e_kettle(menu), false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (on)
            {
                button1.BackgroundImage = Image.FromFile(@".\images\off.jpg");
                on = false;
                MessageBox.Show("Now the kettle is turned off.", "e-Kettle® Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                button1.BackgroundImage = Image.FromFile(@".\images\on.jpg");
                on = true;
                MessageBox.Show("Now the kettle is turned on, it will stop automatically when the water has boiled.", "e-Kettle® Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
