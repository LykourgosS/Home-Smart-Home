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
    public partial class temp_humid : Form
    {
        bool celcious = true;

        public temp_humid()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            int c_degrees = 5;
            if (celcious)
            {
                label1.Text = CelciousToFahrenheit(c_degrees).ToString() + "°F";
                celcious = false;
            }
            else
            {
                label1.Text = FahrenheitToCelcious(CelciousToFahrenheit(c_degrees)).ToString() + "°C";
                celcious = true;
            }
        }

        private double CelciousToFahrenheit(double celcious)
        {
            return celcious * 1.8 + 32;
        }

        private double FahrenheitToCelcious(double fahrenheit)
        {
            return (fahrenheit - 32) / 1.8;
        }

        private void progressBar3_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Value = 23;
        }

        //
        //help button
        //
        private void button3_Click(object sender, EventArgs e)
        {
            help help_form1 = new help("temp_hum");
            help_form1.ShowDialog();
        }

        private void temp_humid_Load(object sender, EventArgs e)
        {
            //tooltip
            toolTip1.SetToolTip(this.label1, "Click to change measurement unit (°F/°C)");
            toolTip1.SetToolTip(this.button3, "Help");
            toolTip1.SetToolTip(this.progressBar1, "Click to retake measurement...");
        }
    }
}
