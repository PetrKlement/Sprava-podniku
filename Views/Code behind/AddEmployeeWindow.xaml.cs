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
    /// Interaction logic for AddEmployeeWindow.xaml
    /// </summary>
    public partial class AddEmployeeWindow : Window
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
        public AddEmployeeWindow(FactoryValidator factoryValidator)
        {
            this.factoryValidator = factoryValidator;
            InitializeComponent();
            // Zdroj zobrazených dat
            DataContext = factoryValidator;
            // Zdroj zobrazených dat pro employeeDataGrid
            employeeDataGrid.ItemsSource = factoryValidator.VMEmployeeCollection;
        }

        /// <summary>
        /// Tlačítko Add - Přidání nové zaměstnance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                factoryValidator.VMAddEmployee(nameTextBox.Text, surnameTextBox.Text, wagesTextBox.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tlačítko Zavřít - Zavření okna AddEmployeeWindow. Otevření okna CuttingDepartmentWindow. (V něm se nachází přehled celé firmy.)
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
        /// Tlačítko Delete employee - Smazání zvoleného zaměstance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(employeeDataGrid.SelectedItem != null)
            {

                try
                {
                    Employee employee = (Employee)employeeDataGrid.SelectedItem;
                    factoryValidator.VMDeleteEmployee(employee);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }

        private void EmployeeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }      
    }
}
