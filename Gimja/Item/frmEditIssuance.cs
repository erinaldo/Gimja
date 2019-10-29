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
    public partial class frmEditIssuance : DevExpress.XtraEditors.XtraForm
    {
        private IList<IssueData> issueList;
        private IList<IssuedItemData> issueDetails;
        private IssueData selectedIssue;

        private bool isUpdating;

        public frmEditIssuance()
        {
            InitializeComponent();
            issueList = new List<IssueData>();
            issueDetails = new List<IssuedItemData>();
            selectedIssue = new IssueData();

            //Disable editing controls
            EnableControls(true);//make them readonly

            PopulateIssueList();
            PopulateStoresList();
            PopulateUsersList();
            PopulateItemsList();
        }

        private void PopulateIssueList()
        {
            issueList = new List<IssueData>(IssueBL.GetIssueData());
            grdIssueList.DataSource = issueList;
            if (issueList.Count > 0)
            {
                UpdateIssueDisplay();
            }
            else
            {//the list is empty and no issuance data selected
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void UpdateIssueDisplay()
        {
            var _selectedRowIndex = grdViewIssueList.FocusedRowHandle;
            if (_selectedRowIndex >= 0 && _selectedRowIndex < issueList.Count)
            {
                selectedIssue = (IssueData)grdViewIssueList.GetFocusedRow();
                if (selectedIssue != null && !string.IsNullOrEmpty(selectedIssue.ID))
                {
                    txtIssueNo.Text = selectedIssue.ID;
                    lkeStoreId.EditValue = selectedIssue.StoreID;
                    dtIssueDate.DateTime = selectedIssue.Date;
                    lkeWarehouseId.EditValue = selectedIssue.WarehouseID;
                    lkeIssuedTo.EditValue = selectedIssue.IssuedTo;
                    lkeIssuedBy.EditValue = selectedIssue.IssuedBy;
                    txtApprovedBy.Text = selectedIssue.ApprovedBy;
                    if (selectedIssue.ApprovedDate.HasValue)
                        dtApprovedDate.DateTime = Convert.ToDateTime(selectedIssue.ApprovedDate);
                    else
                        dtApprovedDate.Text = string.Empty;
                    //if (!string.IsNullOrEmpty(selectedReceipt.ApprovedBy))
                    if (!string.IsNullOrEmpty(selectedIssue.ApprovedBy))
                    {//it is approved, hence unable to edit
                        btnEdit.Enabled = false;
                        btnDelete.Enabled = false;
                    }
                    //populate the details
                    issueDetails = IssueBL.GetIssueDetails(selectedIssue.ID);
                    grdIssueDetails.DataSource = issueDetails;

                    grdIssueList.Enabled = true;
                    btnRefresh.Enabled = true;
                }
                else
                {//the list is empty and no issuance data selected
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;
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

        private void PopulateUsersList()
        {
            var _users = new UserBL().GetData(true);//.GetUsers();
            lkeIssuedTo.Properties.DataSource = _users;
        }

        private void PopulateItemsList()
        {
            ItemBL itemBL = new ItemBL();
            var _items = itemBL.GetData(true);
            lkeRepositoryItemID.DataSource = _items;
        }

        private void grdViewReceiptList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            selectedIssue = (IssueData)grdViewIssueList.GetRow(e.FocusedRowHandle);
            if (selectedIssue != null)
            {//populate the controls
                lkeStoreId.EditValue = selectedIssue.StoreID;
                lkeWarehouseId.EditValue = selectedIssue.WarehouseID;
                dtIssueDate.DateTime = selectedIssue.Date;
                lkeIssuedBy.EditValue = selectedIssue.IssuedBy;
                lkeIssuedTo.EditValue = selectedIssue.IssuedTo;
                txtApprovedBy.Text = selectedIssue.ApprovedBy;
                if (selectedIssue.ApprovedDate.HasValue)
                    dtApprovedDate.DateTime = Convert.ToDateTime(selectedIssue.ApprovedDate);
                else
                    dtApprovedDate.Text = string.Empty;
                if (!string.IsNullOrEmpty(selectedIssue.ApprovedBy))
                {
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;
                    lblIssuanceInfo.Text = "the issuance record is approved and can't be edited.";
                }
                else
                {
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = true;
                    lblIssuanceInfo.Text = string.Empty;
                }
                //get the receipt details
                issueDetails = IssueBL.GetIssueDetails(selectedIssue.ID);
                grdIssueDetails.DataSource = issueDetails;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateIssueList();
        }

        private void grdViewReceiptDetails_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            grdViewIssueDetails.ClearColumnErrors();
            var _detail = (IssuedItemData)e.Row;
            if (_detail != null)
                e.Valid = ValidateIssuedItemData(_detail);
        }

        private bool ValidateIssuedItemData(IssuedItemData _detail)
        {
            bool _result = true;
            if (_detail != null)
            {
                if (string.IsNullOrEmpty(_detail.ItemID))
                {
                    grdViewIssueDetails.SetColumnError(grdColItemId, "Item ID is required.");
                    _result = false;
                }
                string branchId = Convert.ToString(lkeStoreId.EditValue);
                if (!string.IsNullOrEmpty(_detail.ItemID))
                {
                    var _item = ItemBL.GetAvailableItems(branchId, _detail.ItemID).FirstOrDefault();
                    if (_item != null && _item.Available < _detail.Quantity)
                    {
                        grdViewIssueDetails.SetColumnError(grdColQuantity, "Invalid quantity.");
                        _result = false;
                    }
                }
                //details with the same Item Id - are there duplicates
                var duplicateItemsCount = from d in issueDetails
                                          group d by d.ItemID into ITEMS
                                          select ITEMS.Count();
                if (duplicateItemsCount.Any(x => x > 1))
                {
                    grdViewIssueDetails.SetColumnError(grdColItemId, "Duplicate Items found.");
                    _result = false;
                }
                if (_detail.NoPack.HasValue && _detail.NoPack < 0)
                {
                    grdViewIssueDetails.SetColumnError(grdColNoPack, "Invalid no. of packs.");
                    _result = false;
                }
                if (_detail.QtyPerPack.HasValue && _detail.QtyPerPack < 0)
                {
                    grdViewIssueDetails.SetColumnError(grdColQtyPerPack, "Invalid quantity per pack.");
                    _result = false;
                }
                if (_detail.Quantity <= 0)
                {
                    grdViewIssueDetails.SetColumnError(grdColQuantity, "Invalid quantity.");
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
            else if (dtIssueDate.DateTime > DateTime.Now || dtIssueDate.DateTime < DateTime.Today.AddDays(-5))
            {
                _result = false;
                dtIssueDate.Focus();
                GimjaHelper.ShowError("The receipt date is required.");
            }
            else if (string.IsNullOrEmpty(Convert.ToString(lkeWarehouseId.EditValue)))
            {
                _result = false;
                lkeWarehouseId.Focus();
                GimjaHelper.ShowError("The store from which items are being received is required.");
            }
            else if (string.IsNullOrWhiteSpace(Convert.ToString(lkeIssuedBy.EditValue)))
            {
                _result = false;
                lkeIssuedBy.Focus();
                GimjaHelper.ShowError("The user processing the request is required.");
            }
            else if (string.IsNullOrEmpty(Convert.ToString(lkeIssuedTo.EditValue)))
            {
                _result = false;
                lkeIssuedTo.Focus();
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
                    foreach (var _detail in issueDetails)
                    {
                        _validReceipt = ValidateIssuedItemData(_detail);
                        if (!_validReceipt)
                            break;
                    }
                }
                if (_validReceipt)
                {//valid receipt data
                    DateTime _currentDateTime = DateTime.Now;
                    selectedIssue.StoreID = Convert.ToString(lkeStoreId.EditValue);
                    selectedIssue.WarehouseID = Convert.ToString(lkeWarehouseId.EditValue);
                    selectedIssue.Date = dtIssueDate.DateTime;
                    selectedIssue.IssuedTo = Convert.ToString(lkeIssuedTo.EditValue);
                    selectedIssue.IssuedBy = Convert.ToString(lkeIssuedBy.EditValue);
                    selectedIssue.LastUpdatedBy = GimjaHelper.GetCurrentUserID(this); //"LogonUser";//TODO: ADD THE LOGON USER HERE
                    selectedIssue.LastUpdatedDate = _currentDateTime;

                    try
                    {
                        //save to database
                        bool _result = IssueBL.Update(selectedIssue, issueDetails);
                        if (_result)
                        {
                            GimjaHelper.ShowInformation("The receipt data updated successfully.");
                            isUpdating = false;
                            EnableControls(true);//make them readonly
                            PopulateIssueList();
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
            var _newDetail = (IssuedItemData)grdViewIssueDetails.GetRow(e.RowHandle);
            if (_newDetail != null)
            {
                _newDetail.IssuanceID = selectedIssue.ID;
                _newDetail.IssueDetailID = Guid.NewGuid();
                _newDetail.CreatedBy = GimjaHelper.GetCurrentUserID(this); // "LogonUser";//TODO: ADD LOGON USER HERE.
                _newDetail.CreatedDate = DateTime.Now;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedIssue != null && !string.IsNullOrEmpty(selectedIssue.ID))
            {
                isUpdating = true;

                EnableControls(false);//they should not be readonly
                btnDelete.Enabled = false;
                btnCancel.Enabled = true;
                btnEdit.Enabled = false;
                btnSave.Enabled = true;
                //disable the receipts list
                grdIssueList.Enabled = false;
                btnRefresh.Enabled = false;
                //update the manufacturer and item list
                PopulateLookupEdits();
            }
        }

        private void PopulateLookupEdits()
        {
            PopulateItemsList();
            PopulateStoresList();
            PopulateUsersList();
        }

        private void EnableControls(bool readonlyStatus)
        {
            lkeStoreId.Properties.ReadOnly = readonlyStatus;
            dtIssueDate.Properties.ReadOnly = readonlyStatus;
            lkeWarehouseId.Properties.ReadOnly = readonlyStatus;
            lkeIssuedTo.Properties.ReadOnly = readonlyStatus;
            //enable/disable buttons
            btnSave.Enabled = !readonlyStatus; btnEdit.Enabled = readonlyStatus;
            btnDelete.Enabled = readonlyStatus; btnCancel.Enabled = !readonlyStatus;

            EnableReceiptDetails(readonlyStatus);
            //make the store field control as the focused default control
            lkeStoreId.Focus();
        }

        private void EnableReceiptDetails(bool readonlyStatus)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn column in grdViewIssueDetails.Columns)
            {
                column.OptionsColumn.AllowEdit = !readonlyStatus;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var _response = GimjaHelper.ShowQuestion("Are you sure to delete the currently displayed receipt data?");
            if (_response == System.Windows.Forms.DialogResult.Yes)
            {
                selectedIssue.IsDeleted = true;
                bool _result = IssueBL.Delete(selectedIssue);
                if (_result)
                {
                    UpdateIssueDisplay();
                    EnableControls(true);
                    GimjaHelper.ShowInformation("The receipt data is deleted succesfully.");
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (isUpdating)
            {//only if the edit button was pressed
                grdViewIssueDetails.ClearColumnErrors();//clear any errors

                isUpdating = false;
                EnableControls(true);//make them readonly
                UpdateIssueDisplay();
            }
        }

        private void frmEditIssuance_Load(object sender, EventArgs e)
        {

        }
    }
}