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
    public partial class frmEditReceipt : DevExpress.XtraEditors.XtraForm
    {
        private IList<ReceiptData> receiptList;
        private IList<ReceivedItemData> receiptDetails;
        private ReceiptData selectedReceipt;

        private bool isUpdating;

        public frmEditReceipt()
        {
            InitializeComponent();
            receiptList = new List<ReceiptData>();
            receiptDetails = new List<ReceivedItemData>();
            selectedReceipt = new ReceiptData();

            //Disable editing controls
            EnableControls(true);//make them readonly

            PopulateReceiptList();
            PopulateSuppliersList();
            PopulateStoresList();
            PopulateUsersList();
            PopulateManufacturersList();
            PopulateItemsList();
        }

        private void PopulateReceiptList()
        {
            receiptList = new List<ReceiptData>(ReceiptBL.GetActiveReceipts());
            grdReceiptList.DataSource = receiptList;
            if (receiptList.Count > 0)
            {
                UpdateReceiptDisplay();
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void UpdateReceiptDisplay()
        {
            var _selectedRowIndex = grdViewReceiptList.FocusedRowHandle;
            if (_selectedRowIndex >= 0 && _selectedRowIndex < receiptList.Count)
            {
                selectedReceipt = (ReceiptData)grdViewReceiptList.GetFocusedRow();
                if (selectedReceipt != null)
                {
                    lkeStoreId.EditValue = selectedReceipt.StoreID;
                    chkIsWarehouse.Checked = selectedReceipt.IsStoreWarehouse ?? false;
                    lkeSupplierId.EditValue = selectedReceipt.SupplierID;
                    dtReceiptDate.DateTime = selectedReceipt.Date;
                    lkeWarehouseId.EditValue = selectedReceipt.ReceivedFrom;
                    lkeReceivedBy.EditValue = selectedReceipt.ReceivedBy;
                    txtProcessedBy.Text = selectedReceipt.ProcessedBy;
                    txtApprovedBy.Text = selectedReceipt.ApprovedBy;
                    if (selectedReceipt.ApprovedDate.HasValue)
                        dtApprovedDate.DateTime = Convert.ToDateTime(selectedReceipt.ApprovedDate);
                    else
                        dtApprovedDate.Text = string.Empty;
                    //if (!string.IsNullOrEmpty(selectedReceipt.ApprovedBy))
                    if (selectedReceipt.IsApproved ?? false)
                    {//it is approved, hence unable to edit
                        btnEdit.Enabled = false;
                        btnDelete.Enabled = false;
                    }
                    //populate the details
                    receiptDetails = ReceiptBL.GetReceivedItems(selectedReceipt.ID);
                    grdReceiptDetails.DataSource = receiptDetails;

                    grdReceiptList.Enabled = true;
                    btnRefresh.Enabled = true;
                }
            }
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

            lkeStoreId.Properties.DataSource = _branchStoreList;
            lkeWarehouseId.Properties.DataSource = _branchStoreList;
        }

        private void PopulateSuppliersList()
        {
            SupplierBL supplierLogic = new SupplierBL();
            var _suppliers = supplierLogic.GetData();
            lkeSupplierId.Properties.DataSource = _suppliers;
            lkeRepositorySupplierID.DataSource = _suppliers;
        }

        private void PopulateManufacturersList()
        {
            ManufacturerBL _manufLogic = new ManufacturerBL();
            var _manufacturers = _manufLogic.GetData();
            lkeRepositoryManufacturer.DataSource = _manufacturers;
        }

        private void PopulateUsersList()
        {
            var _users = new UserBL().GetData(true);//.GetUsers();
            lkeReceivedBy.Properties.DataSource = _users;
        }

        private void PopulateItemsList()
        {
            ItemBL itemBL = new ItemBL();
            var _items = itemBL.GetData(true);
            lkeRepositoryItemID.DataSource = _items;
        }

        private void grdViewReceiptList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            selectedReceipt = (ReceiptData)grdViewReceiptList.GetRow(e.FocusedRowHandle);
            if (selectedReceipt != null)
            {//populate the controls
                lkeStoreId.EditValue = selectedReceipt.StoreID;
                lkeWarehouseId.EditValue = selectedReceipt.ReceivedFrom;
                chkIsWarehouse.Checked = selectedReceipt.IsStoreWarehouse ?? false;
                lkeSupplierId.EditValue = selectedReceipt.SupplierID;
                dtReceiptDate.DateTime = selectedReceipt.Date;
                txtProcessedBy.EditValue = selectedReceipt.ProcessedBy;
                lkeReceivedBy.EditValue = selectedReceipt.ReceivedBy;
                txtApprovedBy.Text = selectedReceipt.ApprovedBy;
                if (selectedReceipt.ApprovedDate.HasValue)
                    dtApprovedDate.DateTime = Convert.ToDateTime(selectedReceipt.ApprovedDate);
                else
                    dtApprovedDate.Text = string.Empty;
                if (selectedReceipt.IsApproved ?? false)
                {
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;
                    lblReceiptInfo.Text = "This receipt record is approved and can't be edited.";
                }
                else
                {
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = true;
                    lblReceiptInfo.Text = string.Empty;
                }
                //get the receipt details
                receiptDetails = ReceiptBL.GetReceivedItems(selectedReceipt.ID);
                grdReceiptDetails.DataSource = receiptDetails;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateReceiptList();
        }

        private void grdViewReceiptDetails_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            grdViewReceiptDetails.ClearColumnErrors();
            var _detail = (ReceivedItemData)e.Row;
            if (_detail != null)
                e.Valid = ValidateReceivedItemData(_detail);
        }

        private bool ValidateReceivedItemData(ReceivedItemData _detail)
        {
            bool _result = true;
            if (_detail != null)
            {
                if (string.IsNullOrEmpty(_detail.ItemID))
                {
                    grdViewReceiptDetails.SetColumnError(grdColItemId, "Item ID is required.");
                    _result = false;
                }
                //details with the same Item Id - are there duplicates
                var duplicateItemsCount = from d in receiptDetails
                                          group d by d.ItemID into ITEMS
                                          select ITEMS.Count();
                if (duplicateItemsCount.Any(x => x > 1))
                {
                    grdViewReceiptDetails.SetColumnError(grdColItemId, "Duplicate Items found.");
                    _result = false;
                }
                if (string.IsNullOrEmpty(_detail.ManufacturerID))
                {
                    grdViewReceiptDetails.SetColumnError(grdColManufacturerID, "ManufacturerID is required.");
                    _result = false;
                }
                if (_detail.NoPack.HasValue && _detail.NoPack < 0)
                {
                    grdViewReceiptDetails.SetColumnError(grdColNoPack, "Invalid no. of packs.");
                    _result = false;
                }
                if (_detail.QtyPerPack.HasValue && _detail.QtyPerPack < 0)
                {
                    grdViewReceiptDetails.SetColumnError(grdColQtyPerPack, "Invalid quantity per pack.");
                    _result = false;
                }
                if (_detail.Quantity <= 0)
                {
                    grdViewReceiptDetails.SetColumnError(grdColQuantity, "Invalid quantity.");
                    _result = false;
                }
                if (_detail.Price <= 0)
                {
                    grdViewReceiptDetails.SetColumnError(grdColPrice, "Invalid acquisition price.");
                    _result = false;
                }
                if (_detail.UnitSellingPrice <= 0 || _detail.UnitSellingPrice < _detail.Price)//TODO: IS UNIT SELLING PRICE LESS THAN/GREATER THAN PRICE
                {
                    grdViewReceiptDetails.SetColumnError(grdColUnitSellingPrice, "Invalid unit selling price.");
                    _result = false;
                }
            }
            else
                _result = false;

            return _result;
        }

        private void grdViewReceiptDetails_ShowingEditor(object sender, CancelEventArgs e)
        {
            bool _valid = ValidateReceipt();
            if (!_valid)
                e.Cancel = true;
        }

        private bool ValidateReceipt()
        {
            bool _result = true;
            if (string.IsNullOrEmpty(Convert.ToString(lkeStoreId.EditValue)))
            {
                _result = false;
                lkeStoreId.Focus();
                GimjaHelper.ShowError("The branch receiving the items is required.");
            }
            else if (string.IsNullOrEmpty(Convert.ToString(lkeSupplierId.EditValue)))
            {
                _result = false;
                lkeSupplierId.Focus();
                GimjaHelper.ShowError("The supplier is required.");
            }
            else if (dtReceiptDate.DateTime > DateTime.Now || dtReceiptDate.DateTime < DateTime.Today.AddDays(-5))
            {
                _result = false;
                dtReceiptDate.Focus();
                GimjaHelper.ShowError("The receipt date is required.");
            }
            else if (string.IsNullOrEmpty(Convert.ToString(lkeWarehouseId.EditValue)))
            {
                _result = false;
                lkeWarehouseId.Focus();
                GimjaHelper.ShowError("The store from which items are being received is required.");
            }
            else if (string.IsNullOrWhiteSpace(txtProcessedBy.Text))
            {
                _result = false;
                txtProcessedBy.Focus();
                GimjaHelper.ShowError("The user processing the request is required.");
            }
            else if (string.IsNullOrEmpty(Convert.ToString(lkeReceivedBy.EditValue)))
            {
                _result = false;
                lkeReceivedBy.Focus();
                GimjaHelper.ShowError("The user receiving the items is required.");
            }
            return _result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isUpdating)
            {
                bool _validReceipt = ValidateReceipt();
                if (_validReceipt)
                {
                    foreach (var _detail in receiptDetails)
                    {
                        _validReceipt = ValidateReceivedItemData(_detail);
                        if (!_validReceipt)
                            break;
                    }
                }
                if (_validReceipt)
                {//valid receipt data
                    DateTime _currentDateTime = DateTime.Now;
                    selectedReceipt.StoreID = Convert.ToString(lkeStoreId.EditValue);
                    selectedReceipt.IsStoreWarehouse = chkIsWarehouse.Checked;
                    selectedReceipt.ReceivedFrom = Convert.ToString(lkeWarehouseId.EditValue);
                    selectedReceipt.SupplierID = Convert.ToString(lkeSupplierId.EditValue);
                    selectedReceipt.Date = dtReceiptDate.DateTime;
                    selectedReceipt.ReceivedBy = Convert.ToString(lkeReceivedBy.EditValue);
                    selectedReceipt.ProcessedBy = txtProcessedBy.Text.Trim();
                    selectedReceipt.LastUpdatedBy = GimjaHelper.GetCurrentUserID(this); //"LogonUser";//TODO: ADD THE LOGON USER HERE
                    selectedReceipt.LastUpdatedDate = _currentDateTime;

                    try
                    {
                        //save to database
                        bool _result = ReceiptBL.Update(selectedReceipt, receiptDetails);
                        if (_result)
                        {
                            GimjaHelper.ShowInformation("The receipt data updated successfully.");
                            isUpdating = false;
                            EnableControls(true);//make them readonly
                            PopulateReceiptList();
                        }
                    }
                    catch (Exception ex)
                    {
                        GimjaHelper.ShowError("Error occured in updating receipt data. " + ex.Message);
                    }
                }
            }
        }

        private void grdViewReceiptDetails_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            var _newDetail = (ReceivedItemData)grdViewReceiptDetails.GetRow(e.RowHandle);
            if (_newDetail != null)
            {
                _newDetail.ReceiptID = selectedReceipt.ID;
                _newDetail.ReceiptDetailsID = Guid.NewGuid();
                _newDetail.CreatedBy = GimjaHelper.GetCurrentUserID(this); //"LogonUser";//TODO: ADD LOGON USER HERE.
                _newDetail.CreatedDate = DateTime.Now;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedReceipt != null && !string.IsNullOrEmpty(selectedReceipt.ID))
            {
                isUpdating = true;

                EnableControls(false);//they should not be readonly
                btnDelete.Enabled = false;
                btnCancel.Enabled = true;
                btnEdit.Enabled = false;
                btnSave.Enabled = true;
                //disable the receipts list
                grdReceiptList.Enabled = false;
                btnRefresh.Enabled = false;
                //update the manufacturer and item list
                PopulateLookupEdits();
            }
        }

        private void PopulateLookupEdits()
        {
            PopulateItemsList();
            PopulateManufacturersList();
            PopulateStoresList();
            PopulateSuppliersList();
            PopulateUsersList();
        }

        private void EnableControls(bool readonlyStatus)
        {
            lkeStoreId.Properties.ReadOnly = readonlyStatus;
            chkIsWarehouse.Properties.ReadOnly = readonlyStatus;
            lkeSupplierId.Properties.ReadOnly = readonlyStatus;
            dtReceiptDate.Properties.ReadOnly = readonlyStatus;
            lkeWarehouseId.Properties.ReadOnly = readonlyStatus;
            lkeReceivedBy.Properties.ReadOnly = readonlyStatus;
            //enable/disable buttons
            btnSave.Enabled = !readonlyStatus; btnEdit.Enabled = readonlyStatus;
            btnDelete.Enabled = readonlyStatus; btnCancel.Enabled = !readonlyStatus;

            EnableReceiptDetails(readonlyStatus);
            //make the store field control as the focused default control
            lkeStoreId.Focus();
        }

        private void EnableReceiptDetails(bool readonlyStatus)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn column in grdViewReceiptDetails.Columns)
            {
                column.OptionsColumn.AllowEdit = !readonlyStatus;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedReceipt != null && !string.IsNullOrEmpty(selectedReceipt.ID))
            {
                var _response = GimjaHelper.ShowQuestion("Are you sure to delete the currently displayed receipt data?");
                if (_response == System.Windows.Forms.DialogResult.Yes)
                {
                    selectedReceipt.IsDeleted = true;
                    bool _result = ReceiptBL.Delete(selectedReceipt);
                    if (_result)
                    {
                        UpdateReceiptDisplay();
                        EnableControls(true);
                        GimjaHelper.ShowInformation("The receipt data is deleted succesfully.");
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (isUpdating)
            {//only if the edit button was pressed
                grdViewReceiptDetails.ClearColumnErrors();//clear any errors

                isUpdating = false;
                EnableControls(true);//make them readonly
                UpdateReceiptDisplay();
            }
        }
    }
}