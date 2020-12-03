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
using OxyPlot;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace KIMI_Sim
{
    public partial class Load_data_for_interpolation : Form
    {
        public bool Experimental_input_status;
        public List<string[]> Experimental_input;
        public int interval_pocet_items;
        public double sirka_intervalu_items;
        public int pocet_prvkou;
        public int[,] new_hlavicka;
        double[,] pure_data;
        int calculation_typ;
        public double[,] output_data;
        public int[] output_hlavicka;
        public int aktual_pomer = 0;

        public Load_data_for_interpolation()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Load experimental data";
            openFileDialog1.FileName = "data.csv";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != "")
                {
                    Cursor.Current = Cursors.WaitCursor;
                    StreamReader read = new StreamReader(openFileDialog1.OpenFile());
                    string load = read.ReadToEnd();
                    read.Dispose();
                    read.Close();
                    List<string[]> Data_input = new List<string[]>();
                    int i = 0;
                    bool look = false;
                    bool enter = false;
                    bool text = false;
                    foreach (char ch in load)
                    {
                        if (look)
                        {
                            if (ch == ',')
                            {
                                if (!text)
                                {
                                    i++;
                                }
                            }
                            if (ch == '\r')
                            {
                                look = false;
                            }
                            if (ch == '\"')
                            {
                                if (text)
                                {
                                    text = false;
                                }
                                else
                                {
                                    text = true;
                                }
                            }

                        }
                        if (ch == '\n' && enter == false)
                        {
                            enter = true;
                            look = true;
                        }
                    }
                    string str = "";
                    string[] line = new string[i + 1];
                    int cnt = 0;
                    foreach (char ch in load)
                    {
                        if (ch == '\r')
                        {
                            try
                            {
                                line[cnt] = str;
                            }
                            catch { }
                            Data_input.Add(line);
                            line = new string[i + 1];
                            str = "";
                            cnt = 0;
                        }
                        else
                        {
                            if (ch == '\"')
                            {
                                if (text)
                                {
                                    text = false;
                                }
                                else
                                {
                                    text = true;
                                }
                            }
                            if (ch == '\n')
                            {
                            }
                            else
                            {
                                if (!text)
                                {
                                    if (ch == ',' && cnt <= i)
                                    {
                                        line[cnt] = str;
                                        str = "";
                                        cnt++;
                                    }
                                    else
                                    {

                                        str += ch.ToString();
                                    }
                                }
                                else
                                {
                                    str += ch.ToString();
                                }
                            }
                        }
                    }
                    Experimental_input = Data_input;
                    Experimental_input_status = true;
                    profile_3();
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void profile_3()
        {
            // inicialization of the graph type
            int i = 0;
            string[] hlavicka = new string[1];
            // extraction of header from data set
            foreach (string[] line in Experimental_input)
            {
                if (i == 0)
                {
                    hlavicka = line;
                }
                i++;
            }
            try
            {
                Experimental_input.Remove(hlavicka);
            }
            catch { }
            // clarify positions and munber of mz measured products
            int masses = 0;
            foreach (string str in hlavicka)
            {
                if (str.Contains("mz"))
                {
                    masses++;
                }
            }
            pocet_prvkou = masses;
            new_hlavicka = new int[masses, 3];
            int k = 0, l = 0;
            foreach (string str in hlavicka)
            {
                if (str.Contains("mz"))
                {
                    string number = "";
                    foreach (char ch in str)
                    {
                        if (ch == '0' || ch == '1' || ch == '2' || ch == '3' || ch == '4' || ch == '5' || ch == '6' || ch == '7' || ch == '8' || ch == '9')
                        {
                            number += ch.ToString();
                        }
                    }
                    int _number = Convert.ToInt32(number);
                    new_hlavicka[k, 0] = _number;
                    new_hlavicka[k, 1] = l;
                    k++;
                }
                l++;
            }
            // transformation of data set into a field composited only from mz data
            pure_data = new double[masses, Experimental_input.Count];
            for (int m = 0; m < masses; m++)
            {
                int n = 0;
                foreach (string[] line in Experimental_input)
                {
                    string stri = line[new_hlavicka[m, 1]];
                    pure_data[m, n] = Convert.ToDouble(stri);
                    n++;
                }
            }
            for (i = 0; i < masses; i++ )
            {
                if (i == 0)
                {
                    textBoxC1.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG1.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 1)
                {
                    textBoxC2.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG2.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 2)
                {
                    textBoxC3.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG3.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 3)
                {
                    textBoxC4.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG4.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 4)
                {
                    textBoxC5.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG5.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 5)
                {
                    textBoxC6.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG6.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 6)
                {
                    textBoxC7.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG7.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 7)
                {
                    textBoxC8.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG8.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 8)
                {
                    textBoxC9.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG9.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 9)
                {
                    textBoxC10.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG10.Text = new_hlavicka[i, 2].ToString();
                }
            }
            if (masses > 10)
            { // nemusime posuvat
                vScrollBar2.LargeChange = 101 * 10 / masses;
                vScrollBar2.Value = 1;
                interval_pocet_items = (masses) - 9;
                double b = (10 / Convert.ToDouble(masses));
                double a = 101 * (1 - b);
                sirka_intervalu_items = a / interval_pocet_items;
            }
            calculation_typ = 1;

        }

        void do_it()
        {
            // calculation of relative values 
            output_data = new double[pocet_prvkou + 1, Experimental_input.Count];
            if (checkBox3.Checked)
            {                 // prispevok ionov
                for(int l = 0; l < pure_data.GetLength(1); l++)
                {
                    for (int i = 0; i < new_hlavicka.GetLength(0); i++)
                    {
                        double suma = 0;
                        for (int j = 0; j < new_hlavicka.GetLength(0); j++)
                        {
                            if (new_hlavicka[i, 2] == new_hlavicka[j, 2])
                            {
                                // zhoda v ID grupy
                                suma += pure_data[j, l];
                            }
                        }
                        output_data[i + 1, l] = pure_data[i, l] / suma;
                    }
                }                 
            }
            else
            {
                for (int l = 0; l < pure_data.GetLength(1); l++)
                {
                    for (int i = 0; i < new_hlavicka.GetLength(0); i++)
                    {
                        output_data[i + 1, l] = pure_data[i, l];
                    }
                }
            }

            for (int i = 0; i < Experimental_input.Count; i++)
            {
                double H3O_sum = 0;
                for (int j = 0; j < pocet_prvkou; j++)
                {
                    if (new_hlavicka[j, 0] == 19)
                    {
                        H3O_sum += pure_data[j, i];
                    }
                    if (new_hlavicka[j, 0] == 37)
                    {
                        H3O_sum += pure_data[j, i];
                    }
                    if (new_hlavicka[j, 0] == 55)
                    {
                        H3O_sum += pure_data[j, i];
                    }
                    if (new_hlavicka[j, 0] == 73)
                    {
                        H3O_sum += pure_data[j, i];
                    }
                }
                double H3O = 0;
                for (int j = 0; j < pocet_prvkou; j++)
                {
                    if (new_hlavicka[j, 0] == 19)
                    {
                        H3O = pure_data[j, i];
                    }
                }
                double partial_X = -1 * Math.Log((H3O / H3O_sum), Math.E);// x-os
                if(checkBox1.Checked)
                {
                    try
                    {
                        partial_X = partial_X * Main.Convertor(textBox1.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Unable to apply conversion factor!");
                    }
                }
                output_data[0, i] = partial_X;
            }
            // usporiadanie podla x-ovej polohy
            bool sorting = true;
            while (sorting)
            {
                int iteration = 0;
                for (int i = 1; i < Experimental_input.Count; i++)
                {
                    if (output_data[0, i - 1] > output_data[0, i])
                    {
                        // swap
                        for (int j = 0; j < pocet_prvkou + 1; j++)
                        {
                            double deposit = output_data[j, i - 1];
                            output_data[j, i - 1] = output_data[j, i];
                            output_data[j, i] = deposit;
                        }
                        iteration++;
                    }
                }
                if (iteration == 0)
                {
                    sorting = false;
                }
            }
            output_hlavicka = new int[new_hlavicka.GetLength(0) + 1];
            for (int i = 0; i < new_hlavicka.GetLength(0); i++)
            {
                output_hlavicka[i + 1] = new_hlavicka[i, 0];
            }
        }

        void do_it2()
        {
            // calculation of relative values 
            output_data = new double[pocet_prvkou + 1, Experimental_input.Count];
            if (checkBox3.Checked)
            {                 // prispevok ionov
                for (int l = 0; l < pure_data.GetLength(1); l++)
                {
                    for (int i = 0; i < new_hlavicka.GetLength(0); i++)
                    {
                        double suma = 0;
                        for (int j = 0; j < new_hlavicka.GetLength(0); j++)
                        {
                            if (new_hlavicka[i, 2] == new_hlavicka[j, 2])
                            {
                                // zhoda v ID grupy
                                suma += pure_data[j, l];
                            }
                        }
                        output_data[i + 1, l] = pure_data[i, l] / suma;
                    }
                }
            }
            else
            {
                for (int l = 0; l < pure_data.GetLength(1); l++)
                {
                    for (int i = 0; i < new_hlavicka.GetLength(0); i++)
                    {
                        output_data[i + 1, l] = pure_data[i, l];
                    }
                }
            }
            //
            for (int i = 0; i < Experimental_input.Count; i++)
            {
                // Uprava V na E/N parameter
                double partial_X = 0; // represent E/N
                double E_field = pure_data[pure_data.GetLength(0) - 1,i] / Main.Distance; // V/cm
                partial_X = (1.0354 * Main.Temperature * E_field) / (100 * Main.Pressure);
                output_data[0, i] = Math.Round(partial_X, 3);
            }
            //
            // usporiadanie podla x-ovej polohy
            bool sorting = true;
            while (sorting)
            {
                int iteration = 0;
                for (int i = 1; i < Experimental_input.Count; i++)
                {
                    if (output_data[0, i - 1] > output_data[0, i])
                    {
                        // swap
                        for (int j = 0; j < pocet_prvkou + 1; j++)
                        {
                            double deposit = output_data[j, i - 1];
                            output_data[j, i - 1] = output_data[j, i];
                            output_data[j, i] = deposit;
                        }
                        iteration++;
                    }
                }
                if (iteration == 0)
                {
                    sorting = false;
                }
            }
            output_hlavicka = new int[new_hlavicka.GetLength(0) + 1];
            for (int i = 0; i < new_hlavicka.GetLength(0); i++)
            {
                output_hlavicka[i + 1] = new_hlavicka[i, 0];
            }
        }

        void do_it3()
        {
            // calculation of relative values 
            output_data = new double[pocet_prvkou + 1, Experimental_input.Count];
            if (checkBox3.Checked)
            {                 // prispevok ionov
                for (int l = 0; l < pure_data.GetLength(1); l++)
                {
                    for (int i = 0; i < new_hlavicka.GetLength(0); i++)
                    {
                        double suma = 0;
                        for (int j = 0; j < new_hlavicka.GetLength(0); j++)
                        {
                            if (new_hlavicka[i, 2] == new_hlavicka[j, 2])
                            {
                                // zhoda v ID grupy
                                suma += pure_data[j, l];
                            }
                        }
                        output_data[i + 1, l] = pure_data[i, l] / suma;
                    }
                }
            }
            else
            {
                for (int l = 0; l < pure_data.GetLength(1); l++)
                {
                    for (int i = 0; i < new_hlavicka.GetLength(0); i++)
                    {
                        output_data[i + 1, l] = pure_data[i, l];
                    }
                }
            }

            for (int i = 0; i < Experimental_input.Count; i++)
            {
                double NO_sum = 0;
                for (int j = 0; j < pocet_prvkou; j++)
                {
                    if (new_hlavicka[j, 0] == 30)
                    {
                        NO_sum += pure_data[j, i];
                    }
                    if (new_hlavicka[j, 0] == 48)
                    {
                        NO_sum += pure_data[j, i];
                    }
                    if (new_hlavicka[j, 0] == 66)
                    {
                        NO_sum += pure_data[j, i];
                    }
                }
                double NO = 0;
                for (int j = 0; j < pocet_prvkou; j++)
                {
                    if (new_hlavicka[j, 0] == 30)
                    {
                        NO = pure_data[j, i];
                    }
                }
                double partial_X = -1 * Math.Log((NO / NO_sum), Math.E);// x-os
                output_data[0, i] = partial_X;
            }
            // usporiadanie podla x-ovej polohy
            bool sorting = true;
            while (sorting)
            {
                int iteration = 0;
                for (int i = 1; i < Experimental_input.Count; i++)
                {
                    if (output_data[0, i - 1] > output_data[0, i])
                    {
                        // swap
                        for (int j = 0; j < pocet_prvkou + 1; j++)
                        {
                            double deposit = output_data[j, i - 1];
                            output_data[j, i - 1] = output_data[j, i];
                            output_data[j, i] = deposit;
                        }
                        iteration++;
                    }
                }
                if (iteration == 0)
                {
                    sorting = false;
                }
            }
            output_hlavicka = new int[new_hlavicka.GetLength(0) + 1];
            for (int i = 0; i < new_hlavicka.GetLength(0); i++)
            {
                output_hlavicka[i + 1] = new_hlavicka[i, 0];
            }
        }

        private void button_cancle_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (calculation_typ == 1)
            {
                if(radioH3O.Checked == true)
                {
                    if (!checkBox1.Checked)
                    {
                        do_it();
                        Calculation_control.Popis_X = "-ln(H3O/total(H2O))";
                    }
                    else
                    {
                        do_it();
                        calculation_typ = 10;
                        Calculation_control.Popis_X = "[H2O]";
                    }
                }
                if (radioNO.Checked == true)
                {
                    do_it3();
                    Calculation_control.Popis_X = "-ln(NO/total(NO))";
                }
            }
            if (calculation_typ == 2)
            {
                do_it2();
                Calculation_control.Popis_X = "Electric field [E/N]";
            }
            if(checkBox3.Checked)
            {
                Calculation_control.Load_relative = true;
            }
            this.Dispose();
            Calculation_control.Load_hlavicka = output_hlavicka;
            Calculation_control.Loaded_data = output_data;
            Calculation_control.Load_type = calculation_typ;
            Calculation_control.show_loaded_data();
            
        }

        private void labelG1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC1.Text)
                    {
                        int nmb = Convert.ToInt32(labelG1.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG1.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC1.Text)
                    {
                        int nmb = Convert.ToInt32(labelG1.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG1.Text = nmb.ToString();
                    }
                }
            }
        }

        private void labelG2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC2.Text)
                    {
                        int nmb = Convert.ToInt32(labelG2.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG2.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC2.Text)
                    {
                        int nmb = Convert.ToInt32(labelG2.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG2.Text = nmb.ToString();
                    }
                }
            }
        }

        private void labelG3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC3.Text)
                    {
                        int nmb = Convert.ToInt32(labelG3.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG3.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC3.Text)
                    {
                        int nmb = Convert.ToInt32(labelG3.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG3.Text = nmb.ToString();
                    }
                }
            }
        }

        private void labelG4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC4.Text)
                    {
                        int nmb = Convert.ToInt32(labelG4.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG4.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC4.Text)
                    {
                        int nmb = Convert.ToInt32(labelG4.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG4.Text = nmb.ToString();
                    }
                }
            }
        }

        private void labelG5_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC5.Text)
                    {
                        int nmb = Convert.ToInt32(labelG5.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG5.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC5.Text)
                    {
                        int nmb = Convert.ToInt32(labelG5.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG5.Text = nmb.ToString();
                    }
                }
            }
        }

        private void labelG6_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC6.Text)
                    {
                        int nmb = Convert.ToInt32(labelG6.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG6.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC6.Text)
                    {
                        int nmb = Convert.ToInt32(labelG6.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG6.Text = nmb.ToString();
                    }
                }
            }
        }

        private void labelG7_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC7.Text)
                    {
                        int nmb = Convert.ToInt32(labelG7.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG7.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC7.Text)
                    {
                        int nmb = Convert.ToInt32(labelG7.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG7.Text = nmb.ToString();
                    }
                }
            }
        }

        private void labelG8_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC8.Text)
                    {
                        int nmb = Convert.ToInt32(labelG8.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG8.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC8.Text)
                    {
                        int nmb = Convert.ToInt32(labelG8.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG8.Text = nmb.ToString();
                    }
                }
            }
        }

        private void labelG9_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC9.Text)
                    {
                        int nmb = Convert.ToInt32(labelG9.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG9.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC9.Text)
                    {
                        int nmb = Convert.ToInt32(labelG9.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG9.Text = nmb.ToString();
                    }
                }
            }
        }

        private void labelG10_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC10.Text)
                    {
                        int nmb = Convert.ToInt32(labelG10.Text);
                        nmb++;
                        new_hlavicka[i, 2] = nmb;
                        labelG10.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                for (int i = 0; i < pocet_prvkou; i++)
                {
                    if ("m/z " + new_hlavicka[i, 0].ToString() == textBoxC10.Text)
                    {
                        int nmb = Convert.ToInt32(labelG10.Text);
                        nmb--;
                        new_hlavicka[i, 2] = nmb;
                        labelG10.Text = nmb.ToString();
                    }
                }
            }
        }

        private void vScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            int value = vScrollBar2.Value;
            if (vScrollBar2.Value > 101 - vScrollBar2.LargeChange)
            {
                value = 101 - vScrollBar2.LargeChange;
            }
            int pomer = 0;
            try
            {
                pomer = value / Convert.ToInt16(sirka_intervalu_items);
            }
            catch { }
            if (pomer > interval_pocet_items - 1)
            {
                pomer = interval_pocet_items - 1;
            }
            if (pomer != aktual_pomer)
            {
                int i = 1 - pomer;
                for (int j = 0; j < pocet_prvkou; j++)
                {
                    if (i == 1)
                    {
                        textBoxC1.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG1.Text = new_hlavicka[j, 2].ToString();
                    }
                    if (i == 2)
                    {
                        textBoxC2.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG2.Text = new_hlavicka[j, 2].ToString();
                    }
                    if (i == 3)
                    {
                        textBoxC3.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG3.Text = new_hlavicka[j, 2].ToString();
                    }
                    if (i == 4)
                    {
                        textBoxC4.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG4.Text = new_hlavicka[j, 2].ToString();
                    }
                    if (i == 5)
                    {
                        textBoxC5.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG5.Text = new_hlavicka[j, 2].ToString();
                    }
                    if (i == 6)
                    {
                        textBoxC6.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG6.Text = new_hlavicka[j, 2].ToString();
                    }
                    if (i == 7)
                    {
                        textBoxC7.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG7.Text = new_hlavicka[j, 2].ToString();
                    }
                    if (i == 8)
                    {
                        textBoxC8.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG8.Text = new_hlavicka[j, 2].ToString();
                    }
                    if (i == 9)
                    {
                        textBoxC9.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG9.Text = new_hlavicka[j, 2].ToString();
                    }
                    if (i == 10)
                    {
                        textBoxC10.Text = "m/z " + new_hlavicka[j, 0].ToString();
                        labelG10.Text = new_hlavicka[j, 2].ToString();
                    }
                    i++;
                }
            }
            aktual_pomer = pomer;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Load experimental data";
            openFileDialog1.FileName = "data.csv";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != "")
                {
                    Cursor.Current = Cursors.WaitCursor;
                    StreamReader read = new StreamReader(openFileDialog1.OpenFile());
                    string load = read.ReadToEnd();
                    read.Dispose();
                    read.Close();
                    List<string[]> Data_input = new List<string[]>();
                    int i = 0;
                    bool look = false;
                    bool enter = false;
                    bool text = false;
                    foreach (char ch in load)
                    {
                        if (look)
                        {
                            if (ch == ',')
                            {
                                if (!text)
                                {
                                    i++;
                                }
                            }
                            if (ch == '\r')
                            {
                                look = false;
                            }
                        }
                        if (ch == '\n' && enter == false)
                        {
                            enter = true;
                            look = true;
                        }
                    }
                    string str = "";
                    string[] line = new string[i + 1];
                    int cnt = 0;
                    foreach (char ch in load)
                    {
                        if (ch == '\r')
                        {
                            try
                            {
                                line[cnt] = str;
                            }
                            catch { }
                            Data_input.Add(line);
                            line = new string[i + 1];
                            str = "";
                            cnt = 0;
                        }
                        else
                        {
                            if (ch == '\n')
                            {
                            }
                            else
                            {
                                if (!text)
                                {
                                    if (ch == ',' && cnt <= i)
                                    {
                                        line[cnt] = str;
                                        str = "";
                                        cnt++;
                                    }
                                    else
                                    {

                                        str += ch.ToString();
                                    }
                                }
                                else
                                {
                                    str += ch.ToString();
                                }
                            }
                        }
                    }
                    Experimental_input = Data_input;
                    Experimental_input_status = true;
                    SIDT_conc_dep();
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void SIDT_conc_dep()
        {
            // inicialization of the graph type
            int i = 0;
            string[] hlavicka = new string[1];
            // extraction of header from data set
            foreach (string[] line in Experimental_input)
            {
                if (i == 0)
                {
                    hlavicka = line;
                }
                i++;
            }
            try
            {
                Experimental_input.Remove(hlavicka);
            }
            catch { }
            // clarify positions and nunber of mz measured products
            int masses = 0;
            foreach (string str in hlavicka)
            {
                if (str.Contains("m/z"))
                {
                    masses++;
                }
            }
            pocet_prvkou = masses;
            new_hlavicka = new int[masses, 3];
            int k = 0, l = 0;
            foreach (string str in hlavicka)
            {
                if (str.Contains("m/z"))
                {
                    string number = "";
                    foreach (char ch in str)
                    {
                        if (ch == '0' || ch == '1' || ch == '2' || ch == '3' || ch == '4' || ch == '5' || ch == '6' || ch == '7' || ch == '8' || ch == '9')
                        {
                            number += ch.ToString();
                        }
                        if (ch == '.')
                        {
                            break;
                        }
                    }
                    int _number = Convert.ToInt32(number);
                    new_hlavicka[k, 0] = _number;
                    new_hlavicka[k, 1] = l;
                    k++;
                }
                l++;
            }
            // transformation of data set into a field composited only from mz data
            pure_data = new double[masses, Experimental_input.Count];
            for (int m = 0; m < masses; m++)
            {
                int n = 0;
                foreach (string[] line in Experimental_input)
                { 
                    pure_data[m, n] = Main.Convertor(line[new_hlavicka[m, 1]]);
                    n++;
                }
            }
            for (i = 0; i < masses; i++)
            {
                if (i == 0)
                {
                    textBoxC1.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG1.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 1)
                {
                    textBoxC2.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG2.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 2)
                {
                    textBoxC3.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG3.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 3)
                {
                    textBoxC4.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG4.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 4)
                {
                    textBoxC5.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG5.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 5)
                {
                    textBoxC6.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG6.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 6)
                {
                    textBoxC7.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG7.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 7)
                {
                    textBoxC8.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG8.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 8)
                {
                    textBoxC9.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG9.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 9)
                {
                    textBoxC10.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG10.Text = new_hlavicka[i, 2].ToString();
                }
            }
            if (masses > 10)
            { // nemusime posuvat
                vScrollBar2.LargeChange = 101 * 10 / masses;
                vScrollBar2.Value = 1;
                interval_pocet_items = (masses) - 9;
                double b = (10 / Convert.ToDouble(masses));
                double a = 101 * (1 - b);
                sirka_intervalu_items = a / interval_pocet_items;
            }
            calculation_typ = 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Load experimental data";
            openFileDialog1.FileName = "data.csv";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != "")
                {
                    Cursor.Current = Cursors.WaitCursor;
                    StreamReader read = new StreamReader(openFileDialog1.OpenFile());
                    string load = read.ReadToEnd();
                    read.Dispose();
                    read.Close();
                    List<string[]> Data_input = new List<string[]>();
                    int i = 0;
                    bool look = false;
                    bool enter = false;
                    bool text = false;
                    foreach (char ch in load)
                    {
                        if (look)
                        {
                            if (ch == ',')
                            {
                                if (!text)
                                {
                                    i++;
                                }
                            }
                            if (ch == '\r')
                            {
                                look = false;
                            }
                        }
                        if (ch == '\n' && enter == false)
                        {
                            enter = true;
                            look = true;
                        }
                    }
                    string str = "";
                    string[] line = new string[i + 1];
                    int cnt = 0;
                    foreach (char ch in load)
                    {
                        if (ch == '\r')
                        {
                            try
                            {
                                line[cnt] = str;
                            }
                            catch { }
                            Data_input.Add(line);
                            line = new string[i + 1];
                            str = "";
                            cnt = 0;
                        }
                        else
                        {
                            if (ch == '\n')
                            {
                            }
                            else
                            {
                                if (!text)
                                {
                                    if (ch == ',' && cnt <= i)
                                    {
                                        line[cnt] = str;
                                        str = "";
                                        cnt++;
                                    }
                                    else
                                    {

                                        str += ch.ToString();
                                    }
                                }
                                else
                                {
                                    str += ch.ToString();
                                }
                            }
                        }
                    }
                    Experimental_input = Data_input;
                    Experimental_input_status = true;
                    SIDT_elec_dep();
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void SIDT_elec_dep()
        {
            // inicialization of the graph type
            int i = 0;
            string[] hlavicka = new string[1];
            // extraction of header from data set
            foreach (string[] line in Experimental_input)
            {
                if (i == 0)
                {
                    hlavicka = line;
                }
                i++;
            }
            try
            {
                Experimental_input.Remove(hlavicka);
            }
            catch { }
            // clarify positions and nunber of mz measured products
            int masses = 0;
            foreach (string str in hlavicka)
            {
                if (str.Contains("m/z"))
                {
                    masses++;
                }
            }
            pocet_prvkou = masses;
            new_hlavicka = new int[masses, 3];
            int k = 0, l = 0;
            foreach (string str in hlavicka)
            {
                if (str.Contains("m/z"))
                {
                    string number = "";
                    foreach (char ch in str)
                    {
                        if (ch == '0' || ch == '1' || ch == '2' || ch == '3' || ch == '4' || ch == '5' || ch == '6' || ch == '7' || ch == '8' || ch == '9')
                        {
                            number += ch.ToString();
                        }
                        if (ch == '.')
                        {
                            break;
                        }
                    }
                    int _number = Convert.ToInt32(number);
                    new_hlavicka[k, 0] = _number;
                    new_hlavicka[k, 1] = l;
                    k++;
                }
                l++;
            }
            // transformation of data set into a field composited only from mz data
            pure_data = new double[masses + 1, Experimental_input.Count];
            for (int m = 0; m < masses; m++)
            {
                int n = 0;
                foreach (string[] line in Experimental_input)
                {
                    pure_data[m, n] = Main.Convertor(line[new_hlavicka[m, 1]]);
                    n++;
                }
            }
            int DVM1 = 0, DVM2 = 0;
            int o = 0;
            foreach (string str in hlavicka)
            {
                if(str.Contains("DVM1"))
                {
                    DVM1 = o;
                }
                if(str.Contains("DVM2"))
                {
                    DVM2 = o;
                }
                o++;
            }
            o = 0;
            foreach (string[] line in Experimental_input)
            {
                pure_data[pure_data.GetLength(0) - 1, o] = Math.Round(Math.Abs(Main.Convertor(line[DVM2]) - Main.Convertor(line[DVM1])),3);
                o++;
            }

            for (i = 0; i < masses; i++)
            {
                if (i == 0)
                {
                    textBoxC1.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG1.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 1)
                {
                    textBoxC2.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG2.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 2)
                {
                    textBoxC3.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG3.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 3)
                {
                    textBoxC4.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG4.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 4)
                {
                    textBoxC5.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG5.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 5)
                {
                    textBoxC6.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG6.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 6)
                {
                    textBoxC7.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG7.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 7)
                {
                    textBoxC8.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG8.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 8)
                {
                    textBoxC9.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG9.Text = new_hlavicka[i, 2].ToString();
                }
                if (i == 9)
                {
                    textBoxC10.Text = "m/z " + new_hlavicka[i, 0].ToString();
                    labelG10.Text = new_hlavicka[i, 2].ToString();
                }
            }
            if (masses > 10)
            { // nemusime posuvat
                vScrollBar2.LargeChange = 101 * 10 / masses;
                vScrollBar2.Value = 1;
                interval_pocet_items = (masses) - 9;
                double b = (10 / Convert.ToDouble(masses));
                double a = 101 * (1 - b);
                sirka_intervalu_items = a / interval_pocet_items;
            }
            calculation_typ = 2;
        }

        private void Load_data_for_interpolation_Load(object sender, EventArgs e)
        {
            this.Location = this.Owner.Location;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                textBox1.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
            }
        }
    }
}