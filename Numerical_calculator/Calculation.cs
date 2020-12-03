using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIMI_Sim
{
    class Calculation
    {
        public double dt { get; set; }
        public List<Premenna> new_premenna { get; set; }
        public bool e_field { get; set; }
        public bool calc_type { get; set; }

        public void calculate_step(List<Equation> equation, List<Konstants> konstants, List<Premenna> premenna)
        {
            // pre kazdu rovnicu vypocitaj nultu, prvu druhu a tretiu konstantu postupne

            foreach (Equation eq in equation) // pocitaj nulty koeficient
            {
                double vysledok = 0;
                foreach (Equation_element partial_eq in eq.equation)
                {
                    double prispevok = 1;
                    foreach (string str in partial_eq.equation_element)
                    {
                        if (str == "True")
                        {
                            prispevok = prispevok * (-1);

                        }
                        foreach (Premenna pr in premenna)
                        {
                            if (pr.name == str)
                            {
                                prispevok = prispevok * pr.value;
                            }
                        }
                        foreach (Konstants ko in konstants)
                        {
                            if (ko.name == str)
                            {
                                prispevok = prispevok * ko.value;
                            }
                        }
                    }
                    prispevok = prispevok * dt;
                    if (!calc_type)
                    {
                        double velocity = Main.Ion_velociy / 100; // cm*s-1 to m*s-1
                        if (e_field)
                        {
                            velocity += partial_eq.velocity / 100;
                        }
                        prispevok /= velocity; // v in (m*s-1)
                    }
                    vysledok = vysledok + prispevok;
                }
                eq.koeficient_0 = vysledok; // f(x,y,t)*dt             
            }
            foreach (Equation eq in equation) // pocitaj prvy koeficient
            {
                double vysledok = 0;
                foreach (Equation_element partial_eq in eq.equation)
                {
                    double prispevok = 1;
                    foreach (string str in partial_eq.equation_element)
                    {
                        if (str == "True")
                        {
                            prispevok = prispevok * (-1);
                        }
                        foreach (Premenna pr in premenna)
                        {
                            if (pr.name == str)
                            {
                                foreach (Equation e in equation)
                                {
                                    if (str == e.name)
                                    {
                                        prispevok = prispevok * (pr.value + (e.koeficient_0) / 2);
                                    }
                                }
                            }
                        }
                        foreach (Konstants ko in konstants)
                        {
                            if (ko.name == str)
                            {
                                prispevok = prispevok * ko.value;
                            }
                        }
                    }
                    prispevok = prispevok * dt;
                    if (!calc_type)
                    {
                        double velocity = Main.Ion_velociy / 100; // cm*s-1 to m*s-1
                        if (e_field)
                        {
                            velocity += partial_eq.velocity / 100;
                        }
                        prispevok /= velocity; // v in (m*s-1)
                    }
                    vysledok = vysledok + prispevok;
                }
                eq.koeficient_1 = vysledok; // f(x,y,t)*dt   
            }
            foreach (Equation eq in equation) // pocitaj druhy koeficient
            {
                double vysledok = 0;
                foreach (Equation_element partial_eq in eq.equation)
                {
                    double prispevok = 1;
                    foreach (string str in partial_eq.equation_element)
                    {
                        if (str == "True")
                        {
                            prispevok = prispevok * (-1);
                        }
                        foreach (Premenna pr in premenna)
                        {
                            if (pr.name == str)
                            {
                                foreach (Equation e in equation)
                                {
                                    if (str == e.name)
                                    {
                                        prispevok = prispevok * (pr.value + (e.koeficient_1) / 2);
                                    }
                                }
                            }
                        }
                        foreach (Konstants ko in konstants)
                        {
                            if (ko.name == str)
                            {
                                prispevok = prispevok * ko.value;
                            }
                        }
                    }
                    prispevok = prispevok * dt;
                    if (!calc_type)
                    {
                        double velocity = Main.Ion_velociy / 100; // cm*s-1 to m*s-1
                        if (e_field)
                        {
                            velocity += partial_eq.velocity / 100;
                        }
                        prispevok /= velocity; // v in (m*s-1)
                    }
                    vysledok = vysledok + prispevok;
                }
                eq.koeficient_2 = vysledok; // f(x,y,t)*dt   
            }
            foreach (Equation eq in equation) // pocitaj treti koeficient
            {
                double vysledok = 0;
                foreach (Equation_element partial_eq in eq.equation)
                {
                    double prispevok = 1;
                    foreach (string str in partial_eq.equation_element)
                    {
                        if (str == "True")
                        {
                            prispevok = prispevok * (-1);
                        }
                        foreach (Premenna pr in premenna)
                        {
                             if (pr.name == str)
                            {
                                foreach (Equation e in equation)
                                {
                                    if (str == e.name)
                                    {
                                        prispevok = prispevok * (pr.value + e.koeficient_2);
                                    }
                                }
                            }
                        }
                        foreach (Konstants ko in konstants)
                        {
                            if (ko.name == str)
                            {
                                prispevok = prispevok * ko.value;
                            }
                        }
                    }
                    prispevok = prispevok * dt;
                    if (!calc_type)
                    {
                        double velocity = Main.Ion_velociy / 100; // cm*s-1 to m*s-1
                        if (e_field)
                        {
                            velocity += partial_eq.velocity / 100;
                        }
                        prispevok /= velocity; // v in (m*s-1)
                    }
                    vysledok = vysledok + prispevok;
                }
                eq.koeficient_3 = vysledok; // f(x,y,t)*dt   
            }
            foreach (Equation eq in equation)
            {
                double increment = (eq.koeficient_0 + 2 * eq.koeficient_1 + 2 * eq.koeficient_2 + eq.koeficient_3) / 6;
                foreach (Premenna pr in premenna)
                {
                    if (eq.name == pr.name)
                    {
                        pr.value = pr.value + increment;
                    }
                }
            }
            new_premenna = premenna;
        }
    }
}
