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


namespace Factory
{
    /// <summary>
    /// Interaction logic for AddContractWindow.xaml
    /// </summary>
    public partial class AddContractWindow : Window
    {
        /// <summary>
        /// Instance třídy FactoryValidator, která slouží jako validátor a ViewModel projektu
        /// </summary>
        private FactoryValidator factoryValidator;

        /// <summary>
        /// Konstruktor okna
        /// </summary>
        /// <param name="factoryValidator"></param>
        /// 
        public AddContractWindow(FactoryValidator factoryValidator)
        {
            this.factoryValidator = factoryValidator;
            InitializeComponent();
            // Zdroj zobrazených dat
            DataContext = factoryValidator;
            // Zdroj zobrazených dat pro contractDataGrid
            contractDataGrid.ItemsSource = factoryValidator.VMContractCollection;

        }

        /// <summary>
        /// Tlačítko Zavřít - Zavření okna AddContractWindow. Otevření okna ContractsWindow. (V něm se nachází přehled aktuálních zakázek a lze je v něm také přidávat nebo mazat.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            ContractsWindow contractsWindow = new ContractsWindow(factoryValidator);
            contractsWindow.ShowDialog();
        }

        /// <summary>
        /// Tlačítko Add - Přidání nové zakázky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddContractButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                factoryValidator.VMAddContract(nameTextBox.Text, submitterTextBox.Text, piecesTextBox.Text, rewardTextBox.Text, materialComboBox.SelectedItem.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
        
        /// <summary>
        /// Tlačítko Delete contract - Smazání zvolené zakázky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                Contract contract = (Contract)contractDataGrid.SelectedItem;
                factoryValidator.VMDestroyContract(contract);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContractDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MaterialComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
