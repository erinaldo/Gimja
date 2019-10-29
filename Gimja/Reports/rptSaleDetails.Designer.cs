namespace Gimja.Reports
{
    partial class rptSaleDetails
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.tblCellItem = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblCellSoldFrom = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblCellQuantity = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblCellUnitPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblCellDiscount = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblCellTotalPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.SaleID = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
            this.Detail.HeightF = 26.04167F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable2
            // 
            this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(10F, 0F);
            this.xrTable2.Name = "xrTable2";
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
            this.xrTable2.SizeF = new System.Drawing.SizeF(610.8333F, 24.99999F);
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tblCellItem,
            this.tblCellSoldFrom,
            this.tblCellQuantity,
            this.tblCellUnitPrice,
            this.tblCellDiscount,
            this.tblCellTotalPrice});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Weight = 1D;
            // 
            // tblCellItem
            // 
            this.tblCellItem.Name = "tblCellItem";
            this.tblCellItem.Text = "Item";
            this.tblCellItem.Weight = 0.5D;
            // 
            // tblCellSoldFrom
            // 
            this.tblCellSoldFrom.Name = "tblCellSoldFrom";
            this.tblCellSoldFrom.Text = "Sold From";
            this.tblCellSoldFrom.Weight = 0.5D;
            // 
            // tblCellQuantity
            // 
            this.tblCellQuantity.Name = "tblCellQuantity";
            this.tblCellQuantity.Text = "Quantity";
            this.tblCellQuantity.Weight = 0.5D;
            // 
            // tblCellUnitPrice
            // 
            this.tblCellUnitPrice.Name = "tblCellUnitPrice";
            this.tblCellUnitPrice.Text = "Unit Price";
            this.tblCellUnitPrice.Weight = 0.5D;
            // 
            // tblCellDiscount
            // 
            this.tblCellDiscount.Name = "tblCellDiscount";
            this.tblCellDiscount.Text = "Discount";
            this.tblCellDiscount.Weight = 0.5D;
            // 
            // tblCellTotalPrice
            // 
            this.tblCellTotalPrice.Name = "tblCellTotalPrice";
            this.tblCellTotalPrice.Text = "Total Price";
            this.tblCellTotalPrice.Weight = 0.5D;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 0F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 0F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // SaleID
            // 
            this.SaleID.Description = "The Sale ID";
            this.SaleID.Name = "SaleID";
            this.SaleID.Visible = false;
            // 
            // rptSaleDetails
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Margins = new System.Drawing.Printing.Margins(100, 100, 0, 0);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.SaleID});
            this.Version = "13.1";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.rptSaleDetails_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRTable xrTable2;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
        private DevExpress.XtraReports.UI.XRTableCell tblCellItem;
        private DevExpress.XtraReports.UI.XRTableCell tblCellSoldFrom;
        private DevExpress.XtraReports.UI.XRTableCell tblCellQuantity;
        private DevExpress.XtraReports.UI.XRTableCell tblCellUnitPrice;
        private DevExpress.XtraReports.UI.XRTableCell tblCellDiscount;
        private DevExpress.XtraReports.UI.XRTableCell tblCellTotalPrice;
        internal DevExpress.XtraReports.Parameters.Parameter SaleID;
    }
}
