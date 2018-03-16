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
    public partial class fridgeList : Form
    {
        icebox icebox1;
        string progressBar_name;

        public fridgeList(icebox f, string name)
        {
            InitializeComponent();
            icebox1 = f;
            progressBar_name = name;
        }

        private void fridgeList_Load(object sender, EventArgs e)
        {
            //
            //called from order phase
            //
            if (progressBar_name == "order")
            {
                this.Text = "Icebox®: Fridge & Shopping food lists";
                if (icebox1.stored_food.Count > 0)
                {
                    foreach (item item1 in icebox1.stored_food)
                    {
                        listView1.Items.Add(item1.detailsOfItem());
                    }
                }
                else
                {
                    listView1.Items.Add("Oops... The fridge is empty.");
                }
                if (icebox1.temp_list.Count > 0)
                {
                    foreach (item item1 in icebox1.temp_list)
                    {
                        listView2.Items.Add(item1.detailsOfItem());
                    }
                }
                else
                {
                    listView2.Items.Add("Your shopping list is empty...");
                }
            }
            //
            //called from inside
            //   
            else if (progressBar_name == "fridge")
            {
                this.Text = "Icebox®: Fridge food list";
                groupBox2.Visible = false;
                if (icebox1.stored_food.Count > 0)
                {
                    foreach (item item1 in icebox1.stored_food)
                    {
                        listView1.Items.Add(item1.detailsOfItem());
                    }
                }
                else
                {
                    listView1.Items.Add("Oops... The fridge is empty.");
                }
            }
        }

        private void fridgeList_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
