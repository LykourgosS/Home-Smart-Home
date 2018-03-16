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
    public partial class lights_out : Form
    {
        menu_form menu;
        List<Rooms> lst_of_rooms = new List<Rooms>();

        public lights_out(menu_form f)
        {
            InitializeComponent();
            menu = f;
        }

        private void closet_FormClosed(object sender, FormClosedEventArgs e)
        {
            menu.open_devices[1] = new Device(new lights_out(menu), false);
        }

        private Rooms room_exists(string str)
        {
            Rooms room1 = lst_of_rooms.Find(room => room.name == str);
            if (room1 != null)
            {
                return room1;
            }
            else
            {
                Rooms new_room = new Rooms(str);
                lst_of_rooms.Add(new_room);
                return new_room;
            }
        }

        private void set_room()
        {
            if (room_exists(comboBox1.SelectedItem.ToString()).on)
            {
                pictureBox1.ImageLocation = room_exists(comboBox1.SelectedItem.ToString()).img_path();
                button1.BackgroundImage = Image.FromFile(@".\images\on.jpg");
            }
            else
            {
                pictureBox1.ImageLocation = room_exists(comboBox1.SelectedItem.ToString()).img_path1();
                button1.BackgroundImage = Image.FromFile(@".\images\off.jpg");
            }
            button1.Visible = true;
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            set_room();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Rooms room1 = lst_of_rooms.Find(room => room.name == comboBox1.SelectedItem.ToString());
            if (room1.on)
            {
                room1.on = false;
            }
            else
            {
                room1.on = true;
            }
            set_room();
        }
    }

    class Rooms
    {
        public string name;
        public bool on = false;

        public Rooms(string n)
        {
            name = n;
        }

        public string img_path()
        { 
            return @".\images\" + name + ".jpg";
        }

        public string img_path1()
        {
            return @".\images\" + name + "1.jpg";
        }
    }
}
