using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class WarehouseBL
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

        public WarehouseBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts Warehouse data to tblWarehouse
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(WarehouseData warehouseData, bool isSync = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    if (!isSync)
                    {
                        string[] _splitted = warehouseData.Name.Split(null);

                        string _namePrefix = String.Empty;

                        foreach (var _splName in _splitted)
                            _namePrefix = _namePrefix + _splName[0].ToString();

                        var _newWarehouseID = _context.GenerateID("lkWarehouse", _namePrefix).SingleOrDefault();

                        if (_newWarehouseID == null)
                            throw new InvalidOperationException("Warehouse WarehouseID could not be generated");

                        warehouseData.WarehouseID = _newWarehouseID.id;
                    }

                    var _warehouse = CreateWarehouse(warehouseData);

                    if (!isSync && _warehouse != null)
                    {
                        var _warehouseElem = SyncTransactionBL.GetWarehouseElement(_warehouse);
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            action = "insert",
                            tableName = "lkWarehouse",
                            value = _warehouseElem.ToString(),
                            branchID = "BRC-B-1"//TODO: ADD THE CURRENT BRANCH
                        };
                        _context.SyncTransactions.Add(_sync);
                    }

                    _context.Warehouses.Add(_warehouse);

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
        /// Updates Warehouse data on tblWarehouse
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(WarehouseData warehouseData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _warehouse = _context.Warehouses.Single(i => i.lkWarehouseID == warehouseData.WarehouseID);

                if (_warehouse == null)
                    throw new InvalidOperationException("Warehouse detail could not be found.");

                //sets the new warehouse data
                _warehouse.name = warehouseData.Name;
                _warehouse.description = warehouseData.Description;
                _warehouse.isActive = warehouseData.IsActive;
                _warehouse.lastUpdatedBy = Singleton.Instance.UserID;
                _warehouse.lastUpdatedDate = DateTime.Now;

                //sets address data to be added/updated
                if (warehouseData.Address != null)
                {
                    var _existingAddress = _context.Addresses.FirstOrDefault(a => a.id == warehouseData.Address.ID);
                    if (_existingAddress != null)
                    {
                        _existingAddress.kebele = warehouseData.Address.Kebele;
                        _existingAddress.woreda = warehouseData.Address.Woreda;
                        _existingAddress.subcity = warehouseData.Address.Subcity;
                        _existingAddress.city_town = warehouseData.Address.City_Town;
                        _existingAddress.street = warehouseData.Address.Street;
                        _existingAddress.houseNo = warehouseData.Address.HouseNo;
                        _existingAddress.pobox = warehouseData.Address.PoBox;
                        _existingAddress.primaryEmail = warehouseData.Address.PrimaryEmail;
                        _existingAddress.secondaryEmail = warehouseData.Address.SecondaryEmail;
                        _existingAddress.state_region = warehouseData.Address.State_Region;
                        _existingAddress.country = warehouseData.Address.Country;
                        _existingAddress.zipCode_postalCode = warehouseData.Address.ZipCode_PostalCode;
                        _existingAddress.additionalInfo = warehouseData.Address.AdditionalInfo;
                        _existingAddress.lastUpdatedBy = Singleton.Instance.UserID;

                        _existingAddress.lastUpdatedDate = DateTime.Now;
                    }
                    else
                    {
                        //address is new; insert it
                        var _address = AddressBL.CreateAddressObject(warehouseData.Address);
                        _warehouse.addressID = _address.id;
                        _context.Addresses.Add(_address);
                    }
                }

                //sets telephone/fax data to be added/updated
                if (warehouseData.TelephoneFax != null)
                {
                    foreach (var _telFax in warehouseData.TelephoneFax)
                    {
                        var _tf = _context.TelephoneFaxes.FirstOrDefault(tf => tf.parentID == warehouseData.WarehouseID && tf.id == _telFax.ID && _telFax.ID != 0);

                        if (_tf != null) //update telephoneFax
                        {
                            _tf.type = _telFax.Type;
                            _tf.number = _telFax.Number;
                            _tf.isActive = _telFax.IsActive;
                            _tf.lastUpdatedBy = Singleton.Instance.UserID;
                            _tf.lastUpdatedDate = DateTime.Now;
                        }
                        else //telephoneFax is new, insert it
                        {
                            var _telephoneFaxes = new TelephoneFaxData()
                            {
                                Type = _telFax.Type,
                                Number = _telFax.Number,
                                IsActive = _telFax.IsActive,
                                ParentID = _warehouse.lkWarehouseID,

                                CreatedBy = Singleton.Instance.UserID,
                                CreatedDate = DateTime.Now
                            };

                            var _telephoneFax = TelephoneFaxBL.CreateTelephoneFaxObject(_telephoneFaxes);
                            _context.TelephoneFaxes.Add(_telephoneFax);
                        }
                    }
                }

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

         ///<summary>
         ///Deletes Warehouse data from tblWarehouse table
         ///</summary>
         ///<param name="warehouseID">The current warehouse's WarehouseID</param>
         ///<returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(string warehouseID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _warehouse = _context.Warehouses.SingleOrDefault(c => c.lkWarehouseID == warehouseID);

                if (_warehouse == null)
                    throw new InvalidOperationException("The warehouse you are trying to delete doesn't exist");

                _warehouse.isDeleted = true;
                _warehouse.lastUpdatedBy = Singleton.Instance.UserID;
                _warehouse.lastUpdatedDate = DateTime.Now;

                var _address = _context.Addresses.SingleOrDefault(a => a.id == _warehouse.addressID);
                if (_address != null)
                {
                    _address.isDeleted = true;
                    _address.lastUpdatedBy = Singleton.Instance.UserID;
                    _address.lastUpdatedDate = DateTime.Now;
                }

                var _telephone = _context.TelephoneFaxes.Where(t => t.parentID == _warehouse.lkWarehouseID).ToList();
                if (_telephone != null)
                {
                    foreach (var _tel in _telephone)
                    {
                        _tel.isDeleted = true;
                        _tel.lastUpdatedBy = Singleton.Instance.UserID;
                        _tel.lastUpdatedDate = DateTime.Now;
                    }
                }

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Returns Warehouse data from tblWarehouse table
        /// </summary>
        /// <returns>IEnumerable WarehouseData</returns>
        public IEnumerable<WarehouseData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {

                var _warehouse = isActive == true ? _context.Warehouses.Where(w=>!(w.isDeleted ?? false) && w.isActive).ToList() : _context.Warehouses.Where(w=>!(w.isDeleted ?? false)).ToList();

                List<WarehouseData> _result = new List<WarehouseData>();
                foreach (var _w in _warehouse)
                {
                    _result.Add(CreateWarehouseData(_w));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates WarehouseData to be return for GetData call
        /// </summary>
        /// <returns>WarehouseData</returns>
        internal WarehouseData CreateWarehouseData(Warehouse source)
        {
            if (source == null)
                return null;

            WarehouseData _retValue = new WarehouseData();

            _retValue.WarehouseID = source.lkWarehouseID;
            _retValue.Name = source.name;
            _retValue.Description = source.description;
            _retValue.IsActive = source.isActive;
            _retValue.AddressID = source.addressID;
            _retValue.IsDefault = source.isDefault;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;

            return _retValue;
        }

        /// <summary>
        /// Returns Warehouse object from the WarehouseData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Warehouse CreateWarehouse(WarehouseData source)
        {
            if (source == null)
                return null;
            var _warehouse = new Warehouse()
            {
                lkWarehouseID = source.WarehouseID,
                name = source.Name,
                addressID = source.AddressID,
                description = source.Description,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            AddressData _warehouseAddress = source.Address;
            if (_warehouseAddress != null)
            {
                var _address = AddressBL.CreateAddressObject(_warehouseAddress);
                _warehouse.Address = _address;
            }
            IEnumerable<TelephoneFaxData> _telephoneFaxes = source.TelephoneFax;

            if (_telephoneFaxes != null)
            {
                foreach (var tele in _telephoneFaxes)
                {
                    tele.ParentID = _warehouse.lkWarehouseID;
                    var telephoneFax = TelephoneFaxBL.CreateTelephoneFaxObject(tele);
                    _warehouse.TelephoneFaxes.Add(telephoneFax);
                }
            }

            return _warehouse;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Warehouses.Where(w => !(w.isDeleted ?? false)).Count() == 0 ? false : true);
            }
        }

        //private static WarehouseData CreateWarehouseObject(Warehouse c)
        //{
        //    return new WarehouseData
        //    {
        //        WarehouseID = c.lkWarehouseID,
        //        Name = c.name,
        //        Description = c.description,
        //        IsActive = c.isActive,
        //        AddressID = c.addressID
        //    };
        //}

        //public static IList<WarehouseData> GetWarehouses()
        //{
        //    using (var db = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _warehouses = (from w in db.Warehouses
        //                           where w.isActive && !(w.isDeleted ?? false)
        //                           select new WarehouseData
        //                           {
        //                               WarehouseID = w.lkWarehouseID,
        //                               Name = w.name,
        //                               Description = w.description,
        //                               IsActive = w.isActive,
        //                               CreatedBy = w.createdBy,
        //                               CreatedDate = w.createdDate,
        //                               LastUpdatedBy = w.lastUpdatedBy,
        //                               LastUpdatedDate = w.lastUpdatedDate,
        //                               IsDeleted = w.isDeleted
        //                           }).ToList();

        //        return _warehouses;
        //    }
        //}

        /// <summary>
        /// Returns list of warehouses that are active and not set to default
        /// </summary>
        /// <returns>Returns true when warehouse is set to default. Fasle, otherwise</returns>
        public IEnumerable<WarehouseData> GetActiveNotDefaultWarehouses()
        {
            try
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    var _warehouse = _context.Warehouses.Where(w => !(w.isDeleted ?? false) && w.isActive && w.isDefault==false).ToList();

                    List<WarehouseData> _result = new List<WarehouseData>();
                    foreach (var _w in _warehouse)
                    {
                        _result.Add(CreateWarehouseData(_w));
                    }

                    return _result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the default warehouse
        /// </summary>
        /// <param name="warehouseID">the warehouseID to be set default warehouse</param>
        /// <returns>Returns true when warehouse is set to default. Fasle, otherwise</returns>
        public bool SetDefault(string warehouseID)
        {
            try
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    var _warehouse = _context.Warehouses.SingleOrDefault(w => w.lkWarehouseID == warehouseID && w.isActive && !(w.isDeleted ?? false));

                    if (_warehouse == null)
                        throw new InvalidOperationException("Default could not be set. Try again!");

                    var _alreadyDefaultWarehouse = _context.Warehouses.SingleOrDefault(w => w.isActive && !(w.isDeleted ?? false) && w.isDefault);
                    if (_alreadyDefaultWarehouse != null)
                        _alreadyDefaultWarehouse.isDefault = false;

                    _warehouse.isDefault = true;
                    int _row = _context.SaveChanges();
                    return _row > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the name of the new warehouse is already taken
        /// </summary>
        /// <param name="warehouseName">The name of the new warehouse</param>
        /// <returns>True if name is available. False, otherwise</returns>
        public bool IsValid(string warehouseName)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                return _context.Warehouses.Where(w => !(w.isDeleted ?? false) && w.name == warehouseName).Count() > 0 ? false : true;
            }
        }


        /// <summary>
        /// Checks whether the warehouse record exists in database
        /// </summary>
        /// <param name="warehouseID">the warehouse ID</param>
        /// <returns></returns>
        public bool Exists(string warehouseID)
        {
            if (String.IsNullOrWhiteSpace(warehouseID))
                return false;
            using (var db = new eDMSEntity("eDMSEntities"))
            {
                var _exists = db.Warehouses.Any(w => w.lkWarehouseID == warehouseID);

                return _exists;
            }
        }
        #endregion
    }

    public class WarehouseData
    {
        public string WarehouseID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.Guid> AddressID { get; set; }
        public bool IsDefault { get; set; }

        public AddressData Address { get; set; }
        public IEnumerable<TelephoneFaxData> TelephoneFax { get; set; }
    }
}
