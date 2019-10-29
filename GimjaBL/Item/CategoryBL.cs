using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class CategoryBL
    {
        #region Member Variables
        //private int categoryID;
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

//        Category category;
        #endregion

        public CategoryBL()
        {
            //category = new Category();
            HasData();
        }

        #region Properties

        //public int CategoryID
        //{
        //    get { return categoryID; }
        //    set { categoryID = value; }
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
        /// Inserts category data to lkCategory
        /// </summary>
        /// <param name="categoryData">CategoryData object</param>
        /// <returns></returns>
        public bool Add(CategoryData categoryData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _category = CreateCategory(categoryData);
                    _context.Categories.Add(_category);

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
        /// Updates Category data on lkCategory
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(CategoryData categoryData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _categoryData = _context.Categories.Single(tf => tf.lkCategoryID == categoryData.CategoryID);

                if (_categoryData == null)
                    throw new InvalidOperationException("Category detail could not be found.");

                //sets the new category data
                _categoryData.name = categoryData.Name;
                _categoryData.description = categoryData.Description;
                _categoryData.isActive = categoryData.IsActive;

                _categoryData.lastUpdatedBy = Singleton.Instance.UserID;
                _categoryData.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Deletes Category data from lkCategory table
        /// </summary>
        /// <param name="categoryID">The current category's CategoryID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(int categoryID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _category = _context.Categories.SingleOrDefault(c => c.lkCategoryID == categoryID);

                if (_category == null)
                    throw new InvalidOperationException("The category you are trying to delete doesn't exist");

                _category.isDeleted = true;
                _category.lastUpdatedBy = Singleton.Instance.UserID;
                _category.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Returns Category data from lkCategory table
        /// </summary>
        /// <returns>IEnumerable CategoryData</returns>
        public IEnumerable<CategoryData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _category;

                if (isActive)
                {
                    _category = _context.Categories.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).OrderBy(r => r.name).ToList();
                }
                else
                {
                    _category = _context.Categories.Where(b => !(b.isDeleted ?? false)).OrderBy(r => r.name).ToList();
                }

                List<CategoryData> _result = new List<CategoryData>();
                foreach (var _b in _category)
                {
                    _result.Add(CreateCategoryData(_b));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates CategoryData to be return for GetData call
        /// </summary>
        /// <param name="source"></param>
        /// <returns>CategoryData</returns>
        internal CategoryData CreateCategoryData(Category source)
        {
            if (source == null)
                return null;

            CategoryData _retValue = new CategoryData();
            _retValue.CategoryID = source.lkCategoryID;
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
        /// Returns Category object from the CategoryData provided 
        /// </summary>
        /// <param name="source">CategoryData object</param>
        /// <returns></returns>
        public static Category CreateCategory(CategoryData source)
        {
            if (source == null)
                return null;
            var _category = new Category()
            {
                lkCategoryID = source.CategoryID,
                name = source.Name,
                description = source.Description,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            return _category;
        }


        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Categories.Count(b => !(b.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        /// <summary>
        /// Checks if the new category name already exists
        /// </summary>
        /// <param name="name">The new category name to be added</param>
        /// <returns>true if the new category name does not exist. False, otherwise</returns>
        public bool IsValid(string name)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    return (_context.Categories.Where(b => b.name.Equals(name) && !(b.isDeleted ?? false)).Count() > 0 ? false : true);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns Filtered Category data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategoryData> GetFilteredCategories(int categoryID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _category = _context.GetFilteredCategories(categoryID).ToList();
                //var _category = _context.Categories.Where(x => x.lkCategoryID == categoryID).ToList();

                List<CategoryData> _retVal = new List<CategoryData>();

                foreach (var _c in _category)
                {
                    CategoryData _result = new CategoryData();

                    _result.CategoryID = _c.lkCategoryID;
                    _result.Name = _c.name;
                    _result.Description = _c.description;
                    _result.CreatedBy = _c.createdBy;
                    _result.CreatedDate = _c.createdDate;
                    _result.IsActive = _c.isActive;
                    _result.IsDeleted = _c.isDeleted;
                    _result.LastUpdatedBy = _c.lastUpdatedBy;
                    _result.LastUpdatedDate = _c.lastUpdatedDate;
                    _retVal.Add(_result);
                }

                return _retVal;
            }
        }


        /// <summary>
        /// Returns the details of the specified category
        /// </summary>
        /// <param name="categoryID">categoryID</param>
        /// <returns>CategoryData</returns>
        public IEnumerable<CategoryData> GetCategory(int categoryID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _category = _context.Categories.Where(b => b.lkCategoryID == categoryID && !(b.isDeleted ?? false) && b.isActive == true).OrderBy(r => r.name).ToList();
                List<CategoryData> _result = new List<CategoryData>();
                foreach (var _c in _category)
                {
                    _result.Add(CreateCategoryData(_c));
                }

                return _result;
            }
        }

        #endregion

    }

    public class CategoryData
    {
        public int CategoryID { get; set; }
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
