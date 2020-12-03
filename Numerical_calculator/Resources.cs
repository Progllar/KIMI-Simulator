using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace KIMI_Sim
{
    public partial class Resources : Form
    {
        public List<Items> ItemColection { get; set; }
        private int j = 0;
        public static rate_functions item_mobility;
        bool item_is_selected = false;

        public Resources()
        {
            InitializeComponent();
            toolTip1.SetToolTip(this.label14, "Reduced ion mobility, K0, (in cm^2/Vs) is a function of reduced electric field (E/N). \nPlease create a representative mobility function or select a constatn value (A).");
            toolTip1.SetToolTip(this.label13, "Diffusion coeficient (in cm^2/s) shall be estimated for standard experimental condition, \n1 Torr (1.33 mbar) and 300 K. Diffusion is then corrected according to the instrumental settings. \nDiffusion coeficient can be estimated based on the ion mass.");
        }

        public void proceed()
        {
            foreach (Items Item in ItemColection)
            {
                if (Item.cation && checkBox1.Checked)
                {
                    this.listBox1.Items.Add(Item);
                }
                if (!Item.cation && checkBox2.Checked)
                {
                    this.listBox1.Items.Add(Item);
                }
            }
        }

        public static void use_new_mobility()
        {
            // insert data to textbox
            textBox_mobility.Text = item_mobility.representation;
        }

        private void button_cancle_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            Main.save_settings();
            Main.load_settings();
            this.Dispose();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            item_is_selected = true;
            int i = 0;
            foreach (Items Item in ItemColection)
            {
                if (Item.name == listBox1.GetItemText(listBox1.SelectedItem))
                {
                    textBox_name.Text = Item.name;
                    textBox_formula.Text = Item.formula;
                    textBox_shortname.Text = Item.s_name;
                    if (Item.concentration > 100)
                    {
                        textBox_concentration.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        textBox_concentration.Text = Item.concentration.ToString();
                    }
                    textBox_mobility.Text = Item.mobility.representation;
                    item_mobility = Item.mobility;
                    textBox_mass.Text = Item.mass.ToString();
                    textBox_difusion.Text = Item.diffusion.ToString();
                    if (Item.cation)
                    {
                        radioButton_cation.Checked = true;
                        radioButton_neutral.Checked = false;
                        textBox_difusion.Enabled = true;
                        button3.Enabled = true;
                        button4.Enabled = true;
                    }
                    else
                    {
                        radioButton_cation.Checked = false;
                        radioButton_neutral.Checked = true;
                        textBox_difusion.Enabled = false;
                        button3.Enabled = false;
                        button4.Enabled = false;
                    }
                    i++;
                }
            }
            item_is_selected = false;
        }

        private void try_to_enable_Apply()
        {
            if (
                (textBox_name.Text != "" &&
                textBox_formula.Text != "" &&
                textBox_shortname.Text != "" &&
                textBox_concentration.Text != "" &&
                textBox_mobility.Text != "" &&
                item_mobility != null &&
                textBox_difusion.Text != "" &&
                radioButton_cation.Checked) || 
                (textBox_name.Text != "" &&
                textBox_formula.Text != "" &&
                textBox_shortname.Text != "" &&
                textBox_concentration.Text != "" &&
                radioButton_neutral.Checked)
                )
            {
                button_ApplyElement.Enabled = true;
            }
            else
            {
                button_ApplyElement.Enabled = false;
            }
        }

        private void button_ClearElement_Click(object sender, EventArgs e)
        {
            textBox_name.Text = "";
            textBox_formula.Text = "";
            textBox_shortname.Text = "";
            textBox_concentration.Text = "";
            textBox_mobility.Text = "Select a new function";
            textBox_mass.Text = "";
            textBox_difusion.Text = "";
            item_mobility = null;
            textBox_difusion.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            j = 0;
        }

        private void button_DeleteElement_Click(object sender, EventArgs e)
        {
            Items removed_item = new Items();
            bool nasiel_sa = false;
            foreach (Items Item in ItemColection)
            {
                if (Item.name == textBox_name.Text)
                {
                    removed_item = Item;
                    nasiel_sa = true;
                }
            }
            if (!nasiel_sa)
            {
                MessageBox.Show("Item not found!");
            }
            else
            {
                textBox_name.Text = "";
                textBox_formula.Text = "";
                textBox_shortname.Text = "";
                textBox_concentration.Text = "";
                textBox_mobility.Text = "Select a new function";
                item_mobility = null;
                textBox_mass.Text = "";
                textBox_difusion.Text = "";
                textBox_difusion.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                ItemColection.Remove(removed_item);
                listBox1.Items.Remove(removed_item);
            }
        }

        private void button_ApplyElement_Click(object sender, EventArgs e)
        {
            bool pokracuj = true;
            bool jetu = false;
            foreach (Items Item in ItemColection)
            {
                if (Item.name == textBox_name.Text)
                {
                    jetu = true;
                }
            }
            if (jetu)
            {
                if (MessageBox.Show("Do you want to continue?", "Name already exist.",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.No)
                {
                    pokracuj = false;
                }
            }
            if (pokracuj)
            {
                if (jetu)
                {
                    Items removed_item = new Items();
                    foreach (Items Item in ItemColection)
                    {
                        if (Item.name == textBox_name.Text)
                        {
                            removed_item = Item;
                        }
                    }
                    ItemColection.Remove(removed_item);
                    listBox1.Items.Remove(removed_item);
                }
                Items new_item = new Items();
                new_item.name = textBox_name.Text;
                new_item.formula = name_modification(textBox_formula.Text);
                new_item.s_name = name_modification(textBox_shortname.Text);
                new_item.concentration = Convertor(textBox_concentration.Text);
                new_item.mobility = item_mobility;
                MwtWinDll.MolecularWeightCalculator objMwtWin = new MwtWinDll.MolecularWeightCalculator();
                try
                {
                    new_item.mass = objMwtWin.ComputeMass(Convert_formula(new_item.formula));
                }
                catch { MessageBox.Show("Unable to read formula and calculate mass!"); }
                new_item.diffusion = Convertor(textBox_difusion.Text);
                if (radioButton_cation.Checked)
                {
                    new_item.cation = true;
                }
                else
                {
                    new_item.cation = false;
                    new_item.mobility = new Constant(0.0, true);
                    new_item.diffusion = 0.0;
                }
                ItemColection.Add(new_item);
                this.listBox1.Items.Add(new_item);
            }
        }

        private string name_modification(string S)
        {
            string s = "";
            foreach(char ch in S)
            {
                bool character = false;
                if(ch == '1')
                {
                    s += "₁";
                    character = true;
                }
                if (ch == '2')
                {
                    s += "₂";
                    character = true;
                }
                if (ch == '3')
                {
                    s += "₃";
                    character = true;
                }
                if (ch == '4')
                {
                    s += "₄";
                    character = true;
                }
                if (ch == '5')
                {
                    s += "₅";
                    character = true;
                }
                if (ch == '6')
                {
                    s += "₆";
                    character = true;
                }
                if (ch == '7')
                {
                    s += "₇";
                    character = true;
                }
                if (ch == '8')
                {
                    s += "₈";
                    character = true;
                }
                if (ch == '9')
                {
                    s += "₉";
                    character = true;
                }
                if (ch == '0')
                {
                    s += "₀";
                    character = true;
                }
                if (ch == '+')
                {
                    s += "⁺";
                    character = true;
                }
                if(!character)
                {//normal letter
                    s += ch.ToString();
                }
            }
            return s;
        }

        private double Convertor(string Str)
        {
            double dou = 0;
            string d = "";
            try
            {
                dou = Convert.ToDouble(Str);
            }
            catch
            {
                d = "";
                foreach (char ch in Str)
                {
                    if (ch == ',')
                    {
                        d += ".";
                    }
                    else
                    {
                        d += ch.ToString();
                    }
                }
                try
                {
                    dou = Convert.ToDouble(d);
                }
                catch
                {
                    d = "";
                    foreach (char ch in Str)
                    {
                        if (ch == '.')
                        {
                            d += ",";
                        }
                        else
                        {
                            d += ch.ToString();
                        }
                    }
                    try
                    {
                        dou = Convert.ToDouble(d);
                    }
                    catch
                    {
                        MessageBox.Show("Unable to convert string to double !");
                    }
                }
            }
            return dou;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox_filter.Text = "";
            checkBox1.Checked = true;
            checkBox2.Checked = true;
            apply_filter();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            apply_filter();
        }

        private void apply_filter()
        {
            this.listBox1.Items.Clear();
            if (textBox_filter.Text == "")
            {
                foreach (Items Item in ItemColection)
                {
                    if ((Item.cation && checkBox1.Checked) || (!Item.cation && checkBox2.Checked))
                    {
                        this.listBox1.Items.Add(Item);
                    }
                }
            }
            else
            {
                foreach (Items Item in ItemColection)
                {
                    if ((Item.cation && checkBox1.Checked) || (!Item.cation && checkBox2.Checked))
                    {
                        if (Item.name.Contains(textBox_filter.Text)) //  System.StringComparison.CurrentCultureIgnoreCase
                        {
                            this.listBox1.Items.Add(Item);
                        }
                    }
                }
            }
        }

        private void Resources_Load(object sender, EventArgs e)
        {
            this.Location = this.Owner.Location;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            apply_filter();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            apply_filter();
        }

        private void textBox_filter_TextChanged(object sender, EventArgs e)
        {
            apply_filter();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Rate_Konstatn_TypeOf new_rate_typeof = new Rate_Konstatn_TypeOf();
            new_rate_typeof.Text = "Select a function for ion mobility";
            new_rate_typeof.Show(this);
            new_rate_typeof.proceed(item_mobility, true, true, true);
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            try_to_enable_Apply();
        }

        private void textBox_formula_TextChanged(object sender, EventArgs e)
        {
            try_to_enable_Apply();
        }

        private void textBox_shortname_TextChanged(object sender, EventArgs e)
        {
            try_to_enable_Apply();
        }

        private void textBox_mobility_TextChanged(object sender, EventArgs e)
        {
            try_to_enable_Apply();
        }

        private void textBox_concentration_TextChanged(object sender, EventArgs e)
        {
            try_to_enable_Apply();
        }

        private void textBox_mass_TextChanged(object sender, EventArgs e)
        {
            try_to_enable_Apply();
        }

        private void textBox_difusion_TextChanged(object sender, EventArgs e)
        {
            try_to_enable_Apply();
        }

        private void radioButton_neutral_TextChanged(object sender, EventArgs e)
        {
            try_to_enable_Apply();
        }

        private string Convert_formula(string formula)
        {
            string _formula = "";
            foreach (char ch in formula)
            {
                bool character = false;
                if (ch == '₁')
                {
                    _formula += "1";
                    character = true;
                }
                if (ch == '₂')
                {
                    _formula += "2";
                    character = true;
                }
                if (ch == '₃')
                {
                    _formula += "3";
                    character = true;
                }
                if (ch == '₄')
                {
                    _formula += "4";
                    character = true;
                }
                if (ch == '₅')
                {
                    _formula += "5";
                    character = true;
                }
                if (ch == '₆')
                {
                    _formula += "6";
                    character = true;
                }
                if (ch == '₇')
                {
                    _formula += "7";
                    character = true;
                }
                if (ch == '₈')
                {
                    _formula += "8";
                    character = true;
                }
                if (ch == '₉')
                {
                    _formula += "9";
                    character = true;
                }
                if (ch == '₀')
                {
                    _formula += "0";
                    character = true;
                }
                if (ch == '⁺')
                {
                    character = true;
                }
                if (!character)
                {//normal letter
                    _formula += ch.ToString();
                }
            }
            return _formula;
        }

        private void radioButton_neutral_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton_neutral.Checked == true && item_is_selected == false)
            {
                textBox_difusion.Text = "NaN";
                textBox_difusion.Enabled = false;
                textBox_mobility.Text = "NaN";
                button3.Enabled = false;
                button4.Enabled = false;
            }
        }

        private void radioButton_cation_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_cation.Checked == true && item_is_selected == false)
            {
                textBox_difusion.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                textBox_difusion.Text = "0";
                textBox_mobility.Text = "Select a new function";          
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string present_formula = name_modification(textBox_formula.Text);
                MwtWinDll.MolecularWeightCalculator objMwtWin = new MwtWinDll.MolecularWeightCalculator();
                try
                {
                    textBox_mass.Text = objMwtWin.ComputeMass(Convert_formula(present_formula)).ToString();
                }
                catch { MessageBox.Show("Unable to read formula and calculate mass!"); }
                double exact_mass = Convert.ToDouble(textBox_mass.Text);
                double estimated_diffusion = 140 * Math.Log(383 / exact_mass);
                textBox_difusion.Text = estimated_diffusion.ToString("0.00", CultureInfo.InvariantCulture);
            }
            catch
            {

            }
        }
    }
}
