using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class UserBL//:IDataManager<UserData>
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

        public UserBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts User data to tblUser
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(UserData userData, bool isSync = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _user = CreateUser(userData);

                    if (!isSync && _user != null)
                    {
                        var _userElem = SyncTransactionBL.GetUserElement(_user);
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            action = "insert",
                            tableName = "tblUser",
                            value = _userElem.ToString(),
                            branchID = "BRC-B-1"//TODO: ADD THE CURRENT BRANCH
                        };
                        _context.SyncTransactions.Add(_sync);
                    }

                    _context.Users.Add(_user);
                    _context.SaveChanges();

                    //IEnumerable<RoleData> _userRole = userData.Role;
                    //if (_userRole != null)
                    //{
                    //    foreach (var _r in _userRole)
                    //    {
                    //        var _role = _context.Roles.FirstOrDefault(r => r.id.Equals(_r.ID));
                    //        var _newUser = _context.Users.FirstOrDefault(u => u.userID == _user.userID);
                    //        _newUser.Roles.Add(_role);
                    //    }
                    //}

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
        /// Updates User data on tblUser
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(UserData userData)
        {
            try
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    var _user = _context.Users.Single(i => i.userID == userData.UserID);

                    if (_user == null)
                        throw new InvalidOperationException("User detail could not be found.");

                    //sets the new user data
                    _user.userID = userData.UserID;
                    _user.roleID = userData.RoleID;
                    _user.fullName = userData.FullName;
                    _user.password = userData.Password;
                    _user.isActive = userData.IsActive;

                    _user.lastUpdatedBy = Singleton.Instance.UserID;
                    _user.lastUpdatedDate = DateTime.Now;

                    #region update address
                    //sets address data to be added/updated
                    if (userData.Address != null)
                    {
                        var _existingAddress = _context.Addresses.FirstOrDefault(a => a.id == userData.Address.ID);
                        if (_existingAddress != null)
                        {
                            _existingAddress.kebele = userData.Address.Kebele;
                            _existingAddress.woreda = userData.Address.Woreda;
                            _existingAddress.subcity = userData.Address.Subcity;
                            _existingAddress.city_town = userData.Address.City_Town;
                            _existingAddress.street = userData.Address.Street;
                            _existingAddress.houseNo = userData.Address.HouseNo;
                            _existingAddress.pobox = userData.Address.PoBox;
                            _existingAddress.primaryEmail = userData.Address.PrimaryEmail;
                            _existingAddress.secondaryEmail = userData.Address.SecondaryEmail;
                            _existingAddress.state_region = userData.Address.State_Region;
                            _existingAddress.country = userData.Address.Country;
                            _existingAddress.zipCode_postalCode = userData.Address.ZipCode_PostalCode;
                            _existingAddress.additionalInfo = userData.Address.AdditionalInfo;
                            _existingAddress.lastUpdatedBy = Singleton.Instance.UserID;

                            _existingAddress.lastUpdatedDate = DateTime.Now;
                        }
                        else
                        {
                            //address is new; insert it
                            var _address = AddressBL.CreateAddressObject(userData.Address);
                            _user.addressID = _address.id;
                            _context.Addresses.Add(_address);
                        }
                    }

                    #endregion

                    #region update user role
                    ////sets user role data to be updated/added
                    //if (userData.Role != null)
                    //{
                    //    //delete existing roles
                    //    var _existingRoles = _context.Users.FirstOrDefault(u => u.userID == userData.UserID);
                    //    if (_existingRoles != null)
                    //    {
                    //        foreach (var _r in _existingRoles.Roles.ToList())
                    //        {
                    //            _existingRoles.Roles.Remove(_r);
                    //        }
                    //    }

                    //    //insert the new roles
                    //    IEnumerable<RoleData> _userRole = userData.Role;
                    //    foreach (var _r in _userRole)
                    //    {
                    //        var _role = _context.Roles.SingleOrDefault(r => r.id.Equals(_r.ID));
                    //        _user.Roles.Add(_role);
                    //    }
                    //}

                    #endregion

                    #region update telephoneFax

                    //sets telephone/fax data to be added/updated
                    if (userData.TelephoneFax != null)
                    {
                        foreach (var _telFax in userData.TelephoneFax)
                        {
                            var _tf = _context.TelephoneFaxes.FirstOrDefault(tf => tf.parentID == userData.UserID && tf.id == _telFax.ID && _telFax.ID != 0);

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
                                //var _existingTF = _context.TelephoneFaxes.Where(tf => tf.parentID == userData.UserID);
                                //delete existing TF first
                                //foreach (var _tfExisting in _existingTF.ToList())
                                //    _context.TelephoneFaxes.Remove(_tfExisting);
                                
                                var _telephoneFaxes = new TelephoneFaxData()
                                {
                                    Type = _telFax.Type,
                                    Number = _telFax.Number,
                                    IsActive = _telFax.IsActive,
                                    ParentID = _user.userID,

                                    CreatedBy = Singleton.Instance.UserID,
                                    CreatedDate = DateTime.Now
                                };

                                var _telephoneFax = TelephoneFaxBL.CreateTelephoneFaxObject(_telephoneFaxes);
                                _context.TelephoneFaxes.Add(_telephoneFax);
                            }
                        }
                    }

                    #endregion

                    int _row = _context.SaveChanges();
                    return _row > 0;
                }
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

        /// <summary>
        /// Deletes User data from tblUser table
        /// </summary>
        /// <param name="userID">The current user's UserID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(string userID)
        {
            try
            {

                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    var _user = _context.Users.SingleOrDefault(c => c.userID == userID);

                    if (_user == null)
                        throw new InvalidOperationException("The user you are trying to delete doesn't exist");

                    #region delete user
                    ////sets the new user data
                    //_user.isDeleted = true;
                    //_user.lastUpdatedBy = Singleton.Instance.userID;
                    //_user.lastUpdatedDate = DateTime.Now;
                    //#endregion

                    //#region delete address
                    ////sets address data to be added/updated
                    //if (_user.Address != null)
                    //{
                    //    var _existingAddress = _context.Addresses.FirstOrDefault(a => a.id == _user.Address.id);
                    //    if (_existingAddress != null)
                    //    {
                    //        _existingAddress.isDeleted = true;
                    //        _existingAddress.lastUpdatedBy = Singleton.Instance.userID;
                    //        _existingAddress.lastUpdatedDate = DateTime.Now;
                    //    }
                    //}

                    #endregion

                    #region Delete user roles
                    ////delete existing roles
                    //if (_user != null)
                    //{
                    //    foreach (var _u in _user.Roles.ToList())
                    //    {
                    //        _user.Roles.Remove(_u);
                    //    }
                    //}

                    //_context.Users.Remove(_user);

                    #endregion

                    #region delete telephoneFax

                    ////sets telephone/fax data to be added/updated
                    //if (_user.TelephoneFaxes != null)
                    //{
                    //    foreach (var _telFax in _user.TelephoneFaxes)
                    //    {
                    //        var _tf = _context.TelephoneFaxes.FirstOrDefault(tf => tf.parentID == _user.userID && tf.id == _telFax.id && _telFax.id != 0);

                    //        if (_tf != null) //update telephoneFax
                    //        {
                    //            _tf.isDeleted = true;
                    //            _tf.lastUpdatedBy = Singleton.Instance.userID;
                    //            _tf.lastUpdatedDate = DateTime.Now;
                    //        }
                    //    }
                    //}

                    #endregion

                    int _row = _context.SaveChanges();
                    return _row > 0;
                }
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

        /// <summary>
        /// Returns User data from tblUser table
        /// </summary>
        /// <returns>IEnumerable UserData</returns>
        public IEnumerable<UserData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _user = (isActive == true) ? _context.Users.Where(b => b.isActive.Equals(isActive) && !(b.isDeleted ?? false)).ToList(): _context.Users.Where(u=>!(u.isDeleted ?? false)).ToList();
                
                List<UserData> _result = new List<UserData>();
                foreach (var _r in _user)
                {
                    _result.Add(CreateUserData(_r));
                }

                return _result;
            }
        }

        /// <summary>
        /// Returns User data from tblUser table
        /// </summary>
        /// <returns>IEnumerable UserData</returns>
        public IEnumerable<UserData> GetData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _user;

                if(Singleton.Instance.UserID == "superadmin")
                    _user = _context.Users.Where(u => !(u.isDeleted ?? false)).ToList();
                else
                    _user = _context.Users.Where(u=>!(u.isDeleted ?? false) && u.userID!="superadmin").ToList();
                
                List<UserData> _result = new List<UserData>();
                if (_user != null)
                {
                    foreach (var _r in _user)
                    {
                        _result.Add(CreateUserData(_r));
                    }

                    return _result;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns the role id of a user
        /// </summary>
        /// <param name="userID">user id</param>
        /// <returns></returns>
        public int GetRoleID(string userID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _roleID = _context.Users.SingleOrDefault(u => u.userID == userID).roleID;
                return (_roleID != null ? _roleID : 0);
            }
        }
        
        /// <summary>
        /// Returns User role from tblUser table
        /// </summary>
        /// <returns>IEnumerable UserData</returns>
        public IEnumerable<RoleData> GetUserRole()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _loggedInUserRoleID = _context.Users.SingleOrDefault(u => u.userID == Singleton.Instance.UserID).roleID;
                if (_loggedInUserRoleID == 0)
                    throw new InvalidOperationException("Could not access logged in user's role");

                var _superadminRole = "Super Admin";
                var _superadminRoleID = _context.Roles.Where(r => r.roleName == _superadminRole).FirstOrDefault().id;
                

                dynamic _result;
                if (_loggedInUserRoleID == _superadminRoleID)
                {
                    _result = (from r in _context.Roles
                                   where r.isActive == true && !(r.isDeleted ?? false)
                                   select new RoleData
                                   {
                                       ID = r.id,
                                       RoleName = r.roleName
                                   }).ToList();
                }
                else
                {
                    _result = (from r in _context.Roles
                               where r.isActive == true && !(r.isDeleted ?? false) && r.id != _superadminRoleID
                               select new RoleData
                               {
                                   ID = r.id,
                                   RoleName = r.roleName
                               }).ToList();
                }

                return _result;
            }
        }
             
        /// <summary>
        /// Creates UserData to be return for GetData call
        /// </summary>
        /// <returns>UserData</returns>
        internal UserData CreateUserData(User source)
        {
            if (source == null)
                return null;

            UserData _retValue = new UserData();

            _retValue.UserID = source.userID;
            _retValue.FullName = source.fullName;
            _retValue.Password = source.password;
            _retValue.AddressID = source.addressID;
            _retValue.RoleID = source.roleID;
            _retValue.IsActive = source.isActive;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;

            return _retValue;
        }

        /// <summary>
        /// Returns User object from the UserData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static User CreateUser(UserData source)
        {
            if (source == null)
                return null;
            var _user = new User()
            {
                userID = source.UserID,
                fullName = source.FullName,
                addressID = source.AddressID,
                roleID = source.RoleID,
                password = source.Password,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            AddressData _userAddress = source.Address;
            if (_userAddress != null)
            {
                var _address = AddressBL.CreateAddressObject(_userAddress);
                _user.Address = _address;
            }

            IEnumerable<TelephoneFaxData> _telephoneFaxes = source.TelephoneFax;

            if (_telephoneFaxes != null)
            {
                foreach (var tele in _telephoneFaxes)
                {
                    tele.ParentID = _user.userID;
                    var telephoneFax = TelephoneFaxBL.CreateTelephoneFaxObject(tele);
                    _user.TelephoneFaxes.Add(telephoneFax);
                }
            }

            return _user;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Users.Count() == 0 ? false : true);
            }
        }
        
              
        /// <summary>
        /// Gets a user object identified by the specified ID
        /// </summary>
        /// <param name="userID">the user ID</param>
        /// <returns></returns>
        public UserData GetUser(string userID)
        {
            if (string.IsNullOrEmpty(userID))
            {
                return null;
            }
            using (var db = new eDMSEntity("eDMSEntities"))
            {
                var _user = (from u in db.Users
                             where u.userID.ToLower().Equals(userID.ToLower()) && !(u.isDeleted ?? false)
                             select u).SingleOrDefault();
                if (_user != null)
                {
                    var _retValue = CreateUserData(_user);
                    return _retValue;
                }
                return null;
            }
        }

        /// <summary>
        /// Checks whether the user id is already taken or not
        /// </summary>
        /// <param name="userID">the user id to check</param>
        /// <returns>false if the user id is already taken. True, otherwise</returns>
        public bool IsValid(string userID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                return (_context.Users.Where(u=>u.userID == userID && !(u.isDeleted ?? false)).Count() > 0) ? false : true;
            }
        }

        public static IEnumerable<UserData> GetUsers()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _users = (from u in db.Users
                              where u.isActive && !(u.isDeleted ?? false)
                              select new UserData
                              {
                                  UserID = u.userID,
                                  FullName = u.fullName
                              }).ToList();

                return _users;
            }
        }

        /// <summary>
        /// Checks whether the user record exists in database
        /// </summary>
        /// <param name="userID">the user ID</param>
        /// <returns></returns>
        public bool Exists(string userID)
        {
            if (String.IsNullOrWhiteSpace(userID))
                return false;
            using (var db = new eDMSEntity("eDMSEntities"))
            {
                var _exists = db.Users.Any(u => u.userID == userID);

                return _exists;
            }
        }
        #endregion
    }

    public class UserData
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public Guid? AddressID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public AddressData Address { get; set; }
        public IEnumerable<TelephoneFaxData> TelephoneFax { get; set; }
    }
}
