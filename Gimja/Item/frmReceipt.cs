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
using GimjaBL;

namespace Gimja
{
    public partial class frmReceipt : DevExpress.XtraEditors.XtraForm
    {
        private ReceiptData receipt;
        private IList<ReceivedItemData> receiptDetails;
        private IList<ItemSelection> itemSelectionList;
        public class ItemSelection
        {
            public bool Selected { get; set; }
            public string ItemID { get; set; }
            public int BrandID { get; set; }
            public int CategoryID { get; set; }
            public Nullable<double> UnitPrice { get; set; }
            public Nullable<double> ReorderLevel { get; set; }
            public Nullable<double> PickFaceQty { get; set; }
            public Nullable<int> UnitID { get; set; }
            public Nullable<double> OrderQuantity { get; set; }
            public string Description { get; set; }
            public Nullable<bool> IsActive { get; set; }
            public Nullable<int> TaxTypeID { get; set; }
            public Nullable<bool> IsTaxExempted { get; set; }
        }

        public frmReceipt()
        {
            InitializeComponent();
            receipt = new ReceiptData();
            receiptDetails = new List<ReceivedItemData>();

            tabStep2.PageEnabled = false;
            tabStep3.PageEnabled = false;

            PopulateItemList();
        }

        private void PopulateItemList()
        {
            ItemBL itemBL = new ItemBL();
            var _items = itemBL.GetData(true); //.GetItems();
            itemSelectionList = (from i in _items
                                 select new ItemSelection()
                                 {
                                     Selected = false,
                                     ItemID = i.ItemID,
                                     BrandID = i.BrandID,
                                     CategoryID = i.CategoryID,
                                     UnitPrice = i.UnitPrice,
                                     ReorderLevel = i.ReorderLevel,
                                     PickFaceQty = i.PickFaceQty,
                                     UnitID = i.UnitID,
                                     OrderQuantity = i.OrderQuantity,
                                     Description = i.Description,
                                     IsActive = i.IsActive,
                                     TaxTypeID = i.TaxTypeID,
                                     IsTaxExempted = i.IsTaxExempted
                                 }).ToList();
            //set as datasource
            grdItem.DataSource = itemSelectionList;
        }

        private void PopulateStoresList()
        {
            BranchBL _branchBL = new BranchBL();
            var _branches = _branchBL.GetData(true);
            WarehouseBL _warehouseBL = new WarehouseBL();
            var _warehouses = _warehouseBL.GetData(true);

            var _branchStoreList = (from b in _branches select new { ID = b.ID, Name = b.Name }).ToList();
            var _warehouseStoreList = (from w in _warehouses select new { ID = w.WarehouseID, Name = w.Name }).ToList();
            _branchStoreList.AddRange(_warehouseStoreList);

            lkeStore.Properties.DataSource = _branchStoreList;
            lkeReceivedFrom.Properties.DataSource = _branchStoreList;
        }

        private void PopulateSuppliersList()
        {
            SupplierBL supplierLogic = new SupplierBL();
            var _suppliers = supplierLogic.GetData();
            lkeSupplier.Properties.DataSource = _suppliers;
        }

        private void PopulateManufacturersList()
        {
            ManufacturerBL _manufLogic = new ManufacturerBL();
            var _manufacturers = _manufLogic.GetData();
            lkeRepositoryManufacturer.DataSource = _manufacturers;
            lkeRepositoryPreviewManufacturer.DataSource = _manufacturers;
        }

        private void PopulateUsersList()
        {
            var _users = new UserBL().GetData(true);//.GetUsers();
            lkeReceivedBy.Properties.DataSource = _users;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            receiptDetails = new List<ReceivedItemData>();
            //get the selected items
            for (int k = 0; k < grdViewItem.RowCount; k++)
            {
                bool _isSelected = Convert.ToBoolean(grdViewItem.GetRowCellValue(k, grdColItemSelected));
                if (_isSelected)
                {
                    ItemSelection _row = (ItemSelection)grdViewItem.GetRow(k);
                    if (_row != null)
                    {//the selected item row
                        receiptDetails.Add(new ReceivedItemData()
                        {
                            ItemID = _row.ItemID,
                            UnitSellingPrice = _row.UnitPrice ?? 0d,
                            Price = _row.UnitPrice ?? 0d,
                            ReceiptDetailsID = Guid.NewGuid()
                        });
                    }
                }
            }
            if (receiptDetails.Count > 0)
            {//there are selected items to be received
                //set the received item datasource
                grdReceivedItem.DataSource = receiptDetails;
                tabStep2.PageEnabled = true;
                tabReceiveItem.SelectedTabPage = tabStep2;
                //get the logon user as processedby user
                txtProcessedBy.Text = GimjaHelper.GetCurrentUserID(this);// "LogonUser";//TODO: ADD THE LOGON USER HERE.
            }
            else
            {
                GimjaHelper.ShowError("There are no selected items to receive.");
            }
        }

        private void tabReceiveItem_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            if (e.Page == tabStep2)
            {
                PopulateStoresList();
                PopulateSuppliersList();
                PopulateUsersList();
                PopulateManufacturersList();
            }
            else if (e.Page == tabStep3)
            {
                PopulateManufacturersList();
            }
        }

        private void tabReceiveItem_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page == tabStep1)
            {
                //disable the other two tab pages
                tabStep2.PageEnabled = false;
                tabStep3.PageEnabled = false;
            }
            else if (e.Page == tabStep2)
            {
                tabStep2.PageEnabled = true;
                tabStep3.PageEnabled = false;
            }
            else if (e.Page == tabStep3)
            {
                tabStep3.PageEnabled = true;
            }
        }

        private void grdReceivedItem_Enter(object sender, EventArgs e)
        {
            ValidateReceiptData();
            //the receipt is validated

        }

        private bool ValidateReceiptData()
        {
            if (string.IsNullOrEmpty(Convert.ToString(lkeStore.EditValue)))
            {
                lkeStore.Focus();
                GimjaHelper.ShowError("The Store to which the items are going is required.");
                return false;
            }
            if (string.IsNullOrEmpty(Convert.ToString(lkeReceivedFrom.EditValue)))
            {
                lkeReceivedFrom.Focus();
                GimjaHelper.ShowError("The store from which items will be received is required.");
                return false;
            }
            if (Convert.ToString(lkeStore.EditValue).Equals(Convert.ToString(lkeReceivedFrom.EditValue)))
            {
                lkeReceivedFrom.Focus();
                GimjaHelper.ShowError("The receipient store and the warehouse cannot be similar.");
                return false;
            }
            if (string.IsNullOrEmpty(Convert.ToString(lkeSupplier.EditValue)))
            {
                lkeSupplier.Focus();
                GimjaHelper.ShowError("The supplier of the items being received is required.");
                return false;
            }
            if (dtReceivedDate.DateTime > DateTime.Now || dtReceivedDate.DateTime < DateTime.Today.AddDays(-5))
            {
                dtReceivedDate.Focus();
                GimjaHelper.ShowError("Ivalid receipt date.");
                return false;
            }
            if (string.IsNullOrEmpty(Convert.ToString(lkeReceivedBy.EditValue)))
            {
                lkeReceivedBy.Focus();
                GimjaHelper.ShowError("The user receiving the items is required.");
                return false;
            }
            if (string.IsNullOrEmpty(Convert.ToString(txtProcessedBy.Text)))
            {
                txtProcessedBy.Focus();
                GimjaHelper.ShowError("The user handling the receipt is required.");
                return false;
            }
            return true;
        }

        private void grdViewReceivedItem_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            grdViewReceivedItem.ClearColumnErrors();
            var _detail = (ReceivedItemData)e.Row;
            if (_detail != null)
            {
                e.Valid = ValidateReceiptDetail(_detail);
            }
        }

        private bool ValidateReceiptDetail(ReceivedItemData _detail)
        {
            bool _result = true;
            if (_detail == null)
                _result = false;
            else
            {
                if (_detail.ReceiptDetailsID == Guid.Empty)
                {
                    _result = false;
                }
                if (_detail.Quantity <= 0)
                {
                    grdViewReceivedItem.SetColumnError(grdColReceivedItemQty, "Invalid quantity!");
                    _result = false;
                }
                if (_detail.UnitSellingPrice <= 0)
                {
                    grdViewReceivedItem.SetColumnError(grdColReceivedItemUnitSellingPrice, "Invalid unit selling price!");
                    _result = false;
                }
                if (_detail.Price <= 0)
                {
                    grdViewReceivedItem.SetColumnError(grdColReceivedItemPrice, "Invalid price!");
                    _result = false;
                }
                if (string.IsNullOrWhiteSpace(_detail.ItemID))
                {
                    grdViewReceivedItem.SetColumnError(grdColReceivedItemID, "Item ID is required.");
                    _result = false;
                }
                if (string.IsNullOrWhiteSpace(_detail.ManufacturerID))
                {
                    grdViewReceivedItem.SetColumnError(grdColReceivedItemManufacturer, "Manufacturer ID is required.");
                    _result = false;
                }
            }
            return _result;
        }

        private void btnNext2_Click(object sender, EventArgs e)
        {
            bool _isValidReceipt = ValidateReceiptData();
            if (_isValidReceipt)
            {
                #region Validate Receipt Details
                bool _isValidReceiptDetail = true;
                for (int i = 0; i < grdViewReceivedItem.RowCount; i++)
                {
                    var _row = grdViewReceivedItem.GetRow(i);
                    var _detail = (ReceivedItemData)_row;
                    if (_detail != null)
                    {
                        _isValidReceiptDetail = ValidateReceiptDetail(_detail);
                        if (!_isValidReceiptDetail)
                        {
                            grdViewReceivedItem.Focus();
                            break;
                        }
                    }
                }
                #endregion
                if (_isValidReceiptDetail)
                {
                    var _branchId = Convert.ToString(lkeStore.EditValue);
                    //data is valid
                    receipt = new ReceiptData()
                    {
                        ID = ReceiptBL.GetReceiptNumber(_branchId),
                        StoreID = _branchId,
                        IsStoreWarehouse = ckIsWarehouse.Checked,
                        ReceivedFrom = Convert.ToString(lkeReceivedFrom.EditValue),
                        SupplierID = Convert.ToString(lkeSupplier.EditValue),
                        Date = dtReceivedDate.DateTime,
                        ReceivedBy = Convert.ToString(lkeReceivedBy.EditValue),
                        ProcessedBy = txtProcessedBy.Text.Trim(),
                        CreatedBy = GimjaHelper.GetCurrentUserID(this), //"LogonUser",//TODO: ADD THE LOGON USER HERE
                        CreatedDate = DateTime.Now
                    };
                    foreach (var _d in receiptDetails)
                    {
                        _d.ReceiptID = receipt.ID;
                        _d.CreatedBy = GimjaHelper.GetCurrentUserID(this); //"LogonUser";//TODO: ADD THE LOGON USER HERE
                        _d.CreatedDate = DateTime.Now;
                    }
                    //prepare to preview
                    string previewReceipt = string.Format("{0} items received from '{1}' by {2}\n" +
                        "Store: \t{3} (Is Warehouse - {7})\nDate: \t{4}\nSupplier: \t{5}\nUser: \t{6}\n", receiptDetails.Count, receipt.ReceivedFrom,
                        receipt.ReceivedBy, receipt.StoreID, receipt.Date, lkeSupplier.Text, receipt.ProcessedBy, (receipt.IsStoreWarehouse ?? false) ? "Yes" : "No");
                    lblReceiptData.Text = previewReceipt;
                    grdPreviewReceiptDetails.DataSource = receiptDetails;
                    tabStep3.PageEnabled = true;
                    tabReceiveItem.SelectedTabPage = tabStep3;
                }
                else
                    GimjaHelper.ShowError("Invalid receipt detail information to save.");
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            //insert the receipt with details and reset the tab to 1st page
            bool _result = ReceiptBL.Insert(receipt, receiptDetails);
            if (_result)
            {
                GimjaHelper.ShowInformation("The receipt data is inserted successfully.");
                receipt = new ReceiptData();
                receiptDetails = new List<ReceivedItemData>();
                tabReceiveItem.SelectedTabPage = tabStep1;
            }
        }
    }
}