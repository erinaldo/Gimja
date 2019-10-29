using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class SaleReturnBL
    {
        public static IList<SaleReturnData> GetSaleReturns()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _returns = (from r in db.SalesReturns.Include("Sale")
                                where !(r.isDeleted ?? false)
                                select new SaleReturnData()
                                {
                                    ID = r.id,
                                    SalesID = r.salesID,
                                    Date = r.date,
                                    ProcessedBy = r.processedBy,
                                    IsDeleted = r.isDeleted,
                                    Reason = r.reason,
                                    ReferenceNo = r.Sale.referenceNo
                                }).ToList();

                return _returns;
            }
        }

        public static bool Insert(SaleReturnData returnData, IList<ReturnItemData> saleReturnsList, bool isSync = false)
        {
            if (returnData == null || saleReturnsList == null || (saleReturnsList != null && saleReturnsList.Count <= 0))
                throw new InvalidOperationException("Invalid return data, unable to save.");
            else
            {//both the return and its detail list are not null
                using (var db = new eDMSEntity("eDMSEntity"))
                {
                    DateTime _currentDateTime = DateTime.Now;
                    returnData.CreatedDate = _currentDateTime;
                    var _retObj = new SalesReturn()
                    {
                        id = Guid.NewGuid(),
                        salesID = returnData.SalesID,
                        date = returnData.Date,
                        processedBy = returnData.ProcessedBy,
                        reason = returnData.Reason,
                        createdBy = returnData.CreatedBy,
                        createdDate = returnData.CreatedDate
                    };
                    db.SalesReturns.Add(_retObj);
                    Guid _retDataId = _retObj.id;
                    if (_retDataId != Guid.Empty)
                    {//the id is generated appropriately
                        DateTime currentDateTime = DateTime.Now;
                        foreach (var item in saleReturnsList)
                        {
                            item.CreatedDate = _currentDateTime;
                            var _returnItem = new ReturnedItem()
                            {
                                returnedDetailID = Guid.NewGuid(),
                                itemID = item.ItemID,
                                salesReturnID = _retDataId,
                                quantity = item.Quantity,
                                refundedAmount = item.RefundedAmount,
                                createdBy = item.CreatedBy,
                                createdDate = item.CreatedDate,
                                isDeleted = item.IsDeleted
                            };
                            db.ReturnedItems.Add(_returnItem);
                        }
                    }
                    int rows = db.SaveChanges();
                    if (rows > 0 && !isSync)
                    {
                        var _retElem = SyncTransactionBL.GetSaleReturnElement(_retObj.id);
                        if (_retElem != null)
                        {
                            var _s = SalesBL.GetSales(_retObj.salesID);
                            if (_s != null)
                            {
                                var _syncVal = new SyncTransaction()
                                {
                                    id = Guid.NewGuid(),
                                    tableName = "tblSalesReturn",
                                    action = "insert",
                                    value = _retElem.ToString(),
                                    branchID = _s.BranchID,
                                    isDeleted = null
                                };
                                db.SyncTransactions.Add(_syncVal);
                                db.SaveChanges();
                            }
                        }
                    }
                    return (rows > 0);
                }
            }
        }

        public static bool Update(SaleReturnData returnData, IList<ReturnItemData> saleReturnsList, bool isSync = false)
        {
            if (returnData == null || saleReturnsList == null || (saleReturnsList != null && saleReturnsList.Count <= 0) || returnData.ID == Guid.Empty)
                throw new InvalidOperationException("Invalid return data, unable to save.");
            else
            {//both the return and its detail list are not null
                using (var db = new eDMSEntity("eDMSEntity"))
                {
                    DateTime _currentDateTime = DateTime.Now;
                    var _existingReturn = db.SalesReturns.Where(r => r.id == returnData.ID).SingleOrDefault();
                    if (_existingReturn == null)
                        throw new InvalidOperationException("The specified return data is not found.");
                    _existingReturn.salesID = returnData.SalesID;
                    _existingReturn.date = returnData.Date;
                    _existingReturn.processedBy = returnData.ProcessedBy;
                    _existingReturn.isDeleted = returnData.IsDeleted;
                    _existingReturn.createdBy = returnData.CreatedBy;
                    _existingReturn.createdDate = _currentDateTime;

                    var _invalidReturnDetails = saleReturnsList.Any(r => r.SalesReturnID != returnData.ID);
                    if (_invalidReturnDetails)
                        throw new InvalidOperationException("The given return details are not related with the specified return data.");
                    var _returnDetails = db.ReturnedItems.Where(ri => ri.salesReturnID == _existingReturn.id).ToList();
                    if (_returnDetails == null || _returnDetails.Count <= 0)
                    {//new return details to be inserted
                    }
                    else
                    {
                        var _createdBy = _returnDetails.First().createdBy;
                        var _createdDate = _returnDetails.First().createdDate;
                        db.ReturnedItems.RemoveRange(_returnDetails);//remove any previous returned item
                        foreach (var item in saleReturnsList)
                        {
                            var _returnedItem = new ReturnedItem
                            {
                                salesReturnID = _existingReturn.id,
                                itemID = item.ItemID,
                                quantity = item.Quantity,
                                refundedAmount = item.RefundedAmount,
                                createdBy = _createdBy,
                                createdDate = _createdDate,
                                lastUpdatedBy = item.LastUpdatedBy,
                                lastUpdatedDate = _currentDateTime,
                                isDeleted = item.IsDeleted
                            };
                            db.ReturnedItems.Add(_returnedItem);
                        }
                    }
                    //save to database
                    int rows = db.SaveChanges();
                    if (rows > 0 && !isSync)
                    {
                        var _retElem = SyncTransactionBL.GetSaleReturnElement(_existingReturn.id);
                        if (_retElem != null)
                        {
                            var _sales = SalesBL.GetSales(_existingReturn.salesID);
                            if (_sales != null)
                            {
                                var _syncVal = new SyncTransaction()
                                {
                                    id = Guid.NewGuid(),
                                    tableName = "tblSalesReturn",
                                    action = "update",
                                    value = _retElem.ToString(),
                                    branchID = _sales.BranchID,
                                    isDeleted = null
                                };
                                db.SyncTransactions.Add(_syncVal);
                                db.SaveChanges();
                            }
                        }
                    }
                    return rows > 0;
                }
            }
        }

        public static IList<ReturnItemData> GetReturnedItems(Guid returnId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _returnedItems = (from ri in db.ReturnedItems
                                      where ri.salesReturnID == returnId
                                      select new ReturnItemData
                                      {
                                          SalesReturnID = ri.salesReturnID,
                                          ReturnedDetailsID = ri.returnedDetailID,
                                          ItemID = ri.itemID,
                                          Quantity = ri.quantity,
                                          RefundedAmount = ri.refundedAmount,
                                          CreatedBy = ri.createdBy,
                                          CreatedDate = ri.createdDate,
                                          LastUpdatedBy = ri.lastUpdatedBy,
                                          LastUpdatedDate = ri.lastUpdatedDate,
                                          IsDeleted = ri.isDeleted
                                      }).ToList();

                return _returnedItems;
            }
        }

        public static bool Delete(SaleReturnData _currentRecord, bool isSync = false)
        {
            if (_currentRecord == null || _currentRecord.ID == Guid.Empty)
            {
                throw new ArgumentNullException("The argument currentRecord must be valid Sales Return object.");
            }
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //get the sales return data from db
                var _existingReturn = db.SalesReturns.Where(r => r.id == _currentRecord.ID).SingleOrDefault();
                if (_existingReturn == null)
                {
                    throw new InvalidOperationException("The existing sales return data to remove cannot be found.");
                }
                _existingReturn.isDeleted = _currentRecord.IsDeleted;
                _existingReturn.reason = _currentRecord.Reason;
                _existingReturn.processedBy = _currentRecord.ProcessedBy;
                _existingReturn.date = _currentRecord.Date;

                int rows = db.SaveChanges();
                if (rows > 0 && !isSync)
                {
                    var _retElem = SyncTransactionBL.GetSaleReturnElement(_existingReturn.id);
                    if (_retElem != null)
                    {
                        var _sales = SalesBL.GetSales(_existingReturn.salesID);
                        if (_sales != null)
                        {
                            var _syncVal = new SyncTransaction()
                            {
                                id = Guid.NewGuid(),
                                tableName = "tblSalesReturn",
                                action = "delete",
                                value = _retElem.ToString(),
                                branchID = _sales.BranchID,
                                isDeleted = null
                            };
                            db.SyncTransactions.Add(_syncVal);
                            db.SaveChanges();
                        }
                    }
                }
                return rows > 0;
            }
        }
        /// <summary>
        /// Checks whether a sale return object identified by the specified ID exists in database
        /// </summary>
        /// <param name="retId">the sale return ID</param>
        /// <returns>true if the return exists, false otherwise</returns>
        public static bool Exists(Guid retId)
        {
            if (retId == Guid.Empty)
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _exists = db.SalesReturns.Any(sr => sr.id == retId);

                return _exists;
            }
        }

        public static bool CheckPrerequisites(SaleReturnData _retObj, List<ReturnItemData> retItems)
        {
            if (_retObj == null || retItems == null)
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _saleExists = db.Sales.Any(s => s.id == _retObj.SalesID);
                if (!_saleExists) return false;
                var _userExists = db.Users.Any(u => u.userID.ToLower().Equals(_retObj.ProcessedBy.ToLower()) ||
                    u.userID.ToLower().Equals(_retObj.CreatedBy.ToLower()) ||
                    (!string.IsNullOrEmpty(_retObj.LastUpdatedBy) && u.userID.ToLower().Equals(_retObj.LastUpdatedBy)));
                if (!_userExists) return false;

                return true;
            }
        }
    }

    public class SaleReturnData
    {
        public Guid ID { get; set; }
        public Guid SalesID { get; set; }
        public DateTime Date { get; set; }
        public string ProcessedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Reason { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }

        public string ReferenceNo { get; set; }
    }

    public class ReturnItemData
    {
        public Guid SalesReturnID { get; set; }
        public Guid ReturnedDetailsID { get; set; }
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public double RefundedAmount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
