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
    public partial class frmIssuance : DevExpress.XtraEditors.XtraForm
    {
        private IssueData issueData;
        private IList<IssuedItemData> issueItems;
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
            public long Available { get; set; }
        }

        public frmIssuance()
        {
            InitializeComponent();
            issueData = new IssueData();
            issueItems = new List<IssuedItemData>();
            //disable step 2 and step 3 tab pages
            tabStep2.PageEnabled = false;
            tabStep3.PageEnabled = false;

            itemSelectionList = new List<ItemSelection>();
            PopulateItemList();

        }

        private void PopulateItemList()
        {
            var _items = ItemBL.GetAvailableItems();
            if (_items != null)
            {
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
                                         IsTaxExempted = i.IsTaxExempted,
                                         Available = i.Available
                                     }).ToList();
                //set as datasource
                grdItems.DataSource = itemSelectionList;
            }
            else
            {
                grdItems.DataSource = null;
                btnNext.Enabled = false;
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
            var _userLogic = new UserBL();
            var _users = _userLogic.GetData(true);//.GetUsers();
            lkeIssuedTo.Properties.DataSource = _users;
            lkeIssuedBy.Properties.DataSource = _users;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            issueItems = new List<IssuedItemData>();
            DateTime _currentDateTime = DateTime.Now;
            //get the selected issued items
            for (int i = 0; i < grdViewItems.RowCount; i++)
            {
                var _item = (ItemSelection)grdViewItems.GetRow(i);
                var _isItemSelected = Convert.ToBoolean(grdViewItems.GetRowCellValue(i, colItemSelected));
                if (_isItemSelected && _item != null)
                {
                    issueItems.Add(new IssuedItemData()
                    {
                        IssueDetailID = Guid.NewGuid(),
                        ItemID = _item.ItemID,
                        Quantity = (int)_item.Available,
                        CreatedBy = GimjaHelper.GetCurrentUserID(this), //"LogonUser",//TODO: ADD THE LOGON USER HERE
                        CreatedDate = _currentDateTime
                    });
                }
            }
            if (issueItems.Count > 0)
            {//there are selected items
                grdIssuedItems.DataSource = issueItems;
                tabStep2.PageEnabled = true;
                tabIssueItem.SelectedTabPage = tabStep2;
                //get the logon user as processedby user
                lkeIssuedBy.EditValue = "aman";// GimjaHelper.GetCurrentUserID(this); //"LogonUser";//TODO: ADD THE LOGON USER HERE.
            }
            else
            {
                GimjaHelper.ShowError("There are no selected items to issue.");
            }
        }

        private void tabIssueItem_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            if (e.Page == tabStep2)
            {
                PopulateStoresList();
                PopulateUsersList();
                //set the current date as default date
                dtIssueDate.DateTime = DateTime.Today;
            }
        }

        private void tabIssueItem_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page == tabStep1)
            {
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

        private void grdIssuedItems_Enter(object sender, EventArgs e)
        {
            bool _isValid = ValidateIssuanceData();
        }

        private bool ValidateIssuanceData()
        {
            if (string.IsNullOrEmpty(Convert.ToString(lkeStoreId.EditValue)))
            {
                lkeStoreId.Focus();
                GimjaHelper.ShowError("The branch is required.");
                return false;
            }
            if (string.IsNullOrEmpty(Convert.ToString(lkeWarehouseId.EditValue)))
            {
                lkeWarehouseId.Focus();
                GimjaHelper.ShowError("The warehouse is required.");
                return false;
            }
            if (string.IsNullOrEmpty(Convert.ToString(lkeIssuedTo.EditValue)))
            {
                lkeIssuedTo.Focus();
                GimjaHelper.ShowError("The person to whom the items are being issued is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(lkeIssuedBy.Text))
            {
                lkeIssuedBy.Focus();
                GimjaHelper.ShowError("The person issueing the items is required.");
                return false;
            }
            if (dtIssueDate.DateTime > DateTime.Now || dtIssueDate.DateTime < DateTime.Today.AddDays(-5))
            {
                dtIssueDate.Focus();
                GimjaHelper.ShowError("Invalid issue date.");
                return false;
            }
            return true;
        }

        private void grdViewIssuedItems_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            grdViewIssuedItems.ClearColumnErrors();
            var _detail = (IssuedItemData)e.Row;
            if (_detail != null)
                e.Valid = ValidateIssueDetail(_detail);
            else
                e.Valid = false;
        }

        private bool ValidateIssueDetail(IssuedItemData _detail)
        {
            bool _result = true;
            if (_detail.Quantity <= 0)
            {
                grdViewIssuedItems.SetColumnError(grdColIssuanceQuantity, "Invalid issued item quantity.");
                _result = false;
            }
            var _item = itemSelectionList.Where(x => x.ItemID.Equals(_detail.ItemID)).SingleOrDefault();
            if (_detail.Quantity > _item.Available)
            {
                grdViewIssuedItems.SetColumnError(grdColIssuanceQuantity, "Quantity cannot exceed the available quantity.");
                _result = false;
            }
            if (_detail.NoPack < 0)
            {
                grdViewIssuedItems.SetColumnError(grdColIssuanceNoPack, "Invalid number of packs value.");
                _result = false;
            }
            if (_detail.QtyPerPack < 0)
            {
                grdViewIssuedItems.SetColumnError(grdColIssuanceQtyPerPack, "Invalid quantity per pack.");
                _result = false;
            }
            return _result;
        }

        private void btnNext2_Click(object sender, EventArgs e)
        {
            bool _invalidRow = false;
            for (int i = 0; i < grdViewIssuedItems.RowCount; i++)
            {
                var _detail = (IssuedItemData)grdViewIssuedItems.GetRow(i);
                if (_detail != null)
                    _invalidRow = !ValidateIssueDetail(_detail);
                else
                    _invalidRow = false;
                if (_invalidRow)
                {
                    grdViewIssuedItems.FocusedRowHandle = i;
                    break;//already an invalid row is found.
                }
            }
            if (_invalidRow)
            {
                GimjaHelper.ShowError("Invalid row is found.");
                return;
            }
            if (ValidateIssuanceData())
            {
                for (int i = 0; i < grdViewIssuedItems.RowCount; i++)
                {
                    var _detail = (IssuedItemData)grdViewIssuedItems.GetRow(i);
                    if (_detail != null)
                    {
                        bool _result = ValidateIssueDetail(_detail);
                        if (!_result)
                        {
                            grdViewIssuedItems.Focus();
                            break;
                        }
                    }
                }
                var _branchId = Convert.ToString(lkeStoreId.EditValue);
                issueData = new IssueData()
                {
                    ID = IssueBL.GetIssueNumber(_branchId),
                    StoreID = _branchId,
                    WarehouseID = Convert.ToString(lkeWarehouseId.EditValue),
                    IssuedTo = Convert.ToString(lkeIssuedTo.EditValue),
                    IssuedBy = Convert.ToString(lkeIssuedBy.EditValue),
                    Date = dtIssueDate.DateTime,
                    CreatedBy = "LogonUser",
                    CreatedDate = DateTime.Now
                };
                foreach (var _issueItem in issueItems)
                {
                    _issueItem.IssuanceID = issueData.ID;
                    _issueItem.CreatedBy = "LogonUser";
                    _issueItem.CreatedDate = DateTime.Now;
                }
                //prepare to preview
                string previewIssue = string.Format("{0} items issued to '{1}' by {2}\n" +
                    "Warehouse: \t{3}\nDate: \t{4}\nUser: \t{5}\n", issueItems.Count, issueData.StoreID,
                    issueData.IssuedBy, issueData.WarehouseID, issueData.Date, issueData.IssuedTo);
                lblPreviewIssueItems.Text = previewIssue;
                grdPreviewIssueItems.DataSource = issueItems;
                tabStep3.PageEnabled = true;
                tabIssueItem.SelectedTabPage = tabStep3;
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            //insert the receipt with details and reset the tab to 1st page
            bool _result = IssueBL.Insert(issueData, issueItems);
            if (_result)
            {
                GimjaHelper.ShowInformation("The issue data is inserted successfully.");
                issueData = new IssueData();
                issueItems = new List<IssuedItemData>();
                tabIssueItem.SelectedTabPage = tabStep1;

                itemSelectionList = new List<ItemSelection>();
                PopulateItemList();
                //reset the user and store selections
                lkeStoreId.EditValue = null;
                lkeWarehouseId.EditValue = null;
                lkeIssuedTo.EditValue = null;
            }
        }
    }
}