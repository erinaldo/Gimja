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

namespace Gimja
{
    public partial class frmPurchaseOrderOrigin : DevExpress.XtraEditors.XtraForm
    {
        public frmPurchaseOrderOrigin()
        {
            InitializeComponent();
        }

        public string Origin
        {
            get;
            private set;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOrigin.Text))
            {
                Origin = string.Empty;
                //return;
            }
            else
            {
                Origin = txtOrigin.Text.Trim();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Origin = string.Empty;
        }

        private void txtOrigin_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOrigin.Text))
            {
                GimjaHelper.ShowError("The origin is required.");
                e.Cancel = true;
            }
        }
    }
}