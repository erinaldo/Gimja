using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class SalesDetailBL
    {
        public static SalesDetailData CreateSalesDetailObject(SalesDetail source)
        {
            if (source == null)
                return null;
            var _target = new SalesDetailData()
            {
                SalesID = source.salesID,
                SalesDetailID = source.salesDetailID,
                ItemID = source.itemID,
                SalesFrom = source.salesFrom,
                Quantity = source.quantity,
                Discount = source.discount,
                CreatedBy = source.createdBy,
                CreatedDate = source.createdDate,
                LastUpdatedBy = source.lastUpdatedBy,
                LastUpdatedDate = source.lastUpdatedDate,
                IsDeleted = source.isDeleted,
                UnitPrice = source.unitPrice,

                TotalPrice = source.TotalPrice
            };
            return _target;
        }

        public static bool Insert(IList<SalesDetailData> details, IList<ItemRequestData> itemRequestList, bool isSync = false)
        {
            if (details == null || details.Count == 0)
                throw new InvalidOperationException("Invalid sales ID or invalid sales detail attempted to be inserted.");
            var hasNoSalesId = details.Any(d => d.SalesID == Guid.Empty);
            if (hasNoSalesId)
                throw new InvalidOperationException("A detail has no sales ID to which the detail will be saved.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var salesIdList = details.Select(d => d.SalesID).ToList();
                var _existingDetails = db.SalesDetails.Where(sd => salesIdList.Contains(sd.salesID)).ToList();
                foreach (var detail in details)
                {
                    var _itemId = detail.ItemID;
                    var _itemAlreadyUsed = _existingDetails.Any(d => d.itemID == _itemId);
                    if (_itemAlreadyUsed)
                        throw new InvalidOperationException("This item is already inserted for the same sales.");
                }
                Guid _currentSaleId = salesIdList[0];
                var _salesObj = db.Sales.Where(s => s.id == _currentSaleId).First();
                DateTime currentDateTime = DateTime.Now;
                //insert each of the details into the db
                foreach (var d in details)
                {
                    var _detailObj = CreateSalesDetailObject(d);
                    _detailObj.createdDate = currentDateTime;
                    if (_detailObj.salesFrom == 3)
                    {//TODO: ENSURE WAREHOUSE LOCATION HAS ID 3
                        var _req = itemRequestList.Where(r => r.itemID.Equals(d.ItemID)).SingleOrDefault();
                        if (_req != null)
                        {
                            ItemRequest _request = new ItemRequest
                            {
                                branchID = _salesObj.branchID,
                                itemID = _req.itemID,
                                referenceNo = _salesObj.referenceNo,
                                salesID = d.SalesID,
                                warehouseID = _req.warehouseId
                            };
                            //add the item request to database
                            db.ItemRequests.Add(_request);
                        }
                    }
                    db.SalesDetails.Add(_detailObj);
                }
                int rows = db.SaveChanges();
                if (rows > 0 && !isSync)
                {
                }
                return rows > 0;
            }
        }

        public static SalesDetail CreateSalesDetailObject(SalesDetailData d)
        {
            if (d == null)
                return null;

            var _detailObj = new SalesDetail()
            {
                itemID = d.ItemID,
                salesDetailID = d.SalesDetailID,
                salesID = d.SalesID,
                salesFrom = d.SalesFrom,
                quantity = d.Quantity,
                discount = d.Discount,
                createdBy = d.CreatedBy,
                createdDate = d.CreatedDate,
                lastUpdatedBy = d.LastUpdatedBy,
                lastUpdatedDate = d.LastUpdatedDate,
                isDeleted = d.IsDeleted,
                unitPrice = d.UnitPrice
            };
            return _detailObj;
        }

        public static SalesDetailData GetSalesDetail(Guid salesId, string itemId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _details = (from d in db.SalesDetails
                                where d.salesID == salesId && d.itemID.Equals(itemId) && !(d.isDeleted ?? false)
                                select d).SingleOrDefault();

                if (_details == null)
                    return null;
                else
                {
                    var _result = CreateSalesDetailObject(_details);
                    return _result;
                }
            }
        }
    }

    public class SalesDetailData
    {
        public Guid SalesID { get; set; }
        public Guid SalesDetailID { get; set; }
        public string ItemID { get; set; }
        public short SalesFrom { get; set; }
        public double UnitPrice { get; set; }
        public long Quantity { get; set; }
        public Nullable<double> Discount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public double TotalPrice { get; set; }

        public bool Validate()
        {
            bool _result = true;
            if (Quantity <= 0) _result = false;
            else if (string.IsNullOrWhiteSpace(ItemID)) _result = false;
            else if (SalesFrom == 0) _result = false;
            else if (Discount < 0) _result = false;
            return _result;
        }
    }
}
