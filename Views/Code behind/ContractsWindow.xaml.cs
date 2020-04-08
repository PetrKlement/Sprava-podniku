using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Factory
{
    /// <summary>
    /// Interaction logic for ContractsWindow.xaml
    /// </summary>
    public partial class ContractsWindow : Window
    {
        /// <summary>
        /// Instance třídy FactoryValidator, která slouží jako validátor a ViewModel projektu
        /// </summary>
        private FactoryValidator factoryValidator;
        /// <summary>
        /// Konstruktor okna
        /// </summary>
        /// <param name="factoryValidator"></param>
        public ContractsWindow(FactoryValidator factoryValidator)
        {
            InitializeComponent();
            this.factoryValidator = factoryValidator;
            // Zdroj zobrazených dat
            DataContext = factoryValidator;
        }

        /// <summary>
        /// ListBox contracsListBox - Zvolení zakázky, která se má vykonat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContracsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Contract ChosenContract = (Contract)contracsListBox.SelectedItem;
            SubmitterBorder.DataContext = ChosenContract;
        }

        /// <summary>
        /// Tlačítko Numerare - Spočtení nákladů na provedení zakázky.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               NumerareTextBlock.Text = factoryValidator.CalculateCosts(TimeTextBox.Text, OtherCostsTextBox.Text, WorkerComboBox.Text, MaterialComboBox.SelectedIndex, PiecesTextBlock.Text).ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tlačítko Take contract - Odečtení potřebného materiálu a přesun zakázky mezi smazané.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TakeContractButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                factoryValidator.VMContractMaterial((Contract)contracsListBox.SelectedItem);
                factoryValidator.VMDeleteContract((Contract)contracsListBox.SelectedItem);  
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tlačítko Zavřít - Zavření okna ContractWindow. Otevření okna CuttingDepartmentWindow. (V něm se nachází přehled celé firmy.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            CuttingDepartmentWindow cuttingDepartmentWindow = new CuttingDepartmentWindow(factoryValidator);
            cuttingDepartmentWindow.ShowDialog();
        }

        /// <summary>
        /// Tlačítko Buy material - Zavření okna ContractWindow. Otevření okna BuyMaterialWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            BuyMaterialWindow buyMaterialWindow = new BuyMaterialWindow(factoryValidator);
            buyMaterialWindow.ShowDialog();
        }

        /// <summary>
        /// Tlačítko Add contract - Zavření okna ContractWindow. Otevření okna AddContractWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddContractButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            AddContractWindow addContractWindow = new AddContractWindow(factoryValidator);
            addContractWindow.ShowDialog();
        }

        private void MaterialComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
