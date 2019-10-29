using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class IssueBL
    {
        /// <summary>
        /// Gets the next possible issue number for the specified branch
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public static string GetIssueNumber(string branchId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //get the receipt record with maximum receipt number
                var _issue = db.Issuances.Where(x => x.id.ToUpper().Contains(branchId.ToUpper())).OrderByDescending(x => x.id).ToList();
                int _requestNumber = 0;
                string _result = string.Empty;
                if (_issue != null && _issue.Count > 0)
                {//get the one with max value
                    var _maxIssue = _issue.First();
                    //the format is 'xxxxx-nnnnnnnnnn' where x's is the branch ID and n's is serial receipt form number
                    bool validRequestNo = int.TryParse(_maxIssue.id.Substring(_maxIssue.id.LastIndexOf("-") + 1), out _requestNumber);

                }
                _result = string.Format("{0}-{1}", branchId, Convert.ToString(_requestNumber + 1).PadLeft(10, '0'));
                return _result;
            }
        }

        public static IList<IssueData> GetIssueData()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _issues = (from i in db.Issuances
                               where !(i.isDeleted ?? false)
                               select i).ToList();

                var _result = new List<IssueData>();
                _issues.ForEach(x => _result.Add(CreateIssuanceObject(x)));
                return _result;
            }
        }

        private static IssueData CreateIssuanceObject(Issuance i)
        {
            return new IssueData
            {
                ID = i.id,
                IssuedTo = i.issuedTo,
                Date = i.date,
                IssuedBy = i.issuedBy,
                ApprovedDate = i.approvedDate,
                ApprovedBy = i.approvedBy,
                CreatedBy = i.createdBy,
                CreatedDate = i.createdDate,
                LastUpdatedBy = i.lastUpdatedBy,
                LastUpdatedDate = i.lastUpdatedDate,
                IsDeleted = i.isDeleted,
                StoreID = i.storeID,
                WarehouseID = i.warehouseID
            };
        }

        public static bool Insert(IssueData issueData, IList<IssuedItemData> issueItems, bool isSync = false)
        {
            if (issueData == null || issueItems == null || issueItems.Count == 0)
            {
                throw new ArgumentNullException("The specified issue data to insert is invalid.");
            }
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                DateTime _currentDateTime = DateTime.Now;
                issueData.CreatedDate = _currentDateTime;
                var _issue = new Issuance
                {
                    id = issueData.ID,
                    storeID = issueData.StoreID,
                    warehouseID = issueData.WarehouseID,
                    issuedBy = issueData.IssuedBy,
                    issuedTo = issueData.IssuedTo,
                    date = issueData.Date,
                    approvedBy = issueData.ApprovedBy,
                    approvedDate = issueData.ApprovedDate,
                    createdBy = issueData.CreatedBy,
                    createdDate = issueData.CreatedDate,
                    isDeleted = issueData.IsDeleted
                };
                db.Issuances.Add(_issue);
                List<IssuedItem> _issuedList = new List<IssuedItem>();
                foreach (var _item in issueItems)
                {
                    _item.CreatedDate = _currentDateTime;
                    _issuedList.Add(new IssuedItem
                    {
                        issuanceID = _issue.id,
                        issueDetailID = _item.IssueDetailID,
                        itemID = _item.ItemID,
                        quantity = _item.Quantity,
                        noPack = _item.NoPack,
                        qtyPerPack = _item.QtyPerPack,
                        createdBy = _item.CreatedBy,
                        createdDate = _item.CreatedDate,
                        isDeleted = _item.IsDeleted
                    });
                }
                db.IssuedItems.AddRange(_issuedList);

                int rows = db.SaveChanges();
                if (rows > 0 && !isSync)
                {//the issue is inserted
                    var _issueElem = SyncTransactionBL.GetIssuanceElement(issueData.ID);
                    var _syncItem = new SyncTransaction()
                    {
                        id = Guid.NewGuid(),
                        tableName = "tblIssuance",
                        action = "insert",
                        value = _issueElem.ToString(),
                        branchID = issueData.StoreID,
                        isDeleted = null
                    };
                    //insert into database
                    db.SyncTransactions.Add(_syncItem);
                    db.SaveChanges();
                }
                return (rows > 0);
            }
        }

        public static IList<IssuedItemData> GetIssueDetails(string issueId)
        {
            if (string.IsNullOrWhiteSpace(issueId))
                throw new ArgumentNullException("The issue ID specified is null.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _issues = (from i in db.Issuances
                               join ii in db.IssuedItems on i.id equals ii.issuanceID
                               where !(i.isDeleted ?? false) && !(ii.isDeleted ?? false) && i.id.Equals(issueId)
                               select ii).ToList();

                List<IssuedItemData> _result = new List<IssuedItemData>();
                _issues.ForEach(x => _result.Add(CreateIssuedItemObject(x)));
                return _result;
            }
        }

        private static IssuedItemData CreateIssuedItemObject(IssuedItem x)
        {
            return new IssuedItemData()
            {
                IssuanceID = x.issuanceID,
                IssueDetailID = x.issueDetailID,
                ItemID = x.itemID,
                Quantity = x.quantity,
                NoPack = x.noPack,
                QtyPerPack = x.qtyPerPack,
                CreatedBy = x.createdBy,
                LastUpdatedBy = x.lastUpdatedBy,
                LastUpdatedDate = x.lastUpdatedDate,
                IsDeleted = x.isDeleted
            };
        }

        public static bool Update(IssueData selectedIssue, IList<IssuedItemData> issueDetails, bool isSync = false)
        {
            if (selectedIssue == null || issueDetails == null || issueDetails.Count == 0)
            {
                throw new ArgumentNullException("The specified issue object is null.");
            }
            if (!string.IsNullOrEmpty(selectedIssue.ApprovedBy))
                throw new InvalidOperationException("An approved receipt record cannot be deleted.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //is the issue found
                var _existingIssue = db.Issuances.Where(x => x.id.Equals(selectedIssue.ID)).SingleOrDefault();
                if (_existingIssue == null)
                    throw new InvalidOperationException("The specified issue object could not be found.");
                DateTime _currentDateTime = DateTime.Now;
                //update the issue object
                _existingIssue.storeID = selectedIssue.StoreID;
                _existingIssue.warehouseID = selectedIssue.WarehouseID;
                _existingIssue.issuedBy = selectedIssue.IssuedBy;
                _existingIssue.issuedTo = selectedIssue.IssuedTo;
                _existingIssue.date = selectedIssue.Date;
                _existingIssue.lastUpdatedBy = selectedIssue.LastUpdatedBy;
                _existingIssue.lastUpdatedDate = _currentDateTime;
                //get the existing issue details
                var _existingDetails = new List<IssuedItem>();
                foreach (var _d in issueDetails)
                {
                    var _e = db.IssuedItems.Where(x => x.issuanceID.Equals(selectedIssue.ID) && x.issueDetailID == _d.IssueDetailID).SingleOrDefault();
                    if (_e != null)
                    {
                        _e.itemID = _d.ItemID;
                        _e.noPack = _d.NoPack;
                        _e.qtyPerPack = _d.QtyPerPack;
                        _e.quantity = _d.Quantity;
                        _e.lastUpdatedBy = _d.LastUpdatedBy;
                        _e.lastUpdatedDate = _d.LastUpdatedDate;
                    }
                    else
                    {//the detail is new
                        var _n = new IssuedItem()
                        {
                            issuanceID = _d.IssuanceID,
                            issueDetailID = _d.IssueDetailID,
                            itemID = _d.ItemID,
                            quantity = _d.Quantity,
                            noPack = _d.NoPack,
                            qtyPerPack = _d.QtyPerPack,
                            createdBy = _d.CreatedBy,
                            createdDate = _currentDateTime
                        };
                        db.IssuedItems.Add(_n);
                    }
                }
                //save changes to database
                int rows = db.SaveChanges();
                if (rows > 0 && !isSync)
                {//the issuance record is updated
                    var _updatedIssuanceElem = SyncTransactionBL.GetIssuanceElement(_existingIssue.id);
                    if (_updatedIssuanceElem != null)
                    {
                        var _syncValue = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            tableName = "tblIssuance",
                            action = "update",
                            value = _updatedIssuanceElem.ToString(),
                            isDeleted = null,
                            branchID = _existingIssue.storeID
                        };
                        //save to database
                        db.SyncTransactions.Add(_syncValue);
                        db.SaveChanges();
                    }
                }
                return rows > 0;
            }
        }

        public static bool Delete(IssueData selectedIssue, bool isSync = false)
        {
            if (selectedIssue == null)
                throw new ArgumentNullException("The specified issue object is null.");
            if (!string.IsNullOrEmpty(selectedIssue.ApprovedBy))
                throw new InvalidOperationException("An approved receipt record cannot be deleted.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //find the existing issue object
                var _existingIssue = db.Issuances.Where(x => x.id.Equals(selectedIssue.ID)).SingleOrDefault();
                if (_existingIssue == null)
                    throw new InvalidOperationException("The specified issue to delete is not found.");

                _existingIssue.lastUpdatedBy = selectedIssue.LastUpdatedBy;
                _existingIssue.lastUpdatedDate = DateTime.Now;
                _existingIssue.isDeleted = true;
                if (!isSync)
                {//adding the sync data for the delete action of issuance object
                    var _issueXml = SyncTransactionBL.GetIssuanceElement(_existingIssue);
                    SyncTransaction _sync = new SyncTransaction()
                    {
                        tableName = "tblIssuance",
                        action = "delete",
                        value = _issueXml.ToString(),
                        isDeleted = false,
                        branchID = selectedIssue.StoreID,
                        id = Guid.NewGuid()
                    };
                    db.SyncTransactions.Add(_sync);
                }
                int rows = db.SaveChanges();
                return rows > 0;
            }
        }

        public static bool Approve(List<IssueData> issues, string approvedBy)
        {
            if (issues == null || issues.Count == 0)
                throw new ArgumentNullException("Invalid receipts are given.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                List<string> _issueIds = issues.Select(x => x.ID).ToList();
                var _existingIssuances = (from i in db.Issuances
                                          where _issueIds.Contains(i.id)
                                          select i);
                DateTime _currentDateTime = DateTime.Now;
                foreach (var _issue in _existingIssuances)
                {
                    _issue.approvedBy = approvedBy;
                    _issue.approvedDate = _currentDateTime;
                    _issue.lastUpdatedBy = approvedBy;
                    _issue.lastUpdatedDate = _currentDateTime;
                }

                int rows = db.SaveChanges();
                return rows > 0;
            }
        }
        /// <summary>
        /// Gets the list of issuance records that are not approved
        /// </summary>
        /// <returns></returns>
        public static IList<IssueData> GetIssuances2Approve()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _issues = (from i in db.Issuances
                               where !(i.isDeleted ?? false) && string.IsNullOrEmpty(i.approvedBy) && !i.approvedDate.HasValue
                               select i).ToList();

                var _result = new List<IssueData>();
                _issues.ForEach(x => _result.Add(CreateIssuanceObject(x)));
                return _result;
            }
        }
        /// <summary>
        /// Checks whether an issuance record exists in database
        /// </summary>
        /// <param name="id">the issuance record ID</param>
        /// <returns></returns>
        public static bool Exists(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _exists = db.Issuances.Any(i => i.id.ToLower().Equals(id.ToLower()));

                return _exists;
            }
        }

        public static bool CheckPrerequisites(IssueData _issueObj, List<IssuedItemData> issueDetails)
        {
            if (_issueObj == null || issueDetails == null)
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _usersExist = db.Users.Any(u => u.userID.ToLower().Equals(_issueObj.IssuedBy.ToLower()) ||
                    u.userID.ToLower().Equals(_issueObj.IssuedTo.ToLower()) ||
                    u.userID.ToLower().Equals(_issueObj.CreatedBy.ToLower()) ||
                    (!string.IsNullOrEmpty(_issueObj.LastUpdatedBy) && u.userID.ToLower().Equals(_issueObj.LastUpdatedBy.ToLower())) ||
                    (!string.IsNullOrEmpty(_issueObj.ApprovedBy) && u.userID.ToLower().Equals(_issueObj.ApprovedBy.ToLower())));
                if (!_usersExist) return false;
                var _branchExists = db.Branches.Any(b => b.id.ToLower().Equals(_issueObj.StoreID.ToLower()) ||
                    b.id.ToLower().Equals(_issueObj.WarehouseID.ToLower()));
                var _wExists = db.Warehouses.Any(w => w.lkWarehouseID.ToLower().Equals(_issueObj.WarehouseID.ToLower()) ||
                    w.lkWarehouseID.ToLower().Equals(_issueObj.StoreID.ToLower()));
                if (!(_branchExists || _wExists)) return false;
                var _itemIds = issueDetails.Select(id => id.ItemID);
                var _itemExists = db.Items.Any(i => _itemIds.Contains(i.itemID));
                if (!_itemExists) return false;

                return true;
            }
        }
    }

    public class IssueData
    {
        public string ID { get; set; }
        public string IssuedTo { get; set; }
        public System.DateTime Date { get; set; }
        public string IssuedBy { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string StoreID { get; set; }
        public string WarehouseID { get; set; }
    }

    public class IssuedItemData
    {
        public string IssuanceID { get; set; }
        public Guid IssueDetailID { get; set; }
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public Nullable<int> NoPack { get; set; }
        public Nullable<int> QtyPerPack { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
