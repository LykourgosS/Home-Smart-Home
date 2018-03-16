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
    public partial class get_food_to_eat : Form
    {
        icebox icebox1;

        public get_food_to_eat(icebox f)
        {
            InitializeComponent();
            icebox1 = f;
        }

        private void get_food_to_eat_Load(object sender, EventArgs e)
        {
            //tooltip
            toolTip1.SetToolTip(this.button1, "Add product to list");
            toolTip1.SetToolTip(this.button2, "Remove product from list");
            toolTip1.SetToolTip(this.button3, "Take products to eat...");
            toolTip1.SetToolTip(this.progressBar1, "Click to see the available products...");

            //comboBox1.DropDownHeight = comboBox1.ItemHeight * 5;
            foreach (item item1 in icebox1.stored_food)
            {
                comboBox1.Items.Add(item1.name);
            }
            progressBar1.Value = Convert.ToInt32(icebox1.fridge_capacity());
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (listView1.CheckedItems.Count > 0)
            {
                button2.Visible = true;
            }
            else
            {
                button2.Visible = false;
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
            icebox1.show_list("fridge");
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                numericUpDown1.Enabled = true;
                numericUpDown1.Maximum = icebox1.stored_food.Find(item => item.name == comboBox1.SelectedItem.ToString()).quantity;
                numericUpDown1.Value = 0;
            }
            else
            {
                numericUpDown1.Enabled = false;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 0)
            {
                button1.Visible = true;
            }
            else
            {
                button1.Visible = false;
            }
        }

        private void numericUpDown1_EnabledChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Enabled && numericUpDown1.Value > 0)
            {
                button1.Visible = true;
            }
            else
            {
                button1.Visible = false;
            }
        }

        private void update_listView()
        {
            listView1.Clear();
            foreach (item item1 in icebox1.temp_list)
            {
                listView1.Items.Add(item1.detailsOfItem());
            }
        }

        private void update_combobox()
        {
            comboBox1.Items.Clear();
            foreach (item item1 in icebox1.stored_food)
            {
                comboBox1.Items.Add(item1.name);
            }
        }

        private void update_quantities()
        {
            update_listView();
            update_combobox();
            if (icebox1.temp_list.Count > 0)
            {
                button3.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
            }
            progressBar1.Value = Convert.ToInt32(icebox1.fridge_capacity());
        }

        //
        //add a product to eating list
        //
        private void add_to_eating_list(item item1)
        {
            if (numericUpDown1.Value == item1.quantity) //=>took all the available quantity of the product
            {
                icebox1.stored_food.Remove(item1);
            }
            else                                        //=>took some of  the available quantity of the product
            {
                icebox1.stored_food.Find(item => item.name == item1.name).quantity -= numericUpDown1.Value;
            }
            try                                         //check if there is some of this product already in eating list, if yes, the 2 quantities are being added
            {
                icebox1.temp_list.Find(item => item.name == item1.name).quantity += numericUpDown1.Value;
            }
            catch
            {
                icebox1.temp_list.Add(new item(item1.name, numericUpDown1.Value, item1.category));
            }
            update_quantities();
        }

        //
        //button1: add product to eating list
        //
        private void button1_Click(object sender, EventArgs e)
        {
            item item_to_eat = icebox1.stored_food.Find(item => item.name == comboBox1.SelectedItem.ToString());
            add_to_eating_list(item_to_eat);
            button1.Visible = false;
            numericUpDown1.Value = 0;
            numericUpDown1.Enabled = false;
        }

        private void remove_from_eating_list(item old_item)
        {
            try
            {
                icebox1.stored_food.Find(item => item.name == old_item.name).quantity += old_item.quantity;
            }
            catch
            {
                icebox1.stored_food.Add(old_item);
            }
            icebox1.temp_list.Remove(old_item);
        }

        //
        //button2: remove product from eating list
        //
        private void button2_Click(object sender, EventArgs e)
        {
            item item1;
            for (int i = listView1.CheckedItems.Count; i > 0; i--)
            {
                item1 = icebox1.temp_list.Find(item => item.detailsOfItem() == listView1.CheckedItems[i - 1].Text);
                remove_from_eating_list(item1);
            }           
            update_quantities();
            button2.Visible = false;
        }

        //
        //button3: complete serving food
        //
        private void button3_Click(object sender, EventArgs e)
        {
            icebox1.got_food = true;
            MessageBox.Show("Have a nice meal !", "Icebox®: Bon Appétit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}