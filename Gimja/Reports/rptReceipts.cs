using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GimjaBL;

namespace Gimja.Reports
{
    public partial class rptReceipts : DevExpress.XtraReports.UI.XtraReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public rptReceipts()
        {
            InitializeComponent();

            PopulateReceipts();
            var rptDetails = new rptReceiptDetails();

            xrSubreport1.ReportSource = rptDetails;
        }

        private void PopulateReceipts()
        {
            var receipts = ReceiptBL.GetActiveReceipts();

            this.DataSource = receipts;
            //set the databindings to the controls
            lblReceiptNo.DataBindings.Add("Text", this.DataSource, "ID");
            lblReceiptDate.DataBindings.Add("Text", this.DataSource, "Date");
            lblReceivedBy.DataBindings.Add("Text", this.DataSource, "ReceivedBy");
            lblReceivedFrom.DataBindings.Add("Text", this.DataSource, "ReceivedFrom");
            
            this.GroupHeader1.GroupFields.Add(new GroupField("ID"));
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var rptDetails = ((rptReceiptDetails)((XRSubreport)sender).ReportSource);
            
            rptDetails.ReceiptId.Value = Convert.ToString(GetCurrentColumnValue("ID"));

            //DevExpress.XtraReports.Parameters.Parameter p = new DevExpress.XtraReports.Parameters.Parameter();
            //p.Name = "ReceiptId";
            //p.Value = GetCurrentColumnValue("ID");
            //((rptReceiptDetails)((XRSubreport)sender).ReportSource).Parameters.Add(p);
        }
    }
}
