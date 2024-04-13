/* Title:           Main Menu
 * Date:            11-20-17
 * Author:          Terry Holmes
 * 
 * Description:     This is the main menu */

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

namespace InventoryStatistics
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainMenu()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnWarehouseStatistics_Click(object sender, RoutedEventArgs e)
        {
            WarehouseStatistics WarehouseStatistics = new WarehouseStatistics();
            WarehouseStatistics.Show();
            Close();
        }

        private void btnClose_Copy1_Click(object sender, RoutedEventArgs e)
        {
            PartStatistics PartStatistics = new PartStatistics();
            PartStatistics.Show();
            Close();
        }
    }
}
