using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using OxyPlot;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Globalization;

namespace KIMI_Sim
{
    public partial class Results_KineticModel : Form
    {
        public List<Data_storage> data_storage { get; set; }
        public string[] hlavicka { get; set; }
        public string[] hlavicka_din { get; set; }
        public List<double[,]> tabulka { get; set; }
        public List<List<double[]>> data_din { get; set; }

        public string Popis_din { get; set; }
        public bool dinamic_mode { get; set; }
        public string name_din { get; set; }
        private string Popis_X { get; set; }
        private bool is_time = true;
        public double _concentration { get; set; }

        public double _distance { get; set; }
        private bool conc_roll = false;
        private bool dist_roll = false;
        private bool convolution = false;
        private List<string> titul;
        private List<List<double[]>> export = new List<List<double[]>>();

        public Results_KineticModel()
        {
            InitializeComponent();
            trackBar_dist.Maximum = Convert.ToInt32(Main.NumberOfSteps);
            trackBar_dist.Value = trackBar_dist.Maximum;
            if (Main.results_din)
            {
                trackBar_conc.Maximum = Convert.ToInt32(Main.conc_steps - 1);
            }
        }

        public void start()
        {          
            if (Main.calc_type)
            {
                radioButton_kinetic.Checked = true;
                is_time = true;
                label1.Text = "Time [us]:";
                Popis_X = "Time [us]";
                button_t_x.Enabled = true;
            }
            else
            {
                radioButton_kinetic.Checked = true;
                is_time = false;
                label1.Text = "Distance [cm]:";
                Popis_X = "Distance [cm]";
                button_t_x.Enabled = true;
            }
            if (Main.results_din)
            {
                radioButton_dinamic.Enabled = true;
                textBox_conc.Enabled = true;
                trackBar_conc.Enabled = true;
                set_concentration(_concentration);
            }
        }

        private void button_plot_Click(object sender, EventArgs e)
        {
            start_animation();
        }      

        public void start_animation()
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
                draw_dinamic(is_time);
            }
        }

        private void draw_dinamic(bool Time)
        {
            titul = new List<string>();
            export = new List<List<double[]>>();
            if (Time) // previest vystup na cas
            {
                int i = 0;
                PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
                model.LegendTitle = "Legend";
                model.LegendOrientation = LegendOrientation.Horizontal;
                model.LegendPlacement = LegendPlacement.Inside;
                model.LegendPosition = LegendPosition.RightTop;
                foreach (string str in hlavicka_din)
                {
                    if (checkedListBox1.CheckedItems.Contains(str))
                    {
                        titul.Add(str);
                        List<double[]> partial = new List<double[]>();
                        FunctionSeries fs = new FunctionSeries();
                        foreach (List<double[]> tabulka in data_din) // cez koncentracie
                        {
                            foreach (double[] dou in tabulka) // cez suradnicu
                            {
                                if (dou[dou.GetLength(0) - 2] == _distance) // set distance
                                {
                                    //double time_dou = (dou[(dou.GetLength(0) - 1)] * 1000000) / Main.Ion_velociy;
                                    fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1)], dou[i]));
                                    double[] bod = { dou[(dou.GetLength(0) - 1)], dou[i] };
                                    partial.Add(bod);
                                }
                            }
                        }
                        fs.Title = str;
                        model.Series.Add(fs);
                        export.Add(partial);
                    }
                    i++;
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
            else
            {
                int i = 0;
                PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
                model.LegendTitle = "Legend";
                model.LegendOrientation = LegendOrientation.Horizontal;
                model.LegendPlacement = LegendPlacement.Inside;
                model.LegendPosition = LegendPosition.RightTop;
                foreach (string str in hlavicka_din)
                {
                    if (checkedListBox1.CheckedItems.Contains(str))
                    {
                        titul.Add(str);
                        List<double[]> partial = new List<double[]>();
                        FunctionSeries fs = new FunctionSeries();
                        foreach (List<double[]> tabulka in data_din) // cez koncentracie
                        {
                            foreach (double[] dou in tabulka) // cez suradnicu
                            {
                                if (dou[dou.GetLength(0) - 2] == _distance) // set distance
                                {
                                    fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1)], dou[i]));
                                    double[] bod = { dou[(dou.GetLength(0) - 1)], dou[i] };
                                    partial.Add(bod);
                                }
                            }
                        }
                        fs.Title = str;
                        model.Series.Add(fs);
                        export.Add(partial);
                    }
                    i++;
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

        private void draw_ms(bool Time)
        {
            if (!convolution)
            {
                titul = new List<string>();
                export = new List<List<double[]>>();
                if (!Main.results_din)
                {
                    int i = 0;
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
                    foreach (string str in hlavicka)
                    {

                        ColumnSeries fs = new ColumnSeries();
                        List<double[]> partial = new List<double[]>();
                        fs.BaseValue = 1;
                        int c = 1;
                        string formula = "";
                        foreach (Items Item in Main.ItemColection)
                        {
                            if ((str == Item.name) && (Item.cation))
                            {
                                formula = Item.formula;
                            }
                        }
                        string f = "";
                        foreach (char ch in formula)
                        {
                            if (ch == '+')
                            {
                            }
                            else
                            {
                                f += ch.ToString();
                            }
                        }
                        formula = f;
                        string strResults = null;
                        try
                        {
                            intSuccess = objMwtWin.ComputeIsotopicAbundances(ref formula, intChargeState, ref strResults, ref ConvolutedMSData2D, ref ConvolutedMSDataCount, blnAddProtonChargeCarrier);
                        }
                        catch { MessageBox.Show("Error in ComputeIsotopicAbundances: {0}", formula); }
                        if (intSuccess == 0)
                        {
                            List<double[]> isotopes = new List<double[]>();
                            isotopes = isotope_analysis(strResults);
                            foreach (double[,] dou in tabulka)
                            {
                                if (dou[(dou.GetLength(0) - 1), 0] == _distance)
                                {
                                    foreach (double[] m_z in isotopes)
                                    {
                                        bool enough = false;
                                        while (!enough)
                                        {
                                            if (c == Convert.ToInt32(m_z[0]))
                                            {
                                                fs.Items.Add(new ColumnItem(m_z[1] * dou[i, 0]));
                                                double[] bod = { m_z[0], m_z[1] * dou[i, 0] };
                                                partial.Add(bod);
                                                bool add = false;
                                                foreach (double[] dou_tot in total)
                                                {
                                                    if (dou_tot[0] == Convert.ToInt32(m_z[0]))
                                                    {
                                                        dou_tot[1] += (m_z[1] * dou[i, 0]);
                                                    }
                                                    else
                                                    {
                                                        add = true;
                                                    }
                                                }
                                                if (add || (total.Count == 0))
                                                {
                                                    double[] d = new double[] { Convert.ToInt32(m_z[0]), m_z[1] * dou[i, 0] };
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
                            if (checkedListBox1.CheckedItems.Contains(str))
                            {
                                fs.Title = str;
                                model.Series.Add(fs);
                                export.Add(partial);
                                titul.Add(str);
                            }
                        }
                        i++;
                    }
                    if (checkedListBox1.CheckedItems.Contains("Sum") && (total.Count > 0))
                    {
                        titul.Add("Sum");
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
                        export.Add(partial);
                        model.Series.Add(fs);
                    }
                    model.Axes.Add(new CategoryAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });
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
                else
                {
                    int i = 0;
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
                    foreach (string str in hlavicka_din)
                    {

                        ColumnSeries fs = new ColumnSeries();
                        List<double[]> partial = new List<double[]>();
                        fs.BaseValue = 1;
                        int c = 1;
                        string formula = "";
                        foreach (Items Item in Main.ItemColection)
                        {
                            if ((str == Item.name) && (Item.cation))
                            {
                                formula = Item.formula;
                            }
                        }
                        string f = "";
                        foreach (char ch in formula)
                        {
                            if (ch == '+')
                            {
                            }
                            else
                            {
                                f += ch.ToString();
                            }
                        }
                        formula = f;
                        string strResults = null;
                        try
                        {
                            intSuccess = objMwtWin.ComputeIsotopicAbundances(ref formula, intChargeState, ref strResults, ref ConvolutedMSData2D, ref ConvolutedMSDataCount, blnAddProtonChargeCarrier);
                        }
                        catch { MessageBox.Show("Error in ComputeIsotopicAbundances: {0}", formula); }
                        if (intSuccess == 0)
                        {
                            List<double[]> isotopes = new List<double[]>();
                            isotopes = isotope_analysis(strResults);
                            foreach (List<double[]> Tabulka in data_din)
                            {
                                foreach (double[] dou in Tabulka)
                                {
                                    if (dou[(dou.GetLength(0) - 2)] == _distance)
                                    {
                                        if (dou[(dou.GetLength(0) - 1)] == _concentration)
                                        {
                                            foreach (double[] m_z in isotopes)
                                            {
                                                bool enough = false;
                                                while (!enough)
                                                {
                                                    if (c == Convert.ToInt32(m_z[0]))
                                                    {
                                                        fs.Items.Add(new ColumnItem(m_z[1] * dou[i]));
                                                        double[] bod = { m_z[0], m_z[1] * dou[i] };
                                                        partial.Add(bod);
                                                        bool add = false;
                                                        foreach (double[] dou_tot in total)
                                                        {
                                                            if (dou_tot[0] == Convert.ToInt32(m_z[0]))
                                                            {
                                                                dou_tot[1] += (m_z[1] * dou[i]);
                                                            }
                                                            else
                                                            {
                                                                add = true;
                                                            }
                                                        }
                                                        if (add || (total.Count == 0))
                                                        {
                                                            double[] d = new double[] { Convert.ToInt32(m_z[0]), m_z[1] * dou[i] };
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
                            if (checkedListBox1.CheckedItems.Contains(str))
                            {
                                fs.Title = str;
                                model.Series.Add(fs);
                                export.Add(partial);
                                titul.Add(str);
                            }
                        }
                        i++;
                    }
                    if (checkedListBox1.CheckedItems.Contains("Sum") && (total.Count > 0))
                    {
                        ColumnSeries fs = new ColumnSeries();
                        List<double[]> partial = new List<double[]>();
                        titul.Add("Sum");
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
                        export.Add(partial);
                        model.Series.Add(fs);
                    }
                    model.Axes.Add(new CategoryAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });
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
            }
            else
            { // convlution
                titul = new List<string>();
                export = new List<List<double[]>>();
                if (!Main.results_din)
                {
                    int i = 0;
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
                    List<List<List<double[]>>> total = new List<List<List<double[]>>>();
                    foreach (string str in hlavicka)
                    {
                        LineSeries fs = new LineSeries();
                        List<List<double[]>> partial = new List<List<double[]>>();
                        string formula = "";
                        foreach (Items Item in Main.ItemColection)
                        {
                            if ((str == Item.name) && (Item.cation))
                            {
                                formula = Item.formula;
                            }
                        }
                        string f = "";
                        foreach (char ch in formula)
                        {
                            if (ch == '+')
                            {
                            }
                            else
                            {
                                f += ch.ToString();
                            }
                        }
                        formula = f;
                        string strResults = null;
                        try
                        {
                            intSuccess = objMwtWin.ComputeIsotopicAbundances(ref formula, intChargeState, ref strResults, ref ConvolutedMSData2D, ref ConvolutedMSDataCount, blnAddProtonChargeCarrier);
                        }
                        catch { MessageBox.Show("Error in ComputeIsotopicAbundances: {0}", formula); }
                        if (intSuccess == 0)
                        {
                            List<double[]> isotopes = new List<double[]>();
                            isotopes = isotope_analysis(strResults);
                            if (!Main.results_din)
                            {
                                foreach (double[,] dou in tabulka)
                                {
                                    if (dou[(dou.GetLength(0) - 1), 0] == _distance)
                                    {
                                        foreach (double[] m_z in isotopes)
                                        {
                                            partial.Add(Gauss_function(m_z[0], Main.Gauss_signa, dou[i, 0]));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (List<double[]> Tabulka in data_din)
                                {
                                    foreach (double[] dou in Tabulka)
                                    {
                                        if (dou[(dou.GetLength(0) - 2)] == _distance)
                                        {
                                            if (dou[(dou.GetLength(0) - 1)] == _concentration)
                                            {
                                                foreach (double[] m_z in isotopes)
                                                {
                                                    partial.Add(Gauss_function(m_z[0], Main.Gauss_signa, dou[i]));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        total.Add(partial);
                        i++;
                        if (checkedListBox1.CheckedItems.Contains(str))
                        {
                            List<double[]> export_partial = new List<double[]>(); 
                            foreach (List<double[]> dou in partial)
                            {
                                foreach (double[] d in dou)
                                {
                                    bool presented = false;
                                    foreach (double[] exp_dou in export_partial)
                                    {
                                        if(exp_dou[0] == d[0])
                                        {
                                            presented = true;
                                            exp_dou[1] += d[1];
                                        }
                                    }
                                    if (!presented)
                                    {
                                        double[] ne = new double[] { d[0], d[1]};
                                        export_partial.Add(ne);
                                    }
                                }
                            }
                            foreach (double[] exp in export_partial)
                            {
                                fs.Points.Add(new DataPoint(exp[0], exp[1]));
                            }
                            fs.Title = str;
                            model.Series.Add(fs);
                        }
                    }
                    if (checkedListBox1.CheckedItems.Contains("Sum") && (total.Count > 0))
                    {
                        LineSeries fs = new LineSeries();
                        List<double[]> export_partial = new List<double[]>();
                        foreach (List<List<double[]>> partial in total)
                        {
                            foreach (List<double[]> dou in partial)
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
                        }
                        foreach (double[] exp in export_partial)
                        {
                            fs.Points.Add(new DataPoint(exp[0], exp[1]));
                        }
                        fs.Title = "Sum";
                        model.Series.Add(fs);
                    }
                    model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });
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
            }
        }

        private void draw_kinetic(bool Time)
        {
            titul = new List<string>();
            export = new List<List<double[]>>();
            if (Main.results_din == false) // koncentracia je nepodstatna
            {
                if (Time)
                {
                    int i = 0;
                    PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };                 
                    model.LegendTitle = "Legend";
                    model.LegendOrientation = LegendOrientation.Horizontal;
                    model.LegendPlacement = LegendPlacement.Inside;
                    model.LegendPosition = LegendPosition.RightTop;
                    foreach (string str in hlavicka)
                    {
                        if (checkedListBox1.CheckedItems.Contains(str))
                        {
                            titul.Add(str);
                            List<double[]> partial = new List<double[]>();
                            FunctionSeries fs = new FunctionSeries();
                            foreach (double[,] dou in tabulka)
                            {
                                if (Main.calc_type)
                                {
                                    fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1), 0], dou[i, 0]));
                                    double[] bod = { dou[(dou.GetLength(0) - 1), 0], dou[i, 0] };
                                    partial.Add(bod);
                                }
                                else
                                {
                                    double _dou = (dou[(dou.GetLength(0) - 1), 0] * 1000000) / Main.Ion_velociy;
                                    fs.Points.Add(new DataPoint(_dou, dou[i, 0]));
                                    double[] bod = { _dou, dou[i, 0] };
                                    partial.Add(bod);
                                }
                            }
                            export.Add(partial);
                            fs.Title = str;
                            model.Series.Add(fs);
                        }
                        i++;
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
                else
                {
                    int i = 0;
                    PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
                    model.LegendTitle = "Legend";
                    model.LegendOrientation = LegendOrientation.Horizontal;
                    model.LegendPlacement = LegendPlacement.Inside;
                    model.LegendPosition = LegendPosition.RightTop;
                    foreach (string str in hlavicka)
                    {
                        if (checkedListBox1.CheckedItems.Contains(str))
                        {
                            titul.Add(str);
                            List<double[]> partial = new List<double[]>();
                            FunctionSeries fs = new FunctionSeries();
                            foreach (double[,] dou in tabulka)
                            {
                                if (Main.calc_type)
                                {
                                    double _dou = (dou[(dou.GetLength(0) - 1), 0] * Main.Ion_velociy) / 1000000;
                                    fs.Points.Add(new DataPoint(_dou, dou[i, 0]));
                                    double[] bod = { _dou, dou[i, 0] };
                                    partial.Add(bod);
                                }
                                else
                                {
                                    fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 1), 0], dou[i, 0]));
                                    double[] bod = { dou[(dou.GetLength(0) - 1), 0], dou[i, 0] };
                                    partial.Add(bod);
                                }
                            }
                            fs.Title = str;
                            export.Add(partial);
                            model.Series.Add(fs);
                        }
                        i++;
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
            else
            { // existuje moznoist menit v zavislosti od koncentracie
                if (Time)
                {                   
                    int i = 0;
                    PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
                    model.LegendTitle = "Legend";
                    model.LegendOrientation = LegendOrientation.Horizontal;
                    model.LegendPlacement = LegendPlacement.Inside;
                    model.LegendPosition = LegendPosition.RightTop;
                    foreach (string str in hlavicka_din)
                    {
                        if (checkedListBox1.CheckedItems.Contains(str))
                        {
                            titul.Add(str);
                            List<double[]> partial = new List<double[]>();
                            FunctionSeries fs = new FunctionSeries();
                            foreach (List<double[]> tabulka in data_din) // cez koncentracie
                            {
                                foreach (double[] dou in tabulka) // cez suradnicu
                                {
                                    if (dou[dou.GetLength(0) - 1] == _concentration) // set concentration
                                    {                                       
                                        if (Main.calc_type)
                                        {
                                            fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 2)], dou[i]));
                                            double[] bod = { dou[(dou.GetLength(0) - 2)], dou[i] };
                                            partial.Add(bod);
                                        }
                                        else
                                        {
                                            double time_dou = (dou[(dou.GetLength(0) - 2)] * 1000000) / Main.Ion_velociy;
                                            fs.Points.Add(new DataPoint(time_dou, dou[i]));
                                            double[] bod = { time_dou, dou[i] };
                                            partial.Add(bod);
                                        }
                                    }
                                }
                            }
                            fs.Title = str;
                            model.Series.Add(fs);
                            export.Add(partial);
                        }
                        i++;
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
                else
                {
                    int i = 0;
                    PlotModel model = new PlotModel() { LegendSymbolLength = 24, IsLegendVisible = true };
                    model.LegendTitle = "Legend";
                    model.LegendOrientation = LegendOrientation.Horizontal;
                    model.LegendPlacement = LegendPlacement.Inside;
                    model.LegendPosition = LegendPosition.RightTop;
                    foreach (string str in hlavicka_din)
                    {
                        if (checkedListBox1.CheckedItems.Contains(str))
                        {
                            titul.Add(str);
                            List<double[]> partial = new List<double[]>();
                            FunctionSeries fs = new FunctionSeries();
                            foreach (List<double[]> tabulka in data_din) // cez koncentracie
                            {
                                foreach (double[] dou in tabulka) // cez suradnicu
                                {
                                    if (dou[dou.GetLength(0) - 1] == _concentration) // set concentration
                                    {
                                        if (Main.calc_type)
                                        {
                                            double time_dou = (dou[(dou.GetLength(0) - 2)] * Main.Ion_velociy) / 1000000;
                                            fs.Points.Add(new DataPoint(time_dou, dou[i]));
                                            double[] bod = { time_dou, dou[i] };
                                            partial.Add(bod);
                                        }
                                        else
                                        {
                                            fs.Points.Add(new DataPoint(dou[(dou.GetLength(0) - 2)], dou[i]));
                                            double[] bod = { dou[(dou.GetLength(0) - 2)], dou[i] };
                                            partial.Add(bod);
                                        }
                                    }
                                }
                            }
                            fs.Title = str;
                            model.Series.Add(fs);
                            export.Add(partial);
                        }
                        i++;
                    }
                    model.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, Title = Popis_X, MajorGridlineStyle = LineStyle.Dash });

                    if (checkBox_log.Checked)
                    {
                        model.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
                    }
                    else
                    {
                        model.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 100, Title = "Concentration" });
                    } plot1.Model = model;
                }
            }
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Output";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                int pocet_itemov = titul.Count;
                List<double[]> _export = new List<double[]>();
                string[] legenda = new string[pocet_itemov + 1];
                if (dinamic_mode)
                {
                    legenda[0] = Popis_din;

                }
                else
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

        public void iniciate()
        {
            checkedListBox1.Items.Clear();
            if (radioButton_kinetic.Checked)
            {
                foreach (string str in hlavicka)
                {
                    checkedListBox1.Items.Add(str, true);
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
                            checkedListBox1.Items.Add(str, true);
                        }
                    }
                }
                checkedListBox1.Items.Add("Sum", true);
            }
            if (radioButton_dinamic.Checked)
            {
                foreach (string str in hlavicka_din)
                {
                    checkedListBox1.Items.Add(str, true);
                }
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

        private void radioButton_kinetic_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_kinetic.Checked)
            {
                button_ms.Enabled = false;
                textBox_dist.Enabled = false;
                trackBar_dist.Enabled = false;
                trackBar_dist.Value = 0;
                //label3.Text = "";
                //label4.Text = "";
                textBox_dist.Text = "";
                button_ms.Enabled = false;
                if (Main.results_din)
                {
                    textBox_conc.Enabled = true;
                    trackBar_conc.Enabled = true;
                    //label6.Text = Main.conc_start.ToString();
                    //label5.Text = Main.conc_end.ToString();
                    set_concentration(_concentration);
                }
                // draw kinetic
                Popis_X = "Time [us]";
                iniciate();
                start_animation();

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
                    //label3.Text = "0";
                    //label4.Text = Main.Time_duration.ToString();
                }
                else
                {
                    set_distanc(_distance);
                    //label3.Text = "0";
                    //label4.Text = Main.Distance.ToString();
                }
                button_ms.Enabled = true;
                if (Main.results_din)
                {
                    textBox_conc.Enabled = true;
                    trackBar_conc.Enabled = true;
                    //label6.Text = Main.conc_start.ToString();
                    //label5.Text = Main.conc_end.ToString();
                    set_concentration(_concentration);
                }
                // draw ms
                Popis_X = "m/z";
                iniciate();
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
                //label6.Text = "";
                //label5.Text = "";
                trackBar_conc.Enabled = false;
                trackBar_conc.Value = 0;
                textBox_conc.Text = "";
                button_ms.Enabled = false;
                if (is_time)
                {
                    set_time(_distance);
                    label3.Text = "0";
                    label4.Text = Main.Time_duration.ToString();
                }
                else
                {
                    set_distanc(_distance);
                    //label3.Text = "0";
                    //label4.Text = Main.Distance.ToString();
                }
                // draw din
                Popis_X = "Concentration [" + name_din  + "]";
                iniciate();
                start_animation();
            }
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
                label1.Text = "Distance [cm]:";
               // label3.Text = "0";
                //label4.Text = Main.Distance.ToString();
                set_distanc((time() * Main.Ion_velociy) / 1000000 );
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
                label1.Text = "Time [us]:";
                //label3.Text = "0";
                //label4.Text = Main.Time_duration.ToString();
                set_time((distance() * 1000000) / Main.Ion_velociy);
                if (!textBox_dist.Enabled)
                {
                    textBox_dist.Text = "";
                }
                button_t_x.Text = "t --> x";
            }
            start_animation();
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
                cnt = Math.Round(cnt, 10);
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
            start_animation();
        }

        private void trackBar_conc_Scroll(object sender, EventArgs e)
        {
            conc_roll = true;
            double actual = Main.conc_start + (trackBar_conc.Value * (Main.conc_end - Main.conc_start)) / (Main.conc_steps - 1);
            set_concentration(actual);
            conc_roll = false;
            start_animation();
        }

        private void textBox_conc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                set_concentration(Main.Convertor(textBox_conc.Text));
                start_animation();
            }
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

        private void textBox_conc_Leave(object sender, EventArgs e)
        {
            set_concentration(Main.Convertor(textBox_conc.Text));
        }

        private void trackBar_conc_Leave(object sender, EventArgs e)
        {
            start_animation();
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
                dou[0] = Math.Round(dou[0],2);
                if (dou[2] == 100)
                {
                    foreach (double[] d in isotopes_prop_list)
                    {
                        d[2] = Math.Round((100 * d[2]) /dou[1], 5);
                    }
                }
            }
            return isotopes_prop_list;
        }

        private void checkBox_log_CheckedChanged(object sender, EventArgs e)
        {
            start_animation();
        }
    
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button_ms_Click(object sender, EventArgs e)
        {
            if (convolution)
            {
                button_ms.Text = "C-OFF";
                convolution = false;
            }
            else
            {
                button_ms.Text = "C-ON";
                convolution = true;
            }
        }

        private List<double[]> Gauss_function(double Mass, double Sigma, double Intensity)
        {
            List<double[]> ResultsColection = new List<double[]>();
            int max_mass = Convert.ToInt32(Mass) + 10;
            double res_koef = Intensity / (Sigma * Math.PI * Math.Sqrt(2));
            for (int i = 0; i < (max_mass*100); i ++)
            {
                double x = i / 100;
                double res_exp = ((-1)*(Math.Pow((x - Mass),2))/(2*Sigma*Sigma));
                double res = res_koef*Math.Exp(res_exp);
                double[] dou = new double[2];
                dou[0] = x;
                dou[1] = res;
                ResultsColection.Add(dou);
            }
            return ResultsColection;
        }
    }
}