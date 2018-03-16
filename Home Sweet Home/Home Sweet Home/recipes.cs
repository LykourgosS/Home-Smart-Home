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
    public partial class recipes : Form
    {
        public List<item> recipe_list = new List<item>();
        icebox icebox1;
        string missing_products = "";


        public recipes(icebox f)
        {
            InitializeComponent();
            icebox1 = f;
        }

        private void recipes_Load(object sender, EventArgs e)
        {
            //tooltip
            toolTip1.SetToolTip(this.button1, "Order");
            toolTip1.SetToolTip(this.button2, "Start cooking");
            toolTip1.SetToolTip(this.button3, "Help");
            toolTip1.SetToolTip(this.pictureBox1, "Serving suggestion");

            if (icebox1.recipes_is_waiting)
            {
                comboBox1.SelectedItem = icebox1.selected_recipe;
                update_recipes();
            }
            else
            {
                listView1.Items.Add("No recipe has been selected...");
            }
        }

        private void fish_broccoli()
        {
            recipe_list.Add(new item("Fish", 2, "kg(s)"));
            recipe_list.Add(new item("Broccoli", 3, "kg(s)"));
        }

        private void chicken_veggies()
        {
            recipe_list.Add(new item("Chicken", 2, "kg(s)"));
            recipe_list.Add(new item("Carrots", 1, "kg(s)"));
            recipe_list.Add(new item("Eggplants", 1, "kg(s)"));
            recipe_list.Add(new item("Lettuce", 1, "kg(s)"));
        }

        private void kebab()
        {
            recipe_list.Add(new item("Beef", 3, "kg(s)"));
            recipe_list.Add(new item("Yoghurt (1kg)", 2, "piece(s)"));
            recipe_list.Add(new item("Tomatoes", 1, "kg(s)"));
        }

        private void set_recipe()
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Fish & Broccoli":
                    pictureBox1.ImageLocation = @".\images\fish_broccoli.jpg";
                    fish_broccoli();
                    break;
                case "Roasted Chicken and Veggies":
                    pictureBox1.ImageLocation = @".\images\Roasted-chicken-and-vegetables.jpg";
                    chicken_veggies();
                    break;
                case "Kebab with Tomato Sauce & Yoghurt":
                    pictureBox1.ImageLocation = @".\images\kebab.jpg";
                    kebab();
                    break;
                case null:
                    pictureBox1.Image = null;
                    listView1.Items.Add("No receipe have been selected...");
                    break;
            }
        }

        private void load_recipe()
        {
            listView1.Items.Clear();
            item itemForRecipe;
            ListViewItem lstV_item1;
            foreach (item item1 in recipe_list)
            {
                itemForRecipe = null;
                lstV_item1 = new ListViewItem(item1.detailsOfItem());
                itemForRecipe = icebox1.stored_food.Find(item => item.name == item1.name && item.quantity >= item1.quantity);
                if (itemForRecipe == null)
                {
                    itemForRecipe = item1;
                    icebox1.add_to_order_list(itemForRecipe.name, itemForRecipe.quantity, itemForRecipe.category, false);
                    //MessageBox.Show(icebox1.temp_list.Count.ToString() + icebox1.recipes_is_waiting); //for testing
                    lstV_item1.ForeColor = Color.Red;
                    missing_products += "• " + itemForRecipe.detailsOfItem() + Environment.NewLine;
                }
                listView1.Items.Add(lstV_item1);
            }            
        }

        private void set_buttons()
        {
            if (missing_products != "")
            {
                label1.Visible = true;
                button1.Visible = true;
                button2.Visible = false;
            }
            else
            {
                label1.Visible = false;
                button1.Visible = false;
                button2.Visible = true;
            }
        }

        private void update_recipes()
        {            
            icebox1.selected_recipe = null;
            missing_products = "";
            recipe_list.Clear();
            icebox1.temp_list.Clear();
            //MessageBox.Show("cleared_after_updating_recipes"); //-->debugging
            set_recipe();
            load_recipe();
            set_buttons();
            icebox1.recipes_is_waiting = false;
        }

        //
        //comboBox for choosing recipe
        //
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            update_recipes();
        }

        //
        //button1: order button
        //
        private void button1_Click(object sender, EventArgs e)
        {
            if (icebox1.fridge_capacity() + icebox1.get_list_quantity(recipe_list) <= 40)
            {
                DialogResult result = MessageBox.Show("The following product(s) are missing:" + Environment.NewLine + missing_products + "Do you want to order them online now?", "Warning: Products are missing!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    icebox1.recipes_is_waiting = true;
                    icebox1.orderState();
                    icebox1.selected_recipe = comboBox1.SelectedItem.ToString();
                    this.Close();
                    //MessageBox.Show(icebox1.temp_list.Count.ToString() + icebox1.recipes_is_waiting); //-->debugging
                }
            }
            else
            {
                MessageBox.Show("There is not enough space in the fridge to order the missing products:" + Environment.NewLine + missing_products + "Free up space and then try placing a new order.", "Oops, no more space in the fridge!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //this.Close();
            }           
        }

        private void remove_from_stored()
        {
            foreach (item item1 in recipe_list)
            {
                item stored_item = icebox1.stored_food.Find(item => item.name == item1.name);                
                if (stored_item.quantity == item1.quantity)
                {
                    icebox1.stored_food.Remove(stored_item);
                }
                else
                {
                    stored_item.quantity -= item1.quantity;
                }
            }
        }

        //
        //button2: serve food and close recipes
        //
        private void button2_Click(object sender, EventArgs e)
        {
            if (icebox1.temp_list.Count == 0 && recipe_list.Count > 0)
            {
                DialogResult result = MessageBox.Show("Do you want to start cooking?", "Get ready to cook...!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    remove_from_stored();
                    this.Close();
                    icebox1.mainState();
                }
            }
            else if (recipe_list.Count == 0)
            {
                MessageBox.Show("You have to select a recipe to start cooking!", "Error: You don't have select a recipe!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("You don't have all the products for the selected recipe!", "Error: Products are missing!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void recipes_FormClosed(object sender, FormClosedEventArgs e)
        {
            //icebox1.recipes_is_open = false;
            if (!icebox1.recipes_is_waiting)
            {
                icebox1.temp_list.Clear();
                //MessageBox.Show("cleared_no_recipe_is_waiting"); //-->debugging
            }            
        }

        //
        //help button
        //
        private void button3_Click(object sender, EventArgs e)
        {
            help help_form1 = new help("recipes");
            help_form1.ShowDialog();
        }
    }
}

/*
Milk 1.5% fat (1lt)
Milk 3.5% fat (1lt)
Yoghurt (1kg)
Gouda Cheese (500g)
Feta Cheese (500g)
Eggs (15 pieces)
Orange Juice (1lt)
Green Cola (1.5lt)
Ketchup (300g)
Mustard (300g)
Mayonnaise (295ml)
*/
/*
Fish
Pork
Beef
Chicken
Broccoli
Lettuce
Tomatoes
Eggplants
Carrots
Bananas
Apples
Oranges
*/
