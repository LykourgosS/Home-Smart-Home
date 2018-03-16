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
    public partial class supermarket_selection : Form
    {
        icebox icebox1;

        public supermarket_selection(icebox f)
        {
            InitializeComponent();
            icebox1 = f;
        }

        private void order_message(string name)
        {
            string message = "Your order has been sent to " + name + "." + Environment.NewLine + "You will be notified by an e-mail from " + name + " about the order's status!";
            MessageBox.Show(message, "Order has been sent to " + name, MessageBoxButtons.OK, MessageBoxIcon.Information);            
            icebox1.order_completed = true;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            order_message("Σκλαβενίτης");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            order_message("My Market");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            order_message("Metro");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            order_message("ΑΒ Βασιλόπουλος");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
