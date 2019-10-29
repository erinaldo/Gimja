using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Gimja
{
    public partial class rptInventoryProfitability : DevExpress.XtraReports.UI.XtraReport
    {
        public rptInventoryProfitability()
        {
            InitializeComponent();

            lblCurrentDate.Text = DateTime.Now.ToString("D");
        }

    }
}
