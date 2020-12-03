using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KIMI_Sim
{
    [Serializable]
    public class Items
    {
        private string _name;
        private string _formula;
        private string _s_name;
        private double _concentration;
        private rate_functions _mobility;
        private double _mass;
        private double _diffusion;
        private bool _cation;

        public int group_ID { get; set; }

        public virtual string name          { get { return _name; }         set { _name = value; } }
        public virtual string formula       { get { return _formula; }      set { _formula = value; } }
        public virtual string s_name        { get { return _s_name; }       set { _s_name = value; } }
        public virtual double concentration { get { return _concentration;} set { _concentration = value; } }
        public virtual rate_functions mobility      { get { return _mobility; }     set { _mobility = value; } }
        public virtual double mass          { get { return _mass; }         set { _mass = value; } }
        public virtual bool cation          { get { return _cation; }       set { _cation = value; } }
        public virtual double diffusion     { get { return _diffusion; }    set { _diffusion = value; } }

        public override string ToString()
        {
            return name;
        }

        public Items() { }

        public Items(string Name, string Formula, string S_name, double Concentration, rate_functions Mobility, double Mass, double Diffusion, bool Cation)
        {
            _name = Name;
            _formula = Formula;
            _s_name = S_name;
            _concentration = Concentration;
            _mobility = Mobility;
            _mass = Mass;
            _diffusion = Diffusion;
            _cation = Cation;
        }

        public Items(Items Item)
        {
            _name = Item.name;
            _formula = Item.formula;
            _s_name = Item.s_name;
            _concentration = Item.concentration;
            _mobility = Item.mobility;
            _mass = Item.mass;
            _diffusion = Item.diffusion;
            _cation = Item.cation;
        }

        public void Import_used(Items_used Item_used)
        {
            _name = Item_used.name;
            _formula = Item_used.formula;
            _s_name = Item_used.s_name;
            _concentration = Item_used.concentration;
            _mobility = Item_used.mobility;
            _mass = Item_used.mass;
            _diffusion = Item_used.diffusion;
            _cation = Item_used.cation;
        }
    }

    [Serializable]
    public class Items_used : Items
    {
        private Items _item;
        private int _box_number;
        private int _relative_x;
        private int _relative_y;
        private bool _used_control;

        public int box_number    { get { return _box_number; }   set { _box_number = value;   } }
        public int relative_x    { get { return _relative_x; }   set { _relative_x = value;   } }
        public int relative_y    { get { return _relative_y; }   set { _relative_y = value;   } }
        public bool used_control { get { return _used_control; } set { _used_control = value; } }

        public override double concentration { get { return _item.concentration; }   set { _item.concentration = value; } }
        public override string name          { get { return _item.name; } }
        public override string formula       { get { return _item.formula; } }
        public override string s_name        { get { return _item.s_name; } }
        public override rate_functions mobility      { get { return _item.mobility; } }
        public override double mass          { get { return _item.mass; } }
        public override bool cation          { get { return _item.cation; } }
        public override double diffusion     { get { return _item.diffusion; } }

        public Items_used() { }

        public Items_used(Items Item, int Box_Number, int Relative_x, int Relative_y, bool Used_control)
        {
            _item = Item;
            _box_number = Box_Number;
            _relative_x = Relative_x;
            _relative_y = Relative_y;
            _used_control = Used_control;
        }

        public Items_used(Items Item, bool Used_control)
        {
            _item = Item;
            _used_control = Used_control;
        }

        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public void rewrite_Item(Items Item)
        {
            _item = Item;
        }

        public Items GetItem()
        {
            return _item;
        }
    }

    [Serializable]
    public class Reactions
    {
        private string _name;
        private rate_functions _rate_konstant;
        private rate_functions _rate_konstant_;
        private int _reaction_type;
        private Items _item_A;
        private Items _item_B;
        private List<Items> _neutrals_A;
        private List<Items> _neutrals_B;

        public virtual string name                   { get { return _name; }           set { _name = value; } }
        public virtual rate_functions rate_konstant  { get { return _rate_konstant; }  set { _rate_konstant = value; } }
        public virtual rate_functions rate_konstant_ { get { return _rate_konstant_; } set { _rate_konstant_ = value; } }
        public virtual int reaction_type             { get { return _reaction_type; }  set { _reaction_type = value; } }
        public virtual Items item_A                  { get { return _item_A; }         set { _item_A = value; } }
        public virtual Items item_B                  { get { return _item_B; }         set { _item_B = value; } }
        public virtual List<Items> neutrals_A        { get { return _neutrals_A; }     set { _neutrals_A = value; } }
        public virtual List<Items> neutrals_B        { get { return _neutrals_B; }     set { _neutrals_B = value; } }

        public override string ToString()
        {
            return name;
        }

        public Reactions() { }

        public Reactions(Reactions Reaction)
        {
            _name = Reaction.name;
            _rate_konstant = Reaction.rate_konstant;
            _rate_konstant_ = Reaction.rate_konstant_;
            _reaction_type = Reaction.reaction_type;
            _item_A = Reaction.item_A;
            _item_B = Reaction.item_B;
            _neutrals_A = Reaction.neutrals_A;
            _neutrals_B = Reaction.neutrals_B;
        }

        public Reactions(string Name, rate_functions Rate_konstant, rate_functions Rate_konstant_, int Reaction_type, Items Item_A, Items Item_B, List<Items> Neutrals_A, List<Items> Neutrals_B)
        {
            _name = Name;
            _rate_konstant = Rate_konstant;
            _rate_konstant_ = Rate_konstant_;
            _reaction_type = Reaction_type;
            _item_A = Item_A;
            _item_B = Item_B;
            _neutrals_A = Neutrals_A;
            _neutrals_B = Neutrals_B;
        }
    }

    [Serializable]
    public class Reactions_used : Reactions
    {
        private Reactions _reaction;
        private Point _pointA;
        private int _item_number_A;
        private Point _pointB;
        private int _item_number_B;
        private Items_used _item_A;
        private Items_used _item_B;
        private List<Items_used> _neutrals_A;
        private List<Items_used> _neutrals_B;

        public Point pointA                     { get { return _pointA; }           set { _pointA = value; } }
        public int item_number_A                { get { return _item_number_A; }    set { _item_number_A = value; } }
        public Point pointB                     { get { return _pointB; }           set { _pointB = value; } }
        public int item_number_B                { get { return _item_number_B; }    set { _item_number_B = value; } }
        public Items_used item_used_A           { get { return _item_A; }           set { _item_A = value; } }
        public Items_used item_used_B           { get { return _item_B; }           set { _item_B = value; } }
        public List<Items_used> neutrals_used_A { get { return _neutrals_A; }       set { _neutrals_A = value; } }
        public List<Items_used> neutrals_used_B { get { return _neutrals_B; }       set { _neutrals_B = value; } }

        public override string name                     { get { return _reaction.name; } }
        public override rate_functions rate_konstant    { get { return _reaction.rate_konstant; } set { _reaction.rate_konstant = value; } }
        public override rate_functions rate_konstant_   { get { return _reaction.rate_konstant_; } set { _reaction.rate_konstant_ = value; } }
        public override int reaction_type               { get { return _reaction.reaction_type; } }
        public override Items item_A                    { get { return _reaction.item_A; } }
        public override Items item_B                    { get { return _reaction.item_B; } }
        public override List<Items> neutrals_A          { get { return _reaction.neutrals_A; } }
        public override List<Items> neutrals_B          { get { return _reaction.neutrals_B; } }

        public override string ToString()
        {
            return name;
        }

        public string Specific_name()
        {
            if (reaction_type == 2)
            {
                string new_name = "";
                foreach (char ch in name)
                {
                    new_name += ch.ToString();
                    if (ch == '>')
                    {
                        new_name += ch.ToString();
                    }
                }
                return new_name;
            }
            else
            {
                return name;
            }
        }

        public string Specific_name_()
        {
            if (reaction_type == 2)
            {
                string new_name = "";
                foreach (char ch in name)
                {
                    new_name += ch.ToString();
                    if (ch == '<')
                    {
                        new_name += ch.ToString();
                    }
                }
                return new_name;
            }
            else
            {
                return name;
            }
        }

        public Reactions_used() { }

        public Reactions_used(Reactions Reaction, Point PointA, int Item_number_A, Point PointB, int Item_number_B, Items_used Item_A, Items_used Item_B, List<Items_used> Neutrals_A, List<Items_used> Neutrals_B)
        {
            _reaction = Reaction;
            _pointA = PointA;
            _item_number_A = Item_number_A;
            _pointB = PointB;
            _item_number_B = Item_number_B;
            _item_A = Item_A;
            _item_B = Item_B;
            _neutrals_A = Neutrals_A;
            _neutrals_B = Neutrals_B;
        }

        public Reactions GetReaction()
        {
            return _reaction;
        }
    }

    [Serializable]
    public class rate_functions                        
     {
        private string _name = "";
        private int _dimension = 0;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _C;
        private bool _C_lock;
        private double _D;
        private bool _D_lock;
        private double _A_int;
        private double _B_int;
        private double _C_int;
        private double _D_int;
        private double _mass_element;
        private rate_functions _reactant_mobility;

        public virtual string name { get { return _name; } }
        public virtual int dimension { get { return _dimension; } }

        public virtual string representation { get { return _A.ToString() + " + " + _B.ToString() + "*E"; } }
        public virtual double A { get { return _A; } set { _A = value; } }
        public virtual bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public virtual double B { get { return _B; } set { _B = value; } }
        public virtual bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public virtual double C { get { return _C; } set { _C = value; } }
        public virtual bool C_lock { get { return _C_lock; } set { _C_lock = value; } }
        public virtual double D { get { return _D; } set { _D = value; } }
        public virtual bool D_lock { get { return _D_lock; } set { _D_lock = value; } }

        public virtual double A_int { get { return _A_int; } set { _A_int = value; } }
        public virtual double B_int { get { return _B_int; } set { _B_int = value; } }
        public virtual double C_int { get { return _C_int; } set { _C_int = value; } }
        public virtual double D_int { get { return _D_int; } set { _D_int = value; } }
        public virtual double mass_element { get { return _mass_element; } set { _mass_element = value; } }
        public virtual rate_functions reactant_mobility { get { return _reactant_mobility; } set { _reactant_mobility = value; } }

        public virtual double Value(double E)
        {
            return E;
        }

        public override string ToString()
        {
            return name;
        }

        public rate_functions()
        { }

        public rate_functions(string Name, string Representation)
        {
            if (Name == "A")
            {

            }
            if (Name == "A + B*E")
            {

            }
            if (Name == "A*EXP(B*E)")
            {

            }
        }

        public void update(string Representation)
        {
            if (name == "A")
            {
                try
                {
                    A = Main.Convertor(Representation);
                }
                catch
                { }
            }
            if (name == "A + B*(E/N)")
            {
                try
                {
                    double new_a = 0;
                    double new_b = 0;
                    string data = "";
                    foreach (char ch in Representation)
                    {
                        if (ch == '+')
                        {
                            new_a = Main.Convertor(data);
                            data = "";
                        }
                        else
                        {
                            if (ch == '*')
                            {
                                new_b = Main.Convertor(data);
                            }
                            else
                            {
                                data += ch.ToString();
                            }
                        }
                    }
                    A = new_a;
                    B = new_b;
                }
                catch
                { }
            }
            if (name == "A + B*(E/N) + C*(E/N)^2")
            {
                try
                {
                    double new_a = 0;
                    bool a_done = false;
                    double new_b = 0;
                    bool b_done = false;
                    double new_c = 0;
                    string data = "";
                    foreach (char ch in Representation)
                    {
                        if (ch == '+')
                        {
                            if (!a_done)
                            {
                                new_a = Main.Convertor(data);
                                data = "";
                                a_done = true;
                            }
                            else
                            {
                                data = "";
                            }
                        }
                        else
                        {
                            if (ch == '*')
                            {
                                if (!b_done)
                                {
                                    new_b = Main.Convertor(data);
                                    data = "";
                                    b_done = true;
                                }
                                else
                                {
                                    new_c = Main.Convertor(data);
                                }
                            }
                            else
                            {
                                data += ch.ToString();
                            }
                        }
                    }
                    A = new_a;
                    B = new_b;
                    C = new_c;
                }
                catch
                { }
            }
            if (name == "A + B*(E/N) + C*(E/N)^2 + D*(E/N)^3")
            {
                try
                {
                    double new_a = 0;
                    bool a_done = false;
                    double new_b = 0;
                    bool b_done = false;
                    double new_c = 0;
                    bool c_done = false;
                    double new_d = 0;
                    string data = "";
                    foreach (char ch in Representation)
                    {
                        if (ch == '+')
                        {
                            if (!a_done)
                            {
                                new_a = Main.Convertor(data);
                                data = "";
                                a_done = true;
                            }
                            else
                            {
                                data = "";
                            }
                        }
                        else
                        {
                            if (ch == '*')
                            {
                                if (!b_done)
                                {
                                    new_b = Main.Convertor(data);
                                    data = "";
                                    b_done = true;
                                }
                                else
                                {
                                    if (!c_done)
                                    {
                                        new_c = Main.Convertor(data);
                                        data = "";
                                        c_done = true;
                                    }
                                    else
                                    {
                                        new_d = Main.Convertor(data);
                                    }
                                }
                            }
                            else
                            {
                                data += ch.ToString();
                            }
                        }
                    }
                    A = new_a;
                    B = new_b;
                    C = new_c;
                    D = new_d;
                }
                catch
                { }
            }
            if (name == "A*EXP(B*(E/N))")
            {
                try
                {
                    double new_a = 0;
                    double new_b = 0;
                    string data = "";
                    short v = 0;
                    foreach (char ch in Representation)
                    {
                        if (ch == '*' && v == 0)
                        {
                            new_a = Main.Convertor(data);
                            data = "";
                            v++;
                        }
                        else
                        {
                            if (ch == '[')
                            {
                                data = "";
                            }
                            else
                            {
                                if (ch == '*')
                                {
                                    new_b = Main.Convertor(data);
                                }
                                else
                                {
                                    data += ch.ToString();
                                }
                            }
                        }
                    }
                    A = new_a;
                    B = new_b;
                }
                catch
                {
                }
            }
            if (name == "A*EXP(B/Er(E/N))")
            {
                try
                {
                    double new_a = 0;
                    double new_b = 0;
                    string data = "";
                    short v = 0;
                    foreach (char ch in Representation)
                    {
                        if (ch == '*' && v == 0)
                        {
                            new_a = Main.Convertor(data);
                            data = "";
                            v++;
                        }
                        else
                        {
                            if (ch == '[')
                            {
                                data = "";
                            }
                            else
                            {
                                if (ch == '/')
                                {
                                    if (new_b == 0)
                                    {
                                        new_b = Main.Convertor(data);
                                    }
                                }
                                else
                                {
                                    data += ch.ToString();
                                }
                            }
                        }
                    }
                    A = new_a;
                    B = new_b;
                }
                catch
                {
                }
            }
            if (name == "C + A*EXP(B/(E/N))")
            {
                try
                {
                    double new_a = 0;
                    double new_b = 0;
                    double new_c = 0;
                    string data = "";
                    short v = 0;
                    foreach (char ch in Representation)
                    {
                        if (ch == '+' && v == 0)
                        {
                            new_c = Main.Convertor(data);
                            data = "";
                            v++;
                        }
                        else
                        {
                            if (ch == '*' && v == 1)
                            {
                                new_a = Main.Convertor(data);
                                data = "";
                                v++;
                            }
                            else
                            {
                                if (ch == '[')
                                {
                                    data = "";
                                }
                                else
                                {
                                    if (ch == '/')
                                    {
                                        new_b = Main.Convertor(data);
                                    }
                                    else
                                    {
                                        data += ch.ToString();
                                    }
                                }
                            }
                        }
                    }
                    A = new_a;
                    B = new_b;
                    C = new_c;
                }
                catch
                {
                }
            }
            if (name == "A/(1+(E/N))^B")
            {
                try
                {
                    double new_a = 0;
                    string data = "";
                    short v = 0;
                    foreach (char ch in Representation)
                    {
                        if (ch == '/' && v == 0)
                        {
                            new_a = Main.Convertor(data);
                            data = "";
                            v++;
                        }
                        else
                        {
                            if (ch == '^')
                            {
                                data = "";
                            }
                            else
                            {
                                data += ch.ToString();
                            }
                        }
                    }
                    A = new_a;
                    B = Main.Convertor(data);
                }
                catch
                {
                }
            }
            if (name == "A*EXP(B*(E/N)^2)")
            {
                try
                {
                    double new_a = 0;
                    double new_b = 0;
                    string data = "";
                    short v = 0;
                    foreach (char ch in Representation)
                    {
                        if (ch == '*' && v == 0)
                        {
                            new_a = Main.Convertor(data);
                            data = "";
                            v++;
                        }
                        else
                        {
                            if (ch == '[')
                            {
                                data = "";
                            }
                            else
                            {
                                if (ch == '*')
                                {
                                    new_b = Main.Convertor(data);
                                }
                                else
                                {
                                    data += ch.ToString();
                                }
                            }
                        }
                    }
                    A = new_a;
                    B = new_b;
                }
                catch
                {
                }
            }
            if (name == "A/Er(E/N)^B")
            {
                try
                {
                    double new_a = 0;
                    string data = "";
                    short v = 0;
                    foreach (char ch in Representation)
                    {
                        if (ch == '/' && v == 0)
                        {
                            new_a = Main.Convertor(data);
                            data = "";
                            v++;
                        }
                        else
                        {
                            if (ch == '^')
                            {
                                data = "";
                            }
                            else
                            {
                                data += ch.ToString();
                            }
                        }
                    }
                    A = new_a;
                    B = Main.Convertor(data);
                }
                catch
                {
                }
            }
        }
    }

    [Serializable]
    public class Constant : rate_functions
    {
        private string _name = "A";
        private int _dimension = 0;
        private double _A;
        private bool _A_lock;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }

        public override string representation { get { return _A.ToString(); } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }

        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Constant()
        { }

        public Constant(double A, bool A_lock)
        {
            _A = A;
            _A_lock = A_lock;
        }

        public override double Value(double E)
        {
            return _A;
        }
    }

    [Serializable]
    public class Linear : rate_functions
    {
        private string _name = "A + B*(E/N)";
        private int _dimension = 2;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _A_int;
        private double _B_int;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }

        public override string representation { get { return _A.ToString() + " + " + _B.ToString() + "*(E/N)"; } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public override double B { get { return _B; } set { _B = value; } }
        public override bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public double A_int { get { return _A_int; } set { _A_int = value; } }
        public double B_int { get { return _B_int; } set { _B_int = value; } }

        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Linear()
        { }

        public Linear(double A, double B, bool A_lock, bool B_lock)
        {
            _A = A;
            _B = B;
            _A_lock = A_lock;
            _B_lock = B_lock;
            _A_int = A;
            _B_int = B;
        }

        public override double Value(double E)
        {
            double value = _A + (_B * E);
            return value;
        }
    }

    [Serializable]
    public class Quadratic : rate_functions
    {
        private string _name = "A + B*(E/N) + C*(E/N)^2";
        private int _dimension = 4;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _C;
        private bool _C_lock;
        private double _A_int;
        private double _B_int;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }

        public override string representation { get { return _A.ToString() + " + " + _B.ToString() + "*(E/N)" + " + " + _C.ToString() + "*(E/N)^2"; } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public override double B { get { return _B; } set { _B = value; } }
        public override bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public override double C { get { return _C; } set { _C = value; } }
        public override bool C_lock { get { return _C_lock; } set { _C_lock = value; } }

        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Quadratic()
        { }

        public Quadratic(double A, double B, double C, bool A_lock, bool B_lock, bool C_lock)
        {
            _A = A;
            _B = B;
            _C = C;
            _A_lock = A_lock;
            _B_lock = B_lock;
            _C_lock = C_lock;
        }

        public override double Value(double E)
        {
            double value = _A + (_B * E) + (_C * E * E);
            return value;
        }
    }

    [Serializable]
    public class Cubic : rate_functions
    {
        private string _name = "A + B*(E/N) + C*(E/N)^2 + D*(E/N)^3";
        private int _dimension = 4;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _C;
        private bool _C_lock;
        private double _D;
        private bool _D_lock;
        private double _A_int;
        private double _B_int;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }

        public override string representation { get { return _A.ToString() + " + " + _B.ToString() + "*(E/N)" + " + " + _C.ToString() + "*(E/N)^2" + " + " + _D.ToString() + "*(E/N)^3"; } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public override double B { get { return _B; } set { _B = value; } }
        public override bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public override double C { get { return _C; } set { _C = value; } }
        public override bool C_lock { get { return _C_lock; } set { _C_lock = value; } }
        public override double D { get { return _D; } set { _D = value; } }
        public override bool D_lock { get { return _D_lock; } set { _D_lock = value; } }

        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Cubic()
        { }

        public Cubic(double A, double B, double C, double D, bool A_lock, bool B_lock, bool C_lock, bool D_lock)
        {
            _A = A;
            _B = B;
            _C = C;
            _D = D;
            _A_lock = A_lock;
            _B_lock = B_lock;
            _C_lock = C_lock;
            _D_lock = D_lock;
        }

        public override double Value(double E)
        {
            double value = _A + (_B * E) + (_C * E * E) + (_D * E * E * E);
            return value;
        }
    }

    [Serializable]
    public class Exponencional : rate_functions
    {
        private string _name = "A*EXP(B*(E/N))";
        private int _dimension = 2;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _A_int;
        private double _B_int;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }

        public override string representation { get { return _A.ToString() + "*EXP[" + _B.ToString() + "*(E/N)]"; } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public override double B { get { return _B; } set { _B = value; } }
        public override bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public double A_int { get { return _A_int; } set { _A_int = value; } }
        public double B_int { get { return _B_int; } set { _B_int = value; } }


        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Exponencional()
        { }

        public Exponencional(double A, double B, bool A_lock, bool B_lock)
        {
            _A = A;
            _B = B;
            _A_lock = A_lock;
            _B_lock = B_lock;
            _A_int = A;
            _B_int = B;
        }

        public override double Value(double E)
        {
            double value = _A * Math.Exp(_B * E);
            return value;
        }
    }

    [Serializable]
    public class Exponencional2 : rate_functions
    {
        private string _name = "A*EXP(B/Er(E/N))";
        private int _dimension = 2;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _A_int;
        private double _B_int;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }
        private double _mass_element;
        private rate_functions _reactant_mobility;

        public override string representation { get { return _A.ToString() + "*EXP[" + _B.ToString() + "/Er(E/N)]"; } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public override double B { get { return _B; } set { _B = value; } }
        public override bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public double A_int { get { return _A_int; } set { _A_int = value; } }
        public double B_int { get { return _B_int; } set { _B_int = value; } }
        public override double mass_element { get { return _mass_element; } set { _mass_element = value; } }
        public override rate_functions reactant_mobility { get { return _reactant_mobility; } set { _reactant_mobility = value; } }

        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Exponencional2()
        { }

        public Exponencional2(double A, double B, bool A_lock, bool B_lock)
        {
            _A = A;
            _B = B;
            _A_lock = A_lock;
            _B_lock = B_lock;
            _A_int = A;
            _B_int = B;
        }

        public override double Value(double E)
        {
            double thermal_energy = 3 * 1.38 * 1E-23 * Main.Temperature / 2; // J
            double v_d = (_reactant_mobility.Value(E) * E * 2.6887); // m*s-1
            double reaction_energy = 0.5 * _mass_element * v_d * v_d + thermal_energy; // J
            double reaction_energy_eV = reaction_energy / 1.602E-19;
            double value = _A * Math.Exp(_B / reaction_energy_eV);
            return value;
        }

        public double Reaction_energy(double E)
        {
            double thermal_energy = 3 * 1.38 * 1E-23 * Main.Temperature / 2;
            double v_d = (_reactant_mobility.Value(E) * E * 2.6887); // m*s-1
            double reaction_energy = 0.5 * _mass_element * v_d * v_d + thermal_energy;
            double reaction_energy_eV = reaction_energy / 1.602E-19;
            return reaction_energy_eV;
        }
    }

    [Serializable]
    public class Exponencional3 : rate_functions
    {
        private string _name = "C + A*EXP(B/(E/N))";
        private int _dimension = 3;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _C;
        private bool _C_lock;
        private double _A_int;
        private double _B_int;
        private double _C_int;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }

        public override string representation { get { return _C.ToString() + " + " + _A.ToString() + "*EXP[" + _B.ToString() + "/(E/N)]"; } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public override double B { get { return _B; } set { _B = value; } }
        public override bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public override double C { get { return _C; } set { _C = value; } }
        public override bool C_lock { get { return _C_lock; } set { _C_lock = value; } }
        public double A_int { get { return _A_int; } set { _A_int = value; } }
        public double B_int { get { return _B_int; } set { _B_int = value; } }
        public double C_int { get { return _C_int; } set { _C_int = value; } }

        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Exponencional3()
        { }

        public Exponencional3(double A, double B, double C, bool A_lock, bool B_lock, bool C_lock)
        {
            _A = A;
            _B = B;
            _C = C;
            _A_lock = A_lock;
            _B_lock = B_lock;
            _C_lock = C_lock;
            _A_int = A;
            _B_int = B;
            _C_int = C;
        }

        public override double Value(double E)
        {
            double value = _C + (_A * Math.Exp(_B / E));
            return value;
        }
    }

    [Serializable]
    public class Exponencional4 : rate_functions
    {
        private string _name = "A/(1+(E/N))^B";
        private int _dimension = 2;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _A_int;
        private double _B_int;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }

        public override string representation { get { return _A.ToString() + "/(1+(E/N))^" + _B.ToString(); } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public override double B { get { return _B; } set { _B = value; } }
        public override bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public double A_int { get { return _A_int; } set { _A_int = value; } }
        public double B_int { get { return _B_int; } set { _B_int = value; } }

        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Exponencional4()
        { }

        public Exponencional4(double A, double B, bool A_lock, bool B_lock)
        {
            _A = A;
            _B = B;
            _A_lock = A_lock;
            _B_lock = B_lock;
            _A_int = A;
            _B_int = B;
        }

        public override double Value(double E)
        {
            double value = _A / (Math.Pow(1 + E, _B));
            return value;
        }
    }

    [Serializable]
    public class Exponencional5 : rate_functions
    {
        private string _name = "A*EXP(B*(E/N)^2)";
        private int _dimension = 2;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _A_int;
        private double _B_int;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }

        public override string representation { get { return _A.ToString() + "*EXP[" + _B.ToString() + "*(E/N)^2]"; } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public override double B { get { return _B; } set { _B = value; } }
        public override bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public double A_int { get { return _A_int; } set { _A_int = value; } }
        public double B_int { get { return _B_int; } set { _B_int = value; } }


        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Exponencional5()
        { }

        public Exponencional5(double A, double B, bool A_lock, bool B_lock)
        {
            _A = A;
            _B = B;
            _A_lock = A_lock;
            _B_lock = B_lock;
            _A_int = A;
            _B_int = B;
        }

        public override double Value(double E)
        {
            double value = _A * Math.Exp(_B * E * E);
            return value;
        }
    }

    [Serializable]
    public class Association : rate_functions
    {
        private string _name = "A/Er(E/N)^B";
        private int _dimension = 2;
        private double _A;
        private bool _A_lock;
        private double _B;
        private bool _B_lock;
        private double _A_int;
        private double _B_int;
        private double _mass_element;
        private rate_functions _reactant_mobility;

        public override string name { get { return _name; } }
        public override int dimension { get { return _dimension; } }

        public override string representation { get { return _A.ToString() + "/Er(E/N)^" + _B.ToString(); } }
        public override double A { get { return _A; } set { _A = value; } }
        public override bool A_lock { get { return _A_lock; } set { _A_lock = value; } }
        public override double B { get { return _B; } set { _B = value; } }
        public override bool B_lock { get { return _B_lock; } set { _B_lock = value; } }
        public double A_int { get { return _A_int; } set { _A_int = value; } }
        public double B_int { get { return _B_int; } set { _B_int = value; } }
        public override double mass_element { get { return _mass_element; } set { _mass_element = value; } }
        public override rate_functions reactant_mobility { get { return _reactant_mobility; } set { _reactant_mobility = value; } }

        public override string ToString()
        {
            try
            {
                return name;
            }
            catch
            {
                return "";
            }
        }

        public Association()
        { }

        public Association(double A, double B, bool A_lock, bool B_lock)
        {
            _A = A;
            _B = B;
            _A_lock = A_lock;
            _B_lock = B_lock;
            _A_int = A;
            _B_int = B;
        }

        public override double Value(double E)
        {
            double thermal_energy = 3 * 1.38 * 1E-23 * Main.Temperature / 2;
            double v_d = (_reactant_mobility.Value(E) * E * 2.6887); // m*s-1
            double reaction_energy = 0.5 * _mass_element * v_d * v_d + thermal_energy;
            double reaction_energy_eV = reaction_energy / 1.602E-19;
            double value = _A / (Math.Pow(reaction_energy_eV, _B));
            return value;
        }

        public double Reaction_energy(double E)
        {
            double thermal_energy = 3 * 1.38 * 1E-23 * Main.Temperature / 2;
            double v_d = (_reactant_mobility.Value(E) * E * 2.6887); // m*s-1
            double reaction_energy = 0.5 * _mass_element * v_d * v_d + thermal_energy;
            double reaction_energy_eV = reaction_energy / 1.602E-19;
            return reaction_energy_eV;
        }
    }

    [Serializable]
    public class Data_storage
    {
        private DateTime _time;
        private int _dimension;
        private string _concentration_name;
        private double _electric_field_konstant_value;
        private string[] _hlavicka;
        private List<Reactions_used> _reactions_collection_used;
        private List<Items_used> _items_collection_used;
        private List<List<List<double[,]>>> _calculation_memory;
        private Calculation_setings _used_settings;

        private bool _is_time;
        private bool _is_con_din;
        private bool _is_ele;
        private bool _is_ele_din;

        public DateTime Time { get { return _time; } set { _time = value; } }
        public int Dimension { get { return _dimension; } set { _dimension = value; } }
        public List<Reactions_used> Reactions_collection_used { get { return _reactions_collection_used; } set { _reactions_collection_used = value; } }
        public List<Items_used> Items_collection_used { get { return _items_collection_used; } set { _items_collection_used = value; } }
        public string Concentration_name { get { return _concentration_name; } set { _concentration_name = value; } }
        public double Electric_field_konstant_value { get { return _electric_field_konstant_value; } set { _electric_field_konstant_value = value; } }

        public string[] Header { get { return _hlavicka; } set { _hlavicka = value; } }
        public List<List<List<double[,]>>> Calculation_memmory { get { return _calculation_memory; } set { _calculation_memory = value; } }
        public Calculation_setings Used_settings { get { return _used_settings; } set { _used_settings = value; } }

        public bool Is_Time { get { return _is_time; } set { _is_time = value; } }
        public bool Is_Con_din { get { return _is_con_din; } set { _is_con_din = value; } }
        public bool Is_Ele { get { return _is_ele; } set { _is_ele = value; } }
        public bool Is_Ele_din { get { return _is_ele_din; } set { _is_ele_din = value; } }

        public Data_storage() { }

        public Data_storage(DateTime Time, int Dimension, List<Reactions_used> Reactions_collection_used, List<Items_used> Items_collection_used, string Concentration_name, double Electric_field_konstant_value, string[] Header, List<List<List<double[,]>>> Calculation_memmory, Calculation_setings Used_settings)
        {
            _time = Time;
            _dimension = Dimension;
            _concentration_name = Concentration_name;
            _hlavicka = Header;
            _reactions_collection_used = Reactions_collection_used;
            _items_collection_used = Items_collection_used;
            _calculation_memory = Calculation_memmory;
            _electric_field_konstant_value = Electric_field_konstant_value;
            _used_settings = Used_settings;
        }

        public void Calculation_type(bool Is_Time, bool Is_Con_din, bool Is_Ele, bool Is_Ele_din)
        {
            _is_time = Is_Time;
            _is_con_din = Is_Con_din;
            _is_ele = Is_Ele;
            _is_ele_din = Is_Ele_din;
        }

        public string ToString()
        {
            string s = _time.ToString() + ", Dim: " + _dimension.ToString();
            return s;
        }
    }

    [Serializable]
    public class Calculation_setings
    {
        private double _time_duration;
        private double _numberOfSteps;
        private double _distance;
        private double _ion_velociy;
        private double _gauss_signa;
        private double _temperature;
        private double _pressure;
        private double _radius;
        private double _gauss_density;
        private bool _diffusion;
        private bool _infinite_system;
        private Items _carrier_gas;

        public double Time_duration { get { return _time_duration; }    set { _time_duration = value; } }
        public double NumberOfSteps { get { return _numberOfSteps; }    set { _numberOfSteps = value; } }
        public double Distance      { get { return _distance; }         set { _distance = value; } }
        public double Ion_velociy   { get { return _ion_velociy; }      set { _ion_velociy = value; } }
        public double Gauss_signa   { get { return _gauss_signa; }      set { _gauss_signa = value; } }
        public double Temperature   { get { return _temperature; }      set { _temperature = value; } }
        public double Pressure      { get { return _radius; }           set { _radius = value; } }
        public double Gauss_density { get { return _gauss_density; }    set { _gauss_density = value; } }
        public double Radius        { get { return _time_duration; }    set { _time_duration = value; } }
        public bool Diffusion       { get { return _diffusion; }        set { _diffusion = value; } }
        public bool Infinite_system { get { return _infinite_system; }  set { _infinite_system = value; } }
        public Items Carrier_gas    { get { return _carrier_gas; }      set { _carrier_gas = value; } }

        public Calculation_setings(double Time_duration, double NumberOfSteps, double Distance, double Ion_velociy, double Gauss_signa, double Temperature, double Pressure, double Gauss_density, double Radius, bool Diffusion, bool Infinite_system, Items Carrier_gas)
        {
            _time_duration = Time_duration;
            _numberOfSteps = NumberOfSteps;
            _distance = Distance;
            _ion_velociy = Ion_velociy;
            _gauss_signa = Gauss_signa;
            _temperature = Temperature;
            _pressure = Pressure;
            _radius = Radius;
            _gauss_density = Gauss_density;
            _diffusion = Diffusion;
            _infinite_system = Infinite_system;
            _carrier_gas = Carrier_gas;
        }

        public Calculation_setings()
        {
            _time_duration = Main.Time_duration;
            _numberOfSteps = Main.NumberOfSteps;
            _distance = Main.Distance;
            _ion_velociy = Main.Ion_velociy;
            _gauss_signa = Main.Gauss_signa;
            _temperature = Main.Temperature;
            _pressure = Main.Pressure;
            _radius = Main.Radius;
            _gauss_density = Main.Gauss_density;
            _diffusion = Main.Diffusion;
            _infinite_system = Main.Infinite_system;
            _carrier_gas = Main.carrier_gas_type;
        }

        public void Apply_settings()
        {
            Main.Time_duration = _time_duration;
            Main.NumberOfSteps = _numberOfSteps;
            Main.Distance = _distance;
            Main.Ion_velociy = _ion_velociy;
            Main.Gauss_signa = _gauss_signa;
            Main.Temperature = _temperature;
            Main.Pressure = _pressure;
            Main.Radius = _radius;
            Main.Gauss_density = _gauss_density;
            Main.Diffusion = _diffusion;
            Main.Infinite_system = _infinite_system;
            Main.carrier_gas_type = _carrier_gas;
        }
    }
     
    public class Item_box
    {
        public int Number { get; set; }
        public int Relaive_x { get; set; }
        public int Relaive_y { get; set; }
        public int Position_x { get; set; }
        public int Position_y { get; set; }
        public int Dim_x { get; set; }
        public int Dim_y { get; set; }
        public int Center_x { get; set; }
        public int Center_y { get; set; }
    }

    public class Legend_entity
    {
        public string name { get; set; }
        public bool status { get; set; }
    }
}
