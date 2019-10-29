using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class ItemRequestBL
    {
        public static bool Insert(ItemRequestData requestData)
        {
            if (requestData == null || (requestData.salesID == Guid.Empty || string.IsNullOrEmpty(requestData.branchID) ||
                string.IsNullOrEmpty(requestData.itemID) || string.IsNullOrEmpty(requestData.warehouseId)))
                throw new InvalidOperationException("Invalid request data to insert.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _request = new ItemRequest()
                {
                    salesID = requestData.salesID,
                    branchID = requestData.branchID,
                    referenceNo = requestData.referenceNo,
                    itemID = requestData.itemID,
                    warehouseID = requestData.warehouseId
                };
                var _existingRequest = db.ItemRequests.Where(ir => ir.salesID == _request.salesID && ir.itemID.Equals(_request.itemID)).ToList();
                if (_existingRequest != null && _existingRequest.Count > 0)
                    throw new InvalidOperationException("An item request already exists.");

                db.ItemRequests.Add(_request);
                int rows = db.SaveChanges();
                return rows > 0;
            }
        }

        public static bool Remove(ItemRequestData requestData)
        {
            if (requestData == null || (requestData.salesID == Guid.Empty || string.IsNullOrEmpty(requestData.itemID)))
                throw new InvalidOperationException("Invalid request data to insert.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _request = db.ItemRequests.Where(ir => ir.itemID.Equals(requestData.itemID) && ir.salesID == requestData.salesID).SingleOrDefault();
                if (_request == null)
                    throw new InvalidOperationException("The item request record to remove is not found.");

                db.ItemRequests.Remove(_request);
                int rows = db.SaveChanges();
                return rows > 0;
            }
        }
    }

    public class ItemRequestData
    {
        public System.Guid salesID { get; set; }
        public string branchID { get; set; }
        public string referenceNo { get; set; }
        public string itemID { get; set; }
        public string warehouseId { get; set; }
    }
}
