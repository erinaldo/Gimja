using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class SupplierBL
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

        public SupplierBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts Supplier data to tblSupplier
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(SupplierData supplierData, bool isSync = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    if (!isSync && supplierData!=null)
                    {
                        string[] _splitted = supplierData.CompanyName.Split(null);

                        string _namePrefix = String.Empty;

                        foreach (var _splName in _splitted)
                            _namePrefix = _namePrefix + _splName[0].ToString();

                        var _newSupplierID = _context.GenerateID("lkSupplier", _namePrefix).SingleOrDefault();

                        if (_newSupplierID == null)
                            throw new InvalidOperationException("Supplier ID could not be generated");

                        supplierData.SupplierID = _newSupplierID.id;
                    }

                    var _supplier = CreateSupplier(supplierData);

                    if (!isSync && _supplier != null)
                    {
                        var _supplierElem = SyncTransactionBL.GetSupplierElement(_supplier);
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            action = "insert",
                            tableName = "lkSupplier",
                            value = _supplierElem.ToString(),
                            branchID = "BRC-B-1" //TODO: ADD THE CURRENT BRANCH
                        };
                        _context.SyncTransactions.Add(_sync);
                    }
                    _context.Suppliers.Add(_supplier);

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
        /// Updates Supplier data on tblSupplier
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(SupplierData supplierData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _supplier = _context.Suppliers.Single(i => i.lkSupplierID == supplierData.SupplierID);

                if (_supplier == null)
                    throw new InvalidOperationException("Supplier detail could not be found.");

                //sets the new supplier data
                _supplier.companyName = supplierData.CompanyName;
                _supplier.contactPerson = supplierData.ContactPerson;
                _supplier.description = supplierData.Description;
                _supplier.isActive = supplierData.IsActive;

                _supplier.lastUpdatedBy = Singleton.Instance.UserID;
                _supplier.lastUpdatedDate = DateTime.Now;

                //sets address data to be added/updated
                if (supplierData.Address != null)
                {
                    var _existingAddress = _context.Addresses.FirstOrDefault(a => a.id == supplierData.Address.ID);
                    if (_existingAddress != null)
                    {
                        _existingAddress.kebele = supplierData.Address.Kebele;
                        _existingAddress.woreda = supplierData.Address.Woreda;
                        _existingAddress.subcity = supplierData.Address.Subcity;
                        _existingAddress.city_town = supplierData.Address.City_Town;
                        _existingAddress.street = supplierData.Address.Street;
                        _existingAddress.houseNo = supplierData.Address.HouseNo;
                        _existingAddress.pobox = supplierData.Address.PoBox;
                        _existingAddress.primaryEmail = supplierData.Address.PrimaryEmail;
                        _existingAddress.secondaryEmail = supplierData.Address.SecondaryEmail;
                        _existingAddress.state_region = supplierData.Address.State_Region;
                        _existingAddress.country = supplierData.Address.Country;
                        _existingAddress.zipCode_postalCode = supplierData.Address.ZipCode_PostalCode;
                        _existingAddress.additionalInfo = supplierData.Address.AdditionalInfo;
                        _existingAddress.lastUpdatedBy = Singleton.Instance.UserID;

                        _existingAddress.lastUpdatedDate = DateTime.Now;
                    }
                    else
                    {
                        //address is new; insert it
                        var _address = AddressBL.CreateAddressObject(supplierData.Address);
                        _supplier.addressID = _address.id;
                        _context.Addresses.Add(_address);
                    }
                }

                //sets telephone/fax data to be added/updated
                if (supplierData.TelephoneFax != null)
                {
                    foreach (var _telFax in supplierData.TelephoneFax)
                    {
                        var _tf = _context.TelephoneFaxes.FirstOrDefault(tf => tf.parentID == supplierData.SupplierID && tf.id == _telFax.ID && _telFax.ID != 0);

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
                                ParentID = _supplier.lkSupplierID,

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
        /// Deletes Supplier data from tblSupplier table
        /// </summary>
        /// <param name="supplierID">The current supplier's SupplierID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(string supplierID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _supplier = _context.Suppliers.SingleOrDefault(c => c.lkSupplierID == supplierID);

                if (_supplier == null)
                    throw new InvalidOperationException("The supplier you are trying to delete doesn't exist");

                _supplier.isDeleted = true;
                _supplier.lastUpdatedBy = Singleton.Instance.UserID;
                _supplier.lastUpdatedDate = DateTime.Now;

                var _address = _context.Addresses.SingleOrDefault(a => a.id == _supplier.addressID);
                if (_address != null)
                {
                    _address.isDeleted = true;
                    _address.lastUpdatedBy = Singleton.Instance.UserID;
                    _address.lastUpdatedDate = DateTime.Now;
                }

                var _telephone = _context.TelephoneFaxes.Where(t => t.parentID == _supplier.lkSupplierID).ToList();
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
        /// Returns Supplier data from tblSupplier table
        /// </summary>
        /// <returns>IEnumerable SupplierData</returns>
        public IEnumerable<SupplierData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _supplier;

                if (isActive)
                {
                    _supplier = _context.Suppliers.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).ToList();
                }
                else
                {
                    _supplier = _context.Suppliers.Where(b => !(b.isDeleted ?? false)).ToList();
                }

                List<SupplierData> _result = new List<SupplierData>();
                foreach (var _r in _supplier)
                {
                    _result.Add(CreateSupplierData(_r));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates SupplierData to be return for GetData call
        /// </summary>
        /// <returns>SupplierData</returns>
        internal SupplierData CreateSupplierData(Supplier source)
        {
            if (source == null)
                return null;

            SupplierData _retValue = new SupplierData();

            _retValue.SupplierID = source.lkSupplierID;
            _retValue.CompanyName = source.companyName;
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
        /// Returns Supplier object from the SupplierData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Supplier CreateSupplier(SupplierData source)
        {
            if (source == null)
                return null;
            var _supplier = new Supplier()
            {
                lkSupplierID = source.SupplierID,
                companyName =  source.CompanyName,
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
                _supplier.Address = _address;
            }
            IEnumerable<TelephoneFaxData> _telephoneFaxes = source.TelephoneFax;

            if (_telephoneFaxes != null)
            {
                foreach (var tele in _telephoneFaxes)
                {
                    tele.ParentID = _supplier.lkSupplierID;
                    var telephoneFax = TelephoneFaxBL.CreateTelephoneFaxObject(tele);
                    _supplier.TelephoneFaxes.Add(telephoneFax);
                }
            }

            return _supplier;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Suppliers.Count(i => !(i.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        public static IEnumerable<SupplierData> GetActiveSuppliers()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _suppliers = (from c in db.Suppliers
                                  where c.isActive && !(c.isDeleted ?? false)
                                  select c).ToList();

                List<SupplierData> _result = new List<SupplierData>();
                _suppliers.ForEach(c => _result.Add(CreateSupplierObject(c)));

                return _result;
            }
        }

        private static SupplierData CreateSupplierObject(Supplier c)
        {
            return new SupplierData
            {
                SupplierID = c.lkSupplierID,
                CompanyName = c.companyName,
                ContactPerson = c.contactPerson,
                Description = c.description,
                AddressID = c.addressID,
                IsActive = c.isActive
            };
        }

        /// <summary>
        /// Checks whether the supplier record exists in database
        /// </summary>
        /// <param name="supplierID">the supplier ID</param>
        /// <returns></returns>
        public bool Exists(string supplierID)
        {
            if (String.IsNullOrWhiteSpace(supplierID))
                return false;
            using (var db = new eDMSEntity("eDMSEntities"))
            {
                var _exists = db.Suppliers.Any(s => s.lkSupplierID == supplierID);

                return _exists;
            }
        }

        #endregion
    }
    public class SupplierData
    {
        public string SupplierID { get; set; }
        public string CompanyName { get; set; }
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
