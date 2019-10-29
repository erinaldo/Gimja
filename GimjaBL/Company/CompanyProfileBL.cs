using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class CompanyProfileBL
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

        public CompanyProfileBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts CompanyProfile data to tblCompanyProfile
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(CompanyProfileData companyProfileData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    string[] _splitted = companyProfileData.EnglishName.Split(null);

                    string _namePrefix = String.Empty;

                    foreach (var _splName in _splitted)
                        _namePrefix = _namePrefix + _splName[0].ToString();

                    var _newCompanyProfileID = _context.GenerateID("tblCompanyInfo", _namePrefix).ToList();

                    if (_newCompanyProfileID == null)
                        throw new InvalidOperationException("Company Profile ID could not be generated");

                    foreach (var _newID in _newCompanyProfileID)
                    {
                        companyProfileData.ID = _newID.id;
                    }

                    var _companyProfile = CreateCompanyInfo(companyProfileData);
                    _context.CompanyInfoes.Add(_companyProfile);

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
        /// Updates CompanyProfile data on tblCompanyProfile
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(CompanyProfileData companyProfileData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _companyProfile = _context.CompanyInfoes.Single(i => i.id == companyProfileData.ID);

                if (_companyProfile == null)
                    throw new InvalidOperationException("CompanyProfile detail could not be found.");

                //sets the new company info data
                _companyProfile.amharicName = companyProfileData.AmharicName;
                _companyProfile.englishName = companyProfileData.EnglishName;
                _companyProfile.TINNumber = companyProfileData.TINNumber;
                _companyProfile.VATRegistrationNo = companyProfileData.VATRegistrationNo;
                _companyProfile.VATRegistrationDate = companyProfileData.VATRegistrationDate;
                _companyProfile.logo = companyProfileData.Logo;
                _companyProfile.lastUpdatedBy = Singleton.Instance.UserID;
                _companyProfile.lastUpdatedDate = DateTime.Now;

                //sets address data to be added/updated
                if (companyProfileData.Address != null)
                {
                    var _existingAddress = _context.Addresses.FirstOrDefault(a => a.id == companyProfileData.Address.ID);
                    if (_existingAddress != null)
                    {
                        _existingAddress.kebele = companyProfileData.Address.Kebele;
                        _existingAddress.woreda = companyProfileData.Address.Woreda;
                        _existingAddress.subcity = companyProfileData.Address.Subcity;
                        _existingAddress.city_town = companyProfileData.Address.City_Town;
                        _existingAddress.street = companyProfileData.Address.Street;
                        _existingAddress.houseNo = companyProfileData.Address.HouseNo;
                        _existingAddress.pobox = companyProfileData.Address.PoBox;
                        _existingAddress.primaryEmail = companyProfileData.Address.PrimaryEmail;
                        _existingAddress.secondaryEmail = companyProfileData.Address.SecondaryEmail;
                        _existingAddress.state_region = companyProfileData.Address.State_Region;
                        _existingAddress.country = companyProfileData.Address.Country;
                        _existingAddress.zipCode_postalCode = companyProfileData.Address.ZipCode_PostalCode;
                        _existingAddress.additionalInfo = companyProfileData.Address.AdditionalInfo;
                        _existingAddress.lastUpdatedBy = Singleton.Instance.UserID;

                        _existingAddress.lastUpdatedDate = DateTime.Now;
                    }
                    else
                    {
                        //address is new; insert it
                        var _address = AddressBL.CreateAddressObject(companyProfileData.Address);
                        _companyProfile.addressID = _address.id;
                        _context.Addresses.Add(_address);
                    }
                }

                //sets telephone/fax data to be added/updated
                if (companyProfileData.TelephoneFax != null)
                {
                    foreach (var _telFax in companyProfileData.TelephoneFax)
                    {
                        var _tf = _context.TelephoneFaxes.FirstOrDefault(tf => tf.parentID == companyProfileData.ID && tf.id == _telFax.ID && _telFax.ID != 0);

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
                                ParentID = _companyProfile.id,

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
        /// Deletes CompanyProfile data from tblCompanyProfile table
        /// </summary>
        /// <param name="companyProfileID">The current company info's ID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        //public bool Delete(string companyProfileID)
        //{
        //    using (var _context = new eDMSEntity("eDMSEntities"))
        //    {
        //        var _companyProfile = _context.CompanyInfoes.SingleOrDefault(c => c.id == companyProfileID);

        //        if (_companyProfile == null)
        //            throw new InvalidOperationException("The company info you are trying to delete doesn't exist");

        //        _companyProfile.isDeleted = true;
        //        _companyProfile.lastUpdatedBy = Singleton.Instance.userID;
        //        _companyProfile.lastUpdatedDate = DateTime.Now;

        //        var _address = _context.Addresses.SingleOrDefault(a => a.id == _companyProfile.addressID);
        //        if (_address != null)
        //        {
        //            _address.isDeleted = true;
        //            _address.lastUpdatedBy = Singleton.Instance.userID;
        //            _address.lastUpdatedDate = DateTime.Now;
        //        }

        //        var _telephone = _context.TelephoneFaxes.Where(t => t.parentID == _companyProfile.id).ToList();
        //        if (_telephone != null)
        //        {
        //            foreach (var _tel in _telephone)
        //            {
        //                _tel.isDeleted = true;
        //                _tel.lastUpdatedBy = Singleton.Instance.userID;
        //                _tel.lastUpdatedDate = DateTime.Now;
        //            }
        //        }

        //        int _row = _context.SaveChanges();
        //        return _row > 0;
        //    }
        //}

        /// <summary>
        /// Returns CompanyProfile data from tblCompanyProfile table
        /// </summary>
        /// <returns>IEnumerable CompanyProfileData</returns>
        public IEnumerable<CompanyProfileData> GetData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _companyProfile = _context.CompanyInfoes.ToList();

                List<CompanyProfileData> _result = new List<CompanyProfileData>();
                foreach (var _c in _companyProfile)
                {
                    _result.Add(CreateCompanyProfileData(_c));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates CompanyProfileData to be return for GetData call
        /// </summary>
        /// <returns>CompanyProfileData</returns>
        internal CompanyProfileData CreateCompanyProfileData(CompanyInfo source)
        {
            if (source == null)
                return null;

            CompanyProfileData _retValue = new CompanyProfileData();

            _retValue.ID = source.id;
            _retValue.AmharicName = source.amharicName;
            _retValue.EnglishName = source.englishName;
            _retValue.TINNumber = source.TINNumber;
            _retValue.VATRegistrationNo = source.VATRegistrationNo;
            _retValue.VATRegistrationDate = source.VATRegistrationDate;
            _retValue.AddressID = source.addressID;
            _retValue.Logo = source.logo;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.Address = _retValue.AddressID.HasValue ? new AddressBL().GetAddress(_retValue.AddressID.Value) : null;

            return _retValue;
        }

        /// <summary>
        /// Returns CompanyInfo object from the CompanyProfileData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static CompanyInfo CreateCompanyInfo(CompanyProfileData source)
        {
            if (source == null)
                return null;
            var _companyProfile = new CompanyInfo()
            {
                id = source.ID,
                amharicName = source.AmharicName,
                addressID = source.AddressID,
                englishName = source.EnglishName,
                TINNumber = source.TINNumber,
                VATRegistrationNo = source.VATRegistrationNo,
                VATRegistrationDate = source.VATRegistrationDate,
                logo = source.Logo,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            AddressData _customerAddress = source.Address;
            if (_customerAddress != null)
            {
                var _address = AddressBL.CreateAddressObject(_customerAddress);
                _companyProfile.Address = _address;
            }
            IEnumerable<TelephoneFaxData> _telephoneFaxes = source.TelephoneFax;

            if (_telephoneFaxes != null)
            {
                foreach (var tele in _telephoneFaxes)
                {
                    tele.ParentID = _companyProfile.id;
                    var telephoneFax = TelephoneFaxBL.CreateTelephoneFaxObject(tele);
                    _companyProfile.TelephoneFaxes.Add(telephoneFax);
                }
            }

            return _companyProfile;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.CompanyInfoes.Count() == 0 ? false : true);
            }
        }

        private static CompanyProfileData CreateCompanyInfoObject(CompanyInfo c)
        {
            return new CompanyProfileData
            {
                ID = c.id,
                AmharicName = c.amharicName,
                EnglishName = c.englishName,
                TINNumber = c.TINNumber,
                VATRegistrationNo = c.VATRegistrationNo,
                VATRegistrationDate = c.VATRegistrationDate,
                AddressID = c.addressID,
                Logo = c.logo
            };
        }

        public string GetCompanyAddress()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _companyAddress = _context.Addresses.Where(a => a.CompanyInfoes.Any(c => c.addressID == a.id)).SingleOrDefault();
                string _address = string.Empty ;

                if (_companyAddress != null)
                {
                    if (!string.IsNullOrEmpty(_companyAddress.kebele))
                        _address = "Kebele: " + _companyAddress.kebele + " | ";

                    if (!string.IsNullOrEmpty(_companyAddress.woreda))
                        _address = _address + "Woreda: " + _companyAddress.woreda + " | ";

                    if (!string.IsNullOrEmpty(_companyAddress.subcity))
                        _address = _address + "Sub city: " + _companyAddress.subcity + " | ";

                    if (!string.IsNullOrEmpty(_companyAddress.city_town))
                        _address = _address + "Town/City: " + _companyAddress.city_town + " | ";

                    if (!string.IsNullOrEmpty(_companyAddress.country))
                        _address = _address + _companyAddress.country;

                    return _address;
                }

                return "";
            }
        }

        #endregion
    }
    
    public class CompanyProfileData
    {
        public string ID { get; set; }
        public string EnglishName { get; set; }
        public string AmharicName { get; set; }
        public string TINNumber { get; set; }
        public string VATRegistrationNo { get; set; }
        public Nullable<System.DateTime> VATRegistrationDate { get; set; }
        public byte[] Logo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<System.Guid> AddressID { get; set; }

        public AddressData Address { get; set; }
        public IEnumerable<TelephoneFaxData> TelephoneFax { get; set; }

    }
}
