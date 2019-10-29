using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GimjaBL;

namespace Gimja.Reports
{
    public partial class rptReceiptDetails : DevExpress.XtraReports.UI.XtraReport
    {
        public rptReceiptDetails()
        {
            InitializeComponent();
            PopulateReceiptDetails();

            tblCellItem.DataBindings.Add("Text", this.DataSource, "ItemID");
            tblCellManufacturer.DataBindings.Add("Text", this.DataSource, "ManufacturerID");
            tblCellQuantity.DataBindings.Add("Text", this.DataSource, "Quantity");
            tblCellPrice.DataBindings.Add("Text", this.DataSource, "Price");
            tblCellSellingPrice.DataBindings.Add("Text", this.DataSource, "SellingPrice");
        }

        private void PopulateReceiptDetails()
        {

        }

        private void rptReceiptDetails_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var receiptId = Convert.ToString(this.Parameters["ReceiptId"].Value);
            var receivedItems = ReceiptBL.GetReceivedItems(receiptId);

            this.DataSource = receivedItems;
        }
    }
}
