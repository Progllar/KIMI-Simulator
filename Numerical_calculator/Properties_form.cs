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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KIMI_Sim
{
    public partial class Properties_form : Form
    {
        private bool start = false;
        public Properties_form()
        {
            InitializeComponent();
            start = true;
            if (Main.Time_duration == 0)
            {
                textBox1.Text = "300";
            }
            else
            {
                textBox1.Text = Main.Time_duration.ToString();
            }
            if (Main.NumberOfSteps == 0)
            {
                textBox2.Text = "100";
            }
            else
            {
                textBox2.Text = Main.NumberOfSteps.ToString();
            }
            if (Main.Distance == 0)
            {
                textBox3.Text = "5";
            }
            else
            {
                textBox3.Text = Main.Distance.ToString();
            }
            if (Main.Ion_velociy == 0)
            {
                textBox4.Text = "1000";
            }
            else
            {
                textBox4.Text = Main.Ion_velociy.ToString();
            }
            if (Main.Radius == 0)
            {
                textBox16.Text = "1";
            }
            else
            {
                textBox16.Text = Main.Radius.ToString();
            }
            textBox5.Text = Main.Gauss_signa.ToString();
            textBox6.Text = Main.Temperature.ToString();
            textBox7.Text = Main.Pressure.ToString();
            textBox15.Text = Main.Gauss_density.ToString();
            if (Main.Infinite_system)
            {
                checkBox2.Checked = true;
            }
            if (Main.Diffusion)
            {
                this.checkBox2.Enabled = true;
                this.textBox16.Enabled = true;
                checkBox1.Checked = true;
            }
            if (Main.calc_type)
            {
                radioButton1.Checked = true;
                textBox1.Enabled = true;
                textBox3.Enabled = false;
            }
            else
            {
                radioButton2.Checked = true;
                textBox1.Enabled = false;
                textBox3.Enabled = true;
            }
            foreach(Items Item in Main.ItemColection)
            {
                if(Item.cation == false)
                {
                    comboBox1.Items.Add(Item);
                }
            }
            comboBox1.Text = Main.carrier_gas_type.ToString();
            label65.Text = Lambda(checkBox2.Checked).ToString("0.000", CultureInfo.InvariantCulture);
            start = false;
            Carrier_gas_info();
            Electric_field_info();
        }

        private void button1_Click(object sender, EventArgs e)
            {
            calculate_prop();
            Main.Distance = Main.Convertor(textBox3.Text);
            Main.Ion_velociy = Main.Convertor(textBox4.Text);
            Main.Time_duration = Main.Convertor(textBox1.Text);
            Main.NumberOfSteps = Main.Convertor(textBox2.Text);
            Main.Gauss_signa = Main.Convertor(textBox5.Text);
            Main.Temperature = Main.Convertor(textBox6.Text);
            Main.Pressure = Main.Convertor(textBox7.Text);
            Main.Diffusion = this.checkBox1.Checked;
            Main.Gauss_density = Main.Convertor(textBox15.Text);
            Main.Radius = Main.Convertor(textBox16.Text);
            Main.calc_type = radioButton1.Checked;
            Main.Infinite_system = checkBox2.Checked;
            foreach (Items Item in Main.ItemColection)
            {
                if(Item.name == comboBox1.Text)
                {
                    Main.carrier_gas_type = Item;
                    carrier_mass = Item.mass;
                }
            }
            Main.save_settings();
            this.Dispose();
        }

        private void button_cancle_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox1.Enabled = true;
                textBox3.Enabled = false;
            }
            else
            {
                textBox1.Enabled = false;
                textBox3.Enabled = true;
            }
        }
        private void calculate_prop()
        {
            if (radioButton1.Checked)
            {
                try
                {
                    textBox3.Text = (Main.Convertor(textBox1.Text) * Main.Convertor(textBox4.Text) / 1000000).ToString();
                }
                catch { textBox3.Text = "NaN"; }
            }
            else
            {
                try
                {
                    textBox1.Text = ((Main.Convertor(textBox3.Text) * 1000000) / Main.Convertor(textBox4.Text)).ToString();
                }
                catch { textBox1.Text = "NaN"; }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                calculate_prop();
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                calculate_prop();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            calculate_prop();
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            calculate_prop();
        }

        private void Properties_form_Load(object sender, EventArgs e)
        {
            this.Location = this.Owner.Location;
        }

        private bool calculate_presure = true;
        private void textBox7_TextChanged(object sender, EventArgs e) //Torr
        {
            if (calculate_presure)
            {
                calculate_presure = false;
                textBox10.Text = (Main.Convertor(textBox7.Text) * 1.33322).ToString();
                calculate_presure = true;
            }
            Carrier_gas_info();
            Electric_field_info();
        }

        private void textBox10_TextChanged(object sender, EventArgs e) //mbar
        {
            if (calculate_presure)
            {
                calculate_presure = false;
                textBox7.Text = (Main.Convertor(textBox10.Text) * 0.75).ToString();
                calculate_presure = true;
            }
        }

        private bool calculate_temperature = true;

        private void textBox6_TextChanged(object sender, EventArgs e) //K
        {
            if (calculate_temperature)
            {
                calculate_temperature = false;
                textBox11.Text = (Main.Convertor(textBox6.Text) - 273.15).ToString();
                calculate_temperature = true;
            }
            Carrier_gas_info();
            Electric_field_info();
        }

        private void textBox11_TextChanged(object sender, EventArgs e) //C
        {
            if (calculate_temperature)
            {
                calculate_temperature = false;
                textBox6.Text = (Main.Convertor(textBox11.Text) + 273.15).ToString();
                calculate_temperature = true;
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            textBox9.Text = (Main.Convertor(textBox8.Text)*1.5).ToString();
            Carrier_gas_info();
            Electric_field_info();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
            label65.Text = Lambda(checkBox2.Checked).ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
            foreach (Items Item in Main.ItemColection)
            {
                if (Item.name == comboBox1.Text)
                {
                    carrier_mass = Item.mass;
                }
            }

        }
        private double carrier_mass = 0;
        private double N;
        private void Carrier_gas_info()
        {// calculate additional informations about carrier gas 
            //number density
            if (!start)
            {
                N = Main.Convertor(textBox7.Text) * 133.322368 / (Main.Convertor(textBox6.Text) * 1.38064852E-23);
                label29.Text = (N).ToString("0.000 E00", CultureInfo.InvariantCulture) + " particles in m^3"; //N/V
                double n = Main.Convertor(textBox7.Text) * 133.322368 / (Main.Convertor(textBox6.Text) * 8.31446);
                label30.Text = (n).ToString("0.0000", CultureInfo.InvariantCulture) + " mol/m^3"; //N/V
            }
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }

        private void Electric_field_info()
        {
            if (!start)
            {
                label35.Text = (Main.Convertor(textBox13.Text) / Main.Convertor(textBox3.Text)).ToString("0.00", CultureInfo.InvariantCulture); //V/cm
                label36.Text = ((Main.Convertor(textBox13.Text) * 1.0354 * Main.Convertor(textBox6.Text)) / (Main.Convertor(textBox3.Text) * 100 * Main.Convertor(textBox7.Text))).ToString("0.00", CultureInfo.InvariantCulture); //V/cm
                double kT = (3 * Main.Convertor(textBox6.Text) * 1.38064852E-23) / 2;
                label42.Text = (kT * 6.02214179E23 / 1000).ToString("0.000", CultureInfo.InvariantCulture); //3/2kT
                label39.Text = (kT / 1.60217662E-19).ToString("0.000", CultureInfo.InvariantCulture); //3/2kT
                label48.Text = (1 / (Math.Sqrt(2) * 1000 * Main.Convertor(textBox14.Text) * 1E-27 * N)).ToString("0.00", CultureInfo.InvariantCulture); // λ
                double col_freq = (Main.Convertor(textBox14.Text) * 1E-18) * Math.Sqrt((4 * 1.38064852E-23 * Main.Convertor(textBox6.Text)) / (Math.PI * carrier_mass)) * N;
                label51.Text = (col_freq).ToString("0.0E00", CultureInfo.InvariantCulture); //Z
                label54.Text = (Math.Abs(Main.Convertor(textBox1.Text) * 1E-6 / col_freq)).ToString("0.0E00", CultureInfo.InvariantCulture); //Z

                label75.Text = (Main.Convertor(label48.Text) / (Main.Convertor(textBox16.Text) * 2 * 1E4)).ToString("0.0E00", CultureInfo.InvariantCulture); //Kn
                label76.Text = (Main.Convertor(textBox8.Text) * 1E-2 * Main.Convertor(textBox16.Text) * 2 * 1E-2 * Main.Convertor(textBox22.Text) * Main.Convertor(textBox7.Text) / (Main.Convertor(textBox21.Text) * 760)).ToString("0.0E00", CultureInfo.InvariantCulture); //Kn
                textBox19.Text = (Main.Convertor(textBox17.Text) * 0.001689).ToString("0.0000", CultureInfo.InvariantCulture);
                textBox20.Text = (Main.Convertor(textBox18.Text) * 0.001689).ToString("0.0000", CultureInfo.InvariantCulture);
                textBox23.Text = ( Math.PI * Math.Pow(Main.Convertor(textBox16.Text) * 1E-2 , 4) * Main.Convertor(textBox7.Text) * 133.33 / ( 8 * Main.Convertor(textBox21.Text) * Main.Convertor(textBox3.Text) * 1E-2)).ToString("0.000E00", CultureInfo.InvariantCulture); // C
                textBox24.Text = ((Main.Convertor(textBox19.Text) + Main.Convertor(textBox20.Text)) / Main.Convertor(textBox23.Text) ).ToString("0.00", CultureInfo.InvariantCulture); // delta p
                textBox25.Text = ((Main.Convertor(textBox19.Text) + Main.Convertor(textBox20.Text)) * Main.Convertor(textBox6.Text) * 1E2 / (Main.Convertor(textBox7.Text) * 133.33 * Math.PI * Math.Pow(Main.Convertor(textBox16.Text)*1E-2,2) * 273)).ToString("0.000E00", CultureInfo.InvariantCulture); // vg
                textBox26.Text = ((Main.Convertor(textBox19.Text) + Main.Convertor(textBox20.Text)) * Main.Convertor(textBox6.Text) * 760 / (273 * Main.Convertor(textBox20.Text) * Main.Convertor(textBox7.Text) * 2.687E19)).ToString("0.000E00", CultureInfo.InvariantCulture);
                textBox12.Text = textBox25.Text = ((Main.Convertor(textBox19.Text) + Main.Convertor(textBox20.Text)) * Main.Convertor(textBox6.Text) * 1E2 / (Main.Convertor(textBox7.Text) * 133.33 * Math.PI * Math.Pow(Main.Convertor(textBox16.Text) * 1E-2, 2) * 273)).ToString("0.00", CultureInfo.InvariantCulture); // vg

                textBox1.Text = (Main.Convertor(textBox3.Text) * 1E6 / Main.Convertor(textBox4.Text)).ToString("0.00", CultureInfo.InvariantCulture);
                label22.Text = (Main.Convertor(textBox1.Text) / Main.Convertor(textBox2.Text)).ToString("0.00", CultureInfo.InvariantCulture);
                label23.Text = (Main.Convertor(textBox3.Text) / Main.Convertor(textBox2.Text)).ToString("0.0000", CultureInfo.InvariantCulture);
            }
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.checkBox2.Enabled = true;
            }
            else
            {
                this.checkBox2.Enabled = false;
            }
            label65.Text = Lambda(checkBox2.Checked).ToString("0.000", CultureInfo.InvariantCulture);
        }

        public double Lambda(bool Infinite)
        {
            double _lambda = 0;
            if(Infinite)
            {
                _lambda = (Main.Convertor(textBox16.Text) / 2.405);
            }
            else
            {
                double num = 1;
                num = (2.405 / Main.Convertor(textBox16.Text)) * (2.405 / Main.Convertor(textBox16.Text)) + (Math.PI/ Main.Convertor(textBox3.Text)) * (Math.PI / Main.Convertor(textBox3.Text));
                _lambda = Math.Sqrt(1 / num);
            }
            return _lambda;
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            label65.Text = Lambda(checkBox2.Checked).ToString("0.000", CultureInfo.InvariantCulture);
            Carrier_gas_info();
            Electric_field_info();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            label65.Text = Lambda(checkBox2.Checked).ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Load
            openFileDialog1.Title = "Load schema";
            openFileDialog1.FileName = "Calculation properties.prp";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
            }
            if (openFileDialog1.FileName != "")
            {
                Cursor.Current = Cursors.WaitCursor;
                StreamReader read = new StreamReader(openFileDialog1.OpenFile());
                string load = read.ReadToEnd();
                read.Dispose();
                read.Close();
                Calculation_setings setings;
                try
                {
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(load)))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        setings = (Calculation_setings)bf.Deserialize(ms);
                        setings.Apply_settings();                       
                    }
                    textBox1.Text = Main.Time_duration.ToString();
                    textBox2.Text = Main.NumberOfSteps.ToString();
                    textBox3.Text = Main.Distance.ToString();
                    textBox4.Text = Main.Ion_velociy.ToString();
                    textBox5.Text = Main.Gauss_signa.ToString();
                    textBox6.Text = Main.Temperature.ToString();
                    textBox7.Text = Main.Pressure.ToString();
                    textBox15.Text = Main.Gauss_density.ToString();
                    textBox16.Text = Main.Radius.ToString();
                    if (Main.Infinite_system)
                    {
                        checkBox2.Checked = true;
                    }
                    if (Main.Diffusion)
                    {
                        this.checkBox2.Enabled = true;
                        this.textBox16.Enabled = true;
                        checkBox1.Checked = true;
                    }
                    if (Main.calc_type)
                    {
                        radioButton1.Checked = true;
                        textBox1.Enabled = true;
                        textBox3.Enabled = false;
                    }
                    else
                    {
                        radioButton2.Checked = true;
                        textBox1.Enabled = false;
                        textBox3.Enabled = true;
                    }
                    foreach (Items Item in Main.ItemColection)
                    {
                        if (Item.cation == false)
                        {
                            comboBox1.Items.Add(Item);
                        }
                    }
                    comboBox1.Text = Main.carrier_gas_type.ToString();
                    label65.Text = Lambda(checkBox2.Checked).ToString("0.000", CultureInfo.InvariantCulture);
                    Carrier_gas_info();
                    Electric_field_info();
                }
                catch
                {
                    MessageBox.Show("Unable to load schema.");
                }
                this.Invalidate();
            }
            Cursor.Current = Cursors.Default;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Save
            Calculation_setings setings = new Calculation_setings();
            saveFileDialog1.Title = "Save properties";
            saveFileDialog1.FileName = "Calculation properties";
            saveFileDialog1.Filter = "prp files (*.prp)|*.prp";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            string save = "";
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, setings);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                save += Convert.ToBase64String(buffer);
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Cursor.Current = Cursors.WaitCursor;
                    StreamWriter write = new StreamWriter(saveFileDialog1.OpenFile());
                    write.Write(save);
                    write.Dispose();
                    write.Close();
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            Carrier_gas_info();
            Electric_field_info();
        }
    }
}
