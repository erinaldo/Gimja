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
using DevExpress.XtraGrid.Menu;

namespace Gimja
{
    public partial class frmPurchaseOrder : DevExpress.XtraEditors.XtraForm
    {
        private PurchaseOrderData poData;
        private IList<PurchaseOrderDetailData> poDetails;
        private IList<PurchaseOrderData> poDataList;

        private bool isAdding;
        private bool isUpdating;

        public frmPurchaseOrder()
        {
            InitializeComponent();
            poDetails = new BindingList<PurchaseOrderDetailData>();
            poDataList = new List<PurchaseOrderData>();
            //enable/disable controls
            EnableControls(false);
            PopulatePurchaseOrders();
            PopulateSuppliers();
            PopulateItems();
            PopulateManufacturers();
            PopulateUsersList();
            //bind the purchase order details
            grdPurchaseOrderDetails.DataSource = poDetails;
        }

        private void frmPurchaseOrder_Load(object sender, EventArgs e)
        {
        }

        private void PopulateItems()
        {
            //get the items list
            ItemBL itemBL = new ItemBL();
            var _items = itemBL.GetData(true); //ItemBL.GetItems();
            lkeRepositoryItemId.DataSource = _items;
            if (_items.Count() == 0)
            {
                btnAdd.Enabled = false;
                btnAdd.ToolTip = "No items to place purchase order.";
            }
            else
            {
                btnAdd.Enabled = true;
                btnAdd.ToolTip = string.Empty;
            }
        }

        private void PopulateManufacturers()
        {
            //get the manufacturer list
            var _manufacturers = new ManufacturerBL().GetData();
            lkeRepositoryManufacturers.DataSource = _manufacturers;
        }

        private void PopulateSuppliers()
        {
            //get the supplier list
            var _suppliers = new SupplierBL().GetData();
            lkeSupplier.Properties.DataSource = _suppliers;
            lkeRepositorySupplier.DataSource = _suppliers;
        }

        private void PopulatePurchaseOrders()
        {
            poDataList = PurchaseOrderBL.GetPurchaseOrders();
            grdPurchaseOrderList.DataSource = poDataList;
            if (poDataList.Count > 0)
                grdViewPurchaseOrderList.FocusedRowHandle = 0;
            else
            {//there are no purchase order to edit and delete
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void PopulateUsersList()
        {
            UserBL _userBL = new UserBL();
            var _users = _userBL.GetData(true);
            lkeProcessedBy.Properties.DataSource = _users;
        }

        private void grdViewPurchaseOrderList_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {//it is viewing a record
                UpdatePurchaseOrderDisplay(e.FocusedRowHandle);
            }
        }

        private void UpdatePurchaseOrderDisplay()
        {
            var _poList = PurchaseOrderBL.GetPurchaseOrders();
            grdPurchaseOrderList.DataSource = _poList;
        }

        private void UpdatePurchaseOrderDisplay(int rowHandle)
        {
            if (rowHandle >= 0 && rowHandle < grdViewPurchaseOrderList.RowCount)
            {
                poData = (PurchaseOrderData)grdViewPurchaseOrderList.GetRow(rowHandle);
                if (poData != null)
                {//view the purchase order data
                    var _details = PurchaseOrderBL.GetPurchaseOrderDetails(poData.ID);
                    poDetails = new BindingList<PurchaseOrderDetailData>(_details);
                    //enable/disable the approve and report buttons
                    if (string.IsNullOrEmpty(poData.ApprovedBy))
                    {//it is not approved
                        btnApprove.Enabled = true;
                        btnPurchaseOrderReport.Enabled = false;//report is allowed for approved purchase orders
                    }
                    else
                    {
                        btnApprove.Enabled = false;//it is already approved
                        btnPurchaseOrderReport.Enabled = true;
                    }
                    ShowControlValues(poData, poDetails);
                    if (!string.IsNullOrEmpty(poData.ApprovedBy))
                    {//approved purchased orders can't be edited/deleted
                        btnEdit.Enabled = false;
                        btnDelete.Enabled = false;
                        btnApprove.Enabled = false;
                        lblPurchaseOrderInfo.Text = "This purchase order is approved and can't be edited.";
                    }
                    else
                    {
                        btnEdit.Enabled = true;
                        btnDelete.Enabled = true;
                        btnApprove.Enabled = true;
                        lblPurchaseOrderInfo.Text = string.Empty;
                    }
                }
                CalculateGrandTotal();
            }
            else
            {//the list is empty or not item is selected
                //disable buttons
                btnSave.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnCancel.Enabled = false;
                btnApprove.Enabled = false;
                btnPurchaseOrderReport.Enabled = false;
                //clear summary values
                lblTotalPriceText.Text = string.Empty;
                txtGrandTotalPrice.Text = string.Empty;
            }
        }

        private void CalculateGrandTotal()
        {
            double _grandTotalPrice = 0;
            for (int i = 0; i < grdViewPurchaseOrderDetails.RowCount; i++)
            {
                var _totalPriceVal = grdViewPurchaseOrderDetails.GetRowCellValue(i, grdColTotalPrice);
                double _totalPrice;
                if (_totalPriceVal != null && double.TryParse(_totalPriceVal.ToString(), out _totalPrice))
                {
                    _grandTotalPrice += _totalPrice;
                }
            }
            //set to the display
            txtGrandTotalPrice.Text = _grandTotalPrice.ToString("C2");
            if (_grandTotalPrice > 0)//show the price in text
                lblTotalPriceText.Text = NumberToText.Convert(Convert.ToDecimal(_grandTotalPrice));
            else
                lblTotalPriceText.Text = string.Empty;
        }

        private void grdViewPurchaseOrderList_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.RowHandle >= 0)
            {//it is viewing a record
                UpdatePurchaseOrderDisplay(e.RowHandle);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmSelectItems _itemSelector = new frmSelectItems();
            if (_itemSelector.AllPOItems.Count <= 0)
            {
                var _response = GimjaHelper.ShowQuestion("There are no items to create purchase orders for. Do you want to set individual item settings?");
                if (_response == System.Windows.Forms.DialogResult.Yes)
                {
                    PrepareForAdd();
                    poDetails = new BindingList<PurchaseOrderDetailData>();
                    grdPurchaseOrderDetails.DataSource = poDetails;
                }
            }
            else
            {
                _itemSelector.StartPosition = FormStartPosition.CenterParent;
                DialogResult _result = _itemSelector.ShowDialog(this);
                if (_result == System.Windows.Forms.DialogResult.OK)
                {
                    var purchaseOrderItems = _itemSelector.SelectedItems;
                    if (purchaseOrderItems.Count > 0)
                    {
                        PrepareForAdd();
                        poDetails = (from i in purchaseOrderItems
                                     //where i.Selected
                                     select new PurchaseOrderDetailData
                                     {
                                         PurchaseOrderID = Guid.NewGuid(),
                                         PurchaseOrderDetailID = Guid.NewGuid(),
                                         ItemID = i.ItemId,
                                         Origin = null,
                                         Quantity = (int)(i.OrderQuantity ?? 0),
                                         UnitPrice = i.UnitPrice ?? 0,
                                         ManufacturerID = null,
                                         Remark = null,
                                         CreatedBy = GimjaHelper.GetCurrentUserID(this), //"LogonUser",//TODO: ADD LOGON USER HERE
                                         CreatedDate = DateTime.Now,
                                         LastUpdatedBy = null,
                                         LastUpdatedDate = null,
                                         IsDeleted = false
                                     }).ToList();
                        grdPurchaseOrderDetails.DataSource = new List<PurchaseOrderDetailData>(poDetails);
                    }
                    else
                    {//no selected items to be included in the puchase order
                        var _confirm = GimjaHelper.ShowQuestion("There are no selected items to include in the purchase order. Do you want to select individual items and set their values?");
                        if (_confirm == System.Windows.Forms.DialogResult.Yes)
                        {
                            PrepareForAdd();
                            poDetails = new BindingList<PurchaseOrderDetailData>();
                            grdPurchaseOrderDetails.DataSource = poDetails;
                        }
                    }
                }
                else
                {//cancel is clicked
                    var _confirm = GimjaHelper.ShowQuestion("You have chosen cancel button. Do you want to select individual items and set their values?");
                    if (_confirm == System.Windows.Forms.DialogResult.Yes)
                    {
                        PrepareForAdd();
                        poDetails = new BindingList<PurchaseOrderDetailData>();
                        grdPurchaseOrderDetails.DataSource = poDetails;
                    }
                }
            }
        }

        private void PrepareForAdd()
        {
            isAdding = true;
            ClearControls();
            EnableControls(true);
        }

        private void ClearControls()
        {
            lkeProcessedBy.EditValue = null;
            dtPurchaseOrderDate.DateTime = DateTime.Now;
            lkeSupplier.EditValue = null;
            txtApprovedBy.Text = string.Empty;

            poDetails = new List<PurchaseOrderDetailData>();
            grdPurchaseOrderDetails.DataSource = poDetails;
        }

        private void ShowControlValues(PurchaseOrderData poData, IList<PurchaseOrderDetailData> poDetails)
        {
            dtPurchaseOrderDate.DateTime = poData.Date;
            lkeSupplier.EditValue = poData.SupplierID;
            lkeProcessedBy.EditValue = poData.ProcessedBy;
            txtApprovedBy.Text = poData.ApprovedBy;

            if (!string.IsNullOrEmpty(poData.ApprovedBy))
            {
                btnEdit.Enabled = false;//authorized sales record cannot be edited
                btnEdit.ToolTip = "The record is approved.";
            }
            else
            {
                btnEdit.ToolTip = string.Empty;
            }
            //display details
            grdPurchaseOrderDetails.DataSource = poDetails;
        }

        private void EnableControls(bool status)
        {
            lkeSupplier.Properties.ReadOnly = !status;
            lkeProcessedBy.Properties.ReadOnly = !status;
            dtPurchaseOrderDate.Properties.ReadOnly = !status;
            //txtApprovedBy.Properties.ReadOnly = !status;//authorization is done using different interface

            EnableDetailsControls(status);

            //disable row selection in the sales list
            //grdPurchaseOrderList.Enabled = status;
            //enable/disable buttons
            btnAdd.Enabled = !status; btnEdit.Enabled = !status;
            btnSave.Enabled = status; btnDelete.Enabled = !status;
            btnCancel.Enabled = status;
        }

        private void EnableDetailsControls(bool status)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in grdViewPurchaseOrderDetails.Columns)
            {
                if (col == grdColItem || col == grdColManufacturerId || col == grdColQuantity ||
                    col == grdColUnitPrice || col == grdColOrigin)
                {
                    col.OptionsColumn.AllowEdit = status;
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (poData == null)
            {
                GimjaHelper.ShowError("Unable to get the purchase order to edit.");
                return;
            }
            else if (!string.IsNullOrEmpty(poData.ApprovedBy))
            {
                GimjaHelper.ShowError("An approved purchase order can't be edited.");
                return;
            }
            if (poDetails == null)
                poDetails = PurchaseOrderBL.GetPurchaseOrderDetails(poData.ID);

            isUpdating = true;
            isAdding = false;//if any

            EnableControls(true);
        }

        private void grdViewPurchaseOrderDetails_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData)
            {
                if (e.Column == grdColSN)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isAdding = false;
            isUpdating = false;

            ClearControls();
            EnableControls(false);
            poDetails = new List<PurchaseOrderDetailData>();
            UpdatePurchaseOrderDisplay(grdViewPurchaseOrderList.FocusedRowHandle);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string _currentUserId = GimjaHelper.GetCurrentUserID(this);
            if (string.IsNullOrEmpty(_currentUserId))
            {
                GimjaHelper.ShowError("The current user processing the order can not be found!");
                return;
            }
            DateTime _currentDateTime = DateTime.Now;
            if (isAdding)
            {//saving new purchase order data
                bool isPOValid = ValidPurchaseOrder();
                if (isPOValid)
                {
                    poData = new PurchaseOrderData()
                    {
                        ID = Guid.NewGuid(),
                        SupplierID = Convert.ToString(lkeSupplier.EditValue),
                        ProcessedBy = Convert.ToString(lkeProcessedBy.EditValue),
                        Date = dtPurchaseOrderDate.DateTime,
                        CreatedBy = _currentUserId, //"LogonUser", //TODO: ADD THE LOGON USER HERE
                        CreatedDate = _currentDateTime
                    };

                    if (poDetails == null || poDetails.Count == 0)
                    {
                        throw new InvalidOperationException("A purchase order cannot be saved without an item to be ordered.");
                    }
                    //set the user and date for each purchase order detail
                    foreach (var _d in poDetails)
                    {
                        _d.CreatedBy = _currentUserId;
                        _d.CreatedDate = _currentDateTime;
                    }
                    var _result = PurchaseOrderBL.Insert(poData, poDetails);
                    if (_result)
                    {
                        GimjaHelper.ShowInformation("Inserted successfully.");
                        //end the insertion
                        EnableControls(false);
                        UpdatePurchaseOrderDisplay();
                    }
                    else
                        GimjaHelper.ShowError("Error in saving the purchase order information.");
                }
            }
            else if (isUpdating)
            {//updating an existing purchase order data
                //get the currently edited purchase order
                int _currentRowIndex = grdViewPurchaseOrderList.FocusedRowHandle;
                if (_currentRowIndex >= 0)
                {
                    var _po = (PurchaseOrderData)grdViewPurchaseOrderList.GetRow(_currentRowIndex);
                    if (_po != null)
                    {
                        if (poDetails == null || poDetails.Count == 0)
                        {
                            throw new InvalidOperationException("A purchase order cannot be saved without an item to be ordered.");
                        }
                        _po.SupplierID = lkeSupplier.EditValue.ToString();
                        _po.ProcessedBy = Convert.ToString(lkeProcessedBy.EditValue);
                        _po.Date = dtPurchaseOrderDate.DateTime;
                        _po.LastUpdatedBy = _currentUserId; // "LogonUser";//TODO: ADD THE LOGON USER HERE
                        _po.LastUpdatedDate = _currentDateTime;
                        foreach (var _detail in poDetails)
                        {
                            if (_detail.PurchaseOrderID == Guid.Empty)
                            {//new purchase order detail
                                _detail.PurchaseOrderID = _po.ID;
                            }
                            if (!string.IsNullOrEmpty(_detail.CreatedBy))
                            {
                                _detail.LastUpdatedBy = _currentUserId; //"LogonUser";
                                _detail.LastUpdatedDate = _currentDateTime;
                            }
                            else
                            {
                                _detail.CreatedBy = _currentUserId; //"LogonUser";
                                _detail.CreatedDate = _currentDateTime;
                            }
                        }
                        bool _result = PurchaseOrderBL.Update(_po, poDetails);
                        if (_result)
                        {
                            GimjaHelper.ShowInformation("Updated successfully.");
                            EnableControls(false);
                            UpdatePurchaseOrderDisplay();
                        }
                        else
                            GimjaHelper.ShowError("An error when updateing the purchase order.");
                    }
                }
                else
                {
                    GimjaHelper.ShowError("Invalid purchase order to edit.");
                }
            }
            else
            {//not known, may be due to error
            }

        }

        private bool ValidPurchaseOrder()
        {
            if (string.IsNullOrEmpty(Convert.ToString(lkeSupplier.EditValue)))
            {
                GimjaHelper.ShowError("Supplier is required.");
                return false;
            }
            else if (string.IsNullOrEmpty(Convert.ToString(lkeProcessedBy.EditValue)))
            {
                GimjaHelper.ShowError("Who is processing this transaction? It is required.");
                return false;
            }
            else if (dtPurchaseOrderDate.DateTime > DateTime.Now || dtPurchaseOrderDate.DateTime.AddDays(-1) < DateTime.Today.AddDays(-1))
            {
                GimjaHelper.ShowError("Invalid purchase order date.");
                return false;
            }
            return true;
        }

        private void grdViewPurchaseOrderDetails_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            GridView view = (GridView)sender;

            var _item = Convert.ToString(view.GetRowCellValue(e.RowHandle, grdColItem));
            var _detail = (PurchaseOrderDetailData)e.Row;
            if (string.IsNullOrEmpty(_item))
            {
                view.SetColumnError(grdColItem, "Item is required.");
                e.Valid = false;
            }
            else
            {
                var _groupCounts = from sd in poDetails
                                   group sd by sd.ItemID into Items
                                   select Items.Count();
                var _multipleItems = _groupCounts.Any(g => g > 1);
                if (_multipleItems)
                {
                    e.Valid = false;
                    view.SetColumnError(grdColItem, "Duplicate items found.");
                }
            }
            var _qty = view.GetRowCellValue(e.RowHandle, grdColQuantity);
            int _qtyAmt = Convert.ToInt32(_qty);
            if (_qtyAmt <= 0)
            {
                view.SetColumnError(grdColQuantity, "Invalid order quantity.");
                e.Valid = false;
            }
            var _originVal = view.GetRowCellValue(e.RowHandle, grdColOrigin);
            if (string.IsNullOrEmpty(_originVal.ToString()))
            {
                view.SetColumnError(grdColOrigin, "Origin is required.");
                e.Valid = false;
            }
            var _manufacturerId = view.GetRowCellValue(e.RowHandle, grdColManufacturerId);
            if (string.IsNullOrEmpty(Convert.ToString(_manufacturerId)))
            {
                view.SetColumnError(grdColManufacturerId, "Manufacturer is required.");
                e.Valid = false;
            }
        }

        private void grdViewPurchaseOrderDetails_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        }

        private void grdViewPurchaseOrderDetails_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var _newDetail = (PurchaseOrderDetailData)grdViewPurchaseOrderDetails.GetRow(e.RowHandle);
            if (_newDetail != null)
            {
                _newDetail.PurchaseOrderDetailID = Guid.NewGuid();
                _newDetail.CreatedDate = DateTime.Now;
            }
        }

        private void grdViewPurchaseOrderDetails_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            CalculateGrandTotal();
        }

        private void grdViewPurchaseOrderList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo _hi = e.HitInfo;
            if (_hi.InRow)
            {
                GridViewMenu _menu = e.Menu as GridViewMenu;
                if (e.HitInfo.InRow)
                {
                    var _po = (PurchaseOrderData)grdViewPurchaseOrderList.GetRow(e.HitInfo.RowHandle);
                    if (_po != null && string.IsNullOrEmpty(_po.ApprovedBy))
                        _menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Approve", OnPurchaseOrderContextMenu));
                }
                _menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Approve Multiple", OnPurchaseOrderContextMenu, imgListMenuIcons.Images[1]));
                _menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Refresh", OnPurchaseOrderContextMenu, imgListMenuIcons.Images[0]));
            }
        }

        private void OnPurchaseOrderContextMenu(object sender, EventArgs e)
        {
            DevExpress.Utils.Menu.DXMenuItem _currentMenu = sender as DevExpress.Utils.Menu.DXMenuItem;
            if (_currentMenu != null)
            {
                GridView _view = grdViewPurchaseOrderList as GridView;
                switch (_currentMenu.Caption)
                {
                    case "Approve":
                        var _currentRow = _view.GetFocusedRow();
                        if (_currentRow != null)
                        {
                            PurchaseOrderData _data = (PurchaseOrderData)_currentRow;
                            if (_data != null && !string.IsNullOrEmpty(_data.ApprovedBy))
                            {
                                GimjaHelper.ShowInformation("The purchase order is already approved.");
                            }
                            else
                            {
                                var _response = GimjaHelper.ShowQuestion("Are you sure to approve the currently selected purchase order?");
                                if (_response == System.Windows.Forms.DialogResult.Yes)
                                {
                                    string _userID = GimjaHelper.GetCurrentUserID(this);
                                    _data.ApprovedBy = _userID; //"LogonUser";//TODO: ADD THE LOGON USER HERE
                                    _data.LastUpdatedBy = _userID; //"LogonUser";
                                    _data.LastUpdatedDate = DateTime.Now;
                                    bool _result = PurchaseOrderBL.ApproveAll(new Guid[] { _data.ID }, "LogonUser");
                                    if (_result)
                                    {
                                        GimjaHelper.ShowInformation("The purchase order is approved successfully.");
                                        UpdatePurchaseOrderDisplay();
                                    }
                                    else
                                        GimjaHelper.ShowError("Approval failed with error.");
                                }
                            }
                        }
                        else
                            GimjaHelper.ShowError("Approval is allowed on a valid purchase order row.");
                        break;
                    case "Approve Multiple":
                        frmApprovePurchaseOrders _approvePO = new frmApprovePurchaseOrders();
                        _approvePO.StartPosition = FormStartPosition.CenterParent;
                        _approvePO.ShowDialog(this);
                        UpdatePurchaseOrderDisplay();
                        break;
                    case "Refresh":
                        UpdatePurchaseOrderDisplay();
                        break;
                    default:
                        break;
                }
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (grdViewPurchaseOrderList.SelectedRowsCount > 1)
            {
                GimjaHelper.ShowInformation("Multiple row selection is not allowed for approval using this method. Instead please right click and choose Approve All.");
            }
            else if (grdViewPurchaseOrderList.SelectedRowsCount == 1)
            {//there are selected rows
                int _selectedIndex = grdViewPurchaseOrderList.GetSelectedRows()[0];
                var _selectedPurchaseOrder = (PurchaseOrderData)grdViewPurchaseOrderList.GetRow(_selectedIndex);
                if (_selectedPurchaseOrder != null)
                {
                    if (string.IsNullOrEmpty(_selectedPurchaseOrder.ApprovedBy))
                    {
                        var _response = GimjaHelper.ShowQuestion("Are you sure to approve the currently selected purchase order?");
                        if (_response == System.Windows.Forms.DialogResult.Yes)
                        {
                            bool _result = PurchaseOrderBL.ApproveAll(new Guid[] { _selectedPurchaseOrder.ID }, "LogonUser");
                            if (_result)
                            {
                                GimjaHelper.ShowInformation("The selected purchase order is approved successfully.");
                                PopulatePurchaseOrders();
                            }
                        }
                    }
                }
            }
            else
            {
                GimjaHelper.ShowError("There are no selected purchase orders to approve.");
            }
        }

        private void btnPurchaseOrderReport_Click(object sender, EventArgs e)
        {
            if (isAdding || isUpdating)
            {//the sales record is not saved
                GimjaHelper.ShowError("The current purchase order record is not saved. Please save and retry.");
                return;
            }
            else
            {
                var _currentRecord = (PurchaseOrderData)grdViewPurchaseOrderList.GetFocusedRow();
                if (_currentRecord != null)
                {
                    if (string.IsNullOrEmpty(_currentRecord.ApprovedBy))
                    {//the record is not approved
                        GimjaHelper.ShowError("The purchase order record is not approved. The purchase order need to be approved to show the report.");
                        return;
                    }
                    else
                    {
                        frmPurchaseOrderReport _report = new frmPurchaseOrderReport(_currentRecord);
                        _report.ShowDialog(this);
                    }
                }
                else
                {
                    GimjaHelper.ShowError("No selected purchase order record to show!");
                }
            }
        }

        private void grdViewPurchaseOrderDetails_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo _hi = e.HitInfo;
            if (_hi.InColumn)
            {
                e.Allow = false;
            }
            if ((_hi.InRow || _hi.InRowCell) && _hi.Column == grdColOrigin && !grdViewPurchaseOrderDetails.IsNewItemRow(_hi.RowHandle))
            {
                GridViewMenu _menu = e.Menu;
                _menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Origin...", OnPurchaseOrderDetailContextMenu));
            }
        }

        private void OnPurchaseOrderDetailContextMenu(object sender, EventArgs e)
        {
            DevExpress.Utils.Menu.DXMenuItem _currentMenu = sender as DevExpress.Utils.Menu.DXMenuItem;
            if (_currentMenu != null)
            {
                GridView _view = grdViewPurchaseOrderDetails as GridView;
                switch (_currentMenu.Caption)
                {
                    case "Origin...":
                        //get selected purchase order details
                        var _selectedDetailRowIndices = _view.GetSelectedRows();
                        if (_selectedDetailRowIndices.Length == 0)
                        {//there are no selected rows
                            var _response = GimjaHelper.ShowQuestion("There are no selected purchase order detail rows. Do you want to set origin for all rows?");
                            if (_response == System.Windows.Forms.DialogResult.Yes)
                            {
                                _view.SelectAll();
                                _selectedDetailRowIndices = _view.GetSelectedRows();
                            }
                            else
                            {
                                GimjaHelper.ShowError("Selected purchase order details are required to set an origin country.");
                                return;
                            }
                        }
                        //get the origin value
                        frmPurchaseOrderOrigin _origin = new frmPurchaseOrderOrigin();
                        _origin.StartPosition = FormStartPosition.CenterParent;
                        var _dlgResult = _origin.ShowDialog(this);
                        if (_dlgResult == System.Windows.Forms.DialogResult.OK)
                        {
                            var _originText = _origin.Origin;
                            if (string.IsNullOrEmpty(_originText))
                            {
                                GimjaHelper.ShowError("Origin is required!");
                                return;
                            }
                            else
                            {
                                foreach (int i in _selectedDetailRowIndices)
                                {
                                    if (_view.IsValidRowHandle(i))
                                    {
                                        _view.SetRowCellValue(i, "Origin", _originText);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}