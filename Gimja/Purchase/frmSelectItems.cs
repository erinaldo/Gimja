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
using DevExpress.XtraGrid.Menu;

namespace Gimja
{
    public partial class frmSelectItems : DevExpress.XtraEditors.XtraForm
    {
        private IList<ItemSelectionData> allPOItems;
        private IList<ItemSelectionData> selectedItemList;
        public IList<ItemSelectionData> SelectedItems
        {
            get { return selectedItemList; }
        }

        public IList<ItemSelectionData> AllPOItems
        {
            get { return allPOItems; }
        }

        public class ItemSelectionData
        {
            public bool Selected { get; set; }
            public string ItemId { get; set; }
            public int BrandId { get; set; }
            public int CategoryId { get; set; }
            public Nullable<double> UnitPrice { get; set; }
            public Nullable<double> ReorderLevel { get; set; }
            public Nullable<double> PickFaceQty { get; set; }
            public Nullable<int> UnitId { get; set; }
            public Nullable<double> OrderQuantity { get; set; }
            public string Description { get; set; }
            public Nullable<bool> IsActive { get; set; }
            public Nullable<int> TaxTypeID { get; set; }
            public Nullable<bool> IsTaxExempted { get; set; }
            public string CreatedBy { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public string LastUpdatedBy { get; set; }
            public Nullable<System.DateTime> LastUpdatedDate { get; set; }
            public Nullable<bool> IsDeleted { get; set; }

            public long Available { get; set; }
        }

        public frmSelectItems()
        {
            InitializeComponent();
            allPOItems = new List<ItemSelectionData>();
            selectedItemList = new List<ItemSelectionData>();

            PopulatePurchaseOrderItems();
        }

        private void frmSelectItems_Load(object sender, EventArgs e)
        {
            grdSelectItems.DataSource = new List<ItemSelectionData>(allPOItems);
        }

        private void PopulatePurchaseOrderItems()
        {
            var _items = ItemBL.GetItemsForPurchaseOrder();
            allPOItems = (from i in _items
                          select new ItemSelectionData()
                          {
                              Selected = false,
                              ItemId = i.ItemID,
                              BrandId = i.BrandID,
                              CategoryId = i.CategoryID,
                              UnitPrice = i.UnitPrice,
                              ReorderLevel = i.ReorderLevel,
                              PickFaceQty = i.PickFaceQty,
                              UnitId = i.UnitID,
                              OrderQuantity = i.OrderQuantity,
                              Description = i.Description,
                              IsActive = i.IsActive,
                              TaxTypeID = i.TaxTypeID,
                              IsTaxExempted = i.IsTaxExempted,
                              CreatedBy = i.CreatedBy,
                              CreatedDate = i.CreatedDate,
                              LastUpdatedBy = i.LastUpdatedBy,
                              LastUpdatedDate = i.LastUpdatedDate,
                              IsDeleted = i.IsDeleted,
                              Available = i.Available
                          }).ToList();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            selectedItemList = allPOItems.Where(p => p.Selected).ToList();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            selectedItemList = new List<ItemSelectionData>();
        }

        private void grdViewSelectItems_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Column)
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = e.HitInfo;
                if (hi.InColumn)
                {
                    DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as GridViewColumnMenu;
                    //Erasing the default menu items 
                    menu.Items.Clear();
                    if (menu.Column != null)
                    {
                        menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Select All", OnContextMenuClick));
                        menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Deselect All", OnContextMenuClick));
                        menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Refresh", OnContextMenuClick));
                    }
                }
            }
        }

        private void OnContextMenuClick(object sender, EventArgs e)
        {
            DevExpress.Utils.Menu.DXMenuItem _menu = (DevExpress.Utils.Menu.DXMenuItem)sender;
            if (_menu != null)
            {
                if (_menu.Caption.Equals("Select All"))
                {
                    grdViewSelectItems.SelectAll();
                    var _selectedRowHandles = grdViewSelectItems.GetSelectedRows();
                    for (int i = 0; i < _selectedRowHandles.Length; i++)
                    {
                        grdViewSelectItems.SetRowCellValue(i, grdColSelect, true);
                    }
                }
                else if (_menu.Caption.Equals("Deselect All"))
                {
                    var _selectedRows = grdViewSelectItems.GetSelectedRows();
                    for (int i = 0; i < _selectedRows.Length; i++)
                    {
                        grdViewSelectItems.UnselectRow(i);
                        grdViewSelectItems.SetRowCellValue(i, grdColSelect, false);
                    }
                }
                else if (_menu.Caption.Equals("Refresh"))
                {
                    PopulatePurchaseOrderItems();
                }
            }
        }

        private void grdViewSelectItems_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Column == grdColSelect)
            {
                var _isSelected = Convert.ToBoolean(e.CellValue);
                if (_isSelected)
                    grdViewSelectItems.SelectRow(e.RowHandle);
            }
        }

        private void grdViewSelectItems_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            var _isSelected = grdViewSelectItems.IsRowSelected(e.RowHandle);
            if (_isSelected)
            {
                var _selected = Convert.ToBoolean(grdViewSelectItems.GetRowCellValue(e.RowHandle, grdColSelect));
                if (!_selected)
                {
                    grdViewSelectItems.SetRowCellValue(e.RowHandle, grdColSelect, true);
                }
            }
        }
    }
}