using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Factory
{
    /// <summary>
    /// Třída reprezentuje jednotlivé zakázky
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// Název zakázky
        /// </summary>
        public string ContractName { get; set; }
        /// <summary>
        /// Jméno zadavatele zakázky
        /// </summary>
        public string SubmitterName { get; set; }
        /// <summary>
        /// Počet kusů materiálu, který je potřeba ke splnění zakázky
        /// </summary>
        public int Pieces { get; set; }
        /// <summary>
        /// Odměna za splnění zakázky
        /// </summary>
        public int Reward { get; set; }
        /// <summary>
        /// Seznam výčtových typů pro druhy materiálů
        /// </summary>
        public enum KindMaterial { SteelPlate_1mm, SteelPlate_2mm, SteelPlate_3mm, SteelPlate_4mm, SteelPlate_5mm, SteelPlate_6mm, SteelPlate_7mm };
        /// <summary>
        /// Druh materiálu, který je potřeba na slpnění zakázky
        /// </summary>
        public KindMaterial KindMaterials { get; set; }
        /// <summary>
        /// Datum zadání zakázky
        /// </summary>
        public DateTime OrderDateTime { get; set; }
        /// <summary>
        /// Stáří zakázky ve dnech
        /// </summary>
        public int TimeSinceAdding
        {
            get
            {
                TimeSpan timeSinceAdding = DateTime.Now - OrderDateTime;
                int days = (int)timeSinceAdding.TotalDays;
                return days;
            }
        }

        /// <summary>
        /// Konstruktor pro nově vzniklé zakázky
        /// </summary>
        /// <param name="name">Název zakázky</param>
        /// <param name="submitter">Jméno zadavatele zakázky</param>
        /// <param name="pieces">Počet kusů materiálu, který je potřeba ke splnění zakázky</param>
        /// <param name="reward">Odměna za splnění zakázky</param>
        /// <param name="kindOfMaterial">Druh materiálu, který je potřeba na slpnění zakázky</param>
        public Contract(string name, string submitter, int pieces,int reward, KindMaterial kindOfMaterial)
        {
            ContractName = name;
            SubmitterName = submitter;
            Pieces = pieces;
            Reward = reward;
            this.KindMaterials = kindOfMaterial;
            OrderDateTime = DateTime.Now;
        }

        /// <summary>
        /// Konstruktor pro zakázky vzniklé tříděním dat
        /// </summary>
        /// <param name="name">Název zakázky</param>
        /// <param name="submitter">Jméno zadavatele zakázky</param>
        /// <param name="pieces">Počet kusů materiálu, který je potřeba ke splnění zakázky</param>
        /// <param name="reward">Odměna za splnění zakázky</param>
        /// <param name="kindOfMaterial">Druh materiálu, který je potřeba na slpnění zakázky</param>
        /// <param name="orderDateTime">Doba přijetí zakázky</param>
        public Contract(string name, string submitter, int pieces, int reward, KindMaterial kindOfMaterial, DateTime orderDateTime)
        {
            ContractName = name;
            SubmitterName = submitter;
            Pieces = pieces;
            Reward = reward;
            this.KindMaterials = kindOfMaterial;
            OrderDateTime = orderDateTime;
        }

        /// <summary>
        /// Konstruktor pro chod serializéru
        /// </summary>
        public Contract()
        {
        }

        /// <summary>
        /// Přepsání metody
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ContractName;
        }

    }
}
