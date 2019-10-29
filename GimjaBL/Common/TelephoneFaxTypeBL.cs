using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class TelephoneFaxTypeBL
    {    
        #region Member Variables
        private bool isUpdate;
        private bool isDataAvailable;
        #endregion

        #region Properties

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
        #endregion

        public TelephoneFaxTypeBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts TelephoneFaxType data to lkTelephoneFaxType
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(TelephoneFaxTypeData telephoneFaxTypeData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _telephoneFaxType = CreateTelephoneFax(telephoneFaxTypeData);
                    _context.TelephoneFaxTypes.Add(_telephoneFaxType);

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
        /// Updates TelephoneFaxType data on lkTelephoneFaxType
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(TelephoneFaxTypeData telephoneFaxTypeData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _telephoneFaxType = _context.TelephoneFaxTypes.Single(tf => tf.lkTelephoneFaxTypeID == telephoneFaxTypeData.TelephoneFaxTypeID);

                if (_telephoneFaxType == null)
                    throw new InvalidOperationException("TelephoneFaxType detail could not be found.");

                //sets the new telephoneFax data
                _telephoneFaxType.name = telephoneFaxTypeData.Name;
                _telephoneFaxType.isActive = telephoneFaxTypeData.IsActive;

                _telephoneFaxType.lastUpdatedBy = Singleton.Instance.UserID;
                _telephoneFaxType.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Deletes TelephoneFaxType data from lkTelephoneFaxType table
        /// </summary>
        /// <param name="telephoneFaxID">The current telephoneFax's TelephoneFaxTypeID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(int roleID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _telephoneFaxType = _context.TelephoneFaxTypes.SingleOrDefault(c => c.lkTelephoneFaxTypeID == roleID);

                if (_telephoneFaxType == null)
                    throw new InvalidOperationException("The role you are trying to delete doesn't exist");

                _telephoneFaxType.isDeleted = true;
                _telephoneFaxType.lastUpdatedBy = Singleton.Instance.UserID;
                _telephoneFaxType.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Returns TelephoneFaxType data from lkTelephoneFaxType table
        /// </summary>
        /// <returns>IEnumerable TelephoneFaxTypeData</returns>
        public IEnumerable<TelephoneFaxTypeData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _telephoneFaxType;

                if (isActive)
                {
                    _telephoneFaxType = _context.TelephoneFaxTypes.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).OrderBy(r=>r.name).ToList();
                }
                else
                {
                    _telephoneFaxType = _context.TelephoneFaxTypes.Where(b => !(b.isDeleted ?? false)).OrderBy(r => r.name).ToList();
                }

                List<TelephoneFaxTypeData> _result = new List<TelephoneFaxTypeData>();
                foreach (var _r in _telephoneFaxType)
                {
                    _result.Add(CreateTelephoneFaxData(_r));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates TelephoneFaxTypeData to be return for GetData call
        /// </summary>
        /// <returns>TelephoneFaxTypeData</returns>
        internal TelephoneFaxTypeData CreateTelephoneFaxData(TelephoneFaxType source)
        {
            if (source == null)
                return null;

            TelephoneFaxTypeData _retValue = new TelephoneFaxTypeData();

            _retValue.TelephoneFaxTypeID = source.lkTelephoneFaxTypeID;
            _retValue.Name = source.name;
            _retValue.IsActive = source.isActive;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;
            return _retValue;
        }

        /// <summary>
        /// Returns TelephoneFaxType object from the TelephoneFaxTypeData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TelephoneFaxType CreateTelephoneFax(TelephoneFaxTypeData source)
        {
            if (source == null)
                return null;
            var _telephoneFaxType = new TelephoneFaxType()
            {
                lkTelephoneFaxTypeID = source.TelephoneFaxTypeID,
                name =  source.Name,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            return _telephoneFaxType;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.TelephoneFaxTypes.Count(tf => !(tf.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        ///// <summary>
        ///// Returns the active telephone/fax types
        ///// </summary>
        ///// <returns></returns>
        //public static IEnumerable<TelephoneFaxTypeData> GetActiveTelephoneFaxType()
        //{
        //    using (var db = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _telephoneFaxTypes = (from c in db.TelephoneFaxTypes
        //                          where c.isActive && !(c.isDeleted ?? false)
        //                          select c).ToList();

        //        List<TelephoneFaxTypeData> _result = new List<TelephoneFaxTypeData>();
        //        _telephoneFaxTypes.ForEach(c => _result.Add(CreateTelephoneFaxTypeObject(c)));

        //        return _result;
        //    }
        //}

        /// <summary>
        /// Creates TelephoneFaxType object
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static TelephoneFaxTypeData CreateTelephoneFaxTypeObject(TelephoneFaxType c)
        {
            return new TelephoneFaxTypeData
            {
                TelephoneFaxTypeID = c.lkTelephoneFaxTypeID,
                Name = c.name,
                IsActive = c.isActive
            };
        }

        /// <summary>
        /// Checks if the new telephoneFax name already exists
        /// </summary>
        /// <param name="name">The new telephoneFax name to be added</param>
        /// <returns>true if the new telephoneFax name does not exist. False, otherwise</returns>
        public bool IsValid(string name)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    return (_context.TelephoneFaxTypes.Where(r=>r.name.Equals(name) && !(r.isDeleted ?? false)).Count()>0 ? false : true);
                }
            }

            return false;
        }
        
        #endregion
    }


    public class TelephoneFaxTypeData
    {
        public short TelephoneFaxTypeID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
    }
}
