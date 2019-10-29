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
using DevExpress.XtraGrid.Views.Grid;

namespace Gimja
{
    public partial class frmSalesReturn : DevExpress.XtraEditors.XtraForm
    {
        private bool isAdding;
        private bool isUpdating;

        private SaleReturnData returnData;
        private IList<ReturnItemData> returnedItemsList;
        private SalesData selectedSales;
        private ItemData selectedItem;
        private IList<SaleReturnData> returnList;

        public frmSalesReturn()
        {
            InitializeComponent();
            returnList = new List<SaleReturnData>();
            selectedSales = new SalesData();

            ClearControls();
            EnableControls(true);
        }

        private void ClearControls()
        {
            lkeSalesRefNo.EditValue = null;
            dtReturnDate.DateTime = DateTime.Now;
            lkeUsers.EditValue = null;
            txtReason.Text = string.Empty;

            returnedItemsList = new List<ReturnItemData>();
            grdReturnDetails.DataSource = returnedItemsList;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            isAdding = true;
            EnableControls(false);
            ClearControls();
        }

        private void EnableControls(bool readOnly)
        {
            lkeSalesRefNo.Properties.ReadOnly = readOnly;
            dtReturnDate.Properties.ReadOnly = readOnly;
            lkeUsers.Properties.ReadOnly = readOnly;
            txtReason.Properties.ReadOnly = readOnly;

            btnAdd.Enabled = readOnly; btnSave.Enabled = !readOnly;
            btnEdit.Enabled = readOnly; btnDelete.Enabled = readOnly;
            btnCancel.Enabled = !readOnly;

            EnableReturnDetails(readOnly);
        }

        private void EnableReturnDetails(bool readOnly)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in grdViewSaleReturns.Columns)
            {
                if (col == grdColItemId || col == grdColQuantity || col == grdColRefundAmt)
                {
                    col.OptionsColumn.AllowEdit = !readOnly;
                }
            }
        }

        private void grdReturnDetails_Enter(object sender, EventArgs e)
        {
            grdViewReturnDetails_GotFocus(grdReturnDetails, e);
        }

        private void grdViewReturnDetails_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column == grdColItemId)
            {//the item id is changed or set
                var _item = grdViewReturnDetails.GetRowCellValue(e.RowHandle, e.Column).ToString();
                if (!string.IsNullOrEmpty(_item))
                {
                    selectedItem = ItemBL.GetItem(_item);
                    if (selectedItem != null)
                    {

                    }
                }
            }
            if (e.Column == grdColRefundAmt)
            {
                CalculateTotalRefundAmount();
            }
        }

        private void grdViewReturnDetails_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {

        }

        private void grdViewReturnDetails_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view != null)
            {
                var _qtyVal = Convert.ToString(view.GetRowCellValue(e.RowHandle, grdColQuantity));
                long _qty;
                bool isValidQty = long.TryParse(_qtyVal, out _qty);
                if ((isValidQty && _qty <= 0) || !isValidQty)
                {
                    e.Valid = false;
                    view.SetColumnError(grdColQuantity, "Invalid Quantity.");
                    return;
                }

                var _refund = Convert.ToDouble(view.GetRowCellValue(e.RowHandle, grdColRefundAmt));
                if (_refund <= 0)
                {
                    e.Valid = false;
                    view.SetColumnError(grdColRefundAmt, "Invalid Refund Amount.");
                    return;
                }

                var _item = Convert.ToString(view.GetRowCellValue(e.RowHandle, grdColItemId));
                if (!string.IsNullOrEmpty(_item))
                {//check for duplicate item selection
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        if (i != e.RowHandle)
                        {
                            var _thisItem = view.GetRowCellValue(i, grdColItemId).ToString();
                            if (_thisItem.Equals(_item))
                            {
                                e.Valid = false;
                                view.SetColumnError(grdColItemId, "Duplicate item selection.");
                                return;
                            }
                        }
                    }

                    var _selectedSalesDetail = SalesDetailBL.GetSalesDetail(selectedSales.ID, _item);
                    if (isValidQty && _selectedSalesDetail.Quantity < _qty)
                    {
                        e.Valid = false;
                        view.SetColumnError(grdColQuantity, "Return quantity should be utmost equal to sold quantity.");
                        return;
                    }
                }
                else
                {//no item selected
                    e.Valid = false;
                    view.SetColumnError(grdColItemId, "Item selection is required.");
                    return;
                }
            }
        }

        private void grdViewReturnDetails_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region create a return object
            var _refNo = lkeSalesRefNo.EditValue.ToString();
            var _returnDate = dtReturnDate.DateTime;
            var _user = lkeUsers.EditValue.ToString();
            var _reason = txtReason.Text.Trim();
            if (string.IsNullOrEmpty(_reason))
            {
                txtReason.Focus();
                GimjaHelper.ShowError("Reason is required.");
                return;
            }
            if (isAdding || isUpdating)
            {
                if (returnedItemsList == null || returnedItemsList.Count <= 0)
                {
                    grdViewReturnDetails.Focus();
                    GimjaHelper.ShowError("You need to select return details before saving the return data.");
                    return;
                }
                returnData = new SaleReturnData()
                {
                    SalesID = selectedSales.ID,
                    ProcessedBy = _user,
                    Date = _returnDate,
                    Reason = _reason
                };
            }
            #endregion
            #region Adding New Return Data
            if (isAdding)
            {
                returnData.CreatedBy = GimjaHelper.GetCurrentUserID(this); //"LogonUser";//TODO: ADD LOGON USER HERE
                foreach (var _returnedItem in returnedItemsList)
                {
                    _returnedItem.CreatedBy = GimjaHelper.GetCurrentUserID(this); //"LogonUser";//TODO: ADD LOGON USER HERE
                }
                bool _result = SaleReturnBL.Insert(returnData, returnedItemsList);
                if (_result)
                {
                    GimjaHelper.ShowInformation("Inserted successfully.");
                    EnableControls(true);
                    btnAdd.Enabled = true;
                    btnSave.Enabled = false; btnCancel.Enabled = false;
                    btnDelete.Enabled = true;

                    isAdding = false;
                }
            }
            #endregion
            #region Editing a return data
            else if (isUpdating)
            {
                returnData.LastUpdatedBy = GimjaHelper.GetCurrentUserID(this); //"LogonUser";//TODO: ADD LOGON USER HERE
                foreach (var _returnedItem in returnedItemsList)
                {
                    _returnedItem.LastUpdatedBy = GimjaHelper.GetCurrentUserID(this); //"LogonUser";//TODO: ADD LOGON USER HERE
                }

                bool _result = SaleReturnBL.Update(returnData, returnedItemsList);
                if (_result)
                {
                    GimjaHelper.ShowInformation("Updated successfully.");
                    EnableControls(true);
                    btnAdd.Enabled = true;
                    btnSave.Enabled = false; btnCancel.Enabled = false;
                    btnDelete.Enabled = true;

                    isUpdating = false;
                }
            }
            #endregion
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (grdViewSaleReturns.FocusedRowHandle < 0)
            {
                GimjaHelper.ShowError("Unable to edit a return data.");
                return;
            }
            UpdateSaleReturnDisplay();

            isUpdating = true;

            EnableControls(false);
            btnAdd.Enabled = false;
            btnEdit.Enabled = false; btnSave.Enabled = true; btnDelete.Enabled = false; btnCancel.Enabled = true;
            grdSaleReturns.Enabled = false;
        }

        private void UpdateSaleReturnDisplay()
        {
            int _currentRowHandle = grdViewSaleReturns.FocusedRowHandle;
            if (_currentRowHandle >= 0)
            {
                returnData = (SaleReturnData)grdViewSaleReturns.GetFocusedRow();
                dtReturnDate.DateTime = returnData.Date;
                txtReason.Text = returnData.Reason;
                lkeUsers.EditValue = returnData.ProcessedBy;

                //get the return details
                returnedItemsList = SaleReturnBL.GetReturnedItems(returnData.ID);
                grdReturnDetails.DataSource = returnedItemsList;

                CalculateTotalRefundAmount();

                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void CalculateTotalRefundAmount()
        {
            double _totalRefundAmt = 0;
            for (int i = 0; i < grdViewReturnDetails.RowCount; i++)
            {
                var _refundAmt = Convert.ToDouble(grdViewReturnDetails.GetRowCellValue(i, grdColRefundAmt));
                _totalRefundAmt += _refundAmt;
            }
            txtTotalRefundAmount.Text = _totalRefundAmt.ToString("f2");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var _response = GimjaHelper.ShowQuestion("Are you sure to remove the selected sales return data?");
            if (_response == System.Windows.Forms.DialogResult.Yes)
            {
                var _currentRecord = (SaleReturnData)grdViewSaleReturns.GetFocusedRow();
                if (_currentRecord != null)
                {
                    _currentRecord.IsDeleted = true;
                    bool _result = SaleReturnBL.Delete(_currentRecord);
                    if (_result)
                    {
                        GimjaHelper.ShowInformation("The record is removed successfully.");
                        PopulateSaleReturns();//refresh the return list display
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isAdding = isUpdating = false;
            EnableControls(true);
            PopulateSaleReturns();
        }

        private void grdViewSaleReturns_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateSaleReturnDisplay();
        }

        private void grdViewReturnDetails_GotFocus(object sender, EventArgs e)
        {
            if (isAdding || isUpdating)
            {
                var _refNo = Convert.ToString(lkeSalesRefNo.EditValue);
                if (string.IsNullOrEmpty(_refNo))
                {
                    lkeSalesRefNo.Focus();
                    GimjaHelper.ShowError("Sales reference number is required for a return.");
                    return;
                }

                var _returnDate = dtReturnDate.DateTime;
                if (_returnDate <= DateTime.MinValue)
                {
                    dtReturnDate.Focus();
                    GimjaHelper.ShowError("Return date is required.");
                    return;
                }
                var _user = Convert.ToString(lkeUsers.EditValue);
                if (string.IsNullOrEmpty(_user))
                {
                    lkeUsers.Focus();
                    GimjaHelper.ShowError("The user processing this return is required.");
                    return;
                }
                if (selectedSales == null)
                {
                    lkeSalesRefNo.Focus();
                    GimjaHelper.ShowError("The selected saes reference number is invalid.");
                    return;
                }
                else
                {
                    //the return date should be later than the sales date
                    if (selectedSales.SalesDate > _returnDate)
                    {
                        dtReturnDate.Focus();
                        GimjaHelper.ShowError("The return date should be later than the sales date.");
                        return;
                    }
                }
            }
        }

        private void grdViewReturnDetails_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData)
            {
                if (e.Column == grdColSerialNo)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
        }

        private void lkeSalesRefNo_EditValueChanged(object sender, EventArgs e)
        {
            var _refNo = Convert.ToString(lkeSalesRefNo.EditValue);
            if (_refNo != null && !string.IsNullOrEmpty(_refNo))
            {
                selectedSales = SalesBL.GetSales(_refNo);
                var _saleDetails = SalesBL.GetSaleDetails(_refNo);
                if (_saleDetails != null && _saleDetails.Count > 0)
                {
                    returnedItemsList = new List<ReturnItemData>();
                    _saleDetails.ForEach(d => returnedItemsList.Add(
                        new ReturnItemData()
                        {
                            ReturnedDetailsID = Guid.NewGuid(),
                            ItemID = d.ItemID,
                            CreatedBy = GimjaHelper.GetCurrentUserID(this), //"LogonUser",//TODO: ADD LOGON USER HERE
                            CreatedDate = DateTime.Now,
                            Quantity = (int)d.Quantity,
                            RefundedAmount = 0d
                        }));
                    //get the item list
                    var _itemList = _saleDetails.Select(d => d.ItemID).ToList();
                    //lkeRepositoryItemId.DataSource = _itemList;
                    cboRepositoryItemId.Items.AddRange(_itemList);
                    //set the list as data source
                    grdReturnDetails.DataSource = returnedItemsList;
                }
            }
        }

        private void frmSalesReturn_Load(object sender, EventArgs e)
        {
            PopulateSaleReturns();
            //populate the sales reference number
            var _sales = SalesBL.GetSalesToReturn();
            if (_sales.Count == 0)//there are no sales to retur
                btnAdd.Enabled = false;
            var _referenceNos = _sales.Select(s => s.ReferenceNo).ToList();
            lkeSalesRefNo.Properties.DataSource = _referenceNos;
            //populate the list of users
            var _users = UserBL.GetUsers();
            lkeUsers.Properties.DataSource = _users;
        }

        private void PopulateSaleReturns()
        {
            ClearControls();//first, clear anything displayed on the controls
            //get the return records
            var _returns = SaleReturnBL.GetSaleReturns();
            grdSaleReturns.DataSource = _returns;
            if (_returns.Count > 0)
            {//there are sale return records
                if (grdViewSaleReturns.FocusedRowHandle < 0 || grdViewSaleReturns.FocusedRowHandle >= _returns.Count)
                    grdViewSaleReturns.FocusedRowHandle = 0;
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }
    }
}