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

namespace Gimja.Sale
{
    public partial class frmSalesSelectionList : DevExpress.XtraEditors.XtraForm
    {
        private Guid? salesId;

        private IList<SalesData> salesList;

        public frmSalesSelectionList()
        {
            InitializeComponent();
            salesList = new List<SalesData>();
        }

        private void frmSalesSelectionList_Load(object sender, EventArgs e)
        {
            salesList = SalesBL.GetSales();
            grdSalesList.DataSource = salesList;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            salesId = null;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var _selectedIndex = grdViewSalesList.FocusedRowHandle;
            if (_selectedIndex >= 0)
            {
                var _salesIdVal = grdViewSalesList.GetListSourceRowCellValue(_selectedIndex, "SalesID");
                Guid _salesId = new Guid(_salesIdVal.ToString());
                salesId = _salesId;
            }
        }
    }
}