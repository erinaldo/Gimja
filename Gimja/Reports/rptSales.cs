using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Linq;
using GimjaBL;

namespace Gimja.Reports
{
    public partial class rptSales : DevExpress.XtraReports.UI.XtraReport
    {
        public rptSales()
        {
            InitializeComponent();

            PrintCompanyInfo();
            PopulateSales();
        }

        private void PopulateSales()
        {
            var sales = GimjaBL.SalesBL.GetSales();
            this.DataSource = sales;

            lblBranch.DataBindings.Add("Text", this.DataSource, "BranchID");
            lblSalesDate.DataBindings.Add("Text", this.DataSource, "SalesDate");
            lblCustomer.DataBindings.Add("Text", this.DataSource, "CumstomerID");
            lblReferenceNo.DataBindings.Add("Text", this.DataSource, "ReferenceNo");
            lblReceipt.DataBindings.Add("Text", this.DataSource, "ReceiptID");
            lblProcessedBy.DataBindings.Add("Text", this.DataSource, "ProcessedBy");
        }

        private void PrintCompanyInfo()
        {
            var _companies = new CompanyProfileBL().GetData();//.GetCompanyInfo();
            if (_companies != null && _companies.Count() > 0)
            {
                var _comp = _companies.First();
                var _companyInfo = string.Format("{0}({1})\nTIN: {2}\tVAT: {3}", _comp.AmharicName,
                    _comp.EnglishName, _comp.TINNumber, _comp.VATRegistrationNo);
                //show the company info
                lblCompanyInfo.Text = _companyInfo;
            }
        }

        private void rptSales_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var rptDetails = ((rptSaleDetails)((XRSubreport)sender).ReportSource);

            rptDetails.SaleID.Value = Convert.ToString(GetCurrentColumnValue("ID"));
        }

    }
}
