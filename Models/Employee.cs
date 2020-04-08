using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory
{
    /// <summary>
    /// Třída reprezentuje jednotlivé zaměstnance
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Jméno zaměstnance
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Hodinová mzda zaměstnance
        /// </summary>
        public double HourlyWage { get; set; }
        /// <summary>
        /// Měsiční mzda zaměstnance
        /// </summary>
        public double Wages { get; set; }
        /// <summary>
        /// Datum přijetí zaměstnance
        /// </summary>
        public DateTime HireDateTime { get; set; }
        /// <summary>
        /// Počet dní od nástupu zaměstnance do práce
        /// </summary>
        public int TimeSinceHire
        {
            get
            {
                TimeSpan timeSinceAdding = DateTime.Now - HireDateTime;
                int days = (int)timeSinceAdding.TotalDays;
                return days;
            }
        }

        /// <summary>
        /// Konstruktor pro nově přijaté zaměstnance
        /// </summary>
        /// <param name="name">Jméno a příjmení zaměstnance</param>
        /// <param name="wages">Měsiční mzda</param>
        public Employee(string name, double wages)
        {
            Name = name;
            Wages = wages;
            HourlyWage = wages/80;
            HireDateTime = DateTime.Now;
        }

        /// <summary>
        /// Konstruktor pro zaměstmance vzniklé tříděním kolekcí
        /// </summary>
        /// <param name="name">Jméno a příjmení zaměstnance</param>
        /// <param name="wages">Měsiční mzda</param>
        /// <param name="hireDateTime">/// Datum přijetí zaměstnance</param>
        public Employee(string name, double wages, DateTime hireDateTime)
        {
            Name = name;
            Wages = wages;
            HourlyWage = wages / 80;
            HireDateTime = hireDateTime;
        }

        /// <summary>
        /// Konstruktor pro chod serializéru
        /// </summary>
        public Employee()
        {

        }

        /// <summary>
        /// Přepsání metody
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
