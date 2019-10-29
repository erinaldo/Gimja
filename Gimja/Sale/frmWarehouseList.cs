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
    public partial class frmWarehouseList : DevExpress.XtraEditors.XtraForm
    {
        private string _warehouseId;
        public frmWarehouseList()
        {
            InitializeComponent();
        }

        private void frmWarehouseList_Load(object sender, EventArgs e)
        {
            PopulateWarehouses();
        }

        private void PopulateWarehouses()
        {
            WarehouseBL _warehouseBL = new WarehouseBL();
            var _warehouses = _warehouseBL.GetData(true);
            lkeWarehouses.Properties.DataSource = _warehouses;
        }

        public string SelectWarehouse(IWin32Window parent)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            DialogResult _result = this.ShowDialog(parent);
            if (_result == System.Windows.Forms.DialogResult.OK)
                return _warehouseId;
            else
                return null;
        }

        private void lkeWarehouses_EditValueChanged(object sender, EventArgs e)
        {
            _warehouseId = lkeWarehouses.EditValue.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _warehouseId = lkeWarehouses.EditValue.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _warehouseId = null;
        }
    }
}