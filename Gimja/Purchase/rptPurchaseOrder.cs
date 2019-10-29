using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;
using System.Collections.Generic;

namespace Gimja
{
    public partial class rptPurchaseOrder : DevExpress.XtraReports.UI.XtraReport
    {
        private GimjaBL.PurchaseOrderData _currentOrder;
        decimal grandTotalPrice = 0;
        public rptPurchaseOrder()
        {
            InitializeComponent();
        }

        public rptPurchaseOrder(GimjaBL.PurchaseOrderData _currentOrder)
        {
            InitializeComponent();
            // TODO: Complete member initialization
            this._currentOrder = _currentOrder;
            if (_currentOrder != null)
            {//if the purchase order is not null
                //get the company info
                var _companies = new GimjaBL.CompanyProfileBL().GetData();
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
                //set the date, supplier
                lblDate.Text = _currentOrder.Date.ToShortDateString();
                lblSupplier.Text = _currentOrder.SupplierID;
                //get the manufacturer list
                var _manufacturers = GimjaBL.ManufacturerBL.GetActiveManufacturers();
                var _items = new GimjaBL.ItemBL().GetData(true);
                var _orderDetail = GimjaBL.PurchaseOrderBL.GetPurchaseOrderDetails(_currentOrder.ID);
                int counter = 1;
                var _source = new List<PurchaseOrderReportData>();
                foreach (var _detail in _orderDetail)
                {
                    var _item = _items.Where(i => i.ItemID.Equals(_detail.ItemID)).SingleOrDefault();
                    var _m = _manufacturers.Where(m => m.ManufacturerID.Equals(_detail.ManufacturerID)).SingleOrDefault();
                    var _orderReport = new PurchaseOrderReportData()
                    {
                        SerialNo = counter++,
                        Item = (_item != null) ? string.Format("{0} ({1})", _item.Description, _detail.ItemID) : _detail.ItemID,
                        Origin = _detail.Origin,
                        Manufacturer = (_m != null) ? _m.Name : _detail.ManufacturerID,
                        Quantity = _detail.Quantity,
                        UnitPrice = _detail.UnitPrice,
                        TotalPrice = (_detail.Quantity * _detail.UnitPrice)
                    };
                    _source.Add(_orderReport);
                }
                //set as datasource
                this.DataSource = _source;
                tblCellSerialNo.DataBindings.Add("Text", this.DataSource, "SerialNo");
                tblCellItem.DataBindings.Add("Text", this.DataSource, "Item");
                tblCellOrigin.DataBindings.Add("Text", this.DataSource, "Origin");
                tblCellManufacturer.DataBindings.Add("Text", this.DataSource, "Manufacturer");
                tblCellQuantity.DataBindings.Add("Text", this.DataSource, "Quantity");
                tblCellUnitPrice.DataBindings.Add("Text", this.DataSource, "UnitPrice");
                tblCellTotalPrice.DataBindings.Add("Text", this.DataSource, "TotalPrice");
            }
        }

        class PurchaseOrderReportData
        {
            public int SerialNo { get; set; }
            public string Item { get; set; }
            public string Origin { get; set; }
            public string Manufacturer { get; set; }
            public int Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double TotalPrice { get; set; }
        }

        private void xrTable2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var tbl = (XRTable)sender;
            if (tbl != null)
            {
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    decimal _totalPrice;
                    bool _isValidPrice = decimal.TryParse(tbl.Rows[i].Cells["tblCellTotalPrice"].Text, out _totalPrice);
                    if (_isValidPrice)
                    {
                        grandTotalPrice += _totalPrice;
                    }
                }

                lblGrandTotal.Text = grandTotalPrice.ToString("F2");
                lblGrandTotalText.Text = NumberToText.Convert(grandTotalPrice);
            }
        }
    }
}
