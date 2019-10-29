using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class SalesBL
    {
        /// <summary>
        /// Gets the next valid reference number for new sales record
        /// </summary>
        /// <returns></returns>
        public static string GetReferenceNumber()
        {
            string _result = string.Empty;
            using (eDMSEntity _db = new eDMSEntity())
            {
                var _sales = _db.Sales.OrderByDescending(s => s.date).ToList();

                var _recent = _sales.FirstOrDefault();
                if (_recent != null)
                {
                    long refNo;
                    bool validRef = long.TryParse(_recent.referenceNo, out refNo);
                    if (validRef)
                    {
                        _result = (refNo + 1).ToString();
                    }
                }
                else//when the sales has no record
                    _result = "1";

                _result = _result.PadLeft(10, '0');
            }
            return _result;
        }

        public static List<SalesData> GetSales()
        {
            using (var db = new eDMSEntity())
            {
                var _sales = (from s in db.Sales.Include("Customer")
                              let fullName = s.Customer.name + " " + s.Customer.fatherName
                              where !(s.isVoid ?? false)
                              select s).ToList();

                var _retValue = new List<SalesData>();
                _sales.ForEach(s => _retValue.Add(CreateSalesObject(s)));

                return _retValue;
            }
        }

        public static SalesData GetSales(Guid salesId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                Sale _sales;
                try
                {
                    _sales = (from s in db.Sales.Include("Customer")
                              where s.id == salesId && !(s.isVoid ?? false)
                              select s).SingleOrDefault();
                }
                catch
                {
                    _sales = null;
                }
                if (_sales == null) return null;
                else
                {
                    var _result = CreateSalesObject(_sales);
                    return _result;
                }
            }
        }

        public static SalesData GetSales(string refNo)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                Sale _sales;
                try
                {
                    _sales = (from s in db.Sales.Include("Customer")
                              where s.referenceNo.Equals(refNo) && !(s.isVoid ?? false)
                              select s).SingleOrDefault();
                }
                catch
                {
                    _sales = null;
                }
                if (_sales == null) return null;
                else
                {
                    var _result = CreateSalesObject(_sales);
                    return _result;
                }
            }
        }

        public static List<SalesDetailData> GetSaleDetails(Guid salesId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                List<SalesDetail> _details = (from d in db.SalesDetails.Include("SalesLocation").Include("Item")
                                              where d.salesID == salesId
                                              select d).ToList();
                var _retValue = new List<SalesDetailData>();
                foreach (var item in _details)
                {
                    _retValue.Add(CreateSalesDetailObject(item));
                }

                return _retValue;
            }
        }

        private static SalesDetailData CreateSalesDetailObject(SalesDetail d)
        {
            return new SalesDetailData()
            {
                SalesID = d.salesID,
                SalesDetailID = d.salesDetailID,
                ItemID = d.itemID,
                SalesFrom = d.salesFrom,
                Quantity = d.quantity,
                Discount = d.discount,
                CreatedBy = d.createdBy,
                CreatedDate = d.createdDate,
                LastUpdatedBy = d.lastUpdatedBy,
                LastUpdatedDate = d.lastUpdatedDate,
                IsDeleted = d.isDeleted,
                TotalPrice = d.quantity * (d.unitPrice - (d.discount ?? 0)),
                UnitPrice = d.unitPrice
            };
        }

        public static List<SalesData> GetSales(DateTime? _dtFrom, DateTime _dtTo)//, string _refNo)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _sales = from s in db.Sales.Include("Customer")
                             where s.date <= _dtTo && !(s.isVoid ?? false)
                             select s;
                if (_dtFrom.HasValue)
                    _sales = _sales.Where(s => s.date >= _dtFrom);
                //if (!string.IsNullOrWhiteSpace(_refNo))
                //    _sales = _sales.Where(s => s.referenceNo.Equals(_refNo));

                var _result = from sd in _sales.ToList()
                              select CreateSalesObject(sd);
                return _result.ToList();
            }
        }

        private static SalesData CreateSalesObject(Sale sd)
        {
            return new SalesData
            {
                ID = sd.id,
                BranchID = sd.branchID,
                SalesDate = sd.date,
                CustomerID = sd.customerID,
                ProcessedBy = sd.processedBy,
                ReceiptID = sd.receiptID,
                AuthorizedBy = sd.authorizedBy,
                ReferenceNo = sd.referenceNo,
                CreatedBy = sd.createdBy,
                CreatedDate = sd.createdDate,
                LastUpdatedBy = sd.lastUpdatedBy,
                LastUpdatedDate = sd.lastUpdatedDate,
                IsVoid = sd.isVoid,
                CustomerName = sd.Customer.FullName,
                IsSalesCredit = sd.isSalesCredit,
                FSNo = sd.fsNo,
                Reference = sd.reference,
                RefNo = sd.refNo,
                RefNote = sd.refNote
            };
        }

        /// <summary>
        /// Inserts the specified sales data to database
        /// </summary>
        /// <param name="salesData"></param>
        /// <returns>the reference number of the saved sales data</returns>
        public static bool Insert(SalesData salesData, IList<SalesDetailData> details, IList<ItemRequestData> itemRequestList = null, bool isSync = false)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                if (salesData == null || salesData.ID == Guid.Empty)
                    throw new InvalidOperationException("Invalid sales data found.");
                else
                {
                    var _sales = db.Sales.Where(s => s.referenceNo == salesData.ReferenceNo).SingleOrDefault();
                    if (_sales != null)
                    {
                        throw new InvalidOperationException("Duplicate reference number found.");
                    }
                    DateTime currentDateTime = DateTime.Now;
                    Sale _newSaleObj = CreateSalesObject(salesData);
                    _newSaleObj.createdDate = currentDateTime;

                    if (_newSaleObj.receiptID.Equals(string.Empty))
                        _newSaleObj.receiptID = null;

                    db.Sales.Add(_newSaleObj);
                    if (_newSaleObj.isSalesCredit)
                    {
                        var _totalCost = details.Select(d => d.Quantity * (d.UnitPrice - (d.Discount ?? 0))).Sum();
                        CustomerLedger _ledger = new CustomerLedger()
                        {
                            customerID = _newSaleObj.customerID,
                            referenceNo = _newSaleObj.referenceNo,
                            receivable = Convert.ToDecimal(_totalCost),
                            date = currentDateTime,
                            isActive = true,
                            lkCreditStatusID = 1//TODO: ENSURE THE PENDING STATUS HAS VALUE 1
                        };
                        //add to the database collection
                        db.CustomerLedgers.Add(_ledger);
                    }

                    #region Inserting Sales Detail
                    if (details == null || details.Count == 0)
                        throw new InvalidOperationException("Invalid sales ID or invalid sales detail attempted to be inserted.");
                    var hasNoSalesId = details.Any(d => d.SalesID == Guid.Empty || d.SalesID != salesData.ID);
                    if (hasNoSalesId)
                        throw new InvalidOperationException("A detail has no sales ID or has a different sales ID to which the detail will be saved.");

                    var _currentSaleId = salesData.ID;
                    var _existingDetails = db.SalesDetails.Where(sd => _currentSaleId == sd.salesID).ToList();
                    var _duplicateDetails = (from ed in _existingDetails
                                             join pd in details on ed.itemID equals pd.ItemID
                                             select ed).ToList();
                    if (_duplicateDetails != null && _duplicateDetails.Count > 0)
                        throw new InvalidOperationException("A duplicate item is found for the same sales record.");

                    var _detailItemIds = details.GroupBy(d => d.ItemID).Select(g => g.Count()).ToList();
                    if (_detailItemIds.Any(c => c > 1))
                    {//the item ids are more than one for the same sale
                        throw new InvalidOperationException("This item is duplicated for the same sales.");
                    }
                    SaleLocationBL _location = new SaleLocationBL();
                    var _warehouseLocation = _location.GetWarehouseLocation();
                    //insert each of the details into the db
                    foreach (var d in details)
                    {
                        var _detailObj = SalesDetailBL.CreateSalesDetailObject(d);
                        _detailObj.createdDate = currentDateTime;
                        if (_warehouseLocation != null && _warehouseLocation.SalesLocationID == _detailObj.salesFrom)// _detailObj.salesFrom == 3)
                        {//TODO: ENSURE WAREHOUSE LOCATION IS BEING CHECKED APPROPRIATELY
                            var _req = itemRequestList.Where(r => r.itemID.Equals(d.ItemID)).SingleOrDefault();
                            if (_req != null)
                            {
                                ItemRequest _request = new ItemRequest
                                {
                                    branchID = _newSaleObj.branchID,
                                    itemID = _req.itemID,
                                    referenceNo = _newSaleObj.referenceNo,
                                    salesID = d.SalesID,
                                    warehouseID = _req.warehouseId
                                };
                                //add the item request to database
                                db.ItemRequests.Add(_request);
                            }
                        }
                        db.SalesDetails.Add(_detailObj);
                    }
                    #endregion
                    int rows = db.SaveChanges();
                    if (rows > 0 && !isSync)
                    {
                        var _salesElem = SyncTransactionBL.GetSaleElement(salesData.ID);
                        if (_salesElem != null)
                        {
                            var _syncVal = new SyncTransaction()
                            {
                                id = Guid.NewGuid(),
                                tableName = "tblSales",
                                action = "insert",
                                value = _salesElem.ToString(),
                                branchID = _newSaleObj.branchID,
                                isDeleted = null
                            };
                            db.SyncTransactions.Add(_syncVal);
                            //save to database
                            db.SaveChanges();
                        }
                    }
                    return rows > 0;
                }
            }
        }

        public static bool Update(SalesData salesData)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                if (salesData == null || salesData.ID == Guid.Empty)
                    return false;
                else
                {
                    var _sales = db.Sales.Where(s => s.id == salesData.ID).SingleOrDefault();
                    if (_sales == null)
                        throw new InvalidOperationException("The sales record to edit could not be found.");

                    _sales.branchID = salesData.BranchID;
                    _sales.customerID = salesData.CustomerID;
                    _sales.processedBy = salesData.ProcessedBy;
                    _sales.receiptID = salesData.ReceiptID;
                    _sales.date = salesData.SalesDate;
                    _sales.authorizedBy = salesData.AuthorizedBy;
                    _sales.lastUpdatedBy = salesData.LastUpdatedBy;
                    _sales.lastUpdatedDate = DateTime.Now;

                    int _rows = db.SaveChanges();
                    return _rows > 0;
                }
            }
        }

        private static Sale CreateSalesObject(SalesData salesData)
        {
            return new Sale()
            {
                id = salesData.ID,
                branchID = salesData.BranchID,
                date = salesData.SalesDate,
                customerID = salesData.CustomerID,
                processedBy = salesData.ProcessedBy,
                receiptID = salesData.ReceiptID,
                authorizedBy = salesData.AuthorizedBy,
                createdBy = salesData.CreatedBy,
                createdDate = salesData.CreatedDate,
                lastUpdatedBy = salesData.LastUpdatedBy,
                lastUpdatedDate = salesData.LastUpdatedDate,
                isVoid = salesData.IsVoid,
                referenceNo = salesData.ReferenceNo,
                isSalesCredit = salesData.IsSalesCredit,
                fsNo = salesData.FSNo,
                reference = salesData.Reference,
                refNo = salesData.RefNo,
                refNote = salesData.RefNote
            };
        }

        public static bool Void(Guid _salesId, bool isSync = false)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //get the sales record
                var _sales = db.Sales.Where(s => s.id == _salesId).SingleOrDefault();
                if (_sales != null)
                {
                    _sales.isVoid = true;
                    var _ledgers = db.CustomerLedgers.Where(l => l.referenceNo.Equals(_sales.referenceNo)).ToList();
                    if (_ledgers != null && _ledgers.Count > 0)
                    {
                        _ledgers.ForEach(l => l.isActive = false);
                    }
                }
                int rows = db.SaveChanges();
                if (rows > 0 && !isSync)
                {
                    var _salesElem = SyncTransactionBL.GetSaleElement(_salesId);
                    if (_salesElem != null)
                    {
                        var _syncVal = new SyncTransaction()
                        {
                            tableName = "tblSales",
                            action = "delete",
                            id = Guid.NewGuid(),
                            value = _salesElem.ToString(),
                            branchID = _sales.branchID,
                            isDeleted = null
                        };
                        db.SyncTransactions.Add(_syncVal);
                        db.SaveChanges();
                    }
                }
                return rows > 0;
            }
        }

        public static bool Void(SalesData salesData, bool isSync = false)
        {
            if (salesData == null)
                return false;
            else
            {
                using (var db = new eDMSEntity("eDMSEntity"))
                {
                    //get the sales record
                    var _sales = db.Sales.Where(s => s.id == salesData.ID && s.referenceNo.Equals(salesData.ReferenceNo)).SingleOrDefault();
                    if (_sales != null)
                    {
                        _sales.isVoid = true;
                        var _ledgers = db.CustomerLedgers.Where(l => l.referenceNo.Equals(_sales.referenceNo)).ToList();
                        if (_ledgers != null && _ledgers.Count > 0)
                        {
                            _ledgers.ForEach(l => l.isActive = false);
                        }
                    }

                    int rows = db.SaveChanges();
                    if (rows > 0 && !isSync)
                    {
                        var _salesElem = SyncTransactionBL.GetSaleElement(_sales.id);
                        if (_salesElem != null)
                        {
                            var _syncVal = new SyncTransaction()
                            {
                                tableName = "tblSales",
                                action = "delete",
                                id = Guid.NewGuid(),
                                value = _salesElem.ToString(),
                                branchID = _sales.branchID,
                                isDeleted = null
                            };
                            db.SyncTransactions.Add(_syncVal);
                            db.SaveChanges();
                        }
                    }
                    return rows > 0;
                }
            }
        }

        public static bool Update(SalesData salesData, IList<SalesDetailData> salesDetails, IList<ItemRequestData> requestList = null, bool isSync = false)
        {
            if (salesData == null || salesDetails == null || salesDetails.Count == 0)
                throw new InvalidOperationException("Unable to get a sales record to update.");
            if (!string.IsNullOrEmpty(salesData.AuthorizedBy))
                throw new InvalidOperationException("Authorized sales data cannot be edited.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //get the sales record
                var _sales = db.Sales.Where(s => s.id == salesData.ID && s.referenceNo.Equals(salesData.ReferenceNo)).SingleOrDefault();
                if (_sales != null)
                {
                    Guid salesId = _sales.id;
                    var _validDetails = salesDetails.Any(sd => (sd.SalesID != salesId) && sd.SalesID != Guid.Empty);
                    if (_validDetails)
                    {
                        throw new InvalidOperationException("The sales details are not for the given sales record. Please retry!");
                    }
                    DateTime currentDateTime = DateTime.Now;
                    _sales.branchID = salesData.BranchID;
                    _sales.customerID = salesData.CustomerID;
                    _sales.processedBy = salesData.ProcessedBy;
                    _sales.receiptID = salesData.ReceiptID;
                    _sales.date = salesData.SalesDate; //_sales.authorizedBy = salesData.AuthorizedBy;
                    _sales.isSalesCredit = salesData.IsSalesCredit;
                    _sales.fsNo = salesData.FSNo;
                    _sales.refNo = salesData.RefNo;
                    _sales.reference = salesData.Reference;
                    _sales.refNote = salesData.RefNote;
                    _sales.lastUpdatedBy = salesData.LastUpdatedBy;
                    _sales.lastUpdatedDate = currentDateTime;
                    if (_sales.isSalesCredit)
                    {
                        var _totalCost = salesDetails.Select(d => d.Quantity * (d.UnitPrice - (d.Discount ?? 0))).Sum();
                        var _ledgers = db.CustomerLedgers.ToList();
                        _ledgers = _ledgers.Where(l => l.referenceNo.Equals(_sales.referenceNo)).ToList();
                        if (_ledgers != null && _ledgers.Count > 0)
                        {
                            _ledgers.ForEach(l => l.receivable = Convert.ToDecimal(_totalCost));
                        }
                        else
                        {//the customer ledger is new
                            var _ledger = new CustomerLedger()
                            {
                                customerID = _sales.customerID,
                                referenceNo = _sales.referenceNo,
                                date = _sales.date,
                                receivable = Convert.ToDecimal(_totalCost),
                                lkCreditStatusID = 1,//TODO: Ensure that the pending credit status ID is 1
                                isActive = true,
                                branchID = _sales.branchID
                            };
                            db.CustomerLedgers.Add(_ledger);
                        }
                    }

                    foreach (var detail in salesDetails)
                    {
                        var _salesDetail = db.SalesDetails.Where(sd => sd.salesID == detail.SalesID && sd.itemID == detail.ItemID).SingleOrDefault();
                        if (_salesDetail == null)
                        {//adding new sales detail for the existing sales
                            detail.SalesID = salesId;
                            detail.SalesDetailID = Guid.NewGuid();
                            detail.CreatedDate = currentDateTime;
                            _salesDetail = SalesDetailBL.CreateSalesDetailObject(detail);
                            db.SalesDetails.Add(_salesDetail);
                            //handle request data
                            if (_salesDetail.salesFrom == 3)
                            {//warehouse location is selected to pick items from
                                if (requestList != null)
                                {
                                    var _existingRequest = db.ItemRequests.Where(r => r.itemID.Equals(_salesDetail.itemID) && r.salesID == _sales.id).SingleOrDefault();
                                    if (_existingRequest == null)
                                    {//create new item request object
                                        var _requestForItem = requestList.Where(r => r.itemID.Equals(_salesDetail.itemID)).SingleOrDefault();
                                        if (_requestForItem != null)
                                        {
                                            var _request = new ItemRequest()
                                            {
                                                itemID = _requestForItem.itemID,
                                                warehouseID = _requestForItem.warehouseId,
                                                branchID = _sales.branchID,
                                                referenceNo = _sales.referenceNo,
                                                salesID = _sales.id
                                            };
                                            db.ItemRequests.Add(_request);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {//modifying an existing sales details
                            //_salesDetail.salesID = salesId;
                            _salesDetail.discount = detail.Discount;
                            _salesDetail.quantity = detail.Quantity;
                            _salesDetail.salesFrom = detail.SalesFrom; //_salesDetail.itemID = detail.ItemId;
                            //handle item requests
                            var _existingRequest = db.ItemRequests.Where(r => r.itemID.Equals(_salesDetail.itemID) && r.salesID == _sales.id).SingleOrDefault();
                            if (_salesDetail.salesFrom == 3)
                            {//it is warehouse pick location
                                if (_existingRequest == null)
                                {//create new item request object
                                    var _requestForItem = requestList.Where(r => r.itemID.Equals(_salesDetail.itemID)).SingleOrDefault();
                                    if (_requestForItem != null)
                                    {
                                        var _request = new ItemRequest()
                                        {
                                            itemID = _requestForItem.itemID,
                                            warehouseID = _requestForItem.warehouseId,
                                            branchID = _sales.branchID,
                                            referenceNo = _sales.referenceNo,
                                            salesID = _sales.id
                                        };
                                        db.ItemRequests.Add(_request);
                                    }
                                }
                            }
                            else
                            {//another pick location
                                if (_existingRequest != null)
                                {
                                    db.ItemRequests.Remove(_existingRequest);
                                }
                            }
                        }
                    }
                    if (!isSync)
                    {//handle update sync data
                        var _salesElem = SyncTransactionBL.GetSaleElement(_sales.id);
                        if (_salesElem != null)
                        {
                            var _syncVal = new SyncTransaction()
                            {
                                id = Guid.NewGuid(),
                                tableName = "tblSales",
                                action = "update",
                                value = _salesElem.ToString(),
                                branchID = _sales.branchID,
                                isDeleted = null
                            };
                            db.SyncTransactions.Add(_syncVal);
                        }
                    }
                    int rows = db.SaveChanges();
                    return rows > 0;
                }
                return false;
            }
        }

        public static bool UpdateCreditStatus(List<SalesCreditStatusData> updatedSales)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //get the sales with the ref. numbers
                List<string> _refNos = (from us in updatedSales select us.ReferenceNo).ToList();
                var _sales = (from s in db.Sales//.Include("CreditPayment")
                              where _refNos.Contains(s.referenceNo)
                              select s).ToDictionary(s => s.referenceNo, v => v.id);

                foreach (var statusUpdate in updatedSales)
                {
                    //SalesCreditStatusData _newStatus = updatedSales.Where(us=>us.RefNo==sale.referenceNo);
                    SalesCreditStatusData existingStatus = CreditStatusBL.GetCreditStatus(statusUpdate.ReferenceNo, statusUpdate.Item);
                    if (statusUpdate.CreditStatusID != existingStatus.CreditStatusID)
                    {
                        Guid _id = _sales[statusUpdate.ReferenceNo];
                        var _creditPmt = db.CreditPayments.Where(cp => cp.salesID == _id && cp.itemID == statusUpdate.Item).SingleOrDefault();
                        if (_creditPmt != null)
                        {
                            _creditPmt.rlkCreditStatusID = statusUpdate.CreditStatusID;
                            _creditPmt.processedBy = "LogonUser";//TODO: PASS THE LOGON USER HERE
                            _creditPmt.lastUpdatedBy = "LogonUser";
                            _creditPmt.lastUpdatedDate = DateTime.Now;
                        }
                    }
                }

                int rows = db.SaveChanges();
                return rows > 0;
            }
        }

        public static List<AuthorizeSalesData> GetSalesToAuthorize()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _sales = (from s in db.Sales.Include("Customer")
                              where (string.IsNullOrEmpty(s.authorizedBy)) && !(s.isVoid ?? false)
                              let fullName = s.Customer.name + " " + s.Customer.fatherName
                              select new AuthorizeSalesData
                              {
                                  Selected = false,
                                  ReferenceNo = s.referenceNo,
                                  SalesDate = s.date,
                                  Customer = s.Customer.FullName,
                                  Cashier = s.processedBy,
                                  SalesID = s.id
                              }).ToList();

                return _sales;
            }
        }

        public static bool AuthorizeSales(List<Guid> saleIds, string authorizedBy)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _sales = db.Sales.Where(s => saleIds.Contains(s.id) && (s.authorizedBy.Equals(null) || s.authorizedBy.Equals("")) && !(s.isVoid ?? false)).ToList();

                foreach (var _sale in _sales)
                {
                    _sale.authorizedBy = authorizedBy;
                }

                int rows = db.SaveChanges();
                return rows > 0;
            }
        }
        /// <summary>
        /// Gets the list of sale details for the sales record 
        /// </summary>
        /// <param name="_refNo">the sales reference number</param>
        /// <returns></returns>
        public static List<SalesDetailData> GetSaleDetails(string _refNo)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _details = from s in db.Sales
                               join d in db.SalesDetails.Include("SalesLocation").Include("Item") on s.id equals d.salesID
                               where s.referenceNo.Equals(_refNo)
                               select new SalesDetailData()
                               {
                                   SalesID = d.salesID,
                                   SalesDetailID = d.salesDetailID,
                                   ItemID = d.itemID,
                                   SalesFrom = d.salesFrom,
                                   Quantity = d.quantity,
                                   Discount = d.discount,
                                   CreatedBy = d.createdBy,
                                   CreatedDate = d.createdDate,
                                   LastUpdatedBy = d.lastUpdatedBy,
                                   LastUpdatedDate = d.lastUpdatedDate,
                                   IsDeleted = d.isDeleted,
                                   TotalPrice = d.quantity * (d.unitPrice - (d.discount ?? 0))
                               };

                return _details.ToList();
            }
        }
        /// <summary>
        /// Gets list of sales data objects that has no return data
        /// </summary>
        /// <returns></returns>
        public static List<SalesData> GetSalesToReturn()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _returnSalesIds = (from r in db.SalesReturns.Include("ReturnedItems")
                                       where !(r.isDeleted ?? false)
                                       select r.salesID).ToList();
                var _sales = (from s in db.Sales.Include("SalesDetails")
                              where !_returnSalesIds.Contains(s.id)
                              select s).ToList();

                List<SalesData> _result = new List<SalesData>();
                _sales.ForEach(s => _result.Add(CreateSalesObject(s)));

                return _result;
            }
        }
        /// <summary>
        /// Checks whether the sales record exists in database
        /// </summary>
        /// <param name="saleId">the sale ID</param>
        /// <returns></returns>
        public static bool Exists(Guid saleId)
        {
            if (saleId == Guid.Empty)
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _exists = db.Sales.Any(s => s.id == saleId);

                return _exists;
            }
        }

        public static bool CheckPrerequisites(SalesData _saleObj, List<SalesDetailData> saleDetails)
        {
            if (_saleObj == null || saleDetails == null)
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _branchExists = db.Branches.Any(b => b.id.ToLower().Equals(_saleObj.BranchID.ToLower()));
                if (!_branchExists) return false;
                var _userExists = db.Users.Any(u => u.userID.ToLower().Equals(_saleObj.ProcessedBy.ToLower()) ||
                    u.userID.ToLower().Equals(_saleObj.CreatedBy.ToLower()) ||
                    (string.IsNullOrEmpty(_saleObj.AuthorizedBy) && u.userID.ToLower().Equals(_saleObj.AuthorizedBy.ToLower())));
                if (!_userExists) return false;
                var _custExists = db.Customers.Any(c => c.id.ToLower().Equals(_saleObj.CustomerID.ToLower()));
                if (!_custExists) return false;
                var _itemIds = saleDetails.Select(sd => sd.ItemID.ToLower());
                var _itemExists = db.Items.Any(i => _itemIds.Contains(i.itemID.ToLower()));
                if (!_itemExists) return false;
                var _locationIds = saleDetails.Select(sd => sd.SalesFrom);
                var _locationExists = db.SalesLocations.Any(l => _locationIds.Contains(l.lkSalesLocationID));
                if (!_locationExists) return false;

                //otherwise
                return true;
            }
        }
        /// <summary>
        /// Gets the total price of a given sales
        /// </summary>
        /// <param name="saleId"></param>
        /// <returns></returns>
        public static double GetTotalPrice(Guid saleId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _totalPrice = (from s in db.Sales.Include("SalesDetails")
                                   where !(s.isVoid ?? false) && s.id == saleId
                                   select s.SalesDetails.Sum(sd => sd.unitPrice * sd.quantity)).FirstOrDefault();

                return _totalPrice;
            }
        }
    }

    public class SalesData
    {
        public Guid ID { get; set; }
        public string BranchID { get; set; }
        public DateTime SalesDate { get; set; }
        public string CustomerID { get; set; }
        public string ProcessedBy { get; set; }
        public string ReceiptID { get; set; }
        public string AuthorizedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsVoid { get; set; }
        /// <summary>
        /// An automatically generated ref # for each sale
        /// </summary>
        public string ReferenceNo { get; set; }
        public bool IsSalesCredit { get; set; }
        /// <summary>
        /// User Input ref # from the cash register
        /// </summary>
        public string RefNo { get; set; }
        public string Reference { get; set; }
        public string FSNo { get; set; }
        public string RefNote { get; set; }

        public string CustomerName { get; set; }

        public IList<SalesDetailData> SalesDetails { get; set; }
    }

    public class AuthorizeSalesData
    {
        public bool Selected { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime SalesDate { get; set; }
        public string Customer { get; set; }
        public string Cashier { get; set; }
        public Guid SalesID { get; set; }
    }

    public class SalesStatusData
    {

    }
}
