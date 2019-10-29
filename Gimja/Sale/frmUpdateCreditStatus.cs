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
    public partial class frmUpdateCreditStatus : DevExpress.XtraEditors.XtraForm
    {
        IList<CustomerLedgerData> sourceData;
        CreditStatusBL creditLogic = new CreditStatusBL();
        CustomerLedgerBL customerLedger = new CustomerLedgerBL();

        public frmUpdateCreditStatus()
        {
            InitializeComponent();

            PopulateCreditSalesList();
        }

        private void PopulateCreditSalesList()
        {
            sourceData = new BindingList<CustomerLedgerData>(customerLedger.GetNewCustomerLedgers());

            grdCreditSalesStatus.DataSource = sourceData;
            if (sourceData.Count > 0)
            {
                btnSave.Enabled = true;
                btnSaveClose.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveCreditStatusChanges())
                PopulateCreditSalesList();
        }

        private bool SaveCreditStatusChanges()
        {
            btnRefresh.Enabled = false;
            int _rows = grdViewCreditSalesStatus.RowCount;
            if (_rows > 0)
            {//there are rows displayed
                //get the modified rows (their amount paid exceeds zero)
                var _modifiedLedgers = sourceData.Where(l => l.AmountPaid > 0).ToList();
                if (_modifiedLedgers.Count > 0)
                {
                    bool _result = customerLedger.InsertLedgers(_modifiedLedgers);
                    if (_result) GimjaHelper.ShowInformation("Updated Successfully.");
                    return _result;
                }
                else
                {
                    GimjaHelper.ShowError("No sales records to update.");
                }
            }
            btnRefresh.Enabled = true;
            return false;
        }

        private void frmUpdateCreditStatus_Load(object sender, EventArgs e)
        {
            PopulateCreditStatusList();
        }

        private void PopulateCreditStatusList()
        {
            var _statusList = CreditStatusBL.GetCreditStatusList();
            repositoryCreditStatus.DataSource = _statusList;
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            if (SaveCreditStatusChanges())
                this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            PopulateCreditSalesList();
        }

        private void grdViewCreditSalesStatus_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (view != null)
            {
                if (e.Column == grdColAmountPaid)
                {//amount paid is changed
                    //add all amount paid values and compare with the receivable - if equal or more, change credit status to paid
                    var _currentRow = (CustomerLedgerData)view.GetRow(e.RowHandle);
                    if (_currentRow != null)
                    {
                        var _refNo = _currentRow.ReferenceNo;
                        bool isCompleted = customerLedger.IsCompleted(_refNo);
                        if (isCompleted)
                            grdViewCreditSalesStatus.SetRowCellValue(e.RowHandle, grdColCreditStatus, 2);//TODO: ASSUMING 2 IS PAID CREDIT STATUS
                    }
                }
            }
        }

        private void grdViewCreditSalesStatus_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (view != null)
            {
                view.ClearColumnErrors();//clear any previous columns
                //get the row
                var _currentRow = (CustomerLedgerData)e.Row;
                if (_currentRow != null)
                {
                    if (_currentRow.AmountPaid <= 0)
                    {
                        view.SetColumnError(grdColAmountPaid, "Invalid amount.");
                        e.Valid = false;
                    }
                }
            }
        }

        private void grdViewCreditSalesStatus_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {

        }

        int _clickedRowIndex = -1;
        private void grdViewCreditSalesStatus_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Column)
            {
                e.Allow = false;
            }
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                //clear the existing menu, if any
                e.Menu.Items.Clear();
                //add the write-off menu
                e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Write Off", OnPopupEventHandler));
                _clickedRowIndex = e.HitInfo.RowHandle;
            }
        }

        private void OnPopupEventHandler(object sender, EventArgs e)
        {
            var menu = (DevExpress.Utils.Menu.DXMenuItem)sender;
            if (menu != null)
            {
                switch (menu.Caption.ToLower())
                {
                    case "write off":
                        if (_clickedRowIndex >= 0 && _clickedRowIndex < grdViewCreditSalesStatus.RowCount)
                        {
                            var _selectedStatus = (CustomerLedgerData)grdViewCreditSalesStatus.GetRow(_clickedRowIndex);
                            if (_selectedStatus != null)
                            {
                                var _response = GimjaHelper.ShowQuestion(string.Format("Are you sure to write off the credit owed by {0} worth of {1}?", _selectedStatus.CustomerName, _selectedStatus.Remaining));
                                if (_response == System.Windows.Forms.DialogResult.Yes)
                                {
                                    var _userId = Singleton.Instance.UserID;
                                    var _writeOff = new WriteOffData()
                                    {
                                        CustomerID = _selectedStatus.CustomerID,
                                        Amount = _selectedStatus.Remaining,
                                        CreatedBy = _userId,
                                        CreatedDate = DateTime.Now,
                                        LastUpdatedBy = _userId,
                                        LastUpdatedDate = DateTime.Now
                                    };
                                    bool _isSaved = CustomerLedgerBL.SaveWriteOff(_writeOff, _selectedStatus.ID);
                                    if (_isSaved)
                                    {
                                        GimjaHelper.ShowInformation("The customer ledger is written off.");
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