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
    public partial class frmCustomerProfitabilityReportViewer : DevExpress.XtraEditors.XtraForm
    {
        public frmCustomerProfitabilityReportViewer()
        {
            InitializeComponent();

            LoadReport();
        }

        private void LoadReport()
        {
            var _custProfitabilityReport = new rptCustomerProfitability();
            _custProfitabilityReport.CreateDocument();

            this.documentViewer.PrintingSystem = _custProfitabilityReport.PrintingSystem;
        }
    }
}