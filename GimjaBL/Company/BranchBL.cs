using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class BranchBL
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

        public BranchBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts Branch data to tblBranch
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(BranchData branchData, bool isSync = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    if (!isSync)
                    {//for a synchronized branch record, no need to generate a new ID
                        string[] _splitted = branchData.Name.Split(null);

                        string _namePrefix = String.Empty;

                        foreach (var _splName in _splitted)
                            _namePrefix = _namePrefix + _splName[0].ToString();

                        var _newBranchID = _context.GenerateID("tblBranch", _namePrefix).SingleOrDefault();

                        if (_newBranchID == null)
                            throw new InvalidOperationException("Branch ID could not be generated");

                        branchData.ID = _newBranchID.id; 
                    }

                    var _branch = CreateBranch(branchData);
                    if (!isSync && _branch != null)
                    {
                        var _branchElem = SyncTransactionBL.GetBranchElement(_branch);
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            action = "insert",
                            tableName = "tblBranch",
                            value = _branchElem.ToString(),
                            branchID = "BRC-B-1"//TODO: ADD THE CURRENT BRANCH
                        };
                        _context.SyncTransactions.Add(_sync);
                    }
                    _context.Branches.Add(_branch);

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
        /// Updates Branch data on tblBranch
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(BranchData branchData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _branch = _context.Branches.Single(i => i.id == branchData.ID);

                if (_branch == null)
                    throw new InvalidOperationException("Branch detail could not be found.");

                //sets the new branch data
                _branch.name = branchData.Name;
                _branch.description = branchData.Description;
                _branch.isActive = branchData.IsActive;
                _branch.lastUpdatedBy = Singleton.Instance.UserID;
                _branch.lastUpdatedDate = DateTime.Now;

                //sets address data to be added/updated
                if (branchData.Address != null)
                {
                    var _existingAddress = _context.Addresses.FirstOrDefault(a => a.id == branchData.Address.ID);
                    if (_existingAddress != null)
                    {
                        _existingAddress.kebele = branchData.Address.Kebele;
                        _existingAddress.woreda = branchData.Address.Woreda;
                        _existingAddress.subcity = branchData.Address.Subcity;
                        _existingAddress.city_town = branchData.Address.City_Town;
                        _existingAddress.street = branchData.Address.Street;
                        _existingAddress.houseNo = branchData.Address.HouseNo;
                        _existingAddress.pobox = branchData.Address.PoBox;
                        _existingAddress.primaryEmail = branchData.Address.PrimaryEmail;
                        _existingAddress.secondaryEmail = branchData.Address.SecondaryEmail;
                        _existingAddress.state_region = branchData.Address.State_Region;
                        _existingAddress.country = branchData.Address.Country;
                        _existingAddress.zipCode_postalCode = branchData.Address.ZipCode_PostalCode;
                        _existingAddress.additionalInfo = branchData.Address.AdditionalInfo;
                        _existingAddress.lastUpdatedBy = Singleton.Instance.UserID;

                        _existingAddress.lastUpdatedDate = DateTime.Now;
                    }
                    else
                    {
                        //address is new; insert it
                        var _address = AddressBL.CreateAddressObject(branchData.Address);
                        _branch.addressID = _address.id;
                        _context.Addresses.Add(_address);
                    }
                }

                //sets telephone/fax data to be added/updated
                if (branchData.TelephoneFax != null)
                {
                    foreach (var _telFax in branchData.TelephoneFax)
                    {
                        var _tf = _context.TelephoneFaxes.FirstOrDefault(tf => tf.parentID == branchData.ID && tf.id == _telFax.ID && _telFax.ID != 0);

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
                                ParentID = _branch.id,

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
        ///Deletes Branch data from tblBranch table
        ///</summary>
        ///<param name="branchID">The current branch's ID</param>
        ///<returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(string branchID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _branch = _context.Branches.SingleOrDefault(c => c.id == branchID);

                if (_branch == null)
                    throw new InvalidOperationException("The branch you are trying to delete doesn't exist");

                _branch.isDeleted = true;
                _branch.lastUpdatedBy = Singleton.Instance.UserID;
                _branch.lastUpdatedDate = DateTime.Now;

                var _address = _context.Addresses.SingleOrDefault(a => a.id == _branch.addressID);
                if (_address != null)
                {
                    _address.isDeleted = true;
                    _address.lastUpdatedBy = Singleton.Instance.UserID;
                    _address.lastUpdatedDate = DateTime.Now;
                }

                var _telephone = _context.TelephoneFaxes.Where(t => t.parentID == _branch.id).ToList();
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
        /// Returns Branch data from tblBranch table
        /// </summary>
        /// <returns>IEnumerable BranchData</returns>
        public IEnumerable<BranchData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {

                var _branch = isActive == true ? _context.Branches.Where(w => !(w.isDeleted ?? false) && w.isActive).ToList() : _context.Branches.Where(w => !(w.isDeleted ?? false)).ToList();

                List<BranchData> _result = new List<BranchData>();
                foreach (var _w in _branch)
                {
                    _result.Add(CreateBranchData(_w));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates BranchData to be return for GetData call
        /// </summary>
        /// <returns>BranchData</returns>
        internal BranchData CreateBranchData(Branch source)
        {
            if (source == null)
                return null;

            BranchData _retValue = new BranchData();

            _retValue.ID = source.id;
            _retValue.Name = source.name;
            _retValue.Description = source.description;
            _retValue.IsActive = source.isActive;
            _retValue.IsDefault = source.isDefault;
            _retValue.AddressID = source.addressID;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;

            return _retValue;
        }

        /// <summary>
        /// Returns Branch object from the BranchData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Branch CreateBranch(BranchData source)
        {
            if (source == null)
                return null;
            var _branch = new Branch()
            {
                id = source.ID,
                name = source.Name,
                addressID = source.AddressID,
                description = source.Description,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            AddressData _branchAddress = source.Address;
            if (_branchAddress != null)
            {
                var _address = AddressBL.CreateAddressObject(_branchAddress);
                _branch.Address = _address;
            }
            IEnumerable<TelephoneFaxData> _telephoneFaxes = source.TelephoneFax;

            if (_telephoneFaxes != null)
            {
                foreach (var tele in _telephoneFaxes)
                {
                    tele.ParentID = _branch.id;
                    var telephoneFax = TelephoneFaxBL.CreateTelephoneFaxObject(tele);
                    _branch.TelephoneFaxes.Add(telephoneFax);
                }
            }

            return _branch;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Branches.Where(w => !(w.isDeleted ?? false)).Count() == 0 ? false : true);
            }
        }

        /// <summary>
        /// Checks whether the branch record exists in database
        /// </summary>
        /// <param name="branchID">the branch ID</param>
        /// <returns></returns>
        public bool Exists(string branchID)
        {
            if (String.IsNullOrWhiteSpace(branchID))
                return false;
            using (var db = new eDMSEntity("eDMSEntities"))
            {
                var _exists = db.Branches.Any(b => b.id == branchID);

                return _exists;
            }
        }

        //public static IList<BranchData> GetWarehouses()
        //{
        //    using (var db = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _branches = (from w in db.Branches
        //                           where w.isActive && !(w.isDeleted ?? false)
        //                           select new BranchData
        //                           {
        //                               BranchID = w.id,
        //                               Name = w.name,
        //                               Description = w.description,
        //                               IsActive = w.isActive,
        //                               CreatedBy = w.createdBy,
        //                               CreatedDate = w.createdDate,
        //                               LastUpdatedBy = w.lastUpdatedBy,
        //                               LastUpdatedDate = w.lastUpdatedDate,
        //                               IsDeleted = w.isDeleted
        //                           }).ToList();

        //        return _branches;
        //    }
        //}



        //public static IEnumerable<BranchData> GetActiveBranches()
        //{
        //    using (var db = new eDMSEntity("eDMSEntity"))
        //    {
        //        var _branches = (from b in db.Branches
        //                         where b.isActive && !(b.isDeleted ?? false)
        //                         select new BranchData()
        //                         {
        //                             BranchID = b.id,
        //                             Name = b.name,
        //                             Description = b.description,
        //                             IsActive = b.isActive,
        //                             CreatedBy = b.createdBy,
        //                             CreatedDate = b.createdDate,
        //                             LastUpdatedBy = b.lastUpdatedBy,
        //                             LastUpdatedDate = b.lastUpdatedDate,
        //                             IsDeleted = b.isDeleted
        //                         }).ToList();

        //        return _branches;
        //    }
        //}

        /// <summary>
        /// Returns list of branches that are active and not set to default
        /// </summary>
        /// <returns>Returns true when branch is set to default. Fasle, otherwise</returns>
        public IEnumerable<BranchData> GetActiveNotDefaultBranches()
        {
            try
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    var _branch = _context.Branches.Where(w => !(w.isDeleted ?? false) && w.isActive && w.isDefault == false).ToList();

                    List<BranchData> _result = new List<BranchData>();
                    foreach (var _w in _branch)
                    {
                        _result.Add(CreateBranchData(_w));
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
        /// Sets the default branch
        /// </summary>
        /// <param name="branchID">the branchID to be set default branch</param>
        /// <returns>Returns true when branch is set to default. Fasle, otherwise</returns>
        public bool SetDefault(string branchID)
        {
            try
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    var _branch = _context.Branches.SingleOrDefault(b => b.id == branchID && b.isActive && !(b.isDeleted ?? false));

                    if (_branch == null)
                        throw new InvalidOperationException("Default could not be set. Try again!");

                    var _alreadyDefaultBranch = _context.Branches.SingleOrDefault(b => b.isActive && !(b.isDeleted ?? false) && b.isDefault);
                    if (_alreadyDefaultBranch != null)
                        _alreadyDefaultBranch.isDefault = false;

                    _branch.isDefault = true;
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
        /// Checks whether the name of the new branch is already taken
        /// </summary>
        /// <param name="branchName">The name of the new branch</param>
        /// <returns>True if name is available. False, otherwise</returns>
        public bool IsValid(string branchName)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                return _context.Branches.Where(b => !(b.isDeleted ?? false) && b.name == branchName).Count() > 0 ? false : true;
            }
        }

        #endregion
    }

    public class BranchData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public Guid? AddressID { get; set; }
        public bool IsDefault { get; set; }

        public AddressData Address { get; set; }
        public IEnumerable<TelephoneFaxData> TelephoneFax { get; set; }
    }
}
