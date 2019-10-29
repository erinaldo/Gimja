using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class CustomerBL
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

        public CustomerBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts Customer data to tblCustomer
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(CustomerData customerData, bool isSync = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    if (!isSync && customerData!=null)
                    {
                        string _namePrefix = customerData.Name[0].ToString() + customerData.FatherName[0].ToString();

                        var _newCustomerID = _context.GenerateID("tblCustomer", _namePrefix).SingleOrDefault();

                        if (_newCustomerID == null)
                            throw new InvalidOperationException("Customer ID could not be generated");

                        customerData.ID = _newCustomerID.id;
                        
                    }

                    var _customer = CreateCustomer(customerData);
                    if (!isSync && _customer != null)
                    {
                        var _customerElem = SyncTransactionBL.GetCustomerElement(_customer);
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            action = "insert",
                            tableName = "tblCustomer",
                            value = _customerElem.ToString(),
                            branchID = "BRC-B-1"//TODO: ADD THE CURRENT BRANCH
                        };
                        _context.SyncTransactions.Add(_sync);
                    }
                    
                    _context.Customers.Add(_customer);

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
        /// Updates Customer data on tblCustomer
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(CustomerData customerData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _customer = _context.Customers.Single(i => i.id == customerData.ID);

                if (_customer == null)
                    throw new InvalidOperationException("Customer detail could not be found.");

                //sets the new customer data
                _customer.name = customerData.Name;
                _customer.fatherName = customerData.FatherName;
                _customer.grandfatherName = customerData.GrandfatherName;
                _customer.companyName = customerData.CompanyName;
                _customer.TINNo = customerData.TINNo;
                _customer.VATRegistrationNo = customerData.VATRegistrationNo;
                _customer.VATRegistrationDate = customerData.VATRegistrationDate;
                _customer.isActive = customerData.IsActive;

                _customer.lastUpdatedBy = Singleton.Instance.UserID;
                _customer.lastUpdatedDate = DateTime.Now;

                //sets address data to be added/updated
                if (customerData.Address != null)
                {
                    var _existingAddress = _context.Addresses.FirstOrDefault(a => a.id == customerData.Address.ID);
                    if (_existingAddress != null)
                    {
                        _existingAddress.kebele = customerData.Address.Kebele;
                        _existingAddress.woreda = customerData.Address.Woreda;
                        _existingAddress.subcity = customerData.Address.Subcity;
                        _existingAddress.city_town = customerData.Address.City_Town;
                        _existingAddress.street = customerData.Address.Street;
                        _existingAddress.houseNo = customerData.Address.HouseNo;
                        _existingAddress.pobox = customerData.Address.PoBox;
                        _existingAddress.primaryEmail = customerData.Address.PrimaryEmail;
                        _existingAddress.secondaryEmail = customerData.Address.SecondaryEmail;
                        _existingAddress.state_region = customerData.Address.State_Region;
                        _existingAddress.country = customerData.Address.Country;
                        _existingAddress.zipCode_postalCode = customerData.Address.ZipCode_PostalCode;
                        _existingAddress.additionalInfo = customerData.Address.AdditionalInfo;

                        _existingAddress.lastUpdatedBy = Singleton.Instance.UserID;
                        _existingAddress.lastUpdatedDate = DateTime.Now;
                    }
                    else
                    {
                        //address is new; insert it
                        var _address = AddressBL.CreateAddressObject(customerData.Address);
                        _context.Addresses.Add(_address);
                    }
                }

                //sets telephone/fax data to be added/updated
                if (customerData.TelephoneFax != null)
                {
                    foreach (var _telFax in customerData.TelephoneFax)
                    {
                        var _tf = _context.TelephoneFaxes.FirstOrDefault(tf => tf.parentID == customerData.ID && tf.id == _telFax.ID && _telFax.ID != 0);

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
                                ParentID = _customer.id,

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
        /// Deletes Customer data from tblCustomer table
        /// </summary>
        /// <param name="customerID">The current customer's ID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(string customerID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _customer = _context.Customers.SingleOrDefault(c => c.id == customerID);

                if (_customer == null)
                    throw new InvalidOperationException("The customer you are trying to delete doesn't exist");

                _customer.isDeleted = true;
                _customer.lastUpdatedBy = Singleton.Instance.UserID;
                _customer.lastUpdatedDate = DateTime.Now;

                var _address = _context.Addresses.SingleOrDefault(a => a.id == _customer.addressID);
                if (_address != null)
                {
                    _address.isDeleted = true;
                    _address.lastUpdatedBy = Singleton.Instance.UserID;
                    _address.lastUpdatedDate = DateTime.Now;
                }

                var _telephone = _context.TelephoneFaxes.Where(t => t.parentID == _customer.id).ToList();
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
        /// Returns Customer data from tblCustomer table
        /// </summary>
        /// <returns>IEnumerable CustomerData</returns>
        public IEnumerable<CustomerData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _customer;

                if (isActive)
                {
                    _customer = _context.Customers.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).ToList();
                }
                else
                {
                    _customer = _context.Customers.Where(b => !(b.isDeleted ?? false)).ToList();
                }

                List<CustomerData> _result = new List<CustomerData>();
                foreach (var _r in _customer)
                {
                    _result.Add(CreateCustomerData(_r));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates CustomerData to be return for GetData call
        /// </summary>
        /// <returns>CustomerData</returns>
        internal CustomerData CreateCustomerData(Customer source)
        {
            if (source == null)
                return null;

            CustomerData _retValue = new CustomerData();

            _retValue.ID = source.id;
            _retValue.Name = source.name;
            _retValue.FatherName = source.fatherName;
            _retValue.GrandfatherName = source.grandfatherName;
            _retValue.AddressID = source.addressID;
            _retValue.CompanyName = source.companyName;
            _retValue.TINNo = source.TINNo;
            _retValue.VATRegistrationNo = source.VATRegistrationNo;
            _retValue.VATRegistrationDate = source.VATRegistrationDate;
            _retValue.IsActive = source.isActive;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;

            return _retValue;
        }

        /// <summary>
        /// Returns Customer object from the CustomerData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Customer CreateCustomer(CustomerData source)
        {
            if (source == null)
                return null;
            var _customer = new Customer()
            {
                id = source.ID,
                name = source.Name,
                addressID = source.AddressID,
                fatherName = source.FatherName,
                grandfatherName = source.GrandfatherName,
                companyName = source.CompanyName,
                TINNo = source.TINNo,
                VATRegistrationNo = source.VATRegistrationNo,
                VATRegistrationDate = source.VATRegistrationDate,
                isActive = source.IsActive,
                createdBy = source.CreatedBy,
                createdDate = source.CreatedDate
            };

            AddressData _customerAddress = source.Address;
            if (_customerAddress != null)
            {
                var _address = AddressBL.CreateAddressObject(_customerAddress);
                _customer.Address = _address;
            }
            IEnumerable<TelephoneFaxData> _telephoneFaxes = source.TelephoneFax;

            if (_telephoneFaxes != null)
            {
                foreach (var tele in _telephoneFaxes)
                {
                    tele.ParentID = _customer.id;
                    var telephoneFax = TelephoneFaxBL.CreateTelephoneFaxObject(tele);
                    _customer.TelephoneFaxes.Add(telephoneFax);
                }
            }

            return _customer;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Customers.Count(i => !(i.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        public static IEnumerable<CustomerData> GetActiveCustomers()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _customers = (from c in db.Customers
                                  where c.isActive && !(c.isDeleted ?? false)
                                  select c).ToList();

                List<CustomerData> _result = new List<CustomerData>();
                _customers.ForEach(c => _result.Add(CreateCustomerObject(c)));

                return _result;
            }
        }

        private static CustomerData CreateCustomerObject(Customer source)
        {
            if (source == null)
                return null;

            CustomerData _retValue = new CustomerData();

            _retValue.ID = source.id;
            _retValue.Name = source.name;
            _retValue.FatherName = source.fatherName;
            _retValue.GrandfatherName = source.grandfatherName;
            _retValue.AddressID = source.addressID;
            _retValue.CompanyName = source.companyName;
            _retValue.TINNo = source.TINNo;
            _retValue.VATRegistrationNo = source.VATRegistrationNo;
            _retValue.VATRegistrationDate = source.VATRegistrationDate;
            _retValue.IsActive = source.isActive;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;
            _retValue.AddressID = source.addressID;
            _retValue.Address = _retValue.AddressID.HasValue ? new AddressBL().GetAddress(_retValue.AddressID.Value) : null;
            _retValue.TelephoneFax = new TelephoneFaxBL().GetTelephoneFax(_retValue.ID);

            _retValue.FullName = source.FullName;
            return _retValue;
        }

        /// <summary>
        /// Checks whether the customer record exists in database
        /// </summary>
        /// <param name="customerID">the customer ID</param>
        /// <returns></returns>
        public bool Exists(string customerID)
        {
            if (String.IsNullOrWhiteSpace(customerID))
                return false;
            using (var db = new eDMSEntity("eDMSEntities"))
            {
                var _exists = db.Customers.Any(c => c.id == customerID);

                return _exists;
            }
        }

        #endregion

        public CustomerData GetCustomer(string p)
        {
            if (string.IsNullOrEmpty(p))
                return null;
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _customer = (from c in _context.Customers
                                 where !(c.isDeleted ?? false) && c.isActive && c.id.ToLower().Equals(p.ToLower())
                                 select c).SingleOrDefault();

                if (_customer != null)
                {
                    var _retValue = CreateCustomerObject(_customer);
                    return _retValue;
                }
                return null;
            }
        }
    }

    public class CustomerData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string GrandfatherName { get; set; }
        public Guid? AddressID { get; set; }
        public string CompanyName { get; set; }
        public string TINNo { get; set; }
        public string VATRegistrationNo { get; set; }
        public Nullable<System.DateTime> VATRegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }

        public string FullName { get; set; }

        public AddressData Address { get; set; }
        public IEnumerable<TelephoneFaxData> TelephoneFax { get; set; }
    }
}
