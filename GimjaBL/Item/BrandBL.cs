using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class BrandBL
    {
        #region Member Variables
        //private int brandID;
        //private string name;
        //private string description;
        //private bool isActive;
        //private string createdBy;
        //private DateTime createdDate;
        //private string lastUpdatedBy;
        //private DateTime? lastUpdatedDate;
        //private bool? isDeleted;

        private bool isUpdate;
        private bool isDataAvailable;

        //Brand brand;
        #endregion

        public BrandBL()
        {
            //brand = new Brand();
            HasData();
        }

        #region Properties

        //public int BrandID
        //{
        //    get { return brandID; }
        //    set { brandID = value; }
        //}

        //public string Name
        //{
        //    get { return name; }
        //    set { name = value; }
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
        /// Adds Brand data to lkBrand
        /// </summary>
        public bool Add(BrandData brandData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _brand = CreateBrand(brandData);
                    _context.Brands.Add(_brand);

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
        /// Updates Brand data on lkBrand
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(BrandData brandData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _brandData = _context.Brands.Single(tf => tf.lkBrandID == brandData.BrandID);

                if (_brandData == null)
                    throw new InvalidOperationException("Brand detail could not be found.");

                //sets the new brand data
                _brandData.name = brandData.Name;
                _brandData.description = brandData.Description;
                _brandData.isActive = brandData.IsActive;

                _brandData.lastUpdatedBy = Singleton.Instance.UserID;
                _brandData.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Deletes Brand data from lkBrand table
        /// </summary>
        /// <param name="brandID">The current brand's BrandID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(int brandID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _brand = _context.Brands.SingleOrDefault(c => c.lkBrandID == brandID);

                if (_brand == null)
                    throw new InvalidOperationException("The brand you are trying to delete doesn't exist");

                _brand.isDeleted = true;
                _brand.lastUpdatedBy = Singleton.Instance.UserID;
                _brand.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Returns Brand data from lkBrand table
        /// </summary>
        /// <returns>IEnumerable BrandData</returns>
        public IEnumerable<BrandData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _brand;

                if (isActive)
                {
                    _brand = _context.Brands.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).OrderBy(r => r.name).ToList();
                }
                else
                {
                    _brand = _context.Brands.Where(b => !(b.isDeleted ?? false)).OrderBy(r => r.name).ToList();
                }

                List<BrandData> _result = new List<BrandData>();
                foreach (var _b in _brand)
                {
                    _result.Add(CreateBrandData(_b));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates BrandData to be return for GetData call
        /// </summary>
        /// <param name="source"></param>
        /// <returns>BrandData</returns>
        internal BrandData CreateBrandData(Brand source)
        {
            if (source == null)
                return null;

            BrandData _retValue = new BrandData();
            _retValue.BrandID = source.lkBrandID;
            _retValue.Name = source.name;
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
        /// Returns Brand object from the BrandData provided 
        /// </summary>
        /// <param name="source">BrandData object</param>
        /// <returns></returns>
        public static Brand CreateBrand(BrandData source)
        {
            if (source == null)
                return null;
            var _brand = new Brand()
            {
                lkBrandID = source.BrandID,
                name = source.Name,
                description = source.Description,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            return _brand;
        }

        /// <summary>
        /// Returns the details of the specified brand
        /// </summary>
        /// <param name="brandID">brandID</param>
        /// <returns>BrandData</returns>
        public IEnumerable<BrandData> GetBrand(int brandID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _brand = _context.Brands.Where(b => b.lkBrandID == brandID && !(b.isDeleted ?? false) && b.isActive==true).OrderBy(r => r.name).ToList();
                List<BrandData> _result = new List<BrandData>();
                foreach (var _b in _brand)
                {
                    _result.Add(CreateBrandData(_b));
                }

                return _result;
            }
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Brands.Count(b => !(b.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        /// <summary>
        /// Checks if the new brand name already exists
        /// </summary>
        /// <param name="name">The new brand name to be added</param>
        /// <returns>true if the new brand name does not exist. False, otherwise</returns>
        public bool IsValid(string name)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    return (_context.Brands.Where(b => b.name.Equals(name) && !(b.isDeleted ?? false)).Count() > 0 ? false : true);
                }
            }

            return false;
        }

        #endregion

    }

    public class BrandData
    {
        public int BrandID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
