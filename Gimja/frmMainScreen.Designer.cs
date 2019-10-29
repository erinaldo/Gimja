namespace Gimja
{
    partial class frmMainScreen
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainScreen));
            this.ribbonMain = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barCurrentDate = new DevExpress.XtraBars.BarStaticItem();
            this.barCurrentTime = new DevExpress.XtraBars.BarStaticItem();
            this.barLoggedInUser = new DevExpress.XtraBars.BarStaticItem();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.navBarMain = new DevExpress.XtraNavBar.NavBarControl();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarMain)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonMain
            // 
            this.ribbonMain.ApplicationIcon = global::Gimja.Properties.Resources.bearing1;
            this.ribbonMain.ExpandCollapseItem.Id = 0;
            this.ribbonMain.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonMain.ExpandCollapseItem,
            this.barCurrentDate,
            this.barCurrentTime,
            this.barLoggedInUser});
            this.ribbonMain.Location = new System.Drawing.Point(0, 0);
            this.ribbonMain.MaxItemId = 7;
            this.ribbonMain.Name = "ribbonMain";
            this.ribbonMain.Size = new System.Drawing.Size(957, 49);
            this.ribbonMain.StatusBar = this.ribbonStatusBar;
            this.ribbonMain.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ribbonMain_ItemClick);
            // 
            // barCurrentDate
            // 
            this.barCurrentDate.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.barCurrentDate.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            this.barCurrentDate.Id = 4;
            this.barCurrentDate.Name = "barCurrentDate";
            this.barCurrentDate.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barCurrentTime
            // 
            this.barCurrentTime.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.barCurrentTime.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            this.barCurrentTime.Id = 5;
            this.barCurrentTime.Name = "barCurrentTime";
            this.barCurrentTime.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barLoggedInUser
            // 
            this.barLoggedInUser.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
            this.barLoggedInUser.Id = 6;
            this.barLoggedInUser.Name = "barLoggedInUser";
            this.barLoggedInUser.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.ItemLinks.Add(this.barLoggedInUser);
            this.ribbonStatusBar.ItemLinks.Add(this.barCurrentDate);
            this.ribbonStatusBar.ItemLinks.Add(this.barCurrentTime);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 500);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbonMain;
            this.ribbonStatusBar.Size = new System.Drawing.Size(957, 31);
            // 
            // navBarMain
            // 
            this.navBarMain.ActiveGroup = null;
            this.navBarMain.Dock = System.Windows.Forms.DockStyle.Left;
            this.navBarMain.Location = new System.Drawing.Point(0, 49);
            this.navBarMain.Name = "navBarMain";
            this.navBarMain.OptionsNavPane.ExpandedWidth = 199;
            this.navBarMain.Size = new System.Drawing.Size(199, 451);
            this.navBarMain.TabIndex = 2;
            this.navBarMain.Text = "navBarControl1";
            this.navBarMain.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(this.navBarMain_LinkClicked);
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(199, 49);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(5, 451);
            this.splitterControl1.TabIndex = 3;
            this.splitterControl1.TabStop = false;
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "ribbonPage1";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // frmMainScreen
            // 
            this.AllowFormGlass = DevExpress.Utils.DefaultBoolean.True;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 531);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.navBarMain);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbonMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "frmMainScreen";
            this.Ribbon = this.ribbonMain;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "Gimja";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMainScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonMain;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraNavBar.NavBarControl navBarMain;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.BarStaticItem barCurrentDate;
        private DevExpress.XtraBars.BarStaticItem barCurrentTime;
        private DevExpress.XtraBars.BarStaticItem barLoggedInUser;
        private System.Windows.Forms.Timer timer;
    }
}