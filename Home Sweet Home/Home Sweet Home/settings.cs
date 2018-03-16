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
    public partial class settings : Form
    {
        icebox icebox1;

        public settings(icebox f)
        {
            InitializeComponent();
            icebox1 = f;
        }

        private void settings_Load(object sender, EventArgs e)
        {
            //tooltip
            toolTip1.SetToolTip(this.ok_button, "Save changes");
            toolTip1.SetToolTip(this.cancel_button, "Discard changes");
            toolTip1.SetToolTip(this.button3, "Help");

            numericUpDown1.Value = icebox1.threshold;
            numericUpDown2.Value = icebox1.min;
            numericUpDown3.Value = icebox1.default_q;
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            icebox1.threshold = numericUpDown1.Value;
            icebox1.min = numericUpDown2.Value;
            icebox1.default_q = numericUpDown3.Value;
            this.Close();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            help help_form1 = new help("settings");
            help_form1.ShowDialog();
        }
    }
}
