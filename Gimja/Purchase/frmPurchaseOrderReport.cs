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

namespace Gimja
{
    public partial class frmPurchaseOrderReport : DevExpress.XtraEditors.XtraForm
    {
        private GimjaBL.PurchaseOrderData purchaseOrder;

        public frmPurchaseOrderReport()
        {
            InitializeComponent();
        }

        public frmPurchaseOrderReport(GimjaBL.PurchaseOrderData _currentOrder)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this.purchaseOrder = _currentOrder;
            rptPurchaseOrder _poReport = new rptPurchaseOrder(_currentOrder);
            _poReport.CreateDocument();
            this.documentViewer1.PrintingSystem = _poReport.PrintingSystem;
        }
    }
}