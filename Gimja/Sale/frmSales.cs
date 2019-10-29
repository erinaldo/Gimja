using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using GimjaBL;
using DevExpress.XtraGrid.Views.Grid;

namespace Gimja
{
    public partial class frmSales : DevExpress.XtraEditors.XtraForm
    {
        private bool isNewRecord;
        private bool isUpdating;

        private IList<SalesDetailData> salesDetails;
        private SalesData salesData;
        private IList<ItemRequestData> itemRequestList;

        private int selectedRowIndex = -1;
        public frmSales()
        {
            InitializeComponent();
            //salesDetails = new BindingList<SalesDetailData>();

            PopulateBranches();
            PopulateSaleLocations();
            PopulateReceipts();
            PopulateCustomers();
            PopulateRepositoryItems();
            //populate existing sales records
            PopulateSalesList();
        }

        private void PopulateRepositoryItems()
        {
            ItemBL itemBL = new ItemBL();
            var _itemList = itemBL.GetData(true); //ItemBL.GetItems();
            lkeRepositoryItemsList.DataSource = _itemList;
        }

        private void PopulateReceipts()
        {
            var _receipts = ReceiptBL.GetActiveReceipts();
            lkeReceipts.Properties.DataSource = _receipts;
        }

        private void PopulateCustomers()
        {
            var _customers = CustomerBL.GetActiveCustomers();
            lkeCustomers.Properties.DataSource = _customers;
        }

        private void frmSales_Load(object sender, EventArgs e)
        {
            LoadCashierList();
            EnableControls(false);
        }

        private void LoadCashierList()
        {
            lkeCashiers.Properties.DataSource = UserBL.GetUsers();
        }

        private void PopulateSalesList()
        {
            ClearControls();
            salesDetails = new List<SalesDetailData>();
            List<SalesData> _salesData = SalesBL.GetSales();
            int _currentIndex = grdViewSales.FocusedRowHandle;
            grdSales.DataSource = _salesData;
            //var _details=SalesBL.GetSalesDetail(
            grdSaleDetails.DataSource = salesDetails;

            if (_salesData.Count > 0)
            {
                if (_currentIndex >= 0 && _currentIndex < grdViewSales.RowCount)
                    grdViewSales.FocusedRowHandle = _currentIndex;
                else if (_currentIndex >= grdViewSales.RowCount)
                    grdViewSales.FocusedRowHandle = grdViewSales.RowCount - 1;
                else if (grdViewSales.RowCount > 0)
                    grdViewSales.FocusedRowHandle = 0;
                else
                    grdViewSales.FocusedRowHandle = -1;
                ////populate the reference number list
                //cboRefNo.Properties.Items.Clear();
                //List<string> _refNoList = (from s in _salesData select s.ReferenceNo).ToList();
                //foreach (var _refNo in _refNoList)
                //{
                //    cboRefNo.Properties.Items.Add(_refNo);
                //}
            }
            else
            {//there are no sale records
                btnEdit.Enabled = false;
                btnVoid.Enabled = false;
            }
        }

        private void PopulateBranches()
        {
            BranchBL _branchBL = new BranchBL();
            var _branches = _branchBL.GetData(true);
            lkeBranches.Properties.DataSource = _branches;
        }

        private void PopulateSaleLocations()
        {
            var _locations = SaleLocationBL.GetActiveSaleLocations();
            lkeRepositoryPickFrom.DataSource = _locations;
        }

        private void grdViewSaleDetails_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //GimjaHelper.ShowInformation("grdViewSaleDetails_CellValueChanging");

        }

        private void grdViewSaleDetails_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            grdViewSaleDetails.ClearColumnErrors();
            if (e.Column == grdColDiscount || e.Column == grdColQty)
            {
                CalculateSalesTotal();
            }
            else if (e.Column == grdColPickFrom)
            {
                var _pickFrom = Convert.ToInt32(e.Value);
                var _item = grdViewSaleDetails.GetRowCellValue(e.RowHandle, grdColItem);
                if (_item == null)
                {
                    GimjaHelper.ShowError("You need to select the item before the location to pick from.");
                    return;
                }
                if (_pickFrom == 3)//TODO: ENSURE WAREHOUSE LOCATION HAS ID 3
                {//warehouse location is selected
                    frmWarehouseList w = new frmWarehouseList();
                    string _selectedWarehouse = w.SelectWarehouse(this);
                    if (_selectedWarehouse == null)
                    {
                        GimjaHelper.ShowError("Invalid warehouse selected.");
                        grdViewSaleDetails.SetColumnError(e.Column, "Invalid warehouse!");
                        return;
                    }
                    else
                    {
                        var _request = new ItemRequestData()
                        {
                            warehouseId = _selectedWarehouse,
                            itemID = _item.ToString()
                        };
                        var _exists = itemRequestList.Any(r => r.itemID.Equals(_item.ToString()));
                        if (!_exists)
                            itemRequestList.Add(_request);
                    }
                }
                else
                {//other sale locations are chosen
                    if (itemRequestList != null)
                    {
                        var _existingRequest = itemRequestList.Where(r => r.itemID == _item.ToString()).FirstOrDefault();

                        itemRequestList.Remove(_existingRequest);
                    }
                }
            }
            else if (e.Column == grdColItem)
            {
                SalesDetailData _data = (SalesDetailData)grdViewSaleDetails.GetRow(e.RowHandle);
                if (_data != null)
                {
                    var _item = ItemBL.GetItem(_data.ItemID);
                    if (_item != null)
                    {
                        _data.UnitPrice = _item.UnitPrice;
                    }
                    else
                        _data.UnitPrice = 0d;
                }
            }
        }

        private void CalculateSalesTotal()
        {
            double _subTotal = 0d, _taxable = 0d;
            for (int i = 0; i < grdViewSaleDetails.RowCount; i++)
            {
                object _totalPriceVal = grdViewSaleDetails.GetRowCellValue(i, grdColTotalPrice);
                var _unitPriceVal = grdViewSaleDetails.GetRowCellValue(i, grdColUnitPrice);
                var _qty = grdViewSaleDetails.GetRowCellValue(i, grdColQty);
                double _totalPrice = 0d;
                bool _validTotalPrice = false;
                if (_totalPriceVal != null)
                {
                    _validTotalPrice = double.TryParse(_totalPriceVal.ToString(), out _totalPrice);
                    if (_validTotalPrice)
                        _subTotal += _totalPrice;
                }
                object _itemVal = grdViewSaleDetails.GetRowCellValue(i, grdColItem);
                if (_itemVal != null)
                {
                    var _item = ItemBL.GetItem(_itemVal.ToString());
                    if (_item != null)
                    {
                        if (!(_item.IsTaxExempted) ?? false && _validTotalPrice)
                        {
                            _taxable += _totalPrice;
                        }
                    }
                }
            }
            //get the vat rate
            double _tax = 0d;
            var _taxType = TaxTypeBL.GetTaxType("VAT");
            if (_taxType != null)
                _tax = _taxable * _taxType.Rate;
            //get the total payment
            var _total = _subTotal + _tax;
            //show the values in textbox
            txtSalesSubtotal.Text = _subTotal.ToString("f2");
            txtTaxable.Text = _taxable.ToString("f2");
            txtSalesVATTax.Text = _tax.ToString("f2");
            txtSalesTotal.Text = _total.ToString("f2");
            //show the total in words
            if (_total == 0)
                lblTotalWords.Text = string.Empty;
            else
                lblTotalWords.Text = NumberToText.Convert(Convert.ToDecimal(_total));
        }

        private void grdViewSaleDetails_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            //GimjaHelper.ShowInformation("grdViewSaleDetails_SelectionChanged");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            EnableControls(true);
            isNewRecord = true;
            //clear controls
            ClearControls();
            string _refNo = SalesBL.GetReferenceNumber();
            txtReferenceNo.Text = _refNo;
            btnAdd.Enabled = false; btnEdit.Enabled = false; btnSave.Enabled = true; btnVoid.Enabled = false; btnCancel.Enabled = true;
            btnFind.Enabled = false;

            itemRequestList = new List<ItemRequestData>();

            lkeBranches.Focus();
        }

        private void ClearControls()
        {
            txtReferenceNo.Text = string.Empty; dtSalesDate.DateTime = DateTime.Now;
            lkeBranches.EditValue = null;
            lkeCustomers.EditValue = null;
            lkeCashiers.EditValue = null;
            lkeReceipts.EditValue = null;
            txtAuthorizedBy.Text = string.Empty;

            salesDetails = new BindingList<SalesDetailData>();
            grdSaleDetails.DataSource = salesDetails;
            //update the sale location list
            PopulateSaleLocations();
            //update the items list
            PopulateRepositoryItems();
        }

        private void ShowControlValues(SalesData sales, IList<SalesDetailData> saleDetails)
        {
            //display sale values
            lkeBranches.EditValue = sales.BranchID; lkeCustomers.EditValue = sales.CustomerID;
            lkeCashiers.EditValue = sales.ProcessedBy; txtReferenceNo.Text = sales.ReferenceNo;
            lkeReceipts.EditValue = sales.ReceiptID; dtSalesDate.DateTime = sales.SalesDate;
            chkIsSalesCredit.Checked = sales.IsSalesCredit;
            txtFSNo.Text = sales.FSNo; txtRefNo.Text = sales.RefNo;
            txtReference.Text = sales.Reference; txtRefNote.Text = sales.RefNote;
            txtAuthorizedBy.Text = sales.AuthorizedBy;
            if (!string.IsNullOrEmpty(sales.AuthorizedBy))
            {
                btnEdit.Enabled = false;//authorized sales record cannot be edited
                btnEdit.ToolTip = "The record is authorized.";
            }
            else
            {
                btnEdit.ToolTip = string.Empty;
            }
            //display details
            this.grdSaleDetails.DataSource = saleDetails;
        }

        private void EnableControls(bool status)
        {
            lkeBranches.Properties.ReadOnly = !status;
            lkeCustomers.Properties.ReadOnly = !status;
            lkeCashiers.Properties.ReadOnly = !status; //txtRefNo.Properties.ReadOnly = !p;
            lkeReceipts.Properties.ReadOnly = !status;
            dtSalesDate.Properties.ReadOnly = !status;
            chkIsSalesCredit.Properties.ReadOnly = !status;
            txtFSNo.Properties.ReadOnly = !status;
            txtRefNo.Properties.ReadOnly = !status;
            txtReference.Properties.ReadOnly = !status;
            txtRefNote.Properties.ReadOnly = !status;
            //txtAuthorizedBy.Properties.ReadOnly = !p;//authorization is done using different interface
            EnableSalesDetailsControls(status);
            //grdSaleDetails.Enabled = p;
            //disable row selection in the sales list
            grdSales.Enabled = !status;
        }

        private void EnableSalesDetailsControls(bool status)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in grdViewSaleDetails.Columns)
            {
                if (col == grdColItem || col == grdColPickFrom || col == grdColQty ||
                    col == grdColUnitPrice || col == grdColItemUnit || col == grdColDiscount)
                {
                    col.OptionsColumn.AllowEdit = status;
                }
                //if (isUpdating && col == grdColItem)
                //{//can't update the item column
                //    col.OptionsColumn.AllowEdit = false;
                //}
            }
        }

        private void grdViewSales_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            //GimjaHelper.ShowInformation("grdViewSales_RowCellClick");
            selectedRowIndex = e.RowHandle;
            if (selectedRowIndex >= 0)
            {
                var _selRefNo = grdViewSales.GetRowCellValue(selectedRowIndex, grdColSaleRefNo);//the ref no value
                //var _selSales = SalesBL.GetSales(_selRefNo);
                UpdateSalesDisplay(selectedRowIndex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (isNewRecord)
            {//add button was clicked
                EnableControls(false);
                ClearControls();
                btnFind.Enabled = true; btnAdd.Enabled = true; btnSave.Enabled = false; btnVoid.Enabled = false;
                btnCancel.Enabled = false; btnEdit.Enabled = false;

                PopulateSalesList();

                isNewRecord = false;
            }
            else if (isUpdating)
            {//the edit button was clicked
                EnableControls(false);
                ClearControls();
                btnFind.Enabled = true; btnAdd.Enabled = true; btnSave.Enabled = false; btnVoid.Enabled = false;
                btnCancel.Enabled = false; btnEdit.Enabled = false;

                PopulateSalesList();

                isUpdating = false;
            }
            //empty the sales detail list
            //salesDetails = null;
            UpdateSalesDisplay(grdViewSales.FocusedRowHandle);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region Validating Sales Data
            var _referenceNo = txtReferenceNo.Text;
            var _saleDate = dtSalesDate.DateTime;
            var _branch = lkeBranches.EditValue;
            var _customer = lkeCustomers.EditValue;
            var _cashier = lkeCashiers.EditValue;
            var _receipt = Convert.ToString(lkeReceipts.EditValue);
            var _authorizedBy = txtAuthorizedBy.Text;
            bool _isValid = true;
            if (string.IsNullOrWhiteSpace(_referenceNo))
            {
                txtReferenceNo.Focus();
                GimjaHelper.ShowError("Reference number is required.");
                _isValid = false;
            }
            else if (string.IsNullOrWhiteSpace(_branch.ToString()))
            {
                lkeBranches.Focus();
                GimjaHelper.ShowError("Shop branch is required.");
                _isValid = false;
            }
            else if (string.IsNullOrWhiteSpace(_customer.ToString()))
            {
                lkeCustomers.Focus();
                GimjaHelper.ShowError("Choose Customer!");
            }
            else if (string.IsNullOrWhiteSpace(_cashier.ToString()))
            {
                lkeCashiers.Focus();
                GimjaHelper.ShowError("Please choose the cashier processing the sale.");
                _isValid = false;
            }
            //else if (string.IsNullOrWhiteSpace(_receipt))
            //{
            //    lkeReceipts.Focus();
            //    GimjaHelper.ShowError("Choose receipt!");
            //}
            #endregion

            #region New Sales Record to Save
            if (isNewRecord)
            {//save new sales record
                if (_isValid)
                {
                    string _receiptId = _receipt;
                    DateTime currentDateTime = DateTime.Now;
                    string _currentUserId = Singleton.Instance.UserID;
                    try
                    {
                        salesData = new SalesData()
                        {
                            ID = Guid.NewGuid(),
                            BranchID = _branch.ToString(),
                            CustomerID = _customer == null ? string.Empty : _customer.ToString(),
                            ProcessedBy = _cashier == null ? string.Empty : _cashier.ToString(),
                            ReferenceNo = _referenceNo,
                            ReceiptID = _receiptId,
                            SalesDate = _saleDate,
                            IsSalesCredit = chkIsSalesCredit.Checked,
                            AuthorizedBy = string.IsNullOrEmpty(_authorizedBy) ? null : _authorizedBy,
                            CreatedBy = _currentUserId, //"LogonUser",//TODO: SET THE LOGON USER HERE
                            CreatedDate = currentDateTime,
                            RefNo = txtRefNo.Text.Trim(),
                            Reference = txtReference.Text.Trim(),
                            FSNo = txtFSNo.Text.Trim(),
                            RefNote = txtRefNote.Text.Trim()
                        };
                        //bool _retValue = SalesBL.Insert(salesData);
                        foreach (var _detail in salesDetails)
                        {
                            _detail.SalesID = salesData.ID;
                            _detail.SalesDetailID = Guid.NewGuid();
                            _detail.CreatedBy = _currentUserId; //"LogonUser";//TODO: ADD THE LOGON USER HERE
                            _detail.CreatedDate = currentDateTime;
                        }

                        //insert the sales details
                        var _result = SalesBL.Insert(salesData, salesDetails, itemRequestList);
                        if (_result)
                        {
                            GimjaHelper.ShowInformation("Inserted successfully.");
                            //show the attachment dialog
                            frmSalesAttachment _attachment = new frmSalesAttachment(salesData);
                            _attachment.ShowDialog(this);
                        }
                        else
                            GimjaHelper.ShowError("Unable to save the sales data.");

                    }
                    catch (Exception ex)
                    {
                        GimjaHelper.ShowError("Unable to save the sales detail list.");
                    }
                    //finally
                    {
                        //update the sales list
                        PopulateSalesList();
                        EnableControls(false);
                        btnAdd.Enabled = true; btnSave.Enabled = false; btnFind.Enabled = true;
                        btnCancel.Enabled = false; btnEdit.Enabled = true; btnVoid.Enabled = true;
                        isNewRecord = false;
                    }
                }
            }
            #endregion

            #region Edited Record to Save
            else if (isUpdating)
            {//the edit button was clicked
                if (salesData != null && salesDetails != null)
                {
                    salesData.BranchID = lkeBranches.EditValue.ToString();
                    salesData.CustomerID = lkeCustomers.EditValue.ToString();
                    salesData.ProcessedBy = lkeCashiers.EditValue.ToString();
                    salesData.ReferenceNo = txtReferenceNo.Text;
                    salesData.ReceiptID = Convert.ToString(lkeReceipts.EditValue);
                    salesData.LastUpdatedBy = GimjaHelper.GetCurrentUserID(this); // "LogonUser"; //TODO: SET THE LOGON USER HERE
                    salesData.LastUpdatedDate = DateTime.Now;
                    salesData.IsSalesCredit = chkIsSalesCredit.Checked;
                    salesData.FSNo = txtFSNo.Text.Trim();
                    salesData.RefNo = txtRefNo.Text.Trim();
                    salesData.Reference = txtReference.Text.Trim();
                    salesData.RefNote = txtRefNote.Text.Trim();

                    bool _retValue = SalesBL.Update(salesData, salesDetails, itemRequestList);
                    if (_retValue)
                        GimjaHelper.ShowInformation("Updated successfully.");
                    //update the interface
                    PopulateSalesList();
                    EnableControls(false);
                    btnAdd.Enabled = true; btnSave.Enabled = false; btnCancel.Enabled = false;
                    btnEdit.Enabled = false; btnVoid.Enabled = true;
                    btnFind.Enabled = true;

                    isUpdating = false;
                }
            }
            #endregion
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (grdViewSales.FocusedRowHandle < 0)
                throw new InvalidOperationException("No valid sales record selected to edit.");

            UpdateSalesDisplay(grdViewSales.FocusedRowHandle);

            isUpdating = true;
            EnableControls(true);
            btnAdd.Enabled = false; btnSave.Enabled = true; btnEdit.Enabled = false;
            btnVoid.Enabled = false; btnCancel.Enabled = true;
            btnFind.Enabled = false;

            itemRequestList = new List<ItemRequestData>();
            lkeBranches.Focus();
        }

        private void btnVoid_Click(object sender, EventArgs e)
        {
            var _response = GimjaHelper.ShowQuestion("Are you sure to make the selected sales record 'void'?");
            if (_response == DialogResult.Yes)
            {
                if (salesData != null)
                {
                    bool _retValue = SalesBL.Void(salesData);
                }
                else
                {
                    int _selectedRowIndex = grdViewSaleDetails.FocusedRowHandle;
                    if (_selectedRowIndex >= 0)
                    {
                        var _salesId = grdViewSales.GetRowCellValue(_selectedRowIndex, grdColSalesId);
                        if (!string.IsNullOrWhiteSpace(_salesId.ToString()))
                            SalesBL.Void(new Guid(_salesId.ToString()));
                        else
                            GimjaHelper.ShowError("Unable to get the selected sales data ID.");
                    }
                    else
                    {
                        grdViewSales.Focus();
                        GimjaHelper.ShowError("No selected sales record to void");
                    }
                }
            }
            PopulateSalesList();
        }

        private void grdViewSaleDetails_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            var view = sender as GridView;
            //initializing the discount column to zero
            view.SetRowCellValue(e.RowHandle, grdColDiscount, 0);
            view.SetRowCellValue(e.RowHandle, grdColPickFrom, 1);//TODO: ENSURE THE PICK FACE HAS VALUE OF 1
            //reset the tax and grand total boxes
            txtSalesSubtotal.Text = string.Empty;
            txtTaxable.Text = string.Empty;
            txtSalesTotal.Text = string.Empty;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            DateTime? _dtFrom = dtFromDate.DateTime;
            DateTime _dtTo = dtToDate.DateTime;
            //var _refNo = cboRefNo.EditValue.ToString();
            if (_dtFrom == null || _dtFrom <= DateTime.MinValue)
                _dtFrom = null;
            else
                _dtFrom = _dtFrom.Value.Date;
            if (_dtTo == null || _dtTo <= DateTime.MinValue)
                _dtTo = DateTime.Now;
            else
                _dtTo = _dtTo.Date;
            //if (!string.IsNullOrWhiteSpace(_refNo))
            //    _refNo = _refNo.Trim();

            List<SalesData> _result = SalesBL.GetSales(_dtFrom, _dtTo);//, _refNo);

            grdSales.DataSource = _result;
            if (_result.Count > 0)
                grdViewSales.FocusedRowHandle = 0;
            ClearControls();
        }

        private void grdViewSaleDetails_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            grdViewSaleDetails.ClearColumnErrors();
            var view = sender as GridView;
            if (view.IsNewItemRow(e.RowHandle))
            {
                SalesDetailData _saleDetails = (SalesDetailData)view.GetRow(e.RowHandle);
                if (_saleDetails.SalesFrom == 3)
                {//warehosue is selected
                    var _request = itemRequestList.Where(r => r.itemID == _saleDetails.ItemID).SingleOrDefault();
                    if (_request != null)
                    {
                        var _wId = _request.warehouseId;
                        if (string.IsNullOrEmpty(_wId))
                        {
                            view.SetColumnError(grdColItem, "A warehouse must be selected.");
                            e.Valid = false;
                        }
                    }
                    else
                    {
                        view.SetColumnError(grdColItem, "An item request need to be set for a warehouse location.");
                        e.Valid = false;
                    }
                }
                bool _isValid = _saleDetails.Validate();
                if (!_isValid)
                    GimjaHelper.ShowError("Invalid data found.");
                e.Valid = _isValid;
            }
            var _qtyVal = view.GetRowCellValue(e.RowHandle, grdColQty);
            int qty;
            bool isValidQty = int.TryParse(_qtyVal == null ? string.Empty : _qtyVal.ToString(), out qty);
            if ((isValidQty && qty <= 0) || !isValidQty)
            {
                e.Valid = false;
                view.SetColumnError(grdColQty, "Invalid Quantity.");
                return;
            }
            var _unitPriceVal = view.GetRowCellValue(e.RowHandle, grdColUnitPrice);
            double _price;
            bool isValidPrice = double.TryParse(_unitPriceVal == null ? string.Empty : _unitPriceVal.ToString(), out _price);
            if ((isValidPrice && _price <= 0) || (!isValidPrice))
            {
                e.Valid = false;
                view.SetColumnError(grdColUnitPrice, "Invalid unit price.");
                return;
            }
            var _discountVal = view.GetRowCellValue(e.RowHandle, grdColDiscount);
            double _discount;
            if (_discountVal == null)
            {
                view.SetRowCellValue(e.RowHandle, grdColDiscount, 0);
            }
            bool isValidDiscount = double.TryParse(_discountVal == null ? string.Empty : _discountVal.ToString(), out _discount);
            if (isValidDiscount && _discount < 0)
            {
                e.Valid = false;
                view.SetColumnError(grdColDiscount, "Invalid discount value.");
                return;
            }
            var _itemVal = view.GetRowCellValue(e.RowHandle, grdColItem);
            if (_itemVal == null || string.IsNullOrWhiteSpace(_itemVal.ToString()))
            {
                e.Valid = false;
                view.SetColumnError(grdColItem, "Item selection is required.");
                return;
            }
            int _saleFrom;
            var _pickLocationVal = view.GetRowCellValue(e.RowHandle, grdColPickFrom);
            bool _isValidLocation = int.TryParse(_pickLocationVal == null ? string.Empty : _pickLocationVal.ToString(), out _saleFrom);
            if (string.IsNullOrWhiteSpace(_pickLocationVal.ToString()) || !_isValidLocation || _saleFrom <= 0)
            {
                e.Valid = false;
                view.SetColumnError(grdColPickFrom, "Please choose where the sales is from.");
                return;
            }
            //is the sales in credit and the pending status is found
            //bool _isCreditSales = Convert.ToBoolean(view.GetRowCellValue(e.RowHandle, grdColItemUnit));
            //if (_isCreditSales)
            //{//the sales is expected to be in credit
            //    var _creditStatusList = CreditStatusBL.GetCreditStatusList(true);
            //    if (_creditStatusList == null || _creditStatusList.Count == 0)
            //    {
            //        e.Valid = false;
            //        view.SetColumnError(grdColItemUnit, "There is no possible credit status to set for credit sales.");
            //    }
            //    else
            //    {
            //        var _status = _creditStatusList.Where(x => x.Name.ToLower().Equals("pending")).SingleOrDefault();
            //        if (_status == null)
            //        {
            //            e.Valid = false;
            //            view.SetColumnError(grdColItemUnit, "The pending credit status could not be found for credit sales.");
            //        }
            //    }
            //}
            if (_price < _discount)
            {
                e.Valid = false;
                view.SetColumnError(grdColTotalPrice, "Unit price cannot be less than discount!");
                return;
            }
            var _groupCounts = from sd in salesDetails
                               group sd by sd.ItemID into Items
                               select Items.Count();

            var _multipleItems = _groupCounts.Any(g => g > 1);
            if (_multipleItems)
            {
                e.Valid = false;
                view.SetColumnError(grdColItem, "Duplicate item found.");
            }
            if (isUpdating)
            {
                bool isCredit = Convert.ToBoolean(view.GetRowCellValue(e.RowHandle, grdColItemUnit));
                SalesCreditStatusData _payment = CreditStatusBL.GetCreditStatus(txtReferenceNo.Text.Trim(), _itemVal.ToString());
                if (_payment != null && _payment.CreditStatusID != 1)
                {//the credit status is modified, hence invalid to change the status - infact anything
                    view.SetColumnError(grdColItemUnit, "The credit payment is already modified.");
                    e.Valid = false;
                }
            }
        }

        private void grdViewSaleDetails_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
        }

        private void grdViewSaleDetails_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData)
            {
                if (e.Column == grdColSerialNo)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
                else if (e.Column == grdColUnitPrice)
                {
                    SalesDetailData _data = (SalesDetailData)e.Row;
                    if (_data != null)
                    {
                        var _item = ItemBL.GetItem(_data.ItemID);
                        if (_item != null)
                        {
                            e.Value = _item.UnitPrice;
                        }
                        else
                            e.Value = 0d;
                    }
                    else
                        e.Value = 0d;
                }
                else if (e.Column == grdColItemUnit)
                {
                    SalesDetailData _data = (SalesDetailData)e.Row;
                    if (_data != null)
                    {
                        var _item = ItemBL.GetItem(_data.ItemID);
                        if (_item != null)
                        {
                            e.Value = _item.UnitID;
                        }
                        else
                            e.Value = null;
                    }
                    else
                        e.Value = null;
                }
                //else if (e.Column == grdColTotalPrice)
                //{
                //    var _details = (SalesDetailData)e.Row;
                //    if (_details != null)
                //    {
                //        var _item = ItemBL.GetItem(_details.ItemId);
                //        if (_item != null)
                //        {
                //            double _unitPrice = _item.UnitPrice ?? 0d;
                //            var _totalPrice = _details.Quantity * (_unitPrice - _details.Discount ?? 0);
                //            e.Value = _totalPrice;
                //        }
                //        else
                //            e.Value = 0d;
                //    }
                //}
            }
        }

        private void grdViewSaleDetails_GotFocus(object sender, EventArgs e)
        {
            if (isNewRecord || isUpdating)
            {//inserting new sales record and the sales detailes grid is focused
                //Validate the sales record
                var _branch = lkeBranches.EditValue == null ? string.Empty : lkeBranches.EditValue.ToString();
                if (string.IsNullOrWhiteSpace(_branch))
                {
                    lkeBranches.Focus();
                    GimjaHelper.ShowError("Branch is required!");
                    return;
                }
                //var _customer = lkeCustomers.EditValue == null ? string.Empty : lkeCustomers.EditValue.ToString();
                //if (string.IsNullOrWhiteSpace(_customer))
                //{
                //    lkeCustomers.Focus();
                //    GimjaHelper.ShowError("Customer is required!");
                //    return;
                //}
                var _cashier = lkeCashiers.EditValue == null ? string.Empty : lkeCashiers.EditValue.ToString();
                if (string.IsNullOrWhiteSpace(_cashier))
                {
                    lkeCashiers.Focus();
                    GimjaHelper.ShowError("Cashier is required!");
                    return;
                }
                //int _receipt;
                //bool _validReceipt = int.TryParse(lkeReceipts.EditValue == null ? string.Empty : lkeReceipts.EditValue.ToString(), out _receipt);
                //if (!_validReceipt || (_validReceipt && _receipt <= 0))
                //{
                //    lkeCashiers.Focus();
                //    GimjaHelper.ShowError("Receipt is required!");
                //    return;
                //}
            }
            if (isUpdating)
            {
                //ensure the ref no exists
            }
        }

        private void dtSalesDate_Properties_Validating(object sender, CancelEventArgs e)
        {
            DateTime _salesDate = dtSalesDate.DateTime;
            if (_salesDate < DateTime.Today.AddDays(-5) || _salesDate > DateTime.Now)
            {//valid sales date is from 5 days earlier to today
                GimjaHelper.ShowError("Invalid sales date; please retry!");
                e.Cancel = true;
            }
        }

        private void grdSaleDetails_Enter(object sender, EventArgs e)
        {
            grdViewSaleDetails_GotFocus(grdViewSaleDetails, e);
        }

        private void grdViewSales_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var view = (GridView)sender;
            if (view != null)
            {
                UpdateSalesDisplay(e.FocusedRowHandle);
            }
        }

        private void UpdateSalesDisplay(int focusedRowHandle)
        {
            if (focusedRowHandle >= 0)
            {
                var _rowSalesData = (SalesData)grdViewSales.GetRow(focusedRowHandle);//get the currently focused row
                if (_rowSalesData != null)
                {
                    Guid _salesId = _rowSalesData.ID;
                    salesData = SalesBL.GetSales(_salesId);

                    var _details = SalesBL.GetSaleDetails(_salesId);
                    this.salesDetails = new BindingList<SalesDetailData>(_details);
                    ShowControlValues(salesData, this.salesDetails);
                    if (salesData != null && string.IsNullOrEmpty(salesData.AuthorizedBy))
                    {//the sales data is existing
                        btnEdit.Enabled = true;
                        if (!(salesData.IsVoid ?? false))
                            btnVoid.Enabled = true;
                    }
                    //update the summary fields
                    CalculateSalesTotal();
                }
            }
        }

        private void grdViewSaleDetails_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            CalculateSalesTotal();
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            frmCustomer customer = new frmCustomer();
            customer.WindowState = FormWindowState.Normal;
            customer.MaximizeBox = false;
            customer.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            customer.StartPosition = FormStartPosition.CenterParent;
            customer.ShowDialog(this);
            //update the customers list if any new customer is added
            PopulateCustomers();
        }

        private void btnRefreshSalesList_Click(object sender, EventArgs e)
        {
            if (!(isNewRecord || isUpdating))
            {
                PopulateSalesList();
            }
        }

        private void btnShowReport_Click(object sender, EventArgs e)
        {
            if (isNewRecord || isUpdating)
            {//the sales record is not saved
                GimjaHelper.ShowError("The current sales record is not saved. Please save and retry.");
                return;
            }
            else
            {
                var _currentSalesRecord = (SalesData)grdViewSales.GetFocusedRow();
                if (_currentSalesRecord != null)
                {
                    frmSalesAttachment _attachment = new frmSalesAttachment(_currentSalesRecord);
                    _attachment.ShowDialog(this);
                }
                else
                {
                    GimjaHelper.ShowError("No selected sales record to show!");
                }
            }
        }
    }
}