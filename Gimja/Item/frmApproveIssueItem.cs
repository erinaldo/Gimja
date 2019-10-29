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
    public partial class frmApproveIssueItem : DevExpress.XtraEditors.XtraForm
    {
        private IList<IssueData> issues;
        public frmApproveIssueItem()
        {
            InitializeComponent();
            PopulateIssuances();
        }

        private void PopulateIssuances()
        {
            issues = IssueBL.GetIssuances2Approve();
            grdIssuance.DataSource = issues;
        }

        int _clickedRowIndex = -1;
        private void grdViewIssuance_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Column)
                e.Allow = false;
            else if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {//the menu type is displayed when a row is right clicked
                e.Menu.Items.Clear();//clear any previous menu items
                if (e.HitInfo.InRow)
                {
                    var _rowSelected = grdViewIssuance.IsRowSelected(e.HitInfo.RowHandle);
                    if (_rowSelected)
                        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Deselect", OnPopupMenuClick));
                    else
                        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Select", OnPopupMenuClick));

                    _clickedRowIndex = e.HitInfo.RowHandle;
                }
                e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Refresh", OnPopupMenuClick));
                if (grdViewIssuance.SelectedRowsCount == grdViewIssuance.RowCount)
                {//all rows are selected
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Deselect All", OnPopupMenuClick));
                }
                else
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Select All", OnPopupMenuClick));
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
                        if (_clickedRowIndex >= 0 && _clickedRowIndex < grdViewIssuance.RowCount)
                        {//select this row only
                            grdViewIssuance.SelectRow(_clickedRowIndex);
                        }
                        break;
                    case "DESELECT":
                        if (_clickedRowIndex >= 0 && _clickedRowIndex < grdViewIssuance.RowCount)
                        {//select this row only
                            grdViewIssuance.UnselectRow(_clickedRowIndex);
                        }
                        break;
                    case "REFRESH":
                        issues = new List<IssueData>();
                        PopulateIssuances();
                        break;
                    case "SELECT ALL":
                        grdViewIssuance.SelectAll();
                        break;
                    case "DESELECT ALL":
                        for (int r = 0; r < grdViewIssuance.RowCount; r++)
                        {
                            grdViewIssuance.UnselectRow(r);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void grdViewIssuance_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData)
            {
                if (e.Column == colIssuanceIsApproved)
                {
                    var _i = (IssueData)e.Row;
                    if (_i != null)
                        e.Value = !string.IsNullOrEmpty(_i.ApprovedBy) && _i.ApprovedDate.HasValue;
                }
            }
        }

        private void btnCloseIssuance_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApproveIssuance_Click(object sender, EventArgs e)
        {
            if (grdViewIssuance.SelectedRowsCount > 0)
            {//there are selected rows
                var _response = GimjaHelper.ShowQuestion(string.Format("Are you sure to approve the selected {0} issuance records.", grdViewIssuance.SelectedRowsCount));
                if (_response == System.Windows.Forms.DialogResult.Yes)
                {
                    //get the selected records
                    var _selectedIssuances = new List<IssueData>();
                    var _selectedRowIndices = grdViewIssuance.GetSelectedRows();
                    for (int r = 0; r < _selectedRowIndices.Length; r++)
                    {
                        var _issue = (IssueData)grdViewIssuance.GetRow(r);
                        if (_issue != null)
                            _selectedIssuances.Add(_issue);
                    }
                    if (_selectedIssuances.Count > 0)
                    {
                        bool _isApproved = IssueBL.Approve(_selectedIssuances, "LogonUser");
                        if (_isApproved)
                        {
                            GimjaHelper.ShowInformation(string.Format("{0} receipt record{1} approved", _selectedIssuances.Count, (_selectedIssuances.Count > 1) ? "s are" : " is"));
                            PopulateIssuances();
                        }
                    }
                }
            }
            else
                GimjaHelper.ShowError("Please select an issuance record to approve.");
        }
    }
}