using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using GimjaBL;

namespace Gimja
{
    public partial class frmAuthorizeSales : DevExpress.XtraEditors.XtraForm
    {
        IList<AuthorizeSalesData> source;

        public frmAuthorizeSales()
        {
            InitializeComponent();
        }

        private void btnAuthorize_Click(object sender, EventArgs e)
        {
            int _rowCount = grdViewSales.RowCount;
            List<Guid> _selectedSaleIds = new List<Guid>();
            for (int i = 0; i < _rowCount; i++)
            {
                var _isSelected = Convert.ToBoolean(grdViewSales.GetRowCellValue(i, grdColSelect));
                if (_isSelected)
                {
                    var _saleId = new Guid(grdViewSales.GetRowCellValue(i, grdColSalesId).ToString());
                    if (_saleId != Guid.Empty)
                        _selectedSaleIds.Add(_saleId);
                }
            }
            if (_selectedSaleIds.Count > 0)
            {//there are selected records
                DialogResult _response = GimjaHelper.ShowQuestion(string.Format("Are you sure you want to authorize the selected {0} record{1}?", _selectedSaleIds.Count, (_selectedSaleIds.Count > 1) ? "s" : ""));
                if (_response == System.Windows.Forms.DialogResult.Yes)
                {
                    bool _retValue = SalesBL.AuthorizeSales(_selectedSaleIds, GimjaHelper.GetCurrentUserID(this)); //"LogonUser");//TODO: ADD THE LOGON USER HERE
                    if (_retValue)
                    {
                        UpdateSalesList();
                        GimjaHelper.ShowInformation(string.Format("{0} sales record{1} authorized successfully.", _selectedSaleIds.Count, _selectedSaleIds.Count > 1 ? "s are" : " is"));
                    }
                }
            }
        }

        private void frmAuthorizeSales_Load(object sender, EventArgs e)
        {
            UpdateSalesList();
        }

        private void UpdateSalesList()
        {
            source = SalesBL.GetSalesToAuthorize();

            grdSales.DataSource = source;
            if (source.Count == 0)
                btnAuthorize.Enabled = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateSalesList();
        }
    }
}