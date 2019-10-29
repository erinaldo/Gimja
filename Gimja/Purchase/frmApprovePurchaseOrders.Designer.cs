namespace Gimja
{
    partial class frmApprovePurchaseOrders
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmApprovePurchaseOrders));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnApprove = new DevExpress.XtraEditors.SimpleButton();
            this.grdPurchaseOrders = new DevExpress.XtraGrid.GridControl();
            this.grdViewPurchaseOrders = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColSupplier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lkeRepositorySupplier = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColProcessedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColOrderQuantity = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColUnitPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColItemCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdPurchaseOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewPurchaseOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkeRepositorySupplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.btnApprove);
            this.layoutControl1.Controls.Add(this.grdPurchaseOrders);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(317, 179, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(781, 440);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnClose
            // 
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(638, 406);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(131, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnApprove
            // 
            this.btnApprove.Image = ((System.Drawing.Image)(resources.GetObject("btnApprove.Image")));
            this.btnApprove.Location = new System.Drawing.Point(503, 406);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(131, 22);
            this.btnApprove.StyleController = this.layoutControl1;
            this.btnApprove.TabIndex = 5;
            this.btnApprove.Text = "Approve";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            // 
            // grdPurchaseOrders
            // 
            this.grdPurchaseOrders.Location = new System.Drawing.Point(12, 12);
            this.grdPurchaseOrders.MainView = this.grdViewPurchaseOrders;
            this.grdPurchaseOrders.Name = "grdPurchaseOrders";
            this.grdPurchaseOrders.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.lkeRepositorySupplier});
            this.grdPurchaseOrders.Size = new System.Drawing.Size(757, 390);
            this.grdPurchaseOrders.TabIndex = 4;
            this.grdPurchaseOrders.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewPurchaseOrders});
            // 
            // grdViewPurchaseOrders
            // 
            this.grdViewPurchaseOrders.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColId,
            this.grdColSupplier,
            this.grdColProcessedBy,
            this.grdColDate,
            this.grdColOrderQuantity,
            this.grdColUnitPrice,
            this.grdColItemCount});
            this.grdViewPurchaseOrders.GridControl = this.grdPurchaseOrders;
            this.grdViewPurchaseOrders.Name = "grdViewPurchaseOrders";
            this.grdViewPurchaseOrders.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.grdViewPurchaseOrders.OptionsBehavior.Editable = false;
            this.grdViewPurchaseOrders.OptionsSelection.MultiSelect = true;
            this.grdViewPurchaseOrders.OptionsView.ShowGroupPanel = false;
            this.grdViewPurchaseOrders.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.grdViewPurchaseOrders_SelectionChanged);
            // 
            // grdColId
            // 
            this.grdColId.Caption = "ID";
            this.grdColId.FieldName = "Id";
            this.grdColId.Name = "grdColId";
            // 
            // grdColSupplier
            // 
            this.grdColSupplier.Caption = "Supplier";
            this.grdColSupplier.ColumnEdit = this.lkeRepositorySupplier;
            this.grdColSupplier.FieldName = "SupplierID";
            this.grdColSupplier.Name = "grdColSupplier";
            this.grdColSupplier.Visible = true;
            this.grdColSupplier.VisibleIndex = 0;
            // 
            // lkeRepositorySupplier
            // 
            this.lkeRepositorySupplier.AutoHeight = false;
            this.lkeRepositorySupplier.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lkeRepositorySupplier.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SupplierID", "ID"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyName", "Name")});
            this.lkeRepositorySupplier.DisplayMember = "CompanyName";
            this.lkeRepositorySupplier.Name = "lkeRepositorySupplier";
            this.lkeRepositorySupplier.NullText = "";
            this.lkeRepositorySupplier.ValueMember = "SupplierID";
            // 
            // grdColProcessedBy
            // 
            this.grdColProcessedBy.Caption = "Processed By";
            this.grdColProcessedBy.FieldName = "ProcessedBy";
            this.grdColProcessedBy.Name = "grdColProcessedBy";
            this.grdColProcessedBy.Visible = true;
            this.grdColProcessedBy.VisibleIndex = 1;
            // 
            // grdColDate
            // 
            this.grdColDate.Caption = "Date";
            this.grdColDate.FieldName = "Date";
            this.grdColDate.Name = "grdColDate";
            this.grdColDate.Visible = true;
            this.grdColDate.VisibleIndex = 2;
            // 
            // grdColOrderQuantity
            // 
            this.grdColOrderQuantity.Caption = "Order Quantity";
            this.grdColOrderQuantity.FieldName = "OrderQuantity";
            this.grdColOrderQuantity.Name = "grdColOrderQuantity";
            this.grdColOrderQuantity.Visible = true;
            this.grdColOrderQuantity.VisibleIndex = 3;
            // 
            // grdColUnitPrice
            // 
            this.grdColUnitPrice.Caption = "Unit Price";
            this.grdColUnitPrice.FieldName = "UnitPrice";
            this.grdColUnitPrice.Name = "grdColUnitPrice";
            this.grdColUnitPrice.Visible = true;
            this.grdColUnitPrice.VisibleIndex = 4;
            // 
            // grdColItemCount
            // 
            this.grdColItemCount.Caption = "# of Items";
            this.grdColItemCount.FieldName = "ItemCount";
            this.grdColItemCount.Name = "grdColItemCount";
            this.grdColItemCount.Visible = true;
            this.grdColItemCount.VisibleIndex = 5;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem1,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(781, 440);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 394);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(491, 26);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.grdPurchaseOrders;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(761, 394);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnApprove;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(491, 394);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(135, 26);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(135, 26);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(135, 26);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnClose;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(626, 394);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(135, 26);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(135, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(135, 26);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "layoutControlItem3";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // frmApprovePurchaseOrders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 440);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmApprovePurchaseOrders";
            this.Text = "Approve Purchase Orders";
            this.Load += new System.EventHandler(this.frmApprovePurchaseOrders_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdPurchaseOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewPurchaseOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkeRepositorySupplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.GridControl grdPurchaseOrders;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewPurchaseOrders;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnApprove;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraGrid.Columns.GridColumn grdColId;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSupplier;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lkeRepositorySupplier;
        private DevExpress.XtraGrid.Columns.GridColumn grdColProcessedBy;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDate;
        private DevExpress.XtraGrid.Columns.GridColumn grdColOrderQuantity;
        private DevExpress.XtraGrid.Columns.GridColumn grdColUnitPrice;
        private DevExpress.XtraGrid.Columns.GridColumn grdColItemCount;
    }
}