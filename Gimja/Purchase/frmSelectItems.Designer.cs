namespace Gimja
{
    partial class frmSelectItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectItems));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.grdSelectItems = new DevExpress.XtraGrid.GridControl();
            this.grdViewSelectItems = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColSelect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColUnitPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColOrderQuantity = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColReorderLevel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColAvailable = new DevExpress.XtraGrid.Columns.GridColumn();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSelectItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewSelectItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Controls.Add(this.btnOK);
            this.layoutControl1.Controls.Add(this.grdSelectItems);
            this.layoutControl1.Controls.Add(this.labelControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(342, 187, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(716, 383);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(571, 349);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(133, 22);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.Location = new System.Drawing.Point(434, 349);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(133, 22);
            this.btnOK.StyleController = this.layoutControl1;
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grdSelectItems
            // 
            this.grdSelectItems.Location = new System.Drawing.Point(12, 29);
            this.grdSelectItems.MainView = this.grdViewSelectItems;
            this.grdSelectItems.Name = "grdSelectItems";
            this.grdSelectItems.Size = new System.Drawing.Size(692, 316);
            this.grdSelectItems.TabIndex = 5;
            this.grdSelectItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewSelectItems});
            // 
            // grdViewSelectItems
            // 
            this.grdViewSelectItems.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColSelect,
            this.grdColItemId,
            this.grdColUnitPrice,
            this.grdColOrderQuantity,
            this.grdColReorderLevel,
            this.grdColAvailable});
            this.grdViewSelectItems.GridControl = this.grdSelectItems;
            this.grdViewSelectItems.Name = "grdViewSelectItems";
            this.grdViewSelectItems.OptionsSelection.MultiSelect = true;
            this.grdViewSelectItems.OptionsView.ShowAutoFilterRow = true;
            this.grdViewSelectItems.OptionsView.ShowGroupPanel = false;
            this.grdViewSelectItems.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.grdViewSelectItems_RowClick);
            this.grdViewSelectItems.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.grdViewSelectItems_RowCellClick);
            this.grdViewSelectItems.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.grdViewSelectItems_PopupMenuShowing);
            // 
            // grdColSelect
            // 
            this.grdColSelect.Caption = "Select";
            this.grdColSelect.FieldName = "Selected";
            this.grdColSelect.Name = "grdColSelect";
            this.grdColSelect.Visible = true;
            this.grdColSelect.VisibleIndex = 0;
            this.grdColSelect.Width = 52;
            // 
            // grdColItemId
            // 
            this.grdColItemId.Caption = "Item ID";
            this.grdColItemId.FieldName = "ItemId";
            this.grdColItemId.Name = "grdColItemId";
            this.grdColItemId.OptionsColumn.ReadOnly = true;
            this.grdColItemId.Visible = true;
            this.grdColItemId.VisibleIndex = 1;
            this.grdColItemId.Width = 192;
            // 
            // grdColUnitPrice
            // 
            this.grdColUnitPrice.Caption = "Unit Price";
            this.grdColUnitPrice.FieldName = "UnitPrice";
            this.grdColUnitPrice.Name = "grdColUnitPrice";
            this.grdColUnitPrice.OptionsColumn.ReadOnly = true;
            this.grdColUnitPrice.Visible = true;
            this.grdColUnitPrice.VisibleIndex = 2;
            this.grdColUnitPrice.Width = 103;
            // 
            // grdColOrderQuantity
            // 
            this.grdColOrderQuantity.Caption = "Order Quantity";
            this.grdColOrderQuantity.FieldName = "OrderQuantity";
            this.grdColOrderQuantity.Name = "grdColOrderQuantity";
            this.grdColOrderQuantity.OptionsColumn.ReadOnly = true;
            this.grdColOrderQuantity.Visible = true;
            this.grdColOrderQuantity.VisibleIndex = 3;
            this.grdColOrderQuantity.Width = 96;
            // 
            // grdColReorderLevel
            // 
            this.grdColReorderLevel.Caption = "Reorder Level";
            this.grdColReorderLevel.FieldName = "ReorderLevel";
            this.grdColReorderLevel.Name = "grdColReorderLevel";
            this.grdColReorderLevel.OptionsColumn.ReadOnly = true;
            this.grdColReorderLevel.Visible = true;
            this.grdColReorderLevel.VisibleIndex = 4;
            this.grdColReorderLevel.Width = 129;
            // 
            // grdColAvailable
            // 
            this.grdColAvailable.Caption = "Available";
            this.grdColAvailable.FieldName = "Available";
            this.grdColAvailable.Name = "grdColAvailable";
            this.grdColAvailable.OptionsColumn.ReadOnly = true;
            this.grdColAvailable.Visible = true;
            this.grdColAvailable.VisibleIndex = 5;
            this.grdColAvailable.Width = 102;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(455, 13);
            this.labelControl1.StyleController = this.layoutControl1;
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "These items need a new purchase order. Select the items to be included in the pur" +
    "chase order.";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(716, 383);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.labelControl1;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(696, 17);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.grdSelectItems;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 17);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(696, 320);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnOK;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(422, 337);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(137, 26);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(137, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(137, 26);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "layoutControlItem3";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnCancel;
            this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
            this.layoutControlItem4.Location = new System.Drawing.Point(559, 337);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(137, 26);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(137, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(137, 26);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.Text = "layoutControlItem4";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextToControlDistance = 0;
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 337);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(422, 26);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmSelectItems
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(716, 383);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectItems";
            this.Text = "Select Items";
            this.Load += new System.EventHandler(this.frmSelectItems_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdSelectItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewSelectItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.GridControl grdSelectItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewSelectItems;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSelect;
        private DevExpress.XtraGrid.Columns.GridColumn grdColItemId;
        private DevExpress.XtraGrid.Columns.GridColumn grdColUnitPrice;
        private DevExpress.XtraGrid.Columns.GridColumn grdColOrderQuantity;
        private DevExpress.XtraGrid.Columns.GridColumn grdColReorderLevel;
        private DevExpress.XtraGrid.Columns.GridColumn grdColAvailable;
    }
}