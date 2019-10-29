using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaDL
{
    public partial class Item
    {
        public long Available
        {
            get
            {
                try
                {
                    var _receiptQty = (from r in ReceivedItems where !(r.isDeleted ?? false) select r.quantity).Sum();

                    var _issuedQty = (from i in IssuedItems where !(i.isDeleted ?? false) select i.quantity).Sum();

                    var _soldQty = (from s in SalesDetails where !(s.isDeleted ?? false) select s.quantity).Sum();

                    var _retQty = (from r in ReturnedItems where !(r.isDeleted ?? false) select r.quantity).Sum();

                    var _lostList = (from la in LossAdjustments where !(la.isDeleted ?? false) select la).ToList();
                    var _lostQty = 0;
                    foreach (var lost in _lostList)
                    {
                        if (lost.isLoss)
                            _lostQty -= lost.quantity;
                        else
                            _lostQty += lost.quantity;
                    }

                    var _avail = _receiptQty - _issuedQty - _soldQty + _retQty + _lostQty;
                    return _avail;
                }
                catch (Exception ex)
                {
                    return 0L;
                }
            }
        }
    }
}
