using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Objects;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PO_Search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string gsPOHeaderTable = "EIWarehouse.dbo.POHeader";
        string gsEmployeeTable = "EIWarehouse.dbo.Employee";
        POClassesDataContext context = new POClassesDataContext();
        List<Buyer> _oBuyers;
        List<Vendor> _oVendors;
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;


            POClassesDataContext context = new POClassesDataContext();

            List<employee> oEmployees = dbRetrieveData(gsEmployeeTable);
            List<string> sVendors = sdbRetrieveData(gsPOHeaderTable, "VendorName");

            _oBuyers = new List<Buyer>();
            _oVendors = new List<Vendor>();

            foreach (employee oEmployee in oEmployees)
            {
                _oBuyers.Add(new Buyer(oEmployee.fName.ToLower(), oEmployee.lName.ToLower(), oEmployee.id));
          
            }
            foreach(string sVendor in sVendors)
            {
                _oVendors.Add(new Vendor(sVendor.ToLower()));
            }
            dgBuyers.ItemsSource = _oBuyers;
            dgSupplier.ItemsSource = _oVendors;

            //context.ExecuteQuery<POHeader>("TRUNCATE TABLE BuyerPhone");


            //context.POLines.InsertOnSubmit(null);
            //context.SubmitChanges();
        }

        private List<String> sdbRetrieveData(string sTableName, string sColumn)
        {

            List<string> dbTableData = context.ExecuteQuery<string>("SELECT DISTINCT " + sColumn +  " FROM " + sTableName).ToList();

            return dbTableData;
        }
        private List<employee> dbRetrieveData( string sTableName)
        {
            
            List<employee> dbTableData = context.ExecuteQuery<employee>("SELECT DISTINCT * FROM " + sTableName).ToList();

            return dbTableData;
        }
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ResultView result = new ResultView();
            result.ShowDialog();
        }

        private void BtnSearchMulti_Click(object sender, RoutedEventArgs e)
        {
            ResultView result = new ResultView();
            List<POLine> dblPOLine = new List<POLine>();
            string sPartNumber = "";
            string sDescription = "";
            string sManufacturer = "";
            string sSupplierPartNumber = "";
            int iCount = 0;

            if (cbPartNumber.IsChecked ?? false)
            {
                sPartNumber = " PartNumber LIKE '%" + txtFieldsContain.Text;
                iCount++;
            }
            if (cbDescription.IsChecked ?? false)
            {
                if(iCount > 0)
                    sDescription = " %' OR LineDesc LIKE '%" + txtFieldsContain.Text;
                else
                    sDescription = " LineDesc LIKE '%" + txtFieldsContain.Text;
                iCount++;

            }
            if (cbManufacturer.IsChecked ?? false)
            {
                if (iCount > 0)
                    sManufacturer = " %' OR Manufacturer LIKE '%" + txtFieldsContain.Text;
                else
                    sManufacturer = " Manufacturer LIKE '%" + txtFieldsContain.Text;
                iCount++;
            }
            if (cbSupplierPartNumber.IsChecked == true)
            {
                if (iCount > 0)
                    sSupplierPartNumber = " %' OR VenPartNum LIKE '%" + txtFieldsContain.Text;
                else
                    sSupplierPartNumber = " VenPartNum LIKE '%" + txtFieldsContain.Text;
                iCount++;
            }

            if (iCount > 0)
            {
                dblPOLine = context.ExecuteQuery<POLine>("SELECT DISTINCT * FROM EIWarehouse.dbo.POLine WHERE" + sPartNumber + sDescription + sManufacturer + sSupplierPartNumber + "%'").ToList();
                result.dgResult.ItemsSource = dblPOLine;
                result.ShowDialog();
            }

        }
        private void txtSupplier_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var filtered = _oVendors.Where(Vendor => Vendor.Supplier.StartsWith(txtSupplier.Text.ToLower()));
            dgSupplier.ItemsSource = filtered;
        }
        private void txtBuyer_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var filtered = _oBuyers.Where(Buyer => Buyer.Buyers.StartsWith(txtBuyer.Text.ToLower()) || Buyer.ID.ToString().StartsWith(txtBuyer.Text.ToLower()));
            dgBuyers.ItemsSource = filtered;
        }
        private void txtFieldsContain_EnterKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                BtnSearchMulti_Click(sender, e);
            }
        }
        class Vendor
        {
            public string Supplier { get; set; }
            public Vendor(string name)
            {
                Supplier = name;
            }
        }
        class Buyer
        {
            public string Buyers { get; set; }
            public int? ID { get; set; }

            public Buyer(string fname, string lname, int? id)
            {
                Buyers = fname + " " + lname;
                ID = id;
            }
        }
    }
}
