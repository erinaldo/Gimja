using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using GimjaBL;
using System.Xml.Linq;

namespace Gimja
{
    public partial class frmSync : DevExpress.XtraEditors.XtraForm
    {
        public frmSync()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.CenterScreen;

            base.OnShown(e);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog _dlg = new FolderBrowserDialog();
            var _result = _dlg.ShowDialog(this);
            if (_result == System.Windows.Forms.DialogResult.OK)
            {
                var folderPath = _dlg.SelectedPath;
                txtLocation.Text = folderPath + string.Format("\\Sync-{0}.xml", DateTime.Now.ToString("MMMM-dd-yyyy"));// HH-mm-ss"));
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            txtExportProgress.Text = string.Empty;
            string _filePath = txtLocation.Text.Trim();
            if (string.IsNullOrEmpty(_filePath))
            {
                GimjaHelper.ShowInformation("The file path is required. Please choose the file to export to.");
                return;
            }
            btnExport.Enabled = false;
            btnExportBrowse.Enabled = false;
            pgSyncImport.PageEnabled = false;
            FileStream _fileStream = null;
            if (!File.Exists(_filePath))
            {
                var _response = GimjaHelper.ShowQuestion("The file doesn't exist. Do you want to create?");
                if (_response == System.Windows.Forms.DialogResult.Yes)
                {
                    //_fileStream = new FileStream();
                    AppendExportProgress(string.Format("Creating the file {0}", _filePath));
                    _fileStream = File.Create(_filePath);
                }
            }
            else
            {//the file already exists
                var _response = GimjaHelper.ShowQuestion("The file already exists. Do you want to overwrite it?");
                if (_response == System.Windows.Forms.DialogResult.Yes)
                {
                    // _fileStream = new FileStream();
                    AppendExportProgress(string.Format("Opening file {0}", _filePath));
                    _fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Write);
                }
            }

            if (_fileStream != null)
            {
                AppendExportProgress("Creating document");
                var _syncDoc = SyncTransactionBL.GetDocument(GimjaHelper.GetCurrentUserID(this));
                if (_syncDoc != null)
                {
                    try
                    {
                        AppendExportProgress("Saving to file.");
                        _syncDoc.Save(_fileStream);
                        var _response = GimjaHelper.ShowQuestion(string.Format("The sync data is saved to the file {0}. Make sure to get a response file \n" +
                            "and run to avoid duplicate syncronizations.\n\n Do you want to open the location?", _filePath));
                        if (_response == System.Windows.Forms.DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", _filePath);
                        }
                        _fileStream.Close();
                    }
                    catch (Exception ex)
                    {
                        GimjaHelper.ShowError("Error while saving the sync data." + ex.Message);
                        AppendExportProgress("Export finished with error.");
                        _fileStream.Close();
                    }
                }
                else
                {
                    GimjaHelper.ShowInformation("There is no sync data to export.");
                }
            }
            btnExport.Enabled = true;
            btnExportBrowse.Enabled = true;
            pgSyncImport.PageEnabled = true;
        }

        private void AppendExportProgress(string p)
        {
            txtExportProgress.Text += (p + "\n");
        }

        private void btnImportBrowse_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog _dlg = new OpenFileDialog();
            _dlg.DefaultExt = ".xml";
            var _result = _dlg.ShowDialog();
            if (_result == System.Windows.Forms.DialogResult.OK)
            {
                var _fileName = _dlg.FileName;
                btnImportBrowse.Text = _fileName;
            }
        }
        private List<SyncLogData> _syncLogList;
        private void btnImport_Click(object sender, EventArgs e)
        {
            txtImportProgress.Text = string.Empty;
            string _filePath = btnImportBrowse.Text.Trim();
            if (string.IsNullOrEmpty(_filePath))
            {
                GimjaHelper.ShowInformation("The file path is required. Please choose the file.");
                return;
            }
            btnImport.Enabled = false;//disable the import button
            btnImportBrowse.Enabled = false;
            pgSyncExport.PageEnabled = false;
            var _userId = GimjaHelper.GetCurrentUserID(this);
            var _currentDate = DateTime.Today;
            _syncLogList = new List<SyncLogData>();
            int _syncInsertElemCount = 0;//the number of insert elements
            int _syncUpdateElemCount = 0;//the number of update elements
            XDocument _syncDoc = XDocument.Load(_filePath);
            var _syncRoot = _syncDoc.Elements();
            var _syncElements = _syncRoot.Elements("sync");
            try
            {
                AppendImportProgress("Start importing records.");
                #region Inserting New Records
                //Get the sync elements for the insert action
                var _syncInsertElems = _syncElements.Where(elem => elem.Attribute("action").Value.Equals("insert"));
                _syncInsertElemCount = _syncInsertElems.Count();
                if (_syncInsertElemCount > 0)
                    AppendImportProgress("Importing insert records.");
                var _tables = new string[] { "tblitem", "tbladdress", "tbltelephonefax" };
                var _syncInsertElem1 = _syncInsertElems.Where(elem => _tables.Contains(elem.Attribute("table").Value));
                foreach (var _elem in _syncInsertElem1)
                {
                    //getting the log data
                    var _syncId = _elem.Attribute("id").Value;
                    var _syncLog = new SyncLogData()
                    {
                        ID = Guid.NewGuid(),
                        UserID = _userId,
                        Date = _currentDate,
                        WarehouseID = null,//TODO: ADD THE CURRENT WAREHOUSE/BRANCH ID HERE
                        SyncTransactionID = new Guid(_syncId)
                    };
                    string _tblName = _elem.Attribute("table").Value.ToLower();
                    switch (_tblName)
                    {
                        case "tblitem":
                            ImportItemData(_elem, _syncLog);
                            break;
                        case "tbladdress":
                            ImportAddressData(_elem, _syncLog);
                            break;
                        case "tbltelephonefax":
                            ImportTelephoneFaxData(_elem, _syncLog);
                            break;
                        default:
                            break;
                    }
                }
                _syncInsertElems = _syncInsertElems.Where(_elem => !(_tables.Contains(_elem.Attribute("table").Value)));
                foreach (var _insertElem in _syncInsertElems)
                {
                    //getting the log data
                    var _syncId = _insertElem.Attribute("id").Value;
                    var _syncLog = new SyncLogData()
                    {
                        ID = Guid.NewGuid(),
                        UserID = _userId,
                        Date = _currentDate,
                        WarehouseID = null,//TODO: ADD THE CURRENT WAREHOUSE/BRANCH ID HERE
                        SyncTransactionID = new Guid(_syncId)
                    };
                    string _tblName = _insertElem.Attribute("table").Value.ToLower();
                    switch (_tblName)
                    {
                        case "tblbranch":
                            ImportBranchData(_insertElem, _syncLog);
                            break;
                        case "tblwarehouse":
                            ImportWarehouseData(_insertElem, _syncLog);
                            break;
                        case "tblcustomer":
                            ImportCustomerData(_insertElem, _syncLog);
                            break;
                        case "tbluser":
                            ImportUserData(_insertElem, _syncLog);
                            break;
                        case "tblmanufacturer":
                            ImportManufacturerData(_insertElem, _syncLog);
                            break;
                        case "tblsupplier":
                            ImportSupplierData(_insertElem, _syncLog);
                            break;
                        case "tblreceipt":
                            ImportReceiptData(_insertElem, _syncLog);
                            break;
                        case "tblsales":
                            ImportSalesData(_insertElem, _syncLog);
                            break;
                        case "tblissuance":
                            ImportIssuanceData(_insertElem, _syncLog);
                            break;
                        case "tblsalereturn":
                            ImportSaleReturnData(_insertElem, _syncLog);
                            break;
                        default:
                            break;
                    }
                }
                #endregion

                #region Updating Records
                //get the sync elements for the update action
                var _syncUpdateElems = _syncElements.Where(elem => elem.Attribute("action").Value.Equals("update"));
                _syncUpdateElemCount = _syncUpdateElems.Count();
                if (_syncUpdateElemCount > 0)
                    AppendImportProgress("Importing update records.");
                foreach (var _updateElem in _syncUpdateElems)
                {
                    //getting the log data
                    var _syncId = _updateElem.Attribute("id").Value;
                    var _syncLog = new SyncLogData()
                    {
                        ID = Guid.NewGuid(),
                        UserID = _userId,
                        Date = _currentDate,
                        WarehouseID = null,//TODO: ADD THE CURRENT WAREHOUSE/BRANCH ID HERE
                        SyncTransactionID = new Guid(_syncId)
                    };
                    string _tblName = _updateElem.Attribute("table").Value.ToLower();
                    switch (_tblName)
                    {
                        case "tblreceipt":
                            ImportReceiptData(_updateElem, _syncLog, true);
                            break;
                        case "tblsales":
                            ImportSalesData(_updateElem, _syncLog, true);
                            break;
                        case "tblissuance":
                            ImportIssuanceData(_updateElem, _syncLog, true);
                            break;
                        case "tblsalereturn":
                            ImportSaleReturnData(_updateElem, _syncLog, true);
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                AppendImportProgress("Save the log data.");
                //save the log data to database
                bool _result = SyncTransactionBL.SaveLog(_syncLogList);
                AppendImportProgress("Complete.");
            }
            catch (Exception ex)
            {
                GimjaHelper.ShowError("Unable to complete importing.\n" + ex.Message);
                AppendImportProgress("Import completed with error.");
            }
            int totalElems = _syncElements.Count();
            if (totalElems > 0)
                GimjaHelper.ShowInformation(string.Format("{0} element{1} found.\n{2} Inserts\n{3} Updates", totalElems,
                    (totalElems > 1 ? "s are" : " is"), _syncInsertElemCount, _syncUpdateElemCount));
            else
                GimjaHelper.ShowInformation("No elements found.");

            btnImport.Enabled = true;//enable the import button
            btnImportBrowse.Enabled = true;
            pgSyncExport.PageEnabled = true;
        }

        private void ImportSupplierData(XElement _insertElem, SyncLogData _syncLog)
        {
            SupplierData _supplier = SyncTransactionBL.GetSupplierData(_insertElem.ToString());
            if (_supplier != null)
            {
                SupplierBL supplierBL = new SupplierBL();

                AppendImportProgress("Supplier object found.");
                bool _exists = supplierBL.Exists(_supplier.SupplierID);
                if (!_exists)
                {//insert into the database
                    bool _isSaved = supplierBL.Add(_supplier, true);
                    if (_isSaved)
                    {
                        AppendImportProgress(string.Format("Supplier inserted ID: {0}", _supplier.SupplierID));
                        _syncLogList.Add(_syncLog);
                    }
                }
                else
                {
                    AppendImportProgress(string.Format("Supplier with ID {0} already exists.", _supplier.SupplierID));
                }
            }
        }

        private void ImportManufacturerData(XElement _insertElem, SyncLogData _syncLog)
        {
            ManufacturerData _manufacturer = SyncTransactionBL.GetManufacturerData(_insertElem.ToString());
            if (_manufacturer != null)
            {
                ManufacturerBL manufacturerBL = new ManufacturerBL();

                AppendImportProgress("Manufacturer object found.");
                bool _exists = manufacturerBL.Exists(_manufacturer.ManufacturerID);
                if (!_exists)
                {//insert into the database
                    bool _isSaved = manufacturerBL.Add(_manufacturer, true);
                    if (_isSaved)
                    {
                        AppendImportProgress(string.Format("Manufacturer inserted ID: {0}", _manufacturer.ManufacturerID));
                        _syncLogList.Add(_syncLog);
                    }
                }
                else
                {
                    AppendImportProgress(string.Format("Manufacturer with ID {0} already exists.", _manufacturer.ManufacturerID));
                }
            }
        }

        private void ImportUserData(XElement _insertElem, SyncLogData _syncLog)
        {
            UserData _user = SyncTransactionBL.GetUserData(_insertElem.ToString());
            if (_user != null)
            {
                UserBL userBL = new UserBL();

                AppendImportProgress("User object found.");
                bool _exists = userBL.Exists(_user.UserID);
                if (!_exists)
                {//insert into the database
                    bool _isSaved = userBL.Add(_user, true);
                    if (_isSaved)
                    {
                        AppendImportProgress(string.Format("User inserted ID: {0}", _user.UserID));
                        _syncLogList.Add(_syncLog);
                    }
                }
                else
                {
                    AppendImportProgress(string.Format("User with ID {0} already exists.", _user.UserID));
                }
            }
        }

        private void ImportCustomerData(XElement _insertElem, SyncLogData _syncLog)
        {
            CustomerData _customer = SyncTransactionBL.GetCustomerData(_insertElem.ToString());
            if (_customer != null)
            {
                CustomerBL customerBL = new CustomerBL();

                AppendImportProgress("Customer object found.");
                bool _exists = customerBL.Exists(_customer.ID);
                if (!_exists)
                {//insert into the database
                    bool _isSaved = customerBL.Add(_customer, true);
                    if (_isSaved)
                    {
                        AppendImportProgress(string.Format("Customer inserted ID: {0}", _customer.ID));
                        _syncLogList.Add(_syncLog);
                    }
                }
                else
                {
                    AppendImportProgress(string.Format("Customer with ID {0} already exists.", _customer.ID));
                }
            }
        }

        private void ImportWarehouseData(XElement _insertElem, SyncLogData _syncLog)
        {
            WarehouseData _warehouse = SyncTransactionBL.GetWarehouseData(_insertElem.ToString());
            if (_warehouse != null)
            {
                WarehouseBL warehouseBL = new WarehouseBL();

                AppendImportProgress("Warehouse object found.");
                bool _exists = warehouseBL.Exists(_warehouse.WarehouseID);
                if (!_exists)
                {//insert into the database
                    bool _isSaved = warehouseBL.Add(_warehouse, true);
                    if (_isSaved)
                    {
                        AppendImportProgress(string.Format("Warehouse inserted ID: {0}", _warehouse.WarehouseID));
                        _syncLogList.Add(_syncLog);
                    }
                }
                else
                {
                    AppendImportProgress(string.Format("Warehouse with ID {0} already exists.", _warehouse.WarehouseID));
                }
            }
        }

        private void ImportBranchData(XElement _insertElem, SyncLogData _syncLog)
        {
            BranchData _branch = SyncTransactionBL.GetBranchData(_insertElem.ToString());
            if (_branch != null)
            {
                BranchBL branchBL = new BranchBL();
                AppendImportProgress("Branch object found.");
                bool _exists = branchBL.Exists(_branch.ID);
                if (!_exists)
                {//insert into the database
                    bool _isSaved = branchBL.Add(_branch, true);
                    if (_isSaved)
                    {
                        AppendImportProgress(string.Format("Branch inserted ID: {0}", _branch.ID));
                        _syncLogList.Add(_syncLog);
                    }
                }
                else
                {
                    AppendImportProgress(string.Format("Branch with ID {0} already exists.", _branch.ID));
                }
            }
        }

        private void ImportTelephoneFaxData(XElement _elem, SyncLogData _syncLog)
        {
            TelephoneFaxData _tele = SyncTransactionBL.GetTelphoneFaxData(_elem.ToString());
            if (_tele != null)
            {
                TelephoneFaxBL telephoneFaxBL = new TelephoneFaxBL();

                AppendImportProgress("Telephone/Fax object found.");
                bool _exists = telephoneFaxBL.Exists(_tele.ID);
                if (!_exists)
                {//insert into the database
                    bool _isSaved = telephoneFaxBL.Add(_tele, true);
                    if (_isSaved)
                    {
                        AppendImportProgress(string.Format("Telephone/Fax inserted"));
                        _syncLogList.Add(_syncLog);
                    }
                }
                else
                {
                    AppendImportProgress(string.Format("Telephone/Fax already exists."));
                }
            }
        }

        private void ImportSaleReturnData(XElement _insertElem, SyncLogData _syncLog, bool update = false)
        {
            List<ReturnItemData> retItems;
            var _retObj = SyncTransactionBL.GetSaleReturnData(_insertElem.ToString(), out retItems);
            if (_retObj != null)
            {
                AppendImportProgress("A sale return object found.");
                var _exists = SaleReturnBL.Exists(_retObj.ID);
                if (!update)
                {
                    if (!_exists)
                    {//insert into the database
                        if (SaleReturnBL.CheckPrerequisites(_retObj, retItems))
                        {
                            var _isSaved = SaleReturnBL.Insert(_retObj, retItems);
                            if (_isSaved)
                            {
                                AppendImportProgress(string.Format("Sale Return inserted (Ref# {0})", _retObj.ReferenceNo));
                                _syncLogList.Add(_syncLog);
                            }
                        }
                        else
                        {
                            AppendImportProgress("Sale return object prerequisites failed.");
                        }
                    }
                    else
                    {
                        AppendImportProgress(string.Format("A sale return with Ref# {0} exists.", _retObj.ReferenceNo));
                    }
                }
                else
                {//it is updating a sale return object
                    if (_exists)
                    {
                        var _isSaved = SaleReturnBL.Update(_retObj, retItems);
                        if (_isSaved)
                        {
                            AppendImportProgress(string.Format("Sale Return updated (Ref# {0})", _retObj.ReferenceNo));
                            _syncLogList.Add(_syncLog);
                        }
                    }
                    else
                    {
                        AppendImportProgress(string.Format("A sale return record with Ref# {0} doesn't exist.", _retObj.ReferenceNo));
                    }
                }
            }
        }

        private void ImportIssuanceData(XElement _insertElem, SyncLogData _syncLog, bool update = false)
        {
            List<IssuedItemData> issueDetails;
            var _issueObj = SyncTransactionBL.GetIssuanceData(_insertElem.ToString(), out issueDetails);
            if (_issueObj != null)
            {
                AppendImportProgress("Issue object found.");
                var _exists = IssueBL.Exists(_issueObj.ID);
                if (!update)
                {
                    if (!_exists)
                    {//insert into database
                        if (IssueBL.CheckPrerequisites(_issueObj, issueDetails))
                        {
                            var _isSaved = IssueBL.Insert(_issueObj, issueDetails);
                            if (_isSaved)
                            {
                                AppendImportProgress(string.Format("Issue record inserted ID: {0}", _issueObj.ID));
                                _syncLogList.Add(_syncLog);
                            }
                        }
                        else
                        {
                            AppendImportProgress("Issuance object prerequisites failed.");
                        }
                    }
                    else
                    {
                        AppendImportProgress(string.Format("An issuance record with ID {0} exists", _issueObj.ID));
                    }
                }
                else
                {//it is updating an issuance record
                    if (_exists)
                    {
                        var _isSaved = IssueBL.Update(_issueObj, issueDetails);
                        if (_isSaved)
                        {
                            AppendImportProgress(string.Format("Issue record updated ID: {0}", _issueObj.ID));
                            _syncLogList.Add(_syncLog);
                        }
                    }
                    else
                    {
                        AppendImportProgress(string.Format("Issuance record with ID {0} doesn't exist.", _issueObj.ID));
                    }
                }
            }
        }

        private void ImportSalesData(XElement _insertElem, SyncLogData _syncLog, bool update = false)
        {
            List<SalesDetailData> saleDetails;
            var _saleObj = SyncTransactionBL.GetSalesData(_insertElem.ToString(), out saleDetails);
            if (_saleObj != null)
            {
                AppendImportProgress("Sale record found.");
                var _exists = SalesBL.Exists(_saleObj.ID);
                if (!update)
                {
                    if (!_exists)
                    {//insert into the database
                        if (SalesBL.CheckPrerequisites(_saleObj, saleDetails))
                        {
                            var _isSaved = SalesBL.Insert(_saleObj, saleDetails);
                            if (_isSaved)
                            {
                                AppendImportProgress(string.Format("Sale record is inserted with Ref# {0}.", _saleObj.ReferenceNo));
                                _syncLogList.Add(_syncLog);
                            }
                        }
                        else
                        {
                            AppendImportProgress("Sales object prerequisites failed.");
                        }
                    }
                    else
                    {
                        AppendImportProgress(string.Format("Sale with Ref# {0} exists.", _saleObj.ReferenceNo));
                    }
                }
                else
                {//it is updating the database
                    if (_exists)
                    {
                        var _isSaved = SalesBL.Update(_saleObj, saleDetails);
                        if (_isSaved)
                        {
                            AppendImportProgress(string.Format("Sale record is updated with Ref# {0}.", _saleObj.ReferenceNo));
                            _syncLogList.Add(_syncLog);
                        }
                    }
                    else
                    {
                        AppendImportProgress(string.Format("Sale object with Ref# {0} doesn't exist.", _saleObj.ReferenceNo));
                    }
                }
            }
        }

        private void ImportAddressData(XElement _elem, SyncLogData _syncLog)
        {
            AddressData _address = SyncTransactionBL.GetAddressData(_elem.ToString());
            if (_address != null)
            {
                AppendImportProgress("Address object found.");
                AddressBL addressBL = new AddressBL();

                bool _exists = addressBL.Exists(_address.ID);
                if (!_exists)
                {//insert into the database
                    var _isSaved = addressBL.Add(_address, true);
                    if (_isSaved)
                    {
                        AppendImportProgress(string.Format("Address inserted."));
                        _syncLogList.Add(_syncLog);
                    }
                }
                else
                {
                    AppendImportProgress(string.Format("Address already exists."));
                }
            }
        }

        private void ImportItemData(XElement _elem, SyncLogData _syncLog)
        {
            ItemData _item = SyncTransactionBL.GetItemData(_elem.ToString());
            if (_item != null)
            {
                ItemBL itemBL = new ItemBL();
                AppendImportProgress("Item object found.");
                bool _exists = itemBL.Exists(_item.ItemID);
                if (!_exists)
                {//insert into the database
                    var _isSaved = itemBL.Add(_item, true);
                    if (_isSaved)
                    {
                        AppendImportProgress(string.Format("Item inserted ID: {0}", _item.ItemID));
                        _syncLogList.Add(_syncLog);
                    }
                }
                else
                {
                    AppendImportProgress(string.Format("Item with ID {0} already exists.", _item.ItemID));
                }
            }
        }

        private void ImportReceiptData(XElement _insertElem, SyncLogData _syncLog, bool update = false)
        {
            List<ReceivedItemData> receivedItems;
            var _receiptObj = SyncTransactionBL.GetReceiptData(_insertElem.ToString(), out receivedItems);
            if (_receiptObj != null)
            {
                AppendImportProgress("Receipt object found.");
                bool _exists = ReceiptBL.Exists(_receiptObj.ID);
                if (!update)
                {//it is inserting a new receipt object
                    if (!_exists)
                    {//insert into the database
                        if (ReceiptBL.CheckPreRequisites(_receiptObj, receivedItems))
                        {
                            var _isSaved = ReceiptBL.Insert(_receiptObj, receivedItems, true);
                            if (_isSaved)
                            {
                                AppendImportProgress(string.Format("Receipt inserted ID: {0}", _receiptObj.ID));
                                _syncLogList.Add(_syncLog);
                            }
                        }
                        else
                        {
                            AppendImportProgress("Receipt object prerequisites failed.");
                        }
                    }
                    else
                    {
                        AppendImportProgress(string.Format("Receipt with ID {0} already exists.", _receiptObj.ID));
                    }
                }
                else
                {//it is updating an existing receipt
                    if (_exists)
                    {//update the database
                        var _isSaved = ReceiptBL.Update(_receiptObj, receivedItems, true);
                        if (_isSaved)
                        {
                            AppendImportProgress(string.Format("Receipt updated ID: {0}", _receiptObj.ID));
                            _syncLogList.Add(_syncLog);
                        }
                    }
                    else
                    {
                        AppendImportProgress(string.Format("No receipt record with ID {0} found.", _receiptObj.ID));
                    }
                }
            }
        }

        private void AppendImportProgress(string text)
        {
            txtImportProgress.Text += (text + "\n");
        }
    }
}