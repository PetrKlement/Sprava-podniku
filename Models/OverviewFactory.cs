using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory
{
    /// <summary>
    /// Třída složí jako přehled počtu zaměstnanců, počtu zakázek a množství peněz na účtě.
    /// </summary>
    public class OverviewFactory
    {
        /// <summary>
        /// Množství peněz na účtě
        /// </summary>
        public double MoneyOnAccount { get; set; }
        /// <summary>
        /// Počet zakázek firmy
        /// </summary>
        public int NumberOfOrders { get; set; }
        /// <summary>
        /// Počet zaměstnanců firmy
        /// </summary>
        public int NumberOfEmployee { get; set; }

        /// <summary>
        /// Kontruktor přehledu firmy
        /// </summary>
        /// <param name="account">Množství peněz na účtě</param>
        /// <param name="numberOrders">Počet zakázek firmy</param>
        /// <param name="numberEmployee">Počet zaměstnanců firmy</param>
        public OverviewFactory(double account, int numberOrders, int numberEmployee)
        {
            MoneyOnAccount = account;
            NumberOfOrders = numberOrders;
            NumberOfEmployee = numberEmployee;
        }

        /// <summary>
        /// Konstruktor pro chod serializéru
        /// </summary>
        public OverviewFactory()
        {
        }

    }
}
