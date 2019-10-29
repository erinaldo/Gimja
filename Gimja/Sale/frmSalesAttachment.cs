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
    public partial class frmSalesAttachment : DevExpress.XtraEditors.XtraForm
    {
        private SalesData _currentSalesRecord;

        public frmSalesAttachment()
        {
            InitializeComponent();
        }

        public frmSalesAttachment(SalesData _currentSalesRecord)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this._currentSalesRecord = _currentSalesRecord;
            rptAttachment _attachmentReport = new rptAttachment(_currentSalesRecord);

            _attachmentReport.CreateDocument();
            this.docViewerAttachment.PrintingSystem = _attachmentReport.PrintingSystem;
        }
    }
}