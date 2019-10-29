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
    public partial class frmApprovePurchaseOrders : DevExpress.XtraEditors.XtraForm
    {
        private IList<PurchaseOrderForApproval> purchaseOrders;
        public frmApprovePurchaseOrders()
        {
            InitializeComponent();

            purchaseOrders = new List<PurchaseOrderForApproval>();
        }

        private void frmApprovePurchaseOrders_Load(object sender, EventArgs e)
        {
            PopulatePuchaseOrders();
        }

        private void PopulatePuchaseOrders()
        {
            //get the supplier list
            var _suppliers = new SupplierBL().GetData();
            lkeRepositorySupplier.DataSource = _suppliers;

            purchaseOrders = PurchaseOrderBL.GetPurchaseOrdersToApprove();
            grdPurchaseOrders.DataSource = purchaseOrders;
        }

        private void grdViewPurchaseOrders_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            int _selectedOrders = grdViewPurchaseOrders.SelectedRowsCount;
            if (_selectedOrders > 0)
            {
                btnApprove.Text = string.Format("Approve ({0})", _selectedOrders);
                btnApprove.Enabled = true;
            }
            else
            {
                btnApprove.Text = "Approve";
                btnApprove.Enabled = false;
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            var _selectedRowHandles = grdViewPurchaseOrders.GetSelectedRows();
            List<Guid> _selectedOrders = new List<Guid>();
            foreach (var _handle in _selectedRowHandles)
            {
                Guid _poId = new Guid(Convert.ToString(grdViewPurchaseOrders.GetRowCellValue(_handle, grdColId)));
                _selectedOrders.Add(_poId);
            }
            if (_selectedOrders.Count > 0)
            {
                bool _approved = PurchaseOrderBL.ApproveAll(_selectedOrders, GimjaHelper.GetCurrentUserID(this)); //"LogonUser");//TODO: ADD THE LOGON USER HERE
                if (_approved)
                {
                    GimjaHelper.ShowInformation("Selected purchase order are approved successfully.");
                    PopulatePuchaseOrders();
                }
                else
                {
                    GimjaHelper.ShowError("Approval completed with error.");
                }
            }
            else
                GimjaHelper.ShowError("There are no selected purchase orders to approve.");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}