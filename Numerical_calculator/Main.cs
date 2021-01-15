using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;


namespace KIMI_Sim
{
    public partial class Main : Form
    {
        public int rad_zobrazenia = 2;
        public List<Item_box> ListOfItemBoxes;
        public Graphics g_stare;
        public static List<Items> ItemColection = new List<Items>();
        public static List<Reactions> RactionColection = new List<Reactions>();
        public List<Items_used> ItemColection_used = new List<Items_used>();
        public List<Reactions_used> ReactionColection_used = new List<Reactions_used>();
        public static Items carrier_gas_type = new Items();
        public int parameter_x = 0;
        public int parameter_y = 0;
        public int Actual_Center_x;
        public int Actual_Center_y;
        public int[,] Item_array;
        public int[] Name_array;
        public int curent_name = -1;
        public int sirka_bunky = 0;
        public bool mouse = false;
        public Bitmap buffer, backround, pure_backround;
        public System.Windows.Forms.Label new_label;
        public bool selected = false;
        public int selected_nmb = 0;
        public int selected_nmb_old = 0;
        public Point selected_coor;
        public Point selected_coor_new;
        public Point selected_coor_old;
        public bool draw_conection = false;
        public bool conection_posibility = false;
        public bool create_reaction = false;
        public int create_reaction_initial;
        public bool tahanie = false;
        public bool chyba_kliku = true;
        public bool constanta_live = false;
        public bool bola_zmena = false;
        public bool shit = false;
        public int draw_index = 0;
        public string s;
        public static double Distance; // cm
        public static double Ion_velociy; // cm*s(-1)
        public static double Time_duration;
        public static double NumberOfSteps;
        public static double Pressure; // Torr
        public static double Radius; // Torr
        public static double Gauss_density; // Torr
        public static bool Diffusion;
        public static bool Infinite_system;
        public Items reactant_name, product_name, neutral_A_name, neutral_B_name;
        public int reactant_nmb, product_nmb;
        public Reactions_used selected_reaction_used;

        //premenne bez el. pola
        public static List<Data_storage> data_storage = new List<Data_storage>();

        public static bool tvor_meno = true;
        public static bool calc_type = true;
        public static bool results_din = false;
        public static string name_din = "";

        public static double conc_start, conc_end, conc_steps;
        public static double field_start, field_end, field_steps;
        bool go_mouse = false;
        public static double Gauss_signa;

        public List<string[]> Experimental_input;
        public bool Experimental_input_status = false;
        public static double Temperature;

        public static rate_functions new_rate_function;
        public static rate_functions rate_function = new rate_functions();
        public static rate_functions _rate_function = new rate_functions();
        public static rate_functions item_mobility = new rate_functions();



        public Main()
        {
            InitializeComponent();
            //
            // Load Items from external file
            //
            try
            {
                load_settings();
            }
            catch
            {
                MessageBox.Show("Unable to load setting.");
            }
        }

        #region GUI interface

        public void panel1_refresh(object sender, PaintEventArgs e)
        {
            foreach (Label lab in panel1.Controls.OfType<Label>())
            {
                ///
                /// just to be sure !
                /// 
                lab.Enabled = false;
                lab.Visible = false;
                Point p = new Point(-1000, -1000);
                lab.Location = p;
                lab.Dispose();
            }
            panel1.Controls.Clear(); /// just to be sure !
            foreach (Label lab in panel1.Controls.OfType<Label>())
            {
                lab.Dispose();
            }
            buffer = new Bitmap(panel1.Width, panel1.Height);
            System.Drawing.Graphics graphics = Graphics.FromImage(buffer);
            graphics.Clear(Color.WhiteSmoke);

            Pen pen_dot = new Pen(Color.Blue, 2);
            int pocet_prvkov = 6 * rad_zobrazenia * rad_zobrazenia;
            int pocet_prvkov_x = 3 * rad_zobrazenia;
            int pocet_prvkov_y = 2 * rad_zobrazenia;
            int x_diff = panel1.Width / pocet_prvkov_x;
            sirka_bunky = x_diff;
            int y_diff = panel1.Height / pocet_prvkov_y;
            int cnt = 0, rel_x = 0, rel_y = 0, pos_x = 0, pos_y = 0, cent_x, cent_y;
            ListOfItemBoxes = new List<Item_box>();
            for (int i = 0; i < pocet_prvkov_x; i++)
            {
                rel_y = 0;
                pos_y = 0;
                for (int j = 0; j < pocet_prvkov_y; j++)
                {
                    cent_x = pos_x + (x_diff / 2);
                    cent_y = pos_y + (y_diff / 2);
                    ListOfItemBoxes.Add(new Item_box { Number = cnt, Relaive_x = rel_x, Relaive_y = rel_y,
                        Center_x = cent_x, Center_y = cent_y,
                        Dim_y = y_diff,
                        Dim_x = x_diff,
                        Position_x = pos_x, Position_y = pos_y });
                    cnt++;
                    rel_y++;
                    pos_y += y_diff;
                }
                rel_x++;
                pos_x += x_diff;
            }
            graphics.Dispose();
            System.Drawing.Graphics graphicsObj = e.Graphics;
            graphicsObj.Clear(Color.WhiteSmoke);
            graphicsObj.DrawImage(buffer, 0, 0, buffer.Width, buffer.Height);
            Create_array();
            backround = (Bitmap)buffer.Clone();
            pure_backround = (Bitmap)buffer.Clone();
            foreach (Items_used Item in ItemColection_used)
            {
                foreach (Item_box IB in ListOfItemBoxes)
                {
                    if ((Item.relative_x == IB.Relaive_x) && (Item.relative_y == IB.Relaive_y))
                    {
                        Item.box_number = IB.Number;
                        int limit = sirka_bunky - sirka_bunky / 4;
                        new_label = new System.Windows.Forms.Label();
                        new_label.AutoSize = true;
                        new_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                        new_label.Location = new System.Drawing.Point(IB.Center_x, IB.Center_y);
                        new_label.Name = "Name" + Item.box_number.ToString();
                        new_label.Size = new System.Drawing.Size(20, 30);
                        new_label.TabIndex = 7;
                        new_label.Text = "";
                        this.new_label.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
                        this.new_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
                        this.new_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
                        this.new_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);

                        Point point = new Point();
                        panel1.Controls.Add(new_label);
                        new_label.Text = "[" + Item.s_name + "]";
                        point.X = IB.Center_x - (new_label.Size.Width / 2);
                        point.Y = IB.Center_y - (new_label.Size.Height / 2);
                        new_label.Location = point;
                        if (new_label.Size.Width > limit)
                        {
                            float old = new_label.Font.Size;
                            while (new_label.Size.Width > limit)
                            {
                                new_label.Font = new System.Drawing.Font("Microsoft Sans Serif", old, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                                old--;
                            }
                            point = new Point();
                            panel1.Controls.Add(new_label);
                            new_label.Text = "[" + Item.s_name + "]";
                            point.X = IB.Center_x - (new_label.Size.Width / 2);
                            point.Y = IB.Center_y - (new_label.Size.Height / 2);
                            new_label.Location = point;
                        }
                    }
                }
            }
            foreach (Reactions_used Reactions in ReactionColection_used)
            {
                foreach (Item_box IB in ListOfItemBoxes)
                {
                    if ((Reactions.item_used_A.relative_x == IB.Relaive_x) && (Reactions.item_used_A.relative_y == IB.Relaive_y))
                    {
                        Reactions.pointA = new Point(IB.Center_x, IB.Center_y);
                    }
                    if ((Reactions.item_used_B.relative_x == IB.Relaive_x) && (Reactions.item_used_B.relative_y == IB.Relaive_y))
                    {
                        Reactions.pointB = new Point(IB.Center_x, IB.Center_y);
                    }
                }
            }
            draw_reactions();
            go_mouse = true;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (go_mouse)
            {
                Point cur_pos = panel1.PointToClient(Cursor.Position);
                mouse = true;
                // textBox_mobility.Text = constanta_live.ToString() +  "  " + tahanie.ToString();
                for (int i = 0; i < Item_array.GetLength(1); i++)
                {
                    if (cur_pos.X > Item_array[0, i] &&
                        cur_pos.X < Item_array[0, i] + Item_array[2, i] &&
                        cur_pos.Y > Item_array[1, i] &&
                        cur_pos.Y < Item_array[1, i] + Item_array[3, i])
                    {
                        if (curent_name != i)
                        {
                            System.Drawing.Graphics graphics = this.panel1.CreateGraphics();
                            graphics.DrawImage(buffer, 0, 0, buffer.Width, buffer.Height);
                            int point_x = Item_array[0, i] + Item_array[2, i] / 10;
                            int point_y = Item_array[1, i] + Item_array[3, i] / 10;
                            int delta_x = Item_array[2, i] - Item_array[2, i] / 5;
                            int delta_y = Item_array[3, i] - Item_array[3, i] / 5;
                            Pen pen = new Pen(Color.Blue, 1);
                            graphics.DrawRectangle(pen, point_x, point_y, delta_x, delta_y);
                            Pen pen2 = new Pen(Color.WhiteSmoke, 1);
                            graphics.DrawLine(pen2, point_x + delta_x / 3, point_y, point_x + delta_x - delta_x / 3, point_y);
                            graphics.DrawLine(pen2, point_x + delta_x / 3, point_y + delta_y, point_x + delta_x - delta_x / 3, point_y + delta_y);
                            graphics.DrawLine(pen2, point_x, point_y + delta_y / 3, point_x, point_y + delta_y - delta_y / 3);
                            graphics.DrawLine(pen2, point_x + delta_x, point_y + delta_y / 3, point_x + delta_x, point_y + delta_y - delta_y / 3);
                            curent_name = i;
                            if (draw_conection && conection_posibility)
                            {
                                // mis je dole, kresli spojnicu 
                                Pen pen3 = new Pen(Color.SkyBlue);
                                selected_coor_new = new Point(Item_array[4, i], Item_array[5, i]);
                                graphics.DrawLine(pen3, selected_coor, selected_coor_new);
                                create_reaction = true;
                                tahanie = true;
                            }
                            graphics.Dispose();
                        }
                    }
                }
            }
        }

        private void Test_Load(object sender, EventArgs e)
        {

            this.Paint += panel1_refresh;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            draw_conection = true;
            selected_nmb_old = selected_nmb;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (chyba_kliku)
            {
                selected_nmb = curent_name;
                create_reaction_initial = curent_name;
                if (!tahanie)
                {
                    foreach (Item_box IB in ListOfItemBoxes)
                    {
                        if (curent_name == IB.Number)
                        {
                            selected_coor_old = new Point(IB.Center_x, IB.Center_y);
                        }
                    }
                }
                buffer = (Bitmap)backround.Clone();
                textBox1.Text = "";
                textBox_Product.Text = "";
                listBox_products.Items.Clear();
                listBox_reactants.Items.Clear();
                _rate_function = null;
                rate_function = null;
                listBox_Direction.Text = "";
                textBox_rate.Text = "";
                textBox_rate_.Text = "";
                comboBox_reaction.Text = "";
                comboBox_reaction.Items.Clear();
                if (bola_zmena)
                {
                    draw_reactions();
                    bola_zmena = false;
                }
                constanta_live = false;
                konstanta_enable(false);
                neutrals_konstants(false);
                button_DeleteReaction.Enabled = false;
                if (ItemColection.Count == 0)
                {
                    new_item(selected_nmb);
                }
                else
                {
                    bool is_new_item = true;
                    foreach (Items_used I in ItemColection_used)
                    {
                        if (I.box_number == selected_nmb)
                        {
                            is_new_item = false;
                        }
                    }
                    if (is_new_item == true)
                    {
                        new_item(selected_nmb);
                    }
                    else
                    {
                        existing_item(selected_nmb);

                    }
                }
            }
            draw_conection = false;
            tahanie = false;
            if (create_reaction)
            {
                // allow reaction 
                constanta_live = true;
                konstanta_enable(true);
                neutrals_konstants(true);
                button_ClearReaction.Enabled = true;
                foreach (Items_used Item in ItemColection_used)
                {
                    if (selected_nmb_old == Item.box_number)
                    {
                        reactant_nmb = selected_nmb_old;
                        textBox1.Text = Item.s_name;
                        reactant_name = new Items();
                        reactant_name.Import_used(Item);
                    }
                    if (selected_nmb == Item.box_number)
                    {
                        product_nmb = selected_nmb;
                        textBox_Product.Text = Item.s_name;
                        product_name = new Items();
                        product_name.Import_used(Item);
                    }
                }
                bool okej = true;
                foreach (Reactions_used Reaction in ReactionColection_used)
                {
                    if (((Reaction.item_number_A == selected_nmb_old) && (Reaction.item_number_B == selected_nmb)) ||
                        ((Reaction.item_number_B == selected_nmb_old) && (Reaction.item_number_A == selected_nmb)))
                    {
                        // oznacena reakcia existuje, vyfarbi a 
                        Fill_Reaction_Boxes(Reaction);
                        if (Reaction.reaction_type == 2)
                        {
                            button_rate_E_.Enabled = true;
                        }
                        else
                        {
                            button_rate_E_.Enabled = false;
                        }
                        selected_reaction_used = Reaction;
                        reactant_nmb = Reaction.item_number_A;
                        product_nmb = Reaction.item_number_B;
                        button_DeleteReaction.Enabled = true;
                        draw_reactions();
                        bola_zmena = true;
                        constanta_live = false;
                        okej = false;
                        foreach (Item_box IB in ListOfItemBoxes)
                        {
                            if ((selected_coor.Y == IB.Center_y) && (selected_coor.X == IB.Center_x))
                            {
                                int point_x, point_y, delta_x, delta_y;
                                using (Graphics g = Graphics.FromImage(buffer))
                                {
                                    point_x = 2 + IB.Position_x + IB.Dim_x / 10;
                                    point_y = 2 + IB.Position_y + IB.Dim_y / 10;
                                    delta_x = IB.Dim_x - (IB.Dim_x / 5) - 4;
                                    delta_y = IB.Dim_y - (IB.Dim_y / 5) - 4;
                                    Pen pen = new Pen(Color.Red, 1);
                                    g.DrawRectangle(pen, point_x, point_y, delta_x, delta_y);
                                    g.Dispose();
                                }
                                System.Drawing.Graphics graphics = this.panel1.CreateGraphics();
                                graphics.DrawImage(buffer, 0, 0, buffer.Width, buffer.Height);
                                graphics.Dispose();
                            }
                        }
                    }
                }
                if (okej)
                {
                    comboBox_reaction.Items.Clear();
                    foreach (Reactions Reaction in RactionColection)
                    {
                        if ((compare(Reaction.item_A, reactant_name) && compare(Reaction.item_B, product_name)) ||
                            (compare(Reaction.item_B, reactant_name) && compare(Reaction.item_A, product_name)))
                        {
                            if (!comboBox_reaction.Items.Contains(Reaction))
                            {
                                comboBox_reaction.Items.Add(Reaction);
                            }
                        }
                    }
                }
            }
            try_enable_konstanta_add();
            create_reaction = false;
            chyba_kliku = true;
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            // ak nie je  nic, vytvor Item
            chyba_kliku = false;
            selected_nmb = curent_name;
            create_reaction_initial = curent_name;
            if (!tahanie)
            {
                foreach (Item_box IB in ListOfItemBoxes)
                {
                    if (curent_name == IB.Number)
                    {
                        selected_coor_old = new Point(IB.Center_x, IB.Center_y);
                    }
                }
            }
            buffer = (Bitmap)backround.Clone();
            textBox1.Text = "";
            textBox_Product.Text = "";
            listBox_products.Items.Clear();
            listBox_reactants.Items.Clear();
            listBox_Direction.SelectedIndex = -1;
            textBox_rate.Text = "";
            textBox_rate_.Text = "";
            comboBox_reaction.Text = "";
            comboBox_reaction.Items.Clear();
            selected_reaction_used = null;
            _rate_function = null;
            rate_function = null;
            if (bola_zmena)
            {
                draw_reactions();
                bola_zmena = false;
            }
            constanta_live = false;
            konstanta_enable(false);
            neutrals_konstants(false);
            button_DeleteReaction.Enabled = false;
            button_ClearReaction.Enabled = false;
            if (ItemColection.Count == 0)
            {
                new_item(selected_nmb);
            }
            else
            {
                bool is_new_item = true;
                foreach (Items_used I in ItemColection_used)
                {
                    if (I.box_number == selected_nmb)
                    {
                        is_new_item = false;
                    }
                }
                if (is_new_item == true)
                {
                    new_item(selected_nmb);
                }
                else
                {
                    existing_item(selected_nmb);

                }
            }
        }

        private void existing_item(int Name)
        {
            button_DeleteElement.Enabled = true;
            conection_posibility = true;
            comboBox_reaction.Items.Clear();
            foreach (Items_used I in ItemColection_used)
            {
                if (I.box_number == Name)
                {
                    Fill_Element_Boxes(I);
                    if (constanta_live)
                    {
                        textBox_Product.Text = I.s_name;
                    }
                }
            }
            if (!create_reaction)
            {
                bool su_reakcie = false;
                comboBox_reaction.Items.Clear();
                foreach (Reactions_used Reaction in ReactionColection_used)
                {
                    if ((Reaction.item_number_A == selected_nmb) || (Reaction.item_number_B == selected_nmb))
                    {
                        comboBox_reaction.Items.Add(Reaction);
                        su_reakcie = true;
                    }
                }
                if (su_reakcie)
                {
                    konstanta_enable(true);
                }
            }
            foreach (Item_box IB in ListOfItemBoxes)
            {
                if (Name == IB.Number)
                {
                    selected_coor = new Point();
                    selected_coor.Y = IB.Center_y;
                    selected_coor.X = IB.Center_x;
                    using (Graphics g = Graphics.FromImage(buffer))
                    {
                        int point_x, point_y, delta_x, delta_y;
                        point_x = 2 + IB.Position_x + IB.Dim_x / 10;
                        point_y = 2 + IB.Position_y + IB.Dim_y / 10;
                        delta_x = IB.Dim_x - (IB.Dim_x / 5) - 4;
                        delta_y = IB.Dim_y - (IB.Dim_y / 5) - 4;
                        Pen pen = new Pen(Color.Red, 1);
                        g.DrawRectangle(pen, point_x, point_y, delta_x, delta_y);
                        g.Dispose();
                        System.Drawing.Graphics graphics = this.panel1.CreateGraphics();
                        graphics.DrawImage(buffer, 0, 0, buffer.Width, buffer.Height);
                        point_x = IB.Position_x + IB.Dim_x / 10;
                        point_y = IB.Position_y + IB.Dim_y / 10;
                        delta_x = IB.Dim_x - (IB.Dim_x / 5);
                        delta_y = IB.Dim_y - (IB.Dim_y / 5);
                        Pen pen1 = new Pen(Color.Blue, 1);
                        graphics.DrawRectangle(pen1, point_x, point_y, delta_x, delta_y);
                        Pen pen2 = new Pen(Color.WhiteSmoke, 1);
                        Pen pen3 = new Pen(Color.SkyBlue);

                        graphics.DrawLine(pen2, point_x + delta_x / 3, point_y, point_x + delta_x - delta_x / 3, point_y);
                        graphics.DrawLine(pen2, point_x + delta_x / 3, point_y + delta_y, point_x + delta_x - delta_x / 3, point_y + delta_y);
                        graphics.DrawLine(pen2, point_x, point_y + delta_y / 3, point_x, point_y + delta_y - delta_y / 3);
                        graphics.DrawLine(pen2, point_x + delta_x, point_y + delta_y / 3, point_x + delta_x, point_y + delta_y - delta_y / 3);
                        if (create_reaction)
                        {   //draw line
                            graphics.DrawLine(pen3, selected_coor_old, selected_coor);
                        }
                        using (Graphics b = Graphics.FromImage(buffer))
                        {
                            b.DrawLine(pen3, selected_coor_old, selected_coor);
                            b.Dispose();
                        }
                        graphics.Dispose();
                    }
                }
            }
        }

        private void new_item(int Name)
        {
            button_DeleteElement.Enabled = false;
            conection_posibility = false;
            formula_enable(true);
            foreach (Item_box IB in ListOfItemBoxes)
            {
                if (IB.Number == Name) // podl mena najdem box
                {
                    selected_coor = new Point();
                    selected_coor.Y = IB.Center_y;
                    selected_coor.X = IB.Center_x;
                    comboBox_name.Text = "";
                    textBox_formula.Text = "";
                    textBox_shortname.Text = "";
                    textBox_concentration.Text = "";
                    textBox_mobility.Text = "";
                    item_mobility = null;
                    textBox_difusion.Text = "";
                    int point_x, point_y, delta_x, delta_y;
                    using (Graphics g = Graphics.FromImage(buffer))
                    {
                        point_x = 2 + IB.Position_x + IB.Dim_x / 10;
                        point_y = 2 + IB.Position_y + IB.Dim_y / 10;
                        delta_x = IB.Dim_x - (IB.Dim_x / 5) - 4;
                        delta_y = IB.Dim_y - (IB.Dim_y / 5) - 4;
                        Pen pen = new Pen(Color.Red, 1);
                        g.DrawRectangle(pen, point_x, point_y, delta_x, delta_y);
                        g.Dispose();
                    }
                    System.Drawing.Graphics graphics = this.panel1.CreateGraphics();
                    graphics.DrawImage(buffer, 0, 0, buffer.Width, buffer.Height);
                    point_x = IB.Position_x + IB.Dim_x / 10;
                    point_y = IB.Position_y + IB.Dim_y / 10;
                    delta_x = IB.Dim_x - (IB.Dim_x / 5);
                    delta_y = IB.Dim_y - (IB.Dim_y / 5);
                    Pen pen1 = new Pen(Color.Blue, 1);
                    graphics.DrawRectangle(pen1, point_x, point_y, delta_x, delta_y);
                    Pen pen2 = new Pen(Color.WhiteSmoke, 1);
                    Pen pen3 = new Pen(Color.SkyBlue);
                    graphics.DrawLine(pen2, point_x + delta_x / 3, point_y, point_x + delta_x - delta_x / 3, point_y);
                    graphics.DrawLine(pen2, point_x + delta_x / 3, point_y + delta_y, point_x + delta_x - delta_x / 3, point_y + delta_y);
                    graphics.DrawLine(pen2, point_x, point_y + delta_y / 3, point_x, point_y + delta_y - delta_y / 3);
                    graphics.DrawLine(pen2, point_x + delta_x, point_y + delta_y / 3, point_x + delta_x, point_y + delta_y - delta_y / 3);
                    if (create_reaction)
                    {   //draw line
                        graphics.DrawLine(pen3, selected_coor_old, selected_coor);
                    }
                    using (Graphics g = Graphics.FromImage(buffer))
                    {
                        g.DrawLine(pen3, selected_coor_old, selected_coor);
                        g.Dispose();
                    }
                    graphics.Dispose();
                }
            }
        }

        private void draw_reactions()
        {
            foreach (Reactions_used Reaction in ReactionColection_used)
            {
                try
                {
                    //kresli reakcnu plochu
                    int posun = 0;
                    foreach (Label lab in panel1.Controls.OfType<Label>())
                    {
                        if ((lab.Name == "Name" + Reaction.item_number_A.ToString()) || (lab.Name == "Name" + Reaction.item_number_B.ToString()))
                        {
                            posun += lab.Size.Width / 2;
                        }
                    }
                    int delta = Convert.ToInt32(Math.Sqrt(Math.Pow(Math.Abs(Reaction.pointA.X - Reaction.pointB.X), 2) + Math.Pow(Math.Abs(Reaction.pointA.Y - Reaction.pointB.Y), 2)));
                    int sirka = delta - posun - 4;
                    int vyska = (sirka_bunky) / 9;
                    double uhol = Math.Atan(Convert.ToDouble(Reaction.pointA.Y - Reaction.pointB.Y) / Convert.ToDouble(Reaction.pointA.X - Reaction.pointB.X));
                    uhol = 180 * uhol / Math.PI;
                    if (Reaction.pointA.X >= Reaction.pointB.X)
                    {
                        uhol += 180;
                    }
                    Matrix matrix = new Matrix();
                    PointF fix = new PointF((float)(Reaction.pointA.X + (Reaction.pointB.X - Reaction.pointA.X) / 2), (float)(Reaction.pointA.Y + (Reaction.pointB.Y - Reaction.pointA.Y) / 2));
                    matrix.RotateAt((float)uhol, fix);
                    using (Graphics b = Graphics.FromImage(backround))
                    {
                        b.Transform = matrix;
                        SolidBrush bursh = new SolidBrush(Color.Black);
                        if (Reaction == selected_reaction_used)
                        {
                            bursh = new SolidBrush(Color.SkyBlue);
                        }
                        Rectangle rec = new Rectangle();
                        FillMode newFillMode = FillMode.Winding;
                        PointF point1 = new PointF();
                        PointF point2 = new PointF();
                        PointF point3 = new PointF();
                        if (Reaction.reaction_type == 0 || Reaction.reaction_type == 2)
                        {   // podmienka smeru 1
                            point1 = new PointF(fix.X + (sirka / 2), fix.Y);
                            point2 = new PointF(fix.X + (sirka / 2) - 3 * vyska / 2, fix.Y - vyska / 2);
                            point3 = new PointF(fix.X + (sirka / 2) - 3 * vyska / 2, fix.Y + vyska / 2);
                            PointF[] curvePoints = { point1, point2, point3 };
                            b.FillPolygon(bursh, curvePoints, newFillMode);
                        }
                        else
                        {
                            b.FillRectangle(bursh, fix.X + (sirka / 2) - 3 * vyska / 2, (fix.Y - vyska / 2) + vyska / 3, 3 * vyska / 2, vyska / 3);
                        }
                        if (Reaction.reaction_type == 1 || Reaction.reaction_type == 2)
                        {   // podmienka smeru 1
                            point1 = new PointF(fix.X - (sirka / 2), fix.Y);
                            point2 = new PointF(fix.X - (sirka / 2) + 3 * vyska / 2, fix.Y - vyska / 2);
                            point3 = new PointF(fix.X - (sirka / 2) + 3 * vyska / 2, fix.Y + vyska / 2);
                            PointF[] curvePoints = { point1, point2, point3 };
                            b.FillPolygon(bursh, curvePoints, newFillMode);
                        }
                        else
                        {
                            b.FillRectangle(bursh, fix.X - (sirka / 2), (fix.Y - vyska / 2) + vyska / 3, 3 * vyska / 2, vyska / 3);
                        }
                        // centralne telo


                        rec.X = Convert.ToInt32(fix.X - (sirka / 2) + 3 * vyska / 2);
                        rec.Y = Convert.ToInt32((fix.Y - vyska / 2) + vyska / 3);
                        rec.Width = sirka - 3 * vyska;
                        rec.Height = vyska / 3;
                        b.FillRectangle(bursh, rec);
                        b.ResetTransform();
                        b.Dispose();
                    }
                }
                catch
                { }
            }
            buffer = (Bitmap)backround.Clone();
            System.Drawing.Graphics graphics = this.panel1.CreateGraphics();
            graphics.DrawImage(buffer, 0, 0, buffer.Width, buffer.Height);
            graphics.Dispose();
        }

        private void button_Proceed_Click(object sender, EventArgs e)
        {
            // start new calculation control
            Calculation_control recap = new Calculation_control();
            recap.ItemColection_used = ItemColection_used;
            recap.ReactionColection_used = ReactionColection_used;
            recap.Show(this);
            recap.show_values();
        }

        private void Create_array()
        {
            Item_array = new int[8, ListOfItemBoxes.Count];
            Name_array = new int[ListOfItemBoxes.Count];
            int i = 0;
            foreach (Item_box IB in ListOfItemBoxes)
            {
                Name_array[i] = IB.Number;
                Item_array[0, i] = IB.Position_x;
                Item_array[1, i] = IB.Position_y;
                Item_array[2, i] = IB.Dim_x;
                Item_array[3, i] = IB.Dim_y;
                Item_array[4, i] = IB.Center_x;
                Item_array[5, i] = IB.Center_y;
                i++;
            }
        }

        #endregion

        #region Element

        private void Fill_Element_Boxes(Items_used Item)
        {
            comboBox_name.Text = Item.name;
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
            textBox_difusion.Text = Item.diffusion.ToString();
            if (Item.cation)
            {
                radioButton_cation.Checked = true;
            }
            else
            {
                radioButton_neutral.Checked = true;
            }
        }


        private void button_ApplyElement_Click(object sender, EventArgs e)
        {
            bool dont_show_neutral = false;
            bool pokracuj_ = true;
            bool pokracuj = true;
            bool zmaz_povodne = false;
            bool zmaz_povodne_ = false;
            Items to_remove_ = new Items();
            Items_used to_remove = new Items_used(new Items(), false);
            Items new_item = new Items(comboBox_name.Text, textBox_formula.Text, textBox_shortname.Text, Convertor(textBox_concentration.Text), item_mobility, 0
                                       , Convertor(textBox_difusion.Text), radioButton_cation.Checked);
            MwtWinDll.MolecularWeightCalculator objMwtWin = new MwtWinDll.MolecularWeightCalculator();
            try
            {
                new_item.mass = objMwtWin.ComputeMass(Convert_formula(new_item.formula));
            }
            catch { MessageBox.Show("Unable to read formula and calculate mass!"); }
            if (radioButton_neutral.Checked)
            {
                new_item.mobility = new Constant(0.0, true);
                new_item.diffusion = 0.0;
            }
            if (constanta_live)
            {
                textBox_Product.Text = textBox_shortname.Text;
                product_name = new Items(new_item);
            }
            // zachovanie jedinecnosti mena - ak existuje item s rovnakym menom, bude prepisany
            if (!new_item.cation)
            {
                MessageBox.Show("Existing item is a neutral and thus will not be added to the reaction scheme. Neutrals can be founded in the Resource manager!", "Neutral detected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                zmaz_povodne_ = false;
                dont_show_neutral = true;
            }
            foreach (Items Item in ItemColection)
            {
                if (comboBox_name.Text == Item.name)
                {
                    if (Item.formula != textBox_formula.Text ||
                        Item.s_name != textBox_shortname.Text ||
                        Item.concentration != Convertor(textBox_concentration.Text) ||
                        !compare(Item.mobility, new_item.mobility) ||
                        Item.diffusion != Convertor(textBox_difusion.Text) ||
                        radioButton_cation.Checked != Item.cation
                        )
                    {
                        if (MessageBox.Show("Existing item will be rewriten! \nDo you want to continue?", "Name already exist.",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.No)
                        {
                            pokracuj_ = false;
                        }
                        else
                        {
                            to_remove = new Items_used();
                            zmaz_povodne_ = true;
                            to_remove_ = Item;
                        }
                    }
                }
            }
            if (zmaz_povodne_) // zhoda mena
            {
                ItemColection.Remove(to_remove_);
                // prejdi cez vsetky Item_used a rections_used a zmen meno na nove
                foreach (Items_used Item in ItemColection_used)
                {
                    if (Item.name == to_remove_.name)
                    {
                        Item.name = comboBox_name.Text;
                        Item.formula = textBox_formula.Text;
                        Item.s_name = textBox_shortname.Text;
                        Item.concentration = Convertor(textBox_concentration.Text);
                        Item.mobility = item_mobility;
                        Item.diffusion = Convertor(textBox_difusion.Text);
                        if (radioButton_cation.Checked == true)
                        { Item.cation = true; }
                        else { Item.cation = false; }
                        //
                        // treba zmenit aj vsetok label so starym menom 
                        //
                        foreach (Label lab in panel1.Controls.OfType<Label>())
                        {
                            if (lab.Name == "Name" + Item.box_number.ToString())
                            {
                                foreach (Item_box IB in ListOfItemBoxes)
                                {
                                    if (IB.Number == Item.box_number)
                                    {
                                        Point point = new Point();
                                        lab.Text = "[" + textBox_shortname.Text + "]";
                                        point.X = IB.Center_x - (new_label.Size.Width / 2);
                                        point.Y = IB.Center_y - (new_label.Size.Height / 2);
                                        new_label.Location = point;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (Reactions_used Reactions in ReactionColection_used)
                {
                    if (compare(Reactions.item_A, to_remove_))
                    {
                        Reactions.item_A = new_item;
                        Reactions.item_used_A.rewrite_Item(new_item);
                        // nove meno
                        string str = Reactions.name;
                        str.Replace(to_remove_.name, textBox_shortname.Text);
                        Reactions.name = str;

                    }
                    if (compare(Reactions.item_B, to_remove_))
                    {
                        Reactions.item_B = new_item;
                        Reactions.item_used_B.rewrite_Item(new_item);
                        // nove meno
                        string str = Reactions.name;
                        str.Replace(to_remove_.name, textBox_shortname.Text);
                        Reactions.name = str;
                    }

                }
            }
            if (pokracuj_)
            {
                bool jetu = false;
                foreach (Items Item in ItemColection)
                {
                    if (Item.name == comboBox_name.Text)
                    {
                        jetu = true;
                    }
                }
                if (!jetu)
                {
                    ItemColection.Add(new_item);
                    comboBox_name.Items.Add(new_item);
                }
                if (dont_show_neutral == false) // neutral nezobrazuj
                {
                    foreach (Items_used I in ItemColection_used)
                    {
                        if (selected_nmb == I.box_number)
                        {
                            if (MessageBox.Show("You are going to edit a used item. \nDo you want to continue?", "Item detected.",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == DialogResult.No)
                            {
                                pokracuj = false;
                            }
                            else
                            {
                                zmaz_povodne = true;
                                to_remove = I;
                            }
                        }
                    }
                    if (pokracuj == true) // rovnake cislo bunky
                    {
                        if (zmaz_povodne)
                        {
                            if (to_remove.name != null)
                            {
                                ItemColection_used.Remove(to_remove);
                            }
                            foreach (Label lab in panel1.Controls.OfType<Label>())
                            {
                                if (lab.Name == "Name" + selected_nmb.ToString())
                                {
                                    lab.Dispose();
                                }
                            }
                        }
                        Items_used new_Item_used = new Items_used(new_item, selected_nmb, 0, 0, false);
                        foreach (Item_box IB in ListOfItemBoxes)
                        {
                            if (IB.Number == selected_nmb)
                            {
                                new_Item_used.relative_x = IB.Relaive_x;
                                new_Item_used.relative_y = IB.Relaive_y;
                            }
                        }
                        ItemColection_used.Add(new_Item_used);
                        button_DeleteElement.Enabled = true;
                        ///
                        if (to_remove.name != null)
                        {
                            foreach (Reactions_used reactions in ReactionColection_used)
                            {
                                if ((reactions.item_number_A == to_remove.box_number) && (to_remove != new_Item_used))
                                {
                                    reactions.item_number_A = new_Item_used.box_number;
                                    reactions.item_A.Import_used(new_Item_used);
                                    reactions.item_used_A = new_Item_used;
                                    foreach (Item_box IB in ListOfItemBoxes)
                                    {
                                        if ((IB.Relaive_x == new_Item_used.relative_x) && (IB.Relaive_y == new_Item_used.relative_y))
                                        {
                                            reactions.pointA = new Point(IB.Center_x, IB.Center_y);
                                        }
                                    }
                                }
                                if ((reactions.item_number_B == to_remove.box_number) && (to_remove != new_Item_used))
                                {
                                    reactions.item_number_B = new_Item_used.box_number;
                                    reactions.item_B.Import_used(new_Item_used);
                                    reactions.item_used_B = new_Item_used;
                                    foreach (Item_box IB in ListOfItemBoxes)
                                    {
                                        if ((IB.Relaive_x == new_Item_used.relative_x) && (IB.Relaive_y == new_Item_used.relative_y))
                                        {
                                            reactions.pointB = new Point(IB.Center_x, IB.Center_y);
                                        }
                                    }
                                }
                            }
                        }
                        ///
                        try
                        {
                            foreach (Item_box IB in ListOfItemBoxes)
                            {
                                if (IB.Number == selected_nmb)
                                {
                                    // vytvor label
                                    int limit = sirka_bunky - sirka_bunky / 4;
                                    new_label = new System.Windows.Forms.Label();
                                    new_label.AutoSize = true;
                                    new_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                                    new_label.Location = new System.Drawing.Point(IB.Center_x, IB.Center_y);
                                    new_label.Name = "Name" + selected_nmb.ToString();
                                    new_label.Size = new System.Drawing.Size(20, 30);
                                    new_label.TabIndex = 7;
                                    new_label.Text = "";
                                    this.new_label.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
                                    this.new_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
                                    this.new_label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
                                    this.new_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);

                                    Point point = new Point();
                                    panel1.Controls.Add(new_label);
                                    new_label.Text = "[" + textBox_shortname.Text + "]";
                                    point.X = IB.Center_x - (new_label.Size.Width / 2);
                                    point.Y = IB.Center_y - (new_label.Size.Height / 2);
                                    new_label.Location = point;
                                    if (new_label.Size.Width > limit)
                                    {
                                        float old = new_label.Font.Size;
                                        while (new_label.Size.Width > limit)
                                        {
                                            new_label.Font = new System.Drawing.Font("Microsoft Sans Serif", old, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
                                            old--;
                                        }
                                        point = new Point();
                                        panel1.Controls.Add(new_label);
                                        new_label.Text = "[" + textBox_shortname.Text + "]";
                                        point.X = IB.Center_x - (new_label.Size.Width / 2);
                                        point.Y = IB.Center_y - (new_label.Size.Height / 2);
                                        new_label.Location = point;
                                    }
                                }
                            }
                        }
                        catch { MessageBox.Show("Shit happend ... "); }
                    }
                }
            }
            if ((groupBox2.Enabled == true) && (!constanta_live))
            {
                comboBox_reaction.Items.Clear();
                foreach (Reactions_used Reaction in ReactionColection_used)
                {
                    if ((Reaction.item_number_A == selected_nmb) || (Reaction.item_number_B == selected_nmb))
                    {
                        comboBox_reaction.Items.Add(Reaction);
                    }
                }
                textBox1.Text = "";
                textBox_Product.Text = "";
                listBox_products.Items.Clear();
                listBox_reactants.Items.Clear();
                listBox_Direction.Text = "";
                textBox_rate.Text = "";
                textBox_rate_.Text = "";
                comboBox_reaction.Text = "";
                button_DeleteReaction.Enabled = false;
            }
            if (constanta_live)
            {
                foreach (Reactions Reactions in RactionColection)
                {
                    if ((compare(Reactions.item_A, reactant_name) && compare(Reactions.item_B, product_name)) ||
                        (compare(Reactions.item_B, reactant_name) && compare(Reactions.item_A, product_name)))
                    {
                        if (!comboBox_reaction.Items.Contains(Reactions.name))
                        {
                            comboBox_reaction.Items.Add(Reactions.name);
                        }
                    }
                }
            }
            save_settings();
            try_enable_proceed();
        }

        private void button_DeleteElement_Click(object sender, EventArgs e)
        {
            bool reaction_del = false;
            List<Reactions_used> to_remove_r = new List<Reactions_used>();
            Items_used to_remove = new Items_used();
            if (groupBox2.Enabled == true)
            {
                groupBox2.Enabled = false;
                textBox1.Text = "";
                textBox_Product.Text = "";
                listBox_products.Items.Clear();
                listBox_reactants.Items.Clear();
                listBox_Direction.Text = "";
                textBox_rate.Text = "";
                textBox_rate_.Text = "";
                comboBox_reaction.Text = "";
                button_DeleteReaction.Enabled = false;
            }
            foreach (Items_used item in ItemColection_used)
            {
                if (compare(item.GetItem(), (Items)comboBox_name.SelectedItem) || (item.box_number == selected_nmb))
                {
                    to_remove = item;
                }
            }
            foreach (Reactions_used Reaction in ReactionColection_used)
            {
                if (compare(Reaction.item_used_A, to_remove) || compare(Reaction.item_used_B, to_remove))
                {
                    to_remove_r.Add(Reaction);
                    reaction_del = true;
                }
            }
            bool answer = true;
            if (reaction_del)
            {
                if (
                MessageBox.Show("Selected item is conected with a reactions. Affected rections will be deleted as well as selected item. \nDo you want to continue?", " Reaction detected",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.No)
                { answer = false; }
            }
            if (answer)
            {
                foreach (Reactions_used Reaction in to_remove_r)
                {
                    ReactionColection_used.Remove(Reaction);
                }
                ItemColection_used.Remove(to_remove);
                comboBox_name.Text = "";
                textBox_formula.Text = "";
                textBox_shortname.Text = "";
                textBox_concentration.Text = "";
                textBox_mobility.Text = "";
                item_mobility = null;
                textBox_difusion.Text = "";
                foreach (Label lab in panel1.Controls.OfType<Label>())
                {
                    if (lab.Name == "Name" + selected_nmb.ToString())
                    {
                        lab.Dispose();
                    }
                }
                if (reaction_del)
                {
                    textBox1.Text = "";
                    textBox_Product.Text = "";
                    listBox_products.Items.Clear();
                    listBox_reactants.Items.Clear();
                    listBox_Direction.Text = "";
                    textBox_rate.Text = "";
                    textBox_rate_.Text = "";
                    comboBox_reaction.Text = "";
                }
                backround = (Bitmap)pure_backround.Clone();
                draw_reactions();
            }
        }

        private void button_ClearElement_Click(object sender, EventArgs e)
        {
            comboBox_name.Text = "";
            textBox_formula.Text = "";
            textBox_shortname.Text = "";
            textBox_concentration.Text = "";
            textBox_mobility.Text = "";
            item_mobility = null;
            textBox_difusion.Text = "";
        }


        private void comboBox_name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button_ApplyElement_Click(sender, e);
            }
        }

        private void comboBox_name_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Items Item in ItemColection)
            {
                if (compare((Items)comboBox_name.SelectedItem, Item))
                {
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
                    textBox_difusion.Text = Item.diffusion.ToString();
                    if (Item.cation)
                    {
                        radioButton_cation.Checked = true;
                    }
                    else
                    {
                        radioButton_cation.Checked = false;
                    }
                }
            }
            try_enable_formula_add();
        }

        private void radioButton_cation_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_cation.Checked)
            {
                radioButton_neutral.Checked = false;
            }
            else
            {
                radioButton_neutral.Checked = true;
            }
        }

        private void radioButton_neutral_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_neutral.Checked)
            {
                radioButton_cation.Checked = false;
            }
            else
            {
                radioButton_cation.Checked = true;
            }
        }


        private void textBox_formula_TextChanged(object sender, EventArgs e)
        {
            try_enable_formula_add();
        }

        private void textBox_shortname_TextChanged(object sender, EventArgs e)
        {
            try_enable_formula_add();
        }

        private void textBox_concentration_TextChanged(object sender, EventArgs e)
        {
            try_enable_formula_add();
        }

        private void textBox_mobility_TextChanged(object sender, EventArgs e)
        {
            try_enable_formula_add();
        }

        private void textBox_mass_TextChanged(object sender, EventArgs e)
        {
            try_enable_formula_add();
        }

        private void textBox_difusion_TextChanged(object sender, EventArgs e)
        {
            try_enable_formula_add();
        }

        #endregion

        #region Reaction

        private void Fill_Reaction_Boxes(Reactions_used reaction)

        {
            tvor_meno = false;
            listBox_products.Items.Clear();
            listBox_reactants.Items.Clear();
            comboBox_reaction.Items.Clear();
            textBox_rate.Text = "";
            textBox_rate_.Text = "";

            textBox1.Text = reaction.item_used_A.s_name;
            textBox_Product.Text = reaction.item_used_B.s_name;
            if (reaction.neutrals_used_A != null && reaction.neutrals_used_A.Count > 0)
            {
                foreach (Items_used item in reaction.neutrals_used_A)
                {
                    listBox_reactants.Items.Add(item);
                }
            }
            if (reaction.neutrals_used_B != null && reaction.neutrals_used_B.Count > 0)
            {
                foreach (Items_used item in reaction.neutrals_used_B)
                {
                    listBox_products.Items.Add(item);
                }
            }
            listBox_Direction.SelectedIndex = reaction.reaction_type;
            try
            {
                textBox_rate.Text = reaction.rate_konstant.representation;
                rate_function = reaction.rate_konstant;
            }
            catch { }
            try
            {
                textBox_rate_.Text = reaction.rate_konstant_.representation;
                _rate_function = reaction.rate_konstant_;
            }
            catch { }
            foreach (Reactions Reaction in RactionColection)
            {
                if ((compare(Reaction.item_A, reactant_name) && compare(Reaction.item_B, product_name)) ||
                    (compare(Reaction.item_B, reactant_name) && compare(Reaction.item_A, product_name)))
                {
                    if (!comboBox_reaction.Items.Contains(Reaction))
                    {
                        comboBox_reaction.Items.Add(Reaction);
                    }
                }
            }
            comboBox_reaction.Text = reaction.name;
            listbox_change();
            tvor_meno = true;

        }

        private void Fill_Reaction_Boxes(Reactions reaction)
        {
            tvor_meno = false;
            listBox_products.Items.Clear();
            listBox_reactants.Items.Clear();
            textBox_rate.Text = "";
            textBox_rate_.Text = "";

            textBox1.Text = reaction.item_A.s_name;
            textBox_Product.Text = reaction.item_B.s_name;
            if (reaction.neutrals_A != null && reaction.neutrals_A.Count > 0)
            {
                foreach (Items item in reaction.neutrals_A)
                {
                    listBox_reactants.Items.Add(item);
                }
            }
            if (reaction.neutrals_B != null && reaction.neutrals_B.Count > 0)
            {
                foreach (Items item in reaction.neutrals_B)
                {
                    listBox_products.Items.Add(item);
                }
            }
            listBox_Direction.SelectedIndex = reaction.reaction_type;
            try
            {
                textBox_rate.Text = reaction.rate_konstant.representation;
                rate_function = reaction.rate_konstant;
            }
            catch { }
            try
            {
                textBox_rate_.Text = reaction.rate_konstant_.representation;
                _rate_function = reaction.rate_konstant_;
            }
            catch { }
            comboBox_reaction.Text = reaction.name;
            try_enable_konstanta_add();
            tvor_meno = true;
        }

        private void button_ApplyReaction_Click(object sender, EventArgs e)
        {
            bola_zmena = true;
            bool pokracuj = true;
            bool zmaz_povodne = false;
            bool zmaz_povodne_ = false;
            Reactions_used to_remove = new Reactions_used();
            Reactions to_remove_ = new Reactions();
            Reactions new_reaction = new Reactions();
            bool neutral_missing = false;
            // neutrals A control - control if neutrals are in Items_used
            try
            {
                foreach (Items item in listBox_reactants.Items)
                {
                    bool A_isinUsed = false;
                    foreach (Items_used Items in ItemColection_used)
                    {
                        if (compare(item, Items) && item != null)
                        {
                            A_isinUsed = true;
                        }
                    }
                    if (!A_isinUsed)
                    {
                        bool A_sanasiel = false;
                        Items_used Item = new Items_used();
                        foreach (Items IT in ItemColection)
                        {
                            if (compare(item, IT))
                            {
                                Item = new Items_used(IT, -1, -1, -1, false);
                                A_sanasiel = true;
                                ItemColection_used.Add(Item);
                            }
                        }
                        if (!A_sanasiel) { MessageBox.Show("Neutral " + item.ToString() + " isn't in database! "); neutral_missing = true; }
                    }
                }
            }
            catch { }
            // neutrals B control - control if neutrals are in Items_used    
            try
            {
                foreach (Items item in listBox_products.Items)
                {
                    bool B_isinUsed = false;
                    foreach (Items_used Items in ItemColection_used)
                    {
                        if (compare(item, Items) && item != null)
                        {
                            B_isinUsed = true;
                        }
                    }
                    if (!B_isinUsed)
                    {
                        bool B_sanasiel = false;
                        Items_used Item = new Items_used();
                        foreach (Items IT in ItemColection)
                        {
                            if (compare(item, IT))
                            {
                                Item = new Items_used(IT, -1, -1, -1, false);
                                if (IT.cation == true)
                                {
                                    MessageBox.Show("Shit is real ! ");
                                }
                                B_sanasiel = true;
                                ItemColection_used.Add(Item);
                            }
                        }
                        if (!B_sanasiel) { MessageBox.Show("Neutral " + item.ToString() + " isn't in database! "); neutral_missing = true; }
                    }
                }
            }
            catch { }
            if (!neutral_missing)
            { // no problems with neutrals, continue
                try
                { // form a new reaction_used according to texboxes
                    List<Items> neutral_reactants = new List<Items>();
                    List<Items> neutral_products = new List<Items>();
                    if (listBox_reactants.Items != null && listBox_reactants.Items.Count > 0)
                    {
                        foreach (Items IT in listBox_reactants.Items)
                        {
                            neutral_reactants.Add(IT);
                        }
                    }
                    if (listBox_products.Items != null && listBox_products.Items.Count > 0)
                    {
                        foreach (Items IT in listBox_products.Items)
                        {
                            neutral_products.Add(IT);
                        }
                    }
                    new_reaction = new Reactions(comboBox_reaction.Text, rate_function, _rate_function, listBox_Direction.SelectedIndex,
                                                reactant_name, product_name, neutral_reactants, neutral_products);
                }
                catch
                { MessageBox.Show("Unable to create a new reaction."); }

                foreach (Reactions Reaction in RactionColection)
                { // control if reaction is presented in Reaction collection
                    if ((new_reaction.name == Reaction.name) && (Reaction.rate_konstant_ != null) && (new_reaction.rate_konstant_ != null)) // obojsmerna
                    { // if reaction exist with different settings,it will be rewriten
                        if (!compare(Reaction.item_A, new_reaction.item_A) ||
                            !compare(Reaction.item_B, new_reaction.item_B) ||
                            !compare(Reaction.neutrals_A, new_reaction.neutrals_A) ||
                            !compare(Reaction.neutrals_B, new_reaction.neutrals_B) ||
                            Reaction.rate_konstant.representation != new_reaction.rate_konstant.representation ||
                            Reaction.rate_konstant_.representation != new_reaction.rate_konstant_.representation ||
                            Reaction.reaction_type != new_reaction.reaction_type
                            )
                        {
                            if (MessageBox.Show("This reaction allready exist in the database, but with different settings. \nThe former reaction will be rewriten! \nDo you want to continue?", "Name already exist.",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == DialogResult.No)
                            {
                                pokracuj = false;
                            }
                            else
                            {
                                to_remove_ = new Reactions();
                                zmaz_povodne_ = true;
                                to_remove_ = Reaction;
                            }
                        }
                    }
                    else
                    {
                        if ((new_reaction.name == Reaction.name) && (Reaction.rate_konstant_ == null) && (new_reaction.rate_konstant_ != null)) //jednosmerna
                        {
                            if (!compare(Reaction.item_A, new_reaction.item_A) ||
                                !compare(Reaction.item_B, new_reaction.item_B) ||
                                !compare(Reaction.neutrals_A, new_reaction.neutrals_A) ||
                                !compare(Reaction.neutrals_B, new_reaction.neutrals_B) ||
                                Reaction.rate_konstant.representation != new_reaction.rate_konstant.representation ||
                                Reaction.reaction_type != new_reaction.reaction_type
                                )
                            {
                                if (MessageBox.Show("This reaction allready exist in the database, but with different settings. \nThe former reaction will be rewriten! \nDo you want to continue?", "Name already exist.",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                    == DialogResult.No)
                                {
                                    pokracuj = false;
                                }
                                else
                                {
                                    to_remove_ = new Reactions();
                                    zmaz_povodne_ = true;
                                    to_remove_ = Reaction;
                                }
                            }
                        }
                        else
                        {
                            if ((new_reaction.name == Reaction.name) && (((Reaction.rate_konstant_ == null) && (new_reaction.rate_konstant_ != null)) || ((Reaction.rate_konstant_ != null) && (new_reaction.rate_konstant_ == null))))
                            {
                                if (MessageBox.Show("This reaction allready exist in the database, but with different settings. \nThe former reaction will be rewriten! \nDo you want to continue?", "Name already exist.",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                    == DialogResult.No)
                                {
                                    pokracuj = false;
                                }
                                else
                                {
                                    to_remove_ = new Reactions();
                                    zmaz_povodne_ = true;
                                    to_remove_ = Reaction;
                                }
                            }
                        }
                    }
                }
                foreach (Reactions_used Reaction_ in ReactionColection_used)
                { // aj je v ReactionCollection, ci nieje nahodou uz aj v ReactionsUsed
                    if (((Reaction_.item_number_A == selected_nmb_old) && (Reaction_.item_number_B == selected_nmb)) ||
                        ((Reaction_.item_number_A == reactant_nmb) && (Reaction_.item_number_B == product_nmb)) ||
                        ((Reaction_.item_number_A == product_nmb) && (Reaction_.item_number_B == reactant_nmb)))
                    {
                        // ak je uz pouzity, ci to myslim vazne
                        if (MessageBox.Show("Reaction allready exist between selected items. \nThe former reaction will be rewriten. \nDo you want to continue?", "Reaction already exist.",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.No)
                        {
                            pokracuj = false;
                        }
                        else
                        {
                            to_remove = new Reactions_used();
                            to_remove = Reaction_;
                            zmaz_povodne = true;
                        }
                    }
                }
                if (pokracuj) // if OK so far
                {
                    if (zmaz_povodne_) // zmazanie reakcie
                    {
                        //zmaz povodnu reakciu
                        RactionColection.Remove(to_remove_);
                    }
                    if (zmaz_povodne)
                    {
                        // zmaz prvky predchadzajucej reakcie z panel1
                        ReactionColection_used.Remove(to_remove);
                        backround = (Bitmap)pure_backround.Clone();
                    }
                    // reakcia je nova oproti databaze alebo as tam uz nachadza
                    // pridaj
                    bool jetu = false;
                    foreach (Reactions Reaction in RactionColection)
                    {
                        if (compare(Reaction, new_reaction))
                        {
                            jetu = true;
                        }
                    }
                    if (!jetu)
                    { // pridaj do ReactionCollection ak sa nenachadza
                        RactionColection.Add(new_reaction);
                        comboBox_reaction.Items.Add(new_reaction);
                    }
                    // we need to add a new reaction_used
                    Items_used It_A = new Items_used();
                    Items_used It_B = new Items_used();
                    List<Items_used> List_NA = new List<Items_used>();
                    List<Items_used> List_NB = new List<Items_used>();
                    foreach (Items_used item in ItemColection_used)
                    {
                        if (compare(item.GetItem(), reactant_name) && (item.box_number == selected_nmb_old))
                        {
                            It_A = item;
                        }
                        if (compare(item.GetItem(), product_name) && (item.box_number == selected_nmb))
                        {
                            It_B = item;
                        }
                        foreach (Items IT in listBox_reactants.Items)
                        {
                            if (compare(item.GetItem(), IT))
                            {
                                List_NA.Add(item);
                            }
                        }
                        foreach (Items IT in listBox_products.Items)
                        {
                            if (compare(item.GetItem(), IT))
                            {
                                List_NB.Add(item);
                            }
                        }
                    }
                    Reactions_used new_reaction_used = new Reactions_used(new_reaction, selected_coor_old, selected_nmb_old, selected_coor_new, selected_nmb,
                        It_A, It_B, List_NA, List_NB);
                    ReactionColection_used.Add(new_reaction_used);
                    button_DeleteReaction.Enabled = true;
                    //new reaction was added. now it has to be drawed

                    // Ostava oznacena alebo nie ? 
                    // Teraz je ze nie!
                    selected_coor_old = new Point();
                    selected_coor_new = new Point();
                    try
                    {
                        //kresli reakcnu plochu
                        draw_reactions();
                        foreach (Item_box IB in ListOfItemBoxes)
                        {
                            if ((selected_coor.Y == IB.Center_y) && (selected_coor.X == IB.Center_x))
                            {
                                int point_x, point_y, delta_x, delta_y;
                                using (Graphics g = Graphics.FromImage(buffer))
                                {
                                    point_x = 2 + IB.Position_x + IB.Dim_x / 10;
                                    point_y = 2 + IB.Position_y + IB.Dim_y / 10;
                                    delta_x = IB.Dim_x - (IB.Dim_x / 5) - 4;
                                    delta_y = IB.Dim_y - (IB.Dim_y / 5) - 4;
                                    Pen pen = new Pen(Color.Red, 1);
                                    g.DrawRectangle(pen, point_x, point_y, delta_x, delta_y);
                                    g.Dispose();
                                }
                                System.Drawing.Graphics graphics = this.panel1.CreateGraphics();
                                graphics.DrawImage(buffer, 0, 0, buffer.Width, buffer.Height);
                                graphics.Dispose();
                            }
                        }
                    }
                    catch { MessageBox.Show("Shit is real ! Reaction was not drawed."); }
                    //clear ractions 
                    textBox1.Text = "";
                    textBox_Product.Text = "";
                    listBox_products.Items.Clear();
                    listBox_reactants.Items.Clear();
                    listBox_Direction.SelectedIndex = -1;
                    textBox_rate.Text = "";
                    textBox_rate_.Text = "";
                    comboBox_reaction.Text = "";
                    comboBox_reaction.Items.Clear();
                    constanta_live = false;
                    konstanta_enable(false);
                    neutrals_konstants(false);
                    button_DeleteReaction.Enabled = false;

                    save_settings();
                }
            }
            try_enable_proceed();
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

        private void button_DeleteReaction_Click(object sender, EventArgs e)
        {
            Reactions_used to_remove = new Reactions_used();
            try
            {
                foreach (Reactions_used reaction in ReactionColection_used)
                {
                    if (reaction == selected_reaction_used)
                    {
                        to_remove = reaction;
                    }
                }
                ReactionColection_used.Remove(to_remove);
                textBox1.Text = "";
                textBox_Product.Text = "";
                listBox_products.Items.Clear();
                listBox_reactants.Items.Clear();
                listBox_Direction.Text = "";
                textBox_rate.Text = "";
                textBox_rate_.Text = "";
                comboBox_reaction.Text = "";
                comboBox_reaction.Items.Clear();
                backround = (Bitmap)pure_backround.Clone();
                draw_reactions();
                button_DeleteReaction.Enabled = false;
                neutrals_konstants(false);
            }
            catch { MessageBox.Show("A problem has been detected! Reaction was not deleted."); }
        }

        private void button_ClearReaction_Click(object sender, EventArgs e)
        {
            listBox_products.Items.Clear();
            listBox_reactants.Items.Clear();
            listBox_Direction.Text = "";
            textBox_rate.Text = "";
            textBox_rate_.Text = "";
            button_DeleteReaction.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Results_KineticModel Kinetick_results = new Results_KineticModel();
            Kinetick_results.Show();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //add reactant neutral
            AddNeutral addneutral = new AddNeutral();
            addneutral.Items = ItemColection;
            addneutral.Data_type = false;
            addneutral.Remove_data = false;
            addneutral.to_SRI_listbox = false;
            addneutral.ShowCollection();
            addneutral.Show(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //add product neutral
            AddNeutral addneutral = new AddNeutral();
            addneutral.Items = ItemColection;
            addneutral.Data_type = true;
            addneutral.Remove_data = false;
            addneutral.to_SRI_listbox = false;
            addneutral.ShowCollection();
            addneutral.Show(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox_reactants.SelectedItem != null)
            {
                Items ToBeRemoved = null;
                foreach (Items item in listBox_reactants.Items)
                {
                    if (listBox_reactants.SelectedItem == item)
                    {
                        ToBeRemoved = item;
                    }
                }
                listBox_reactants.Items.Remove(ToBeRemoved);
            }
            else
            {
                AddNeutral addneutral = new AddNeutral();
                List<Items> Neutrals_list = new List<Items>();
                foreach (Items item in listBox_reactants.Items)
                {
                    Neutrals_list.Add(item);
                }
                addneutral.Items = Neutrals_list;
                addneutral.Data_type = false;
                addneutral.Remove_data = true;
                addneutral.to_SRI_listbox = false;
                addneutral.ShowCollection();
                addneutral.Show(this);
            }
            listbox_change();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox_products.SelectedItem != null)
            {
                Items ToBeRemoved = null;
                foreach (Items item in listBox_products.Items)
                {
                    if (listBox_products.SelectedItem == item)
                    {
                        ToBeRemoved = item;
                    }
                }
                listBox_products.Items.Remove(ToBeRemoved);
            }
            else
            {
                AddNeutral addneutral = new AddNeutral();
                List<Items> Neutrals_list = new List<Items>();
                foreach (Items item in listBox_products.Items)
                {
                    Neutrals_list.Add(item);
                }
                addneutral.Items = Neutrals_list;
                addneutral.Data_type = true;
                addneutral.Remove_data = true;
                addneutral.to_SRI_listbox = false;
                addneutral.ShowCollection();
                addneutral.Show(this);
            }
            listbox_change();
        }

        private void button_rate_E_Click(object sender, EventArgs e)
        {
            Rate_Konstatn_TypeOf new_rate_typeof = new Rate_Konstatn_TypeOf();
            new_rate_typeof.Show(this);
            new_rate_typeof.proceed(rate_function, false, false, false);
        }

        private void button_rate_E__Click(object sender, EventArgs e)
        {
            Rate_Konstatn_TypeOf new_rate_typeof = new Rate_Konstatn_TypeOf();
            new_rate_typeof.Show(this);
            new_rate_typeof.proceed(_rate_function, true, false, false);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Rate_Konstatn_TypeOf new_rate_typeof = new Rate_Konstatn_TypeOf();
            new_rate_typeof.Show(this);
            new_rate_typeof.proceed(item_mobility, true, true, false);
        }

        private void comboBox_reaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (constanta_live)
            { // zvolenie itemu po tahani a mouseUp
                try
                {
                    neutrals_konstants(true);
                    Fill_Reaction_Boxes((Reactions)comboBox_reaction.SelectedItem);
                    button_DeleteReaction.Enabled = true;
                }
                catch { }
            }
            else
            { // zvolenie reakcie po kliku na Item
                foreach (Reactions_used Reaction in ReactionColection_used)
                {
                    if (Reaction == (Reactions)comboBox_reaction.SelectedItem)
                    {
                        neutrals_konstants(true);
                        Fill_Reaction_Boxes(Reaction);
                        reactant_nmb = Reaction.item_number_A;
                        product_nmb = Reaction.item_number_B;
                        button_DeleteReaction.Enabled = true;
                        button_ClearReaction.Enabled = true;
                        selected_reaction_used = Reaction;
                    }
                }
                draw_reactions();
                shit = false;
                foreach (Item_box IB in ListOfItemBoxes)
                {
                    if ((selected_coor.Y == IB.Center_y) && (selected_coor.X == IB.Center_x))
                    {
                        int point_x, point_y, delta_x, delta_y;
                        using (Graphics g = Graphics.FromImage(buffer))
                        {
                            point_x = 2 + IB.Position_x + IB.Dim_x / 10;
                            point_y = 2 + IB.Position_y + IB.Dim_y / 10;
                            delta_x = IB.Dim_x - (IB.Dim_x / 5) - 4;
                            delta_y = IB.Dim_y - (IB.Dim_y / 5) - 4;
                            Pen pen = new Pen(Color.Red, 1);
                            g.DrawRectangle(pen, point_x, point_y, delta_x, delta_y);
                            g.Dispose();
                        }
                        System.Drawing.Graphics graphics = this.panel1.CreateGraphics();
                        graphics.DrawImage(buffer, 0, 0, buffer.Width, buffer.Height);
                        graphics.Dispose();
                    }
                }
            }
            bola_zmena = true;
        }

        private void listBox_Direction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Direction.SelectedIndex == 0)
            {
                button_rate_E_.Enabled = false;
                button_rate_E.Enabled = true;
                _rate_function = null;
                textBox_rate_.Text = "";
            }
            if (listBox_Direction.SelectedIndex == 1)
            {
                button_rate_E_.Enabled = false;
                button_rate_E.Enabled = true;
                _rate_function = null;
                textBox_rate_.Text = "";
            }
            if (listBox_Direction.SelectedIndex == 2)
            {
                button_rate_E_.Enabled = true;
                button_rate_E.Enabled = true;
            }
            try_enable_konstanta_add();
        }

        private void textBox_Reactant_TextChanged(object sender, EventArgs e)
        {
            try_enable_konstanta_add();
        }

        private void textBox_Product_TextChanged(object sender, EventArgs e)
        {
            try_enable_konstanta_add();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try_enable_konstanta_add();
        }

        private void textBox_rate_TextChanged(object sender, EventArgs e)
        {
            try_enable_konstanta_add();
        }

        private void comboBox_reaction_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button_ApplyReaction_Click(sender, e);
            }
        }

        #endregion

        #region Menu

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show(this);
        }

        private void calculationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties_form properties_form = new Properties_form();
            properties_form.Show(this);
        }

        private void neutralsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Resources resources = new Resources();
            resources.ItemColection = ItemColection;
            resources.Show(this);
            resources.proceed();
        }

        private void saveSchemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save schema";
            saveFileDialog1.FileName = "Schema.txt";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            string save = "";
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, ItemColection_used);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                save += Convert.ToBase64String(buffer);
            }
            save += "\t";
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, ReactionColection_used);
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

        private void loadSchemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Load schema";
            openFileDialog1.FileName = "Schema.txt";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
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
                    string Item_str = "", Reaction_str = "";
                    string data = "";
                    foreach (char ch in load)
                    {
                        if (ch == '\t')
                        {
                            Item_str = data;
                            data = "";
                        }
                        else
                        {
                            data += ch.ToString();
                        }
                    }
                    Reaction_str = data;
                    try
                    {
                        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Item_str)))
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            ItemColection_used.Clear();
                            ItemColection_used = (List<Items_used>)bf.Deserialize(ms);
                        }
                        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Reaction_str)))
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            ReactionColection_used.Clear();
                            ReactionColection_used = (List<Reactions_used>)bf.Deserialize(ms);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Unable to load schema.");
                    }
                    this.Invalidate();
                    draw_reactions();
                    List<Items> doplnok = new List<Items>();
                    foreach (Items_used UItems in ItemColection_used)
                    {
                        bool ok = false;
                        foreach (Items Items in ItemColection)
                        {
                            if (Items.name == UItems.name)
                            {
                                Items.cation = UItems.cation;
                                Items.concentration = UItems.concentration;
                                Items.diffusion = UItems.diffusion;
                                Items.formula = UItems.formula;
                                Items.group_ID = UItems.group_ID;
                                Items.mass = UItems.mass;
                                Items.mobility = UItems.mobility;
                                Items.s_name = UItems.s_name;
                            }
                            if (compare(Items, UItems.GetItem())) // compare if the two items share all properties
                            {
                                ok = true;
                            }
                        }
                        if (!ok) // if dont, chose if create a new one
                        {
                            Items new_item = new Items(UItems.GetItem());
                            doplnok.Add(new_item);
                        }
                    }
                    foreach (Items Item in doplnok)
                    {
                        ItemColection.Add(Item);
                    }
                    List<Reactions> dodatok = new List<Reactions>();
                    foreach (Reactions_used UReactions in ReactionColection_used)
                    {
                        bool OK = false;
                        foreach (Reactions Reaction in RactionColection)
                        {
                            if (compare(Reaction, UReactions))  // compare if the two reactions share all properties
                            {
                                OK = true;
                            }
                        }
                        if (!OK) // if dont, chose if create a new one
                        {
                            Reactions new_reaction = new Reactions(UReactions.GetReaction());
                            dodatok.Add(new_reaction);
                        }
                    }
                    foreach (Reactions Reaction in dodatok)
                    {
                        RactionColection.Add(Reaction);
                    }

                }
                try_enable_proceed();
                Cursor.Current = Cursors.Default;
            }
        }
    

        private void exportResourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Export resources";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FileName = "Data_resources.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (!saveFileDialog1.Title.Contains(".txt"))
            { saveFileDialog1.Title += ".txt"; }
            string export = "";
            foreach (Items Item in ItemColection)
            {
                string str = "I\t" + Item.name + "\t" + Item.formula + "\t" + Item.s_name + "\t" + Item.concentration.ToString() + "\t" + Item.mobility.ToString() + "\t" + Item.mobility.representation + "\t" + Item.mass.ToString() + "\t"
                    + Item.diffusion.ToString() + "\t" + Item.cation.ToString() + "\t";
                str += "\r\n";
                export += str;
            }
            foreach (Reactions Reaction in RactionColection)
            {
                string str = "R\t" + Reaction.name + "\t" + Reaction.rate_konstant.ToString() + "\t" + Reaction.rate_konstant.representation + "\t";
                if (Reaction.rate_konstant_ != null)
                {
                    str += Reaction.rate_konstant_.ToString() + "\t" + Reaction.rate_konstant_.representation + "\t";
                }
                else
                {
                    str += "$" + "\t" + "$" + "\t";
                }
                str += Reaction.reaction_type.ToString() + "\t" + Reaction.item_A.name + "\t" + Reaction.item_B.name + "\t";
                if (Reaction.neutrals_A != null && Reaction.neutrals_A.Count > 0)
                {
                    foreach (Items item in Reaction.neutrals_A)
                    {
                        str += item.name + "\t";
                    }
                    str += "@\t";
                }
                else
                {
                    str += "@\t"; // A and B neutrals separator
                }
                if (Reaction.neutrals_B != null && Reaction.neutrals_B.Count > 0)
                {
                    foreach (Items item in Reaction.neutrals_A)
                    {
                        str += item.name + "\t";
                    }
                    str += "\t";
                }
                else
                {
                    str += "\t";
                }
                str += "\r\n";
                export += str;
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

        private void loadResourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clear the list of Items and Reactions and replase them with the new one 
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            string import;
            Items Item = new Items();
            Reactions Reaction = new Reactions();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != "")
                {
                    Cursor.Current = Cursors.WaitCursor;
                    StreamReader read = new StreamReader(openFileDialog1.OpenFile());
                    import = read.ReadToEnd();
                    ItemColection.Clear();
                    RactionColection.Clear();
                    int element = 0; ;
                    bool item = false, reaction = false, b_neutrals = false;
                    string data = "";
                    string rate_konstant_name = "";
                    string mobility_name = "";
                    foreach (char ch in import)
                    {
                        if (ch == 'R' && element == 0)
                        {
                            reaction = true;
                            Reaction = new Reactions();
                            Reaction.neutrals_A = new List<Items>();
                            Reaction.neutrals_B = new List<Items>();
                        }
                        if (ch == 'I' && element == 0)
                        {
                            item = true;
                            Item = new Items();
                        }
                        if (ch == '\n')
                        {
                            element = 0;
                            b_neutrals = false;
                            if (reaction)
                            {
                                reaction = false;
                                RactionColection.Add(Reaction);
                            }
                            if (item)
                            {
                                item = false;
                                ItemColection.Add(Item);
                            }
                        }
                        if (reaction)
                        {
                            if (ch == '\t')
                            {
                                if (element == 1)
                                {
                                    Reaction.name = data;
                                }
                                if (element == 2)
                                {
                                    rate_konstant_name = data;
                                }
                                if (element == 3)
                                {
                                    Reaction.rate_konstant = new rate_functions(rate_konstant_name, data);
                                }
                                if (element == 4)
                                {
                                    rate_konstant_name = data;
                                }
                                if (element == 5)
                                {
                                    if (data != "$")
                                    {
                                        Reaction.rate_konstant_ = new rate_functions(rate_konstant_name, data);
                                    }
                                }
                                if (element == 6)
                                {                                    
                                    if (data != "$")
                                    {
                                        Reaction.reaction_type = Convert.ToInt32(data);
                                    }
                                }
                                if (element == 7)
                                {
                                    foreach (Items I in ItemColection)
                                    {
                                        if (I.name == data)
                                        {
                                            Reaction.item_A = I;
                                        }
                                    }
                                }
                                if (element == 8)
                                {
                                    foreach (Items I in ItemColection)
                                    {
                                        if (I.name == data)
                                        {
                                            Reaction.item_B = I;
                                        }
                                    }
                                }
                                if (element >= 9)
                                {
                                    foreach (Items I in ItemColection)
                                    {
                                        if (I.name == data)
                                        {
                                            if (!b_neutrals)
                                            {
                                                Reaction.neutrals_A.Add(I);
                                            }
                                            else
                                            {
                                                Reaction.neutrals_B.Add(I);
                                            }
                                        }
                                    }
                                }
                                element++;
                                data = "";
                            }
                            else
                            {
                                data += ch.ToString();
                            }
                        }
                        if (item)
                        {
                            if (ch == '\t')
                            {
                                if (element == 1)
                                {
                                    Item.name = data;
                                }
                                if (element == 2)
                                {
                                    Item.formula = data;
                                }
                                if (element == 3)
                                {
                                    Item.s_name = data;
                                }
                                if (element == 4)
                                {
                                    Item.concentration = Convertor(data);
                                }
                                if (element == 5)
                                {
                                    mobility_name = data;
                                }
                                if (element == 6)
                                {
                                    Item.mobility = new rate_functions(mobility_name, data);
                                }
                                if (element == 7)
                                {
                                    Item.mass = Convertor(data);
                                }
                                if (element == 8)
                                {
                                    Item.diffusion = Convertor(data);
                                }
                                if (element == 9)
                                {
                                    Item.cation = Convert.ToBoolean(data);
                                }
                                element++;
                                data = "";
                            }
                            else
                            {
                                data += ch.ToString();
                            }
                        }
                    }
                    read.Dispose();
                    read.Close();
                    save_settings();
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void newSchemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool empty = true;
            bool pokracuj = true;
            foreach (Items_used Item in ItemColection_used)
            {
                empty = false;
            }
            if (!empty)
            {
                if (MessageBox.Show("All created reactions will be deleted. \nDo you want to continue?", "Create a new panel",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                == DialogResult.No)
                {
                    pokracuj = false;
                }
            }
            if (pokracuj)
            {
                ItemColection_used.Clear();
                ReactionColection_used.Clear();
                foreach (Label lab in panel1.Controls.OfType<Label>())
                {
                    lab.Visible = false;
                    lab.Text = "";
                    lab.Location = new System.Drawing.Point(-1000, -1000);
                    lab.Dispose();
                }
                foreach (Label lab in panel1.Controls.OfType<Label>())
                {
                    lab.Visible = false;
                    lab.Text = "";
                    lab.Location = new System.Drawing.Point(-1000, -1000);
                    lab.Dispose();
                }
                this.Invalidate();
                draw_reactions();
            }
        }

        private void kineticModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Results_KineticModel Kinetick_results = new Results_KineticModel();
            Kinetick_results.Popis_din = name_din;
            Kinetick_results.data_storage = data_storage;
            Kinetick_results.name_din = name_din;
            if (calc_type)
            {
                Kinetick_results._distance = Time_duration;
            }
            else
            {
                Kinetick_results._distance = Distance;
            }
            if (results_din)
            {
                Kinetick_results._concentration = conc_end;
            }
            //Kinetick_results.start();
            //Kinetick_results.iniciate();
            Kinetick_results.Show(this);
            //Kinetick_results.start_animation();
            /*
             * kinetics_results shall be used to overwiev present and saved data via data_storage
             * Before it can be implemented, data_storage needs to be modified to also copy values (not via indexing) from neutrals list and settings classes
             * it shall contains data_storage import option and all data managing options from Calculation_control
             * 
             * */
        }

        private void profile3ToolStripMenuItem_Click(object sender, EventArgs e)
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
                    try_enable_proceed();
                    Cursor.Current = Cursors.Default;
                }
            }

        }

        private void interpolationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string[]> _experimental_input = new List<string[]>();
            foreach (string[] str in Experimental_input)
            {
                string[] n_str = new string[str.GetLength(0)];
                n_str = str;
                _experimental_input.Add(n_str);
            }
        }

        #endregion

        #region Pomocne triedy

        private void neutrals_konstants(bool status)
        {
            listBox_products.Enabled = status;
            listBox_reactants.Enabled = status;
            listBox_Direction.Enabled = status;
            button_rate_E.Enabled = status;
            button_rate_E_.Enabled = status;
            button3.Enabled = status;
            button4.Enabled = status;
            button5.Enabled = status;
            button6.Enabled = status;
        }

        private void formula_enable(bool Enable)
        {
            groupBox1.Enabled = Enable;
            try_enable_formula_add();
        }

        private static void try_enable_formula_add()
        {
            if (comboBox_name.Text != "" &&
                textBox_formula.Text != "" &&
                textBox_shortname.Text != "" &&
                textBox_concentration.Text != "" &&
                textBox_mobility.Text != "" &&
                item_mobility != null &&                
                textBox_difusion.Text != "")
            {
                button_ApplyElement.Enabled = true;
            }
            else
            {
                button_ApplyElement.Enabled = false;
            }
        }

        private void konstanta_enable(bool Enable)
        {
            groupBox2.Enabled = Enable;
            try_enable_konstanta_add();
        }

        public static void try_enable_konstanta_add()
        {
            create_reaction_name();
            if (button_rate_E_.Enabled)
            {
                if (comboBox_reaction.Text != "" &&
                    textBox1.Text != "" &&
                    textBox_Product.Text != "" &&
                    listBox_Direction.Text != "" &&
                    textBox_rate.Text != "" &&
                    textBox_rate_.Text != ""
                    )
                {
                    button_ApplyReaction.Enabled = true;
                }
                else
                {
                    button_ApplyReaction.Enabled = false;
                }
            }
            else
            {
                if (comboBox_reaction.Text != "" &&
                    textBox1.Text != "" &&
                    textBox_Product.Text != "" &&
                    listBox_Direction.Text != "" &&
                    textBox_rate.Text != ""
                    )
                {
                    button_ApplyReaction.Enabled = true;
                }
                else
                {
                    button_ApplyReaction.Enabled = false;
                }
            }
            chech_neutral_button();
        }

        private void try_enable_proceed()
        {
            bool items_ready = true;
            short cnt = 0;
            foreach (Reactions_used Reaction in ReactionColection_used)
            {
                foreach (Items_used Item in ItemColection_used)
                {
                    if (compare(Reaction.item_used_A, Item) || compare(Reaction.item_used_B, Item))
                    {
                        Item.used_control = true;
                    }
                    foreach (Items_used IT in Reaction.neutrals_used_A)
                    {
                        if (compare(IT, Item))
                        {
                            Item.used_control = true;
                        }
                    }
                    foreach (Items_used IT in Reaction.neutrals_used_B)
                    {
                        if (compare(IT, Item))
                        {
                            Item.used_control = true;
                        }
                    }
                }
            }
            foreach (Items_used Item in ItemColection_used)
            {
                if (Item.used_control == false)
                {
                    items_ready = false;
                }
                cnt++;
            }
            if (items_ready && cnt != 0)
            {
                this.button_Proceed.Enabled = true;
            }
            else
            {
                this.button_Proceed.Enabled = false;
            }
        }

        private static void create_reaction_name()
        {
            if (tvor_meno)
            {
                string str = textBox1.Text;
                if (listBox_reactants.Items.Count > 0)
                {
                    foreach (Items Item in listBox_reactants.Items)
                    {
                        str += " + " + Item.s_name;
                    }
                }
                str += " " + listBox_Direction.GetItemText(listBox_Direction.SelectedItem) + " " + textBox_Product.Text;
                if (listBox_products.Items.Count > 0)
                {
                    foreach (Items Item in listBox_products.Items)
                    {
                        str += " + " + Item.s_name;
                    }
                }
                comboBox_reaction.Text = str;
            }
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
                        MessageBox.Show("Unable to convert string to double ! " + d);
                    }
                }
            }
            return dou;
        }

        private bool compare(Items I1, Items I2)
        {
            bool results = false;
            try
            {
                if ((I1.name == I2.name) && (I1.s_name == I2.s_name) && (I1.cation == I2.cation) && (I1.diffusion == I2.diffusion) && (I1.formula == I2.formula)
                    && compare(I1.mobility,I2.mobility))
                {
                    results = true;
                }
            }
            catch
            {
                if ((I1 == null) && (I2 == null))
                {
                    results = true;
                }
            }
            return results;
        }

        private bool compare(List<Items> LI1, List<Items> LI2)
        {
            bool results = false;
            try
            {
                bool test = true;
                foreach (Items I1 in LI1)
                {
                    foreach (Items I2 in LI2)
                    {
                        if (LI1.IndexOf(I1) == LI2.IndexOf(I2))
                        {
                            if (!compare(I1, I2))
                            {
                                test = false;
                            }
                        }
                    }
                }
                if (test)
                {
                    results = true;
                }
            }
            catch
            {
                if ((LI1 == null) && (LI2 == null))
                {
                    results = true;
                }
            }
            return results;
        }

        private bool compare(Items_used I1, Items_used I2)
        {
            bool results = false;
            try
            {
                if ((I1.name == I2.name) && (I1.s_name == I2.s_name) && (I1.cation == I2.cation) && (I1.diffusion == I2.diffusion) && (I1.formula == I2.formula)
                     && (compare(I1.mobility,I2.mobility)) && (I1.box_number == I2.box_number) && (I1.relative_x == I2.relative_x) && (I1.relative_y == I2.relative_y))
                {
                    results = true;
                }
            }
            catch
            {
                if ((I1 == null) && (I2 == null))
                {
                    results = true;
                }
            }
            return results;
        }

        private bool compare(List<Items_used> LI1, List<Items_used> LI2)
        {
            bool results = false;
            try
            {
                bool test = true;
                foreach (Items_used I1 in LI1)
                {
                    foreach (Items_used I2 in LI2)
                    {
                        if (LI1.IndexOf(I1) == LI2.IndexOf(I2))
                        {
                            if (!compare(I1, I2))
                            {
                                test = false;
                            }
                        }
                    }
                }
                if (test)
                {
                    results = true;
                }
            }
            catch
            {
                if ((LI1 == null) && (LI2 == null))
                {
                    results = true;
                }
            }
            return results;
        }

        private bool compare(Reactions R1, Reactions R2)
        {
            bool results = false;
            try
            {
                if ((R1.name == R2.name) && (compare(R1.item_A, R2.item_A)) && (compare(R1.item_B, R2.item_B)) && compare(R1.neutrals_A, R2.neutrals_A) && compare(R1.neutrals_B, R2.neutrals_B) &&
                    compare(R1.rate_konstant, R2.rate_konstant) && compare(R1.rate_konstant_, R2.rate_konstant_) && (R1.reaction_type == R2.reaction_type))
                {
                    //if()
                    results = true;
                }
            }
            catch
            {
                if ((R1 == null) && (R2 == null))
                {
                    results = true;
                }
            }
            return results;
        }

        private void addFielPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rad_zobrazenia++;
            if (rad_zobrazenia > 10)
            {
                rad_zobrazenia = 10;
            }
            this.Invalidate();
        }

        private void removeFieldSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rad_zobrazenia--;
            if (rad_zobrazenia < 1)
            {
                rad_zobrazenia = 1;
            }
            this.Invalidate();
        }

        private bool compare(Reactions_used R1, Reactions_used R2)
        {
            bool results = false;
            try
            {
                if ((R1.name == R2.name) && (compare(R1.item_A, R2.item_A)) && (compare(R1.item_B, R2.item_B)) && compare(R1.neutrals_A, R2.neutrals_A) && compare(R1.neutrals_B, R2.neutrals_B) &&
                    compare(R1.rate_konstant, R2.rate_konstant) && compare(R1.rate_konstant_, R2.rate_konstant_) && (R1.reaction_type == R2.reaction_type) && (R1.item_number_A == R2.item_number_A) &&
                    ((R1.item_number_B == R2.item_number_B)) && compare(R1.item_used_A, R2.item_used_A) && (compare(R1.item_used_B, R2.item_used_B)) && compare(R1.neutrals_used_A, R2.neutrals_used_A) &&
                    compare(R1.neutrals_used_B, R2.neutrals_used_B))
                {
                    results = true;
                }
            }
            catch
            {
                if ((R1 == null) && (R2 == null))
                {
                    results = true;
                }
            }
            return results;
        }

        private bool compare(rate_functions F1, rate_functions F2)
        {
            bool result = false;
            try
            {
                if ((F1.name == F2.name) && (F1.representation == F2.representation) && (F1.dimension == F2.dimension))
                {
                    result = true;
                }
            }
            catch
            {
                if ((F1 == null) && (F2 == null))
                {
                    result = true;
                }
            }
            return result;
        }

        public static void save_settings()
        {
            Cursor.Current = Cursors.WaitCursor;
            Properties.Settings.Default["Time_duration"] = Time_duration;
            Properties.Settings.Default["Number_of_steps"] = NumberOfSteps;
            Properties.Settings.Default["Distance"] = Distance;
            Properties.Settings.Default["Ion_velocity"] = Ion_velociy;
            Properties.Settings.Default["calc_type"] = calc_type;
            Properties.Settings.Default["Gauss_sigma"] = Gauss_signa;
            Properties.Settings.Default["Temperature"] = Temperature;
            Properties.Settings.Default["Pressure"] = Pressure;
            Properties.Settings.Default["Radius"] = Radius;
            Properties.Settings.Default["Gauss_density"] = Gauss_density;
            Properties.Settings.Default["Diffusion"] = Diffusion;
            Properties.Settings.Default["Infinite_system"] = Infinite_system;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, ItemColection);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.ItemColection = Convert.ToBase64String(buffer);
            }
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, RactionColection);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.RactionColection = Convert.ToBase64String(buffer);
            }
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, carrier_gas_type);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.carrier_gas_type = Convert.ToBase64String(buffer);
            }
            Properties.Settings.Default.Save();
            Cursor.Current = Cursors.Default;
        }

        public static void load_settings()
        {
            Cursor.Current = Cursors.WaitCursor;
            ItemColection.Clear();
            RactionColection.Clear();
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.ItemColection)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                ItemColection = (List<Items>)bf.Deserialize(ms);
            }
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.RactionColection)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                RactionColection = (List<Reactions>)bf.Deserialize(ms);
            }
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.carrier_gas_type)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                carrier_gas_type = (Items)bf.Deserialize(ms);
            }
            comboBox_name.Items.Clear();
            foreach (Items Item in ItemColection)
            {
                if (Item.cation == true)
                {
                    comboBox_name.Items.Add(Item);
                }
            }
            comboBox_name.DisplayMember = "name";
            Time_duration = Properties.Settings.Default.Time_duration;
            NumberOfSteps = Properties.Settings.Default.Number_of_steps;
            Distance = Properties.Settings.Default.Distance;
            Ion_velociy = Properties.Settings.Default.Ion_velocity;
            calc_type = Properties.Settings.Default.calc_type;
            Gauss_signa = Properties.Settings.Default.Gauss_sigma;
            Temperature = Properties.Settings.Default.Temperature;
            Pressure = Properties.Settings.Default.Pressure;
            Radius = Properties.Settings.Default.Radius;
            Gauss_density = Properties.Settings.Default.Gauss_density;
            Diffusion = Properties.Settings.Default.Diffusion;
            Infinite_system = Properties.Settings.Default.Infinite_system;
            Cursor.Current = Cursors.Default;
        }

        public static void use_new_rate_()
        {
            // insert data to textbox
            textBox_rate_.Text = new_rate_function.representation;
            // insert data to reactions
            _rate_function = new rate_functions();
            _rate_function = new_rate_function;
            try_enable_konstanta_add();
        }

        public static void use_new_rate()
        {
            // insert data to textbox
            textBox_rate.Text = new_rate_function.representation;
            // insert data to reactions
            rate_function = new rate_functions();
            rate_function = new_rate_function;
            try_enable_konstanta_add();
        }

        public static void use_new_mobility()
        {
            // insert data to textbox
            textBox_mobility.Text = new_rate_function.representation;
            // insert data to reactions
            item_mobility = new rate_functions();
            item_mobility = new_rate_function;
            try_enable_formula_add();
        }

        public static void listbox_change()
        {
            try_enable_konstanta_add();
            chech_neutral_button();
        }

        private static void chech_neutral_button()
        {
            if (listBox_products.Items.Count == 0)
            {
                button6.Enabled = false;
            }
            else
            {
                button6.Enabled = true;
            }
            if (listBox_reactants.Items.Count == 0)
            {
                button5.Enabled = false;
            }
            else
            {
                button5.Enabled = true;
            }
        }

        public static void calculation_done()
        {
            ResultsToolStripMenuItem.Enabled = true;
        }

        #endregion

    }
}
