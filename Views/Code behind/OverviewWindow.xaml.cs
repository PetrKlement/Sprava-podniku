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
    /// Interaction logic for OverviewWindow.xaml
    /// </summary>    
    public partial class OverviewWindow : Window
    {
        // Povolení pro ComboBoxy aby mohli volat metodu vykreslující Canvas
        private bool CallEnabled = false;
        // Typ zobrazených dat
        public string type = "";
        /// <summary>
        /// Instance třídy FactoryValidator, která slouží jako validátor a ViewModel projektu
        /// </summary>
        private FactoryValidator factoryValidator;

        /// <summary>
        /// Konstruktor okna
        /// </summary>
        /// <param name="factoryValidator"></param>
        public OverviewWindow(FactoryValidator factoryValidator)
        {
            InitializeComponent();
            this.factoryValidator = factoryValidator;
            // Zdroj zobrazených dat
            DataContext = factoryValidator;
            // Výchozí nastavení typu zobrazených dat - transakce
            type = "Overview";
            // ComboBoxy mohou volat metodu DrawCanvas
            CallEnabled = true;
            try
            {
                // Vykreslení grafu
                factoryValidator.DrawCanvas(OverviewCanvas, monthsComboBox.SelectedIndex, yearsComboBox.Text, type, (bool)classicOverviewRadiobutton.IsChecked, classicTimeComboBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem with draw.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tlačítko Zavřít - Zavření okna OverviewWindow. Otevření okna CuttingDepartmentWindow. (V něm se nachází přehled celé firmy.)
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
        /// Změna položky v ComboBoxu, který je určený pro nastavení měsíce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MonthsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CallEnabled)
            {
                try
                {
                    // Vykreslení grafu
                    factoryValidator.DrawCanvas(OverviewCanvas, monthsComboBox.SelectedIndex, yearsComboBox.Text, type, (bool)classicOverviewRadiobutton.IsChecked, classicTimeComboBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Problem with draw.", MessageBoxButton.OK, MessageBoxImage.Error);
                }                
            }           
        }

        /// <summary>
        /// Změna položky v ComboBoxu, který je určený pro nastavení roku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CallEnabled)
            {
                try
                {
                    // Vykreslení grafu
                    factoryValidator.DrawCanvas(OverviewCanvas, monthsComboBox.SelectedIndex, yearsComboBox.Text, type, (bool)classicOverviewRadiobutton.IsChecked, classicTimeComboBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Problem with draw.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Změna položky v ComboBoxu, který je určený pro nastavení časového období od teď
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClassicTimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CallEnabled)
            {
                try
                {
                    // Vykreslení grafu
                    factoryValidator.DrawCanvas(OverviewCanvas, monthsComboBox.SelectedIndex, yearsComboBox.Text, type, (bool)classicOverviewRadiobutton.IsChecked, classicTimeComboBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Problem with draw.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// RadioButton určuje způsob, jakým jsou data filtrována v čase
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClasicRadioButton(object sender, RoutedEventArgs e)
        {
            if (CallEnabled)
            {
                try
                {
                    // Vykreslení grafu
                    factoryValidator.DrawCanvas(OverviewCanvas, monthsComboBox.SelectedIndex, yearsComboBox.Text, type, (bool)classicOverviewRadiobutton.IsChecked, classicTimeComboBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Problem with draw.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Tlačítko Employees - Graf zobrazí data o zaměstnancích
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            type = "Employee";
            try
            {
                factoryValidator.DrawCanvas(OverviewCanvas, monthsComboBox.SelectedIndex, yearsComboBox.Text, type, (bool)classicOverviewRadiobutton.IsChecked, classicTimeComboBox.Text);
            }
            catch (Exception ex)
            {
                // Vykreslení grafu
                MessageBox.Show(ex.Message, "Problem with draw.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tlačítko Contracts - Graf zobrazí data o zakázkách
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            type = "Contract";
            try
            {
                // Vykreslení grafu
                factoryValidator.DrawCanvas(OverviewCanvas, monthsComboBox.SelectedIndex, yearsComboBox.Text, type, (bool)classicOverviewRadiobutton.IsChecked, classicTimeComboBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem with draw.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tlačítko Materials - Graf zobrazí data o materiálech
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaterialButton_Click(object sender, RoutedEventArgs e)
        {
            type = "Material";
            try
            {
                // Vykreslení grafu
                factoryValidator.DrawCanvas(OverviewCanvas, monthsComboBox.SelectedIndex, yearsComboBox.Text, type, (bool)classicOverviewRadiobutton.IsChecked, classicTimeComboBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem with draw.", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Tlačítko Transactions - Graf zobrazí přehled transakcí
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OverviewButton_Click(object sender, RoutedEventArgs e)
        {
            type = "Overview";
            try
            {
                // Vykreslení grafu
                factoryValidator.DrawCanvas(OverviewCanvas, monthsComboBox.SelectedIndex, yearsComboBox.Text, type, (bool)classicOverviewRadiobutton.IsChecked, classicTimeComboBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem with draw.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
