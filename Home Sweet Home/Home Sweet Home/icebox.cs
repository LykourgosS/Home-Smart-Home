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
    public partial class icebox : Form
    {
        menu_form menu;
        public bool recipes_is_waiting = false;                       //used for recipes form
        public string selected_recipe = null;                   //
        public List<item> stored_food = new List<item>();
        public List<item> temp_list = new List<item>();
        public List<item> scarse_food = new List<item>();       //used for get food to eat form
        public bool order_completed = false;                    //used for supermarket selection
        public bool got_food = false;                           //used for get food to eat form
        public decimal min = 1;                                 //used for settings
        public decimal threshold = 3;                           //
        public decimal default_q = 2;                           //
        private int ticks_counter = 0;
        public bool select_all = true;
        public string help_for;

        public icebox(menu_form f)
        {
            InitializeComponent();
            menu = f;
        }

        private void icebox_FormClosed(object sender, FormClosedEventArgs e)
        {
            menu.open_devices[0] = new Device(new icebox(menu), false);
        }

        private void icebox_Load(object sender, EventArgs e)
        {
            comboBox1.DropDownHeight = comboBox1.ItemHeight * 7;
            comboBox2.DropDownHeight = comboBox2.ItemHeight * 7;
            mainState();

            //main screen
            toolTip1.SetToolTip(this.button6, "Fridge's Inside");
            toolTip1.SetToolTip(this.button7, "Order");
            toolTip1.SetToolTip(this.button8, "Temperature & Humidity");
            toolTip1.SetToolTip(this.button9, "Recipes");
            toolTip1.SetToolTip(this.button10, "Apps");
            toolTip1.SetToolTip(this.button11, "Settings");
            toolTip1.SetToolTip(this.progressBar3, "Click to see the available products...");

            //Fridge's inside
            toolTip1.SetToolTip(this.button4, "Take products to eat...");
            toolTip1.SetToolTip(this.button5, "Order more products...");
            toolTip1.SetToolTip(this.button17, "Empty fridge");
            toolTip1.SetToolTip(this.progressBar2, "Click to see the available products...");
            toolTip1.SetToolTip(this.pictureBox1, "Click to open/close fridge's camera...");

            //Apps
            toolTip1.SetToolTip(this.button12, "Calendar");
            toolTip1.SetToolTip(this.button13, "Radio");
            toolTip1.SetToolTip(this.button14, "Weather");
            toolTip1.SetToolTip(this.button15, "Go back");

            //Order
            toolTip1.SetToolTip(this.button1, "Add product to list");
            toolTip1.SetToolTip(this.button2, "Remove product from list");
            toolTip1.SetToolTip(this.button3, "Submit order");
            toolTip1.SetToolTip(this.button16, "Cancel order");
            toolTip1.SetToolTip(this.progressBar1, "Click to see the available products...");
        }

        //
        //
        //
        /************************************************************************************/
        private void fridge_image(string path)
        {
            pictureBox1.ImageLocation = path;
        }

        private void set_fridge_image(bool opened)
        {
            if (opened)
            {
                if (fridge_capacity() == 0)
                {
                    fridge_image(@".\images\empty_fridge.jpg");
                }
                else if (fridge_capacity() <= 15)
                {
                    fridge_image(@".\images\semiFull_fridge.jpg");
                }
                else if (fridge_capacity() <= 35)
                {
                    fridge_image(@".\images\more_than_semiFull_fridge.jpg");
                }
                else
                {
                    fridge_image(@".\images\full_fridge.jpg");
                }
            }
            else
            {
                pictureBox1.Image = null;
            }
        }

        private void update_fridge_display()
        {
            progressBar2.Value = Convert.ToInt32(fridge_capacity());
            set_fridge_image(fridge_is_open);
            danger_button_visibility();
        }

        public decimal temp_fridge_capacity()
        {
            decimal tfc = 0;
            foreach (item item1 in temp_list)
            {
                tfc += item1.quantity;
            }
            return tfc;
        }

        public decimal fridge_capacity()
        {
            decimal fc = 0;
            foreach (item item1 in stored_food)
            {
                fc += item1.quantity;
            }
            return fc;
        }

        private string little_food()
        {
            string food_names = "";
            scarse_food.Clear();
            foreach (item item1 in stored_food)
            {
                if (item1.quantity < threshold)
                {
                    scarse_food.Add(item1);
                    food_names += "• " + item1.name + Environment.NewLine;
                }
            }
            return food_names;
        }

        private void add_from_scarceList_to_tempList()
        {
            foreach (item item1 in scarse_food)
            {
                item1.quantity = default_q;
                add_to_order_list(item1.name, item1.quantity, item1.category, true);
            }
        }

        private void shopping_reminder()
        {
            if (fridge_capacity() <= 10)
            {
                DialogResult result = MessageBox.Show("Your fridge has very little or no food in it!" + Environment.NewLine + "Do you want to order them online now?", "Warning: Little food in the fridge!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    this.Activate();
                    orderToolStripMenuItem.PerformClick();
                }
            }
        }

        private void reminder()
        {
            timer1.Enabled = false;         
            string food_names = little_food();
            if (scarse_food.Count > 0)
            {
                if (fridge_capacity() + scarse_food.Count * default_q <= 40)
                {
                    DialogResult result = MessageBox.Show("The following product(s) have passed the quantity threshold:" + Environment.NewLine + food_names + "Do you want to order them online now?", "Warning: Little food in the fridge!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        orderToolStripMenuItem.PerformClick();
                        add_from_scarceList_to_tempList();
                        update_listview(listView1);
                    }
                }                
            }
            else
            {
                shopping_reminder();
            }
            if (!groupBox1.Visible)
            {
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ticks_counter += 1;
            //label15.Text = ticks_counter.ToString(); //recipes_is_waiting.ToString();
            //label18.Text = select_all.ToString();
            //textToolStripMenuItem.Text = ticks_counter.ToString();
            update_fridge_display();
            if (ticks_counter == min * 3795)
            {
                reminder();                
                ticks_counter = 0;
            }
        }
        //
        //
        //
        /************************************************************************************/
        //
        //  <main screen
        //
        public void mainState()
        {
            this.Text = "Icebox®: " + mainScreenToolStripMenuItem.Text;
            //this.Size = new System.Drawing.Size(684, 649);
            help_for = "main_screen";
            groupBox1.Visible = false;
            groupBox2.Visible = false;            
            groupBox3.Visible = true;
            groupBox4.Visible = false;
            groupBox3.Location = new Point(6, 33);
            //groupBox3.Size = new System.Drawing.Size(642, 559);
            progressBar3.Value = Convert.ToInt32(fridge_capacity());
            timer1.Enabled = true;
            menuStrip1.Enabled = true;
            /*if (!recipes_is_waiting)    //<- there is need for this ??? (i think no... Actually no! Because when you click main while you are on order it clears the shopping list)
            {
                temp_list.Clear();
            }*/
            if (recipes_is_waiting)
            {
                recipesToolStripMenuItem.PerformClick();
            }
        }

        private void mainScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainState();
        }

        //
        //main screen progressbar
        //
        private void progressbar3_Click(object sender, EventArgs e)
        {
            show_list("fridge");
        }

        //
        //button6: inside button
        //
        private void button6_Click(object sender, EventArgs e)
        {
            insideToolStripMenuItem.PerformClick();
        }

        //
        //button7: order button
        //
        private void button7_Click(object sender, EventArgs e)
        {
            orderToolStripMenuItem.PerformClick();
        }

        //
        //button8: temperature & humidity button
        //
        private void button8_Click(object sender, EventArgs e)
        {
            temperatureHumidityToolStripMenuItem.PerformClick();
        }

        //
        //button9: recipes button
        //
        private void button9_Click(object sender, EventArgs e)
        {
            recipesToolStripMenuItem.PerformClick();
        }

        //
        //button9: apps button
        //
        private void button10_Click(object sender, EventArgs e)
        {
            appsToolStripMenuItem.PerformClick();
        }

        //
        //button11: settings button
        //
        private void button11_Click(object sender, EventArgs e)
        {
            settingsToolStripMenuItem.PerformClick();
        }
        //
        //  /main screen>
        //
        /************************************************************************************/
        //
        //  <inside the Fridge
        //

        private void insideState()
        {

            this.Text = "Icebox®: " + insideToolStripMenuItem.Text;
            //this.Size = new System.Drawing.Size(684, 649);
            help_for = "inside_fridge";
            groupBox1.Visible = false;
            groupBox2.Visible = true;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
            groupBox2.Location = new Point(6, 33);
            //groupBox2.Size = new System.Drawing.Size(642, 559);
            danger_button_visibility();
        }

        private void insideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insideState();
        }

        private void update_stored_food()
        {
            if (!got_food)
            {
                foreach (item item1 in temp_list)
                {
                    try
                    {
                        stored_food.Find(item => item.name == item1.name).quantity += item1.quantity;
                    }
                    catch
                    {
                        stored_food.Add(item1);
                    }
                }
            }
            else
            {
                //mainScreenToolStripMenuItem.PerformClick();   //for a reason that I don't remember now, after serving food used to go in main screen
                got_food = false;
            }
            temp_list.Clear();
            //MessageBox.Show("cleared_after_updating_stored"); //-->debugging
        }

        //
        //button4: serve food (of inside state)
        //
        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (fridge_capacity() > 0)
            {
                Form food_to_eat = new get_food_to_eat(this);
                food_to_eat.ShowDialog();
                timer1.Enabled = true;
                update_stored_food();
                update_fridge_display();
            }
            else
            {
                DialogResult result = MessageBox.Show("Ooops... the fridge is empty." + Environment.NewLine + "Do you want to order online now?", "Warning: Empty fridge!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    orderState();
                }
                else
                {
                    timer1.Enabled = true;
                }
                //MessageBox.Show("Ooops... The fridge is epmty.", "Epmty fridge!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //
        //button5: order button
        //
        private void button5_Click(object sender, EventArgs e)
        {
            if (fridge_is_full())
            {
                MessageBox.Show("There is no more space in the fridge. Free up space and then try placing a new order.", "Oops, no more space in the fridge!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                orderState();
            }
        }

        bool fridge_is_open = false;

        //
        //click on the fridge image
        //
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (fridge_is_open)
            {
                fridge_is_open = false;
            }
            else
            {
                fridge_is_open = true;
            }
        }

        public void show_list(string list_cat)
        {
            fridgeList fridge_food = new fridgeList(this, list_cat);
            fridge_food.Show();
        }

        private void progressBar2_Click(object sender, EventArgs e)
        {
            show_list("fridge");
        }

        //
        // danger button: empty fridge
        //
        private void button17_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to empty your fridge???" + Environment.NewLine + "(Warning: This action cannot be undone!!!)", "Warning: Empty fridge?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //message with the food that is gone now ;)
                stored_food.Clear();
            }
        }

        private void danger_button_visibility()
        {
            if (stored_food.Count != 0)
            {
                button17.Visible = true;
            }
            else
            {
                button17.Visible = false;
            }
        }
        //
        //  /inside the Fridge>
        //
        /************************************************************************************/
        //
        //  <Order
        //
        public void orderState()
        {
            this.Text = "Icebox®: " + orderToolStripMenuItem.Text;
            timer1.Enabled = false;
            //this.Size = new System.Drawing.Size(656, 649);
            help_for = "order";
            groupBox1.Location = new Point(6, 33);
            //groupBox1.Size = new System.Drawing.Size(614, 559);
            groupBox1.Visible = true;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
            button3.Enabled = false;
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            update_listview(listView1);
            if (recipes_is_waiting)
            {
                menuStrip1.Enabled = false;
                //button16.Visible = true;
            }
            else
            {
                menuStrip1.Enabled = true;
                //button16.Visible = false;
            }
        }

        private void orderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fridge_is_full())
            {
                MessageBox.Show("There is no more space in the fridge. Free up space and then try placing a new order.", "Oops, no more space in the fridge!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                orderState();
            }
        }

        private void groupBox1_VisibleChanged(object sender, EventArgs e)
        {
            if (groupBox1.Visible)
            {
                timer1.Enabled = false;
                recipesToolStripMenuItem.Enabled = false;
            }
            else
            {
                timer1.Enabled = true;
                recipesToolStripMenuItem.Enabled = true;
                //this.Invalidate();
                //Application.DoEvents();
                /*if (recipes_is_waiting)
                {
                    recipesToolStripMenuItem.PerformClick();
                }
                else
                {
                    temp_list.Clear();
                }*/
            }
        }

        //
        //order progressbar
        //
        private void progressBar1_Click(object sender, EventArgs e)
        {
            show_list("order");
            //MessageBox.Show(temp_list.Count.ToString() + recipes_is_waiting); //for testing
        }

        public bool fridge_is_full()
        {
            return (fridge_capacity() == 40);
        }

        public bool no_more_space_in_the_fridge()
        {
            return (fridge_capacity() + temp_fridge_capacity() > 40);
        }

        public decimal get_list_quantity(List<item> lst)
        {
            decimal q = 0;
            foreach (item item1 in lst)
            {
                q += item1.quantity;
            }
            return q;
        }

        private void disable_unwanted_controls(ComboBox cb_inactive)
        {
            numericUpDown1.Enabled = false;
            numericUpDown1.Value = 0;
            button1.Visible = false;
            cb_inactive.SelectedItem = null;
            cb_inactive.Enabled = false;
        }

        private void disable_unwanted_controls(ComboBox cb_inactive1, ComboBox cb_inactive2)
        {
            numericUpDown1.Enabled = false;
            numericUpDown1.Value = 0;
            button1.Visible = false;
            cb_inactive1.Enabled = false;
            cb_inactive2.Enabled = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                comboBox1.Enabled = true;
                disable_unwanted_controls(comboBox2);
            }
            else if (radioButton2.Checked)
            {
                comboBox2.Enabled = true;
                disable_unwanted_controls(comboBox1);
            }
            else
            {
                disable_unwanted_controls(comboBox1, comboBox2);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                comboBox2.Enabled = true;
                disable_unwanted_controls(comboBox1);
            }
            else if (radioButton1.Checked)
            {
                comboBox1.Enabled = true;
                disable_unwanted_controls(comboBox2);
            }
            else
            {
                disable_unwanted_controls(comboBox1, comboBox2);
            }
        }


        private void comboBox1_EnabledChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = null;
            if (radioButton1.Checked)
            {
                comboBox1.Text = "Select product ...";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                if (!comboBox1.SelectedItem.Equals(null))
                {
                    numericUpDown1.Enabled = true;
                    numericUpDown1.Maximum = 40 - (fridge_capacity() + temp_fridge_capacity());
                }
                else
                {
                    numericUpDown1.Enabled = false;
                }
            }
        }

        private void comboBox2_EnabledChanged(object sender, EventArgs e)
        {
            comboBox2.SelectedItem = null;
            if (radioButton2.Checked)
            {
                comboBox2.Text = "Select product ...";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                if (!comboBox2.SelectedItem.Equals(null))
                {
                    numericUpDown1.Enabled = true;
                    numericUpDown1.Maximum = 40 - (fridge_capacity() + temp_fridge_capacity());
                }
                else
                {
                    numericUpDown1.Enabled = false;
                }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value != 0)
            {
                button1.Visible = true;
            }
            else
            {
                button1.Visible = false;
            }
        }

        public void update_listview(ListView lstV)
        {
            lstV.Clear();
            foreach (item item1 in temp_list)
            {
                lstV.Items.Add(item1.detailsOfItem());
            }
            if (temp_list.Count > 0)
            {
                button3.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
            }
            progressBar1.Value = Convert.ToInt32(temp_fridge_capacity() + fridge_capacity());
            select_all_label_visibity();
        }

        //
        //add product (item) to shopping list
        //
        public void add_to_order_list(string name, decimal quantity, string pieceOrkg, bool show_message)
        {
            item item1 = new item(name, quantity, pieceOrkg);
            if (temp_fridge_capacity() + fridge_capacity() + quantity <= 40)
            {
                try
                {
                    temp_list.Find(item => item.name == name).quantity += quantity;
                }
                catch
                {
                    temp_list.Add(item1);
                }
            }
            else
            {
                if (show_message)
                {
                    MessageBox.Show("There is no more space in the fridge. Free up space and then try placing a new order.", "Oops, no more space in the fridge!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }                
            }
        }

        //
        //button1: add a product to shopping list
        //
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                add_to_order_list(comboBox1.SelectedItem.ToString(), numericUpDown1.Value, "piece(s)", true);
                radioButton1.Checked = false;
            }
            else
            {
                add_to_order_list(comboBox2.SelectedItem.ToString(), numericUpDown1.Value, "kg(s)", true);
                radioButton2.Checked = false;
            }
            update_listview(listView1);
            update_fridge_display();
        }

        //
        //to select what to order
        //
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

        //
        //button2: remove product from shopping list
        //
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = listView1.CheckedItems.Count; i > 0; i--)
            {
                temp_list.Remove(temp_list.Find(item => item.detailsOfItem() == listView1.CheckedItems[i - 1].Text));
            }
            update_listview(listView1);
            button2.Visible = false;
        }

        private void put_new_food_in_the_fridge()
        {
            foreach (item new_item in temp_list)
            {
                try
                {
                    stored_food.Find(item => item.name == new_item.name).quantity += new_item.quantity;
                }
                catch
                {
                    stored_food.Add(new_item);
                }
            }
        }

        //
        //button3: submit order
        //
        private void button3_Click(object sender, EventArgs e)
        {
            Form select_superMarket = new supermarket_selection(this);
            select_superMarket.ShowDialog();
            if (order_completed)
            {
                put_new_food_in_the_fridge();
                order_completed = false;
                mainState();
                //temp_list.Clear(); //TODO: this is the one that kept clearing the temp_list, I still haven't understand the reason!!!
                //MessageBox.Show("cleared_because_submitting"); //-->debugging
            }
            select_all = true;            
        }

        //
        //label17: (de)select all products...
        //
        private void label17_MouseEnter(object sender, EventArgs e)
        {
            label17.Font = new Font(label17.Font, FontStyle.Underline);
        }

        private void label17_MouseLeave(object sender, EventArgs e)
        {
            label17.Font = new Font(label17.Font, FontStyle.Regular);
        }

        private void select_all_label_visibity()
        {
            if (temp_list.Count >= 2)
            {
                label17.Visible = true;
            }
            else
            {
                label17.Visible = false;
            }
        }

        private void label17_VisibleChanged(object sender, EventArgs e)
        {
            if (!label17.Visible)
            {
                label17.Text = "Select all...";
                select_all = true;
            }
        }

        private void label17_Click(object sender, EventArgs e)
        {
            if (select_all)
            {
                select_all_items(listView1);                
            }
            else
            {
                deselect_all_items(listView1);
            }
        }

        public void select_all_items(ListView lstV)
        {
            for (int i = 0; i < lstV.Items.Count; i++)
            {
                lstV.Items[i].Checked = true;
            }
            label17.Text = "Deselect all...";
            select_all = false;
        }

        public void deselect_all_items(ListView lstV)
        {
            for (int i = 0; i < lstV.Items.Count; i++)
            {
                lstV.Items[i].Checked = false;
            }
            label17.Text = "Select all...";
            select_all = true;
        }

        //
        //button16 : cancel order of products
        //
        private void button16_Click_1(object sender, EventArgs e)
        {
            mainState();
        }
        //
        //  /Order>
        //
        /************************************************************************************/
        //
        //  <Temperature&Humidity
        //
        private void temperatureHumidityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            temp_humid temp_humid1 = new temp_humid();
            temp_humid1.ShowDialog();
            timer1.Enabled = true;
        }
        //
        //  Temperature&Humidity/>
        //
        /************************************************************************************/
        //
        //  <Recipes
        //
        private void recipesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;             
            recipes recipes = new recipes(this);
            recipes.ShowDialog();
            if (!groupBox1.Visible)
            {
                timer1.Enabled = true;
            }
        }
        //
        //  Recipes/>
        //
        /************************************************************************************/
        //
        //  <Apps
        //
        private void appsState()
        {
            this.Text = "Icebox®: " + appsToolStripMenuItem.Text;
            help_for = "apps";
            groupBox1.Visible = false;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Text = "Weather, Radio and Calendar";
            groupBox4.Visible = true;
            groupBox4.Location = new Point(6, 33);
            tableLayoutPanel1.Location = new Point(41, 128);
            tableLayoutPanel1.Visible = true;
            monthCalendar1.Visible = false;
            monthCalendar1.Location = new Point(156, 105);
        }
        private void appsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            appsState();
        }

        //
        //button15: go back to main screen
        //
        private void button15_Click(object sender, EventArgs e)
        {
            if (tableLayoutPanel1.Visible)
            {
                mainScreenToolStripMenuItem.PerformClick();
            }
            else
            {
                appsToolStripMenuItem.PerformClick();
            }
        }

        //
        //button12: calendar
        //
        private void gotoSite(string url)
        {
            System.Diagnostics.Process.Start(url);
        }        

        //
        //button14: weather
        //
        private void button14_Click(object sender, EventArgs e)
        {
            string weather_url = "https://www.google.gr/search?q=weather&oq=wea&aqs=chrome.0.69i59l2j69i57j69i60j69i61j0.3581j0j7&sourceid=chrome&ie=UTF-8";
            gotoSite(weather_url);
        }

        //
        //button13: radio
        //
        private void button13_Click(object sender, EventArgs e)
        {
            string radio_url = "http://peradio.com/";
            gotoSite(radio_url);
        }

        //
        //button12: calendar
        //
        private void button12_Click(object sender, EventArgs e)
        {
            groupBox4.Text = "Calendar";
            tableLayoutPanel1.Visible = false;
            monthCalendar1.Visible = true;
        }
        //
        //  Apps/>
        //
        /************************************************************************************/
        //
        //  <Settings
        //
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            settings settings1 = new settings(this);
            settings1.ShowDialog();
            ticks_counter = 0;
            if (!groupBox1.Visible)
            {
                timer1.Enabled = true;
            }            
        }
        //
        //  Settings/>
        //
        /************************************************************************************/
        //
        //  <Help
        //
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            help help1 = new help(help_for);
            help1.ShowDialog();
        }
        //
        //  Help/>
        //
    }

    public class item
    {
        public string name;
        public decimal quantity;
        public string category;

        public item(string n, decimal q, string pieceOrkg)
        {
            name = n;
            quantity = q;
            category = pieceOrkg; 
        }

        public string detailsOfItem()
        {
            return name + " : " + quantity.ToString() + " " + category;
        }
    }
}
