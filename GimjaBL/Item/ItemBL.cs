using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;

namespace GimjaBL
{
    public class ItemBL //: IDataManager<ItemBL>
    {
        #region Member Variables
        //private string itemID;
        //private int brandID;
        //private int categoryID;
        //private double unitPrice;
        //private double? reorderLevel;
        //private double? pickFaceQty;
        //private int unitID;
        //private double? orderQuantity;
        //private int? taxTypeID;
        //private bool? isTaxExempted;
        //private string description;
        //private bool isActive;
        //private string createdBy;
        //private DateTime createdDate;
        //private string lastUpdatedBy;
        //private DateTime? lastUpdatedDate;
        //private bool? isDeleted;

        //private string origin;

        private bool isUpdate;
        private bool isDataAvailable;
        //private Item item;
        #endregion

        #region Properties
        //public string ItemID
        //{
        //    get { return itemID; }
        //    set { itemID = value; }
        //}

        //public int BrandID
        //{
        //    get { return brandID; }
        //    set { brandID = value; }
        //}

        //public int CategoryID
        //{
        //    get { return categoryID; }
        //    set { categoryID = value; }
        //}

        //public double UnitPrice
        //{
        //    get { return unitPrice; }
        //    set { unitPrice = value; }
        //}

        //public double? PickFaceQty
        //{
        //    get { return pickFaceQty; }
        //    set { pickFaceQty = value; }
        //}

        //public double? OrderQuantity
        //{
        //    get { return orderQuantity; }
        //    set { orderQuantity = value; }
        //}

        //public double? ReorderLevel
        //{
        //    get { return reorderLevel; }
        //    set { reorderLevel = value; }
        //}

        //public int UnitID
        //{
        //    get { return unitID; }
        //    set { unitID = value; }
        //}

        //public int? TaxTypeID
        //{
        //    get { return taxTypeID; }
        //    set { taxTypeID = value; }
        //}

        //public bool? IsTaxExempted
        //{
        //    get { return isTaxExempted; }
        //    set { isTaxExempted = value; }
        //}

        //public string Description
        //{
        //    get { return description; }
        //    set { description = value; }
        //}

        //public bool IsActive
        //{
        //    get { return isActive; }
        //    set { isActive = value; }
        //}

        //public string CreatedBy
        //{
        //    get { return createdBy; }
        //    set { createdBy = value; }
        //}

        //public DateTime CreatedDate
        //{
        //    get { return createdDate; }
        //    set { createdDate = value; }
        //}

        //public string LastUpdatedBy
        //{
        //    get { return lastUpdatedBy; }
        //    set { lastUpdatedBy = value; }
        //}

        //public DateTime? LastUpdatedDate
        //{
        //    get { return lastUpdatedDate; }
        //    set { lastUpdatedDate = value; }
        //}

        //public bool? IsDeleted
        //{
        //    get { return isDeleted; }
        //    set { isDeleted = value; }
        //}

        //public string Origin
        //{
        //    get { return origin; }
        //    set { origin = value; }
        //}

        public bool IsUpdate
        {
            get { return isUpdate; }
            set { isUpdate = value; }
        }

        public bool IsDataAvailable
        {
            get { return isDataAvailable; }
            set { isDataAvailable = value; }
        }

        //public bool IsLoadData { get; set; }

        //public long Available { get; set; }
        #endregion

        public ItemBL()
        {
            //item = new Item();

            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts Item data to tblItem table 
        /// </summary>
        public bool Add(ItemData itemData, bool isSync = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _item = CreateItem(itemData);

                    if (!isSync && _item != null)
                    {
                        var _itemElem = SyncTransactionBL.GetItemElement(_item);
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            action = "insert",
                            tableName = "tblItem",
                            value = _itemElem.ToString(),
                            branchID = "BRC-B-1"//TODO: ADD THE CURRENT BRANCH
                        };
                        _context.SyncTransactions.Add(_sync);
                    }

                    _context.Items.Add(_item);

                    int _row = _context.SaveChanges();
                    return _row > 0;
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);

                        foreach (var ve in eve.ValidationErrors)
                            Console.WriteLine("property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Generates new Item ID from the specified brand, category and origin values
        /// </summary>
        public string GenerateItemID(int brandID, int categoryID, string origin)
        {
            using (var _context = new eDMSEntity("eDMSEntity"))
            {
                if (brandID == 0 && categoryID == 0)
                    throw new InvalidOperationException("Brand ID or Category ID can not be blank");

                string _itemID = "";
                _itemID = _context.Brands.Where(b => b.lkBrandID == brandID).Single().name + "-" +
                          _context.Categories.Where(c => c.lkCategoryID == categoryID).Single().name + "-" +
                          origin;

                return _itemID;
            }
        }

        /// <summary>
        /// Updates Item data on tblItem table
        /// </summary>
        public bool Update(ItemData itemData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _itemData = _context.Items.Single(i => i.itemID == itemData.ItemID);

                if (_itemData == null)
                    throw new InvalidOperationException("Item detail could not be found.");

                //sets the new item data
                _itemData.rlkBrandID = itemData.BrandID;
                _itemData.rlkCategoryID = itemData.CategoryID;
                _itemData.origin = itemData.Origin;
                _itemData.unitPrice = itemData.UnitPrice;
                _itemData.reorderLevel = itemData.ReorderLevel;
                _itemData.pickFaceQty = itemData.PickFaceQty;
                _itemData.rlkUnitID = itemData.UnitID;
                _itemData.orderQuantity = itemData.OrderQuantity;
                _itemData.description = itemData.Description;
                _itemData.isActive = itemData.IsActive;
                _itemData.rlkTaxTypeID = itemData.TaxTypeID;
                _itemData.isTaxExempted = itemData.IsTaxExempted;
                _itemData.averageCost = itemData.AverageCost;
                _itemData.lastUpdatedBy = Singleton.Instance.UserID;
                _itemData.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Deletes Item data from tblItem table
        /// </summary>
        /// <param name="itemID">The current item's ItemID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(string itemID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _itemData = _context.Items.SingleOrDefault(i => i.itemID.Equals(itemID));

                if (_itemData == null)
                    throw new InvalidOperationException("The item you are trying to delete doesn't exist");

                _itemData.isDeleted = true;
                _itemData.lastUpdatedBy = Singleton.Instance.UserID;
                _itemData.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Returns Item data from tblItem table
        /// </summary>
        /// <returns>IEnumerable ItemData</returns>
        public IEnumerable<ItemData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _itemData;

                if (isActive)
                {
                    _itemData = _context.Items.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).OrderBy(r => r.itemID).ToList();
                }
                else
                {
                    _itemData = _context.Items.Where(b => !(b.isDeleted ?? false)).OrderBy(r => r.itemID).ToList();
                }

                List<ItemData> _result = new List<ItemData>();
                foreach (var _i in _itemData)
                {
                    _result.Add(CreateItemData(_i));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates ItemData to be return for GetData call
        /// </summary>
        /// <param unitPrice="source"></param>
        /// <returns>ItemData</returns>
        private ItemData CreateItemData(Item source)
        {
            if (source == null)
                return null;

            ItemData _retValue = new ItemData();

            _retValue.ItemID = source.itemID;
            _retValue.BrandID = source.rlkBrandID;
            _retValue.CategoryID = source.rlkCategoryID;
            _retValue.UnitPrice = source.unitPrice;
            _retValue.ReorderLevel = source.reorderLevel;
            _retValue.PickFaceQty = source.pickFaceQty;
            _retValue.UnitID = source.rlkUnitID;
            _retValue.OrderQuantity = source.orderQuantity;
            _retValue.Description = source.description;
            _retValue.IsActive = source.isActive;
            _retValue.TaxTypeID = source.rlkTaxTypeID;
            _retValue.IsTaxExempted = source.isTaxExempted;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;
            _retValue.Origin = source.origin;
            _retValue.AverageCost = source.averageCost;

            _retValue.Available = source.Available;

            return _retValue;
        }

        /// <summary>
        /// Returns Item object from the ItemData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Item CreateItem(ItemData source)
        {
            if (source == null)
                return null;
            var _itemData = new Item()
            {
                itemID = source.ItemID,
                rlkBrandID = source.BrandID,
                rlkCategoryID = source.CategoryID,
                unitPrice = source.UnitPrice,
                reorderLevel = source.ReorderLevel,
                pickFaceQty = source.PickFaceQty,
                rlkUnitID = source.UnitID,
                orderQuantity = source.OrderQuantity,
                description = source.Description,
                isActive = source.IsActive,
                rlkTaxTypeID = source.TaxTypeID,
                isTaxExempted = source.IsTaxExempted,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now,
                origin = source.Origin,
                averageCost = source.AverageCost,
            };

            return _itemData;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntity"))
            {
                IsDataAvailable = (_context.Items.Count(i => !(i.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        /// <summary>
        /// Checks whether the item record exists in database
        /// </summary>
        /// <param name="itemID">the item ID</param>
        /// <returns></returns>
        public bool Exists(string itemID)
        {
            if (String.IsNullOrWhiteSpace(itemID))
                return false;
            using (var db = new eDMSEntity("eDMSEntities"))
            {
                var _exists = db.Items.Any(i => i.itemID == itemID);

                return _exists;
            }
        }

        ///// <summary>
        ///// Returns Brand data
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<BrandData> GetBrands()
        //{
        //    using (var _context = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _brand = _context.Brands.Where(b => !(b.isDeleted ?? false)).ToList();

        //        List<BrandData> _result = new List<BrandData>();
        //        foreach (var _b in _brand)
        //        {
        //            _result.Add(CreateBrandData(_b));
        //        }

        //        return _result;
        //    }
        //}

        ///// <summary>
        ///// Creates BrandData to be return for GetBrands call
        ///// </summary>
        ///// <param name="source"></param>
        ///// <returns></returns>
        //internal BrandData CreateBrandData(Brand source)
        //{
        //    if (source == null)
        //        return null;

        //    BrandData _retValue = new BrandData();

        //    _retValue.BrandID = source.lkBrandID;
        //    _retValue.Description = source.description;
        //    _retValue.IsActive = source.isActive;
        //    _retValue.CreatedBy = source.createdBy;
        //    _retValue.CreatedDate = source.createdDate;
        //    _retValue.LastUpdatedBy = source.lastUpdatedBy;
        //    _retValue.LastUpdatedDate = source.lastUpdatedDate;
        //    _retValue.IsDeleted = source.isDeleted;

        //    return _retValue;
        //}

        ///// <summary>
        ///// Returns Category data
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<CategoryData> GetCategories()
        //{
        //    using (var _context = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _category = _context.Categories.Where(c => !(c.isDeleted ?? false)).ToList();

        //        List<CategoryData> _result = new List<CategoryData>();
        //        foreach (var _c in _category)
        //        {
        //            _result.Add(CreateCategoryData(_c));
        //        }

        //        return _result;
        //    }
        //}

        ///// <summary>
        ///// Creates CategoryData to be return for GetCategories call
        ///// </summary>
        ///// <param name="source"></param>
        ///// <returns></returns>
        //internal CategoryData CreateCategoryData(Category source)
        //{
        //    if (source == null)
        //        return null;

        //    CategoryData _retValue = new CategoryData();

        //    _retValue.CategoryID = source.lkCategoryID;
        //    _retValue.Description = source.description;
        //    _retValue.IsActive = source.isActive;
        //    _retValue.CreatedBy = source.createdBy;
        //    _retValue.CreatedDate = source.createdDate;
        //    _retValue.LastUpdatedBy = source.lastUpdatedBy;
        //    _retValue.LastUpdatedDate = source.lastUpdatedDate;
        //    _retValue.IsDeleted = source.isDeleted;

        //    return _retValue;
        //}

        ///// <summary>
        ///// Returns Category data
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<UnitData> GetUnits()
        //{
        //    using (var _context = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _unit = _context.Units.Where(u => !(u.isDeleted ?? false)).ToList();

        //        List<UnitData> _result = new List<UnitData>();
        //        foreach (var _u in _unit)
        //        {
        //            _result.Add(CreateUnitData(_u));
        //        }

        //        return _result;
        //    }
        //}

        ///// <summary>
        ///// Creates UnitData to be return for GetUnits call
        ///// </summary>
        ///// <param name="source"></param>
        ///// <returns></returns>
        //internal UnitData CreateUnitData(Unit source)
        //{
        //    if (source == null)
        //        return null;

        //    UnitData _retValue = new UnitData();

        //    _retValue.UnitID = source.lkUnitID;
        //    _retValue.UnitName = source.unitName;
        //    _retValue.Description = source.description;
        //    _retValue.IsActive = source.isActive;
        //    _retValue.CreatedBy = source.createdBy;
        //    _retValue.CreatedDate = source.createdDate;
        //    _retValue.LastUpdatedBy = source.lastUpdatedBy;
        //    _retValue.LastUpdatedDate = source.lastUpdatedDate;
        //    _retValue.IsDeleted = source.isDeleted;

        //    return _retValue;
        //}

        ///// <summary>
        ///// Returns Tax Type data
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<TaxTypeData> GetTaxTypes()
        //{
        //    using (var _context = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _taxType = _context.TaxTypes.Where(tt => !(tt.isDeleted ?? false)).ToList();

        //        List<TaxTypeData> _result = new List<TaxTypeData>();
        //        foreach (var _tt in _taxType)
        //        {
        //            _result.Add(CreateTaxTypeData(_tt));
        //        }

        //        return _result;
        //    }
        //}

        ///// <summary>
        ///// Creates TaxTypeData to be return for GetTaxTypes call
        ///// </summary>
        ///// <param name="source"></param>
        ///// <returns></returns>
        //internal TaxTypeData CreateTaxTypeData(TaxType source)
        //{
        //    if (source == null)
        //        return null;

        //    TaxTypeData _retValue = new TaxTypeData();

        //    _retValue.TaxTypeID = source.lkTaxTypeID;
        //    _retValue.TaxTypeName = source.taxTypeName;
        //    _retValue.IsActive = source.isActive;
        //    _retValue.Rate = source.rate;
        //    _retValue.IsDeleted = source.isDeleted;
        //    _retValue.TaxType = source.taxTypeName + " (" + source.rate + ")";
        //    return _retValue;
        //}

        /// <summary>
        /// Returns items that need to be requested to be issued from warehouse
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable RequestIssue()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _item = _context.Items.Where(b => !(b.isDeleted ?? false) && b.isActive == true).OrderBy(r => r.itemID).ToList();

                //Dictionary<string, string> _result = new Dictionary<string, string>();
                DataTable _result = new DataTable();
                _result.Columns.Add("ItemID");
                _result.Columns.Add("Brand");
                _result.Columns.Add("Category");
                _result.Columns.Add("Origin");
                _result.Columns.Add("Available");
                _result.Columns.Add("ProcessedBy");

                foreach (var _i in _item)
                {
                    if (_i.Available <= _i.reorderLevel)
                    {
                        var _brand = new BrandBL().GetBrand(_i.rlkBrandID).FirstOrDefault();
                        var _category = new CategoryBL().GetCategory(_i.rlkCategoryID).FirstOrDefault();
                        var _processedBy = new UserBL().GetUser(Singleton.Instance.UserID);
                        //Todo: add branch here...
                        _result.Rows.Add(_i.itemID, _brand.Name, _category.Name, _i.origin, _i.Available.ToString(), _processedBy.FullName);
                    }
                }

                return _result;
            }
        }


        //public static IEnumerable<ItemData> GetItems(bool active = true)
        //{
        //    using (var db = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _items = (from i in db.Items
        //                      where !(i.isDeleted ?? false)
        //                      select i);
        //        if (active)
        //            _items = _items.Where(x => x.isActive);

        //        var _result = _items.ToList();
        //        List<ItemData> _retValue = new List<ItemData>();
        //        ItemBL itemLogic = new ItemBL();
        //        _result.ForEach(item => _retValue.Add(itemLogic.CreateItemObject(item)));

        //        return _retValue;
        //    }
        //}

        /// <summary>
        /// Gets available list of items
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ItemData> GetAvailableItems()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                try
                {
                    var _items = (from i in db.Items
                                  where !(i.isDeleted ?? false)
                                  select i).ToList();

                    var _availItems = (from i in _items
                                       where i.Available > 0
                                       select new ItemData()
                                       {
                                           ItemID = i.itemID,
                                           BrandID = i.rlkBrandID,
                                           CategoryID = i.rlkCategoryID,
                                           PickFaceQty = i.pickFaceQty,
                                           OrderQuantity = i.orderQuantity,
                                           ReorderLevel = i.reorderLevel,
                                           TaxTypeID = i.rlkTaxTypeID,
                                           UnitPrice = i.unitPrice,
                                           Description = i.description,
                                           UnitID = i.rlkUnitID,
                                           IsTaxExempted = i.isTaxExempted,
                                           IsActive = i.isActive,
                                           CreatedBy = i.createdBy,
                                           CreatedDate = i.createdDate,
                                           Available = i.Available
                                       }).ToList();

                    return _availItems;
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Gets available list of items in a specified store (branch/warehouse)
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public static IEnumerable<ItemData> GetAvailableItems(string storeId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                try
                {
                    var _receivedItems = (from r in db.Receipts
                                          join ri in db.ReceivedItems on r.id equals ri.receiptID
                                          where !string.IsNullOrEmpty(r.approvedBy) && !(r.isDeleted ?? false) && !(ri.isDeleted ?? false) && r.storeID.Equals(storeId)
                                          select new
                                          {
                                              ItemId = ri.itemID,
                                              Quantity = ri.quantity
                                          }).ToList();

                    var _issuedItems = (from i in db.Issuances
                                        join ii in db.IssuedItems on i.id equals ii.issuanceID
                                        where !string.IsNullOrEmpty(i.approvedBy) && !(i.isDeleted ?? false) && !(ii.isDeleted ?? false) && i.warehouseID.Equals(storeId)
                                        select new
                                        {
                                            ItemId = ii.itemID,
                                            Quantity = ii.quantity
                                        }).ToList();

                    var _soldItems = (from s in db.Sales
                                      join sd in db.SalesDetails on s.id equals sd.salesID
                                      join ret in db.SalesReturns.DefaultIfEmpty() on s.id equals ret.salesID
                                      join retd in db.ReturnedItems on ret.id equals retd.salesReturnID
                                      where !(s.isVoid ?? false) && !(sd.isDeleted ?? false) && s.branchID.Equals(storeId) &&
                                      !(ret.isDeleted ?? false) && !(retd.isDeleted ?? false)
                                      select new
                                      {
                                          ItemId = sd.itemID,
                                          Quantity = (int)sd.quantity - retd.quantity
                                      }).ToList();

                    var _availableItems = from item in db.Items
                                          join _r in _receivedItems on item.itemID equals _r.ItemId
                                          join _i in _issuedItems on item.itemID equals _i.ItemId
                                          join _s in _soldItems on item.itemID equals _s.ItemId
                                          where (_r.Quantity - _i.Quantity - _s.Quantity) > 0
                                          select new ItemData()
                                          {
                                              ItemID = item.itemID,
                                              BrandID = item.rlkBrandID,
                                              CategoryID = item.rlkCategoryID,
                                              PickFaceQty = item.pickFaceQty,
                                              OrderQuantity = item.orderQuantity,
                                              ReorderLevel = item.reorderLevel,
                                              TaxTypeID = item.rlkTaxTypeID,
                                              UnitPrice = item.unitPrice,
                                              Description = item.description,
                                              UnitID = item.rlkUnitID,
                                              IsTaxExempted = item.isTaxExempted,
                                              IsActive = item.isActive,
                                              CreatedBy = item.createdBy,
                                              CreatedDate = item.createdDate,
                                              Available = _r.Quantity - _i.Quantity - _s.Quantity
                                          };

                    return _availableItems;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets available items in a specified store (branch/warehouse)
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public static IEnumerable<ItemData> GetAvailableItems(string storeId, string itemId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                try
                {
                    var _receivedItems = (from r in db.Receipts
                                          join ri in db.ReceivedItems on r.id equals ri.receiptID
                                          where !string.IsNullOrEmpty(r.approvedBy) && !(r.isDeleted ?? false) && !(ri.isDeleted ?? false) && r.storeID.Equals(storeId) && ri.itemID.Equals(itemId)
                                          select new
                                          {
                                              ItemId = ri.itemID,
                                              Quantity = ri.quantity
                                          }).ToList();

                    var _issuedItems = (from i in db.Issuances
                                        join ii in db.IssuedItems on i.id equals ii.issuanceID
                                        where !string.IsNullOrEmpty(i.approvedBy) && !(i.isDeleted ?? false) && !(ii.isDeleted ?? false) && i.warehouseID.Equals(storeId) && ii.itemID.Equals(itemId)
                                        select new
                                        {
                                            ItemId = ii.itemID,
                                            Quantity = ii.quantity
                                        }).ToList();

                    var _soldItems = (from s in db.Sales
                                      join sd in db.SalesDetails on s.id equals sd.salesID
                                      join ret in db.SalesReturns.DefaultIfEmpty() on s.id equals ret.salesID
                                      join retd in db.ReturnedItems.DefaultIfEmpty() on ret.id equals retd.salesReturnID
                                      where !(s.isVoid ?? false) && !(sd.isDeleted ?? false) && s.branchID.Equals(storeId) &&
                                      !(ret.isDeleted ?? false) && !(retd.isDeleted ?? false) && sd.itemID.Equals(itemId)
                                      select new
                                      {
                                          ItemId = sd.itemID,
                                          Quantity = (int)sd.quantity + retd.quantity
                                      }).ToList();

                    var _availableItems = from item in db.Items
                                          join _r in _receivedItems on item.itemID equals _r.ItemId
                                          join _i in _issuedItems on item.itemID equals _i.ItemId
                                          join _s in _soldItems on item.itemID equals _s.ItemId
                                          where (_r.Quantity - _i.Quantity - _s.Quantity) > 0
                                          select new ItemData()
                                          {
                                              ItemID = item.itemID,
                                              BrandID = item.rlkBrandID,
                                              CategoryID = item.rlkCategoryID,
                                              PickFaceQty = item.pickFaceQty,
                                              OrderQuantity = item.orderQuantity,
                                              ReorderLevel = item.reorderLevel,
                                              TaxTypeID = item.rlkTaxTypeID,
                                              UnitPrice = item.unitPrice,
                                              Description = item.description,
                                              UnitID = item.rlkUnitID,
                                              IsTaxExempted = item.isTaxExempted,
                                              IsActive = item.isActive,
                                              CreatedBy = item.createdBy,
                                              CreatedDate = item.createdDate,
                                              Available = _r.Quantity - _i.Quantity - _s.Quantity
                                          };

                    return _availableItems;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Checks whether the iteam id is already exists or not
        /// </summary>
        /// <param name="userID">the item id to check</param>
        /// <returns>false if the item id is already taken. True, otherwise</returns>
        public bool IsValid(string itemID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                return (_context.Items.Where(i => i.itemID == itemID).Count() > 0) ? false : true;
            }
        }

        public static ItemData GetItem(string itemId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _item = db.Items.Where(item => item.itemID.ToLower().Equals(itemId.ToLower())).SingleOrDefault();

                ItemBL itemLogic = new ItemBL();
                var _result = itemLogic.CreateItemObject(_item);

                return _result;
            }
        }

        public static IEnumerable<ItemData> GetItemsForPurchaseOrder()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                try
                {
                    var _items = (from i in db.Items
                                  where i.isActive && !(i.isDeleted ?? false)
                                  //i.Available <= i.reorderLevel
                                  select i).ToList();

                    _items = _items.Where(it => it.Available <= it.reorderLevel).ToList();

                    List<ItemData> _retValue = new List<ItemData>();
                    ItemBL itemLogic = new ItemBL();
                    _items.ForEach(item => _retValue.Add(itemLogic.CreateItemObject(item)));

                    return _retValue;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        internal ItemData CreateItemObject(Item source)
        {
            if (source == null)
                return null;

            ItemData _retValue = new ItemData();

            _retValue.ItemID = source.itemID;
            _retValue.BrandID = source.rlkBrandID;
            _retValue.CategoryID = source.rlkCategoryID;
            _retValue.UnitPrice = source.unitPrice;
            _retValue.ReorderLevel = source.reorderLevel;
            _retValue.PickFaceQty = source.pickFaceQty;
            _retValue.UnitID = source.rlkUnitID;
            _retValue.OrderQuantity = source.orderQuantity;
            _retValue.Description = source.description;
            _retValue.IsActive = source.isActive;
            _retValue.TaxTypeID = source.rlkTaxTypeID;
            _retValue.IsTaxExempted = source.isTaxExempted;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;
            _retValue.Origin = source.origin;

            _retValue.Available = source.Available;

            return _retValue;
        }

        #endregion
    }

    public class ItemData
    {
        public string ItemID { get; set; }
        public int BrandID { get; set; }
        public int CategoryID { get; set; }
        public double UnitPrice { get; set; }
        public Nullable<double> ReorderLevel { get; set; }
        public Nullable<double> PickFaceQty { get; set; }
        public int UnitID { get; set; }
        public Nullable<double> OrderQuantity { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> TaxTypeID { get; set; }
        public Nullable<bool> IsTaxExempted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Origin { get; set; }
        public decimal AverageCost { get; set; }

        public long Available { get; set; }
    }
}
