using System;
using System.Linq;
using GimjaDL;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace GimjaBL
{
    public class MenuTypeBL
    {
        #region Member Variables

        private bool isMenuTypeUpdate;
        private bool isDataAvailable;

        #endregion

        #region Properties

        public bool IsMenuTypeUpdate
        {
            get { return isMenuTypeUpdate; }
            set { isMenuTypeUpdate = value; }
        }

        public bool IsDataAvailable
        {
            get { return isDataAvailable; }
            set { isDataAvailable = value; }
        }

        #endregion

        public MenuTypeBL()
        {
            HasData();
        }

        #region methods
      
        /// <summary>
       /// inserts Menu Type data into the database
       /// </summary>
       /// <param name="menuTypeData"></param>
       /// <returns></returns>
        public bool Add(MenuTypeData menuTypeData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _menuType = CreateMenuType(menuTypeData);
                    _context.MenuTypes.Add(_menuType);

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
        /// Updates the specified menutype
        /// </summary>
        public bool Update(MenuTypeData menuTypeData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _menuType = _context.MenuTypes.Single(mt => mt.lkMenuTypeID == menuTypeData.MenuTypeID);

                if (_menuType == null)
                    throw new InvalidOperationException("Menu Type could not be found.");

                //sets the new Menu Type data
                _menuType.type = menuTypeData.Type;
                _menuType.parent = menuTypeData.Parent;
                _menuType.isActive = menuTypeData.IsActive;

                //TODO: assign the loggedin user
                //_menuType.lastUpdatedBy=;
                _menuType.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Deletes menu type data from the database
        /// </summary>
        /// <param name="menuTypeID"></param>
        /// <returns>true if the data is deleted successfully; false otherwise</returns>
        public bool Delete(int menuTypeID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _menuType = _context.MenuTypes.Single(mt => mt.lkMenuTypeID == menuTypeID);
                _menuType.isDeleted = true;

                int row = _context.SaveChanges();
                return row > 0;
            }
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.MenuTypes.Count(mt => !(mt.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        /// <summary>
        /// Returns Menu Typee data from the database
        /// </summary>
        /// <returns>IEnumerable MenuTypeData</returns>
        public IEnumerable<MenuTypeData> GetData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _menuTypes = (from mt in _context.MenuTypes
                                  where !(mt.isDeleted ?? false)
                                  select new MenuTypeData
                                  {
                                      MenuTypeID = mt.lkMenuTypeID,
                                      Type = mt.type,
                                      Parent = mt.parent,
                                      ParentCaption = (from p in _context.MenuTypes where p.lkMenuTypeID == mt.parent select p.type).FirstOrDefault(),
                                      IsActive = mt.isActive,
                                      CreatedBy = mt.createdBy,
                                      CreatedDate = mt.createdDate,
                                      LastUpdatedBy = mt.lastUpdatedBy,
                                      LastUpdatedDate = mt.lastUpdatedDate,
                                      IsDeleted = mt.isDeleted
                                  }).ToList();

                return _menuTypes;
            }
        }

        /// <summary>
        /// Creates MenuTypeData to be returned to GetData call
        /// </summary>
        /// <returns>MenuTypeData</returns>
        internal MenuTypeData CreateMenuTypeData(MenuType source)
        {
            if (source == null)
                return null;

            MenuTypeData _retValue = new MenuTypeData();

            _retValue.MenuTypeID = source.lkMenuTypeID;
            _retValue.Type = source.type;
            _retValue.Parent = source.parent;
            _retValue.IsActive = source.isActive;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;

            return _retValue;
        }

        /// <summary>
        /// Returns MenuType object from the MenuTypeData provided 
        /// </summary>
        /// <param name="source">MenuTypeData</param>
        /// <returns>Menu Type</returns>
        internal MenuType CreateMenuType(MenuTypeData source)
        {
            if (source == null)
                return null;
            var _menuType = new MenuType()
            {
                type = source.Type,
                parent = source.Parent,
                isActive = source.IsActive,
                createdBy = source.CreatedBy,
                createdDate = source.CreatedDate
            };

            return _menuType;
        }

        /// <summary>
        /// returns true if the specified menuType name already exists in the database
        /// </summary>
        /// <param name="type">menu type name</param>
        /// <returns></returns>
        public bool Validate(string type)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                return _context.MenuTypes.Where(mt => mt.type.Equals(type) && !(mt.isDeleted ?? false)).Count() > 0 ? false : true;
            }
        }

        /// <summary>
        /// Returns active Menu Types from the database
        /// </summary>
        /// <returns>IEnumerable MenuTypeData</returns>
        public IEnumerable<MenuTypeData> GetActiveMenuTypes()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _menuTypes = _context.MenuTypes.Where(mt => !(mt.isDeleted ?? false) && mt.isActive).ToList();
                List<MenuTypeData> result = new List<MenuTypeData>();
                foreach (var _mt in _menuTypes)
                {
                    result.Add(CreateMenuTypeData(_mt));
                }
                return result;
            }
        }


        #endregion
    }

    public class MenuTypeData
    {
        public int MenuTypeID { get; set; }
        public string Type { get; set; }
        public Nullable<int> Parent { get; set; }
        public string ParentCaption { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
