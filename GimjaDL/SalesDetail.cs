using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaDL
{
    public partial class SalesDetail
    {
        public double TotalPrice
        {
            get
            {
                var _saleItem = Item;
                if (_saleItem != null)
                {
                    var _totalPrice = quantity * _saleItem.unitPrice;
                    return _totalPrice;
                }
                else
                    return 0d;
            }
        }
    }
}
