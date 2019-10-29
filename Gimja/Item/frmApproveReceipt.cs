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
    public partial class frmApproveReceipt : DevExpress.XtraEditors.XtraForm
    {
        private IList<ApproveReceipt> receiptList;
        public frmApproveReceipt()
        {
            InitializeComponent();
            receiptList = new List<ApproveReceipt>();
            PopulateReceipts();
            PopulateSuppliers();
        }

        private void PopulateReceipts()
        {
            receiptList = ReceiptBL.GetReceipts2Approve();
            grdReceipts.DataSource = receiptList;
        }

        private void PopulateSuppliers()
        {
            SupplierBL supplierLogic = new SupplierBL();
            var _suppliers = supplierLogic.GetData(true);
            lkeRepositorySupplierId.DataSource = _suppliers;
        }

        int _clickedRowIndex = -1;
        private void grdViewReceipts_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {//the menu type is displayed when a row is right clicked
                e.Menu.Items.Clear();//clear any previous menu items
                if (e.HitInfo.InRow)
                {
                    var _rowSelected = grdViewReceipts.IsRowSelected(e.HitInfo.RowHandle);
                    if (_rowSelected)
                        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Deselect", OnPopupMenuClick));
                    else
                        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Select", OnPopupMenuClick));

                    _clickedRowIndex = e.HitInfo.RowHandle;
                }
                e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Refresh", OnPopupMenuClick));
                if (grdViewReceipts.SelectedRowsCount == grdViewReceipts.RowCount)
                {//all rows are selected
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Deselect All", OnPopupMenuClick));
                }
                else
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Select All", OnPopupMenuClick));
            }
            else if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Column)
            {
                e.Allow = false;
            }
        }

        private void OnPopupMenuClick(object sender, EventArgs e)
        {
            var _menuClicked = (DevExpress.Utils.Menu.DXMenuItem)sender;
            if (_menuClicked != null)
            {
                switch (_menuClicked.Caption.ToUpper())
                {
                    case "SELECT":
                        if (_clickedRowIndex >= 0 && _clickedRowIndex < grdViewReceipts.RowCount)
                        {//select this row only
                            grdViewReceipts.SelectRow(_clickedRowIndex);
                        }
                        break;
                    case "DESELECT":
                        if (_clickedRowIndex >= 0 && _clickedRowIndex < grdViewReceipts.RowCount)
                        {//select this row only
                            grdViewReceipts.UnselectRow(_clickedRowIndex);
                        }
                        break;
                    case "REFRESH":
                        receiptList = new List<ApproveReceipt>();
                        PopulateReceipts();
                        break;
                    case "SELECT ALL":
                        grdViewReceipts.SelectAll();
                        break;
                    case "DESELECT ALL":
                        for (int r = 0; r < grdViewReceipts.RowCount; r++)
                        {
                            grdViewReceipts.UnselectRow(r);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void btnApproveSelected_Click(object sender, EventArgs e)
        {
            int _selectedRows = grdViewReceipts.SelectedRowsCount;
            if (_selectedRows > 0)
            {
                var _response = GimjaHelper.ShowQuestion(string.Format("Are you sure to approve the selected {0} receipt item(s).", _selectedRows));
                if (_response == System.Windows.Forms.DialogResult.Yes)
                {
                    //get the selected records
                    var _selectedReceipts = new List<ApproveReceipt>();
                    var _selectedRowIndices = grdViewReceipts.GetSelectedRows();
                    for (int r = 0; r < _selectedRowIndices.Length; r++)
                    {
                        var _receipt = (ApproveReceipt)grdViewReceipts.GetRow(r);
                        if (_receipt != null)
                            _selectedReceipts.Add(_receipt);
                    }
                    if (_selectedReceipts.Count > 0)
                    {
                        bool _isApproved = ReceiptBL.Approve(_selectedReceipts, "LogonUser");
                        if (_isApproved)
                        {
                            GimjaHelper.ShowInformation(string.Format("{0} receipt record{1} approved", _selectedReceipts.Count, (_selectedReceipts.Count > 1) ? "s are" : " is"));
                            PopulateReceipts();
                        }
                    }
                }
            }
            else
                GimjaHelper.ShowInformation("There are no selected receipt items to approve.");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}