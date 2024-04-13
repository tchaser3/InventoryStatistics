/* Title:           Warehouse Statistics
 * Date:            11-20-17
 * Author:          Terry Holmes
 * 
 * Description:     This form will calculate the statistics for a warehouse */

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
using NewEventLogDLL;
using InventoryDLL;
using IssuedPartsDLL;
using NewPartNumbersDLL;
using DateSearchDLL;

namespace InventoryStatistics
{
    /// <summary>
    /// Interaction logic for WarehouseStatistics.xaml
    /// </summary>
    public partial class WarehouseStatistics : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        IssuedPartsClass TheIssuedPartsClass = new IssuedPartsClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data sets
        FindWarehouseInventoryDataSet TheFindWarehouseInventoryDataSet = new FindWarehouseInventoryDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberData = new FindPartByPartNumberDataSet();
        FindIssuedPartsByPartIDWarehouseIDAndDateRangeDataSet TheFindIssuedPartsByPartIDWarehouseIDAndDataSet = new FindIssuedPartsByPartIDWarehouseIDAndDateRangeDataSet();
        WarehouseStatisticsDataSet TheWarehouseStatisticsDataSet = new WarehouseStatisticsDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;

        int gintWarehouseID;
        int gintDayCount;

        public WarehouseStatistics()
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

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the combo box
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intWeeks;
            int intDays;

            try
            {
                gdatEndDate = DateTime.Now;
                gdatEndDate = TheDateSearchClass.RemoveTime(gdatEndDate);
                gdatStartDate = TheDateSearchClass.SubtractingDays(gdatEndDate, 364);

                intWeeks = 364 / 7;
                intDays = intWeeks * 2;
                gintDayCount = 189 - intDays;

                cboWarehouse.Items.Add("Select Warehouse");

                intNumberOfRecords = MainWindow.TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboWarehouse.Items.Add(MainWindow.TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboWarehouse.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Inventory Statistics // Warehouse Statistics // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intInventoryCounter;
            int intInventoryNumberOfRecords;
            int intPartID;
            string strPartNumber;
            int intIssuedCounter;
            int intIssuedNumberOfRecords;
            int intTotalIssued;
            double douMean;
            double douIssueMean;
            double douVariance;
            double douStdDev;
            double douCalculatingVariance;
            double douTotalVariance;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intSelectedIndex = cboWarehouse.SelectedIndex - 1;
                TheWarehouseStatisticsDataSet.parts.Rows.Clear();

                if(intSelectedIndex > -1)
                {
                    gintWarehouseID = MainWindow.TheFindPartsWarehouseDataSet.FindPartsWarehouses[intSelectedIndex].EmployeeID;

                    TheFindWarehouseInventoryDataSet = TheInventoryClass.FindWarehouseInventory(gintWarehouseID);

                    intInventoryNumberOfRecords = TheFindWarehouseInventoryDataSet.FindWarehouseInventory.Rows.Count - 1;

                    for(intInventoryCounter = 0; intInventoryCounter <= intInventoryNumberOfRecords; intInventoryCounter++)
                    {
                        strPartNumber = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intInventoryCounter].PartNumber;

                        TheFindPartByPartNumberData = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                        intPartID = TheFindPartByPartNumberData.FindPartByPartNumber[0].PartID;

                        intTotalIssued = 0;

                        TheFindIssuedPartsByPartIDWarehouseIDAndDataSet = TheIssuedPartsClass.FindIssuedPartsByPartIDWarehouseIDDateRange(intPartID, gintWarehouseID, gdatStartDate, gdatEndDate);

                        intIssuedNumberOfRecords = TheFindIssuedPartsByPartIDWarehouseIDAndDataSet.FindIssuedPartsByPartIDWarehouseIDAndDateRange.Rows.Count - 1;

                        if(intIssuedNumberOfRecords > -1)
                        {
                            for(intIssuedCounter = 0; intIssuedCounter <= intIssuedNumberOfRecords; intIssuedCounter++)
                            {
                                intTotalIssued += TheFindIssuedPartsByPartIDWarehouseIDAndDataSet.FindIssuedPartsByPartIDWarehouseIDAndDateRange[intIssuedCounter].Quantity;
                            }
                        }

                        douIssueMean = Convert.ToDouble(intTotalIssued) / Convert.ToDouble(intIssuedNumberOfRecords + 1);
                        douTotalVariance = 0;

                        if (intIssuedNumberOfRecords > -1)
                        {
                            for (intIssuedCounter = 0; intIssuedCounter <= intIssuedNumberOfRecords; intIssuedCounter++)
                            {
                                douCalculatingVariance = Convert.ToDouble(TheFindIssuedPartsByPartIDWarehouseIDAndDataSet.FindIssuedPartsByPartIDWarehouseIDAndDateRange[intIssuedCounter].Quantity) - douIssueMean;

                                douTotalVariance += (douCalculatingVariance * douCalculatingVariance);
                            }
                        }

                        douStdDev = Math.Sqrt(douTotalVariance);

                        WarehouseStatisticsDataSet.partsRow NewPartRow = TheWarehouseStatisticsDataSet.parts.NewpartsRow();

                        NewPartRow.Description = TheFindPartByPartNumberData.FindPartByPartNumber[0].PartDescription;
                        NewPartRow.DailyMean = 0;
                        NewPartRow.IssueMean = douIssueMean;
                        NewPartRow.PartID = intPartID;
                        NewPartRow.PartNumber = strPartNumber;
                        NewPartRow.QuantityIssued = intTotalIssued;
                        NewPartRow.QuantityOnHand = TheFindWarehouseInventoryDataSet.FindWarehouseInventory[intInventoryCounter].Quantity;
                        NewPartRow.StdDev = douStdDev;
                        NewPartRow.Variance = douTotalVariance;

                        TheWarehouseStatisticsDataSet.parts.Rows.Add(NewPartRow);
                    }

                    intInventoryNumberOfRecords = TheWarehouseStatisticsDataSet.parts.Rows.Count - 1;

                    for(intInventoryCounter = 0; intInventoryCounter <= intInventoryNumberOfRecords; intInventoryCounter++)
                    {
                        douMean = Convert.ToDouble(TheWarehouseStatisticsDataSet.parts[intInventoryCounter].QuantityIssued / Convert.ToDouble(gintDayCount));

                        TheWarehouseStatisticsDataSet.parts[intInventoryCounter].DailyMean = douMean;
                    }

                    dgrResults.ItemsSource = TheWarehouseStatisticsDataSet.parts;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Inventory Statistics // Warehouse Statistics // cbo Warehouse Selected Index Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }
    }
}
