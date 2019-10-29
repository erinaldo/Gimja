namespace Gimja
{
    partial class frmApproveReceipt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmApproveReceipt));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnApproveSelected = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.grdReceipts = new DevExpress.XtraGrid.GridControl();
            this.grdViewReceipts = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColReceiptId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColSupplierId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lkeRepositorySupplierId = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColReceiptDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColStoreId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColReceivedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColQuantity = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColUnitSellingPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblApproveReceiptMessage = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdReceipts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewReceipts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkeRepositorySupplierId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnApproveSelected);
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.grdReceipts);
            this.layoutControl1.Controls.Add(this.lblApproveReceiptMessage);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(299, 180, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(747, 434);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnApproveSelected
            // 
            this.btnApproveSelected.Image = ((System.Drawing.Image)(resources.GetObject("btnApproveSelected.Image")));
            this.btnApproveSelected.Location = new System.Drawing.Point(469, 400);
            this.btnApproveSelected.Name = "btnApproveSelected";
            this.btnApproveSelected.Size = new System.Drawing.Size(131, 22);
            this.btnApproveSelected.StyleController = this.layoutControl1;
            this.btnApproveSelected.TabIndex = 8;
            this.btnApproveSelected.Text = "Approve Selected";
            this.btnApproveSelected.Click += new System.EventHandler(this.btnApproveSelected_Click);
            // 
            // btnClose
            // 
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(604, 400);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(131, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // grdReceipts
            // 
            this.grdReceipts.Location = new System.Drawing.Point(12, 29);
            this.grdReceipts.MainView = this.grdViewReceipts;
            this.grdReceipts.Name = "grdReceipts";
            this.grdReceipts.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.lkeRepositorySupplierId});
            this.grdReceipts.Size = new System.Drawing.Size(723, 367);
            this.grdReceipts.TabIndex = 5;
            this.grdReceipts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewReceipts});
            // 
            // grdViewReceipts
            // 
            this.grdViewReceipts.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColReceiptId,
            this.grdColSupplierId,
            this.grdColReceiptDate,
            this.grdColStoreId,
            this.grdColReceivedBy,
            this.grdColItemId,
            this.grdColQuantity,
            this.grdColPrice,
            this.grdColUnitSellingPrice});
            this.grdViewReceipts.GridControl = this.grdReceipts;
            this.grdViewReceipts.Name = "grdViewReceipts";
            this.grdViewReceipts.OptionsBehavior.Editable = false;
            this.grdViewReceipts.OptionsSelection.MultiSelect = true;
            this.grdViewReceipts.OptionsView.ShowGroupPanel = false;
            this.grdViewReceipts.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.grdViewReceipts_PopupMenuShowing);
            // 
            // grdColReceiptId
            // 
            this.grdColReceiptId.Caption = "Receipt #";
            this.grdColReceiptId.FieldName = "ReceiptID";
            this.grdColReceiptId.Name = "grdColReceiptId";
            this.grdColReceiptId.Visible = true;
            this.grdColReceiptId.VisibleIndex = 0;
            // 
            // grdColSupplierId
            // 
            this.grdColSupplierId.Caption = "Supplier";
            this.grdColSupplierId.ColumnEdit = this.lkeRepositorySupplierId;
            this.grdColSupplierId.FieldName = "SupplierID";
            this.grdColSupplierId.Name = "grdColSupplierId";
            this.grdColSupplierId.Visible = true;
            this.grdColSupplierId.VisibleIndex = 1;
            // 
            // lkeRepositorySupplierId
            // 
            this.lkeRepositorySupplierId.AutoHeight = false;
            this.lkeRepositorySupplierId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lkeRepositorySupplierId.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CompanyName", "Name")});
            this.lkeRepositorySupplierId.DisplayMember = "CompanyName";
            this.lkeRepositorySupplierId.Name = "lkeRepositorySupplierId";
            this.lkeRepositorySupplierId.NullText = "";
            this.lkeRepositorySupplierId.ValueMember = "SupplierID";
            // 
            // grdColReceiptDate
            // 
            this.grdColReceiptDate.Caption = "Date";
            this.grdColReceiptDate.FieldName = "Date";
            this.grdColReceiptDate.Name = "grdColReceiptDate";
            this.grdColReceiptDate.Visible = true;
            this.grdColReceiptDate.VisibleIndex = 2;
            // 
            // grdColStoreId
            // 
            this.grdColStoreId.Caption = "Store";
            this.grdColStoreId.FieldName = "StoreID";
            this.grdColStoreId.Name = "grdColStoreId";
            this.grdColStoreId.Visible = true;
            this.grdColStoreId.VisibleIndex = 3;
            // 
            // grdColReceivedBy
            // 
            this.grdColReceivedBy.Caption = "Received By";
            this.grdColReceivedBy.FieldName = "ReceivedBy";
            this.grdColReceivedBy.Name = "grdColReceivedBy";
            this.grdColReceivedBy.Visible = true;
            this.grdColReceivedBy.VisibleIndex = 4;
            // 
            // grdColItemId
            // 
            this.grdColItemId.Caption = "Item";
            this.grdColItemId.FieldName = "ItemID";
            this.grdColItemId.Name = "grdColItemId";
            this.grdColItemId.Visible = true;
            this.grdColItemId.VisibleIndex = 5;
            // 
            // grdColQuantity
            // 
            this.grdColQuantity.Caption = "Quantity";
            this.grdColQuantity.FieldName = "Quantity";
            this.grdColQuantity.Name = "grdColQuantity";
            this.grdColQuantity.Visible = true;
            this.grdColQuantity.VisibleIndex = 6;
            // 
            // grdColPrice
            // 
            this.grdColPrice.Caption = "Price";
            this.grdColPrice.FieldName = "Price";
            this.grdColPrice.Name = "grdColPrice";
            this.grdColPrice.Visible = true;
            this.grdColPrice.VisibleIndex = 7;
            // 
            // grdColUnitSellingPrice
            // 
            this.grdColUnitSellingPrice.Caption = "Unit Selling Price";
            this.grdColUnitSellingPrice.FieldName = "UnitSellingPrice";
            this.grdColUnitSellingPrice.Name = "grdColUnitSellingPrice";
            this.grdColUnitSellingPrice.Visible = true;
            this.grdColUnitSellingPrice.VisibleIndex = 8;
            // 
            // lblApproveReceiptMessage
            // 
            this.lblApproveReceiptMessage.Location = new System.Drawing.Point(12, 12);
            this.lblApproveReceiptMessage.Name = "lblApproveReceiptMessage";
            this.lblApproveReceiptMessage.Size = new System.Drawing.Size(723, 13);
            this.lblApproveReceiptMessage.StyleController = this.layoutControl1;
            this.lblApproveReceiptMessage.TabIndex = 4;
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
            this.layoutControlItem3,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.OptionsCustomization.AllowDrag = DevExpress.XtraLayout.ItemDragDropMode.Disable;
            this.layoutControlGroup1.ShowInCustomizationForm = false;
            this.layoutControlGroup1.Size = new System.Drawing.Size(747, 434);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 388);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(457, 26);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lblApproveReceiptMessage;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(727, 17);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.grdReceipts;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 17);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(727, 371);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnClose;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(592, 388);
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
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnApproveSelected;
            this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            this.layoutControlItem5.Location = new System.Drawing.Point(457, 388);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(135, 26);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(135, 26);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(135, 26);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.Text = "layoutControlItem5";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextToControlDistance = 0;
            this.layoutControlItem5.TextVisible = false;
            // 
            // frmApproveReceipt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 434);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmApproveReceipt";
            this.Text = "Approve Receipts";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdReceipts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewReceipts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkeRepositorySupplierId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraGrid.GridControl grdReceipts;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewReceipts;
        private DevExpress.XtraEditors.LabelControl lblApproveReceiptMessage;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SimpleButton btnApproveSelected;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraGrid.Columns.GridColumn grdColReceiptId;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSupplierId;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lkeRepositorySupplierId;
        private DevExpress.XtraGrid.Columns.GridColumn grdColReceiptDate;
        private DevExpress.XtraGrid.Columns.GridColumn grdColStoreId;
        private DevExpress.XtraGrid.Columns.GridColumn grdColReceivedBy;
        private DevExpress.XtraGrid.Columns.GridColumn grdColItemId;
        private DevExpress.XtraGrid.Columns.GridColumn grdColQuantity;
        private DevExpress.XtraGrid.Columns.GridColumn grdColPrice;
        private DevExpress.XtraGrid.Columns.GridColumn grdColUnitSellingPrice;
    }
}