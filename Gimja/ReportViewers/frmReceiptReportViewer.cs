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

namespace Gimja.Reports
{
    public partial class frmReceiptReportViewer : DevExpress.XtraEditors.XtraForm
    {
        public frmReceiptReportViewer()
        {
            InitializeComponent();
            LoadReport(ctrlDateFilter1.DateFrom, ctrlDateFilter1.DateTo);
        }

        private void ctrlDateFilter1_ButtonClick(object sender, DateFilterEventArgs x)
        {
            LoadReport(ctrlDateFilter1.DateFrom, ctrlDateFilter1.DateTo);
        }

        private void LoadReport(DateTime dateTime1, DateTime dateTime2)
        {
            var receipts = new rptReceipts();
            receipts.DataSource = ReceiptBL.GetActiveReceipts(dateTime1, dateTime2);
            receipts.lblCriteria.Text = string.Format("Data from {0} to {1}.", dateTime1.ToString("D"), dateTime2.ToString("D"));
            receipts.CreateDocument();//true);
            //this.documentViewer1.DocumentSource = receipts.GetType();//typeof(rptReceipts);
            this.documentViewer1.PrintingSystem = receipts.PrintingSystem;
        }
    }
}