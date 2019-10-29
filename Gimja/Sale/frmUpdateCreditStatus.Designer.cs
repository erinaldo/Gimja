namespace Gimja
{
    partial class frmUpdateCreditStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdateCreditStatus));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveClose = new DevExpress.XtraEditors.SimpleButton();
            this.grdCreditSalesStatus = new DevExpress.XtraGrid.GridControl();
            this.grdViewCreditSalesStatus = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColSalesRefNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCustomer = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lkeRepositoryCustomer = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColTotalPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColCreditStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryCreditStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.grdColAmountPaid = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.lblCreditDescription = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdCreditSalesStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewCreditSalesStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkeRepositoryCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryCreditStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
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
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnSaveClose);
            this.layoutControl1.Controls.Add(this.grdCreditSalesStatus);
            this.layoutControl1.Controls.Add(this.lblCreditDescription);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(588, 180, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(707, 466);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(368, 432);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(103, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(475, 432);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(107, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "&Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveClose
            // 
            this.btnSaveClose.Enabled = false;
            this.btnSaveClose.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveClose.Image")));
            this.btnSaveClose.Location = new System.Drawing.Point(586, 432);
            this.btnSaveClose.Name = "btnSaveClose";
            this.btnSaveClose.Size = new System.Drawing.Size(109, 22);
            this.btnSaveClose.StyleController = this.layoutControl1;
            this.btnSaveClose.TabIndex = 6;
            this.btnSaveClose.Text = "Save && &Close";
            this.btnSaveClose.Click += new System.EventHandler(this.btnSaveClose_Click);
            // 
            // grdCreditSalesStatus
            // 
            this.grdCreditSalesStatus.Location = new System.Drawing.Point(12, 29);
            this.grdCreditSalesStatus.MainView = this.grdViewCreditSalesStatus;
            this.grdCreditSalesStatus.Name = "grdCreditSalesStatus";
            this.grdCreditSalesStatus.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryCreditStatus,
            this.lkeRepositoryCustomer,
            this.repositoryItemComboBox1});
            this.grdCreditSalesStatus.Size = new System.Drawing.Size(683, 399);
            this.grdCreditSalesStatus.TabIndex = 5;
            this.grdCreditSalesStatus.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewCreditSalesStatus});
            // 
            // grdViewCreditSalesStatus
            // 
            this.grdViewCreditSalesStatus.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColSalesRefNo,
            this.grdColCustomer,
            this.grdColDate,
            this.grdColTotalPrice,
            this.grdColCreditStatus,
            this.grdColAmountPaid});
            this.grdViewCreditSalesStatus.GridControl = this.grdCreditSalesStatus;
            this.grdViewCreditSalesStatus.Name = "grdViewCreditSalesStatus";
            this.grdViewCreditSalesStatus.OptionsView.ShowAutoFilterRow = true;
            this.grdViewCreditSalesStatus.OptionsView.ShowGroupPanel = false;
            this.grdViewCreditSalesStatus.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.grdViewCreditSalesStatus_PopupMenuShowing);
            this.grdViewCreditSalesStatus.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.grdViewCreditSalesStatus_InitNewRow);
            this.grdViewCreditSalesStatus.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.grdViewCreditSalesStatus_CellValueChanged);
            this.grdViewCreditSalesStatus.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.grdViewCreditSalesStatus_ValidateRow);
            // 
            // grdColSalesRefNo
            // 
            this.grdColSalesRefNo.Caption = "Ref. No";
            this.grdColSalesRefNo.FieldName = "ReferenceNo";
            this.grdColSalesRefNo.Name = "grdColSalesRefNo";
            this.grdColSalesRefNo.OptionsColumn.AllowEdit = false;
            this.grdColSalesRefNo.OptionsColumn.ReadOnly = true;
            this.grdColSalesRefNo.Visible = true;
            this.grdColSalesRefNo.VisibleIndex = 0;
            this.grdColSalesRefNo.Width = 97;
            // 
            // grdColCustomer
            // 
            this.grdColCustomer.Caption = "Customer";
            this.grdColCustomer.ColumnEdit = this.lkeRepositoryCustomer;
            this.grdColCustomer.FieldName = "CustomerID";
            this.grdColCustomer.Name = "grdColCustomer";
            this.grdColCustomer.OptionsColumn.AllowEdit = false;
            this.grdColCustomer.OptionsColumn.AllowFocus = false;
            this.grdColCustomer.Visible = true;
            this.grdColCustomer.VisibleIndex = 1;
            this.grdColCustomer.Width = 141;
            // 
            // lkeRepositoryCustomer
            // 
            this.lkeRepositoryCustomer.AutoHeight = false;
            this.lkeRepositoryCustomer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lkeRepositoryCustomer.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FullName", "Name")});
            this.lkeRepositoryCustomer.DisplayMember = "FullName";
            this.lkeRepositoryCustomer.Name = "lkeRepositoryCustomer";
            this.lkeRepositoryCustomer.NullText = "";
            this.lkeRepositoryCustomer.ValueMember = "ID";
            // 
            // grdColDate
            // 
            this.grdColDate.Caption = "Date";
            this.grdColDate.DisplayFormat.FormatString = "d";
            this.grdColDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.grdColDate.FieldName = "Date";
            this.grdColDate.Name = "grdColDate";
            this.grdColDate.OptionsColumn.AllowEdit = false;
            this.grdColDate.OptionsColumn.AllowFocus = false;
            this.grdColDate.Visible = true;
            this.grdColDate.VisibleIndex = 2;
            this.grdColDate.Width = 83;
            // 
            // grdColTotalPrice
            // 
            this.grdColTotalPrice.Caption = "Receivable";
            this.grdColTotalPrice.FieldName = "Receivable";
            this.grdColTotalPrice.Name = "grdColTotalPrice";
            this.grdColTotalPrice.OptionsColumn.AllowEdit = false;
            this.grdColTotalPrice.OptionsColumn.AllowFocus = false;
            this.grdColTotalPrice.Visible = true;
            this.grdColTotalPrice.VisibleIndex = 3;
            this.grdColTotalPrice.Width = 127;
            // 
            // grdColCreditStatus
            // 
            this.grdColCreditStatus.Caption = "Credit Status";
            this.grdColCreditStatus.ColumnEdit = this.repositoryCreditStatus;
            this.grdColCreditStatus.FieldName = "CreditStatusID";
            this.grdColCreditStatus.Name = "grdColCreditStatus";
            this.grdColCreditStatus.OptionsColumn.AllowEdit = false;
            this.grdColCreditStatus.Visible = true;
            this.grdColCreditStatus.VisibleIndex = 5;
            this.grdColCreditStatus.Width = 91;
            // 
            // repositoryCreditStatus
            // 
            this.repositoryCreditStatus.AutoHeight = false;
            this.repositoryCreditStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryCreditStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Credit Status"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsActive", "Is Active", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default)});
            this.repositoryCreditStatus.DisplayMember = "Name";
            this.repositoryCreditStatus.Name = "repositoryCreditStatus";
            this.repositoryCreditStatus.NullText = "";
            this.repositoryCreditStatus.ValueMember = "ID";
            // 
            // grdColAmountPaid
            // 
            this.grdColAmountPaid.Caption = "Amount Paid";
            this.grdColAmountPaid.FieldName = "AmountPaid";
            this.grdColAmountPaid.Name = "grdColAmountPaid";
            this.grdColAmountPaid.Visible = true;
            this.grdColAmountPaid.VisibleIndex = 4;
            this.grdColAmountPaid.Width = 126;
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            // 
            // lblCreditDescription
            // 
            this.lblCreditDescription.Location = new System.Drawing.Point(12, 12);
            this.lblCreditDescription.Name = "lblCreditDescription";
            this.lblCreditDescription.Size = new System.Drawing.Size(338, 13);
            this.lblCreditDescription.StyleController = this.layoutControl1;
            this.lblCreditDescription.TabIndex = 4;
            this.lblCreditDescription.Text = "Select the appropriate credit sales record to update/change its status.";
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
            this.layoutControlItem4,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(707, 466);
            this.layoutControlGroup1.Text = "Root";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 420);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(356, 26);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lblCreditDescription;
            this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(687, 17);
            this.layoutControlItem1.Text = "layoutControlItem1";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.grdCreditSalesStatus;
            this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 17);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(687, 403);
            this.layoutControlItem2.Text = "layoutControlItem2";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextToControlDistance = 0;
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSaveClose;
            this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
            this.layoutControlItem3.Location = new System.Drawing.Point(574, 420);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(113, 26);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(113, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(113, 26);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "layoutControlItem3";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextToControlDistance = 0;
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnSave;
            this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
            this.layoutControlItem4.Location = new System.Drawing.Point(463, 420);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(111, 26);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(111, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(111, 26);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.Text = "layoutControlItem4";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextToControlDistance = 0;
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnRefresh;
            this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
            this.layoutControlItem5.Location = new System.Drawing.Point(356, 420);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(107, 26);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(107, 26);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(107, 26);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.Text = "layoutControlItem5";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextToControlDistance = 0;
            this.layoutControlItem5.TextVisible = false;
            // 
            // frmUpdateCreditStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 466);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmUpdateCreditStatus";
            this.Text = "Update Credit Status";
            this.Load += new System.EventHandler(this.frmUpdateCreditStatus_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdCreditSalesStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdViewCreditSalesStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lkeRepositoryCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryCreditStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
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
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.LabelControl lblCreditDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl grdCreditSalesStatus;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewCreditSalesStatus;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSalesRefNo;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCustomer;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDate;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCreditStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repositoryCreditStatus;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnSaveClose;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraGrid.Columns.GridColumn grdColTotalPrice;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lkeRepositoryCustomer;
        private DevExpress.XtraGrid.Columns.GridColumn grdColAmountPaid;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
    }
}