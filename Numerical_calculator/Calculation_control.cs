﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using OxyPlot;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Runtime.Serialization.Formatters.Binary;


namespace KIMI_Sim
{
    /// <summary>
    /// Recapitulation of parameters
    /// save changes/ modification on initial values, wintout changing of assignment
    /// output preparation for Calculation_control
    /// both-way reactions has to be splited into two one-side reactions
    /// </summary>
    public partial class Calculation_control : Form
    {
        public List<Items_used> ItemColection_used { get; set; }
        public List<Reactions_used> ReactionColection_used { get; set; }
        public int interval_pocet_reaction;
        public double sirka_intervalu_rection;
        public int aktual_pomer_reaction = 0;
        public int interval_pocet_items;
        public double sirka_intervalu_items;
        public int aktual_pomer_items = 0;
        public bool Experimental_input_status;
        static bool log_lin;

        // data storage,3
        public string[] hlavicka;
        public List<double[,]> tabulka;
        string Concentration_name = "";
        double current_electric_field;
        public List<List<double[,]>> data_D2 = new List<List<double[,]>>();
        public List<List<List<double[,]>>> data_D3 = new List<List<List<double[,]>>>();
        short data_dim = 1;

        #region Calculation

        public double dt = Main.Time_duration / Main.NumberOfSteps; // us
        public double time_range = Main.Time_duration; //us
        public List<Equation> equation = new List<Equation>();
        public List<Premenna> premenna = new List<Premenna>();
        public List<Konstants> konstanta = new List<Konstants>();
        public int value { get; set; }
        public int interval_pocet;
        public double sirka_intervalu;
        public int aktual_pomer = 0;
        public bool calculation_complete = false;
        public bool calculated_data = false;

        List<List<double[]>> error_evolution;
        List<List<List<double[]>>> calculation_memory = new List<List<List<double[]>>>();
        List<List<string>> calculation_memory_title = new List<List<string>>();
        int calculation_memory_typeOf = 0;
        public int calculation_memory_chosen = 0;

        private bool stop_write = false;
        private DateTime start_time;
        private int actual_cycle = 1;
        private int total_cycle = 1;
        private bool calculation_running = false;
        bool visualtization_started = false;
        bool update_axis = true;

        public Calculation_control()
        {
            InitializeComponent();
        }

        private void data_transformation()
        {
            equation = new List<Equation>();
            premenna = new List<Premenna>();
            konstanta = new List<Konstants>();
            // premenit rekcie na equation a konstanty
            foreach (Items_used Item in ItemColection_used) // transformacia pouzitich ionov na vytvorenie premennych
            {
                bool detector = false;
                foreach (Premenna pr in premenna)
                {
                    if (Item.name == pr.name)
                    {
                        detector = true;
                    }
                }
                if (!detector)
                {
                    novapremenna(Item.name, Item.concentration, Item.concentration);
                    Equation Eq = new Equation { name = Item.name, equation = new List<Equation_element>() };
                    if (Main.Diffusion)
                    { // add Diffusion element to Calculation elements in equations
                        string dif_const = "k(" + Item.name + ")";
                        string[] s = new string[] { "True", dif_const, Item.name };
                        Eq.equation.Add(new Equation_element { mobility = Item.mobility, equation_element = s });
                        double dif_value = Item.diffusion * (Main.Temperature / 300) * (1 / Main.Pressure) / (Lambda() * Lambda());
                        novakonstanta(dif_const, dif_value, new Constant(dif_value, true));
                    }
                    equation.Add(Eq);
                }
            }
            int k = 0;
            foreach (Reactions_used Reaction in ReactionColection_used) // vytvorenie konstant a jednotlivych rovnic na zaklade reakcii
            {
                int number_of_items = 0;
                if (Reaction.reaction_type == 0 || Reaction.reaction_type == 2)
                {
                    number_of_items = 3 + Reaction.neutrals_used_A.Count;
                }
                else
                {
                    number_of_items = 3 + Reaction.neutrals_used_B.Count;
                }
                string k_name = "k(" + k.ToString() + ")";
                if (Reaction.rate_konstant.name == "A/Er(E/N)^B" || Reaction.rate_konstant.name == "A*EXP(B/Er(E/N))")
                {
                    if (Reaction.reaction_type == 0 || Reaction.reaction_type == 2)
                    {
                        double reagent_mass = 0;
                        foreach (Items_used neutrals in Reaction.neutrals_used_A)
                        {
                            reagent_mass = neutrals.mass;
                        }
                        Reaction.rate_konstant.mass_element = (reagent_mass / (reagent_mass + Reaction.item_A.mass)) * (Reaction.item_A.mass + Main.carrier_gas_type.mass) * 1.66053904e-27;
                        Reaction.rate_konstant.reactant_mobility = Reaction.item_A.mobility;
                    }
                    else
                    {
                        double reagent_mass = 0;
                        foreach (Items_used neutrals in Reaction.neutrals_used_B)
                        {
                            reagent_mass = neutrals.mass;
                        }
                        Reaction.rate_konstant.mass_element = (reagent_mass / (reagent_mass + Reaction.item_B.mass)) * (Reaction.item_B.mass + Main.carrier_gas_type.mass) * 1.66053904e-27;
                        Reaction.rate_konstant.reactant_mobility = Reaction.item_B.mobility;
                    }
                }
                novakonstanta(k_name, Reaction.rate_konstant.Value(0), Reaction.rate_konstant);
                foreach (Equation eq in equation)
                {
                    Equation_element element = new Equation_element();
                    string[] s = new string[number_of_items];
                    // s0 - const, s1 - +-, s2- ion- s3... neutrals
                    s[0] = k_name;//konst                   
                    if (Reaction.item_A.name == eq.name)
                    {
                        if (Reaction.reaction_type == 0 || Reaction.reaction_type == 2)
                        {
                            s[1] = "True";
                            s[2] = Reaction.item_A.name;
                            int i = 0;
                            foreach (Items IT in Reaction.neutrals_used_A)
                            {
                                s[3 + i] = IT.name;
                                i++;
                            }
                            element.mobility = Reaction.item_A.mobility;
                            element.equation_element = s;
                            eq.equation.Add(element);
                        }
                        else
                        {
                            s[1] = "False";
                            s[2] = Reaction.item_B.name;
                            int i = 0;
                            foreach (Items IT in Reaction.neutrals_used_B)
                            {
                                s[3 + i] = IT.name;
                                i++;
                            }
                            element.mobility = Reaction.item_B.mobility;
                            element.equation_element = s;
                            eq.equation.Add(element);
                        }
                    }
                    if (Reaction.item_B.name == eq.name)
                    {
                        if (Reaction.reaction_type == 0 || Reaction.reaction_type == 2)
                        {
                            s[1] = "False";
                            s[2] = Reaction.item_A.name;
                            int i = 0;
                            foreach (Items IT in Reaction.neutrals_used_A)
                            {
                                s[3 + i] = IT.name;
                                i++;
                            }
                            element.mobility = Reaction.item_A.mobility;
                            element.equation_element = s;
                            eq.equation.Add(element);
                        }
                        else
                        {
                            s[1] = "True";
                            s[2] = Reaction.item_B.name;
                            int i = 0;
                            foreach (Items IT in Reaction.neutrals_used_B)
                            {
                                s[3 + i] = IT.name;
                                i++;
                            }
                            element.mobility = Reaction.item_B.mobility;
                            element.equation_element = s;
                            eq.equation.Add(element);
                        }
                    }
                    foreach (Items_used IT in Reaction.neutrals_used_A) // OK
                    {
                        try
                        {
                            s = new string[number_of_items];
                            element = new Equation_element();
                            // s0 - const, s1 - +-, s2- ion- s3... neutrals
                            s[0] = k_name;//konst
                            if (IT.name == eq.name)
                            {
                                if (Reaction.reaction_type == 0 || Reaction.reaction_type == 2)
                                {
                                    s[1] = "True";
                                    s[2] = Reaction.item_A.name;
                                    int i = 0;
                                    foreach (Items IT1 in Reaction.neutrals_used_A)
                                    {
                                        s[3 + i] = IT1.name;
                                        i++;
                                    }
                                    element.mobility = Reaction.item_A.mobility;
                                    element.equation_element = s;
                                    eq.equation.Add(element);
                                }
                                else
                                {
                                    s[1] = "False";
                                    s[2] = Reaction.item_B.name;
                                    int i = 0;
                                    foreach (Items IT1 in Reaction.neutrals_used_B)
                                    {
                                        s[3 + i] = IT1.name;
                                        i++;
                                    }
                                    element.mobility = Reaction.item_B.mobility;
                                    element.equation_element = s;
                                    eq.equation.Add(element);
                                }
                            }
                        }
                        catch { }
                    }
                    foreach (Items_used IT in Reaction.neutrals_used_B) // OK
                    {
                        try
                        {
                            s = new string[number_of_items];
                            element = new Equation_element();
                            // s0 - const, s1 - +-, s2- ion- s3... neutrals
                            s[0] = k_name;//konst
                            if (IT.name == eq.name)
                            {
                                if (Reaction.reaction_type == 0 || Reaction.reaction_type == 2)
                                {
                                    s[1] = "False";
                                    s[2] = Reaction.item_A.name;
                                    int i = 0;
                                    foreach (Items IT1 in Reaction.neutrals_used_A)
                                    {
                                        s[3 + i] = IT1.name;
                                        i++;
                                    }
                                    element.mobility = Reaction.item_A.mobility;
                                    element.equation_element = s;
                                    eq.equation.Add(element);
                                }
                                else
                                {
                                    s[1] = "True";
                                    s[2] = Reaction.item_B.name;
                                    int i = 0;
                                    foreach (Items IT1 in Reaction.neutrals_used_B)
                                    {
                                        s[3 + i] = IT1.name;
                                        i++;
                                    }
                                    element.mobility = Reaction.item_B.mobility;
                                    element.equation_element = s;
                                    eq.equation.Add(element);
                                }
                            }
                        }
                        catch { }
                    }
                }
                // The same for -1 reaction
                if (Reaction.reaction_type == 2)
                {
                    number_of_items = 3 + Reaction.neutrals_used_B.Count;
                    string k_name_ = "k(-" + k.ToString() + ")";
                    if (Reaction.rate_konstant_.name == "A/Er(E/N)^B" || Reaction.rate_konstant_.name == "A*EXP(B/Er(E/N))") // 
                    {
                        double reduced_mass = 0;
                        foreach (Items_used neutrals in Reaction.neutrals_used_B)
                        {
                            reduced_mass = neutrals.mass;
                        }
                        Reaction.rate_konstant_.mass_element = (reduced_mass / (reduced_mass + Reaction.item_B.mass)) * (Reaction.item_B.mass + 4) * 1.66053904e-27;
                        Reaction.rate_konstant_.reactant_mobility = Reaction.item_B.mobility;
                    }
                    novakonstanta(k_name_, Reaction.rate_konstant_.Value(0), Reaction.rate_konstant_);
                    foreach (Equation eq in equation)
                    {
                        Equation_element element = new Equation_element();
                        string[] s = new string[number_of_items];
                        // s0 - const, s1 - +-, s2- ion- s3... neutrals
                        s[0] = k_name_;//konst
                        s[2] = Reaction.item_B.name;
                        int i = 0;
                        foreach (Items IT in Reaction.neutrals_used_B)
                        {
                            s[3 + i] = IT.name;
                            i++;
                        }
                        if (Reaction.item_B.name == eq.name)
                        {
                            s[1] = "True";
                            element.mobility = Reaction.item_B.mobility;
                            element.equation_element = s;
                            eq.equation.Add(element);
                        } //Add(new string[] { s[0], s[1], s[2], s[3] })
                        if (Reaction.item_A.name == eq.name)
                        {
                            s[1] = "False";
                            element.mobility = Reaction.item_B.mobility;
                            element.equation_element = s;
                            eq.equation.Add(element);
                        }
                        foreach (Items_used IT in Reaction.neutrals_used_B) // OK
                        {
                            try
                            {
                                s = new string[number_of_items];
                                element = new Equation_element();
                                // s0 - const, s1 - +-, s2- ion- s3... neutrals
                                s[1] = "True";
                                s[0] = k_name_;//konst
                                s[2] = Reaction.item_B.name;
                                i = 0;
                                foreach (Items IT1 in Reaction.neutrals_used_B)
                                {
                                    s[3 + i] = IT1.name;
                                    i++;
                                }
                                if (IT.name == eq.name)
                                {
                                    element.mobility = Reaction.item_B.mobility;
                                    element.equation_element = s;
                                    eq.equation.Add(element);
                                }
                            }
                            catch { }
                        }
                        foreach (Items_used IT in Reaction.neutrals_used_A) // OK
                        {
                            try
                            {
                                s = new string[number_of_items];
                                element = new Equation_element();
                                // s0 - const, s1 - +-, s2- ion- s3... neutrals
                                s[1] = "False";
                                s[0] = k_name_;//konst
                                s[2] = Reaction.item_B.name;
                                i = 0;
                                foreach (Items IT1 in Reaction.neutrals_used_B)
                                {
                                    s[3 + i] = IT1.name;
                                    i++;
                                }
                                if (IT.name == eq.name)
                                {
                                    element.mobility = Reaction.item_B.mobility;
                                    element.equation_element = s;
                                    eq.equation.Add(element);
                                }
                            }
                            catch { }
                        }
                    }
                }
                k++;
            }
        }

        private void calculation_proceed(bool calc_type, bool E_field, double E_value, bool E_change)
        {
            // reset of parameters for the new calculation cycle
            // inicialization of Calculate
            // limitation in case of lelctroc field - calc_type === false (distance)
            if (E_field && E_change)
            {
                // calculation possible for distance as a parameter only
                calc_type = false;
                // set electrick field for velocity and konstatnts value
                foreach (Equation eq in equation)
                {
                    eq.set_velocity_(E_value);
                }
                foreach (Konstants ko in konstanta)
                {
                    ko.set_field(E_value);
                }
            }
            // single shot  measurment at defined Electric field
            if (calc_type)
            { // time is main evaluator
                Calculate(E_field, Main.Time_duration / (Main.NumberOfSteps * 1000000), Main.Time_duration / Main.NumberOfSteps, Main.Time_duration, calc_type);
            }
            if (!calc_type)
            { // distance is main evaluator
                Calculate(E_field, Main.Distance / (Main.NumberOfSteps * 100), Main.Distance / Main.NumberOfSteps, Main.Distance, calc_type);
            }
        }

        public void proceed_settings()
        {
            toolStripStatusLabel1.Text = "Calculation started.";
            statusStrip1.Refresh();
            this.UseWaitCursor = true;
            Cursor.Current = Cursors.WaitCursor;
            data_transformation();
            double progress = 0;
            double Electric_field = 0, Concentration = 0;
            bool Electric_field_IsChanged = false;
            int Electric_field_steps_cnt = 1, Concentration_steps_cnt = 1;
            total_cycle = 1;
            actual_cycle = 1;
            bool Electric_field_Continue = false, Concentration_continue = false;
            double Electric_field_final = 0, Concentration_final = 0;
            bool OK_status = true;
            start_time = DateTime.Now;
            // data storage
            int progress_steps = 1;
            tabulka = new List<double[,]>();
            data_dim = 1;
            if (checkBox1.Checked && (comboBox1.SelectedItem != null))
            {
                data_dim++;
                try
                {
                    Concentration_steps_cnt = Convert.ToInt32(Convertor(textBox3.Text));
                    progress_steps *= Concentration_steps_cnt;
                }
                catch { MessageBox.Show("Unable to estimate \"Concentration_steps_cnt\" value !"); OK_status = false; }
                try
                {
                    Concentration_final = Convertor(textBox2.Text);
                }
                catch { MessageBox.Show("Unable to estimate \"Concentration_final\" value !"); OK_status = false; }
                if (Concentration == Concentration_final)
                { MessageBox.Show("Start and final concentration value is identical ! "); OK_status = false; }
                if (Concentration_steps_cnt < 1)
                { MessageBox.Show("Step of E/N needs to be positive integer ! "); OK_status = false; }
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item == (Items_used)comboBox1.SelectedItem)
                    {
                        Concentration_name = Item.name;
                        Concentration = Item.concentration;
                    }
                }
                Main.conc_start = Concentration;
                Main.conc_end = Concentration_final;
                Main.conc_steps = Concentration_steps_cnt;
                is_con_din = true;
            }
            else
            {
                is_con_din = false;
            }
            if (checkBox2.Checked)
            {
                is_ele = true;
                if (radioButton2.Checked)
                {
                    data_dim++;
                    is_ele_din = true;
                }
                else
                {
                    is_ele_din = false;
                }
                Electric_field_IsChanged = true;
                try
                {
                    Electric_field = Convertor(textBox4.Text);
                }
                catch { MessageBox.Show("Unable to estimate \"Electric_field\" value !"); OK_status = false; }
                if (radioButton2.Checked)
                {
                    try
                    {
                        Electric_field_steps_cnt = Convert.ToInt32(Convertor(textBox6.Text));
                        progress_steps *= Electric_field_steps_cnt;
                    }
                    catch { MessageBox.Show("Unable to estimate \"Electric_field_steps_cnt\" value !"); OK_status = false; }
                    try
                    {
                        Electric_field_final = Convertor(textBox5.Text);
                    }
                    catch { MessageBox.Show("Unable to estimate \"Electric_field_final\" value !"); OK_status = false; }
                    if (Electric_field == Electric_field_final)
                    { MessageBox.Show("Start and final E/N  value is identical ! "); OK_status = false; }
                    if (Electric_field_steps_cnt < 1)
                    { MessageBox.Show("Step of E/N needs to be positive integer ! "); OK_status = false; }
                }
                Main.field_end = Electric_field_final;
                Main.field_start = Electric_field;
                Main.field_steps = Electric_field_steps_cnt;
            }
            else
            {
                is_ele = false;
            }
            // progres visualization
            double memmory_size = Convert.ToInt32(Main.NumberOfSteps);
            if (checkBox1.Checked)
            {
                memmory_size = memmory_size * Concentration_steps_cnt;//Concentration_steps_cnt;
                total_cycle = total_cycle * Concentration_steps_cnt;//Concentration_steps_cnt;
            }
            if (checkBox2.Checked)
            {
                memmory_size = memmory_size * Electric_field_steps_cnt;
                total_cycle = total_cycle * Electric_field_steps_cnt;
            }
            if (memmory_size > 50000000)
            {
                if((MessageBox.Show("Your calculation will alocated significant amount of computer memory (more than 50M values). Please, consider reduction of the calculation density.   \nDo you want to continue?", "Memory overflow risk!",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == DialogResult.No))
                {
                    OK_status = false;
                }
            }
            if (OK_status)
            {
                actual_cycle = 1;
                double progress_differential = 100.0 / progress_steps;
                data_D2 = new List<List<double[,]>>();
                data_D3 = new List<List<List<double[,]>>>();
                current_electric_field = Electric_field;
                do
                { // Electrick field effect analyser
                    foreach (Premenna pre in premenna)
                    {
                        pre.value = pre.initial_value;
                    }
                    double current_concentration = Concentration;
                    do
                    { // Concentration effect analyser
                        calculation_proceed(Main.calc_type, checkBox2.Checked, current_electric_field, Electric_field_IsChanged);
                        if (checkBox1.Checked && (comboBox1.SelectedItem != null))
                        {
                            // save calculation +1D
                            List<double[,]> new_tabulka = new List<double[,]>();
                            foreach (double[,] dou in tabulka)
                            {
                                double[,] new_dou = new double[dou.GetLength(0) + 1, 2];
                                //dou[dou.GetLength(0) - 1] = Math.Round(dou[dou.GetLength(0) - 1], 12);
                                for (int i = 0; i < dou.GetLength(0); i++)
                                {
                                    new_dou[i, 0] = dou[i, 0];
                                    new_dou[i, 1] = dou[i, 1];
                                }
                                new_dou[new_dou.GetLength(0) - 1, 0] = current_concentration;
                                new_dou[new_dou.GetLength(0) - 1, 1] = current_concentration;
                                new_tabulka.Add(new_dou);
                            }
                            data_D2.Add(new_tabulka);
                            // change settings
                            Concentration_continue = true;
                            if (Concentration_name != "")
                            {
                                double diff = Concentration_final - Concentration;
                                double step = diff / (Concentration_steps_cnt - 1);
                                foreach (Premenna pr in premenna)
                                {
                                    if (pr.name == Concentration_name)
                                    {
                                        current_concentration += step;
                                        if (current_concentration > Concentration_final)
                                        {
                                            Concentration_continue = false;
                                        }
                                        else
                                        {
                                            pr.value += step;
                                        }
                                    }
                                    else
                                    {
                                        pr.value = pr.initial_value;
                                    }
                                }
                            }
                        }
                        // progress visualization 
                        progress += progress_differential;
                        if (progress <= 100)
                        {
                            toolStripProgressBar1.Value = Convert.ToInt32(progress);
                        }
                        else
                        {
                            toolStripProgressBar1.Value = 100;
                        }
                        //Time left calculation
                        TimeSpan time_left = DateTime.Now.Subtract(start_time);
                        int time_left_s = Convert.ToInt32(time_left.TotalSeconds * (total_cycle - actual_cycle) / actual_cycle);
                        TimeSpan time_left_show = TimeSpan.FromSeconds(time_left_s);
                        toolStripStatusLabel1.Text = "Time left: " + time_left_show;
                        statusStrip1.Refresh();
                        actual_cycle++;
                    }
                    while (Concentration_continue && calculation_running);
                    if (data_dim == 2)
                    { // one dynamic coeficient - 
                        if (checkBox2.Checked && (radioButton2.Checked))
                        { // concentration has not been dynamic 
                          // save calculation +1D
                            List<double[,]> new_tabulka = new List<double[,]>();
                            foreach (double[,] dou in tabulka)
                            {
                                double[,] new_dou = new double[dou.GetLength(0) + 1, 2];
                                //dou[dou.GetLength(0) - 1] = Math.Round(dou[dou.GetLength(0) - 1], 12);
                                for (int i = 0; i < dou.GetLength(0); i++)
                                {
                                    new_dou[i, 0] = dou[i, 0];
                                    new_dou[i, 1] = dou[i, 1];
                                }
                                new_dou[new_dou.GetLength(0) - 1, 0] = current_electric_field;
                                new_dou[new_dou.GetLength(0) - 1, 1] = current_electric_field;
                                new_tabulka.Add(new_dou);
                            }
                            data_D2.Add(new_tabulka);
                            tabulka = new List<double[,]>();
                            // change settings
                            Electric_field_Continue = true;
                            double diff = Electric_field_final - Electric_field;
                            double step = diff / (Electric_field_steps_cnt - 1);
                            current_electric_field += step;
                            current_electric_field = Math.Round(current_electric_field, 12);
                            if (current_electric_field > Electric_field_final)
                            {
                                Electric_field_Continue = false;
                            }
                        }
                    }
                    if (data_dim == 3)
                    { // both coeficients are dynamic
                        List<List<double[,]>> new_tabulka_2 = new List<List<double[,]>>();
                        foreach (List<double[,]> _tabulka in data_D2)
                        {
                            List<double[,]> new_tabulka = new List<double[,]>();
                            foreach (double[,] dou in _tabulka)
                            {
                                double[,] new_dou = new double[dou.GetLength(0) + 1, 2];
                                //dou[dou.GetLength(0) - 1] = Math.Round(dou[dou.GetLength(0) - 1], 12);
                                for (int i = 0; i < dou.GetLength(0); i++)
                                {
                                    new_dou[i, 0] = dou[i, 0];
                                    new_dou[i, 1] = dou[i, 1];
                                }
                                new_dou[new_dou.GetLength(0) - 1, 0] = current_electric_field;
                                new_dou[new_dou.GetLength(0) - 1, 1] = current_electric_field;
                                new_tabulka.Add(new_dou);
                            }
                            new_tabulka_2.Add(new_tabulka);
                        }
                        data_D3.Add(new_tabulka_2);
                        data_D2 = new List<List<double[,]>>();
                        // change settings
                        Electric_field_Continue = true;
                        double diff = Electric_field_final - Electric_field;
                        double step = diff / (Electric_field_steps_cnt - 1);
                        current_electric_field += step;
                        if (current_electric_field > Electric_field_final)
                        {
                            Electric_field_Continue = false;
                        }
                    }
                }
                while (Electric_field_Continue && calculation_running);
                // save data
                save_data_to_memory();
                Main.calculation_done();
                // start animation  
                visualtization_started = true;
                start_vizualization();
                iniciate_listbox();
                estimate_relative_values();
                visualtization_started = false;
                start_animation();
                this.UseWaitCursor = false;
                Cursor.Current = Cursors.Default;
                toolStripProgressBar1.Value = 0;
                calculated_data = true;
                toolStripStatusLabel1.Text = "Calculation finished";
                calculation_running = false;

            }
            else
            {
                toolStripStatusLabel1.Text = "Calculation failed";
                calculation_running = false;
                this.UseWaitCursor = false;
                Cursor.Current = Cursors.Default;
            }
        }

        void save_data_to_memory()
        {
            //prepare data to be saved
            List<List<List<double[,]>>> Calculation_memory = new List<List<List<double[,]>>>();
            if (data_dim == 1)
            {
                List<List<double[,]>> construct_D2 = new List<List<double[,]>>();
                construct_D2.Add(tabulka);
                Calculation_memory.Add(construct_D2);
            }
            if (data_dim == 2)
            {
                Calculation_memory.Add(data_D2);
            }
            if (data_dim == 3)
            {
                Calculation_memory = data_D3;
            }
            //save data
            List<Reactions_used> reactions_used_to_memory = new List<Reactions_used>();
            foreach(Reactions_used reactions_used in ReactionColection_used)
            {
                reactions_used_to_memory.Add(new Reactions_used(reactions_used));
            }
            List<Items_used> items_used_to_memory = new List<Items_used>();
            foreach (Items_used items_used in ItemColection_used)
            {
                items_used_to_memory.Add(new Items_used(items_used));
            }
            Calculation_setings used_settings = new Calculation_setings();
            Data_storage new_data_storage = new Data_storage(start_time, data_dim, reactions_used_to_memory, items_used_to_memory, Concentration_name, current_electric_field, hlavicka, Calculation_memory, used_settings);
            new_data_storage.Calculation_type(is_time, is_con_din, is_ele, is_ele_din);
            Main.data_storage.Add(new_data_storage);
            Show_data_colection();
        }

        void novapremenna(string s, double Value, double Init_val)
        {
            bool tuje = false;
            foreach (Premenna p in premenna)
            {
                if (p.name == s)
                {
                    tuje = true;
                }
            }
            if (tuje == false)
            {
                premenna.Add(new Premenna() { name = s, value = Value, initial_value = Init_val });
            }
        }

        void novakonstanta(string s, double Value, rate_functions Function)
        {
            bool tuje = false;
            foreach (Konstants k in konstanta)
            {
                if (k.name == s)
                {
                    tuje = true;
                }
            }
            if (tuje == false)
            {
                konstanta.Add(new Konstants() { name = s, value = Value, function = Function });
            }
        }

        void Calculate(bool E_field, double Differencial, double Step, double Lenght, bool Calc_type)
        {
            // calculation of density distribution across the tube 
            int cnt = equation.Count;
            int i = 0;
            double t = 0;
            hlavicka = new string[cnt];
            foreach (Equation eq in equation)
            {
                hlavicka[i] = eq.name;
                i++;
            }
            double[,] d = new double[cnt + 1, 2];
            tabulka = new List<double[,]>();
            i = 0;
            foreach (Equation eq in equation)
            {
                foreach (Premenna pr in premenna)
                {
                    if (pr.name == eq.name)
                    {
                        d[i, 0] = pr.value;
                        i++;
                    }
                }
            }          
            d[cnt, 0] = t;
            d[cnt, 1] = t;
            tabulka.Add(d);
            while (t < Lenght) // go throught drift tube
            {
                double[,] dou = new double[cnt + 1, 2];
                Calculation calculation = new Calculation();
                calculation.e_field = E_field; // presence of e-field
                calculation.calc_type = Calc_type; // type of calcuation
                calculation.dt = Differencial; // calculation differencial
                calculation.calculate_step(equation, konstanta, premenna);
                premenna = calculation.new_premenna; // result
                int j = 0;
                foreach (Equation eq in equation)
                {
                    foreach (Premenna pr in premenna)
                    {
                        if (pr.name == eq.name)
                        {
                            dou[j, 0] = pr.value;
                            j++;
                        }
                    }
                }                
                t = t + Step;
                t = Math.Round(t, 12);
                dou[cnt, 0] = t;
                dou[cnt, 1] = t;
                tabulka.Add(dou); // add step to results
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                comboBox1.Items.Clear();
                foreach (Items_used Item in ItemColection_used)
                {
                    comboBox1.Items.Add(Item);
                }
            }
            else
            {
                comboBox1.Text = "";
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                comboBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item == (Items_used)comboBox1.SelectedItem)
                {
                    if (Item.concentration > 100)
                    {
                        textBox1.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        textBox1.Text = Item.concentration.ToString();
                    }
                    if (Item.concentration > 100)
                    {
                        textBox2.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        textBox2.Text = Item.concentration.ToString();
                    }
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                if (radioButton1.Checked)
                {
                    textBox4.Enabled = true;
                    textBox5.Enabled = false;
                    textBox6.Enabled = false;
                }
                else
                {
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    textBox6.Enabled = true;
                }
            }
            else
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox4.Enabled = true;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox5.Text = "";
                textBox6.Text = "";
            }
            else
            {
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
            }
        }

        #endregion

        #region Fill boxes

        public void show_values()
        {
            fill_items();
            fill_reaction();
            foreach (Items_used items_ in ItemColection_used)
            {
                if (items_.cation == true)
                {
                    comboBox2.Items.Add((Items_used)items_);
                }
            }
            check_button();
        }

        public void fill_items()
        {
            List<string> items_in = new List<string>();
            int premenna_cnt = ItemColection_used.Count;
            if (premenna_cnt <= 10)
            { // nemusime posuvat
                int i = 1;
                foreach (Items_used Item in ItemColection_used)
                {
                    bool is_here = false;
                    foreach (string Items_in in items_in)
                    {
                        if (Items_in == Item.name)
                        {
                            is_here = true;
                        }
                    }
                    if (!is_here)
                    {
                        items_in.Add(Item.name);
                        if (i == 1)
                        {
                            textBoxC1.Text = Item.name;
                            labelG1.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD1.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD1.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 2)
                        {
                            textBoxC2.Text = Item.name;
                            labelG2.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD2.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD2.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 3)
                        {
                            textBoxC3.Text = Item.name;
                            labelG3.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD3.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD3.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 4)
                        {
                            textBoxC4.Text = Item.name;
                            labelG4.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD4.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD4.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 5)
                        {
                            textBoxC5.Text = Item.name;
                            labelG5.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD5.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD5.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 6)
                        {
                            textBoxC6.Text = Item.name;
                            labelG6.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD6.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD6.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 7)
                        {
                            textBoxC7.Text = Item.name;
                            labelG7.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD7.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD7.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 8)
                        {
                            textBoxC8.Text = Item.name;
                            labelG8.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD8.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD8.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 9)
                        {
                            textBoxC9.Text = Item.name;
                            labelG9.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD9.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD9.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 10)
                        {
                            textBoxC10.Text = Item.name;
                            labelG10.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD10.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD10.Text = Item.concentration.ToString();
                            }
                        }
                        i++;
                    }
                }
                vScrollBar2.Enabled = false;
            }
            else
            { // musime posuvat bar
                int i = 1;
                foreach (Items_used Item in ItemColection_used)
                {
                    bool is_here = false;
                    foreach (string Items_in in items_in)
                    {
                        if (Items_in == Item.name)
                        {
                            is_here = true;
                        }
                    }
                    if (!is_here)
                    {
                        items_in.Add(Item.name);
                        if (i == 1)
                        {
                            textBoxC1.Text = Item.name;
                            labelG1.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD1.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD1.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 2)
                        {
                            textBoxC2.Text = Item.name;
                            labelG2.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD2.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD2.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 3)
                        {
                            textBoxC3.Text = Item.name;
                            labelG3.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD3.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD3.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 4)
                        {
                            textBoxC4.Text = Item.name;
                            labelG4.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD4.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD4.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 5)
                        {
                            textBoxC5.Text = Item.name;
                            labelG5.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD5.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD5.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 6)
                        {
                            textBoxC6.Text = Item.name;
                            labelG6.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD6.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD6.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 7)
                        {
                            textBoxC7.Text = Item.name;
                            labelG7.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD7.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD7.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 8)
                        {
                            textBoxC8.Text = Item.name;
                            labelG8.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD8.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD8.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 9)
                        {
                            textBoxC9.Text = Item.name;
                            labelG9.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD9.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD9.Text = Item.concentration.ToString();
                            }
                        }
                        if (i == 10)
                        {
                            textBoxC10.Text = Item.name;
                            labelG10.Text = Item.group_ID.ToString();
                            if (Item.concentration > 100)
                            {
                                textBoxD10.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                textBoxD10.Text = Item.concentration.ToString();
                            }
                        }
                        i++;
                    }
                }
                vScrollBar2.LargeChange = 101 * 10 / premenna_cnt;
                vScrollBar2.Value = 1;
                interval_pocet_items = (ItemColection_used.Count) - 9;
                double b = (10 / Convert.ToDouble(ItemColection_used.Count));
                double a = 101 * (1 - b);
                sirka_intervalu_items = a / interval_pocet_items;
                vScrollBar2.Enabled = true;
            }
        }

        public void fill_reaction()
        {
            int both_side_reactions = 0;
            foreach (Reactions_used R in ReactionColection_used)
            {
                if (R.reaction_type == 2)
                {
                    both_side_reactions++;
                }
            }
            int premenna_cnt = ReactionColection_used.Count + both_side_reactions;
            if (premenna_cnt <= 10)
            { // nemusime posuvat
                int i = 1;
                foreach (Reactions_used rection in ReactionColection_used)
                {
                    string text_to_box = rection.name;
                    if (rection.reaction_type == 2)
                    {
                        text_to_box = rection.Specific_name();
                    }
                    if (i == 1) { textBoxA1.Text = text_to_box; textBoxB1.Text = rection.rate_konstant.representation; }
                    if (i == 2) { textBoxA2.Text = text_to_box; textBoxB2.Text = rection.rate_konstant.representation; }
                    if (i == 3) { textBoxA3.Text = text_to_box; textBoxB3.Text = rection.rate_konstant.representation; }
                    if (i == 4) { textBoxA4.Text = text_to_box; textBoxB4.Text = rection.rate_konstant.representation; }
                    if (i == 5) { textBoxA5.Text = text_to_box; textBoxB5.Text = rection.rate_konstant.representation; }
                    if (i == 6) { textBoxA6.Text = text_to_box; textBoxB6.Text = rection.rate_konstant.representation; }
                    if (i == 7) { textBoxA7.Text = text_to_box; textBoxB7.Text = rection.rate_konstant.representation; }
                    if (i == 8) { textBoxA8.Text = text_to_box; textBoxB8.Text = rection.rate_konstant.representation; }
                    if (i == 9) { textBoxA9.Text = text_to_box; textBoxB9.Text = rection.rate_konstant.representation; }
                    if (i == 10) { textBoxA10.Text = text_to_box; textBoxB10.Text = rection.rate_konstant.representation; }
                    i++;
                    if (rection.reaction_type == 2)
                    {
                        text_to_box = rection.Specific_name_();
                        if (i == 1) { textBoxA1.Text = text_to_box; textBoxB1.Text = rection.rate_konstant_.representation; }
                        if (i == 2) { textBoxA2.Text = text_to_box; textBoxB2.Text = rection.rate_konstant_.representation; }
                        if (i == 3) { textBoxA3.Text = text_to_box; textBoxB3.Text = rection.rate_konstant_.representation; }
                        if (i == 4) { textBoxA4.Text = text_to_box; textBoxB4.Text = rection.rate_konstant_.representation; }
                        if (i == 5) { textBoxA5.Text = text_to_box; textBoxB5.Text = rection.rate_konstant_.representation; }
                        if (i == 6) { textBoxA6.Text = text_to_box; textBoxB6.Text = rection.rate_konstant_.representation; }
                        if (i == 7) { textBoxA7.Text = text_to_box; textBoxB7.Text = rection.rate_konstant_.representation; }
                        if (i == 8) { textBoxA8.Text = text_to_box; textBoxB8.Text = rection.rate_konstant_.representation; }
                        if (i == 9) { textBoxA9.Text = text_to_box; textBoxB9.Text = rection.rate_konstant_.representation; }
                        if (i == 10) { textBoxA10.Text = text_to_box; textBoxB10.Text = rection.rate_konstant_.representation; }
                        i++;
                    }
                }
                vScrollBar1.Enabled = false;
            }
            else
            { // musime posuvat bar
                int i = 1;
                foreach (Reactions_used rection in ReactionColection_used)
                {
                    string text_to_box = rection.name;
                    if (rection.reaction_type == 2)
                    {
                        text_to_box = rection.Specific_name();
                    }
                    if (i == 1) { textBoxA1.Text = text_to_box; textBoxB1.Text = rection.rate_konstant.representation; }
                    if (i == 2) { textBoxA2.Text = text_to_box; textBoxB2.Text = rection.rate_konstant.representation; }
                    if (i == 3) { textBoxA3.Text = text_to_box; textBoxB3.Text = rection.rate_konstant.representation; }
                    if (i == 4) { textBoxA4.Text = text_to_box; textBoxB4.Text = rection.rate_konstant.representation; }
                    if (i == 5) { textBoxA5.Text = text_to_box; textBoxB5.Text = rection.rate_konstant.representation; }
                    if (i == 6) { textBoxA6.Text = text_to_box; textBoxB6.Text = rection.rate_konstant.representation; }
                    if (i == 7) { textBoxA7.Text = text_to_box; textBoxB7.Text = rection.rate_konstant.representation; }
                    if (i == 8) { textBoxA8.Text = text_to_box; textBoxB8.Text = rection.rate_konstant.representation; }
                    if (i == 9) { textBoxA9.Text = text_to_box; textBoxB9.Text = rection.rate_konstant.representation; }
                    if (i == 10) { textBoxA10.Text = text_to_box; textBoxB10.Text = rection.rate_konstant.representation; }
                    i++;
                    if (rection.reaction_type == 2)
                    {
                        text_to_box = rection.Specific_name_();
                        if (i == 1) { textBoxA1.Text = text_to_box; textBoxB1.Text = rection.rate_konstant_.representation; }
                        if (i == 2) { textBoxA2.Text = text_to_box; textBoxB2.Text = rection.rate_konstant_.representation; }
                        if (i == 3) { textBoxA3.Text = text_to_box; textBoxB3.Text = rection.rate_konstant_.representation; }
                        if (i == 4) { textBoxA4.Text = text_to_box; textBoxB4.Text = rection.rate_konstant_.representation; }
                        if (i == 5) { textBoxA5.Text = text_to_box; textBoxB5.Text = rection.rate_konstant_.representation; }
                        if (i == 6) { textBoxA6.Text = text_to_box; textBoxB6.Text = rection.rate_konstant_.representation; }
                        if (i == 7) { textBoxA7.Text = text_to_box; textBoxB7.Text = rection.rate_konstant_.representation; }
                        if (i == 8) { textBoxA8.Text = text_to_box; textBoxB8.Text = rection.rate_konstant_.representation; }
                        if (i == 9) { textBoxA9.Text = text_to_box; textBoxB9.Text = rection.rate_konstant_.representation; }
                        if (i == 10) { textBoxA10.Text = text_to_box; textBoxB10.Text = rection.rate_konstant_.representation; }
                        i++;
                    }
                }
                vScrollBar1.LargeChange = 101 * 10 / premenna_cnt;
                vScrollBar1.Value = 1;
                interval_pocet_reaction = (ReactionColection_used.Count + both_side_reactions) - 9;
                double b = (10 / Convert.ToDouble(ReactionColection_used.Count + both_side_reactions));
                double a = 101 * (1 - b);
                sirka_intervalu_rection = a / interval_pocet_reaction;
                vScrollBar1.Enabled = true;
            }
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            if(calculation_running)
            {
                calculation_running = false;

            }
            else
            {
                // spustenie vypoctu
                if (Load_type == 1 && (comboBox2.SelectedItem == null || listBox1.Items == null))
                {
                    MessageBox.Show(" For this type of experiemntal data, PRI and SRI must be specified!");
                }
                else
                {
                    calculation_running = true;
                    proceed_settings();
                }
            }
        } 

        private void button_cancle_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            stop_write = true;
            int value = vScrollBar1.Value;
            if (vScrollBar1.Value > 101 - vScrollBar1.LargeChange)
            {
                value = 101 - vScrollBar1.LargeChange;
            }
            int pomer = value / Convert.ToInt16(sirka_intervalu_rection);
            if (pomer > interval_pocet_reaction - 1)
            {
                pomer = interval_pocet_reaction - 1;
            }
            if (pomer != aktual_pomer_reaction)
            {
                int i = 1 - pomer;
                foreach (Reactions_used rection in ReactionColection_used)
                {
                    string text_to_box = rection.name;
                    if (rection.reaction_type == 2)
                    {
                        text_to_box = rection.Specific_name();
                    }
                    if (i == 1) { textBoxA1.Text = text_to_box; textBoxB1.Text = rection.rate_konstant.representation; }
                    if (i == 2) { textBoxA2.Text = text_to_box; textBoxB2.Text = rection.rate_konstant.representation; }
                    if (i == 3) { textBoxA3.Text = text_to_box; textBoxB3.Text = rection.rate_konstant.representation; }
                    if (i == 4) { textBoxA4.Text = text_to_box; textBoxB4.Text = rection.rate_konstant.representation; }
                    if (i == 5) { textBoxA5.Text = text_to_box; textBoxB5.Text = rection.rate_konstant.representation; }
                    if (i == 6) { textBoxA6.Text = text_to_box; textBoxB6.Text = rection.rate_konstant.representation; }
                    if (i == 7) { textBoxA7.Text = text_to_box; textBoxB7.Text = rection.rate_konstant.representation; }
                    if (i == 8) { textBoxA8.Text = text_to_box; textBoxB8.Text = rection.rate_konstant.representation; }
                    if (i == 9) { textBoxA9.Text = text_to_box; textBoxB9.Text = rection.rate_konstant.representation; }
                    if (i == 10) { textBoxA10.Text = text_to_box; textBoxB10.Text = rection.rate_konstant.representation; }
                    i++;
                    if (rection.reaction_type == 2)
                    {
                        text_to_box = rection.Specific_name_();
                        if (i == 1) { textBoxA1.Text = text_to_box; textBoxB1.Text = rection.rate_konstant_.representation; }
                        if (i == 2) { textBoxA2.Text = text_to_box; textBoxB2.Text = rection.rate_konstant_.representation; }
                        if (i == 3) { textBoxA3.Text = text_to_box; textBoxB3.Text = rection.rate_konstant_.representation; }
                        if (i == 4) { textBoxA4.Text = text_to_box; textBoxB4.Text = rection.rate_konstant_.representation; }
                        if (i == 5) { textBoxA5.Text = text_to_box; textBoxB5.Text = rection.rate_konstant_.representation; }
                        if (i == 6) { textBoxA6.Text = text_to_box; textBoxB6.Text = rection.rate_konstant_.representation; }
                        if (i == 7) { textBoxA7.Text = text_to_box; textBoxB7.Text = rection.rate_konstant_.representation; }
                        if (i == 8) { textBoxA8.Text = text_to_box; textBoxB8.Text = rection.rate_konstant_.representation; }
                        if (i == 9) { textBoxA9.Text = text_to_box; textBoxB9.Text = rection.rate_konstant_.representation; }
                        if (i == 10) { textBoxA10.Text = text_to_box; textBoxB10.Text = rection.rate_konstant_.representation; }
                        i++;
                    }
                }
            }
            aktual_pomer_reaction = pomer;
            stop_write = false;
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
            if (pomer != aktual_pomer_items)
            {
                int i = 1 - pomer;
                foreach (Items_used Item in ItemColection_used)
                {
                    if (i == 1)
                    {
                        textBoxC1.Text = Item.name;
                        labelG1.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD1.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD1.Text = Item.concentration.ToString();
                        }
                    }
                    if (i == 2)
                    {
                        textBoxC2.Text = Item.name;
                        labelG2.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD2.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD2.Text = Item.concentration.ToString();
                        }
                    }
                    if (i == 3)
                    {
                        textBoxC3.Text = Item.name;
                        labelG3.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD3.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD3.Text = Item.concentration.ToString();
                        }
                    }
                    if (i == 4)
                    {
                        textBoxC4.Text = Item.name;
                        labelG4.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD4.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD4.Text = Item.concentration.ToString();
                        }
                    }
                    if (i == 5)
                    {
                        textBoxC5.Text = Item.name;
                        labelG5.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD5.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD5.Text = Item.concentration.ToString();
                        }
                    }
                    if (i == 6)
                    {
                        textBoxC6.Text = Item.name;
                        labelG6.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD6.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD6.Text = Item.concentration.ToString();
                        }
                    }
                    if (i == 7)
                    {
                        textBoxC7.Text = Item.name;
                        labelG7.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD7.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD7.Text = Item.concentration.ToString();
                        }
                    }
                    if (i == 8)
                    {
                        textBoxC8.Text = Item.name;
                        labelG8.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD8.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD8.Text = Item.concentration.ToString();
                        }
                    }
                    if (i == 9)
                    {
                        textBoxC9.Text = Item.name;
                        labelG9.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD9.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD9.Text = Item.concentration.ToString();
                        }
                    }
                    if (i == 10)
                    {
                        textBoxC10.Text = Item.name;
                        labelG10.Text = Item.group_ID.ToString();
                        if (Item.concentration > 100)
                        {
                            textBoxD10.Text = Item.concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            textBoxD10.Text = Item.concentration.ToString();
                        }
                    }
                    i++;
                }
            }
            aktual_pomer_items = pomer;
        }

        double Convertor(string Str)
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
                    }
                }
            }
            return dou;
        }

        #endregion

        #region textBox_textChange

        private void textBoxB1_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA1.Text)
                    {
                        reaction.rate_konstant.update(textBoxB1.Text);
                    }
                    if (reaction.name != textBoxA1.Text && reaction.Specific_name() == textBoxA1.Text)
                    {
                        reaction.rate_konstant.update(textBoxB1.Text);
                    }
                    if (reaction.name != textBoxA1.Text && reaction.Specific_name_() == textBoxA1.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB1.Text);
                    }
                }
            }
        }

        private void textBoxB2_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA2.Text)
                    {
                        reaction.rate_konstant.update(textBoxB2.Text);
                    }
                    if (reaction.name != textBoxA2.Text && reaction.Specific_name() == textBoxA2.Text)
                    {
                        reaction.rate_konstant.update(textBoxB2.Text);
                    }
                    if (reaction.name != textBoxA2.Text && reaction.Specific_name_() == textBoxA2.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB2.Text);
                    }
                }
            }
        }

        private void textBoxB3_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA3.Text)
                    {
                        reaction.rate_konstant.update(textBoxB3.Text);
                    }
                    if (reaction.name != textBoxA3.Text && reaction.Specific_name() == textBoxA3.Text)
                    {
                        reaction.rate_konstant.update(textBoxB3.Text);
                    }
                    if (reaction.name != textBoxA3.Text && reaction.Specific_name_() == textBoxA3.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB3.Text);
                    }
                }
            }
        }

        private void textBoxB4_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA4.Text)
                    {
                        reaction.rate_konstant.update(textBoxB4.Text);
                    }
                    if (reaction.name != textBoxA4.Text && reaction.Specific_name() == textBoxA4.Text)
                    {
                        reaction.rate_konstant.update(textBoxB4.Text);
                    }
                    if (reaction.name != textBoxA4.Text && reaction.Specific_name_() == textBoxA4.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB4.Text);
                    }
                }
            }
        }

        private void textBoxB5_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA5.Text)
                    {
                        reaction.rate_konstant.update(textBoxB5.Text);
                    }
                    if (reaction.name != textBoxA5.Text && reaction.Specific_name() == textBoxA5.Text)
                    {
                        reaction.rate_konstant.update(textBoxB5.Text);
                    }
                    if (reaction.name != textBoxA5.Text && reaction.Specific_name_() == textBoxA5.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB5.Text);
                    }
                }
            }
        }

        private void textBoxB6_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA6.Text)
                    {
                        reaction.rate_konstant.update(textBoxB6.Text);
                    }
                    if (reaction.name != textBoxA6.Text && reaction.Specific_name() == textBoxA6.Text)
                    {
                        reaction.rate_konstant.update(textBoxB6.Text);
                    }
                    if (reaction.name != textBoxA6.Text && reaction.Specific_name_() == textBoxA6.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB6.Text);
                    }
                }
            }
        }

        private void textBoxB7_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA7.Text)
                    {
                        reaction.rate_konstant.update(textBoxB7.Text);
                    }
                    if (reaction.name != textBoxA7.Text && reaction.Specific_name() == textBoxA7.Text)
                    {
                        reaction.rate_konstant.update(textBoxB7.Text);
                    }
                    if (reaction.name != textBoxA7.Text && reaction.Specific_name_() == textBoxA7.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB7.Text);
                    }
                }
            }
        }

        private void textBoxB8_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA8.Text)
                    {
                        reaction.rate_konstant.update(textBoxB8.Text);
                    }
                    if (reaction.name != textBoxA8.Text && reaction.Specific_name() == textBoxA8.Text)
                    {
                        reaction.rate_konstant.update(textBoxB8.Text);
                    }
                    if (reaction.name != textBoxA8.Text && reaction.Specific_name_() == textBoxA8.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB8.Text);
                    }
                }
            }
        }

        private void textBoxB9_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA9.Text)
                    {
                        reaction.rate_konstant.update(textBoxB9.Text);
                    }
                    if (reaction.name != textBoxA9.Text && reaction.Specific_name() == textBoxA9.Text)
                    {
                        reaction.rate_konstant.update(textBoxB9.Text);
                    }
                    if (reaction.name != textBoxA9.Text && reaction.Specific_name_() == textBoxA9.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB9.Text);
                    }
                }
            }
        }

        private void textBoxB10_TextChanged(object sender, EventArgs e)
        {
            if (!stop_write)
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction.name == textBoxA10.Text)
                    {
                        reaction.rate_konstant.update(textBoxB10.Text);
                    }
                    if (reaction.name != textBoxA10.Text && reaction.Specific_name() == textBoxA10.Text)
                    {
                        reaction.rate_konstant.update(textBoxB10.Text);
                    }
                    if (reaction.name != textBoxA10.Text && reaction.Specific_name_() == textBoxA10.Text)
                    {
                        reaction.rate_konstant_.update(textBoxB10.Text);
                    }
                }
            }
        }

        private void textBoxD1_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC1.Text)
                {
                    Item.concentration = Convertor(textBoxD1.Text);
                }
            }
        }

        private void textBoxD2_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC2.Text)
                {
                    Item.concentration = Convertor(textBoxD2.Text);
                }
            }
        }

        private void textBoxD3_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC3.Text)
                {
                    Item.concentration = Convertor(textBoxD3.Text);
                }
            }
        }

        private void textBoxD4_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC4.Text)
                {
                    Item.concentration = Convertor(textBoxD4.Text);
                }
            }
        }

        private void textBoxD5_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC5.Text)
                {
                    Item.concentration = Convertor(textBoxD5.Text);
                }
            }
        }

        private void textBoxD6_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC6.Text)
                {
                    Item.concentration = Convertor(textBoxD6.Text);
                }
            }
        }

        private void textBoxD7_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC7.Text)
                {
                    Item.concentration = Convertor(textBoxD7.Text);
                }
            }
        }

        private void textBoxD8_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC8.Text)
                {
                    Item.concentration = Convertor(textBoxD8.Text);
                }
            }
        }

        private void textBoxD9_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC9.Text)
                {
                    Item.concentration = Convertor(textBoxD9.Text);
                }
            }
        }

        private void textBoxD10_TextChanged(object sender, EventArgs e)
        {
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.name == textBoxC10.Text)
                {
                    Item.concentration = Convertor(textBoxD10.Text);
                }
            }
        }

        private void labelG1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Only occurs when the Left button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC1.Text)
                    {
                        int nmb = Convert.ToInt32(labelG1.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG1.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC1.Text)
                    {
                        int nmb = Convert.ToInt32(labelG1.Text);
                        nmb--;
                        Item.group_ID = nmb;
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
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC2.Text)
                    {
                        int nmb = Convert.ToInt32(labelG2.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG2.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC2.Text)
                    {
                        int nmb = Convert.ToInt32(labelG2.Text);
                        nmb--;
                        Item.group_ID = nmb;
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
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC3.Text)
                    {
                        int nmb = Convert.ToInt32(labelG3.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG3.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC3.Text)
                    {
                        int nmb = Convert.ToInt32(labelG3.Text);
                        nmb--;
                        Item.group_ID = nmb;
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
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC4.Text)
                    {
                        int nmb = Convert.ToInt32(labelG4.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG4.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC4.Text)
                    {
                        int nmb = Convert.ToInt32(labelG4.Text);
                        nmb--;
                        Item.group_ID = nmb;
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
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC5.Text)
                    {
                        int nmb = Convert.ToInt32(labelG5.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG5.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC5.Text)
                    {
                        int nmb = Convert.ToInt32(labelG5.Text);
                        nmb--;
                        Item.group_ID = nmb;
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
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC6.Text)
                    {
                        int nmb = Convert.ToInt32(labelG6.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG6.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC6.Text)
                    {
                        int nmb = Convert.ToInt32(labelG6.Text);
                        nmb--;
                        Item.group_ID = nmb;
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
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC7.Text)
                    {
                        int nmb = Convert.ToInt32(labelG7.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG7.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC7.Text)
                    {
                        int nmb = Convert.ToInt32(labelG7.Text);
                        nmb--;
                        Item.group_ID = nmb;
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
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC8.Text)
                    {
                        int nmb = Convert.ToInt32(labelG8.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG8.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC8.Text)
                    {
                        int nmb = Convert.ToInt32(labelG8.Text);
                        nmb--;
                        Item.group_ID = nmb;
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
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC9.Text)
                    {
                        int nmb = Convert.ToInt32(labelG9.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG9.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC9.Text)
                    {
                        int nmb = Convert.ToInt32(labelG9.Text);
                        nmb--;
                        Item.group_ID = nmb;
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
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC10.Text)
                    {
                        int nmb = Convert.ToInt32(labelG10.Text);
                        nmb++;
                        Item.group_ID = nmb;
                        labelG10.Text = nmb.ToString();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // Only occurs when the Right button is released
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == textBoxC10.Text)
                    {
                        int nmb = Convert.ToInt32(labelG10.Text);
                        nmb--;
                        Item.group_ID = nmb;
                        labelG10.Text = nmb.ToString();
                    }
                }
            }
        }

        #endregion

        private bool is_time = true;
        private bool is_con_din = false;
        private bool is_ele = false;
        private bool is_ele_din = false;
        public bool dinamic_mode { get; set; }
        public static string Popis_X { get; set; }
        public double _concentration;
        public double _distance;
        public double _E_field;
        private bool conc_roll = false;
        private bool dist_roll = false;
        private bool ele_roll = false;
        private bool convolution = false;
        private List<string> titul;
        private List<List<double[]>> export = new List<List<double[]>>();
        public static int[] Load_hlavicka;
        public static string[] Load_hlavicka_str; 
        public static double[,] Loaded_data;
        public static bool relative_values = false;
        public static int Load_type = -1;
        public static bool Load_relative = false;
        public static int Marker_Size = 2;
        public static MarkerType marker_type = MarkerType.Square;

        public void Results_KineticModel()
        {
            trackBar_dist.Maximum = Convert.ToInt32(Main.NumberOfSteps);
            trackBar_dist.Value = trackBar_dist.Maximum;
            if (is_con_din)
            {
                trackBar_conc.Maximum = Convert.ToInt32(Main.conc_steps - 1);
            }
            if (is_ele_din)
            {
                trackBar_ele.Maximum = Convert.ToInt32(Main.field_steps - 1);
            }
        }

        public void start_vizualization()
        {
            Results_KineticModel();
            if(is_time)
            {
                _distance = Main.Time_duration;
            }
            else
            {
                _distance = Main.Distance;
            }
            if (Main.calc_type) // && !is_ele
            {
                radioButton_kinetic.Checked = true;
                radioButton_dinamic.Enabled = false;
                radioButton_ms.Enabled = true;
                radioButton3.Enabled = false;
                radioButton_kinetic.Checked = true;
                is_time = true;
                label16.Text = "Time [us]:";
                Popis_X = "Time [us]";
                button_t_x.Enabled = true;
            }
            if (!Main.calc_type)
            {
                radioButton_kinetic.Checked = true;
                radioButton_dinamic.Enabled = false;
                radioButton_ms.Enabled = true;
                radioButton3.Enabled = false;
                radioButton_kinetic.Checked = true;
                is_time = false;
                label16.Text = "Distance [cm]:";
                Popis_X = "Distance [cm]";
                if(!is_ele || !is_ele_din)
                {
                    button_t_x.Enabled = true;
                }
                else
                {
                    button_t_x.Enabled = false;
                }
            }
            if (is_con_din)
            {
                radioButton_dinamic.Checked = true;
                radioButton_dinamic.Enabled = true;

                if (!is_ele_din)
                {
                    radioButton3.Checked = false;
                    radioButton3.Enabled = false;
                }
            }
            if (is_ele)
            {
                textBox_ele.Text = Main.field_start.ToString();
            }
            if (is_ele_din)
            {
                radioButton3.Checked = true;
                radioButton3.Enabled = true;
                if (!is_con_din)
                {
                    radioButton_dinamic.Checked = false;
                    radioButton_dinamic.Enabled = false;
                }
            }
            if (Load_type == 1) // SIFT MIM data
            {
                radioButton_kinetic.Enabled = false;
                radioButton_dinamic.Enabled = true;
                radioButton_ms.Enabled = false;
                radioButton3.Enabled = false;
                radioButton_dinamic.Checked = true;
            }
            if (Load_type == 2) // E/N data
            {
                radioButton_kinetic.Enabled = false;
                radioButton_dinamic.Enabled = false;
                radioButton_ms.Enabled = false;
                radioButton3.Enabled = true;
                radioButton3.Checked = true;
            }
        }

        public void iniciate_listbox()
        {
            int c = 0;
            List<Legend_entity> Legend_list = new List<Legend_entity>();
            foreach (var obj in checkedListBox1.Items)
            {
                Legend_entity entity = new Legend_entity();
                entity.name = obj.ToString();
                entity.status = checkedListBox1.GetItemChecked(c);
                c++;
                Legend_list.Add(entity);
            }
            checkedListBox1.Items.Clear();
            if (radioButton_kinetic.Checked)
            {
                foreach (string str in hlavicka)
                {
                    bool done = false;
                    foreach (Legend_entity entity in Legend_list)
                    {
                        if (str == entity.name)
                        {
                            checkedListBox1.Items.Add(str, entity.status);
                            done = true;
                        }
                    }
                    if (done == false)
                    {
                        checkedListBox1.Items.Add(str, true);
                    }
                }
            }
            if (radioButton_ms.Checked)
            {
                foreach (string str in hlavicka)
                {
                    foreach (Items Item in Main.ItemColection)
                    {
                        if ((Item.name == str) && Item.cation)
                        {
                            bool done = false;
                            foreach (Legend_entity entity in Legend_list)
                            {
                                if (str == entity.name)
                                {
                                    checkedListBox1.Items.Add(str, entity.status);
                                    done = true;
                                }
                            }
                            if (done == false)
                            {
                                checkedListBox1.Items.Add(str, true);
                            }
                        }
                    }
                }
                checkedListBox1.Items.Add("Sum", true);
            }
            if (radioButton_dinamic.Checked || radioButton3.Checked)
            {
                foreach (string str in hlavicka)
                {
                    bool done = false;
                    foreach (Legend_entity entity in Legend_list)
                    {
                        if (str == entity.name)
                        {
                            checkedListBox1.Items.Add(str, entity.status);
                            done = true;
                        }
                    }
                    if (done == false)
                    {
                        checkedListBox1.Items.Add(str, true);
                    }
                }
            }
            if (Load_type != -1)
            {
                for (int i = 1; i < Load_hlavicka_str.GetLength(0); i++)
                {
                    string str = Load_hlavicka_str[i].ToString();
                    bool done = false;
                    foreach (Legend_entity entity in Legend_list)
                    {
                        if (str == entity.name)
                        {
                            checkedListBox1.Items.Add(str, entity.status);
                            done = true;
                        }
                    }
                    if (done == false)
                    {
                        checkedListBox1.Items.Add(str, true);
                    }
                }
            }
        }

        public void estimate_relative_values()
        {
            // reclaculation of absolute values to relative if requested
            if (checkBox3.Checked)
            {
                if (hlavicka != null)
                {
                    // creation of index field
                    int[] index_matrix = new int[hlavicka.Length];
                    int i = 0;
                    List<string> doubler_check = new List<string>(); // check for doublers 
                    foreach (string name in hlavicka)
                    {
                        foreach (Items_used Item in ItemColection_used)
                        {
                            if ((name == Item.name) && !doubler_check.Contains(name))
                            {
                                index_matrix[i] = Item.group_ID;
                                doubler_check.Add(name);
                                i++;
                            }
                        }
                    }
                    List<int> index_list = new List<int>();
                    foreach (int ID in index_matrix)
                    {
                        if (!index_list.Contains(ID))
                        {
                            index_list.Add(ID);
                        }
                    }
                    if ((index_matrix.Length > 0) && (index_list != null))
                    {
                        if (is_ele_din && is_con_din) // 3D pole
                        {
                            foreach (List<List<double[,]>> table_2D in data_D3)
                            {
                                foreach (List<double[,]> table in table_2D) // concentraion
                                {
                                    foreach (double[,] dou in table) // coordinates
                                    {
                                        foreach (int index in index_list)
                                        {
                                            double sum = 0;
                                            for (int j = 0; j < index_matrix.Length; j++)
                                            {
                                                if (index == index_matrix[j])
                                                {
                                                    sum += dou[j, 0];
                                                }
                                            }
                                            for (int j = 0; j < index_matrix.Length; j++)
                                            {
                                                if (index == index_matrix[j])
                                                {
                                                    dou[j, 1] = dou[j, 0] / sum;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if ((is_ele_din && !is_con_din) || (!is_ele_din && is_con_din)) // 2D pole
                        {
                            foreach (List<double[,]> table in data_D2) // concentraion 
                            {
                                foreach (double[,] dou in table) // coordinates
                                {
                                    foreach (int index in index_list)
                                    {
                                        double sum = 0;
                                        for (int j = 0; j < index_matrix.Length; j++)
                                        {
                                            if (index == index_matrix[j])
                                            {
                                                sum += dou[j, 0];
                                            }
                                        }
                                        for (int j = 0; j < index_matrix.Length; j++)
                                        {
                                            if (index == index_matrix[j])
                                            {
                                                dou[j, 1] = dou[j, 0] / sum;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!is_con_din && !is_ele_din) //1D pole
                        {
                            foreach (double[,] dou in tabulka)
                            {
                                foreach(int index in index_list)
                                {
                                    double sum = 0;
                                    for(int j = 0; j < index_matrix.Length; j++)
                                    {
                                        if(index == index_matrix[j])
                                        {
                                            sum += dou[j,0];
                                        }
                                    }
                                    for (int j = 0; j < index_matrix.Length; j++)
                                    {
                                        if (index == index_matrix[j])
                                        {
                                            dou[j,1] = dou[j, 0]/sum;
                                        }
                                    }
                                }
                            }                            
                        }
                    }
                }
            }
        }

        public void start_animation()
        {
            if (!visualtization_started)
            {
                if (radioButton_kinetic.Checked)
                {
                    draw_kinetic(is_time);
                }
                if (radioButton_ms.Checked)
                {
                    draw_ms(is_time);
                }
                if (radioButton_dinamic.Checked)
                {
                    draw_dinamic(is_time, true);
                }
                if (radioButton3.Checked)
                {
                    draw_dinamic(is_time, false);
                }
            }
        }

        private void draw_kinetic(bool Time)
        {
            titul = new List<string>();
            export = new List<List<double[]>>();
            int i = 0, cnt = 0;
            PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
            model.LegendTitle = "Legend";
            model.LegendOrientation = LegendOrientation.Horizontal;
            model.LegendPlacement = LegendPlacement.Inside;
            model.LegendPosition = LegendPosition.RightTop;
            int r = 0;
            if (relative_values)
            {
                r = 1;
            }
            else
            {
                r = 0;
            }
            if (hlavicka != null)
            {
                foreach (string str in hlavicka)
                {
                    if (checkedListBox1.CheckedItems.Contains(str))
                    {
                        titul.Add(str);
                        List<double[]> partial = new List<double[]>();
                        FunctionSeries fs = new FunctionSeries();
                        fs.Color = New_color_for_series(cnt);
                        if (!is_con_din && !is_ele_din)
                        {
                            foreach (double[,] dou in tabulka)
                            {
                                double[] data = data_point(dou, 1, Time, i);
                                fs.Points.Add(new DataPoint(data[0], data[1]));
                                partial.Add(data);
                            }
                        }
                        if ((is_con_din && !is_ele_din) || (!is_con_din && is_ele_din))
                        {
                            foreach (List<double[,]> tabulka in data_D2) // concentraion 
                            {
                                foreach (double[,] dou in tabulka) // coordinates
                                {
                                    double comparation_paramenter = 0;
                                    if (is_con_din)
                                    {
                                        comparation_paramenter = _concentration; // concentraion
                                    }
                                    if (is_ele_din)
                                    {
                                        comparation_paramenter = _E_field; // set electric field
                                    }
                                    if (dou[dou.GetLength(0) - 1, r] == comparation_paramenter)
                                    {
                                        double[] data = data_point(dou, 1, Time, i);
                                        fs.Points.Add(new DataPoint(data[0], data[1]));
                                        partial.Add(data);
                                    }
                                }
                            }
                        }
                        if (is_con_din && is_ele_din)
                        {
                            foreach (List<List<double[,]>> tabulka_2D in data_D3)
                            {
                                foreach (List<double[,]> tabulka in tabulka_2D) // concentraion
                                {
                                    foreach (double[,] dou in tabulka) // coordinates
                                    {
                                        if ((dou[dou.GetLength(0) - 2, r] == _concentration) && (dou[dou.GetLength(0) - 1, r] == _E_field))
                                        {
                                            double[] data = data_point(dou, 1, Time, i);
                                            fs.Points.Add(new DataPoint(data[0], data[1]));
                                            partial.Add(data);
                                        }
                                    }
                                }
                            }
                        }
                        export.Add(partial);
                        fs.Title = str;
                        model.Series.Add(fs);
                        cnt++;
                    }
                    i++;
                }
                if (Load_type != -1)
                {
                    foreach (ScatterSeries series in Loaded_model())
                    {   
                        if (checkedListBox1.CheckedItems.Contains(series.Title))
                        {
                            model.Series.Add(series);
                        }
                    }
                    if (update_axis == true)
                    {
                        if (Load_type == 0)
                        {
                            Popis_X += "  /  " + Load_hlavicka_str[0];
                        }
                        else
                        {
                            Popis_X = Load_hlavicka_str[0];
                        }
                    }
                }
                if (calculation_memory_typeOf == 1)
                {
                    calculation_memory.Add(export);
                    calculation_memory_title.Add(titul);
                }
                else
                {
                    calculation_memory_typeOf = 1;
                    calculation_memory = new List<List<List<double[]>>>();
                    calculation_memory_title = new List<List<string>>();
                    calculation_memory.Add(export);
                    calculation_memory_title.Add(titul);
                }
                calculation_memory_update();
                model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });
                if (checkBox_log.Checked)
                {
                    model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
                }
                else
                {
                    model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
                }
                plot1.Model = model;
            }
        }

        private void draw_ms(bool Time)
        {

            titul = new List<string>();
            export = new List<List<double[]>>();
            int i = 0, cnt = 0;
            PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
            model.LegendTitle = "Legend";
            model.LegendOrientation = LegendOrientation.Horizontal;
            model.LegendPlacement = LegendPlacement.Inside;
            model.LegendPosition = LegendPosition.RightTop;
            bool blnAddProtonChargeCarrier = false;
            short intChargeState = 1;
            short intSuccess = 0;
            double[,] ConvolutedMSData2D = null;
            int ConvolutedMSDataCount = 0;
            MwtWinDll.MolecularWeightCalculator objMwtWin = new MwtWinDll.MolecularWeightCalculator();
            List<double[]> total = new List<double[]>();
            List<List<double[]>> total_con = new List<List<double[]>>();

            int r = 0;
            if (relative_values)
            {
                r = 1;
            }
            else
            {
                r = 0;
            }
            foreach (string str in hlavicka)
            {

                ColumnSeries fs = new ColumnSeries();
                LineSeries ls = new LineSeries();
                ls.Color = New_color_for_series(cnt);
                List<double[]> partial = new List<double[]>();
                List<List<double[]>> partial_con = new List<List<double[]>>();
                fs.BaseValue = 1;
                int c = 1;
                string formula = "";
                foreach (Items Item in Main.ItemColection)
                {
                    if ((str == Item.name) && (Item.cation))
                    {
                        formula = Convert_formula(Item.formula);
                    }
                }
                string strResults = null;
                try
                {
                    intSuccess = objMwtWin.ComputeIsotopicAbundances(ref formula, intChargeState, ref strResults, ref ConvolutedMSData2D, ref ConvolutedMSDataCount, blnAddProtonChargeCarrier);
                }
                catch { MessageBox.Show("Error in ComputeIsotopicAbundances: {0}", formula); }
                if (intSuccess == 0)
                {
                    List<double[]> isotopes = new List<double[]>();
                    List<double[]> export_partial = new List<double[]>();
                    isotopes = isotope_analysis(strResults);
                    if (!is_con_din && !is_ele_din)
                    {
                        foreach (double[,] dou in tabulka)
                        {
                            if (dou[(dou.GetLength(0) - 1), r] == _distance)
                            {
                                foreach (double[] m_z in isotopes)
                                {
                                    if (convolution)
                                    {
                                        partial_con.Add(Gauss_function(m_z[0], dou[i, r] * m_z[1]));
                                    }
                                    else
                                    {
                                        bool enough = false;
                                        while (!enough)
                                        {
                                            if (c == Convert.ToInt32(m_z[0]))
                                            {
                                                fs.Items.Add(new ColumnItem(m_z[1] * dou[i, r]));
                                                double[] bod = { m_z[0], m_z[1] * dou[i, r] };
                                                partial.Add(bod);
                                                bool add = false;
                                                foreach (double[] dou_tot in total)
                                                {
                                                    if (dou_tot[0] == Convert.ToInt32(m_z[0]))
                                                    {
                                                        dou_tot[1] += (m_z[1] * dou[i, r]);
                                                    }
                                                    else
                                                    {
                                                        add = true;
                                                    }
                                                }
                                                if (add || (total.Count == 0))
                                                {
                                                    double[] d = new double[] { Convert.ToInt32(m_z[0]), m_z[1] * dou[i, r] };
                                                    total.Add(d);
                                                }
                                                enough = true;
                                            }
                                            else
                                            {
                                                fs.Items.Add(new ColumnItem(1));
                                            }
                                            c++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if ((is_con_din && !is_ele_din) || (!is_con_din && is_ele_din))
                        {
                            double comparation_paramenter = 0;
                            if (is_con_din)
                            {
                                comparation_paramenter = _concentration; // set concentration
                            }
                            if (is_ele_din)
                            {
                                comparation_paramenter = _E_field; // set electric field
                            }
                            foreach (List<double[,]> Tabulka in data_D2)
                            {
                                foreach (double[,] dou in Tabulka)
                                {
                                    if (dou[(dou.GetLength(0) - 2), r] == _distance)
                                    {
                                        if (dou[(dou.GetLength(0) - 1), r] == comparation_paramenter)
                                        {
                                            foreach (double[] m_z in isotopes)
                                            {
                                                if (convolution)
                                                {
                                                    partial_con.Add(Gauss_function(m_z[0], dou[i, r] * m_z[1]));
                                                }
                                                else
                                                {

                                                    bool enough = false;
                                                    while (!enough)
                                                    {
                                                        if (c == Convert.ToInt32(m_z[0]))
                                                        {
                                                            fs.Items.Add(new ColumnItem(m_z[1] * dou[i, r]));
                                                            double[] bod = { m_z[0], m_z[1] * dou[i, r] };
                                                            partial.Add(bod);
                                                            bool add = false;
                                                            foreach (double[] dou_tot in total)
                                                            {
                                                                if (dou_tot[0] == Convert.ToInt32(m_z[0]))
                                                                {
                                                                    dou_tot[1] += (m_z[1] * dou[i, r]);
                                                                }
                                                                else
                                                                {
                                                                    add = true;
                                                                }
                                                            }
                                                            if (add || (total.Count == 0))
                                                            {
                                                                double[] d = new double[] { Convert.ToInt32(m_z[0]), m_z[1] * dou[i, r] };
                                                                total.Add(d);
                                                            }
                                                            enough = true;
                                                        }
                                                        else
                                                        {
                                                            fs.Items.Add(new ColumnItem(1));
                                                        }
                                                        c++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    if (is_con_din && is_ele_din)
                        {
                            foreach (List<List<double[,]>> tabulka_2d in data_D3)
                            {
                                foreach (List<double[,]> Tabulka in tabulka_2d)
                                {
                                    foreach (double[,] dou in Tabulka)
                                    {
                                        if (dou[(dou.GetLength(0) - 3), r] == _distance)
                                        {
                                            if (dou[(dou.GetLength(0) - 2), r] == _concentration)
                                            {
                                                if (dou[(dou.GetLength(0) - 1), r] == _E_field)
                                                {
                                                    foreach (double[] m_z in isotopes)
                                                    {
                                                        if (convolution)
                                                        {
                                                            partial_con.Add(Gauss_function(m_z[0], dou[i, r] * m_z[1]));
                                                        }
                                                        else
                                                        {
                                                    
                                                    
                                                            bool enough = false;
                                                            while (!enough)
                                                            {
                                                                if (c == Convert.ToInt32(m_z[0]))
                                                                {
                                                                    fs.Items.Add(new ColumnItem(m_z[1] * dou[i, r]));
                                                                    double[] bod = { m_z[0], m_z[1] * dou[i, r] };
                                                                    partial.Add(bod);
                                                                    bool add = false;
                                                                    foreach (double[] dou_tot in total)
                                                                    {
                                                                        if (dou_tot[0] == Convert.ToInt32(m_z[0]))
                                                                        {
                                                                            dou_tot[1] += (m_z[1] * dou[i, r]);
                                                                        }
                                                                        else
                                                                        {
                                                                            add = true;
                                                                        }
                                                                    }
                                                                    if (add || (total.Count == 0))
                                                                    {
                                                                        double[] d = new double[] { Convert.ToInt32(m_z[0]), m_z[1] * dou[i, r] };
                                                                        total.Add(d);
                                                                    }
                                                                    enough = true;
                                                                }
                                                                else
                                                                {
                                                                    fs.Items.Add(new ColumnItem(1));
                                                                }
                                                                c++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    if (convolution)
                    {
                        foreach (List<double[]> dou in partial_con)
                        {
                            foreach (double[] d in dou)
                            {
                                bool presented = false;
                                foreach (double[] exp_dou in export_partial)
                                {
                                    if (exp_dou[0] == d[0])
                                    {
                                        presented = true;
                                        exp_dou[1] += d[1];
                                    }
                                }
                                if (!presented)
                                {
                                    double[] ne = new double[] { d[0], d[1] };
                                    export_partial.Add(ne);
                                }
                            }
                        }
                        foreach (double[] exp in export_partial)
                        {
                            ls.Points.Add(new DataPoint(exp[0], exp[1]));
                        }
                        total_con.Add(export_partial);
                    }
                    if (checkedListBox1.CheckedItems.Contains(str))
                    {
                        if (convolution)
                        {                            
                            ls.Title = str;
                            model.Series.Add(ls);
                            export.Add(export_partial);
                            titul.Add(str);
                        }
                        else
                        {
                            fs.Title = str;
                            model.Series.Add(fs);
                            export.Add(partial);
                            titul.Add(str);
                        }
                        cnt++;
                    }
                }
                i++;
            }
            if (checkedListBox1.CheckedItems.Contains("Sum") && ((total.Count > 0) || (total_con.Count > 0)))
                {
                if (convolution)
                {
                    LineSeries fs = new LineSeries();
                    List<double[]> export_partial = new List<double[]>();
                    foreach (List<double[]> dou in total_con)
                    {
                        foreach (double[] d in dou)
                        {
                            bool presented = false;
                            foreach (double[] exp_dou in export_partial)
                            {
                                if (exp_dou[0] == d[0])
                                {
                                    presented = true;
                                    exp_dou[1] += d[1];
                                }
                            }
                            if (!presented)
                            {
                                double[] ne = new double[] { d[0], d[1] };
                                export_partial.Add(ne);
                            }
                        }
                    }                   
                    foreach (double[] exp in export_partial)
                    {
                        fs.Points.Add(new DataPoint(exp[0], exp[1]));
                    }
                    fs.Title = "Sum";
                    export.Add(export_partial);
                    titul.Add("Sum");
                    model.Series.Add(fs);
                }
                else
                {
                    List<double[]> partial = new List<double[]>();
                    ColumnSeries fs = new ColumnSeries();
                    fs.BaseValue = 1;
                    bool enough = false;
                    bool added = false;
                    int c = 1;
                    while (!enough)
                    {
                        List<double> mmry = new List<double>();
                        foreach (double[] dou in total)
                        {
                            if (Convert.ToInt32(dou[0]) == c)
                            {
                                mmry.Add(dou[1]);
                                added = true;
                                int a = total.IndexOf(dou) + 1;
                                if (total.Count == a)
                                {
                                    enough = true;
                                }
                            }

                        }
                        if (added)
                        {
                            double suma = 0;
                            foreach (double d in mmry)
                            {
                                suma += d;
                            }
                            fs.Items.Add(new ColumnItem(suma));
                            double[] bod = { c, suma };
                            partial.Add(bod);
                        }
                        if (!added)
                        {
                            fs.Items.Add(new ColumnItem(1));
                        }
                        added = false;
                        c++;
                    }
                    fs.Title = "Sum";
                    titul.Add("Sum");
                    export.Add(partial);
                    model.Series.Add(fs);
                }
                }
            if (Load_type != -1)
            {
                foreach (ScatterSeries series in Loaded_model())
                {
                    if (checkedListBox1.CheckedItems.Contains(series.Title))
                    {
                        model.Series.Add(series);
                    }
                }
                if (update_axis == true)
                {
                    if (Load_type == 0)
                    {
                        Popis_X += "  /  " + Load_hlavicka_str[0];
                    }
                    else
                    {
                        Popis_X = Load_hlavicka_str[0];
                    }
                }
            }
            if (calculation_memory_typeOf == 2)
            {
                calculation_memory.Add(export);
                calculation_memory_title.Add(titul);
            }
            else
            {
                calculation_memory_typeOf = 2;
                calculation_memory = new List<List<List<double[]>>>();
                calculation_memory_title = new List<List<string>>();
                calculation_memory.Add(export);
                calculation_memory_title.Add(titul);
            }
            calculation_memory_update();
            if(convolution)
            {
                model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });

            }
            else
            {
                model.Axes.Add(new CategoryAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });

            }
            if (checkBox_log.Checked)
            {
                model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration", Minimum = 1 });
            }
            else
            {
                model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
            }
            plot1.Model = model;                       
        }

        private void draw_dinamic(bool Time, bool type)
        {
            titul = new List<string>();
            export = new List<List<double[]>>();
            int i = 0, cnt = 0;
            PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
            model.LegendTitle = "Legend";
            model.LegendOrientation = LegendOrientation.Horizontal;
            model.LegendPlacement = LegendPlacement.Inside;
            model.LegendPosition = LegendPosition.RightTop;
            int r = 0;
            if (relative_values)
            {
                r = 1;
            }
            else
            {
                r = 0;
            }
            // sum
            List<double[]> sigma = new List<double[]>();
            foreach (string str in hlavicka)
            {
                if (checkedListBox1.CheckedItems.Contains(str))
                {
                    titul.Add(str);
                    List<double[]> partial = new List<double[]>();
                    FunctionSeries fs = new FunctionSeries();
                    fs.Color = New_color_for_series(cnt);
                    if (is_ele_din && is_con_din) // 3D pole
                    {
                        foreach (List<List<double[,]>> tabulka_2d in data_D3)
                        {
                            foreach (List<double[,]> tabulka in tabulka_2d) // concentration
                            {
                                foreach (double[,] dou in tabulka) //  coordinates
                                {
                                    if (type)
                                    {
                                        if (dou[dou.GetLength(0) - 1, r] == _E_field)
                                        {
                                            if (dou[dou.GetLength(0) - 3, r] == _distance) // set distance
                                            {
                                                if (Load_type == 1) // Are data loaded ?
                                                {
                                                    //  x modification for profile 3 measurements
                                                    int c = 0;
                                                    double suma = 0;
                                                    double H3O = 0;
                                                    foreach (string st in hlavicka)
                                                    {
                                                        if(st == comboBox2.SelectedItem.ToString())
                                                        {
                                                            H3O = dou[c, 0];
                                                            suma += dou[c, 0];
                                                        }
                                                        foreach (string items_name in listBox1.Items)
                                                        {
                                                            if(st == items_name)
                                                            {
                                                                suma += dou[c, 0];
                                                            }
                                                        }
                                                        c++;
                                                    }
                                                    double X_coord = -1.0 * Math.Log((H3O / suma), Math.E);

                                                    if (Time) // to time coordinates
                                                    {
                                                        double time_dou = (dou[(dou.GetLength(0) - 2), r] * 1000000) / Main.Ion_velociy;
                                                        fs.Points.Add(new DataPoint(X_coord, dou[i, r]));
                                                        double[] bod = { X_coord, dou[i, r] };
                                                        partial.Add(bod);
                                                        bool add = true;
                                                        foreach (double[] dou_tot in sigma)
                                                        {
                                                            if (dou_tot[0] == bod[0])
                                                            {
                                                                dou_tot[1] += dou[i, r];
                                                                add = false;
                                                            }
                                                        }
                                                        if (add || (sigma.Count == 0))
                                                        {
                                                            double[] d = new double[] { bod[0], dou[i, r] };
                                                            sigma.Add(d);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        fs.Points.Add(new DataPoint(X_coord, dou[i, r]));
                                                        double[] bod = { X_coord, dou[i, r] };
                                                        partial.Add(bod);
                                                        bool add = true;
                                                        foreach (double[] dou_tot in sigma)
                                                        {
                                                            if (dou_tot[0] == bod[0])
                                                            {
                                                                dou_tot[1] += dou[i, r];
                                                                add = false;
                                                            }
                                                        }
                                                        if (add || (sigma.Count == 0))
                                                        {
                                                            double[] d = new double[] { bod[0], dou[i, r] };
                                                            sigma.Add(d);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (Time) // to time coordinates
                                                    {
                                                        double time_dou = (dou[(dou.GetLength(0) - 2), r] * 1000000) / Main.Ion_velociy;
                                                        fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 2), r], dou[i, r]));
                                                        double[] bod = { dou[(dou.GetLength(0) - 2), r], dou[i, r] };
                                                        partial.Add(bod);
                                                        bool add = true;
                                                        foreach (double[] dou_tot in sigma)
                                                        {
                                                            if (dou_tot[0] == bod[0])
                                                            {
                                                                dou_tot[1] += dou[i, r];
                                                                add = false;
                                                            }
                                                        }
                                                        if (add || (sigma.Count == 0))
                                                        {
                                                            double[] d = new double[] { bod[0], dou[i, r] };
                                                            sigma.Add(d);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 2), r], dou[i, r]));
                                                        double[] bod = { dou[(dou.GetLength(0) - 2), r], dou[i, r] };
                                                        partial.Add(bod);
                                                        bool add = true;
                                                        foreach (double[] dou_tot in sigma)
                                                        {
                                                            if (dou_tot[0] == bod[0])
                                                            {
                                                                dou_tot[1] += dou[i, r];
                                                                add = false;
                                                            }
                                                        }
                                                        if (add || (sigma.Count == 0))
                                                        {
                                                            double[] d = new double[] { bod[0], dou[i, r] };
                                                            sigma.Add(d);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dou[dou.GetLength(0) - 2, r] == _concentration)
                                        {
                                            if (dou[dou.GetLength(0) - 3, r] == _distance) // set distance
                                            {
                                                if (Time) // to time coordinates
                                                {
                                                    double time_dou = (dou[(dou.GetLength(0) - 1), r] * 1000000) / Main.Ion_velociy;
                                                    fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1), r], dou[i, r]));
                                                    double[] bod = { dou[(dou.GetLength(0) - 1), r], dou[i, r] };
                                                    partial.Add(bod);
                                                    bool add = true;
                                                    foreach (double[] dou_tot in sigma)
                                                    {
                                                        if (dou_tot[0] == bod[0])
                                                        {
                                                            dou_tot[1] += dou[i, r];
                                                            add = false;
                                                        }
                                                    }
                                                    if (add || (sigma.Count == 0))
                                                    {
                                                        double[] d = new double[] { bod[0], dou[i, r] };
                                                        sigma.Add(d);
                                                    }
                                                }
                                                else
                                                {
                                                    fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1), r], dou[i, r]));
                                                    double[] bod = { dou[(dou.GetLength(0) - 1), r], dou[i, r] };
                                                    partial.Add(bod);
                                                    bool add = true;
                                                    foreach (double[] dou_tot in sigma)
                                                    {
                                                        if (dou_tot[0] == bod[0])
                                                        {
                                                            dou_tot[1] += dou[i, r];
                                                            add = false;
                                                        }
                                                    }
                                                    if (add || (sigma.Count == 0))
                                                    {
                                                        double[] d = new double[] { bod[0], dou[i, r] };
                                                        sigma.Add(d);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if ((is_ele_din && !is_con_din) || (!is_ele_din && is_con_din)) // 2D pole
                    {
                        foreach (List<double[,]> tabulka in data_D2) // concentration
                        {
                            foreach (double[,] dou in tabulka) // coordinates
                            {
                                if (type) // concentration
                                {
                                    if (dou[dou.GetLength(0) - 2, r] == _distance) // set distance
                                    {
                                        if (Load_type == 1)
                                        {
                                            //  x modification for profile 3
                                            int c = 0;
                                            double suma = 0;
                                            double H3O = 0;
                                            foreach (string st in hlavicka)
                                            {
                                                if (st == comboBox2.SelectedItem.ToString())
                                                {
                                                    H3O = dou[c, 0];
                                                    suma += dou[c, 0];
                                                }
                                                foreach (Object items_ in listBox1.Items)
                                                {
                                                    if (st == items_.ToString())
                                                    {
                                                        suma += dou[c, 0];
                                                    }
                                                }
                                                c++;
                                            }
                                            double X_coord = -1.0 * Math.Log((H3O / suma), Math.E);

                                            if (Time) // previest vystup na cas
                                            {
                                                double time_dou = (dou[(dou.GetLength(0) - 1), r] * 1000000) / Main.Ion_velociy;
                                                fs.Points.Add(new DataPoint(X_coord, dou[i, r]));
                                                double[] bod = { X_coord, dou[i, r] };
                                                partial.Add(bod);
                                                bool add = true;
                                                foreach (double[] dou_tot in sigma)
                                                {
                                                    if (dou_tot[0] == bod[0])
                                                    {
                                                        dou_tot[1] += dou[i, r];
                                                        add = false;
                                                    }
                                                }
                                                if (add || (sigma.Count == 0))
                                                {
                                                    double[] d = new double[] { bod[0], dou[i, r] };
                                                    sigma.Add(d);
                                                }
                                            }
                                            else
                                            {
                                                fs.Points.Add(new DataPoint(X_coord, dou[i, r]));
                                                double[] bod = { X_coord, dou[i, r] };
                                                partial.Add(bod);
                                                bool add = true;
                                                foreach (double[] dou_tot in sigma)
                                                {
                                                    if (dou_tot[0] == bod[0])
                                                    {
                                                        dou_tot[1] += dou[i, r];
                                                        add = false;
                                                    }
                                                }
                                                if (add || (sigma.Count == 0))
                                                {
                                                    double[] d = new double[] { bod[0], dou[i, r] };
                                                    sigma.Add(d);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Time) // to time coordinates
                                            {
                                                double time_dou = (dou[(dou.GetLength(0) - 1), r] * 1000000) / Main.Ion_velociy;
                                                fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1), r], dou[i, r]));
                                                double[] bod = { dou[(dou.GetLength(0) - 1), r], dou[i, 0], r };
                                                partial.Add(bod);
                                                bool add = true;
                                                foreach (double[] dou_tot in sigma)
                                                {
                                                    if (dou_tot[0] == bod[0])
                                                    {
                                                        dou_tot[1] += dou[i, r];
                                                        add = false;
                                                    }
                                                }
                                                if (add || (sigma.Count == 0))
                                                {
                                                    double[] d = new double[] { bod[0], dou[i, r] };
                                                    sigma.Add(d);
                                                }
                                            }
                                            else
                                            {
                                                fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1), r], dou[i, r]));
                                                double[] bod = { dou[(dou.GetLength(0) - 1), r], dou[i, r] };
                                                partial.Add(bod);
                                                bool add = true;
                                                foreach (double[] dou_tot in sigma)
                                                {
                                                    if (dou_tot[0] == bod[0])
                                                    {
                                                        dou_tot[1] += dou[i, r];
                                                        add = false;
                                                    }
                                                }
                                                if (add || (sigma.Count == 0))
                                                {
                                                    double[] d = new double[] { bod[0], dou[i, r] };
                                                    sigma.Add(d);
                                                }
                                            }
                                        }
                                    }
                                }
                                else // E field
                                {
                                    if (dou[dou.GetLength(0) - 2, r] == _distance) // set distance
                                    {
                                        if (Time) // to time coordinates
                                        {
                                            double time_dou = (dou[(dou.GetLength(0) - 1), r] * 1000000) / Main.Ion_velociy;
                                            fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1), r], dou[i, r]));
                                            double[] bod = { dou[(dou.GetLength(0) - 1), r], dou[i, r] };
                                            partial.Add(bod);
                                            bool add = true;
                                            foreach (double[] dou_tot in sigma)
                                            {
                                                if (dou_tot[0] == bod[0])
                                                {
                                                    dou_tot[1] += dou[i, r];
                                                    add = false;
                                                }
                                            }
                                            if (add || (sigma.Count == 0))
                                            {
                                                double[] d = new double[] { bod[0], dou[i, r] };
                                                sigma.Add(d);
                                            }
                                        }
                                        else
                                        {
                                            fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1), r], dou[i, r]));
                                            double[] bod = { dou[(dou.GetLength(0) - 1), r], dou[i, r] };
                                            partial.Add(bod);
                                            bool add = true;
                                            foreach (double[] dou_tot in sigma)
                                            {
                                                if (dou_tot[0] == bod[0])
                                                {
                                                    dou_tot[1] += dou[i, r];
                                                    add = false;
                                                }
                                            }
                                            if (add || (sigma.Count == 0))
                                            {
                                                double[] d = new double[] { bod[0], dou[i, r] };
                                                sigma.Add(d);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    fs.Title = str;
                    model.Series.Add(fs);
                    export.Add(partial);
                    cnt++;
                }
                i++;
            }
            if (calculation_memory_typeOf == 3)
            {
                calculation_memory.Add(export);
                calculation_memory_title.Add(titul);
            }
            else
            {
                calculation_memory_typeOf = 3;
                calculation_memory = new List<List<List<double[]>>>();
                calculation_memory_title = new List<List<string>>();
                calculation_memory.Add(export);
                calculation_memory_title.Add(titul);
            }
            calculation_memory_update();
            /*
            FunctionSeries fs_sum = new FunctionSeries();
            foreach (double[] dou in sigma)
            {
                fs_sum.Points.Add(new DataPoint(dou[0], dou[1]));
            }
            fs_sum.Title = "Sum";
            model.Series.Add(fs_sum);
            */
            if (Load_type != -1)
            {
                foreach (ScatterSeries series in Loaded_model())
                {
                    if (checkedListBox1.CheckedItems.Contains(series.Title))
                    {
                        model.Series.Add(series);
                    }
                }
                if (update_axis == true)
                {
                    if (Load_type == 0)
                    {
                        Popis_X += "  /  " + Load_hlavicka_str[0];
                    }
                    else
                    {
                        Popis_X = Load_hlavicka_str[0];
                    }
                }
            }
            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });
            if (checkBox_log.Checked)
            {
                model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
            }
            else
            {
                model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
            }
            plot1.Model = model;
            enable_interpolation();
        }

        private double[] data_point(double[,] dataset, short type, bool Time, int line)
        {
            int r;
            if (checkBox3.Checked)
            {
                r = 1;
            }
            else
            {
                r = 0;
            }
            double[] _point = { 0, 0 };
            if (type == 1)
            {
                if (!is_con_din && !is_ele_din)
                {
                    if (Time) // x to t / t to x
                    {
                        if (Main.calc_type && !is_ele)
                        {
                            _point = new double[] { dataset[(dataset.GetLength(0) - 1), r], dataset[line, r] };

                        }
                        else
                        {
                            double _dou = (dataset[(dataset.GetLength(0) - 1), r] * 1000000) / Main.Ion_velociy;
                            _point = new double[] { _dou, dataset[line, r] };
                        }
                    }
                    else
                    {
                        if (Main.calc_type && !is_ele)
                        {
                            double _dou = (dataset[(dataset.GetLength(0) - 1), r] * Main.Ion_velociy) / 1000000;
                            _point = new double[] { _dou, dataset[line, r] };
                        }
                        else
                        {
                            _point = new double[] { dataset[(dataset.GetLength(0) - 1), r], dataset[line, r] };
                        }
                    }
                }
                if ((is_con_din && !is_ele_din) || (!is_con_din && is_ele_din))
                {

                    if (Time) // x to t / t to x
                    {
                        if (Main.calc_type && !is_ele)
                        {
                            _point = new double[] { dataset[(dataset.GetLength(0) - 2), r], dataset[line, r] };
                        }
                        else
                        {
                            double time_dou = (dataset[(dataset.GetLength(0) - 2), r] * 1000000) / Main.Ion_velociy;
                            _point = new double[] { time_dou, dataset[line, r] };
                        }
                    }
                    else
                    {
                        if (Main.calc_type && !is_ele)
                        {
                            double time_dou = (dataset[(dataset.GetLength(0) - 2), r] * Main.Ion_velociy) / 1000000;
                            _point = new double[] { time_dou, dataset[line, r] };
                        }
                        else
                        {
                            _point = new double[] { dataset[(dataset.GetLength(0) - 2), r], dataset[line, r] };
                        }
                    }

                }
                if (is_con_din && is_ele_din)
                {

                    if (Time) // x to t / t to x
                    {
                        if (Main.calc_type && !is_ele)
                        {
                            _point = new double[] { dataset[(dataset.GetLength(0) - 3), r], dataset[line, r] };
                        }
                        else
                        {
                            double time_dou = (dataset[(dataset.GetLength(0) - 3), r] * 1000000) / Main.Ion_velociy;
                            _point = new double[] { time_dou, dataset[line, r] };
                        }
                    }
                    else
                    {
                        if (Main.calc_type && !is_ele)
                        {
                            double time_dou = (dataset[(dataset.GetLength(0) - 3), r] * Main.Ion_velociy) / 1000000;
                            _point = new double[] { time_dou, dataset[line, r] };
                        }
                        else
                        {
                            _point = new double[] { dataset[(dataset.GetLength(0) - 3), r], dataset[line, r] };
                        }
                    }

                }
            }
            return _point;
        }

        private void button_Load_Click(object sender, EventArgs e)
        {
            Load_data_for_interpolation load_data = new Load_data_for_interpolation();
            load_data.Show(this);
        }

        private void try_enable_interpolation()
        {
            if (Experimental_input_status)
            {
                button_interpolation.Enabled = true;
            }
            else
            {
                button_interpolation.Enabled = false;
            }
        }

        public static void show_loaded_data()
        {
            PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
            model.LegendTitle = "Legend";
            model.LegendOrientation = LegendOrientation.Horizontal;
            model.LegendPlacement = LegendPlacement.Inside;
            model.LegendPosition = LegendPosition.RightTop;
            Popis_X = Load_hlavicka_str[0];
            checkedListBox1.Items.Clear();
            int cnt = 0;
            for (int i = 1; i < Load_hlavicka_str.GetLength(0); i++)
            {
                ScatterSeries sc = new ScatterSeries();
                sc.MarkerSize = Marker_Size;
                sc.MarkerType = marker_type;
                sc.MarkerStroke = New_color_for_series(cnt);
                sc.Title = Load_hlavicka_str[i].ToString();
                for (int j = 1; j < Loaded_data.GetLength(1); j++)
                {
                    sc.Points.Add(new ScatterPoint(Loaded_data[0, j], Loaded_data[i, j]));
                }
                model.Series.Add(sc);
                checkedListBox1.Items.Add(Load_hlavicka_str[i].ToString(), true);
                cnt++;
            }
            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });
            if (log_lin)
            {
                model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
            }
            else
            {
                model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
            } 
            plot1.Model = model;
            label26.Text = "Load code: " + Load_type.ToString();
            Load_type_change();
            }

        public List<ScatterSeries> Loaded_model()
        {
            List<ScatterSeries> list = new List<ScatterSeries>();
            int cnt = 0;
            for (int i = 1; i < Load_hlavicka_str.GetLength(0); i++)
            {
                ScatterSeries sc = new ScatterSeries();
                sc.MarkerSize = Marker_Size;
                sc.MarkerType = marker_type;
                sc.MarkerStroke = New_color_for_series(cnt);
                sc.Title = Load_hlavicka_str[i].ToString();
                for (int j = 0; j < Loaded_data.GetLength(1); j++)
                {
                    sc.Points.Add(new ScatterPoint(Loaded_data[0, j], Loaded_data[i, j]));
                }
                list.Add(sc);
                cnt++;
            }
            return list;
        }
          
        private static void Load_type_change()
        {
            if(Load_type == 0)
            {
                // universal input - no restrictions
                if (Load_relative)
                {
                    checkBox3.Checked = true;
                }
                groupBox3.Enabled = false;
            }
            if (Load_type == 1)
            {
                // profile 3 measurement, dependece rel or not, x = -ln(H3O/H2O_total)
                if (Load_relative)
                {
                    checkBox3.Checked = true;
                }
                groupBox2.Enabled = true;
                groupBox5.Enabled = true;
                groupBox3.Enabled = true;
            }
            if (Load_type == 2)
            { // Electric field id dynamic0
                if (Load_relative)
                {
                    checkBox3.Checked = true;
                }
                groupBox2.Enabled = true;
                groupBox5.Enabled = true;
                groupBox3.Enabled = false;
            }

        }

        string writer(string[] Hlavicka, List<double[]> Tabulka)
        {
            string writer = "";
            foreach (string str in Hlavicka)
            {
                writer = writer + str + "\t";
            }
            writer = writer + "\r\n";
            foreach (double[] dou in Tabulka)
            {
                foreach (double cislo in dou)
                {
                    writer = writer + cislo.ToString() + "\t";
                }
                writer = writer + "\r\n";
            }
            return writer;
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Output";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!saveFileDialog1.Title.Contains(".txt"))
                { saveFileDialog1.Title += ".txt"; }
                this.Cursor = Cursors.WaitCursor;
                int pocet_itemov = titul.Count;
                List<double[]> _export = new List<double[]>();
                string[] legenda = new string[pocet_itemov + 1];
                if (is_con_din)
                {
                    legenda[0] = Concentration_name;
                }
                if (is_ele_din)
                {
                    legenda[0] = Concentration_name;
                }
                if (!is_con_din && !is_ele_din)
                {
                    legenda[0] = Popis_X;
                }
                int k = 1;
                foreach (string str in titul)
                {
                    legenda[k] = str;
                    k++;
                }
                double[] export_item;
                foreach (List<double[]> Tabulka in export)
                {
                    foreach (double[] dou in Tabulka)
                    {
                        bool je_tu = false;
                        foreach (double[] d in _export)
                        {
                            if (d[0] == dou[0])
                            {
                                je_tu = true;
                            }
                        }
                        if (!je_tu)
                        {
                            export_item = new double[pocet_itemov + 1];
                            export_item[0] = dou[0];
                            _export.Add(export_item);
                        }
                    }
                }
                int i = 1;
                foreach (List<double[]> Tabulka in export)
                {
                    foreach (double[] dou in Tabulka)
                    {
                        foreach (double[] d in _export)
                        {
                            if (dou[0] == d[0])
                            {
                                d[i] = dou[1];
                            }
                        }
                    }
                    i++;
                }
                if (saveFileDialog1.FileName != "")
                {
                    StreamWriter write = new StreamWriter(saveFileDialog1.OpenFile());
                    write.Write(writer(legenda, _export));
                    write.Dispose();
                    write.Close();
                }
                this.Cursor = Cursors.Default;
            }
        }

        private void calculation_memory_update()
        {
            calculation_memory_chosen = calculation_memory.Count;
            // this.label25.Text = calculation_memory_chosen.ToString() + "/" + calculation_memory.Count.ToString();
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

        private static OxyColor New_color_for_series(int n)
        {
            OxyColor new_color = new OxyColor();
            PlotModel model = new PlotModel();
            IList<OxyColor> color_list = model.DefaultColors;
            int i = 0;
            foreach( OxyColor color in color_list)
            {
                if(i == n || ( n > 11 && ( n % 11) == i))
                {
                    new_color = color;
                }
                i++;
            }
            return new_color;
        }

        # region Private graph methods

        private void radioButton_kinetic_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_kinetic.Checked)
            {
                button_ms.Enabled = false;
                textBox_dist.Enabled = false;
                trackBar_dist.Enabled = false;
                trackBar_dist.Value = 0;
                textBox_dist.Text = "";
                button_ms.Enabled = false;
                if (is_con_din)
                {
                    textBox_conc.Enabled = true;
                    trackBar_conc.Enabled = true;
                    set_concentration(_concentration);
                }
                else
                {
                    textBox_conc.Enabled = false;
                    trackBar_conc.Enabled = false;
                }
                if (is_ele_din)
                {
                    textBox_ele.Enabled = true;
                    trackBar_ele.Enabled = true;
                    set_e_field(_E_field);
                }
                else
                {
                    textBox_ele.Enabled = false;
                    trackBar_ele.Enabled = false;
                }
                // draw kinetic
                if (is_time)
                {
                    Popis_X = "Time [us]";
                }
                else
                {
                    Popis_X = "Distance [cm]";
                }
                if (calculated_data)
                {
                    iniciate_listbox();
                    start_animation();
                }
            }
        }

        private void radioButton_ms_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_ms.Checked)
            {
                button_ms.Enabled = true;
                textBox_dist.Enabled = true;
                trackBar_dist.Enabled = true;
                if (is_time)
                {
                    set_time(_distance);
                }
                else
                {
                    set_distanc(_distance);
                }
                if (is_con_din)
                {
                    textBox_conc.Enabled = true;
                    trackBar_conc.Enabled = true;
                    set_concentration(_concentration);
                }
                else
                {
                    textBox_conc.Enabled = false;
                    trackBar_conc.Enabled = false;
                }
                if (is_ele_din)
                {
                    textBox_ele.Enabled = true;
                    trackBar_ele.Enabled = true;
                    set_e_field(_E_field);
                }
                else
                {
                    textBox_ele.Enabled = false;
                    trackBar_ele.Enabled = false;
                }
                // draw ms
                Popis_X = "m/z";
                iniciate_listbox();
                start_animation();
            }
        }

        private void radioButton_dinamic_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_dinamic.Checked)
            {
                button_ms.Enabled = false;
                textBox_dist.Enabled = true;
                trackBar_dist.Enabled = true;
                trackBar_dist.Value = 0;
                textBox_dist.Text = "";
                textBox_conc.Enabled = false;
                trackBar_conc.Enabled = false;
                trackBar_conc.Value = 0;
                textBox_conc.Text = "";
                button_ms.Enabled = false;
                if (is_time)
                {
                    set_time(_distance);
                    label13.Text = "0";
                    label12.Text = Main.Time_duration.ToString();
                }
                else
                {
                    set_distanc(_distance);
                    label13.Text = "0";
                    label12.Text = Main.Distance.ToString();
                }
                if (is_ele_din)
                {
                    textBox_ele.Enabled = true;
                    trackBar_ele.Enabled = true;
                    set_e_field(_E_field);
                }
                else
                {
                    textBox_ele.Enabled = false;
                    trackBar_ele.Enabled = false;
                }
                // draw din
                Popis_X = "Concentration [" + Concentration_name + "]";
                iniciate_listbox();
                start_animation();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                button_ms.Enabled = false;
                textBox_dist.Enabled = true;
                trackBar_dist.Enabled = true;
                textBox_ele.Enabled = false;
                trackBar_ele.Enabled = false;
                button_ms.Enabled = false;
                if (is_time)
                {
                    set_time(_distance);
                    label13.Text = "0";
                    label12.Text = Main.Time_duration.ToString();
                }
                else
                {
                    set_distanc(_distance);
                    label13.Text = "0";
                    label12.Text = Main.Distance.ToString();
                }
                if (is_con_din)
                {
                    textBox_conc.Enabled = true;
                    trackBar_conc.Enabled = true;
                    set_concentration(_concentration);
                }
                else
                {
                    textBox_conc.Enabled = false;
                    trackBar_conc.Enabled = false;
                    trackBar_conc.Value = 0;
                    textBox_conc.Text = "";
                }
                // draw din
                Popis_X = "E/N [Td]";
                iniciate_listbox();
                start_animation();
            }
        }

        private List<double[]> isotope_analysis(string Str)
        {
            string value = "";
            int enter_cnt = 0;
            int position = 0;
            bool start_transfer = false;
            List<double[]> isotopes_prop_list = new List<double[]>();
            double[] isotopes_prop = new double[3];
            foreach (char c in Str)
            {
                if (start_transfer == true)
                {
                    if (c == '\t')
                    {
                        if (position == 0) { isotopes_prop[0] = Main.Convertor(value); }
                        if (position == 1) { isotopes_prop[1] = Main.Convertor(value); }
                        position++;
                        value = "";
                    }
                    if (c == '\n')
                    {
                        isotopes_prop[2] = Main.Convertor(value);
                        position = 0;
                        value = "";
                        isotopes_prop_list.Add(new double[] { isotopes_prop[0], isotopes_prop[1], isotopes_prop[2] });
                    }
                    else
                    {
                        value += Convert.ToString(c);
                    }
                }
                if (c == '\n') { enter_cnt++; }
                if (enter_cnt == 2) { start_transfer = true; }
            }
            foreach (double[] dou in isotopes_prop_list)
            {
                dou[0] = Math.Round(dou[0], 2);
                if (dou[2] == 100)
                {
                    foreach (double[] d in isotopes_prop_list)
                    {
                        d[2] = Math.Round((100 * d[2]) / dou[1], 5);
                    }
                }
            }
            return isotopes_prop_list;
        }

        private void set_concentration(double Concentration)
        {
            double mmry;
            double step = (Main.conc_end - Main.conc_start) / (Main.conc_steps - 1);
            double cnt = Main.conc_start;
            double real_conc = Main.conc_start;
            while (cnt <= Main.conc_end)
            {
                mmry = Math.Abs(Concentration - real_conc);
                if (Math.Abs(Concentration - cnt) < mmry)
                {
                    real_conc = cnt;
                }
                cnt += step;
                cnt = Math.Round(cnt, 2);
            }
            _concentration = real_conc;
            if (_concentration > 100)
            {
                textBox_conc.Text = _concentration.ToString("0.00E0#", CultureInfo.InvariantCulture);
            }
            else
            {
                textBox_conc.Text = _concentration.ToString();
            }
            if (!conc_roll)
            {
                taskBar_conc_setValue(_concentration);
            }
        }

        private double concentration()
        {
            return _concentration;
        }

        private void set_time(double Time)
        {
            if (Main.calc_type)
            {
                double mmry;
                double dif = Main.Time_duration / Main.NumberOfSteps;
                double cnt = 0;
                double real_time = 0;
                while (cnt <= Main.Time_duration)
                {
                    mmry = Math.Abs(Time - real_time);
                    if (Math.Abs(Time - cnt) < mmry)
                    {
                        real_time = cnt;
                    }
                    cnt += dif;
                    cnt = Math.Round(cnt, 12);
                }
                _distance = real_time;
                textBox_dist.Text = real_time.ToString();
                if (!dist_roll)
                {
                    taskBar_dist_setValue(real_time);
                }
            }
            else
            {
                double mmry;
                double dif = Main.Time_duration / Main.NumberOfSteps;
                double cnt = 0;
                double real_time = 0;
                while (cnt <= Main.Time_duration)
                {
                    mmry = Math.Abs(Time - real_time);
                    if (Math.Abs(Time - cnt) < mmry)
                    {
                        real_time = cnt;
                    }
                    cnt += dif;
                    cnt = Math.Round(cnt, 12);
                }
                _distance = (real_time * Main.Ion_velociy) / 1000000;
                textBox_dist.Text = real_time.ToString();
                if (!dist_roll)
                {
                    taskBar_dist_setValue(real_time);
                }
            }
        }

        private double time()
        {
            double Time = 0;
            if (Main.calc_type)
            {
                Time = _distance;
            }
            else
            {
                Time = (_distance * 1000000) / Main.Ion_velociy;
            }
            return Time;
        }

        private void set_distanc(double Distance)
        {
            if (Main.calc_type)
            {
                double mmry;
                double dif = Main.Distance / Main.NumberOfSteps;
                double cnt = 0;
                double real_distance = 0;
                while (cnt <= Main.Distance)
                {
                    mmry = Math.Abs(Distance - real_distance);
                    if (Math.Abs(Distance - cnt) < mmry)
                    {
                        real_distance = cnt;
                    }
                    cnt += dif;
                    cnt = Math.Round(cnt, 12);
                }
                _distance = (real_distance * 1000000) / Main.Ion_velociy;
                textBox_dist.Text = real_distance.ToString();
                if (!dist_roll)
                {
                    taskBar_dist_setValue(real_distance);
                }
            }
            else
            {
                double mmry;
                double dif = Main.Distance / Main.NumberOfSteps;
                double cnt = 0;
                double real_distance = 0;
                while (cnt <= Main.Distance)
                {
                    mmry = Math.Abs(Distance - real_distance);
                    if (Math.Abs(Distance - cnt) < mmry)
                    {
                        real_distance = cnt;
                    }
                    cnt += dif;
                    cnt = Math.Round(cnt, 12);
                }
                _distance = real_distance;
                textBox_dist.Text = real_distance.ToString();
                if (!dist_roll)
                {
                    taskBar_dist_setValue(real_distance);
                }
            }
        }
         
        private double distance()
        {
            double Distance = 0;
            if (Main.calc_type)
            {
                Distance = (_distance * Main.Ion_velociy) / 1000000;
            }
            else
            {
                Distance = _distance;
            }
            return Distance;
        }

        private void set_e_field(double E_Field)
        {
            double mmry;
            double step = (Main.field_end - Main.field_start) / (Main.field_steps - 1);
            double cnt = Main.field_start;
            double real_field = Main.field_start;
            while (cnt <= Main.field_end)
            {
                mmry = Math.Abs(E_Field - real_field);
                if (Math.Abs(E_Field - cnt) < mmry)
                {
                    real_field = cnt;
                }
                cnt += step;
                cnt = Math.Round(cnt, 10);
            }
            _E_field = real_field;
            if (_E_field > 100)
            {
                textBox_ele.Text = _E_field.ToString("0.00E0#", CultureInfo.InvariantCulture);
            }
            else
            {
                textBox_ele.Text = _E_field.ToString();
            }
            if (!conc_roll)
            {
                taskBar_ele_setValue(_E_field);
            }
        }

        private double e_field()
        {
            return _E_field;
        }

        private void taskBar_dist_setValue(double Value)
        {
            if (Main.calc_type)
            {
                if (is_time)
                {
                    trackBar_dist.Value = Convert.ToInt32((Value * Main.NumberOfSteps) / Main.Time_duration);
                }
                else
                {
                    //double val = (Main.Ion_velociy * Value) / 1000000;
                    trackBar_dist.Value = Convert.ToInt32((Value * Main.NumberOfSteps) / Main.Distance);
                }
            }
            else
            {
                if (is_time)
                {
                    //double val = (1000000 * Value) / Main.Ion_velociy;
                    trackBar_dist.Value = Convert.ToInt32((Value * Main.NumberOfSteps) / Main.Time_duration);
                }
                else
                {
                    trackBar_dist.Value = Convert.ToInt32((Value * Main.NumberOfSteps) / Main.Distance);
                }
            }
        }

        private void taskBar_conc_setValue(double Value)
        {
            trackBar_conc.Value = Convert.ToInt32(((Value - Main.conc_start) * (Main.conc_steps - 1)) / (Main.conc_end - Main.conc_start));
        }

        private void taskBar_ele_setValue(double Value)
        {
            trackBar_ele.Value = Convert.ToInt32(((Value - Main.field_start) * (Main.field_steps - 1)) / (Main.field_end - Main.field_start));

        }

        private void textBox_dist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (is_time)
                {
                    set_time(Main.Convertor(textBox_dist.Text));
                }
                else
                {
                    set_distanc(Main.Convertor(textBox_dist.Text));
                }
                update_axis = false;
                start_animation();
                update_axis = true;
            }
        }

        private void textBox_conc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                set_concentration(Main.Convertor(textBox_conc.Text));
                update_axis = false;
                start_animation();
                update_axis = true;
            }
        }

        private void textBox_ele_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                set_e_field(Main.Convertor(textBox_ele.Text));
                update_axis = false;
                start_animation();
                update_axis = true;
            }
        }

        private List<double[]> Gauss_function(double Mass,double Intensity)
        {
            List<double[]> ResultsColection = new List<double[]>();
            int max_mass = Convert.ToInt32(Mass) + 10;
            double res_koef = Intensity; /// (Sigma * Math.Sqrt(2 * Math.PI));
            double MS_density = Main.Gauss_density;
            double Sigma = Main.Gauss_signa;

            for (int i = 0; i < (max_mass * MS_density); i++)
            {
                double x = i / MS_density;
                double res_exp = ((-1) * (Math.Pow((x - Mass), 2)) / (2 * Sigma * Sigma));
                double res = res_koef * Math.Exp(res_exp);
                double[] dou = new double[2];
                dou[0] = x;
                dou[1] = res;
                ResultsColection.Add(dou);
            }
            return ResultsColection;
        }

        private void button_t_x_Click(object sender, EventArgs e)
        {
            if (is_time)
            {
                is_time = false;
                if (Popis_X == "Time [us]")
                {
                    Popis_X = "Distance [cm]";
                }
                label16.Text = "Distance [cm]:";
                set_distanc((time() * Main.Ion_velociy) / 1000000);
                if (!textBox_dist.Enabled)
                {
                    textBox_dist.Text = "";
                }
                button_t_x.Text = "x --> t";
            }
            else
            {
                is_time = true;
                if (Popis_X == "Distance [cm]")
                {
                    Popis_X = "Time [us]";
                }
                label16.Text = "Time [us]:";
                set_time((distance() * 1000000) / Main.Ion_velociy);
                if (!textBox_dist.Enabled)
                {
                    textBox_dist.Text = "";
                }
                button_t_x.Text = "t --> x";
            }
            start_animation();
        }

        private void button_plot_Click(object sender, EventArgs e)
        {
            start_animation();
        }

        private void button_ms_Click(object sender, EventArgs e)
        {
            if (convolution)
            {
                button_ms.Text = "MS Gaus - ON";
                convolution = false;
            }
            else
            {
                button_ms.Text = "MS Gaus - OFF";
                convolution = true;
            }
            start_animation();
        }

        private void textBox_ele_Leave(object sender, EventArgs e)
        {
            set_e_field(Main.Convertor(textBox_ele.Text));
        }

        private void textBox_conc_Leave(object sender, EventArgs e)
        {
            set_concentration(Main.Convertor(textBox_conc.Text));
        }

        private void textBox_dist_Leave(object sender, EventArgs e)
        {
            if (is_time)
            {
                set_time(Main.Convertor(textBox_dist.Text));
            }
            else
            {
                set_distanc(Main.Convertor(textBox_dist.Text));
            }
        }

        private void trackBar_ele_Leave(object sender, EventArgs e)
        {
            update_axis = false;
            start_animation();
            update_axis = true;
        }

        private void trackBar_conc_Leave(object sender, EventArgs e)
        {
            update_axis = false;
            start_animation();
            update_axis = true;
        }

        private void trackBar_dist_Leave(object sender, EventArgs e)
        {
            update_axis = false;
            start_animation();
            update_axis = true;
        }

        private void trackBar_ele_Scroll(object sender, EventArgs e)
        {
            ele_roll = true;
            double actual = Main.field_start + (trackBar_ele.Value * (Main.field_end - Main.field_start)) / (Main.field_steps - 1);
            set_e_field(actual);
            ele_roll = false;
            update_axis = false;
            start_animation();
            update_axis = true;
        }

        private void trackBar_conc_Scroll(object sender, EventArgs e)
        {
            conc_roll = true;
            double actual = Main.conc_start + (trackBar_conc.Value * (Main.conc_end - Main.conc_start)) / (Main.conc_steps - 1);
            set_concentration(actual);
            conc_roll = false;
            update_axis = false;
            start_animation();
            update_axis = true;
        }

        private void trackBar_dist_Scroll(object sender, EventArgs e)
        {
            dist_roll = true;
            if (is_time)
            {
                double actual = (trackBar_dist.Value * Main.Time_duration) / Main.NumberOfSteps;
                set_time(actual);
            }
            else
            {
                double actual = (trackBar_dist.Value * Main.Distance) / Main.NumberOfSteps;
                set_distanc(actual);
            }
            dist_roll = false;
            update_axis = false;
            start_animation();
            update_axis = true;
        }

        private void checkBox_log_CheckedChanged(object sender, EventArgs e)
        {
            start_animation();
            if (checkBox_log.Checked)
            {
                log_lin = true;
            }
            else
            {
                log_lin = false;
            }
        }

        # endregion      

        private void Calculation_control_Load(object sender, EventArgs e)
        {
            Load_type = -1;
            this.Location = this.Owner.Location;
            resize_form();
        }

        private void button2_Click(object sender, EventArgs e)
        { // calculation memory decrease
            if (calculation_memory_chosen > 1)
            {
                calculation_memory_chosen--;
            }
            else
            {
                calculation_memory_chosen = 1;
            }
            int i = 0;
            foreach (List<List<double[]>> memory_part in calculation_memory)
            {
                if (i == calculation_memory_chosen - 1)
                {
                    int j = 0;
                    foreach (List<string> memory_part_title in calculation_memory_title)
                    {
                        if (i == j)
                        {

                            // display
                            PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
                            model.LegendTitle = "Legend";
                            model.LegendOrientation = LegendOrientation.Horizontal;
                            model.LegendPlacement = LegendPlacement.Inside;
                            model.LegendPosition = LegendPosition.RightTop;
                            int k = 0;
                            foreach (List<double[]> line in memory_part)
                            {
                                int l = 0;
                                foreach (string name in memory_part_title)
                                {
                                    if (k == l)
                                    {
                                        FunctionSeries fs = new FunctionSeries();
                                        foreach (double[] dou in line)
                                        {
                                            fs.Points.Add(new DataPoint(dou[0], dou[1]));
                                        }
                                        fs.Title = name;
                                        model.Series.Add(fs);
                                    }
                                }
                            }
                            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });
                            if (checkBox_log.Checked)
                            {
                                model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
                            }
                            else
                            {
                                model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
                            }
                            plot1.Model = model;
                        }
                    }
                }
            }
            this.label25.Text = calculation_memory_chosen.ToString() + "/" + calculation_memory.Count.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {// calculation memory increase
            if (calculation_memory_chosen < calculation_memory.Count)
            {
                calculation_memory_chosen++;
            }
            else
            {
                calculation_memory_chosen = calculation_memory.Count;
            }
            int i = 0;
            foreach (List<List<double[]>> memory_part in calculation_memory)
            {
                if (i == calculation_memory_chosen - 1)
                {
                    int j = 0;
                    foreach (List<string> memory_part_title in calculation_memory_title)
                    {
                        if (i == j)
                        {

                            // display
                            PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
                            model.LegendTitle = "Legend";
                            model.LegendOrientation = LegendOrientation.Horizontal;
                            model.LegendPlacement = LegendPlacement.Inside;
                            model.LegendPosition = LegendPosition.RightTop;
                            int k = 0;
                            foreach (List<double[]> line in memory_part)
                            {
                                int l = 0;
                                foreach (string name in memory_part_title)
                                {
                                    if (k == l)
                                    {
                                        FunctionSeries fs = new FunctionSeries();
                                        foreach (double[] dou in line)
                                        {
                                            fs.Points.Add(new DataPoint(dou[0], dou[1]));
                                        }
                                        fs.Title = name;
                                        model.Series.Add(fs);
                                    }
                                }
                            }
                            model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });
                            if (checkBox_log.Checked)
                            {
                                model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
                            }
                            else
                            {
                                model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
                            }
                            plot1.Model = model;
                        }
                    }
                }
            }
            this.label25.Text = calculation_memory_chosen.ToString() + "/" + calculation_memory.Count.ToString();
        }

        #region Interpolation

        // Structure and control of the interpolation mechanism
        // implementation of the interpolation parametes

        private void button_interpolation_Click(object sender, EventArgs e)
        {

        }

        private void enable_interpolation()
        {
            if (Load_type != -1 && calculated_data)
            {
                interpolation();
            }
        }

        private void interpolation()
        {
            double total_error = Math.Round(total_error_value(), 13);
            if (total_error > 5)
            {
                total_error = Math.Round(total_error_value(), 1);
            }
            else
            {
                total_error = Math.Round(total_error_value(), 13);
            }
            label24.Text = "D: " + total_error.ToString();
        }

        private double total_error_value()
        {
            double total_error = 0;
            int i_str = 0;
            error_evolution = new List<List<double[]>>();
            // sparovanie hmotnosti
            foreach (string str in titul)
            {
                List<double[]> error_partial = new List<double[]>();
                foreach (Items_used Item in ItemColection_used)
                {
                    if (str == Item.name)
                    {
                        for (int i = 0; i < Load_hlavicka.GetLength(0); i++)
                        {
                            if (Item.mass == Load_hlavicka[i])
                            {
                                // i te body v Loaded_data[i,] koresponduju s hlavickou
                                int i_load = 0;
                                foreach (List<double[]> line in export)
                                {
                                    if (i_load == i_str)
                                    {
                                        // porovnanie teorie z line a dat z Loaded_data[i,?]
                                        // 1, pre kazdu dvojicu bodov sprav linear. interpolaciu
                                        for (int j = 0; j < Loaded_data.GetLength(1); j++)
                                        {

                                            double[] old_dou = new double[2];
                                            short p = 0;
                                            foreach (double[] dou in line)
                                            {
                                                if (p > 0)
                                                {
                                                    // hladame ekvivalent pre Loaded_data[i,j];
                                                    // je v rozsahu ?
                                                    if ((Loaded_data[0, j] > old_dou[0]) && (Loaded_data[0, j] <= dou[0]))
                                                    {
                                                        // parametre linernej priamky
                                                        double a = (old_dou[1] - dou[1]) / (old_dou[0] - dou[0]);
                                                        double b = dou[1] - a * dou[0];
                                                        // vyp relat hodnotu v meranom bode 
                                                        double relat_Y = a * Loaded_data[0, j] + b; // teoret hodnota v bode merania
                                                        // porovnanie teoret. a exper. hodnoty
                                                        double different = Math.Pow(relat_Y - Loaded_data[i, j], 2);
                                                        double[] error_point = { Loaded_data[0, j], different };
                                                        error_partial.Add(error_point);
                                                        total_error += different;
                                                    }

                                                }
                                                old_dou = dou;
                                                p++;
                                            }

                                        }


                                    }
                                    i_load++;
                                }
                            }
                        }
                    }
                }
                i_str++;
                error_evolution.Add(error_partial);
            }
            return total_error;
        }

        #endregion

        private bool minimalized = false;

        private void tabControl1_DoubleClick(object sender, EventArgs e)
        {
            if (minimalized)
            { // max
                minimalized = false;
            }
            else
            { // min
                minimalized = true;
            }
            resize_form();
        }

        private void resize_form()
        {
            if (minimalized)
            { // min
                this.tabControl1.Height = 20;
                plot1.Width = this.ClientSize.Width;
                plot1.Location = new Point(0, 20);
            }
            else
            { // max
                this.tabControl1.Height = this.ClientSize.Height - 22;
                plot1.Location = new Point(this.tabControl1.Width, 20);
                plot1.Width = this.ClientSize.Width - this.tabControl1.Width;
            }
        }

        private void Calculation_control_SizeChanged(object sender, EventArgs e)
        {
            resize_form();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            update_axis = false;
            start_animation();
            update_axis = false;
        }

        public double Lambda()
        {
            bool Infinite = Main.Infinite_system;
            double _lambda = 0;
            if (Infinite)
            {
                _lambda = (Main.Radius / 2.405);
            }
            else
            {
                double num = 1;
                num = (2.405 / Main.Radius) * (2.405 / Main.Radius) + (Math.PI / Main.Distance) * (Math.PI / Main.Distance);
                _lambda = Math.Sqrt(1 / num);
            }
            return _lambda;
        }

        #region Data_managment

        private void Show_data_colection()
        {
            listView1.Items.Clear();
            int i = 0;
            foreach(Data_storage data_storage in Main.data_storage)
            {
                string ele_status = "False";
                if(data_storage.Is_Ele)
                {
                    ele_status = "True";
                }
                if (data_storage.Is_Ele_din)
                {
                    ele_status = "Dinamic";
                }
                string[] row = new string[] {data_storage.Time.ToString(), data_storage.Dimension.ToString(), data_storage.Is_Con_din.ToString(), ele_status };
                ListViewItem item = new ListViewItem(row);
                item.Tag = data_storage;
                listView1.Items.Add(item);
                i++;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // save selected
            var item_colection = listView1.SelectedItems;
            Data_storage ItemList = new Data_storage();
            foreach (ListViewItem item in item_colection)
            {
                ItemList = (Data_storage)item.Tag;
            }
            saveFileDialog1.Title = "Save data storage ";
            saveFileDialog1.FileName = "Data storage " + ItemList.ToString() + ".dst";
            saveFileDialog1.Filter = "dst files (*.dst)|*.dst";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;           
            string save = "";
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, ItemList);
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

        private void button4_Click(object sender, EventArgs e)
        {
            // delete data
            var item_colection = listView1.SelectedItems;
            List<Data_storage> DataList = new List<Data_storage>();
            List<ListViewItem> ItemList = new List<ListViewItem>();
            foreach (ListViewItem item in item_colection)
            {
                DataList.Add((Data_storage)item.Tag);
                ItemList.Add(item);
            }
            foreach(Data_storage DS in DataList)
            {
                Main.data_storage.Remove(DS);
            }
            Show_data_colection();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // export details
            var item_colection = listView1.SelectedItems;
            Data_storage ItemList = new Data_storage();
            foreach (ListViewItem item in item_colection)
            {
                ItemList = (Data_storage)item.Tag;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Export data details";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FileName = "Data_details" + ItemList.ToString() + ".txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (!saveFileDialog1.Title.Contains(".txt"))
            { saveFileDialog1.Title += ".txt"; }
            string export = "";
            export += "Time: " + ItemList.Time.ToString() + "\r\n";
            export += "Dimension: " + ItemList.Dimension.ToString() + "\r\n";
            export += "Electric field: " + ItemList.Is_Ele.ToString() + "\r\n";
            export += "Electric field dinamic: " + ItemList.Is_Ele_din.ToString() + "\r\n";
            export += "Concentration dinamic: " + ItemList.Is_Con_din.ToString() + "\r\n\r\n";
        
            export += "Reaction colection:\r\n";
            export += "Reaction\trate constant\r\n";
            foreach (Reactions_used RU in ItemList.Reactions_collection_used)
            {
                export += RU.Specific_name() + "\t" + RU.rate_konstant.representation + "\r\n";
                if (RU.reaction_type == 2)
                {
                    export += RU.Specific_name_() + "\t" + RU.rate_konstant_.representation + "\r\n";
                }
                export += "\r\n";
            }
            export += "Item colection:\r\n";
            export += "Name\tFormula\tConcentration\tDiffusion\tMobility\r\n";

            foreach (Items_used IU in ItemList.Items_collection_used)
            {
                export += IU.name + "\t" + IU.formula + "\t" + IU.concentration + "\t" + IU.diffusion + "\t" + IU.mobility.representation + "\r\n";
            }            
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    Cursor.Current = Cursors.WaitCursor;
                    StreamWriter write = new StreamWriter(saveFileDialog1.OpenFile());
                    write.Write(export);
                    write.Dispose();
                    write.Close();
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void button7_Click(object sender, EventArgs e)
        { // use data
            var item_colection = listView1.SelectedItems;
            Data_storage ItemList = new Data_storage();
            foreach (ListViewItem item in item_colection)
            {
                ItemList = (Data_storage)item.Tag;
            }
            hlavicka = ItemList.Header;
            data_dim = Convert.ToSByte(ItemList.Dimension);
            Concentration_name = ItemList.Concentration_name;
            current_electric_field = ItemList.Electric_field_konstant_value;
            is_time = ItemList.Is_Time;
            is_con_din = ItemList.Is_Con_din;
            is_ele = ItemList.Is_Ele;
            is_ele_din = ItemList.Is_Ele_din;
            ItemColection_used = ItemList.Items_collection_used;
            ReactionColection_used = ItemList.Reactions_collection_used;
            show_values();
            ItemList.Used_settings.Apply_settings();
            if (ItemList.Dimension == 1)
            {
                foreach (List<List<double[,]>> d2construct in ItemList.Calculation_memmory)
                {
                    foreach (List<double[,]> d1construct in d2construct)
                    {
                        tabulka = d1construct;
                    }
                }
            }
            if (ItemList.Dimension == 2)
            {
                foreach (List<List<double[,]>> d2construct in ItemList.Calculation_memmory)
                {
                    data_D2 = d2construct;
                }
            }
            if (ItemList.Dimension == 3)
            {
                data_D3 = ItemList.Calculation_memmory;
            }
            start_vizualization();
            iniciate_listbox();
            start_animation();
            toolStripStatusLabel1.Text = "Data loaded";
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // draw details
            if (listView1.SelectedItems != null)
            {
                try
                {
                    var item_colection = listView1.SelectedItems;
                    string s_1 = "";
                    string s_2 = "";
                    foreach (ListViewItem item in item_colection)
                    {
                        s_1 = "";
                        s_2 = "";
                        Data_storage data_storage = (Data_storage)item.Tag;
                        List<Items_used> items_used = new List<Items_used>();
                        foreach (Reactions_used reactions in data_storage.Reactions_collection_used)
                        {
                            s_1 += reactions.Specific_name() + "\t" + reactions.rate_konstant.representation + "\r\n";
                            if (reactions.reaction_type == 2)
                            {
                                s_1 += reactions.Specific_name_() + "\t" + reactions.rate_konstant_.representation + "\r\n";
                            }
                        }
                    }
                    textBox7.Text = s_1 + s_2;
                    // show data
                    Data_storage ItemList = new Data_storage();
                    foreach (ListViewItem item in item_colection)
                    {
                        ItemList = (Data_storage)item.Tag;
                    }
                    hlavicka = ItemList.Header;
                    data_dim = Convert.ToSByte(ItemList.Dimension);
                    Concentration_name = ItemList.Concentration_name;
                    current_electric_field = ItemList.Electric_field_konstant_value;
                    is_time = ItemList.Is_Time;
                    is_con_din = ItemList.Is_Con_din;
                    is_ele = ItemList.Is_Ele;
                    is_ele_din = ItemList.Is_Ele_din;
                    if (ItemList.Dimension == 1)
                    {
                        foreach (List<List<double[,]>> d2construct in ItemList.Calculation_memmory)
                        {
                            foreach (List<double[,]> d1construct in d2construct)
                            {
                                tabulka = d1construct;
                            }
                        }
                    }
                    if (ItemList.Dimension == 2)
                    {
                        foreach (List<List<double[,]>> d2construct in ItemList.Calculation_memmory)
                        {
                            data_D2 = d2construct;
                        }
                    }
                    if (ItemList.Dimension == 3)
                    {
                        data_D3 = ItemList.Calculation_memmory;
                    }
                    start_vizualization();
                    iniciate_listbox();
                    start_animation();
                    toolStripStatusLabel1.Text = "Data loaded";
                }
                catch { }
            }
        }
        #endregion

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                relative_values = true;
            }
            else
            {
                relative_values = false;
            }
            estimate_relative_values();
            update_axis = false;
            start_animation();
            update_axis = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Add SRI
            AddNeutral addneutral = new AddNeutral();
            List<Items> used_items = new List<Items>();
            foreach(Items_used items_ in ItemColection_used)
            {
                used_items.Add(items_.GetItem());
            }
            addneutral.Items = used_items;
            addneutral.Data_type = false;
            addneutral.Remove_data = false;
            addneutral.to_SRI_listbox = true;
            addneutral.ShowCollection();
            addneutral.Show(this);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Remove SRI
            if (listBox1.SelectedItem != null)
            {
                Items ToBeRemoved = null;
                foreach (Items item in listBox1.Items)
                {
                    if (listBox1.SelectedItem == item)
                    {
                        ToBeRemoved = item;
                    }
                }
                listBox1.Items.Remove(ToBeRemoved);
            }
            else
            {
                AddNeutral addneutral = new AddNeutral();
                List<Items> SRI_list = new List<Items>();
                foreach (Items item in listBox1.Items)
                {
                    SRI_list.Add(item);
                }
                addneutral.Items = SRI_list;
                addneutral.Data_type = false;
                addneutral.Remove_data = true;
                addneutral.to_SRI_listbox = true;
                addneutral.ShowCollection();
                addneutral.Show(this);
            }
            check_button();
        }

        public static void check_button()
        {
            if (listBox1.Items.Count == 0)
            {
                button9.Enabled = false;
            }
            else
            {
                button9.Enabled = true;
            }
        }
    }
}
