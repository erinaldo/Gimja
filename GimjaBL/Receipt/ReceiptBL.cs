using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class ReceiptBL
    {
        /// <summary>
        /// Gets the next possible receipt number for receipts of a specified branch
        /// </summary>
        /// <param name="branchId">the branch ID</param>
        /// <returns>the new receipt number</returns>
        public static string GetReceiptNumber(string branchId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //get the receipt record with maximum receipt number
                var _receipt = db.Receipts.Where(x => x.id.ToUpper().Contains(branchId.ToUpper())).OrderByDescending(x => x.id).ToList();
                int _requestNumber = 0;
                string _result = string.Empty;
                if (_receipt != null && _receipt.Count > 0)
                {//get the one with max value
                    var _maxReceipt = _receipt.First();
                    //the format is 'xxxxx-nnnnnnnnnn' where x's is the branch ID and n's is serial receipt form number
                    bool validRequestNo = int.TryParse(_maxReceipt.id.Substring(_maxReceipt.id.LastIndexOf("-") + 1), out _requestNumber);

                }
                _result = string.Format("{0}-{1}", branchId, Convert.ToString(_requestNumber + 1).PadLeft(10, '0'));
                return _result;
            }
        }

        public static IEnumerable<ReceiptData> GetActiveReceipts()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _receipts = (from r in db.Receipts
                                 where !(r.isDeleted ?? false)
                                 select r).ToList();

                List<ReceiptData> _result = new List<ReceiptData>();
                _receipts.ForEach(x => _result.Add(CreateReceiptObject(x)));

                return _result;
            }
        }

        private static ReceiptData CreateReceiptObject(Receipt r)
        {
            return new ReceiptData()
            {
                ID = r.id,
                SupplierID = r.rlkSupplierID,
                Date = r.date,
                ReceivedBy = r.receivedBy,
                ApprovedBy = r.approvedBy,
                ApprovedDate = r.approvedDate,
                StoreID = r.storeID,
                IsStoreWarehouse = r.isStoreWarehouse,
                IsApproved = r.isApproved,
                CreatedBy = r.createdBy,
                CreatedDate = r.createdDate,
                LastUpdatedBy = r.lastUpdatedBy,
                LastUpdatedDate = r.lastUpdatedDate,
                IsDeleted = r.isDeleted,
                ReceivedFrom = r.receivedFrom,
                ProcessedBy = r.processedBy
            };
        }

        private static Receipt CreateReceiptObject(ReceiptData receipt)
        {
            var _receipt = new Receipt()
            {
                id = receipt.ID,
                rlkSupplierID = receipt.SupplierID,
                date = receipt.Date,
                receivedBy = receipt.ReceivedBy,
                approvedBy = receipt.ApprovedBy,
                approvedDate = receipt.ApprovedDate,
                storeID = receipt.StoreID,
                isStoreWarehouse = receipt.IsStoreWarehouse,
                isApproved = receipt.IsApproved,
                createdDate = receipt.CreatedDate,
                createdBy = receipt.CreatedBy,
                lastUpdatedBy = receipt.LastUpdatedBy,
                lastUpdatedDate = receipt.LastUpdatedDate,
                isDeleted = receipt.IsDeleted,
                receivedFrom = receipt.ReceivedFrom,
                processedBy = receipt.ProcessedBy
            };
            return _receipt;
        }

        public static bool Insert(ReceiptData receipt, IList<ReceivedItemData> receivedItemsList, bool isSync = false)
        {
            if (receipt == null)
                throw new InvalidOperationException("Invalid receipt object to insert.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                DateTime _currentDateTime = DateTime.Now;
                receipt.CreatedDate = _currentDateTime;

                var _receipt = CreateReceiptObject(receipt);

                db.Receipts.Add(_receipt);

                string _newReceiptId = _receipt.id;
                List<ReceivedItem> _newReceivedItems = new List<ReceivedItem>();
                foreach (var _item in receivedItemsList)
                {
                    _item.CreatedDate = _currentDateTime;
                    _item.ReceiptID = _newReceiptId;
                    var _receivedItem = CreateReceivedItemObject(_item);
                    _newReceivedItems.Add(_receivedItem);
                }
                //add the received items list to database
                db.ReceivedItems.AddRange(_newReceivedItems);
                //save to database
                int rows = db.SaveChanges();
                if (rows > 0 && !isSync)
                {//the receipt is inserted
                    var _receiptElem = SyncTransactionBL.GetReceiptElement(receipt.ID);
                    if (_receiptElem != null)
                    {
                        var _sync = new SyncTransaction()
                        {
                            tableName = "tblReceipt",
                            id = Guid.NewGuid(),
                            action = "insert",
                            value = _receiptElem.ToString(),
                            branchID = receipt.StoreID,
                            isDeleted = null
                        };
                        db.SyncTransactions.Add(_sync);
                        //insert to database
                        db.SaveChanges();
                    }
                }
                return rows > 0;
            }
        }

        private static ReceivedItem CreateReceivedItemObject(ReceivedItemData _item)
        {
            var _receivedItem = new ReceivedItem()
            {
                receiptID = _item.ReceiptID,
                receiptDetailID = _item.ReceiptDetailsID,
                itemID = _item.ItemID,
                rlkManufacturerID = _item.ManufacturerID,
                noPack = _item.NoPack,
                qtyPerPack = _item.QtyPerPack,
                quantity = _item.Quantity,
                price = _item.Price,
                unitSellingPrice = _item.UnitSellingPrice,
                createdBy = _item.CreatedBy,
                createdDate = _item.CreatedDate,
                lastUpdatedBy = _item.LastUpdatedBy,
                lastUpdatedDate = _item.LastUpdatedDate,
                isDeleted = _item.IsDeleted
            };
            return _receivedItem;
        }

        public static List<ApproveReceipt> GetReceipts2Approve()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _receipts = (from r in db.Receipts
                                 join rd in db.ReceivedItems on r.id equals rd.receiptID
                                 where !(r.isApproved ?? false) && string.IsNullOrEmpty(r.approvedBy)
                                 select new ApproveReceipt()
                                 {
                                     ReceiptID = r.id,
                                     ReceiptDetailID = rd.receiptDetailID,
                                     SupplierID = r.rlkSupplierID,
                                     Date = r.date,
                                     StoreID = r.storeID,
                                     ReceivedBy = r.receivedBy,
                                     ItemID = rd.itemID,
                                     Quantity = rd.quantity,
                                     Price = rd.price,
                                     UnitSellingPrice = rd.unitSellingPrice
                                 }).ToList();

                return _receipts;
            }
        }

        public static bool Approve(List<ApproveReceipt> _selectedReceipts, string approvedBy, bool isSync = false)
        {
            if (_selectedReceipts == null || _selectedReceipts.Count == 0)
                throw new InvalidOperationException("Invalid receipts are given.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                List<string> receiptIds = _selectedReceipts.Select(x => x.ReceiptID).ToList();
                var _existingReceipts = (from r in db.Receipts.Include("ReceivedItems")
                                         where receiptIds.Contains(r.id)
                                         select r);
                DateTime _currentDateTime = DateTime.Now;
                List<System.Xml.Linq.XElement> _receiptElems = new List<System.Xml.Linq.XElement>();
                string branchId = null;
                foreach (var _receipt in _existingReceipts)
                {
                    _receipt.isApproved = true;
                    _receipt.approvedBy = approvedBy;
                    _receipt.approvedDate = _currentDateTime;
                    _receipt.lastUpdatedBy = approvedBy;
                    _receipt.lastUpdatedDate = _currentDateTime;
                    if (string.IsNullOrEmpty(branchId))
                        branchId = _receipt.storeID;
                    var _rElem = SyncTransactionBL.GetReceiptElement(_receipt);
                    if (_rElem != null)
                        _receiptElems.Add(_rElem);
                }

                int rows = db.SaveChanges();
                if (rows > 0 && _receiptElems.Count > 0 && !isSync)
                {
                    var _sync = (from s in _receiptElems
                                 select new SyncTransaction()
                                 {
                                     tableName = "tblReceipt",
                                     action = "update",
                                     id = Guid.NewGuid(),
                                     value = s.ToString(),
                                     isDeleted = null,
                                     branchID = branchId
                                 });

                    db.SyncTransactions.AddRange(_sync);
                    db.SaveChanges();
                }
                return rows > 0;
            }
        }

        public static List<ReceivedItemData> GetReceivedItems(string receiptId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _receiptDetails = (from r in db.Receipts
                                       join rd in db.ReceivedItems on r.id equals rd.receiptID
                                       where r.id.Equals(receiptId) && !(r.isDeleted ?? false) && !(rd.isDeleted ?? false)
                                       select rd).ToList();

                List<ReceivedItemData> result = new List<ReceivedItemData>();
                _receiptDetails.ForEach(x => result.Add(CreateReceivedItemObject(x)));
                return result;
            }
        }

        private static ReceivedItemData CreateReceivedItemObject(ReceivedItem x)
        {
            return new ReceivedItemData()
            {
                ReceiptID = x.receiptID,
                ReceiptDetailsID = x.receiptDetailID,
                ItemID = x.itemID,
                ManufacturerID = x.rlkManufacturerID,
                NoPack = x.noPack,
                QtyPerPack = x.qtyPerPack,
                Quantity = x.quantity,
                Price = x.price,
                UnitSellingPrice = x.unitSellingPrice,
                CreatedBy = x.createdBy,
                CreatedDate = x.createdDate,
                LastUpdatedBy = x.lastUpdatedBy,
                LastUpdatedDate = x.lastUpdatedDate,
                IsDeleted = x.isDeleted
            };
        }

        public static bool Update(ReceiptData selectedReceipt, IList<ReceivedItemData> receiptDetails, bool isSync = false)
        {
            if (selectedReceipt == null || receiptDetails == null || string.IsNullOrEmpty(selectedReceipt.ID) ||
                receiptDetails.Count == 0)
                throw new ArgumentNullException("Invalid receipt data and details.");
            if ((selectedReceipt.IsApproved ?? false) && (selectedReceipt.IsDeleted ?? false))
                throw new InvalidOperationException("An approved receipt record cannot be deleted.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //is the receipt exists
                var _existingReceipt = db.Receipts.Where(r => r.id.Equals(selectedReceipt.ID)).SingleOrDefault();
                if (_existingReceipt == null)
                {
                    throw new InvalidOperationException("The specified receipt could not be found. Please check the receipt ID.");
                }
                //are there any details with different receipt ID
                var _otherDetails = receiptDetails.Any(rd => !rd.ReceiptID.Equals(selectedReceipt.ID));
                if (_otherDetails)
                {
                    throw new InvalidOperationException("There are receipt details whose receipt ID is not properly set.");
                }
                DateTime _currentDateTime = DateTime.Now;
                //update the existing receipt
                _existingReceipt.storeID = selectedReceipt.StoreID;
                _existingReceipt.isStoreWarehouse = selectedReceipt.IsStoreWarehouse;
                _existingReceipt.rlkSupplierID = selectedReceipt.SupplierID;
                _existingReceipt.date = selectedReceipt.Date;
                _existingReceipt.receivedFrom = selectedReceipt.ReceivedFrom;
                _existingReceipt.receivedBy = selectedReceipt.ReceivedBy;
                _existingReceipt.processedBy = selectedReceipt.ProcessedBy;
                _existingReceipt.lastUpdatedBy = selectedReceipt.LastUpdatedBy;
                _existingReceipt.lastUpdatedDate = _currentDateTime;
                _existingReceipt.approvedBy = selectedReceipt.ApprovedBy;
                _existingReceipt.approvedDate = selectedReceipt.ApprovedDate;
                _existingReceipt.isApproved = selectedReceipt.IsApproved;
                _existingReceipt.isDeleted = selectedReceipt.IsDeleted;

                foreach (var _d in receiptDetails)
                {
                    var _rdItem = db.ReceivedItems.Where(x => _d.ReceiptID.Equals(x.receiptID) && _d.ReceiptDetailsID == x.receiptDetailID).SingleOrDefault();
                    if (_rdItem != null)
                    {//existing receipt details
                        _rdItem.itemID = _d.ItemID;
                        _rdItem.rlkManufacturerID = _d.ManufacturerID;
                        _rdItem.noPack = _d.NoPack;
                        _rdItem.qtyPerPack = _d.QtyPerPack;
                        _rdItem.quantity = _d.Quantity;
                        _rdItem.price = _d.Price;
                        _rdItem.unitSellingPrice = _d.UnitSellingPrice;
                        _rdItem.lastUpdatedBy = selectedReceipt.LastUpdatedBy;
                        _rdItem.lastUpdatedDate = _currentDateTime;
                    }
                    else
                    {//the receipt detail is new
                        var _n = new ReceivedItem()
                        {
                            receiptID = _d.ReceiptID,
                            receiptDetailID = _d.ReceiptDetailsID,
                            itemID = _d.ItemID,
                            rlkManufacturerID = _d.ManufacturerID,
                            noPack = _d.NoPack,
                            qtyPerPack = _d.QtyPerPack,
                            quantity = _d.Quantity,
                            price = _d.Price,
                            unitSellingPrice = _d.UnitSellingPrice,
                            createdBy = _d.CreatedBy,
                            createdDate = _currentDateTime
                        };
                        db.ReceivedItems.Add(_n);
                    }
                }

                int rows = db.SaveChanges();
                if (rows > 0 && !isSync)
                {
                    var _receiptElem = SyncTransactionBL.GetReceiptElement(selectedReceipt.ID);
                    if (_receiptElem != null)
                    {
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            tableName = "tblReceipt",
                            action = "update",
                            value = _receiptElem.ToString(),
                            branchID = selectedReceipt.StoreID,
                            isDeleted = null
                        };
                        db.SyncTransactions.Add(_sync);
                        db.SaveChanges();
                    }
                }
                return rows > 0;
            }
        }

        public static bool Delete(ReceiptData selectedReceipt, bool isSync = false)
        {
            if (selectedReceipt == null || string.IsNullOrEmpty(selectedReceipt.ID))
                throw new ArgumentNullException("The specified receipt is empty.");
            if (selectedReceipt.IsApproved ?? false)
                throw new InvalidOperationException("An approved receipt record cannot be deleted.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //get the existing receipt record
                var _existingReceipt = db.Receipts.Include("ReceivedItems").Where(r => r.id.Equals(selectedReceipt.ID)).SingleOrDefault();
                if (_existingReceipt == null)
                    throw new InvalidOperationException("The specified receipt object could not be found.");

                _existingReceipt.lastUpdatedBy = selectedReceipt.LastUpdatedBy;
                _existingReceipt.isDeleted = selectedReceipt.IsDeleted;
                _existingReceipt.lastUpdatedDate = DateTime.Now;

                int rows = db.SaveChanges();
                if (rows > 0 && !isSync)
                {
                    var _receiptElem = SyncTransactionBL.GetReceiptElement(_existingReceipt);
                    if (_receiptElem != null)
                    {
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            tableName = "tblReceipt",
                            action = "update",
                            value = _receiptElem.ToString(),
                            branchID = _existingReceipt.storeID,
                            isDeleted = null
                        };
                        db.SyncTransactions.Add(_sync);
                        db.SaveChanges();
                    }
                }
                return rows > 0;
            }
        }

        public static List<ReceiptData> GetActiveReceipts(DateTime dateTime1, DateTime dateTime2)
        {
            if (dateTime1 == DateTime.MinValue || dateTime2 == DateTime.MinValue)
            {
                throw new ArgumentException("Infalid date is specified.");
            }
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var receipts = (from r in db.Receipts.Include("ReceivedItems")
                                where r.date >= dateTime1 && r.date <= dateTime2
                                select r);

                var retValue = new List<ReceiptData>();
                foreach (var r in receipts)
                {
                    var _r = CreateReceiptObject(r);
                    retValue.Add(_r);
                }

                return retValue;
            }
        }
        /// <summary>
        /// Checks whether a receipt object exists in the database
        /// </summary>
        /// <param name="id">the receipt ID</param>
        /// <returns></returns>
        public static bool Exists(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _exists = db.Receipts.Any(r => r.id.Equals(id));

                return _exists;
            }
        }
        /// <summary>
        /// Gets the receipt object identified by the given ID
        /// </summary>
        /// <param name="id">the receipt ID</param>
        /// <returns>the receipt object</returns>
        public static ReceiptData GetReceipt(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _receipt = (from r in db.Receipts
                                where r.id.ToLower().Equals(id.ToLower()) && !(r.isDeleted ?? false)
                                select r).SingleOrDefault();

                return CreateReceiptObject(_receipt);
            }
        }

        public static bool CheckPreRequisites(ReceiptData _receiptObj, List<ReceivedItemData> receivedItems)
        {
            if (_receiptObj == null || receivedItems == null)
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _userExists = db.Users.Any(u => u.userID.ToLower().Equals(_receiptObj.ProcessedBy.ToLower()) ||
                    u.userID.ToLower().Equals(_receiptObj.ReceivedBy.ToLower()));
                if (!_userExists) return false;
                var _branchExists = db.Branches.Any(b => b.id.ToLower().Equals(_receiptObj.StoreID.ToLower()) ||
                    b.id.ToLower().Equals(_receiptObj.ReceivedFrom.ToLower()));
                var _wExists = db.Warehouses.Any(w => w.lkWarehouseID.ToLower().Equals(_receiptObj.ReceivedFrom.ToLower()) ||
                    w.lkWarehouseID.ToLower().Equals(_receiptObj.StoreID.ToLower()));
                if (!(_branchExists || _wExists)) return false;
                var _supplierExists = db.Suppliers.Any(s => s.lkSupplierID.ToLower().Equals(_receiptObj.SupplierID.ToLower()));
                if (!_supplierExists) return false;
                var _itemIds = receivedItems.Select(r => r.ItemID);
                var _itemExists = db.Items.Any(i => _itemIds.Contains(i.itemID));
                if (!_itemExists) return false;
                var _manIds = receivedItems.Select(r => r.ManufacturerID);
                var _manExists = db.Manufacturers.Any(m => _manIds.Contains(m.lkManufacturerID));
                if (!_manExists) return false;
                //otherwise - all are existing
                return true;
            }
        }

        public static bool CalculateAverageCost(IEnumerable<ReceivedItemData> _newItemReceipts, bool itemAdded = true)
        {
            if (_newItemReceipts == null || _newItemReceipts.Count() == 0)
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //for each item in the received items list add the quantity, sum their acquisition price
                //and then calc the average cost
                var _itemIds = _newItemReceipts.Select(r => r.ItemID).ToList();
                var _itemPrice = (from r in db.ReceivedItems
                                  where !(r.isDeleted ?? false) && _itemIds.Contains(r.itemID)
                                  group r by r.itemID into ITEMS
                                  select new { ItemID = ITEMS.Key, TotalPrice = ITEMS.Select(i => i.price).Sum(), TotalQuantity = ITEMS.Select(i => i.quantity).Sum() }).ToList();
                //save the average cost
                foreach (var item in _itemPrice)
                {
                    if (!itemAdded)
                    {
                        var _newItem = _newItemReceipts.Where(i => i.ItemID.Equals(item.ItemID)).SingleOrDefault();
                        if (_newItem != null)
                        {
                            var _price = _newItem.Price + item.TotalPrice;
                            var _qty = _newItem.Quantity + item.TotalQuantity;
                            var _averageCost = 0d;
                            if (_qty != 0)
                                _averageCost = _price / _qty;
                            //save the average cost
                            var _item = db.Items.Where(i => i.itemID.Equals(item.ItemID)).SingleOrDefault();
                            if (_item != null)
                                _item.averageCost = Convert.ToDecimal(_averageCost);
                        }
                    }
                    else
                    {
                        var _averageCost = 0d;
                        if (item.TotalQuantity != 0)
                            _averageCost = item.TotalPrice / item.TotalQuantity;
                        //save the average cost
                        var _item = db.Items.Where(i => i.itemID.Equals(item.ItemID)).SingleOrDefault();
                        if (_item != null)
                            _item.averageCost = Convert.ToDecimal(_averageCost);
                    }
                }
                //save to database
                int rows = db.SaveChanges();
                return rows > 0;
            }
        }
    }

    public class ReceiptData
    {
        public string ID { get; set; }
        public string SupplierID { get; set; }
        public DateTime Date { get; set; }
        public string ReceivedBy { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public string StoreID { get; set; }
        public string ProcessedBy { get; set; }
        public Nullable<bool> IsStoreWarehouse { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string ReceivedFrom { get; set; }
    }
    public class ReceivedItemData
    {
        public string ReceiptID { get; set; }
        public Guid ReceiptDetailsID { get; set; }
        public string ItemID { get; set; }
        public string ManufacturerID { get; set; }
        public Nullable<int> NoPack { get; set; }
        public Nullable<int> QtyPerPack { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double UnitSellingPrice { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
    public class ApproveReceipt
    {
        public string ReceiptID { get; set; }
        public Guid ReceiptDetailID { get; set; }
        public string SupplierID { get; set; }
        public DateTime Date { get; set; }
        public string StoreID { get; set; }
        public string ReceivedBy { get; set; }
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double UnitSellingPrice { get; set; }
    }

}
