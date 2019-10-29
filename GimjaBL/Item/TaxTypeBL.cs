using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class TaxTypeBL
    {
        #region Member Variables
        //private int taxTypeID;
        //private string taxTypeName;
        //private double rate;
        //private bool isActive;
        //private bool? isDeleted;
        //private string createdBy;
        //private DateTime createdDate;
        //private string lastUpdatedBy;
        //private DateTime? lastUpdatedDate;

        private bool isUpdate;
        private bool isDataAvailable;

        //TaxType taxType;
        #endregion

        public TaxTypeBL()
        {
            //taxType = new TaxType();
            HasData();
        }

        #region Properties

        //public int TaxTypeID
        //{
        //    get { return taxTypeID; }
        //    set { taxTypeID = value; }
        //}

        //public string TaxTypeName
        //{
        //    get { return taxTypeName; }
        //    set { taxTypeName = value; }
        //}

        //public double Rate
        //{
        //    get { return rate; }
        //    set { rate = value; }
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

        //public string TaxType { get; set; }
        #endregion

        #region Method

        /// <summary>
        /// Inserts TaxType data to lkTaxType
        /// </summary>
        /// <param name="taxTypeData">TaxTypeData object</param>
        /// <returns></returns>
        public bool Add(TaxTypeData taxTypeData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _taxType = CreateTaxType(taxTypeData);
                    _context.TaxTypes.Add(_taxType);

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
        /// Updates TaxType data on lkTaxType
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(TaxTypeData taxTypeData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _taxTypeData = _context.TaxTypes.Single(tf => tf.lkTaxTypeID == taxTypeData.TaxTypeID);

                if (_taxTypeData == null)
                    throw new InvalidOperationException("TaxType detail could not be found.");

                //sets the new taxType data
                _taxTypeData.taxTypeName = taxTypeData.TaxTypeName;
                _taxTypeData.rate = taxTypeData.Rate;
                _taxTypeData.isActive = taxTypeData.IsActive;

                _taxTypeData.lastUpdatedBy = Singleton.Instance.UserID;
                _taxTypeData.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Deletes TaxType data from lkTaxType table
        /// </summary>
        /// <param name="taxTypeID">The current taxType's TaxTypeID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(int taxTypeID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _taxType = _context.TaxTypes.SingleOrDefault(c => c.lkTaxTypeID == taxTypeID);

                if (_taxType == null)
                    throw new InvalidOperationException("The taxType you are trying to delete doesn't exist");

                _taxType.isDeleted = true;
                _taxType.lastUpdatedBy = Singleton.Instance.UserID;
                _taxType.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Returns TaxType data from lkTaxType table
        /// </summary>
        /// <returns>IEnumerable TaxTypeData</returns>
        public IEnumerable<TaxTypeData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _taxType;

                if (isActive)
                {
                    _taxType = _context.TaxTypes.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).OrderBy(r => r.taxTypeName).ToList();
                }
                else
                {
                    _taxType = _context.TaxTypes.Where(b => !(b.isDeleted ?? false)).OrderBy(r => r.taxTypeName).ToList();
                }

                List<TaxTypeData> _result = new List<TaxTypeData>();
                foreach (var _b in _taxType)
                {
                    _result.Add(CreateBrandData(_b));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates TaxTypeData to be return for GetData call
        /// </summary>
        /// <param name="source"></param>
        /// <returns>TaxTypeData</returns>
        internal TaxTypeData CreateBrandData(TaxType source)
        {
            if (source == null)
                return null;

            TaxTypeData _retValue = new TaxTypeData();

            _retValue.TaxTypeID = source.lkTaxTypeID;
            _retValue.TaxTypeName = source.taxTypeName;
            _retValue.Rate = source.rate;
            _retValue.IsActive = source.isActive;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;

            return _retValue;
        }

        /// <summary>
        /// Returns TaxType object from the TaxTypeData provided 
        /// </summary>
        /// <param name="source">TaxTypeData object</param>
        /// <returns></returns>
        public static TaxType CreateTaxType(TaxTypeData source)
        {
            if (source == null)
                return null;
            var _taxType = new TaxType()
            {
                lkTaxTypeID = source.TaxTypeID,
                taxTypeName = source.TaxTypeName,
                rate = source.Rate,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            return _taxType;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.TaxTypes.Count(b => !(b.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        /// <summary>
        /// Checks if the new taxType name already exists
        /// </summary>
        /// <param name="name">The new taxType name to be added</param>
        /// <returns>true if the new taxType name does not exist. False, otherwise</returns>
        public bool IsValid(string name)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    return (_context.TaxTypes.Where(b => b.taxTypeName.Equals(name) && !(b.isDeleted ?? false)).Count() > 0 ? false : true);
                }
            }

            return false;
        }

        #endregion

        //public static IEnumerable<TaxTypeData> GetTaxTypes()
        //{
        //    using (var db = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _taxTypes = (from tt in db.TaxTypes
        //                         where tt.isActive && !(tt.isDeleted ?? false)
        //                         select new TaxTypeData()
        //                         {
        //                             TaxTypeID = tt.lkTaxTypeID,
        //                             TaxTypeName = tt.taxTypeName,
        //                             Rate = tt.rate,
        //                             IsActive = tt.isActive
        //                         }).ToList();

        //        return _taxTypes;
        //    }
        //}

        public static TaxTypeData GetTaxType(int typeId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _taxType = (from tt in db.TaxTypes where tt.isActive && !(tt.isDeleted ?? false) && tt.lkTaxTypeID == typeId select tt).SingleOrDefault();

                if (_taxType == null) return null;
                return new TaxTypeData()
                {
                    TaxTypeID = _taxType.lkTaxTypeID,
                    TaxTypeName = _taxType.taxTypeName,
                    Rate = _taxType.rate,
                    IsActive = _taxType.isActive
                };
            }
        }
        
        public static TaxTypeData GetTaxType(string taxType)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _taxType = (from tt in db.TaxTypes
                                where tt.isActive && !(tt.isDeleted ?? false)
                                && tt.taxTypeName.ToLower().Equals(taxType.ToLower())
                                select tt).SingleOrDefault();

                if (_taxType == null) return null;
                return new TaxTypeData()
                {
                    TaxTypeID = _taxType.lkTaxTypeID,
                    TaxTypeName = _taxType.taxTypeName,
                    Rate = _taxType.rate,
                    IsActive = _taxType.isActive
                };
            }
        }
    }

    public class TaxTypeData
    {
        public int TaxTypeID { get; set; }
        public string TaxTypeName { get; set; }
        public double Rate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }

        public string TaxType { get; set; }
    }
}
