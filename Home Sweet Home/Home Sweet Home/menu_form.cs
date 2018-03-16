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
    public partial class menu_form : Form
    {
        public Device[] open_devices = new Device[3];        

        public menu_form()
        {
            InitializeComponent();            
        }

        private void menu_form_Load(object sender, EventArgs e)
        {
            //tooltip
            toolTip1.SetToolTip(this.button1, "Icebox®");
            toolTip1.SetToolTip(this.button2, "Lights out®");
            toolTip1.SetToolTip(this.button3, "e-Kettle®");

            open_devices[0] = new Device(new icebox(this), false);          //icebox form is in open_devices[0]
            open_devices[1] = new Device(new lights_out(this), false);          //lights_out form is in open_devices[1]
            open_devices[2] = new Device(new e_kettle(this), false);     //e_kettle form is in open_devices[2]
        }

        //icebox button
        private void button1_Click(object sender, EventArgs e)
        {
            if (!open_devices[0].is_open)
            {
                open_devices[0].form.Show();
                open_devices[0].is_open = true;
            }
            else
            {
                open_devices[0].form.Focus();
                //MessageBox.Show("Can't open Icebox, because is already open in another window.", "Oops, Icebox is already open!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //lights_out button
        private void button2_Click(object sender, EventArgs e)
        {
            if (!open_devices[1].is_open)
            {
                open_devices[1].form.Show();
                open_devices[1].is_open = true;
            }
            else
            {
                open_devices[1].form.Activate();
                // MessageBox.Show("Can't open lights_out, because is already open in another window.", "Oops, lights_out is already open!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //e-kettle button
        private void button3_Click(object sender, EventArgs e)
        {
            if (!open_devices[2].is_open)
            {
                open_devices[2].form.Show();
                open_devices[2].is_open = true;
            }
            else
            {
                open_devices[2].form.Activate();
                //MessageBox.Show("Can't open Coffee Maker, because is already open in another window.", "Oops, Coffee Maker is already open!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            button1.PerformClick();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            button2.PerformClick();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            button3.PerformClick();
        }
    }

    public class Device
    {
        public Form form;
        public bool is_open;

        public Device(Form f, bool status)
        {
            form = f;
            is_open = status;
        }
    }


}