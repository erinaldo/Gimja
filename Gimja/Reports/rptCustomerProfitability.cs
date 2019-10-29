using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Gimja
{
    public partial class rptCustomerProfitability : DevExpress.XtraReports.UI.XtraReport
    {
        public rptCustomerProfitability()
        {
            InitializeComponent();

            lblCurrentDate.Text = DateTime.Now.ToString("D");
        }

        private void rptCustomerProfitability_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

    }
}
