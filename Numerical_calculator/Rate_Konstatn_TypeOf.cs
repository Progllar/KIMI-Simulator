using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace KIMI_Sim
{
    public partial class Rate_Konstatn_TypeOf : Form
    {
        public Rate_Konstatn_TypeOf()
        {
            InitializeComponent();
        }

        bool _invert = false;
        bool _mobility = false;
        bool _cal_control = false;

        public void proceed(rate_functions Rate_function, bool invert, bool mobility, bool Cal_control)
        {
            Constant constant = new Constant();
            Linear linear = new Linear();
            Quadratic quadratic = new Quadratic();
            Cubic cubic = new Cubic();
            Exponencional exponencional = new Exponencional();
            Exponencional2 exponencional2 = new Exponencional2();
            Exponencional3 exponencional3 = new Exponencional3();
            Exponencional4 exponencional4 = new Exponencional4();
            Exponencional5 exponencional5 = new Exponencional5();
            Association acoss = new Association();

            if (mobility) // functions for a mobility
            {
                comboBox1.Items.Add(constant);
                comboBox1.Items.Add(linear);
                comboBox1.Items.Add(quadratic);
                comboBox1.Items.Add(cubic);
                comboBox1.Items.Add(exponencional);
                comboBox1.Items.Add(exponencional3);
                comboBox1.Items.Add(exponencional4);
                comboBox1.Items.Add(exponencional5);
            }
            else // functions for a rate konstant
            {
                comboBox1.Items.Add(constant);
                comboBox1.Items.Add(exponencional2);
                comboBox1.Items.Add(exponencional3);
                comboBox1.Items.Add(acoss);
            }
            if (Rate_function != null)
            {
                if (Rate_function.ToString() == constant.ToString())
                {
                    comboBox1.Items.Remove(constant);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = false;
                    checkBox3.Enabled = false;
                    textBox4.Enabled = false;
                    checkBox4.Enabled = false;
                    textBox5.Enabled = false;
                    checkBox5.Enabled = false;
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";

                }
                if (Rate_function.ToString() == linear.ToString())
                {
                    comboBox1.Items.Remove(linear);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox3.Text = Rate_function.B.ToString();
                    checkBox3.Checked = Rate_function.B_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = true;
                    checkBox3.Enabled = true;
                    textBox4.Enabled = false;
                    checkBox4.Enabled = false;
                    textBox5.Enabled = false;
                    checkBox5.Enabled = false;
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
                if (Rate_function.ToString() == quadratic.ToString())
                {
                    comboBox1.Items.Remove(quadratic);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox3.Text = Rate_function.B.ToString();
                    checkBox3.Checked = Rate_function.B_lock;
                    textBox4.Text = Rate_function.C.ToString();
                    checkBox4.Checked = Rate_function.C_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = true;
                    checkBox3.Enabled = true;
                    textBox4.Enabled = true;
                    checkBox4.Enabled = true;
                    textBox5.Enabled = false;
                    checkBox5.Enabled = false;
                    textBox5.Text = "";
                }
                if (Rate_function.ToString() == cubic.ToString())
                {
                    comboBox1.Items.Remove(cubic);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox3.Text = Rate_function.B.ToString();
                    checkBox3.Checked = Rate_function.B_lock;
                    textBox4.Text = Rate_function.C.ToString();
                    checkBox4.Checked = Rate_function.C_lock;
                    textBox5.Text = Rate_function.D.ToString();
                    checkBox5.Checked = Rate_function.D_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = true;
                    checkBox3.Enabled = true;
                    textBox4.Enabled = true;
                    checkBox4.Enabled = true;
                    textBox5.Enabled = true;
                    checkBox5.Enabled = true;
                }
                if (Rate_function.ToString() == exponencional.ToString())
                {
                    comboBox1.Items.Remove(exponencional);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox3.Text = Rate_function.B.ToString();
                    checkBox3.Checked = Rate_function.B_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = true;
                    checkBox3.Enabled = true;
                    textBox4.Enabled = false;
                    checkBox4.Enabled = false;
                    textBox5.Enabled = false;
                    checkBox5.Enabled = false;
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
                if (Rate_function.ToString() == exponencional2.ToString())
                {
                    comboBox1.Items.Remove(exponencional2);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox3.Text = Rate_function.B.ToString();
                    checkBox3.Checked = Rate_function.B_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = true;
                    checkBox3.Enabled = true;
                    textBox4.Enabled = false;
                    checkBox4.Enabled = false;
                    textBox5.Enabled = false;
                    checkBox5.Enabled = false;
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
                if (Rate_function.ToString() == exponencional3.ToString())
                {
                    comboBox1.Items.Remove(exponencional3);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox3.Text = Rate_function.B.ToString();
                    checkBox3.Checked = Rate_function.B_lock;
                    textBox4.Text = Rate_function.C.ToString();
                    checkBox4.Checked = Rate_function.C_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = true;
                    checkBox3.Enabled = true;
                    textBox4.Enabled = true;
                    checkBox4.Enabled = true;
                    textBox5.Enabled = false;
                    checkBox5.Enabled = false;
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
                if (Rate_function.ToString() == exponencional4.ToString())
                {
                    comboBox1.Items.Remove(exponencional4);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox3.Text = Rate_function.B.ToString();
                    checkBox3.Checked = Rate_function.B_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = true;
                    checkBox3.Enabled = true;
                    textBox4.Enabled = false;
                    checkBox4.Enabled = false;
                    textBox5.Enabled = false;
                    checkBox5.Enabled = false;
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
                if (Rate_function.ToString() == exponencional5.ToString())
                {
                    comboBox1.Items.Remove(exponencional5);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox3.Text = Rate_function.B.ToString();
                    checkBox3.Checked = Rate_function.B_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = true;
                    checkBox3.Enabled = true;
                    textBox4.Enabled = false;
                    checkBox4.Enabled = false;
                    textBox5.Enabled = false;
                    checkBox5.Enabled = false;
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
                if (Rate_function.ToString() == acoss.ToString())
                {
                    comboBox1.Items.Remove(acoss);
                    comboBox1.Items.Add(Rate_function);
                    textBox2.Text = Rate_function.A.ToString();
                    checkBox2.Checked = Rate_function.A_lock;
                    textBox3.Text = Rate_function.B.ToString();
                    checkBox3.Checked = Rate_function.B_lock;
                    textBox2.Enabled = true;
                    checkBox2.Enabled = true;
                    textBox3.Enabled = true;
                    checkBox3.Enabled = true;
                    textBox4.Enabled = false;
                    checkBox4.Enabled = false;
                    textBox5.Enabled = false;
                    checkBox5.Enabled = false;
                    textBox4.Text = "";
                    textBox5.Text = "";
                }
                comboBox1.SelectedItem = Rate_function;
                comboBox1.Text = Rate_function.ToString();
            }
            _invert = invert;
            _mobility = mobility;
            _cal_control = Cal_control;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedItem.ToString() == "A")
                {
                    Constant new_constant = new Constant(Convertor(textBox2.Text), checkBox2.Checked);
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_constant;
                    }
                    else
                    {
                        Resources.item_mobility = new_constant;
                    }
                }
                if (comboBox1.SelectedItem.ToString() == "A + B*(E/N)")
                {
                    Linear new_linear = new Linear(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_linear;
                    }
                    else
                    {
                        Resources.item_mobility = new_linear;
                    }
                }
                if (comboBox1.SelectedItem.ToString() == "A + B*(E/N) + C*(E/N)^2")
                {
                    Quadratic new_quadratic = new Quadratic(Convertor(textBox2.Text), Convertor(textBox3.Text), Convertor(textBox4.Text), checkBox2.Checked, checkBox3.Checked, checkBox4.Checked);
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_quadratic;
                    }
                    else
                    {
                        Resources.item_mobility = new_quadratic;
                    }
                }
                if (comboBox1.SelectedItem.ToString() == "A + B*(E/N) + C*(E/N)^2 + D*(E/N)^3")
                {
                    Cubic new_cubic = new Cubic(Convertor(textBox2.Text), Convertor(textBox3.Text), Convertor(textBox4.Text), Convertor(textBox5.Text), checkBox2.Checked, checkBox3.Checked, checkBox4.Checked, checkBox5.Checked);
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_cubic;
                    }
                    else
                    {
                        Resources.item_mobility = new_cubic;
                    }
                }
                if (comboBox1.SelectedItem.ToString() == "A*EXP(B*(E/N))")
                {
                    Exponencional new_exponencional = new Exponencional(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_exponencional;
                    }
                    else
                    {
                        Resources.item_mobility = new_exponencional;
                    }
                }
                if (comboBox1.SelectedItem.ToString() == "A*EXP(B/Er(E/N))")
                {
                    Exponencional2 new_exponencional2 = new Exponencional2(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_exponencional2;
                    }
                    else
                    {
                        Resources.item_mobility = new_exponencional2;
                    }

                }
                if (comboBox1.SelectedItem.ToString() == "C + A*EXP(B/(E/N))")  //Modify to A*EXP(B/C + E)
                {
                    Exponencional3 new_exponencional3 = new Exponencional3(Convertor(textBox2.Text), Convertor(textBox3.Text), Convertor(textBox4.Text), checkBox2.Checked, checkBox3.Checked, checkBox4.Checked);
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_exponencional3;
                    }
                    else
                    {
                        Resources.item_mobility = new_exponencional3;
                    }

                }
                if (comboBox1.SelectedItem.ToString() == "A/(1+(E/N))^B")
                {
                    Exponencional4 new_exponencional4 = new Exponencional4(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_exponencional4;
                    }
                    else
                    {
                        Resources.item_mobility = new_exponencional4;
                    }
                }
                if (comboBox1.SelectedItem.ToString() == "A*EXP(B*(E/N)^2)")
                {
                    Exponencional5 new_exponencional = new Exponencional5(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_exponencional;
                    }
                    else
                    {
                        Resources.item_mobility = new_exponencional;
                    }
                }
                if (comboBox1.SelectedItem.ToString() == "A/Er(E/N)^B")
                {
                    Association new_acoss = new Association(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    Main.new_rate_function = new_acoss;
                    if (!_cal_control)
                    {
                        Main.new_rate_function = new_acoss;
                    }
                    else
                    {
                        Resources.item_mobility = new_acoss;
                    }
                }
                if (!_mobility)
                {
                    if (_invert)
                    {
                        Main.use_new_rate_();
                    }
                    else
                    {
                        Main.use_new_rate();
                    }
                }
                else
                {
                    if (!_cal_control)
                    {
                        Main.use_new_mobility();
                    }
                    else
                    {
                        Resources.use_new_mobility();
                    }
                }
                this.Dispose();
            }
            catch
            {
                MessageBox.Show("Unable to use selected function.");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "A")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = false;
                checkBox3.Enabled = false;
                textBox4.Enabled = false;
                checkBox4.Enabled = false;
                textBox5.Enabled = false;
                checkBox5.Enabled = false;
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox1.Text = "Formula: A   Constat function independent on electric field.";
            }
            if (comboBox1.SelectedItem.ToString() == "A + B*(E/N)")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox3.Enabled = true;
                textBox4.Enabled = false;
                checkBox4.Enabled = false;
                textBox5.Enabled = false;
                checkBox5.Enabled = false;
                textBox4.Text = "";
                textBox5.Text = "";
                textBox1.Text = "Formula: A + B*(E/N)   Linear function";
            }
            if (comboBox1.SelectedItem.ToString() == "A + B*(E/N) + C*(E/N)^2")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox3.Enabled = true;
                textBox4.Enabled = true;
                checkBox4.Enabled = true;
                textBox5.Enabled = false;
                checkBox5.Enabled = false;
                textBox5.Text = "";
                textBox1.Text = "Formula: A + B * (E / N) + C * (E / N) ^ 2    Quadratic function";

            }
            if (comboBox1.SelectedItem.ToString() == "A + B*(E/N) + C*(E/N)^2 + D*(E/N)^3")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox3.Enabled = true;
                textBox4.Enabled = true;
                checkBox4.Enabled = true;
                textBox5.Enabled = true;
                checkBox5.Enabled = true;
                textBox1.Text = "Formula: A + B*(E/N) + C*(E/N)^2 + D*(E/N)^3    Cubic function";

            }
            if (comboBox1.SelectedItem.ToString() == "A*EXP(B*(E/N))")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox3.Enabled = true;
                textBox4.Enabled = false;
                checkBox4.Enabled = false;
                textBox5.Enabled = false;
                checkBox5.Enabled = false;
                textBox4.Text = "";
                textBox5.Text = "";
                textBox1.Text = "Formula: A*EXP(B*(E/N))    Exponentional function";

            }
            if (comboBox1.SelectedItem.ToString() == "A*EXP(B/Er(E/N))")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox3.Enabled = true;
                textBox4.Enabled = false;
                checkBox4.Enabled = false;
                textBox5.Enabled = false;
                checkBox5.Enabled = false;
                textBox4.Text = "";
                textBox5.Text = "";
                textBox1.Text = "Formula: A*EXP(B/(E/N))    Arrhenius formula suitable for a proton transfer reaction or a dissociation. The element B represents actication energy in eV and should be negative. The element A represents upper limit for the rate konstant. Reaction energy is calculated using masses of reagent ion and reagent neutral m = (mr/(mr+mi))*(mi+mc). The animation use mass element = 16 (for mi = 19, mc = 4, mr = 48)";

            }
            if (comboBox1.SelectedItem.ToString() == "C + A*EXP(B/(E/N))")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox3.Enabled = true;
                textBox4.Enabled = true;
                checkBox4.Enabled = true;
                textBox5.Enabled = false;
                checkBox5.Enabled = false;
                textBox4.Text = "";
                textBox5.Text = "";
                textBox1.Text = "Formula: C + A*EXP(B/(E/N))    Arrhenius formula with a constant offset.";
            }
            if (comboBox1.SelectedItem.ToString() == "A/(1+(E/N))^B")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox3.Enabled = true;
                textBox4.Enabled = false;
                checkBox4.Enabled = false;
                textBox5.Enabled = false;
                checkBox5.Enabled = false;
                textBox4.Text = "";
                textBox5.Text = "";
                textBox1.Text = "Formula: A/(1+(E/N))^B    ";

            }
            if (comboBox1.SelectedItem.ToString() == "A*EXP(B*(E/N)^2)")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox3.Enabled = true;
                textBox4.Enabled = false;
                checkBox4.Enabled = false;
                textBox5.Enabled = false;
                checkBox5.Enabled = false;
                textBox4.Text = "";
                textBox5.Text = "";
                textBox1.Text = "Formula: A*EXP(B*(E/N)^2)     Exponentional function";

            }
            if (comboBox1.SelectedItem.ToString() == "A/Er(E/N)^B")
            {
                textBox2.Enabled = true;
                checkBox2.Enabled = true;
                textBox3.Enabled = true;
                checkBox3.Enabled = true;
                textBox4.Enabled = false;
                checkBox4.Enabled = false;
                textBox5.Enabled = false;
                checkBox5.Enabled = false;
                textBox4.Text = "";
                textBox5.Text = "";
                textBox1.Text = "Formula: A/Er(E/N)^B     Rate function suitable for association processes (aduct formation) in the presence of electric field. Reaction energy is calculated using masses of reagent ion and reagent neutral m = (mr/(mr+mi))*(mi+mc). The animation use mass element = 16 (for mi = 19, mc = 4, mr = 48)";
            }
            Draw();
        }

        public static double Convertor(string Str)
        {
            double dou = 0;
            string d = "";
            try
            {
                if (Str != "")
                {
                    dou = Convert.ToDouble(Str);
                }
                else
                {
                    dou = 0;
                }
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
                        //MessageBox.Show("Unable to convert string to double !");
                    }
                }
            }
            return dou;
        }

        private void Rate_Konstatn_TypeOf_Load(object sender, EventArgs e)
        {
            this.Location = this.Owner.Location;
        }

        private void Draw()
        {
            try
            {
                string title = "k (cm^3/s or cm^6/s)";
                if(_mobility)
                {
                    title = "K0(cm ^ 2 / Vs)";
                }
                PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = false };
                model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = "E (E/N)", MajorGridlineStyle = LineStyle.Dash });
                model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = title });
                LineSeries fs = new LineSeries();

                if (comboBox1.SelectedItem.ToString() == "A")
                {
                    Constant new_constant = new Constant(Convertor(textBox2.Text), checkBox2.Checked);
                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_constant.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;
                }
                if (comboBox1.SelectedItem.ToString() == "A + B*(E/N)")
                {
                    Linear new_linear = new Linear(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_linear.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;
                }
                if (comboBox1.SelectedItem.ToString() == "A + B*(E/N) + C*(E/N)^2")
                {
                    Quadratic new_quadratic = new Quadratic(Convertor(textBox2.Text), Convertor(textBox3.Text), Convertor(textBox4.Text), checkBox2.Checked, checkBox3.Checked, checkBox4.Checked);
                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_quadratic.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;
                }
                if (comboBox1.SelectedItem.ToString() == "A + B*(E/N) + C*(E/N)^2 + D*(E/N)^3")
                {
                    Cubic new_cubic = new Cubic(Convertor(textBox2.Text), Convertor(textBox3.Text), Convertor(textBox4.Text), Convertor(textBox5.Text), checkBox2.Checked, checkBox3.Checked, checkBox4.Checked, checkBox5.Checked);
                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_cubic.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;
                }
                if (comboBox1.SelectedItem.ToString() == "A*EXP(B*(E/N))")
                {
                    Exponencional new_exponencional = new Exponencional(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_exponencional.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;
                }
                if (comboBox1.SelectedItem.ToString() == "A*EXP(B/Er(E/N))")
                {
                    Exponencional2 new_exponencional2 = new Exponencional2(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    new_exponencional2.reactant_mobility = new Constant(10, true);
                    new_exponencional2.mass_element = 16*1.66053904e-27;
                    model.Axes.Clear();
                    model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = "E (V/cm)", MajorGridlineStyle = LineStyle.Dash });
                    model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "k for mass element 16u" });
                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_exponencional2.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;

                }
                if (comboBox1.SelectedItem.ToString() == "C + A*EXP(B/(E/N))")
                {
                    Exponencional3 new_exponencional3 = new Exponencional3(Convertor(textBox2.Text), Convertor(textBox3.Text), Convertor(textBox4.Text), checkBox2.Checked, checkBox3.Checked, checkBox4.Checked);

                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_exponencional3.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;

                }
                if (comboBox1.SelectedItem.ToString() == "A/(1+(E/N))^B")
                {
                    Exponencional4 new_exponencional4 = new Exponencional4(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_exponencional4.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;
                }
                if (comboBox1.SelectedItem.ToString() == "A*EXP(B*(E/N)^2)")
                {
                    Exponencional5 new_exponencional = new Exponencional5(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_exponencional.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;
                }
                if (comboBox1.SelectedItem.ToString() == "A/Er(E/N)^B")
                {
                    Association new_acoss = new Association(Convertor(textBox2.Text), Convertor(textBox3.Text), checkBox2.Checked, checkBox3.Checked);
                    new_acoss.reactant_mobility = new Constant(10, true);
                    new_acoss.mass_element = 16 * 1.66053904e-27;
                    model.Axes.Clear();
                    model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = "E (V/cm)", MajorGridlineStyle = LineStyle.Dash });
                    model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "k for mass element 16u" });


                    for (int i = 0; i < 100; i++)
                    {
                        fs.Points.Add(new DataPoint(i, new_acoss.Value(i)));
                    }
                    model.Series.Add(fs);
                    plot1.Model = model;
                }
            }
            catch { }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Draw();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Draw();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Draw();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            Draw();
        }
    }   
}
