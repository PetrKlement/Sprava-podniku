using Lw;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using Path = System.IO.Path;
using Microsoft.Win32;
using Draw = System.Drawing;
using System.Windows.Input;

namespace Factory
{
    /// <summary>
    /// Třída validuje vstupní data a zároveň funguje jako ViewModel
    /// </summary>
    public class FactoryValidator : INotifyPropertyChanged
    {
        /// <summary>
        ///Časovač pro zobrazení aktuálního času v aplikaci 
        /// </summary>
        private DispatcherTimer timer;
        /// <summary>
        /// Návratová hodnota aktuálního času 
        /// </summary>
        public DateTime actualTime= DateTime.Now;
        /// <summary>
        /// Aktuální čas 
        /// </summary>
        public DateTime ActualTime
        { get { return actualTime; }
            set
            {
                actualTime = value;
                CallChange(nameof(ActualTime));
            }
        }
        /// <summary>
        /// Hodnota podle které TimeCheck() pozná, že má vyvolat uálost. 
        /// </summary>
        public int previousSeconds;
        /// <summary>
        /// Náklady na hodinu práce jednodo dělníka 
        /// </summary>
        public double? hourCosts;
        /// <summary>
        /// Cena materiálu 
        /// </summary>
        public double? materialCost;
        /// <summary>
        /// Celková finanční náklady na splnění konkrétní zakázky. 
        /// </summary>
        public double? workPrice;
        /// <summary>
        /// Typ třídy, kterou aktuálně zobrazuje Canvas
        /// </summary>
        private string classType;
        /// <summary>
        /// List, ve které jsou uloženy jednotlivé typy materiálů
        /// </summary>
        public List<string> MaterialString { get; set; } = new List<string>();
        private string[] materialTypes = { "SteelPlate 1mm", "SteelPlate 2mm", "SteelPlate 3mm", "SteelPlate 4mm", "SteelPlate 5mm", "SteelPlate 6mm", "SteelPlate 7mm" };
        /// <summary>
        /// Delegát určený k aktualizaci zobrazovaných dat 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Přehled počtu zaměstnanců, počtu zakázek a peněz na účtě. 
        /// </summary>
        public OverviewFactory VMOverview { get; set; }
        /// <summary>
        /// Instance třídy FactoryModel
        /// </summary>
        public FactoryModel factoryModel  = new FactoryModel();
         /// <summary>
         /// Kolekce aktuálních materiálů, která je načtena při spuštění programu.
         /// </summary>
        public ObservableCollection<Material> VMMaterialCollection { get; set; }
        /// <summary>
        /// Kolekce aktuálních zaměstnanců, která je načtena při spuštění programu.
        /// </summary>
        public ObservableCollection<Employee> VMEmployeeCollection { get; set; }
        /// <summary>
        /// Kolekce aktuálních zakázek, která je načtena při spuštění programu.
        /// </summary>
        public ObservableCollection<Contract> VMContractCollection { get; set; }
       /// <summary>
       /// Kolekce smazaných materiálů
       /// </summary>
        public ObservableCollection<Material> VMDeletedMaterialCollection { get; set; } = new ObservableCollection<Material>();
        /// <summary>
        /// Kolekce smazaných zaměstnanců
        /// </summary>
        public ObservableCollection<Employee> VMDeletedEmployeeCollection { get; set; } = new ObservableCollection<Employee>();
        /// <summary>
        /// Kolekce smazaných zakázek
        /// </summary>
        public ObservableCollection<Contract> VMDeletedContractCollection { get; set; } = new ObservableCollection<Contract>();
        /// <summary>
        /// Kolekce všech materiálů
        /// </summary>
        public ObservableCollection<Material> VMAllMaterialCollection { get; set; } = new ObservableCollection<Material>();
        /// <summary>
        /// Kolekce všech zaměstnanců
        /// </summary>
        public ObservableCollection<Employee> VMAllEmployeeCollection { get; set; } = new ObservableCollection<Employee>();
        /// <summary>
        /// Kolekce všech zakázek
        /// </summary>
        public ObservableCollection<Contract> VMAllContractCollection { get; set; } = new ObservableCollection<Contract>();
        /// <summary>
        /// Kolekce aktuálních materiálů
        /// </summary>
        public ObservableCollection<Material> VMActualMaterialCollection { get; set; } = new ObservableCollection<Material>();
        /// <summary>
        /// Kolekce aktuálních zaměstnanců
        /// </summary>
        public ObservableCollection<Employee> VMActualEmployeeCollection { get; set; } = new ObservableCollection<Employee>();
        /// <summary>
        /// Kolekce aktuálních zakázek
        /// </summary>
        public ObservableCollection<Contract> VMActualContractCollection { get; set; } = new ObservableCollection<Contract>();
        /// <summary>
        /// Kolekce všech transakcí, která je načtena při spuštění programu.
        /// </summary>
        public ObservableCollection<Transaction> VMTransactionCollection { get; set; } = new ObservableCollection<Transaction>();
        /// <summary>
        /// Kolekce vytříděných transakcí, které jsou zobrázovány uživateli podle hodnot, které zadá.
        /// </summary>
        public ObservableCollection<Transaction> VMSortTransactionCollection { get; set; } = new ObservableCollection<Transaction>();
        /// <summary>
        /// Kolekce hodnot, které zobrazuje SelectedListBox v OverviewWindow.xaml
        /// </summary>
        public ObservableCollection<string> VMListBoxCollection { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Kontruktor třídy FactoryValidator
        /// </summary>
        public FactoryValidator()
        {
            // Naplnění Listu typy materiálů
            MaterialString.AddRange(materialTypes);
            // Obsluha změny zobrazeného času
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
            timer.Tick += TimeCheck;
            timer.Start();
        }

        /// <summary>
        /// Metoda zajistí aktualizaci zobrazeného času.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TimeCheck(object sender, EventArgs e)
        {
            if (DateTime.Now.Second != previousSeconds)
            {
                ActualTime = DateTime.Now;
                previousSeconds = DateTime.Now.Second;
            }
        }

        /// <summary>
        /// Metoda spočítá náklady na zpracování zakázky. 
        /// </summary>
        /// <param name="time">Doba, jak dlouho trvá provedení zakázky(v hodinách).</param>
        /// <param name="otherCosts">Další náklady spojené s provedením zakázky</param>
        /// <param name="worker">Vybraný pracovník</param>
        /// <param name="materialIdex">Index, podle kterého metoda pozná jakéhý materiál je na zakázku potřeba</param>
        /// <param name="pieces">Počet kusů</param>
        /// <returns></returns>
        public double CalculateCosts(string time, string otherCosts, string worker, int materialIdex, string pieces)
        {
            materialCost = null;
            hourCosts = null;
            workPrice = null;
            // Vybrání typu materiálu podle indexu v MaterialComboBox
            Material selectedMaterial = VMMaterialCollection[materialIdex];
            foreach (Employee employee in VMEmployeeCollection)
            {
                // Vyhledání zaměstnance podle jména a jeho ceny za hodinu práce
                if (employee.Name == worker )
                    hourCosts = employee.HourlyWage;                
            }
            materialCost = selectedMaterial.MaterialPrice;
            if (hourCosts == null)
                throw new ArgumentException("Worker doesn't exist.");
            if (materialCost == null)
                throw new ArgumentException("Material doesn't exist.");
            workPrice = double.Parse(time) * (double)hourCosts + double.Parse(otherCosts) + (double)materialCost * double.Parse(pieces);
            if (workPrice == null)
                throw new ArgumentException("You didn't numerare.");
            return (double)workPrice;             
        }

        #region Zakázky / Zaměstnanci / Materiál
        /// <summary>
        /// Metoda zpracovává přidání nebo odebrání peněz z účtu.
        /// </summary>
        /// <param name="add">Přidané peníze</param>
        /// <param name="remove">Odebrané peníze</param>
        public void VMAddOrRemoveMoney(string add, string remove)
        {
            if (!double.TryParse(add, out double addedMoney))
                throw new ArgumentException("No number.");
            if (!double.TryParse(remove, out double removedMoney))
                throw new ArgumentException("No number.");
            // Volání metody, která transakci provede
            factoryModel.AddOrRemoveMoney(addedMoney, removedMoney);
            //Aktualizace zobrazených dat
            CallChange("VMOverview");
        }

        /// <summary>
        /// Metoda volá metodu, která odstraní materiál, který byl použit na zakázku a uloží tuto transakci. 
        /// </summary>
        /// <param name="selectedContract">Kontrakt který je zpracováván</param>
        public void VMContractMaterial(Contract selectedContract)
        {
            factoryModel.ContractMaterial(selectedContract);
            //Aktualizace zobrazených dat
            CallChange("VMMaterialCollection");
            CallChange("VMOverview");
        }

        /// <summary>
        /// Metoda volá metodu, která zničí materiál.
        /// </summary>
        /// <param name="material"></param>
        public void VMDestroyMaterial(Material material)
        {
            if (material == null)
                throw new ArgumentException("No material");
            factoryModel.DestroyMaterial(material);
            //Aktualizace zobrazených dat
            CallChange("VMMaterialCollection");
            CallChange("VMAllMaterialCollection");
            CallChange("VMOverview");
        }

        /// <summary>
        /// Metoda zpracuje požadavek na přidání materiálu
        /// </summary>
        /// <param name="material">Typ materiálu</param>
        /// <param name="price">Cena za jeden kus materiálu</param>
        /// <param name="quantity">Množství přidaného materiálu</param>
        public void VMAddMaterial(string material, string price, string quantity)
        {
            double materialCosts;
            Material.MaterialName materials;
            // Převedení typu materiálu ze string do Material.MaterialName
            if (material == "SteelPlate 1mm")
                materials = Material.MaterialName.SteelPlate_1mm;
            else if (material == "SteelPlate 2mm")
                materials = Material.MaterialName.SteelPlate_2mm;
            else if (material == "SteelPlate 3mm")
                materials = Material.MaterialName.SteelPlate_3mm;
            else if (material == "SteelPlate 4mm")
                materials = Material.MaterialName.SteelPlate_4mm;
            else if (material == "SteelPlate 5mm")
                materials = Material.MaterialName.SteelPlate_5mm;
            else if (material == "SteelPlate 6mm")
                materials = Material.MaterialName.SteelPlate_6mm;
            else if (material == "SteelPlate 7mm")
                materials = Material.MaterialName.SteelPlate_7mm;
            // Ošetření chyb na vstupu
            else
                throw new ArgumentException("Material problem.");
          
            if (!double.TryParse(price, out double priceModel))
                throw new ArgumentException("Price isn't number.");
            if (!int.TryParse(quantity, out int quantityModel))
                throw new ArgumentException("Quantity isn't number.");
            materialCosts = priceModel * quantityModel;
            if (materialCosts > VMOverview.MoneyOnAccount)
                throw new ArgumentException("No money on Account.");
            // Pokud je na účtě dostatek peněz odečte je z něj
            else
                VMOverview.MoneyOnAccount -= materialCosts;

            factoryModel.AddMaterial(materials, priceModel, quantityModel);
            VMMaterialCollection = factoryModel.MaterialCollection;
            //Aktualizace zobrazených dat
            CallChange("VMMaterialCollection");
            CallChange("VMOverview");
        }

        /// <summary>
        /// Metoda po zpracování zakázky přesune tuto zakázku mezi smazané.
        /// </summary>
        /// <param name="selectedContract">Vybraná zakázka</param>
        public void VMDeleteContract(Contract selectedContract)
        {
            bool changeAccountCollection = true;
            if (workPrice == null)
                throw new ArgumentException("You didn't numerare.");
            factoryModel.DeleteContract(selectedContract, workPrice, changeAccountCollection);
            // Zněna peněz na účtě
            VMOverview.MoneyOnAccount -= (double)workPrice;
            VMOverview.MoneyOnAccount += (double)selectedContract.Reward;
            //Aktualizace zobrazených dat
            CallChange("VMContractCollection");
            CallChange("VMMaterialCollection");
            CallChange("VMOverview");
        }

        /// <summary>
        /// Metoda smaže zakázku, aniž by byla splněna.
        /// </summary>
        /// <param name="selectedContract">Vybraná zakázka</param>
        public void VMDestroyContract(Contract selectedContract)
        {
            bool changeAccountCollection = false;
            if (selectedContract == null)
                throw new ArgumentException("No contract.");
            factoryModel.DeleteContract(selectedContract, workPrice, changeAccountCollection);
            CallChange("VMContractCollection");
        }

        /// <summary>
        /// Metoda zpracovává požadavek na vytvoření nové zakázky.
        /// </summary>
        /// <param name="name">Jméno zakázky</param>
        /// <param name="submitter">Zadavatel zakázky</param>
        /// <param name="quantity">Množství materiálu an splnění zakázky</param>
        /// <param name="reward">Odměna</param>
        /// <param name="material">Materiál potřebný na zakázku</param>
        public void VMAddContract(string name, string submitter, string quantity, string reward, string material)
        {
            Contract.KindMaterial materials;
            // Převede typ materiálu ze string do Material.MaterialName
            if (material == "SteelPlate 1mm")
                materials = Contract.KindMaterial.SteelPlate_1mm;
            else if (material == "SteelPlate 2mm")
                materials = Contract.KindMaterial.SteelPlate_2mm;
            else if (material == "SteelPlate 3mm")
                materials = Contract.KindMaterial.SteelPlate_3mm;
            else if (material == "SteelPlate 4mm")
                materials = Contract.KindMaterial.SteelPlate_4mm;
            else if (material == "SteelPlate 5mm")
                materials = Contract.KindMaterial.SteelPlate_5mm;
            else if (material == "SteelPlate 6mm")
                materials = Contract.KindMaterial.SteelPlate_6mm;
            else if (material == "SteelPlate 7mm")
                materials = Contract.KindMaterial.SteelPlate_7mm;
            // Ošetření chyb na vstupu
            else
                throw new ArgumentException("Material problem.");

            if (!int.TryParse(reward, out int rewardModel))
                throw new ArgumentException("Price isn't number.");
            if (!int.TryParse(quantity, out int quantityModel))
                throw new ArgumentException("Quantity isn't number.");

            factoryModel.AddContract(name, submitter, quantityModel, rewardModel, materials);
            // Aktualizace zobrazených dat
            CallChange("VMContractCollection");
            CallChange("VMOverview");
        }
       
        /// <summary>
        /// Metoda zpracovává požadavek na přidání nového zaměstnance.
        /// </summary>
        /// <param name="name">Jméno zaměstnance</param>
        /// <param name="surname">Příjmení zaměstnance</param>
        /// <param name="wages">Mzda zaměstnance</param>
        public void VMAddEmployee(string name, string surname, string wages)
        {
            double wage;
            string completName = name + " " + surname;
            if (!double.TryParse(wages, out wage))
                throw new ArgumentException("No wage.");
            factoryModel.AddEmployee(completName, wage);
            // Aktualizace zobrazených dat
            CallChange("VMEmployeeCollection");
            CallChange("VMOverview");
        }

        /// <summary>
        /// Metoda zpracovává požadavek na propuštění zaměstnance. 
        /// </summary>
        /// <param name="employee">Vybraný zaměstnanec</param>
        public void VMDeleteEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentException("No employee");
            factoryModel.DeleteEmployee(employee);
            CallChange("VMEmployeeCollection");
            CallChange("VMOverview");
        }
        #endregion

        #region Obsluha zobrazený dat v OverviewWindow.xaml
        /// <summary>
        /// Metoda slouží k vykreslování OverviewCanvas a SelectedListBox prvků v OverviewWindow.xaml 
        /// </summary>
        /// <param name="canvas">Canvas předaný k vykreslení</param>
        /// <param name="month">Zvolený měsíc</param>
        /// <param name="year">Zvolený rok</param>
        /// <param name="type">Typ informací, které mají být zobrazeny (transakce, materiál, zaměstnanci nebo zakázky)</param>
        /// <param name="classisOverview">Bool proměná určuje, jakým způsobem probíhá filtrace dat podle času</param>
        /// <param name="classicTimeType">Časové období, ze kterého jsou data zobrazena</param>
        public void DrawCanvas(Canvas canvas, int month, string year, string type, bool classisOverview, string classicTimeType)
        {
            classType = type;
            //Smazání prvků v Canvas
            canvas.Children.Clear();
            // Funkce profiltruje data podle zadaných časových údajů.
            FillOverwiewCollection(month, year, classisOverview, classicTimeType);
            // Obdelníky v grafu
            Rectangle FirstRectangle = new Rectangle
            {
                Name = "FirstRectangle",
                Width = 50,
                Height = 0,
                Fill = Brushes.Red
            };            
            Rectangle SecondRectangle = new Rectangle
            {
                Name = "SecondRectangle",
                Width = 50,
                Height = 0,
                Fill = Brushes.Blue
            };
            Rectangle ThirdRectangle = new Rectangle
            {
                Name = "ThirdRectangle",
                Width = 50,
                Height = 0,
                Fill = Brushes.Green
            };
            Rectangle FourthRectangle = new Rectangle
            {
                Name = "FourthRectangle",
                Width = 50,
                Height = 0,
                Fill = Brushes.Green
            };
            // Hodnota vztažená k datům, které reprezentují jednotlivé obdelníky v grafu
            TextBlock FirstNumberTextBlock = new TextBlock
            {
                Text = "0",
                FontSize = 20,
                FontWeight = FontWeights.Black,
               
            };
            TextBlock SecondNumberTextBlock = new TextBlock
            {
                Text = "0",
                FontSize = 20,
                FontWeight = FontWeights.Black,
            };
            TextBlock ThirdNumberTextBlock = new TextBlock
            {
                Text = "0",
                FontSize = 20,
                FontWeight = FontWeights.Black,
            };
            TextBlock FourthNumberTextBlock = new TextBlock
            {
                Text = "0",
                FontSize = 20,
                FontWeight = FontWeights.Black,
            };
            // Popisky jednotlivých sloupců
            TextBlock TextDescribingRectangle1 = new TextBlock
            {
                Text = "ACTUAL",
                FontSize = 15,
            };
            TextBlock TextDescribingRectangle2 = new TextBlock
            {
                Text = "DELETED",
                FontSize = 15,
            };
            TextBlock TextDescribingRectangle3 = new TextBlock
            {
                Text = "WHOLE",
                FontSize = 15,
            };
            TextBlock TextDescribingRectangle4 = new TextBlock
            {
                Text = "",
                FontSize = 15,
            };
            // Linka pod grafem
            Line line = new Line
            {
                X1 = 10,
                Y1 = 300,
                X2 = 440,
                Y2 = 300,
                StrokeThickness = 3,
                Stroke = Brushes.Black,
            };
            // Pokud jsou zobrazovány informace o materiálu
            if (type == "Material")
            {
                // Určení výšky jednotlívých obdelníků v grafu
                if (VMAllMaterialCollection.Count != 0)
                {
                    FirstRectangle.Height = VMActualMaterialCollection.Count * 250 / VMAllMaterialCollection.Count;
                    SecondRectangle.Height = VMDeletedMaterialCollection.Count * 250 / VMAllMaterialCollection.Count;
                    ThirdRectangle.Height = 250;
                }
                // Číselná hodnota vztažena k jednotlivým oddelníkům
                FirstNumberTextBlock.Text = VMActualMaterialCollection.Count.ToString();
                SecondNumberTextBlock.Text = VMDeletedMaterialCollection.Count.ToString();
                ThirdNumberTextBlock.Text = VMAllMaterialCollection.Count.ToString();
            }
            // Pokud jsou zobrazovány informace o zakázkách
            else if (type == "Contract")
            {
                // Určení výšky jednotlívých obdelníků v grafu
                if (VMAllContractCollection.Count != 0)
                {
                    FirstRectangle.Height = VMActualContractCollection.Count * 260 / VMAllContractCollection.Count;
                    SecondRectangle.Height = VMDeletedContractCollection.Count * 260 / VMAllContractCollection.Count;
                    ThirdRectangle.Height = 260;
                }
                // Číselná hodnota vztažena k jednotlivým oddelníkům
                FirstNumberTextBlock.Text = VMActualContractCollection.Count.ToString();
                SecondNumberTextBlock.Text = VMDeletedContractCollection.Count.ToString();
                ThirdNumberTextBlock.Text = VMAllContractCollection.Count.ToString();
            }
            // Pokud jsou zobrazovány informace o zaměstnancích
            else if (type == "Employee")
            {
                // Určení výšky jednotlívých obdelníků v grafu
                if (VMAllEmployeeCollection.Count != 0)
                {
                    FirstRectangle.Height = VMActualEmployeeCollection.Count * 245 / VMAllEmployeeCollection.Count;
                    SecondRectangle.Height = VMDeletedEmployeeCollection.Count * 245 / VMAllEmployeeCollection.Count;
                    ThirdRectangle.Height = 245;
                }
                // Číselná hodnota vztažena k jednotlivým oddelníkům
                FirstNumberTextBlock.Text = VMActualEmployeeCollection.Count.ToString();
                SecondNumberTextBlock.Text = VMDeletedEmployeeCollection.Count.ToString();
                ThirdNumberTextBlock.Text = VMAllEmployeeCollection.Count.ToString();
            }
            // Pokud jsou zobrazovány informace o transakcích
            else if (type == "Overview")
            {
                // Určení výšky jednotlívých obdelníků v grafu
                if ((VMAddUpTransactionCollections(Transaction.TransactionType.MaterilAdd) + VMAddUpTransactionCollections(Transaction.TransactionType.ContractAdd)) != 0)
                {
                    FirstRectangle.Height = VMAddUpTransactionCollections(Transaction.TransactionType.MaterilAdd) * 250 / (VMAddUpTransactionCollections(Transaction.TransactionType.MaterilAdd) + VMAddUpTransactionCollections(Transaction.TransactionType.ContractAdd));
                    SecondRectangle.Height = VMAddUpTransactionCollections(Transaction.TransactionType.ContractAdd) * 250 / (VMAddUpTransactionCollections(Transaction.TransactionType.MaterilAdd) + VMAddUpTransactionCollections(Transaction.TransactionType.ContractAdd));
                }                    
                if ((VMAddUpTransactionCollections(Transaction.TransactionType.OwnerAdd) + VMAddUpTransactionCollections(Transaction.TransactionType.OwnerRemowed)) != 0)
                {
                    ThirdRectangle.Height = VMAddUpTransactionCollections(Transaction.TransactionType.OwnerAdd) * 250 / (VMAddUpTransactionCollections(Transaction.TransactionType.OwnerAdd) + VMAddUpTransactionCollections(Transaction.TransactionType.OwnerRemowed));
                    FourthRectangle.Height = VMAddUpTransactionCollections(Transaction.TransactionType.OwnerRemowed) * 250 / (VMAddUpTransactionCollections(Transaction.TransactionType.OwnerAdd) + VMAddUpTransactionCollections(Transaction.TransactionType.OwnerRemowed));                    
                }
                // Číselná hodnota vztažena k jednotlivým oddelníkům
                FirstNumberTextBlock.Text = VMAddUpTransactionCollections(Transaction.TransactionType.MaterilAdd).ToString();
                SecondNumberTextBlock.Text = VMAddUpTransactionCollections(Transaction.TransactionType.ContractAdd).ToString();
                ThirdNumberTextBlock.Text = VMAddUpTransactionCollections(Transaction.TransactionType.OwnerAdd).ToString();
                FourthNumberTextBlock.Text = VMAddUpTransactionCollections(Transaction.TransactionType.OwnerRemowed).ToString();               
            }            
            // Přidání jedntlivých prvků do Canvas
            canvas.Children.Add(line);
            canvas.Children.Add(FirstRectangle);
            canvas.Children.Add(SecondRectangle);
            canvas.Children.Add(ThirdRectangle);
            canvas.Children.Add(FirstNumberTextBlock);
            canvas.Children.Add(SecondNumberTextBlock);
            canvas.Children.Add(ThirdNumberTextBlock);
            // Pokud není nastaven typ zobrazovaných dat na přehled transakcí, jsou v grafu zobrazeny pouze tři sloupce.
            if (type != "Overview")
            {
                // Přidání jedntlivých prvků do Canvas               
                canvas.Children.Add(TextDescribingRectangle1);
                canvas.Children.Add(TextDescribingRectangle2);
                canvas.Children.Add(TextDescribingRectangle3);
                // Nastavení zarovnání jednotlivý prvků v grafu
                // Zarovnání obdelníků
                Canvas.SetLeft(FirstRectangle, 50);
                Canvas.SetBottom(FirstRectangle, 50);
                Canvas.SetLeft(SecondRectangle, 200);
                Canvas.SetBottom(SecondRectangle, 50);
                Canvas.SetLeft(ThirdRectangle, 350);
                Canvas.SetBottom(ThirdRectangle, 50);
                // Zarovnání textů popisujících jednotlivé sloupce
                Canvas.SetLeft(TextDescribingRectangle1, 50);
                Canvas.SetBottom(TextDescribingRectangle1, 30);
                Canvas.SetLeft(TextDescribingRectangle2, 198);
                Canvas.SetBottom(TextDescribingRectangle2, 30);
                Canvas.SetLeft(TextDescribingRectangle3, 350);
                Canvas.SetBottom(TextDescribingRectangle3, 30);
                // Zarovnání textů, které reprezentují číselnou hodnotu jednotlivích sloupců
                Canvas.SetLeft(FirstNumberTextBlock, 50);
                Canvas.SetBottom(FirstNumberTextBlock, (60 + FirstRectangle.Height));
                Canvas.SetLeft(SecondNumberTextBlock, 200);
                Canvas.SetBottom(SecondNumberTextBlock, (60 + SecondRectangle.Height));
                Canvas.SetLeft(ThirdNumberTextBlock, 350);
                Canvas.SetBottom(ThirdNumberTextBlock, (60 + ThirdRectangle.Height));
                Canvas.SetLeft(FourthRectangle, 400);
                Canvas.SetBottom(FourthRectangle, (60 + ThirdRectangle.Height));
            }
            // Pokud je nastaven typ zobrazovaných dat na přehled transakcí, jsou v grafu zobrazeny čtyři sloupce.
            else
            {
                // Text popisující jednotlivé sloupce v grafu se změní.
                TextDescribingRectangle1.Text = "Mat. expenses";
                TextDescribingRectangle2.Text = "Contract mon.";
                TextDescribingRectangle3.Text = "Added money";
                TextDescribingRectangle4.Text = "Collected mon.";
                // Přidání jedntlivých prvků do Canvas               
                canvas.Children.Add(FourthRectangle);
                canvas.Children.Add(TextDescribingRectangle1);
                canvas.Children.Add(TextDescribingRectangle2);
                canvas.Children.Add(TextDescribingRectangle3);
                canvas.Children.Add(TextDescribingRectangle4);                
                canvas.Children.Add(FourthNumberTextBlock);
                // Nastavení zarovnání jednotlivý prvků v grafu
                // Zarovnání obdelníků
                Canvas.SetLeft(FirstRectangle, 50);
                Canvas.SetBottom(FirstRectangle, 50);
                Canvas.SetLeft(SecondRectangle, 150);
                Canvas.SetBottom(SecondRectangle, 50);
                Canvas.SetLeft(ThirdRectangle, 250);
                Canvas.SetBottom(ThirdRectangle, 50);
                Canvas.SetLeft(FourthRectangle, 350);
                Canvas.SetBottom(FourthRectangle, 50);
                // Zarovnání textů popisujících jednotlivé sloupce
                Canvas.SetLeft(TextDescribingRectangle1, 30);
                Canvas.SetBottom(TextDescribingRectangle1, 30);
                Canvas.SetLeft(TextDescribingRectangle2, 135);
                Canvas.SetBottom(TextDescribingRectangle2, 30);
                Canvas.SetLeft(TextDescribingRectangle3, 240);
                Canvas.SetBottom(TextDescribingRectangle3, 30);
                Canvas.SetLeft(TextDescribingRectangle4, 350);
                Canvas.SetBottom(TextDescribingRectangle4, 30);
                // Zarovnání textů, které reprezentují číselnou hodnotu jednotlivích sloupců
                Canvas.SetLeft(FirstNumberTextBlock, 50);
                Canvas.SetBottom(FirstNumberTextBlock, (60 + FirstRectangle.Height));
                Canvas.SetLeft(SecondNumberTextBlock, 150);
                Canvas.SetBottom(SecondNumberTextBlock, (60 + SecondRectangle.Height));
                Canvas.SetLeft(ThirdNumberTextBlock, 250);
                Canvas.SetBottom(ThirdNumberTextBlock, (60 + ThirdRectangle.Height));
                Canvas.SetLeft(FourthNumberTextBlock, 350);
                Canvas.SetBottom(FourthNumberTextBlock, (60 + FourthRectangle.Height));
            }
            // Obsluha události
            FirstRectangle.MouseDown += Rectangle_MouseDown;
            SecondRectangle.MouseDown += Rectangle_MouseDown;
            ThirdRectangle.MouseDown += Rectangle_MouseDown;
            FourthRectangle.MouseDown += Rectangle_MouseDown;
        }

        /// <summary>
        /// Metoda naplňuje SelectListBox daty podle toho, na jaký sloupec uživatel klikne.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Vyčistění VMVMListBoxCollection
            VMListBoxCollection.Clear();
            // Pokud jsou zobrazovány zakázky
            if (classType == "Contract")
            {
                if (((Rectangle)sender).Name == "FirstRectangle")
                {
                    // Pokud uživatel kliknul na první obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMActualContractCollection.
                    foreach (Contract contract in VMActualContractCollection)
                        VMListBoxCollection.Add(contract.OrderDateTime.ToString("dd/MM/yyyy HH:mm") + "   " + contract.ContractName + "\t" + contract.Reward.ToString() + ",-");
                }
                else if (((Rectangle)sender).Name == "SecondRectangle")
                {
                    // Pokud uživatel kliknul na druhý obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMDeletedContractCollection.
                    foreach (Contract contract in VMDeletedContractCollection)
                        VMListBoxCollection.Add(contract.OrderDateTime.ToString("dd/MM/yyyy HH:mm") + "   " + contract.ContractName + "\t" + contract.Reward.ToString() + ",-");
                }
                else if (((Rectangle)sender).Name == "ThirdRectangle")
                {
                    // Pokud uživatel kliknul na třetí obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMAllContractCollection.
                    foreach (Contract contract in VMAllContractCollection)
                        VMListBoxCollection.Add(contract.OrderDateTime.ToString("dd/MM/yyyy HH:mm") + "   " + contract.ContractName + "\t" + contract.Reward.ToString() + ",-");
                }
            }
            // Pokud jsou zobrazovány materiály
            else if (classType == "Material")
            {
                if (((Rectangle)sender).Name == "FirstRectangle")
                {
                    // Pokud uživatel kliknul na první obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMActualMaterialCollection.
                    foreach (Material material in VMActualMaterialCollection)
                        VMListBoxCollection.Add(material.MaterialQuantity.ToString() + "x\t" + material.MaterialNames.ToString() + "\t    " + Math.Round(material.MaterialPrice).ToString() + ",-");
                }
                else if (((Rectangle)sender).Name == "SecondRectangle")
                {
                    // Pokud uživatel kliknul na druhý obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMDeletedMaterialCollection.
                    foreach (Material material in VMDeletedMaterialCollection)
                    VMListBoxCollection.Add(material.PurchaseDateTime.ToString("dd/MM/yyyy HH:mm") + "   " + material.MaterialNames.ToString() + "\t" + Math.Round(material.MaterialPrice).ToString() + ",-");
                }
                else if (((Rectangle)sender).Name == "ThirdRectangle")
                {
                    // Pokud uživatel kliknul na třetí obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMAllMaterialCollection.
                    foreach (Material material in VMAllMaterialCollection)
                    VMListBoxCollection.Add(material.PurchaseDateTime.ToString("dd/MM/yyyy HH:mm") + "   " + material.MaterialNames.ToString() + "\t" + Math.Round(material.MaterialPrice).ToString() + ",-");
                }
            }
            // Pokud jsou zobrazovány zaměstnanci
            else if (classType == "Employee")
            {
                if (((Rectangle)sender).Name == "FirstRectangle")
                {
                    // Pokud uživatel kliknul na první obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMActualEmployeeCollection.
                    foreach (Employee employee in VMActualEmployeeCollection)
                        VMListBoxCollection.Add(employee.HireDateTime.ToString("dd/MM/yyyy HH:mm") + "   " + employee.Name + "\t" + Math.Round(employee.Wages).ToString() + ",-");
                }
                else if (((Rectangle)sender).Name == "SecondRectangle")
                {
                    // Pokud uživatel kliknul na druhý obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMDeletedEmployeeCollection.
                    foreach (Employee employee in VMDeletedEmployeeCollection)
                     VMListBoxCollection.Add(employee.HireDateTime.ToString("dd/MM/yyyy HH:mm") + "   " + employee.Name + "\t" + Math.Round(employee.Wages).ToString() + ",-");
                }
                else if (((Rectangle)sender).Name == "ThirdRectangle")
                {
                    // Pokud uživatel kliknul na třetí obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMAllEmployeeCollection.
                    foreach (Employee employee in VMAllEmployeeCollection)
                        VMListBoxCollection.Add(employee.HireDateTime.ToString("dd/MM/yyyy HH:mm") + "   " + employee.Name + "\t" + Math.Round(employee.Wages).ToString() + ",-");
                }
            }
            // Pokud jsou zobrazován přehled transakcí
            else if (classType == "Overview")
            {
                if (((Rectangle)sender).Name == "FirstRectangle")
                {
                    // Pokud uživatel kliknul na první obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMSortTransactionCollections, které představují trancakce při nákupu materiálu.
                    foreach (Transaction transaction in VMSortTransactionCollections(Transaction.TransactionType.MaterilAdd))
                        VMListBoxCollection.Add(transaction.TransactionTime.ToString("dd/MM/yyyy HH:mm") + "   " + transaction.Headline + "\t" + Math.Round(transaction.Money, 2).ToString() + ",-");
                }
                else if (((Rectangle)sender).Name == "SecondRectangle")
                {
                    // Pokud uživatel kliknul na druhý obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMSortTransactionCollections, které představují trancakce při splnění zakázky.
                    foreach (Transaction transaction in VMSortTransactionCollections(Transaction.TransactionType.ContractAdd))
                        VMListBoxCollection.Add(transaction.TransactionTime.ToString("dd/MM/yyyy HH:mm") + "   " + transaction.Headline + "\t" + Math.Round(transaction.Money, 2).ToString() + ",-");
                }
                else if (((Rectangle)sender).Name == "ThirdRectangle")
                {
                    // Pokud uživatel kliknul na třetí obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z VMSortTransactionCollections, které představují trancakce při přidání peněz na účet firmy.
                    foreach (Transaction transaction in VMSortTransactionCollections(Transaction.TransactionType.OwnerAdd))
                        VMListBoxCollection.Add(transaction.TransactionTime.ToString("dd/MM/yyyy HH:mm") + "   " + transaction.Headline + "\t" + Math.Round(transaction.Money, 2).ToString() + ",-");
                }
                else if (((Rectangle)sender).Name == "FourthRectangle")
                {
                    // Pokud uživatel kliknul na čtvrtý obdelník v grafu, VMListBoxCollection je naplněna vybranými a převedenými daty z  VMSortTransactionCollections, které představují trancakce při odebrání peněz z účet firmy.
                    foreach (Transaction transaction in VMSortTransactionCollections(Transaction.TransactionType.OwnerRemowed))
                        VMListBoxCollection.Add(transaction.TransactionTime.ToString("dd/MM/yyyy HH:mm") + "   " + transaction.Headline + "\t" + Math.Round(transaction.Money, 2).ToString() + ",-");
                }
                // Aktualizace zobrazených dat
                CallChange("VMListBoxCollection");
            }
        }

        /// <summary>
        /// Metoda profiltruje data ze zobrazených kolekcí v OverviewWindow.xaml podle časových parametrů, které zadáva uživatel.
        /// </summary>
        /// <param name="month">Zvolený měsíc</param>
        /// <param name="year">Zvolený rok</param>
        /// <param name="classisOverview">Parametr rozlišuje způsob filtrace dat podle času</param>
        /// <param name="classicTimeType">Když je hodnota classicOverview true, rozhoduje prametr za jaké časové období zpět, jsou data vybrány</param>
        private void FillOverwiewCollection(int month, string year, bool classisOverview, string classicTimeType)
        {
            // Aktuální čas
            DateTime actualTime = DateTime.Now;
            // Čas určený k porovnávání - je nastaven podle vstupních parametrů
            DateTime compareDate;
            // Podle classicTimeType je určen čas, od kdy jsou data vybrány k zobrazení
            if (classicTimeType == "Week")
                compareDate = actualTime.AddDays(-7);
            else if (classicTimeType == "Month")
                compareDate = actualTime.AddMonths(-1);
            else if (classicTimeType == "Year")
                compareDate = actualTime.AddYears(-1);
            else if (classicTimeType == "All")
                compareDate = actualTime.AddYears(-50);
            else
                throw new ArgumentException("Problem with time.");
            // Vyčištění kolekcí před naplněním
            VMActualMaterialCollection.Clear();
            VMActualEmployeeCollection.Clear();
            VMActualContractCollection.Clear();
            VMDeletedMaterialCollection.Clear();
            VMDeletedEmployeeCollection.Clear();
            VMDeletedContractCollection.Clear();
            VMAllMaterialCollection.Clear();
            VMAllEmployeeCollection.Clear();
            VMAllContractCollection.Clear();
            VMSortTransactionCollection.Clear();
            if (!int.TryParse(year, out int Year))
                throw new ArgumentException("Problem with year");
            foreach ( Material material in factoryModel.AllMaterialCollection)
            {
                // Kolekce všech materiálů je naplněna materiály - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((material.PurchaseDateTime.Year == Year) && (material.PurchaseDateTime.Month == (month + 1)) && (classisOverview == false)) || ((material.PurchaseDateTime > compareDate ) && (classisOverview == true)))
                    VMAllMaterialCollection.Add(new Material(material.MaterialNames, material.MaterialPrice, material.MaterialQuantity, material.PurchaseDateTime));
            }
            foreach (Material material in factoryModel.DeletedMaterialCollection)
            {
                // Kolekce smazaných materiálů je naplněna materiály - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((material.PurchaseDateTime.Year == Year) && (material.PurchaseDateTime.Month == (month + 1)) && (classisOverview == false)) || ((material.PurchaseDateTime > compareDate) && (classisOverview == true)))
                    VMDeletedMaterialCollection.Add(new Material(material.MaterialNames, material.MaterialPrice, material.MaterialQuantity, material.PurchaseDateTime));
            }
            foreach (Material material in factoryModel.MaterialCollection)
            {
                // Kolekce aktuálních materiálů je naplněna materiály - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((material.PurchaseDateTime.Year == Year) && (material.PurchaseDateTime.Month == (month + 1)) && (classisOverview == false)) || ((material.PurchaseDateTime > compareDate) && (classisOverview == true)))
                    VMActualMaterialCollection.Add(new Material(material.MaterialNames, material.MaterialPrice, material.MaterialQuantity, material.PurchaseDateTime));
            }
            foreach (Contract contract in factoryModel.AllContractCollection)
            {
                // Kolekce všech zakázek je naplněna zakázkami - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((contract.OrderDateTime.Year == Year) && (contract.OrderDateTime.Month == (month + 1)) && (classisOverview == false)) || ((contract.OrderDateTime > compareDate) && (classisOverview == true)))
                    VMAllContractCollection.Add(new Contract(contract.ContractName, contract.SubmitterName, contract.Pieces, contract.Reward, contract.KindMaterials, contract.OrderDateTime));
            }
            foreach (Contract contract in factoryModel.DeletedContractCollection)
            {
                // Kolekce smazaných zakázek je naplněna zakázkami - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((contract.OrderDateTime.Year == Year) && (contract.OrderDateTime.Month == (month + 1)) && (classisOverview == false)) || ((contract.OrderDateTime > compareDate) && (classisOverview == true)))
                    VMDeletedContractCollection.Add(new Contract(contract.ContractName, contract.SubmitterName, contract.Pieces, contract.Reward, contract.KindMaterials, contract.OrderDateTime));
            }
            foreach (Contract contract in factoryModel.ContractCollection)
            {
                // Kolekce aktuálních zakázek je naplněna zakázkami - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((contract.OrderDateTime.Year == Year) && (contract.OrderDateTime.Month == (month + 1)) && (classisOverview == false)) || ((contract.OrderDateTime > compareDate) && (classisOverview == true)))
                    VMActualContractCollection.Add(new Contract(contract.ContractName, contract.SubmitterName, contract.Pieces, contract.Reward, contract.KindMaterials, contract.OrderDateTime));
            }
            foreach (Employee employee in factoryModel.AllEmployeeCollection)
            {
                // Kolekce všech zaměstnanců je naplněna zaměstnanci - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((employee.HireDateTime.Year == Year) && (employee.HireDateTime.Month == (month + 1)) && (classisOverview == false)) || ((employee.HireDateTime > compareDate) && (classisOverview == true)))
                    VMAllEmployeeCollection.Add(new Employee(employee.Name, employee.Wages, employee.HireDateTime));
            }
            foreach (Employee employee in factoryModel.DeletedEmployeeCollection)
            {
                // Kolekce smazaných zaměstnanců je naplněna zaměstnanci - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((employee.HireDateTime.Year == Year) && (employee.HireDateTime.Month == (month + 1)) && (classisOverview == false)) || ((employee.HireDateTime > compareDate) && (classisOverview == true)))
                    VMDeletedEmployeeCollection.Add(new Employee(employee.Name, employee.Wages, employee.HireDateTime));
            }
            foreach (Employee employee in factoryModel.EmployeeCollection)
            {
                // Kolekce aktuálních zaměstnanců je naplněna zaměstnanci - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((employee.HireDateTime.Year == Year) && (employee.HireDateTime.Month == (month + 1)) && (classisOverview == false)) || ((employee.HireDateTime > compareDate) && (classisOverview == true)))
                    VMActualEmployeeCollection.Add(new Employee(employee.Name, employee.Wages, employee.HireDateTime));
            }
            foreach (Transaction transaction in factoryModel.TransactionCollection)
            {
                // Kolekce všech transakcí je naplněna transakcemi - buď za konkrétní měsíc, nebo za určité časové období zpět.
                if (((transaction.TransactionTime.Year == Year) && (transaction.TransactionTime.Month == (month + 1)) && (classisOverview == false)) || ((transaction.TransactionTime > compareDate) && (classisOverview == true)))
                    VMSortTransactionCollection.Add(new Transaction(transaction.Headline, transaction.TypeOfTransaction, transaction.Money, transaction.TransactionTime));
            }
        }

        /// <summary>
        /// Metoda vrací kolekci transakcí, které odpovídají zadanému druhu transakce
        /// </summary>
        /// <param name="transactionType">Druh konkrétní transakce</param>
        /// <returns></returns>
        public ObservableCollection<Transaction> VMSortTransactionCollections(Transaction.TransactionType transactionType)
        {
            ObservableCollection<Transaction> returnTransactionCollection = new ObservableCollection<Transaction>();
            // Pokud je transakce odpovídajícího druhu, je uložena do návratové kolekce.
            foreach (Transaction transaction in VMSortTransactionCollection)
            {
                if (transaction.TypeOfTransaction == transactionType)
                    returnTransactionCollection.Add(transaction);
            }
            return returnTransactionCollection;
        }

        /// <summary>
        /// Metoda z kolekce profiltrovaných transací určí parametr, podle kterého jsou určeny výšky jednotlivích obdélníků, které jsou zobrazeny v grafu.
        /// </summary>
        /// <param name="transactionType">Druh konkrétní transakce</param>
        /// <returns></returns>
        private double VMAddUpTransactionCollections(Transaction.TransactionType transactionType)
        {
            double returnMoney = 0;
            // Pokud je transakce odpovídajícího druhu, je její číselná hodnota přičtena k návratové hodnotě.
            foreach (Transaction transaction in VMSortTransactionCollection)
            {
                if (transaction.TypeOfTransaction == transactionType)
                    returnMoney += transaction.Money;
            }
            returnMoney = Math.Round(returnMoney, 2);
            return returnMoney;
        }
        #endregion

        /// <summary>
        /// Metoda aktualizuje data, aby změny na nich byly zobrazeny uživateli za běhu aplikace.
        /// </summary>
        /// <param name="events"></param>
        public void CallChange(string events)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(events));
        }
        /// <summary>
        /// Metoda při spuštění aplikace načte data z XAML souboru do kolekcí a přehledu firmy.
        /// </summary>
        public void VMLoad()
        {
            factoryModel.Load();
            VMContractCollection = factoryModel.ContractCollection;
            VMEmployeeCollection = factoryModel.EmployeeCollection;
            VMMaterialCollection = factoryModel.MaterialCollection;
            VMTransactionCollection = factoryModel.TransactionCollection;
            VMOverview = factoryModel.OverviewModel;
            CallChange("VMEmployeeCollection");
            CallChange("VMContactCollection");
            CallChange("VMMaterialCollection");
            CallChange("VMOverview");
        }
    }
}
