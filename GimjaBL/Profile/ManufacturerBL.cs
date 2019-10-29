using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace GimjaBL
{
    public class ManufacturerBL
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

        public ManufacturerBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts Manufacturer data to tblManufacturer
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(ManufacturerData manufacturerData, bool isSync = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    if (!isSync && manufacturerData !=null)
                    {
                        string[] _splitted = manufacturerData.Name.Split(null);

                        string _namePrefix = String.Empty;

                        foreach (var _splName in _splitted)
                            _namePrefix = _namePrefix + _splName[0].ToString();

                        var _newManufacturerID = _context.GenerateID("lkManufacturer", _namePrefix).SingleOrDefault();

                        if (_newManufacturerID == null)
                            throw new InvalidOperationException("Manufacturer ID could not be generated");

                        manufacturerData.ManufacturerID = _newManufacturerID.id;
                    }

                    var _manufacturer = CreateManufacturer(manufacturerData);

                    if (!isSync && _manufacturer != null)
                    {
                        var _manufacturerElem = SyncTransactionBL.GetManufacturerElement(_manufacturer);
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            action = "insert",
                            tableName = "lkManufacturer",
                            value = _manufacturerElem.ToString(),
                            branchID = "BRC-B-1"//TODO: ADD THE CURRENT BRANCH
                        };
                        _context.SyncTransactions.Add(_sync);
                    }

                    _context.Manufacturers.Add(_manufacturer);

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
        /// Updates Manufacturer data on tblManufacturer
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(ManufacturerData manufacturerData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _manufacturer = _context.Manufacturers.Single(i => i.lkManufacturerID == manufacturerData.ManufacturerID);

                if (_manufacturer == null)
                    throw new InvalidOperationException("Manufacturer detail could not be found.");

                //sets the new manufacturer data
                _manufacturer.name = manufacturerData.Name;
                _manufacturer.contactPerson = manufacturerData.ContactPerson;
                _manufacturer.description = manufacturerData.Description;
                _manufacturer.isActive = manufacturerData.IsActive;

                _manufacturer.lastUpdatedBy=Singleton.Instance.UserID;
                _manufacturer.lastUpdatedDate = DateTime.Now;

                //sets address data to be added/updated
                if (manufacturerData.Address != null)
                {
                    var _existingAddress = _context.Addresses.FirstOrDefault(a => a.id == manufacturerData.Address.ID);
                    if (_existingAddress != null)
                    {
                        _existingAddress.kebele = manufacturerData.Address.Kebele;
                        _existingAddress.woreda = manufacturerData.Address.Woreda;
                        _existingAddress.subcity = manufacturerData.Address.Subcity;
                        _existingAddress.city_town = manufacturerData.Address.City_Town;
                        _existingAddress.street = manufacturerData.Address.Street;
                        _existingAddress.houseNo = manufacturerData.Address.HouseNo;
                        _existingAddress.pobox = manufacturerData.Address.PoBox;
                        _existingAddress.primaryEmail = manufacturerData.Address.PrimaryEmail;
                        _existingAddress.secondaryEmail = manufacturerData.Address.SecondaryEmail;
                        _existingAddress.state_region = manufacturerData.Address.State_Region;
                        _existingAddress.country = manufacturerData.Address.Country;
                        _existingAddress.zipCode_postalCode = manufacturerData.Address.ZipCode_PostalCode;
                        _existingAddress.additionalInfo = manufacturerData.Address.AdditionalInfo;
                        _existingAddress.lastUpdatedBy = Singleton.Instance.UserID;
                        
                        _existingAddress.lastUpdatedDate = DateTime.Now;
                    }
                    else
                    {
                        //address is new; insert it
                        var _address = AddressBL.CreateAddressObject(manufacturerData.Address);
                        _manufacturer.addressID = _address.id;
                        _context.Addresses.Add(_address);
                    }
                }

                //sets telephone/fax data to be added/updated
                if (manufacturerData.TelephoneFax != null)
                {
                    foreach (var _telFax in manufacturerData.TelephoneFax)
                    {
                        var _tf = _context.TelephoneFaxes.FirstOrDefault(tf => tf.parentID == manufacturerData.ManufacturerID && tf.id == _telFax.ID && _telFax.ID != 0);

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
                                ParentID = _manufacturer.lkManufacturerID,

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

        /// <summary>
        /// Deletes Manufacturer data from tblManufacturer table
        /// </summary>
        /// <param name="manufacturerID">The current manufacturer's ManufacturerID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(string manufacturerID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _manufacturer = _context.Manufacturers.SingleOrDefault(c => c.lkManufacturerID == manufacturerID);

                if (_manufacturer == null)
                    throw new InvalidOperationException("The manufacturer you are trying to delete doesn't exist");

                _manufacturer.isDeleted = true;
                _manufacturer.lastUpdatedBy = Singleton.Instance.UserID;
                _manufacturer.lastUpdatedDate = DateTime.Now;

                var _address = _context.Addresses.SingleOrDefault(a=>a.id == _manufacturer.addressID );
                if (_address != null)
                {
                    _address.isDeleted = true;
                    _address.lastUpdatedBy = Singleton.Instance.UserID;
                    _address.lastUpdatedDate = DateTime.Now;
                }

                var _telephone = _context.TelephoneFaxes.Where(t => t.parentID == _manufacturer.lkManufacturerID).ToList();
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
        /// Returns Manufacturer data from tblManufacturer table
        /// </summary>
        /// <returns>IEnumerable ManufacturerData</returns>
        public IEnumerable<ManufacturerData> GetData(bool isActive= false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _manufacturer;

                if (isActive)
                {
                    _manufacturer = _context.Manufacturers.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).ToList();
                }
                else
                {
                    _manufacturer = _context.Manufacturers.Where(b => !(b.isDeleted ?? false)).ToList();
                }

                List<ManufacturerData> _result = new List<ManufacturerData>();
                foreach (var _r in _manufacturer)
                {
                    _result.Add(CreateManufacturerData(_r));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates ManufacturerData to be return for GetData call
        /// </summary>
        /// <returns>ManufacturerData</returns>
        internal ManufacturerData CreateManufacturerData(Manufacturer source)
        {
            if (source == null)
                return null;

            ManufacturerData _retValue = new ManufacturerData();

            _retValue.ManufacturerID = source.lkManufacturerID;
            _retValue.Name = source.name;
            _retValue.ContactPerson = source.contactPerson;
            _retValue.Description = source.description;
            _retValue.AddressID = source.addressID;
            _retValue.IsActive = source.isActive;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;

            return _retValue;
        }

        /// <summary>
        /// Returns Manufacturer object from the ManufacturerData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Manufacturer CreateManufacturer(ManufacturerData source)
        {
            if (source == null)
                return null;
            var _manufacturer = new Manufacturer()
            {
                lkManufacturerID = source.ManufacturerID,
                name = source.Name,
                addressID = source.AddressID,
                contactPerson = source.ContactPerson,
                description = source.Description,
                isActive = source.IsActive,
                createdBy = source.CreatedBy,
                createdDate = source.CreatedDate
                };

            AddressData _customerAddress = source.Address;
            if (_customerAddress != null)
            {
                var _address = AddressBL.CreateAddressObject(_customerAddress);
                _manufacturer.Address = _address;
            }
            IEnumerable<TelephoneFaxData> _telephoneFaxes = source.TelephoneFax;

            if (_telephoneFaxes != null)
            {
                foreach (var tele in _telephoneFaxes)
                {
                    tele.ParentID = _manufacturer.lkManufacturerID;
                    var telephoneFax = TelephoneFaxBL.CreateTelephoneFaxObject(tele);
                    _manufacturer.TelephoneFaxes.Add(telephoneFax);
                }
            }

            return _manufacturer;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Manufacturers.Count(i => !(i.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        public static IEnumerable<ManufacturerData> GetActiveManufacturers()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _customers = (from c in db.Manufacturers
                                  where c.isActive && !(c.isDeleted ?? false)
                                  select c).ToList();

                List<ManufacturerData> _result = new List<ManufacturerData>();
                _customers.ForEach(c => _result.Add(CreateManufacturerObject(c)));

                return _result;
            }
        }

        private static ManufacturerData CreateManufacturerObject(Manufacturer c)
        {
            return new ManufacturerData
            {
                ManufacturerID = c.lkManufacturerID,
                Name = c.name,
                ContactPerson = c.contactPerson,
                Description = c.description,
                AddressID = c.addressID,
                IsActive = c.isActive
            };
        }

        /// <summary>
        /// Checks whether the manufacturer record exists in database
        /// </summary>
        /// <param name="manufacturerID">the manufacturer ID</param>
        /// <returns></returns>
        public bool Exists(string manufacturerID)
        {
            if (String.IsNullOrWhiteSpace(manufacturerID))
                return false;
            using (var db = new eDMSEntity("eDMSEntities"))
            {
                var _exists = db.Manufacturers.Any(m => m.lkManufacturerID == manufacturerID);

                return _exists;
            }
        }

        #endregion
    }
    public class ManufacturerData
    {
        public string ManufacturerID { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Description { get; set; }
        public Guid? AddressID { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public AddressData Address { get; set; }
        public IEnumerable<TelephoneFaxData> TelephoneFax { get; set; }
    }
}
