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
        string gsPOLineFactTable = "EIWarehouse.dbo.POLineFact";
        string gsEmployeeTable = "EIWarehouse.dbo.Employee";
        string gsVendorTable = "EIWarehouse.dbo.Vendor";

        POClassesDataContext context = new POClassesDataContext();
        List<Buyer> _oBuyers;
        List<Vendor> _oVendors;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;


            POClassesDataContext context = new POClassesDataContext();

            List<employee> oEmployees = dbRetrieveData(gsEmployeeTable);
            List<string> sVendors = sdbRetrieveData(gsVendorTable, "VendorName");

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


        }
        #region Button
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ChecksForAllTheSearchContain();
        }

        private void BtnSearchMulti_Click(object sender, RoutedEventArgs e)
        {
            ChecksForMultiSearchContain();

        }
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            txtBuyer.Text = "";
            txtSupplier.Text = "";
            txtPartNumber.Text = "";
            txtDescription.Text = "";
            txtManufacturer.Text = "";
            txtSupplierPart.Text = "";
            txtJobs.Text = "";
            txtAssemblies.Text = "";
            cbClosed.IsChecked = false;
            cbOpen.IsChecked = false;
            cbMaterial.IsChecked = false;
            cbSubcontract.IsChecked = false;
            cbOtherInv.IsChecked = false;
            dpStart.Text = null;
            dpEnd.Text = null;
        }
        private void txtFieldsContain_EnterKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                BtnSearchMulti_Click(sender, e);
            }
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow setting = new SettingWindow();
            setting.ShowDialog();
        }
        #endregion


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

        #region Search Logic
        private void ChecksForMultiSearchContain()
        {
            ResultView result = new ResultView();
            List<POLineFact> dblPOLine = new List<POLineFact>();

            string sQuery = "";
            int iCount = 0;

            if (cbPartNumber.IsChecked == true)
            {
                sQuery += " PartNumber LIKE '%" + txtFieldsContain.Text;
                iCount++;
            }
            if (cbDescription.IsChecked == true)
            {
                if (iCount > 0)
                    sQuery += " %' OR PartDescription LIKE '%" + txtFieldsContain.Text;
                else
                    sQuery += " PartDescription LIKE '%" + txtFieldsContain.Text;
                iCount++;

            }
            if (cbManufacturer.IsChecked == true)
            {
                if (iCount > 0)
                    sQuery += " %' OR Manufacturer LIKE '%" + txtFieldsContain.Text;
                else
                    sQuery += " Manufacturer LIKE '%" + txtFieldsContain.Text;
                iCount++;
            }
            if (cbSupplierPartNumber.IsChecked == true)
            {
                if (iCount > 0)
                    sQuery += " %' OR VendorPartNumber LIKE '%" + txtFieldsContain.Text;
                else
                    sQuery += " VendorPartNumber LIKE '%" + txtFieldsContain.Text;
                iCount++;
            }

            if (iCount > 0)
            {
                dblPOLine = context.ExecuteQuery<POLineFact>("SELECT DISTINCT * FROM EIWarehouse.dbo.POLineFact WHERE" + sQuery + "%'").ToList();
                result.dgResult.ItemsSource = dblPOLine;
                result.ShowDialog();
            }
        }
        private void ChecksForAllTheSearchContain()
        {
            ResultView result = new ResultView();
            List<POLineFact> dblPOLine = new List<POLineFact>();

            int iCount = 0;
            string sQuery = "";

            if (cbOpen.IsChecked == true)
            {
                sQuery += " %' OR CLOSED LIKE '%FALSE";
                iCount++;
            }
            if (cbClosed.IsChecked == true)
            {
                if (iCount > 0)
                    sQuery += " %' OR CLOSED LIKE '%TRUE";
                else
                    sQuery += " CLOSED LIKE '%TRUE";
                iCount++;
            }
            if (cbMaterial.IsChecked == true)
            {
                if (iCount > 0)
                    sQuery += " %' OR TranType LIKE '%PUR-MTL";
                else
                    sQuery += " TranType LIKE '%PUR-MTL";
                iCount++;

            }
            if (cbSubcontract.IsChecked == true)
            {
                if (iCount > 0)
                    sQuery += " %' OR TranType LIKE '%PUR-SUB";
                else
                    sQuery += " TranType LIKE '%PUR-SUB";
                iCount++;

            }
            if (cbOtherInv.IsChecked == true)
            {
                if (iCount > 0)
                    sQuery += " %' OR TranType LIKE '%PUR-UKN";
                else
                    sQuery += " TranType LIKE '%PUR-UKN";
                iCount++;

            }
            if (txtPartNumber.Text != "")
            {
                if (iCount > 0)
                    sQuery += " %' OR PartNumber LIKE '%" + txtPartNumber.Text;
                else
                    sQuery += " PartNumber LIKE '%" + txtPartNumber.Text;
                iCount++;

            }
            if (txtDescription.Text != "")
            {
                if (iCount > 0)
                    sQuery += " %' OR PartDescription LIKE '%" + txtPartNumber.Text;
                else
                    sQuery += " PartDescription LIKE '%" + txtDescription.Text;
                iCount++;

            }
            if (txtManufacturer.Text != "")
            {
                if (iCount > 0)
                    sQuery = " %' OR Manufacturer LIKE '%" + txtPartNumber.Text;
                else
                    sQuery += " Manufacturer LIKE '%" + txtDescription.Text;
                iCount++;

            }
            if (txtSupplierPart.Text != "")
            {
                if (iCount > 0)
                    sQuery += " %' OR VendorPartNumber LIKE '%" + txtPartNumber.Text;
                else
                    sQuery += " VendorPartNumber LIKE '%" + txtSupplierPart.Text;
                iCount++;

            }
            if (txtJobs.Text != "")
            {
                if (iCount > 0)
                    sQuery = " %' OR JobNum LIKE '%" + txtPartNumber.Text;
                else
                    sQuery += " JobNum LIKE '%" + txtJobs.Text;
                iCount++;

            }
            if (txtAssemblies.Text != "")
            {
                if (iCount > 0)
                    sQuery += " %' OR AssemblySeq LIKE '%" + txtPartNumber.Text;
                else
                    sQuery += " AssemblySeq LIKE '%" + txtAssemblies.Text;
                iCount++;

            }
            if (dpStart.Text != "")
            {
                if (iCount > 0)
                    sQuery += " %' OR OrderDate LIKE '%" + dpStart.Text;
                else
                    sQuery += " OrderDate LIKE '%" + dpStart.Text;
                iCount++;

            }
            if (dpEnd.Text != "")
            {
                if (iCount > 0)
                    sQuery += " %' OR DueDate LIKE '%" + dpEnd.Text;
                else
                    sQuery += " DueDate LIKE '%" + dpEnd.Text;
                iCount++;

            }
            if(txtBuyer.Text != "")
            {
                Buyer buyer = (Buyer)dgBuyers.SelectedItem;
                if (iCount > 0)
                    sQuery += " %' OR Buyer LIKE '%" + buyer.ID;
                else
                    sQuery += " Buyer LIKE '%" + buyer.ID;
                iCount++;

            }
            if(txtSupplier.Text != "" || dgSupplier.SelectedItem != null )
            {
                Vendor vendor = (Vendor)dgSupplier.SelectedItem;
                if (iCount > 0)
                    sQuery += " %' OR Vendor LIKE '%" + vendor.Supplier;
                else
                    sQuery += " Vendor LIKE '%" + vendor.Supplier;
                iCount++;

            }
            if (iCount > 0)
            {
                dblPOLine = context.ExecuteQuery<POLineFact>("SELECT DISTINCT * FROM EIWarehouse.dbo.POLineFact WHERE" + sQuery + "%'").ToList();
                result.dgResult.ItemsSource = dblPOLine;
                result.ShowDialog();
            }
        }
        #endregion
        #region Database Pull Data Functions
        private List<String> sdbRetrieveData(string sTableName, string sColumn)
        {

            List<string> dbTableData = context.ExecuteQuery<string>("SELECT DISTINCT " + sColumn + " FROM " + sTableName).ToList();

            return dbTableData;
        }
        private List<employee> dbRetrieveData(string sTableName)
        {

            List<employee> dbTableData = context.ExecuteQuery<employee>("SELECT DISTINCT * FROM " + sTableName).ToList();

            return dbTableData;
        }
        #endregion
        #region Classes
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

        #endregion


    }
}
