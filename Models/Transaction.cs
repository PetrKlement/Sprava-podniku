using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory
{
    /// <summary>
    /// Třída reprezentuje jednotlivé transakce
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Popisek transakce
        /// </summary>
        public string Headline { get; set; }
        /// <summary>
        /// Čas provedení transakce
        /// </summary>
        public DateTime TransactionTime { get; set; }
        /// <summary>
        /// Výčtový typ pro druhy transakcí
        /// </summary>
        public enum TransactionType { OwnerAdd, OwnerRemowed, MaterilAdd, ContractAdd };
        /// <summary>
        /// Typ konkrétní transakce
        /// </summary>
        public TransactionType TypeOfTransaction { get; set; }
        /// <summary>
        /// Množství peněz, které byly při transakci převedeny
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// Konstruktor pro nově vytvořené transakce
        /// </summary>
        /// <param name="headLine">Popisek transakce</param>
        /// <param name="typeOfTransaction">Typ konkrétní transakce</param>
        /// <param name="money">Množství peněz, které byly při transakci převedeny</param>
        public Transaction(string headLine, TransactionType typeOfTransaction, double money)
        {
            Headline = headLine;
            TypeOfTransaction = typeOfTransaction;
            TransactionTime = DateTime.Now;
            Money = money;
        }

        /// <summary>
        /// Konstruktor pro transakce vzniklé zpracováním údajů
        /// </summary>
        /// <param name="headLine">Popisek transakce</param>
        /// <param name="typeOfTransaction">Typ konkrétní transakce</param>
        /// <param name="money">Množství peněz, které byly při transakci převedeny</param>
        /// <param name="transactionTime">Čas ve kterém transakce proběhla</param>
        public Transaction(string headLine, TransactionType typeOfTransaction, double money, DateTime transactionTime)
        {
            Headline = headLine;
            TypeOfTransaction = typeOfTransaction;
            TransactionTime = transactionTime;
            Money = money;
        }

        /// <summary>
        /// Konstruktor pro chod serializéru
        /// </summary>
        public Transaction()
        {
        }
    }
}
