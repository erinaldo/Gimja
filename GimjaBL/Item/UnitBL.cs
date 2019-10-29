using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class UnitBL
    {
        #region Member Variables
        //private int unitID;
        //private string unitName;
        //private string description;
        //private string createdBy;
        //private DateTime createdDate;
        //private string lastUpdatedBy;
        //private DateTime? lastUpdatedDate;
        //private bool isActive;
        //private bool? isDeleted;

        private bool isUpdate;
        private bool isDataAvailable;

        //Unit unit;

        #endregion

        public UnitBL()
        {
            //unit = new Unit();
            HasData();
        }

        #region Properties

        //public int UnitID
        //{
        //    get { return unitID; }
        //    set { unitID = value; }
        //}

        //public string UnitName
        //{
        //    get { return unitName; }
        //    set { unitName = value; }
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

        #region Method

        /// <summary>
        /// Inserts UnitData to lkUnit
        /// </summary>
        /// <param name="unitData"></param>
        /// <returns></returns>
        public bool Add(UnitData unitData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _unit = CreateUnit(unitData);
                    _context.Units.Add(_unit);

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
        /// Updates Unit data on lkUnit
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(UnitData unitData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _unitData = _context.Units.Single(tf => tf.lkUnitID == unitData.UnitID);

                if (_unitData == null)
                    throw new InvalidOperationException("Unit detail could not be found.");

                //sets the new unit data
                _unitData.unitName = unitData.UnitName;
                _unitData.description = unitData.Description;
                _unitData.isActive = unitData.IsActive;

                _unitData.lastUpdatedBy = Singleton.Instance.UserID;
                _unitData.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Deletes Unit data from lkUnit table
        /// </summary>
        /// <param name="unitID">The current unit's UnitID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(int unitID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _unit = _context.Units.SingleOrDefault(c => c.lkUnitID == unitID);

                if (_unit == null)
                    throw new InvalidOperationException("The unit you are trying to delete doesn't exist");

                _unit.isDeleted = true;
                _unit.lastUpdatedBy = Singleton.Instance.UserID;
                _unit.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Returns Unit data from lkUnit table
        /// </summary>
        /// <returns>IEnumerable UnitData</returns>
        public IEnumerable<UnitData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _unit;

                if (isActive)
                {
                    _unit = _context.Units.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).OrderBy(r => r.unitName).ToList();
                }
                else
                {
                    _unit = _context.Units.Where(b => !(b.isDeleted ?? false)).OrderBy(r => r.unitName).ToList();
                }

                List<UnitData> _result = new List<UnitData>();
                foreach (var _b in _unit)
                {
                    _result.Add(CreateUnitData(_b));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates UnitData to be return for GetData call
        /// </summary>
        /// <param name="source"></param>
        /// <returns>UnitData</returns>
        internal UnitData CreateUnitData(Unit source)
        {
            if (source == null)
                return null;

            UnitData _retValue = new UnitData();
            _retValue.UnitID = source.lkUnitID;
            _retValue.UnitName = source.unitName;
            _retValue.Description = source.description;
            _retValue.IsActive = source.isActive;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;

            return _retValue;
        }

        /// <summary>
        /// Returns Unit object from the UnitData provided 
        /// </summary>
        /// <param name="source">UnitData object</param>
        /// <returns></returns>
        public static Unit CreateUnit(UnitData source)
        {
            if (source == null)
                return null;
            var _unit = new Unit()
            {
                lkUnitID = source.UnitID,
                unitName = source.UnitName,
                description = source.Description,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            return _unit;
        }


        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Units.Count(b => !(b.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        /// <summary>
        /// Checks if the new unit name already exists
        /// </summary>
        /// <param name="name">The new unit name to be added</param>
        /// <returns>true if the new unit name does not exist. False, otherwise</returns>
        public bool IsValid(string name)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    return (_context.Units.Where(b => b.unitName.Equals(name) && !(b.isDeleted ?? false)).Count() > 0 ? false : true);
                }
            }

            return false;
        }

        #endregion

    }

    public class UnitData
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
