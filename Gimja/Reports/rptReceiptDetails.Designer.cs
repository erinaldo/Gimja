namespace Gimja.Reports
{
    partial class rptReceiptDetails
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
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.tblCellItem = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblCellManufacturer = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblCellQuantity = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblCellPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.tblCellSellingPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReceiptId = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
            this.Detail.HeightF = 25F;
            this.Detail.Name = "Detail";
            this.Detail.OddStyleName = "xrControlStyle1";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable1
            // 
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(489.5833F, 25F);
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tblCellItem,
            this.tblCellManufacturer,
            this.tblCellQuantity,
            this.tblCellPrice,
            this.tblCellSellingPrice});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 1D;
            // 
            // tblCellItem
            // 
            this.tblCellItem.Name = "tblCellItem";
            this.tblCellItem.Text = "tblCellItem";
            this.tblCellItem.Weight = 1D;
            // 
            // tblCellManufacturer
            // 
            this.tblCellManufacturer.Name = "tblCellManufacturer";
            this.tblCellManufacturer.Text = "tblCellManufacturer";
            this.tblCellManufacturer.Weight = 1.3958338412021381D;
            // 
            // tblCellQuantity
            // 
            this.tblCellQuantity.Name = "tblCellQuantity";
            this.tblCellQuantity.Text = "tblCellQuantity";
            this.tblCellQuantity.Weight = 0.71874953054871593D;
            // 
            // tblCellPrice
            // 
            this.tblCellPrice.Name = "tblCellPrice";
            this.tblCellPrice.Text = "tblCellPrice";
            this.tblCellPrice.Weight = 0.80208349836633341D;
            // 
            // tblCellSellingPrice
            // 
            this.tblCellSellingPrice.Name = "tblCellSellingPrice";
            this.tblCellSellingPrice.Text = "tblCellSellingPrice";
            this.tblCellSellingPrice.Weight = 0.9791665649414063D;
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
            // ReceiptId
            // 
            this.ReceiptId.Description = "The receipt number";
            this.ReceiptId.Name = "ReceiptId";
            this.ReceiptId.Visible = false;
            // 
            // xrControlStyle1
            // 
            this.xrControlStyle1.BackColor = System.Drawing.Color.LightGray;
            this.xrControlStyle1.Name = "xrControlStyle1";
            this.xrControlStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            // 
            // rptReceiptDetails
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.Margins = new System.Drawing.Printing.Margins(100, 100, 0, 0);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ReceiptId});
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrControlStyle1});
            this.Version = "13.1";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.rptReceiptDetails_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell tblCellItem;
        private DevExpress.XtraReports.UI.XRTableCell tblCellManufacturer;
        private DevExpress.XtraReports.UI.XRTableCell tblCellQuantity;
        private DevExpress.XtraReports.UI.XRTableCell tblCellPrice;
        private DevExpress.XtraReports.UI.XRTableCell tblCellSellingPrice;
        public DevExpress.XtraReports.Parameters.Parameter ReceiptId;
        private DevExpress.XtraReports.UI.XRControlStyle xrControlStyle1;
    }
}
