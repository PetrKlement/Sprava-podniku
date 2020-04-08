using Lw.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Factory
{
    /// <summary>
    /// Třída slouží jako Model pro celý projekt.
    /// </summary>
    public class FactoryModel
    {
        /// <summary>
        /// Přehled pro celou společnost.
        /// </summary>
        public OverviewFactory OverviewModel { get; set; } = new OverviewFactory();
        /// <summary>
        /// Kolekce aktuálních materiálů
        /// </summary>
        public ObservableCollection<Material> MaterialCollection { get; set; } = new ObservableCollection<Material>();
        /// <summary>
        /// Kolekce aktuálních zaměstnanců
        /// </summary>
        public ObservableCollection<Employee> EmployeeCollection { get; set; } = new ObservableCollection<Employee>();
        /// <summary>
        /// Kolekce aktuálních zakázek
        /// </summary>
        public ObservableCollection<Contract> ContractCollection { get; set; } = new ObservableCollection<Contract>();
        /// <summary>
        /// Kolekce smazaných materiálů
        /// </summary>

        public ObservableCollection<Material> DeletedMaterialCollection { get; set; } = new ObservableCollection<Material>();
        /// <summary>
        /// Kolekce propuštěných zaměstnanců
        /// </summary>
        public ObservableCollection<Employee> DeletedEmployeeCollection { get; set; } = new ObservableCollection<Employee>();
        /// <summary>
        /// Kolekce provedených zakázek
        /// </summary>
        public ObservableCollection<Contract> DeletedContractCollection { get; set; } = new ObservableCollection<Contract>();
        /// <summary>
        /// Kolekce všech materiálů
        /// </summary>
        public ObservableCollection<Material> AllMaterialCollection { get; set; } = new ObservableCollection<Material>();
        /// <summary>
        /// Kolekce všech zaměstnanců
        /// </summary>
        public ObservableCollection<Employee> AllEmployeeCollection { get; set; } = new ObservableCollection<Employee>();
        /// <summary>
        /// Kolekce všech zakázek
        /// </summary>
        public ObservableCollection<Contract> AllContractCollection { get; set; } = new ObservableCollection<Contract>();
        /// <summary>
        /// Kolekce všech finančních transakcí firmy
        /// </summary>
        public ObservableCollection<Transaction> TransactionCollection { get; set; } = new ObservableCollection<Transaction>();

        // Cesty pro ukládání kolekcí do souborů xaml 
        private string materialTrack = "materials.xml";
        private string employeeTrack = "employees.xml";
        private string contractTrack = "contracts.xml";
        private string deletedMaterialTrack = "deletedMaterials.xml";
        private string deletedEmployeeTrack = "deletedEmployees.xml";
        private string deletedContractTrack = "deletedContracts.xml";
        private string overviewTrack = "overview.xml";
        private string transactionTrack = "transaction.xml";
        /// <summary>
        /// Vlastnost vrací skutečný čas.
        /// </summary>
        public DateTime ActualTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Bezparametrický třídní konstruktor
        /// </summary>
        public FactoryModel()
        {
        }
        #region Zakázky / Zaměstnanci / Materiál
        /// <summary>
        /// Metoda, při splnění zakázky, odstraní materiál, který jí odpovídá.
        /// </summary>
        /// <param name="selectedContract">Vybraná zakázka</param>
        public void ContractMaterial(Contract selectedContract)
        {
            // Materiál ke smazání
            Material deletetMaterial = null;
            bool materialExist = false;
            foreach (Material material in MaterialCollection)
            {
                // Pokud je na skladě materiál, který je potřebný pro splnění zakázky, jeho množství se sníží.
                if ((selectedContract.KindMaterials.ToString() == material.MaterialNames.ToString()) && (material.MaterialQuantity >= selectedContract.Pieces))
                {
                    material.MaterialQuantity -= selectedContract.Pieces;
                    DeletedMaterialCollection.Add(new Material(material.MaterialNames, material.MaterialPrice, selectedContract.Pieces));
                    materialExist = true;
                }
                // Pokud materiál potřebný ke splnění zakázky není v dostatečném množství je vyvolána výjimka.
                else if ((selectedContract.KindMaterials.ToString() == material.MaterialNames.ToString()) && (material.MaterialQuantity < selectedContract.Pieces))
                    throw new ArgumentException("Lack of material");
                // Pokud je množství materiálu nulové, tak je označen pro pozdější smazání.
                else if (material.MaterialQuantity == 0)
                    deletetMaterial = material;          
                    material.MaterialChange("MaterialQuantity");               
            }
            // Podmínka je splněna, pokud typ potřebného materiálu neexistuje.
            if (!materialExist)
            throw new ArgumentException("Material doesn't exist");
            // Smazání materiálu s nulovím množstvím
            MaterialCollection.Remove(deletetMaterial);
            // Uložení změn
            Save();
        }

        /// <summary>
        /// Metoda smaže materiál určený uživatelem.
        /// </summary>
        /// <param name="materialMarked"></param>
        public void DestroyMaterial(Material materialMarked)
        {
            // Materiál vybraný ke smazání
            Material selectMaterial = null;
            bool materialExist = false;
            foreach (Material material in MaterialCollection)
            {
                //Sníží množství materiálu o 1, když je ho dostatek na skladě.
                if ((materialMarked == material) && (material.MaterialQuantity > 1))
                {
                    material.MaterialQuantity--;
                    DeletedMaterialCollection.Add(new Material(material.MaterialNames, material.MaterialPrice, 1));//////
                    materialMarked.MaterialChange("MaterialQuantity");
                    materialExist = true;
                }  
                // Pokud jde o poslední kus materiálu, je označen pro smazání z kolekce celý jeho typ.
               else if ((materialMarked.MaterialNames.ToString() == material.MaterialNames.ToString()) && (material.MaterialQuantity == 1))
               {
                    selectMaterial = material;
                    DeletedMaterialCollection.Add(new Material(material.MaterialNames, material.MaterialPrice, 1));//////
               }              
            }
            // Odstranění materiálu z kolekce.
            if ((selectMaterial != null) && (materialExist == false))
                MaterialCollection.Remove(selectMaterial);
            // Uložení změn
            Save();
        }

        /// <summary>
        /// Metoda přidá materiál do kolekce materiálů na skladě a z jeho ceny a množství vytvoří novou transakci.
        /// </summary>
        /// <param name="materialName">Typ materiálu</param>
        /// <param name="price">Cana za kus</param>
        /// <param name="quantity">Množství materiálu</param>
        public void AddMaterial(Material.MaterialName materialName, double price, int quantity)
        {
            Material selectMaterial = null;
            bool materialFound = false;
            foreach (Material material in MaterialCollection)
            {
               // Pokud daný typ materiálů existuje a má stejnou cenu, tak se zvýší jeho množství.
                if ((materialName == material.MaterialNames) && (material.MaterialPrice == price))
                {
                    material.MaterialQuantity += quantity;
                    material.MaterialChange("MaterialQuantity");
                    materialFound = true;
                    TransactionCollection.Add(new Transaction(material.MaterialNames.ToString(), Transaction.TransactionType.MaterilAdd, (price * quantity)));
                }
                // Pokud daný typ materiálů existuje a nemá stejnou cenu, tak se zvýší jeho množství a zprůměruje cena.
                else if (((materialName == material.MaterialNames) && (material.MaterialPrice != price)))
                {
                    // newPrice je nová cena za daný typ materiálu.
                    double newPrice = ((material.MaterialPrice * material.MaterialQuantity) + (price * quantity)) / (material.MaterialQuantity + quantity);  
                    int newQuantity = material.MaterialQuantity + quantity;
                    material.MaterialPrice = newPrice;
                    material.MaterialQuantity = newQuantity;
                    material.MaterialChange("MaterialQuantity");
                    material.MaterialChange("MaterialPrice");
                    materialFound = true;
                    TransactionCollection.Add(new Transaction(material.MaterialNames.ToString(), Transaction.TransactionType.MaterilAdd, (price * quantity)));
                }
                // Pokud koupený typ materiálu není na skladě, tak je vytvořen
                else if (materialName != material.MaterialNames)
                {
                    selectMaterial = new Material(materialName, price, quantity);                  
                }
            }
            // Přidání nového typu materiálu
            if ((selectMaterial != null) && (materialFound == false))
            MaterialCollection.Add(new Material(selectMaterial.MaterialNames, selectMaterial.MaterialPrice, selectMaterial.MaterialQuantity));
            else if (MaterialCollection.Count == 0)
                MaterialCollection.Add(new Material(materialName, price, quantity));
            // Uložení změn
            Save();            
        }

        /// <summary>
        /// Metoda přidá novou zakázku.
        /// </summary>
        /// <param name="name">Jméno zakázky</param>
        /// <param name="submitter">Jméno zákazníka</param>
        /// <param name="pieces">Počet kusů</param>
        /// <param name="reward">Odměna za provedenou práci</param>
        /// <param name="material">Typ materiálu</param>
        public void AddContract(string name, string submitter, int pieces, int reward, Contract.KindMaterial material)
        {
            ContractCollection.Add(new Contract(name, submitter, pieces, reward, material));
            // Aktualizace OverviewFactory
            RefreshOverviewFactory();
            Save();
        }

        /// <summary>
        /// Metoda odstraní zakázku a pokud byla standartně provedena, vytvoří i novou transakci.
        /// </summary>
        /// <param name="selectedContract">Vybraná zakázka</param>
        /// <param name="workPrice">Cena za provedení zakázky</param>
        /// <param name="changeAccountCollection">Potvrzení, jesti má být vytvořena nová transakce</param>
        public void DeleteContract(Contract selectedContract, double? workPrice, bool changeAccountCollection)
        {
            if (changeAccountCollection)
                TransactionCollection.Add(new Transaction(selectedContract.ContractName.ToString(), Transaction.TransactionType.ContractAdd, (selectedContract.Reward - (double)workPrice)));
            // Smazání zakáky
            ContractCollection.Remove(selectedContract);           
            DeletedContractCollection.Add(new Contract(selectedContract.ContractName, selectedContract.SubmitterName, selectedContract.Pieces, selectedContract.Reward, selectedContract.KindMaterials));
            RefreshOverviewFactory();
            Save();
        }

        /// <summary>
        /// Metoda přidá nového zaměstnance.
        /// </summary>
        /// <param name="name">Jméno a příjmení</param>
        /// <param name="wages">Mzda</param>
        public void AddEmployee(string name, double wages)
        {
            EmployeeCollection.Add(new Employee(name, wages));
            RefreshOverviewFactory();
            Save();
        }

        /// <summary>
        /// Metoda přesune zaměstnance z EmployeeCollection do DeletedEmployeeCollection.
        /// </summary>
        /// <param name="employee">Zvolený zaměstnanec</param>
        public void DeleteEmployee(Employee employee)
        {
            EmployeeCollection.Remove(employee);
            DeletedEmployeeCollection.Add(new Employee(employee.Name, employee.Wages));
            RefreshOverviewFactory();
            Save();
        }
        #endregion      

        #region Uložení / Načtení
        /// <summary>
        /// Metoda uloží kolekce v modelu do XAML souborů (MaterialCollection, EmployeeCollection, ContractCollection, OverviewModel, TransactionCollection, DeletedMaterialCollection, DeletedEmployeeCollection, DeletedContractCollection).
        /// </summary>
        public void Save()
        {
            // K ukládání je využit serializér
            XmlSerializer materialSerializer = new XmlSerializer(MaterialCollection.GetType());
            using (StreamWriter sw = new StreamWriter(materialTrack))
            {
                materialSerializer.Serialize(sw, MaterialCollection);
            }
            XmlSerializer employeeSerializer = new XmlSerializer(EmployeeCollection.GetType());
            using (StreamWriter sw = new StreamWriter(employeeTrack))
            {
                employeeSerializer.Serialize(sw, EmployeeCollection);
            }
            XmlSerializer contractSerializer = new XmlSerializer(ContractCollection.GetType());
            using (StreamWriter sw = new StreamWriter(contractTrack))
            {
                contractSerializer.Serialize(sw, ContractCollection);
            }

            XmlSerializer overviwSerializer = new XmlSerializer(OverviewModel.GetType());
            using (StreamWriter sw = new StreamWriter(overviewTrack))
            {
                overviwSerializer.Serialize(sw, OverviewModel);
            }
            XmlSerializer accountSerializer = new XmlSerializer(TransactionCollection.GetType());
            using (StreamWriter sw = new StreamWriter(transactionTrack))
            {
                accountSerializer.Serialize(sw, TransactionCollection);
            }

            XmlSerializer deletedMaterialSerializer = new XmlSerializer(DeletedMaterialCollection.GetType());
            using (StreamWriter sw = new StreamWriter(deletedMaterialTrack))
            {
                deletedMaterialSerializer.Serialize(sw, DeletedMaterialCollection);
            }
            XmlSerializer deletedEmployeeSerializer = new XmlSerializer(DeletedEmployeeCollection.GetType());
            using (StreamWriter sw = new StreamWriter(deletedEmployeeTrack))
            {
                deletedEmployeeSerializer.Serialize(sw, DeletedEmployeeCollection);
            }
            XmlSerializer deletedContractSerializer = new XmlSerializer(DeletedContractCollection.GetType());
            using (StreamWriter sw = new StreamWriter(deletedContractTrack))
            {
                deletedContractSerializer.Serialize(sw, DeletedContractCollection);
            }
            FillingAllCollections();
        }

        /// <summary>
        /// Metoda naplní kolekce v modelu z XAML souborů (MaterialCollection, EmployeeCollection, ContractCollection, OverviewModel, TransactionCollection, DeletedMaterialCollection, DeletedEmployeeCollection, DeletedContractCollection).
        /// </summary>
        public void Load()
        {
            // Využití deserializace
            XmlSerializer materialSerializer = new XmlSerializer(MaterialCollection.GetType());
            if (File.Exists(materialTrack))
            {
                using (StreamReader sr = new StreamReader(materialTrack))
                {
                    MaterialCollection = (ObservableCollection<Material>)materialSerializer.Deserialize(sr);
                }
            }
            else
                MaterialCollection = new ObservableCollection<Material>();
            XmlSerializer employeeSerializer = new XmlSerializer(EmployeeCollection.GetType());
            if (File.Exists(employeeTrack))
            {
                using (StreamReader sr = new StreamReader(employeeTrack))
                {
                    EmployeeCollection = (ObservableCollection<Employee>)employeeSerializer.Deserialize(sr);
                }
            }
            else
                EmployeeCollection = new ObservableCollection<Employee>();
            XmlSerializer contractSerializer = new XmlSerializer(ContractCollection.GetType());
            if (File.Exists(contractTrack))
            {
                using (StreamReader sr = new StreamReader(contractTrack))
                {
                    ContractCollection = (ObservableCollection<Contract>)contractSerializer.Deserialize(sr);
                }
            }
            else
                ContractCollection = new ObservableCollection<Contract>();

            XmlSerializer overviewSerializer = new XmlSerializer(OverviewModel.GetType());
            if (File.Exists(overviewTrack))
            {
                using (StreamReader sr = new StreamReader(overviewTrack))
                {
                    OverviewModel = (OverviewFactory)overviewSerializer.Deserialize(sr);
                }
            }
            else
                OverviewModel = new OverviewFactory();
            XmlSerializer accountSerializer = new XmlSerializer(TransactionCollection.GetType());
            if (File.Exists(transactionTrack))
            {
                using (StreamReader sr = new StreamReader(transactionTrack))
                {
                    TransactionCollection = (ObservableCollection<Transaction>)accountSerializer.Deserialize(sr);
                }
            }
            else
                TransactionCollection = new ObservableCollection<Transaction>();
            RearrangeMaterialCollection();

            XmlSerializer deletedMaterialSerializer = new XmlSerializer(DeletedMaterialCollection.GetType());
            if (File.Exists(deletedMaterialTrack))
            {
                using (StreamReader sr = new StreamReader(deletedMaterialTrack))
                {
                    DeletedMaterialCollection = (ObservableCollection<Material>)deletedMaterialSerializer.Deserialize(sr);
                }
            }
            else
                DeletedMaterialCollection = new ObservableCollection<Material>();
            XmlSerializer deletedEmployeeSerializer = new XmlSerializer(DeletedEmployeeCollection.GetType());
            if (File.Exists(deletedEmployeeTrack))
            {
                using (StreamReader sr = new StreamReader(deletedEmployeeTrack))
                {
                    DeletedEmployeeCollection = (ObservableCollection<Employee>)deletedEmployeeSerializer.Deserialize(sr);
                }
            }
            else
                DeletedEmployeeCollection = new ObservableCollection<Employee>();
            XmlSerializer deletedEontractSerializer = new XmlSerializer(DeletedContractCollection.GetType());
            if (File.Exists(deletedContractTrack))
            {
                using (StreamReader sr = new StreamReader(deletedContractTrack))
                {
                    DeletedContractCollection = (ObservableCollection<Contract>)deletedEontractSerializer.Deserialize(sr);
                }
            }
            else
                DeletedContractCollection = new ObservableCollection<Contract>();
            FillingAllCollections();
        }
        #endregion

        #region Práce s kolekcemi

        /// <summary>
        /// Metoda smaže a poté naplní  AllMaterialCollection z MaterialCollection a DeletedMaterialCollection, AllContractCollection z ContractCollection a DeletedContractCollection a AllEmployeeCollection z EmployeeCollection a DeletedEmployeeCollection.
        /// </summary>
        private void FillingAllCollections()
        {
            // Vyčištění kolekcí
            AllMaterialCollection.Clear();
            AllContractCollection.Clear();
            AllEmployeeCollection.Clear();
            //Opětovné naplnění kolekcí
            foreach (Material material in MaterialCollection)
            {
                AllMaterialCollection.Add(material);
            }
            foreach (Material material in DeletedMaterialCollection)
            {
                AllMaterialCollection.Add(material);
            }
            foreach (Contract contract in ContractCollection)
            {
                AllContractCollection.Add(contract);
            }
            foreach (Contract contract in DeletedContractCollection)
            {
                AllContractCollection.Add(contract);
            }
            foreach (Employee employee in EmployeeCollection)
            {
                AllEmployeeCollection.Add(employee);
            }
            foreach (Employee employee in DeletedEmployeeCollection)
            {
                AllEmployeeCollection.Add(employee);
            }
        }
        /// <summary>
        /// Metoda aktualizuje počet zaměstnanců a zakázek v OverviewModel. 
        /// </summary>
        public void RefreshOverviewFactory()
        {
            OverviewModel.NumberOfEmployee = EmployeeCollection.Count;
            OverviewModel.NumberOfOrders = ContractCollection.Count;
            Save();
        }
        /// <summary>
        /// Metoda seřadí materiáli v MaterialCollection podle abecedy.
        /// </summary>
        public void RearrangeMaterialCollection()
        {
            // Do helpMaterialCollection jsou uloženy seřazené materiály z MaterialCollection
            IEnumerable<Material> helpMaterialCollection = MaterialCollection.OrderBy(a => a.MaterialNames.ToString());
            MaterialCollection = new ObservableCollection<Material>();
            foreach (Material material in helpMaterialCollection)
            {
                MaterialCollection.Add(new Material(material.MaterialNames, material.MaterialPrice, material.MaterialQuantity));
            }
        }
        #endregion

        /// <summary>
        /// Metoda přidá nebo odebere peníze z účtu firmy.
        /// </summary>
        /// <param name="addedMoney">Přidané peníze</param>
        /// <param name="removedMoney">Odebrané peníze</param>
        public void AddOrRemoveMoney(double addedMoney, double removedMoney)
        {
            // Změna množství peněz an účtě
            OverviewModel.MoneyOnAccount += addedMoney - removedMoney;
            // Vytvoření transakcí
            if (addedMoney != 0)
                TransactionCollection.Add(new Transaction("The owner added money.", Transaction.TransactionType.OwnerAdd, addedMoney));
            if (removedMoney != 0)
                TransactionCollection.Add(new Transaction("The owner removed money.", Transaction.TransactionType.OwnerRemowed, removedMoney));
            Save();
        }
    }
}
