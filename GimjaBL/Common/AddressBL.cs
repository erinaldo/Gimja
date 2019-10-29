using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class AddressBL
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

        public AddressBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts Address data to tblAddress
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(AddressData addressData, bool isSync=false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _address = CreateAddressObject(addressData);

                    if (!isSync && _address != null)
                    {
                        var _addressElem = SyncTransactionBL.GetAddressElement(_address);
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            action = "insert",
                            tableName = "tblAddress",
                            value = _addressElem.ToString(),
                            branchID = "BRC-B-1"//TODO: ADD THE CURRENT BRANCH
                        };
                        _context.SyncTransactions.Add(_sync);
                    }

                    _context.Addresses.Add(_address);

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
        /// Updates Address data on tblAddress
        /// </summary>
        /// <param name="addressData">Receives addressData to be updated</param>
        //public void Update(AddressData addressData)
        //{
        //    using (var _context = new eDMSEntity("eDMSEntities"))
        //    {
        //        var _address = _context.Addresses.Single(i => i.id == addressData.ID);

        //        if (_address == null)
        //            throw new InvalidOperationException("Address info could not be updated.");

        //        _address.kebele = Kebele;
        //        _address.woreda = Woreda;
        //        _address.subcity = Subcity;
        //        _address.city_town = City_Town;
        //        _address.street = Street;
        //        _address.houseNo = HouseNo;
        //        _address.pobox = PoBox;
        //        _address.primaryEmail = PrimaryEmail;
        //        _address.secondaryEmail = SecondaryEmail;
        //        _address.state_region = State_Region;
        //        _address.country = Country;
        //        _address.zipCode_postalCode = ZipCode_PostalCode;
        //        _address.additionalInfo = AdditionalInfo;

        //        //TODO: assign the loggedin user
        //        //_address.lastUpdatedBy=;
        //        _address.lastUpdatedDate = DateTime.Now;

        //        _context.SaveChanges();
        //    }
        //}

        /// <summary>
        /// Deletes Address data from tblAddress table
        /// </summary>
        //public void Delete()
        //{
        //    using (var _context = new eDMSEntity("eDMSEntities"))
        //    {
        //        var _address = _context.Addresses.Single(i => i.id == ID);
        //        _address.isDeleted = IsDeleted;

        //        _context.SaveChanges();
        //    }
        //}

        /// <summary>
        /// Returns Address data from tblAddress table
        /// </summary>
        /// <returns>IEnumerable AddressData</returns>
        public IEnumerable<AddressData> GetData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _address = _context.Addresses.Where(i => !(i.isDeleted ?? false)).ToList();

                List<AddressData> _result = new List<AddressData>();
                foreach (var _r in _address)
                {
                    _result.Add(CreateAddressData(_r));
                }

                return _result;
            }
        }

        /// <summary>
        /// Sets the address values of the specified addressID
        /// </summary>
        /// <param name="addressID">Returns true if address data exists otherwise sets address ID to empty GUID</param>
        public AddressData GetAddress(Guid addressID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _address = _context.Addresses.FirstOrDefault(a => a.id == addressID && !(a.isDeleted ?? false));

                if (_address != null)
                {
                    return CreateAddressData(_address);
                }
                else
                    _address.id = Guid.Empty;

                return null;
            }
        }

        /// <summary>
        /// Creates AddressData to be return for GetData call
        /// </summary>
        /// <returns>AddressData</returns>
        internal AddressData CreateAddressData(Address source)
        {
            if (source == null)
                return null;

            AddressData _retValue = new AddressData();

            _retValue.ID = source.id;
            _retValue.Kebele = source.kebele;
            _retValue.Woreda = source.woreda;
            _retValue.Subcity = source.subcity;
            _retValue.City_Town = source.city_town;
            _retValue.Street = source.street;
            _retValue.HouseNo = source.houseNo;
            _retValue.PoBox = source.pobox;
            _retValue.PrimaryEmail = source.primaryEmail;
            _retValue.SecondaryEmail = source.secondaryEmail;
            _retValue.State_Region = source.state_region;
            _retValue.Country = source.country;
            _retValue.ZipCode_PostalCode = source.zipCode_postalCode;
            _retValue.AdditionalInfo = source.additionalInfo;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;

            return _retValue;
        }

        public static Address CreateAddressObject(AddressData source)
        {
            if (source == null)
                return null;
            var result = new Address
            {
                id = source.ID,
                kebele = source.Kebele,
                woreda = source.Woreda,
                subcity = source.Subcity,
                city_town = source.City_Town,
                street = source.Street,
                houseNo = source.HouseNo,
                pobox = source.PoBox,
                primaryEmail = source.PrimaryEmail,
                secondaryEmail = source.SecondaryEmail,
                state_region = source.State_Region,
                country = source.Country,
                zipCode_postalCode = source.ZipCode_PostalCode,
                additionalInfo = source.AdditionalInfo,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            return result;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Addresses.Count(i => !(i.isDeleted ?? false)) == 0 ? false : true);
            }
        }


        /// <summary>
        /// Checks whether a address object identified by the specified ID exists in database
        /// </summary>
        /// <param name="retId">the address return ID</param>
        /// <returns>true if the return exists, false otherwise</returns>
        public bool Exists(Guid retId)
        {
            if (retId == Guid.Empty)
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _exists = db.Addresses.Any(adr => adr.id == retId);

                return _exists;
            }
        }

        #endregion
    }

    public class AddressData
    {
        public Guid ID { get; set; }
        public string Kebele { get; set; }
        public string Woreda { get; set; }
        public string Subcity { get; set; }
        public string City_Town { get; set; }
        public string Street { get; set; }
        public string HouseNo { get; set; }
        public string PoBox { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string State_Region { get; set; }
        public string Country { get; set; }
        public string ZipCode_PostalCode { get; set; }
        public string AdditionalInfo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
