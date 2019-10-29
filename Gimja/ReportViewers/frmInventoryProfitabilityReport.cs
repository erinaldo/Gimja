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
    public partial class frmInventoryProfitabilityReport : DevExpress.XtraEditors.XtraForm
    {
        public frmInventoryProfitabilityReport()
        {
            InitializeComponent();

            LoadReport();
        }

        private void LoadReport()
        {
            var _inventoryProfitability = new rptInventoryProfitability();
            _inventoryProfitability.CreateDocument();
            this.documentViewer.PrintingSystem = _inventoryProfitability.PrintingSystem;
        }

        private void frmInventoryProfitabilityReport_Load(object sender, EventArgs e)
        {

        }
    }
}