using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory
{
    /// <summary>
    /// Třída reprezentuje jednotlivé materiály
    /// </summary>
    public class Material : INotifyPropertyChanged
    {
        // Delegát určený k aktualizaci množství materiálu
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Seznam výčtových typů všech materiálů
        /// </summary>
        public enum MaterialName { SteelPlate_1mm, SteelPlate_2mm, SteelPlate_3mm, SteelPlate_4mm, SteelPlate_5mm, SteelPlate_6mm, SteelPlate_7mm };
        /// <summary>
        /// Konkrétní typ materiálu
        /// </summary>
        public MaterialName MaterialNames { get; set; }
        /// <summary>
        /// Cena za jeden kus materiálu
        /// </summary>
        public double MaterialPrice { get; set; }
        /// <summary>
        /// Množství materiálu
        /// </summary>
        public int MaterialQuantity { get; set; }
        /// <summary>
        /// Datum nákupu materiálu
        /// </summary>
        public DateTime PurchaseDateTime { get;  set; }

        /// <summary>
        /// Konstruktor pro nově vzniklý materiál
        /// </summary>
        /// <param name="materialNames">Jméno materiálu</param>
        /// <param name="price">Cena za kus</param>
        /// <param name="materialQuantity">Množství materiálu</param>
        public Material(MaterialName materialNames, double price, int materialQuantity)
        {
            MaterialNames = materialNames;
            MaterialPrice = price;
            MaterialQuantity = materialQuantity;
            PurchaseDateTime = DateTime.Now;
        }

        /// <summary>
        /// Konstruktor pro materiál vzniklí tříděním kolekcí
        /// </summary>
        /// <param name="materialNames">Jméno materiálu</param>
        /// <param name="price">Cena za kus</param>
        /// <param name="materialQuantity">Množství materiálu</param>
        /// <param name="purchaseDateTime">Datum nákupu materiálu</param>
        public Material(MaterialName materialNames, double price, int materialQuantity, DateTime purchaseDateTime)
        {
            MaterialNames = materialNames;
            MaterialPrice = price;
            MaterialQuantity = materialQuantity;
            PurchaseDateTime = purchaseDateTime;
        }

        /// <summary>
        /// Konstruktor pro chod serializéru
        /// </summary>
        public Material()
        {

        }        

        /// <summary>
        /// Metoda aktualizuje množství konkrétního materiálu.
        /// </summary>
        /// <param name="events"></param>
        public void MaterialChange(string events)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(events));
        }

        /// <summary>
        /// Přepsání metody
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string viewName = "";
            string[] viewNameHelp = this.MaterialNames.ToString().Split('_');
            foreach (string word in viewNameHelp)
            {
                viewName += word;
                viewName += " ";
            }

            return String.Format("{0}x {1}", MaterialQuantity, viewName);
        }
    }
}
