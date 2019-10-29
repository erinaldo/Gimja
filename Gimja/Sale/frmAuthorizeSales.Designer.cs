namespace Gimja
{
    partial class frmAuthorizeSales
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAuthorizeSales));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnAuthorize = new DevExpress.XtraEditors.SimpleButton();
            this.grdSales = new DevExpress.XtraGrid.GridControl();
            this.grdViewSales = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColSelect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColReferenceNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCustomer = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColSalesDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCashier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositorychkSelect = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.grdColSalesId = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositorychkSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.btnAuthorize);
            this.layoutControl1.Controls.Add(this.grdSales);
            this.layoutControl1.Controls.Add(this.labelControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(430, 232, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(752, 462);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnClose
            // 
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(639, 428);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAuthorize
            // 
            this.btnAuthorize.Image = ((System.Drawing.Image)(resources.GetObject("btnAuthorize.Image")));
            this.btnAuthorize.Location = new System.Drawing.Point(534, 428);
            this.btnAuthorize.Name = "btnAuthorize";
            this.btnAuthorize.Size = new System.Drawing.Size(101, 22);
            this.btnAuthorize.StyleController = this.layoutControl1;
            this.btnAuthorize.TabIndex = 6;
            this.btnAuthorize.Text = "Authorize";
            this.btnAuthorize.Click += new System.EventHandler(this.btnAuthorize_Click);
            // 
            // grdSales
            // 
            this.grdSales.Location = new System.Drawing.Point(12, 29);
            this.grdSales.MainView = this.grdViewSales;
            this.grdSales.Name = "grdSales";
            this.grdSales.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositorychkSelect});
            this.grdSales.Size = new System.Drawing.Size(728, 395);
            this.grdSales.TabIndex = 5;
            this.grdSales.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewSales});
            // 
            // grdViewSales
            // 
            this.grdViewSales.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColSelect,
            this.grdColReferenceNo,
            this.grdColCustomer,
            this.grdColSalesDate,
            this.grdColCashier,
            this.grdColSalesId});
            this.grdViewSales.GridControl = this.grdSales;
            this.grdViewSales.Name = "grdViewSales";
            this.grdViewSales.OptionsView.ShowGroupPanel = false;
            // 
            // grdColSelect
            // 
            this.grdColSelect.Caption = "Select";
            this.grdColSelect.FieldName = "Selected";
            this.grdColSelect.Name = "grdColSelect";
            this.grdColSelect.Visible = true;
            this.grdColSelect.VisibleIndex = 0;
            this.grdColSelect.Width = 65;
            // 
            // grdColReferenceNo
            // 
            this.grdColReferenceNo.Caption = "Reference No.";
            this.grdColReferenceNo.FieldName = "ReferenceNo";
            this.grdColReferenceNo.Name = "grdColReferenceNo";
            this.grdColReferenceNo.OptionsColumn.ReadOnly = true;
            this.grdColReferenceNo.Visible = true;
            this.grdColReferenceNo.VisibleIndex = 1;
            this.grdColReferenceNo.Width = 161;
            // 
            // grdColCustomer
            // 
            this.grdColCustomer.Caption = "Customer";
            this.grdColCustomer.FieldName = "Customer";
            this.grdColCustomer.Name = "grdColCustomer";
            this.grdColCustomer.OptionsColumn.ReadOnly = true;
            this.grdColCustomer.Visible = true;
            this.grdColCustomer.VisibleIndex = 2;
            this.grdColCustomer.Width = 161;
            // 
            // grdColSalesDate
            // 
            this.grdColSalesDate.Caption = "Date";
            this.grdColSalesDate.FieldName = "SalesDate";
            this.grdColSalesDate.Name = "grdColSalesDate";
            this.grdColSalesDate.OptionsColumn.AllowEdit = false;
            this.grdColSalesDate.OptionsColumn.ReadOnly = true;
            this.grdColSalesDate.Visible = true;
            this.grdColSalesDate.VisibleIndex = 3;
            this.grdColSalesDate.Width = 161;
            // 
            // grdColCashier
            // 
            this.grdColCashier.Caption = "Cashier";
            this.grdColCashier.FieldName = "Cashier";
            this.grdColCashier.Name = "grdColCashier";
            this.grdColCashier.OptionsColumn.ReadOnly = true;
            this.grdColCashier.Visible = true;
            this.grdColCashier.VisibleIndex = 4;
            this.grdColCashier.Width = 162;
            // 
            // repositorychkSelect
            // 
            this.repositorychkSelect.AutoHeight = false;
            this.repositorychkSelect.Caption = "Check";
            this.repositorychkSelect.Name = "repositorychkSelect";
            this.repositorychkSelect.PictureChecked = ((System.Drawing.Image)(resources.GetObject("repositorychkSelect.PictureChecked")));
            this.repositorychkSelect.PictureUnchecked = ((System.Drawing.Image)(resources.GetObject("repositorychkSelect.PictureUnchecked")));
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(286, 13);
            this.labelControl1.StyleController = this.layoutControl1;
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "Select the sales records to authorize and choose Authorize.";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem2,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(752, 462);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 416);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(416, 26);
            this.emptySpaceItem2.Text = "emptySpaceItem2";
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.labelControl1;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(732, 17);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.grdSales;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 17);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(732, 399);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnAuthorize;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(522, 416);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(105, 26);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(105, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(105, 26);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "layoutControlItem3";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnClose;
            this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
            this.layoutControlItem4.Location = new System.Drawing.Point(627, 416);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(105, 26);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(105, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(105, 26);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.Text = "layoutControlItem4";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextToControlDistance = 0;
            this.layoutControlItem4.TextVisible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(428, 428);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(102, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnRefresh;
            this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            this.layoutControlItem5.Location = new System.Drawing.Point(416, 416);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(106, 26);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(106, 26);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(106, 26);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.Text = "layoutControlItem5";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextToControlDistance = 0;
            this.layoutControlItem5.TextVisible = false;
            // 
            // grdColSalesId
            // 
            this.grdColSalesId.Caption = "ID";
            this.grdColSalesId.FieldName = "SalesID";
            this.grdColSalesId.Name = "grdColSalesId";
            this.grdColSalesId.OptionsColumn.ReadOnly = true;
            // 
            // frmAuthorizeSales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 462);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmAuthorizeSales";
            this.Text = "Authorize Sales";
            this.Load += new System.EventHandler(this.frmAuthorizeSales_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositorychkSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl grdSales;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewSales;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnAuthorize;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSelect;
        private DevExpress.XtraGrid.Columns.GridColumn grdColReferenceNo;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCustomer;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSalesDate;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCashier;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositorychkSelect;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSalesId;
    }
}