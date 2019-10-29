using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using DevExpress.XtraReports.UI;
using GimjaBL;
using System.Collections.Generic;

namespace Gimja
{
    public partial class rptAttachment : DevExpress.XtraReports.UI.XtraReport
    {
        decimal _taxable = 0, _subTotal = 0;
        public rptAttachment()
        {
            InitializeComponent();
        }

        public rptAttachment(SalesData sales)
        {
            InitializeComponent();
            //get the sales object
            //var _sales = SalesBL.GetSales(salesId);
            if (sales != null)
            {
                //get the customer info
                CustomerData _customer = new CustomerBL().GetCustomer(sales.CustomerID);
                if (_customer != null)
                {
                    lblCustomerName.Text = _customer.FullName;
                    lblCustomerTIN.Text = _customer.TINNo;
                    lblCustomerVATRegNo.Text = _customer.VATRegistrationNo;
                    if (_customer.Address != null)
                    {
                        lblCustomerSubcity.Text = _customer.Address.Subcity;
                        lblCustomerKebele.Text = _customer.Address.Kebele;
                        lblCustomerCity.Text = _customer.Address.City_Town;
                    }
                }
                //get the company info
                var _companies = new CompanyProfileBL().GetData();
                if (_companies != null)
                {
                    var _company = _companies.First();
                    lblCompanyNameAmharic.Text = _company.AmharicName;
                    lblCompanyNameEnglish.Text = _company.EnglishName;
                    lblTIN.Text = _company.TINNumber;
                    lblVATRegNo.Text = _company.VATRegistrationNo;
                    lblRegDate.Text = Convert.ToString(_company.VATRegistrationDate);
                    if (_company.Address != null)
                    {
                        lblSubcity.Text = _company.Address.Subcity;
                        lblKeble.Text = _company.Address.Kebele;
                        lblHouseNo.Text = _company.Address.HouseNo;
                        lblEmail.Text = _company.Address.PrimaryEmail;
                    }
                    if (_company.TelephoneFax != null)
                    {
                        //TODO: choose a telephone/fax number to show
                    }

                }
                //show the sales info
                lblDate.Text = sales.SalesDate.ToShortDateString();
                lblFSNo.Text = sales.FSNo;
                lblReference.Text = sales.Reference;
                lblRefNo.Text = sales.RefNo;
                lblRefNote.Text = sales.RefNote;
                lblPaymentMethod.Text = sales.IsSalesCredit ? "Credit" : "Cash";

                lblPreparedBy.Text = sales.ProcessedBy;
                lblAuthorizedBy.Text = sales.AuthorizedBy;
                var _details = SalesBL.GetSaleDetails(sales.ID);
                if (_details != null)
                {
                    var _source = new List<SalesReportData>();
                    var _units = new UnitBL().GetData(true);
                    var _items = new ItemBL().GetData(true);
                    int _counter = 1;
                    foreach (var _d in _details)
                    {
                        var _item = _items.Where(i => i.ItemID.Equals(_d.ItemID)).SingleOrDefault(); //ItemBL.GetItem(_d.ItemID);
                        var _x = new SalesReportData()
                        {
                            SerialNo = _counter++,
                            ItemID = _d.ItemID,
                            Discount = _d.Discount ?? 0,
                            Quantity = _d.Quantity,
                            UnitPrice = _d.UnitPrice,
                            TotalPrice = _d.TotalPrice
                        };
                        if (_item != null)
                        {
                            _x.Description = _item.Description;
                            var _u = _units.FirstOrDefault(u => u.UnitID == _item.UnitID);
                            _x.Unit = (_u != null) ? _u.UnitName : string.Empty;
                        }
                        //add to the list
                        _source.Add(_x);
                    }

                    this.DataSource = _source;
                    tblCellNo.DataBindings.Add("Text", this.DataSource, "SerialNo");
                    tblCellCode.DataBindings.Add("Text", this.DataSource, "ItemID");
                    tblCellDescription.DataBindings.Add("Text", this.DataSource, "Description");
                    tblCellUnit.DataBindings.Add("Text", this.DataSource, "Unit");
                    tblCellDiscount.DataBindings.Add("Text", this.DataSource, "Discount");
                    tblCellQuantity.DataBindings.Add("Text", this.DataSource, "Quantity");
                    tblCellUnitPrice.DataBindings.Add("Text", this.DataSource, "UnitPrice");
                    tblCellTotal.DataBindings.Add("Text", this.DataSource, "TotalPrice");
                }
            }
        }
        class SalesReportData
        {
            public int SerialNo { get; set; }
            public string ItemID { get; set; }
            public string Description { get; set; }
            public string Unit { get; set; }
            public long Quantity { get; set; }
            public double Discount { get; set; }
            public double UnitPrice { get; set; }
            public double TotalPrice { get; set; }
        }
        private void xrTable3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var tbl = (XRTable)sender;
            if (tbl != null)
            {
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    string itemId = tbl.Rows[i].Cells["tblCellCode"].Text;
                    decimal _totalPrice;
                    bool _isValidPrice = decimal.TryParse(tbl.Rows[i].Cells["tblCellTotal"].Text, out _totalPrice);
                    if (_isValidPrice)
                    {
                        _subTotal += _totalPrice;
                        var _item = ItemBL.GetItem(itemId);
                        if (_item != null && (!_item.IsTaxExempted ?? false))
                        {
                            _taxable += _totalPrice;
                        }
                    }
                }
                tblCellSubTotal.Text = _subTotal.ToString("F2");
                //get the vat rate
                decimal _tax = 0m;
                var _taxType = TaxTypeBL.GetTaxType("VAT");
                if (_taxType != null)
                    _tax = _taxable * (decimal)_taxType.Rate;
                tblCellTotalTax.Text = _tax.ToString("F2");
                //get the total payment
                var _total = _subTotal + _tax;
                tblCellGrandTotal.Text = _total.ToString("F2");
                lblAmountInWords.Text = NumberToText.Convert(_total);
                //decimal _total;
                //bool _validTotal = decimal.TryParse(tblCellGrandTotal.Text, out _total);
                //if (_validTotal)
                //    lblAmountInWords.Text = NumberToText.Convert(_total);
                //else
                //    lblAmountInWords.Text = string.Empty;
            }
        }
    }
}
