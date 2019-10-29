using System;
using System.Collections;
using System.Collections.Generic;
using GimjaBL;

namespace Gimja
{
    public partial class rptRequestIssue : DevExpress.XtraReports.UI.XtraReport
    {
        public rptRequestIssue()
        {
            InitializeComponent();

            PopulateCompanyInfo();

            LoadReportDetail();
        }

        private void PopulateCompanyInfo()
        {
            //sets company info on the report heading
            var _companyInfo = new CompanyProfileBL().GetData();

            if (_companyInfo != null)
            {
                foreach (var _ci in _companyInfo)
                {
                    lblAmharicName.Text = _ci.AmharicName;
                    lblEnglishName.Text = _ci.EnglishName;
                    lblAddress.Text = new CompanyProfileBL().GetCompanyAddress();
                }
            }

            lblCurrentDate.Text = lblRequestedDate.Text = DateTime.Now.ToString("D");
            lblProcessedBy.DataBindings.Add("Text",this.DataSource,"ProcessedBy");
        }

        private void LoadReportDetail()
        {
            lblItemID.DataBindings.Add("Text", this.DataSource, "ItemID");
            lblBrand.DataBindings.Add("Text", this.DataSource, "Brand");
            lblCategory.DataBindings.Add("Text", this.DataSource, "Category");
            lblOrigin.DataBindings.Add("Text", this.DataSource, "Origin");
            lblAvailable.DataBindings.Add("Text", this.DataSource, "Available");
        }

        private void lblCurrentDate_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //SplashScreenManager.ShowForm(typeof (WaitForm1)); 
        }
    }
}
