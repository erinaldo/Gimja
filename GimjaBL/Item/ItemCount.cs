using GimjaDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class ItemCount
    {
    }

    public class StockTakingBL
    {
        public int ID { get; set; }
        public System.DateTime Date { get; set; }
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        /// <summary>
        /// Gets the list of stock taking records
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StockTakingBL> GetStocktakings()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _stocktakings = (from st in db.Stocktakings
                                     where !(st.isDeleted ?? false)
                                     select st).ToList();
                var _retValue = new List<StockTakingBL>();
                _stocktakings.ForEach(s => _retValue.Add(CreateStocktakingObject(s)));

                return _retValue;
            }
        }
        /// <summary>
        /// Gets the stocktaking object identified by the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StockTakingBL GetStocktaking(int id)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _stocktaking = db.Stocktakings.Where(st => !(st.isDeleted ?? false) && st.id == id).SingleOrDefault();

                var _retValue = CreateStocktakingObject(_stocktaking);
                return _retValue;
            }
        }

        public IEnumerable<StockTakingBL> GetStocktakings(string itemID)
        {
            if (string.IsNullOrWhiteSpace(itemID))
                return null;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _stocktakings = from st in db.Stocktakings
                                    where !(st.isDeleted ?? false) && st.itemID.ToLower().Equals(itemID.ToLower())
                                    select CreateStocktakingObject(st);

                return _stocktakings;
            }
        }

        private static StockTakingBL CreateStocktakingObject(Stocktaking st)
        {
            if (st == null)
                return null;
            return new StockTakingBL()
            {
                ID = st.id,
                Date = st.date,
                ItemID = st.itemID,
                Quantity = st.quantity,
                CreatedBy = st.createdBy,
                CreatedDate = st.createdDate,
                LastUpdatedBy = st.lastUpdatedBy,
                LastUpdatedDate = st.lastUpdatedDate,
                IsDeleted = st.isDeleted
            };
        }

        private static Stocktaking CreateStocktakingObject(StockTakingBL stBL)
        {
            if (stBL == null)
                return null;
            return new Stocktaking()
            {
                id = stBL.ID,
                itemID = stBL.ItemID,
                date = stBL.Date,
                quantity = stBL.Quantity,
                createdBy = stBL.CreatedBy,
                createdDate = stBL.CreatedDate,
                lastUpdatedBy = stBL.LastUpdatedBy,
                lastUpdatedDate = stBL.LastUpdatedDate,
                isDeleted = stBL.IsDeleted
            };
        }
        /// <summary>
        /// Adds a new stocktaking record to database
        /// </summary>
        /// <returns></returns>
        public bool Insert()
        {
            if (!IsValid())
                throw new ArgumentException("The stock taking record to insert is invalid.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //check if it's existing
                var _exists = db.Stocktakings.Any(st => st.id == this.ID);
                if (_exists)
                    throw new ArgumentException("The stock taking record already exists.");
                Stocktaking _newStockTaking = CreateStocktakingObject(this);

                db.Stocktakings.Add(_newStockTaking);
                int rows = db.SaveChanges();
                return rows > 0;
            }
        }

        private bool IsValid()
        {
            if (this.Date == DateTime.MinValue)
                return false;
            if (string.IsNullOrWhiteSpace(this.ItemID))
                return false;
            if (string.IsNullOrWhiteSpace(this.CreatedBy))
                return false;
            if (this.Quantity <= 0)
                return false;
            if (this.CreatedDate == DateTime.MinValue)
                return false;

            //otherwise 
            return true;
        }

        public bool Update()
        {
            if (!IsValid() || string.IsNullOrWhiteSpace(this.LastUpdatedBy) || this.LastUpdatedDate == DateTime.MinValue)
                throw new ArgumentException("The stock taking record to update is invalid.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //check if it's existing
                var _existing = db.Stocktakings.Where(st => st.id == this.ID).SingleOrDefault();
                if (_existing == null)//doesn't exist
                    throw new ArgumentException("The stock taking record doesn't exist.");
                //change the values - only quantity can be changed
                if (_existing.quantity != this.Quantity)
                {
                    _existing.quantity = this.Quantity;
                    _existing.lastUpdatedBy = this.LastUpdatedBy;
                    _existing.lastUpdatedDate = this.LastUpdatedDate;
                }
                int rows = db.SaveChanges();
                return rows > 0;
            }
        }
        /// <summary>
        /// Removes the stock taking record from display - setting isDeleted true
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            if (!IsValid() || string.IsNullOrWhiteSpace(this.LastUpdatedBy) || this.LastUpdatedDate == DateTime.MinValue)
                throw new ArgumentException("The stock taking record to update is invalid.");

            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //check if it's existing
                var _existing = db.Stocktakings.Where(st => st.id == this.ID).SingleOrDefault();
                if (_existing == null)//doesn't exist
                    throw new ArgumentException("The stock taking record doesn't exist.");
                _existing.isDeleted = true;
                _existing.lastUpdatedBy = this.LastUpdatedBy;
                _existing.lastUpdatedDate = this.LastUpdatedDate;

                int rows = db.SaveChanges();
                return rows > 0;
            }
        }
    }

    public class LossAdjustmentBL
    {
        public int ID { get; set; }
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public bool IsLoss { get; set; }
        public string Reason { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        /// <summary>
        /// Gets the list of loss adjustment records
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LossAdjustmentBL> GetLossAdjustments()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _adjustments = (from a in db.LossAdjustments
                                    where !(a.isDeleted ?? false)
                                    select a).ToList();
                var _result = new List<LossAdjustmentBL>();
                _adjustments.ForEach(a => _result.Add(CreateLossAdjustmentObject(a)));

                return _result;
            }
        }
        /// <summary>
        /// Gets a loss adjustment record identified by the specified ID
        /// </summary>
        /// <param name="id">the ID of the loss adjustment record</param>
        /// <returns></returns>
        public LossAdjustmentBL GetLossAdjustment(int id)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _adjustment = db.LossAdjustments.Where(la => !(la.isDeleted ?? false) && la.id == id).SingleOrDefault();

                var _retValue = CreateLossAdjustmentObject(_adjustment);
                return _retValue;
            }
        }
        /// <summary>
        /// Gets the list of loss adjustment records for the given item
        /// </summary>
        /// <param name="itemID">the item ID whose loss adjustments are required</param>
        /// <returns></returns>
        public IEnumerable<LossAdjustmentBL> GetLossAdjustments(string itemID)
        {
            if (string.IsNullOrWhiteSpace(itemID))
                return null;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _adjustments = from a in db.LossAdjustments
                                   where !(a.isDeleted ?? false) && a.itemID.ToLower().Equals(itemID.ToLower())
                                   select CreateLossAdjustmentObject(a);

                return _adjustments;
            }
        }

        private static LossAdjustmentBL CreateLossAdjustmentObject(LossAdjustment a)
        {
            if (a == null)
                return null;
            return new LossAdjustmentBL()
            {
                ID = a.id,
                ItemID = a.itemID,
                Quantity = a.quantity,
                Cost = a.cost,
                IsLoss = a.isLoss,
                Reason = a.reason,
                CreatedBy = a.createdBy,
                CreatedDate = a.createdDate,
                LastUpdatedBy = a.lastUpdatedBy,
                LastUpdatedDate = a.lastUpdatedDate,
                IsDeleted = a.isDeleted
            };
        }

        private static LossAdjustment CreateLossAdjustmentObject(LossAdjustmentBL aBL)
        {
            if (aBL == null)
                return null;
            return new LossAdjustment()
            {
                id = aBL.ID,
                itemID = aBL.ItemID,
                quantity = aBL.Quantity,
                cost = aBL.Cost,
                isLoss = aBL.IsLoss,
                reason = aBL.Reason,
                createdBy = aBL.CreatedBy,
                createdDate = aBL.CreatedDate,
                lastUpdatedBy = aBL.LastUpdatedBy,
                lastUpdatedDate = aBL.LastUpdatedDate,
                isDeleted = aBL.IsDeleted
            };
        }

        public bool Insert()
        {
            if (!IsValid())
                throw new ArgumentException("The loss adjustment record to insert is invalid.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _exists = db.LossAdjustments.Any(a => a.id == this.ID);
                if (_exists)
                    throw new ArgumentException("The loss adjustment to insert already exists.");
                var _newAdjustment = CreateLossAdjustmentObject(this);

                db.LossAdjustments.Add(_newAdjustment);
                int rows = db.SaveChanges();
                return rows > 0;
            }
        }

        private bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(this.ItemID))
                return false;
            if (this.Quantity <= 0)
                return false;
            if (this.Cost <= 0m)
                return false;
            if (string.IsNullOrWhiteSpace(this.Reason))
                return false;
            if (string.IsNullOrWhiteSpace(this.CreatedBy))
                return false;
            if (this.CreatedDate == DateTime.MinValue)
                return false;
            //otherwise
            return true;
        }

        public bool Update()
        {
            if (!IsValid() || this.ID == 0 || this.LastUpdatedDate == DateTime.MinValue || string.IsNullOrWhiteSpace(this.LastUpdatedBy))
                throw new ArgumentException("The loss adjustment record to update is invalid.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _existing = db.LossAdjustments.Where(a => a.id == this.ID && !(a.isDeleted ?? false)).SingleOrDefault();
                if (_existing == null)
                    throw new ArgumentException("The loss adjustment record to update doesn't exist.");
                //update the fields
                _existing.quantity = this.Quantity;
                _existing.cost = this.Cost;
                _existing.isLoss = this.IsLoss;
                _existing.lastUpdatedBy = this.LastUpdatedBy;
                _existing.lastUpdatedDate = this.LastUpdatedDate;

                int rows = db.SaveChanges();
                return rows > 0;
            }
        }

        public bool Delete()
        {
            if (!IsValid() || this.ID == 0)
                throw new ArgumentException("The loss adjustment record to delete is invalid.");
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _existing = db.LossAdjustments.Where(a => a.id == this.ID && !(a.isDeleted ?? false)).SingleOrDefault();
                if (_existing == null)
                    throw new ArgumentException("The loss adjustment record to update doesn't exist.");
                //update the fields
                _existing.isDeleted = true;
                _existing.lastUpdatedBy = this.LastUpdatedBy;
                _existing.lastUpdatedDate = this.LastUpdatedDate;

                int rows = db.SaveChanges();
                return rows > 0;
            }
        }
    }
}
