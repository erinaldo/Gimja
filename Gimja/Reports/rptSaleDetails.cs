using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Gimja.Reports
{
    public partial class rptSaleDetails : DevExpress.XtraReports.UI.XtraReport
    {
        public rptSaleDetails()
        {
            InitializeComponent();
        }

        private void rptSaleDetails_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var saleId = Convert.ToString(this.Parameters["SaleID"].Value);
            var saleDetails = GimjaBL.SalesBL.GetSaleDetails(new Guid(saleId));

            this.DataSource = saleDetails;

            tblCellItem.DataBindings.Add("Text", this.DataSource, "ItemID");
            tblCellSoldFrom.DataBindings.Add("Text", this.DataSource, "SalesFrom");
            tblCellQuantity.DataBindings.Add("Text", this.DataSource, "Quantity");
            tblCellUnitPrice.DataBindings.Add("Text", this.DataSource, "UnitPrice");
            tblCellDiscount.DataBindings.Add("Text", this.DataSource, "Discount");
            //tblCellTotalPrice
        }
    }
}
