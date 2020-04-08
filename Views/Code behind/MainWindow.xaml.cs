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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Factory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Instance třídy FactoryValidator, která slouží jako validátor a ViewModel projektu
        /// </summary>
        private FactoryValidator factoryValidator = new FactoryValidator();

        /// <summary>
        /// Konstruktor okna
        /// </summary>
        /// <param name="factoryValidator"></param>
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                // Načtení dat ze XAML souborů
                factoryValidator.VMLoad();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loading problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Zdroj zobrazených dat
            DataContext = factoryValidator;           
        }

        /// <summary>
        /// Tlačítko na obrázku - Otevření okna CuttingDepartmentWindow (V něm se nachází přehled celé firmy.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCuttingDepartmentButton_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                CuttingDepartmentWindow cuttingDepartmentWindow = new CuttingDepartmentWindow(factoryValidator);
                cuttingDepartmentWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Open problem", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Zavření okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
           Close();
        }

        /// <summary>
        /// Tkačítko OK - Spustění metody, která přidává nebo odebírá peníze z účtu firmy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                factoryValidator.VMAddOrRemoveMoney(AddMoneyTextBox.Text, RemoveMoneyTextBox.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem with numbers", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
