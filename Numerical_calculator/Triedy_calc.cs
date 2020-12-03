    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIMI_Sim
{
    public class Equation
    {
        public string name { get; set; }
        public List<Equation_element> equation { get; set; }
        public double koeficient_0 { get; set; }
        public double koeficient_1 { get; set; }
        public double koeficient_2 { get; set; }
        public double koeficient_3 { get; set; }
        public double value { get; set; }

        public void set_velocity_(double E)
        {
            foreach(Equation_element element in equation)
            {
                element.set_velocity_(E);
            }
        }
    }

    public class Equation_element
    {
        private double _velocity; //cm*s^-1
        private rate_functions _mobility;

        public double velocity { get { return _velocity; } }
        public rate_functions mobility { set { _mobility = value; } }
        public string[] equation_element { get; set; }

        public void set_velocity_(double E)
        {
            double K = _mobility.Value(E);
            _velocity = K * 268.6781 * E; //K - reduced mobility, E - E/N (Td), V (cm/s)
        }
    }

    public class Premenna
    {
        public string name { get; set; }
        public double value { get; set; }
        public double initial_value { get; set; }
    }

    public class Konstants
    {
        private double _value;
        public string name { get; set; }
        public double value { get { return _value; } set { _value = value; } }
        public rate_functions function { get; set; }

        public void set_field(double E)
        {
            _value = function.Value(E);
        }
    }
}
