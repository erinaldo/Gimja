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
    public partial class frmRequestIssueReportViewer : DevExpress.XtraEditors.XtraForm
    {
        public frmRequestIssueReportViewer()
        {
            InitializeComponent();

            LoadReport();
        }

        private void LoadReport()
        {
            var _requestIssueReport = new rptRequestIssue();

            _requestIssueReport.DataSource = new ItemBL().RequestIssue();
            _requestIssueReport.CreateDocument();
            this.documentViewer.PrintingSystem = _requestIssueReport.PrintingSystem;
        }

        private void ctrlDateFilter1_ButtonClick(object sender, DateFilterEventArgs x)
        {
            GimjaHelper.ShowInformation(string.Format("From {0} to {1}", ctrlDateFilter1.DateFrom, ctrlDateFilter1.DateTo));
        }
    }
}