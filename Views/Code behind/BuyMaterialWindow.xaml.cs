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
    /// Interaction logic for BuyMaterialWindow.xaml
    /// </summary>
    public partial class BuyMaterialWindow : Window
    {
        /// <summary>
        /// Instance třídy FactoryValidator, která slouží jako validátor a ViewModel projektu
        /// </summary>
        private FactoryValidator factoryValidator;
        
        /// <summary>
        /// Konstruktor okna
        /// </summary>
        /// <param name="factoryValidator"></param>
        public BuyMaterialWindow(FactoryValidator factoryValidator)
        {
            this.factoryValidator = factoryValidator;
            InitializeComponent();
            // Zdroj zobrazených dat
            DataContext = factoryValidator;
            // Zdroj zobrazených dat pro materialDataGrid
            materialDataGrid.ItemsSource = factoryValidator.VMMaterialCollection;
        }

        /// <summary>
        /// Tlačítko Zavřít - Zavření okna BuyMaterialWindow. Otevření okna ContractsWindow. (V lze něm zpracovávat zakázky.)
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
        /// Tlačítko Add - Přičtení materiálu do kolekce a odečtení nákladů
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                factoryValidator.VMAddMaterial(materialComboBox.Text, priceTextBox.Text, piecesTextBox.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tlačítko Delete material - Odstranění materiálu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (materialDataGrid.SelectedItem != null)
            {
                try
                {
                    Material material = (Material)materialDataGrid.SelectedItem;
                    factoryValidator.VMDestroyMaterial(material);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MaterialComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void EmployeeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
