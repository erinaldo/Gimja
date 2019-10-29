using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class PurchaseOrderBL
    {

        public static IList<PurchaseOrderData> GetPurchaseOrders()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                try
                {
                    var _result = (from po in db.PurchaseOrders
                                   where !(po.isDeleted ?? false)
                                   select po).ToList();
                    List<PurchaseOrderData> _retValue = new List<PurchaseOrderData>();
                    _result.ForEach(r => _retValue.Add(CreatePurchaseOrderData(r)));

                    return _retValue;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
        }

        private static PurchaseOrderData CreatePurchaseOrderData(PurchaseOrder po)
        {
            return new PurchaseOrderData()
            {
                ID = po.id,
                ProcessedBy = po.processedBy,
                ApprovedBy = po.approvedBy,
                Date = po.date,
                SupplierID = po.supplierID,
                CreatedBy = po.createdBy,
                CreatedDate = po.createdDate,
                LastUpdatedBy = po.lastUpdatedBy,
                LastUpdatedDate = po.lastUpdatedDate,
                IsDeleted = po.isDeleted
            };
        }
        private static PurchaseOrder CreatePurchaseOrderData(PurchaseOrderData poData)
        {
            return new PurchaseOrder()
            {
                supplierID = poData.SupplierID,
                processedBy = poData.ProcessedBy,
                date = poData.Date,
                approvedBy = poData.ApprovedBy,
                createdBy = poData.CreatedBy,
                createdDate = poData.CreatedDate,
                id = poData.ID,
                lastUpdatedBy = poData.LastUpdatedBy,
                lastUpdatedDate = poData.LastUpdatedDate,
                isDeleted = poData.IsDeleted
            };
        }

        public static IList<PurchaseOrderDetailData> GetPurchaseOrderDetails(Guid purchaseOrderId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _details = (from pod in db.PurchaseOrderDetails
                                where !(pod.isDeleted ?? false) && pod.purchaseOrderID == purchaseOrderId
                                select new PurchaseOrderDetailData()
                                {
                                    PurchaseOrderID = pod.purchaseOrderID,
                                    PurchaseOrderDetailID = pod.purchaseOrderDetailID,
                                    ItemID = pod.itemID,
                                    Origin = pod.origin,
                                    Quantity = pod.quantity,
                                    UnitPrice = pod.unitPrice,
                                    ManufacturerID = pod.lkManufacturerID,
                                    Remark = pod.remark,
                                    CreatedBy = pod.createdBy,
                                    CreatedDate = pod.createdDate,
                                    LastUpdatedBy = pod.lastUpdatedBy,
                                    LastUpdatedDate = pod.lastUpdatedDate,
                                    IsDeleted = pod.isDeleted
                                }).ToList();

                return _details;
            }
        }

        public static bool Insert(PurchaseOrderData poData, IList<PurchaseOrderDetailData> poDetails)
        {
            if (poData == null || poDetails == null || poDetails.Count == 0)
            {
                throw new InvalidOperationException("Invalid purchase order data to be inserted.");
            }
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                try
                {
                    DateTime currentDateTime = DateTime.Now;
                    var _puchaseOrder = CreatePurchaseOrderData(poData);
                    //_puchaseOrder.createdDate = currentDateTime;

                    db.PurchaseOrders.Add(_puchaseOrder);
                    int rows = db.SaveChanges();
                    if (rows > 0)
                    {//the purchase order is inserted successfully
                        Guid _poId = _puchaseOrder.id;
                        List<PurchaseOrderDetail> _purchaseOrderDetails = (from d in poDetails
                                                                           select new PurchaseOrderDetail
                                                                           {
                                                                               purchaseOrderID = _poId,
                                                                               purchaseOrderDetailID = d.PurchaseOrderDetailID,
                                                                               itemID = d.ItemID,
                                                                               lkManufacturerID = d.ManufacturerID,
                                                                               origin = d.Origin,
                                                                               quantity = d.Quantity,
                                                                               unitPrice = d.UnitPrice,
                                                                               remark = d.Remark,
                                                                               createdBy = d.CreatedBy,
                                                                               createdDate = d.CreatedDate,
                                                                               lastUpdatedBy = d.LastUpdatedBy,
                                                                               lastUpdatedDate = d.LastUpdatedDate,
                                                                               isDeleted = d.IsDeleted
                                                                           }).ToList();

                        db.PurchaseOrderDetails.AddRange(_purchaseOrderDetails);
                        rows = db.SaveChanges();
                    }
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// Updates the given purchase order along with the purchase order details
        /// </summary>
        /// <param name="_po">the purchase order data</param>
        /// <param name="poDetails">the details of the specified purchase order</param>
        /// <returns>true if successful, false otherwise</returns>
        public static bool Update(PurchaseOrderData _po, IList<PurchaseOrderDetailData> poDetails)
        {
            if (_po == null || _po.ID == Guid.Empty || poDetails == null || poDetails.Count == 0)
            {
                throw new ArgumentException("Invalid purchase order data to update.");
            }
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _existingPO = db.PurchaseOrders.Include("PurchaseOrderDetails").Where(p => p.id == _po.ID).SingleOrDefault();
                if (_existingPO == null)
                {//the PO to edit is not found
                    throw new InvalidOperationException("The purchase order to edit is not found.");
                }
                _existingPO.supplierID = _po.SupplierID;
                _existingPO.processedBy = _po.ProcessedBy;
                _existingPO.date = _po.Date;
                _existingPO.lastUpdatedBy = _po.LastUpdatedBy;
                _existingPO.lastUpdatedDate = _po.LastUpdatedDate;
                //get the purchase order details
                var _purchaseOrderDetails = _existingPO.PurchaseOrderDetails.ToList();
                //new purchase order details
                var _existingDetails = _purchaseOrderDetails.Where(p => poDetails.Any(x => x.PurchaseOrderDetailID == p.purchaseOrderDetailID)).ToList();
                var _newDetails = poDetails.Where(p => !_purchaseOrderDetails.Any(x => x.purchaseOrderDetailID == p.PurchaseOrderDetailID)).ToList();
                foreach (var _modifiedDetail in _existingDetails)
                {
                    //get the existing details
                    var _pod = poDetails.Where(d => d.PurchaseOrderID == _existingPO.id && d.PurchaseOrderDetailID == _modifiedDetail.purchaseOrderDetailID).SingleOrDefault();
                    if (_pod != null)
                    {
                        _modifiedDetail.itemID = _pod.ItemID;
                        _modifiedDetail.lkManufacturerID = _pod.ManufacturerID;
                        _modifiedDetail.origin = _pod.Origin;
                        _modifiedDetail.quantity = _pod.Quantity;
                        _modifiedDetail.unitPrice = _pod.UnitPrice;
                        _modifiedDetail.lastUpdatedBy = _pod.LastUpdatedBy;
                        _modifiedDetail.lastUpdatedDate = _pod.LastUpdatedDate;
                    }
                }
                foreach (var _newDetail in _newDetails)
                {
                    var _pod = new PurchaseOrderDetail()
                    {
                        purchaseOrderID = _newDetail.PurchaseOrderID,
                        purchaseOrderDetailID = _newDetail.PurchaseOrderDetailID,
                        itemID = _newDetail.ItemID,
                        lkManufacturerID = _newDetail.ManufacturerID,
                        origin = _newDetail.Origin,
                        unitPrice = _newDetail.UnitPrice,
                        quantity = _newDetail.Quantity,
                        createdBy = _newDetail.CreatedBy,
                        createdDate = _newDetail.CreatedDate
                    };
                    db.PurchaseOrderDetails.Add(_pod);
                }

                int rows = db.SaveChanges();
                return rows > 0;
            }
        }
        /// <summary>
        /// Updates only the purchase order assuming no change made to the details
        /// </summary>
        /// <param name="_data">the modified purchase order data to be saved</param>
        /// <returns></returns>
        public static bool Update(PurchaseOrderData _data)
        {
            if (_data == null || _data.ID == Guid.Empty)
            {
                throw new ArgumentNullException("The given purchase order is invalid.");
            }
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //get the existing purchase order
                var _existingPO = db.PurchaseOrders.Where(x => x.id == _data.ID).SingleOrDefault();
                if (_existingPO == null)
                {
                    throw new InvalidOperationException("The specified purchase order is not found.");
                }
                _existingPO.approvedBy = _data.ApprovedBy;
                _existingPO.lastUpdatedBy = _data.LastUpdatedBy;
                _existingPO.lastUpdatedDate = _data.LastUpdatedDate;
                _existingPO.processedBy = _data.ProcessedBy;
                _existingPO.supplierID = _data.SupplierID;
                _existingPO.date = _data.Date;

                int rows = db.SaveChanges();
                return rows > 0;
            }
        }

        public static IList<PurchaseOrderForApproval> GetPurchaseOrdersToApprove()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                try
                {
                    var _result = (from po in db.PurchaseOrders.Include("PurchaseOrderDetails")
                                   where !(po.isDeleted ?? false) && string.IsNullOrEmpty(po.approvedBy)
                                   select po).ToList();

                    List<PurchaseOrderForApproval> _retValue = new List<PurchaseOrderForApproval>();
                    foreach (var _po in _result)
                    {
                        var _poData = new PurchaseOrderForApproval()
                        {
                            ID = _po.id,
                            ProcessedBy = _po.processedBy,
                            ApprovedBy = _po.approvedBy,
                            Date = _po.date,
                            SupplierID = _po.supplierID,
                            CreatedBy = _po.createdBy,
                            CreatedDate = _po.createdDate,
                            LastUpdatedBy = _po.lastUpdatedBy,
                            LastUpdatedDate = _po.lastUpdatedDate,
                            IsDeleted = _po.isDeleted,
                            ItemCount = _po.PurchaseOrderDetails.Count,
                            OrderQuantity = _po.PurchaseOrderDetails.Sum(x => x.quantity),
                            UnitPrice = _po.PurchaseOrderDetails.Sum(x => x.unitPrice)
                        };
                        _retValue.Add(_poData);
                    }
                    return _retValue;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Approves all purchase orders with the given IDs by the specified user
        /// </summary>
        /// <param name="_selectedOrders">the list of purchase order IDs</param>
        /// <param name="approvedBy">the user approving the purchase orders</param>
        /// <returns></returns>
        public static bool ApproveAll(IEnumerable<Guid> _selectedOrders, string approvedBy)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _orders = db.PurchaseOrders.Where(po => _selectedOrders.Contains(po.id)).ToList();
                if (_orders != null && _orders.Count > 0)
                {
                    DateTime _currentDateTime = DateTime.Now;
                    foreach (var _purchaseOrder in _orders)
                    {
                        _purchaseOrder.approvedBy = approvedBy;
                        _purchaseOrder.lastUpdatedBy = approvedBy;
                        _purchaseOrder.lastUpdatedDate = _currentDateTime;
                    }

                    int rows = db.SaveChanges();
                    return rows > 0;
                }
                return false;
            }
        }

        public static bool Delete(Guid poId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _existingPO = (from po in db.PurchaseOrders.Include("PurchaseOrderDetails")
                                   where po.id == poId
                                   select po).SingleOrDefault();
                if (_existingPO != null)
                {//the existing purchase order is found
                    _existingPO.isDeleted = true;
                    foreach (var _detail in _existingPO.PurchaseOrderDetails)
                        _detail.isDeleted = true;//db.PurchaseOrders.Remove(_existingPO);
                    //save the removal
                    int rows = db.SaveChanges();
                    return rows > 0;
                }
                return false;
            }
        }
    }

    public class PurchaseOrderData
    {
        public Guid ID { get; set; }
        public string ProcessedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime Date { get; set; }
        public string SupplierID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class PurchaseOrderDetailData
    {
        public Guid PurchaseOrderID { get; set; }
        public Guid PurchaseOrderDetailID { get; set; }
        public string ItemID { get; set; }
        public string Origin { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string ManufacturerID { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class PurchaseOrderForApproval
    {
        public Guid ID { get; set; }
        public string ProcessedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime Date { get; set; }
        public string SupplierID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        //more info
        public long OrderQuantity { get; set; }
        public double UnitPrice { get; set; }
        public int ItemCount { get; set; }
    }
}
