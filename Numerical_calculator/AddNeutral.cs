﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KIMI_Sim
{
    public partial class AddNeutral : Form
    {
        public List<Items> Items { get; set; }
        public bool Data_type { get; set; }
        public bool Remove_data { get; set; }



        public AddNeutral()
        {
            InitializeComponent();
        }
        public void ShowCollection()
        {
            foreach (Items item in Items)
            {
                if (item.cation == false)
                {
                    this.listBox1.Items.Add(item);
                }
            }
            if (Remove_data)
            {
                button2.Text = "Remove";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Cancle
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Remove_data == false)
            {
                //Add
                if (Data_type == true)
                { // product
                    Main.listBox_products.Items.Add((Items)listBox1.SelectedItem);
                }
                else
                { // reactant
                    Main.listBox_reactants.Items.Add((Items)listBox1.SelectedItem);
                }
            }
            else
            {
                //Remove
                if (Data_type == true)
                { // product
                    Main.listBox_products.Items.Remove((Items)listBox1.SelectedItem);
                }
                else
                { // reactant
                    Main.listBox_reactants.Items.Remove((Items)listBox1.SelectedItem);
                }
            }
            Main.listbox_change();
            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ((Items)listBox1.SelectedItem != null)
                {
                    button2.Enabled = true;
                }
                else
                {
                    button2.Enabled = false;
                }
            }
            catch
            {
                MessageBox.Show("Neutral is not selected!");
            }
        }

        private void AddNeutral_Load(object sender, EventArgs e)
        {
            this.Location = this.Owner.Location;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if ((Items)listBox1.SelectedItem != null)
            {
                if (Remove_data == false)
                {
                    //Add
                    if (Data_type == true)
                    { // product
                        Main.listBox_products.Items.Add((Items)listBox1.SelectedItem);
                    }
                    else
                    { // reactant
                        Main.listBox_reactants.Items.Add((Items)listBox1.SelectedItem);
                    }
                }
                else
                {
                    //Remove
                    if (Data_type == true)
                    { // product
                        Main.listBox_products.Items.Remove((Items)listBox1.SelectedItem);
                    }
                    else
                    { // reactant
                        Main.listBox_reactants.Items.Remove((Items)listBox1.SelectedItem);
                    }
                }
                Main.listbox_change();
                this.Close();
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if ((Items)listBox1.SelectedItem != null)
                {
                    if (Remove_data == false)
                    {
                        //Add
                        if (Data_type == true)
                        { // product
                            Main.listBox_products.Items.Add((Items)listBox1.SelectedItem);
                        }
                        else
                        { // reactant
                            Main.listBox_reactants.Items.Add((Items)listBox1.SelectedItem);
                        }
                    }
                    else
                    {
                        //Remove
                        if (Data_type == true)
                        { // product
                            Main.listBox_products.Items.Remove((Items)listBox1.SelectedItem);
                        }
                        else
                        { // reactant
                            Main.listBox_reactants.Items.Remove((Items)listBox1.SelectedItem);
                        }
                    }
                    Main.listbox_change();
                    this.Close();
                }
            }
        }
    }
}
