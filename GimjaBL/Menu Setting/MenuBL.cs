using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using System.Drawing;
using GimjaDL;
using System.IO;

namespace GimjaBL
{
    public class MenuBL
    {
        #region Member Variables

        private bool isMenuUpdate;
        private bool isDataAvailable;

        #endregion

        #region Properties

        public bool IsMenuUpdate
        {
            get { return isMenuUpdate; }
            set { isMenuUpdate = value; }
        }

        public bool IsDataAvailable
        {
            get { return isDataAvailable; }
            set { isDataAvailable = value; }
        }

        #endregion

        public MenuBL()
        {
            HasData();
        }

        #region methods

        /// <summary>
        /// inserts Menu data into the database
        /// </summary>
        /// <param name="menuData"></param>
        /// <returns></returns>
        public bool Add(MenuData menuData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _order = _context.Menus.Where(m => m.parent == menuData.Parent && !(m.isDeleted ?? false)).GroupBy(g=>g.rlkMenuTypeID).Count();
                    
                    if (_order != null)
                        menuData.Order = Convert.ToInt32(_order) + 1;
                    else
                        menuData.Order = 0;

                    var _menu = CreateMenu(menuData);
                    _context.Menus.Add(_menu);

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
        /// inserts Form (User Control) data into the database
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public bool Add(FormData[] formData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    foreach (var _formData in formData)
                    {
                        var _frm = CreateForm(_formData);
                        _context.Forms.Add(_frm);
                    }

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
        /// Inserts Menu Role into database
        /// </summary>
        /// <param name="roleID">roleID to inser</param>
        /// <param name="selectedMenuIDs">list of menu ids</param>
        /// <returns></returns>
        public bool Add(int roleID, List<int> selectedMenuIDs)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    //deletes all records with the specified roleID 

                    var _role = _context.Roles.FirstOrDefault(r => r.id == roleID);
                    //var _menu = _context.Menus.ToList();

                    foreach (var _m in _role.Menus.ToList())
                    {
                        _role.Menus.Remove(_m);
                    }

                    //adds the records but even blank is data
                    if (selectedMenuIDs.Count > 0)
                    {
                        foreach (var _mr in selectedMenuIDs)
                        {
                            _role = _context.Roles.FirstOrDefault(r => r.id == roleID);
                            var _menu = _context.Menus.FirstOrDefault(m => m.id == _mr);

                            _role.Menus.Add(_menu);
                        }
                    }

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
        /// Updates the specified menu
        /// </summary>
        public bool Update(MenuData menuData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _menu = _context.Menus.Single(m => m.id == menuData.ID);

                if (_menu == null)
                    throw new InvalidOperationException("Menu detail could not be found.");

                //sets the updated data
                _menu.caption = menuData.Caption;
                _menu.rlkMenuTypeID = menuData.MenuTypeID;
                _menu.icon = menuData.Icon;
                _menu.isActive = menuData.IsActive;
                _menu.visible = menuData.Visible;
                _menu.disabled = menuData.Disabled;
                _menu.lastUpdatedBy = menuData.LastUpdatedBy;
                _menu.lastUpdatedDate = menuData.LastUpdatedDate;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }


        /// <summary>
        /// Updates menu order through up/down button
        /// </summary>
        /// <param name="menuData">menuData</param>
        /// <param name="orderValue">1 or -1 depending on the button clicked</param>
        /// <returns></returns>
        public bool Update(MenuData menuData, int orderValue)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                //sets the new order value based on orderValue (+1 or -1)
                int _newOrderValue = (menuData.Order + orderValue) < 0 ? 0 : (menuData.Order + orderValue);

                var _prevOrNexMenu = (from m in _context.Menus
                                      where m.parent == menuData.Parent
                                          && orderValue == -1 ? (m.order <= _newOrderValue) : (m.order >= _newOrderValue)
                                          && !(m.isDeleted ?? false)
                                      group m by m.rlkMenuTypeID into g
                                      orderby g.Key ascending //, m.caption descending
                                      select new MenuData {ID = g.Select(m=>m.id).FirstOrDefault(),Parent=g.Select(m=>m.parent).FirstOrDefault(), Caption=g.Select(m=>m.caption).FirstOrDefault() ,Order = g.Select(m=>m.order).FirstOrDefault()});

                if (_prevOrNexMenu == null || _prevOrNexMenu.Count() == 0)
                    return false;
                else
                {
                    foreach (var _pnMenu in _prevOrNexMenu)
                    {
                        _pnMenu.Order = menuData.Order;
                        menuData.Order = _newOrderValue;
                    }
                }

                var _menu = _context.Menus.SingleOrDefault(m => m.id == menuData.ID);
                _menu.order = menuData.Order;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }
        /// <summary>
        /// Deletes menu data from the database
        /// </summary>
        /// <param name="menuID"></param>
        /// <returns>true if the data is deleted successfully; false otherwise</returns>
        public bool Delete(int menuID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _menu = _context.Menus.Single(m => m.id == menuID);
                _menu.isDeleted = true;

                int row = _context.SaveChanges();
                return row > 0;
            }
        }

        /// <summary>
        /// Deletes the selected menu-form associations from the database
        /// </summary>
        /// <param name="IDs">Selected IDs to be deleted</param>
        /// <returns>true if the data is deleted successfully; false otherwise</returns>
        public bool Delete(Dictionary<int, object> IDs)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                foreach (var _f in IDs.Keys)
                {
                    var _menuForms = _context.Forms.SingleOrDefault(mf => mf.id == (int)_f);

                    if (_menuForms != null)
                        _context.Forms.Remove(_menuForms);
                }

                int row = _context.SaveChanges();
                return row > 0;
            }
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Menus.Count(m => !(m.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        /// <summary>
        /// Returns Menu data from the database
        /// </summary>
        /// <returns>IEnumerable MenuData</returns>
        public IEnumerable<MenuData> GetData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _menus = (from m in _context.Menus
                              join mt in _context.MenuTypes on m.rlkMenuTypeID equals mt.lkMenuTypeID
                              where m.isDeleted != true
                              orderby m.order, m.rlkMenuTypeID, m.caption
                              //select m).ToList();
                              select new MenuData
                              {
                                  ID = m.id,
                                  CaptionType = m.caption + " (" + mt.type + ")",
                                  Caption = m.caption,
                                  MenuTypeID = m.rlkMenuTypeID,
                                  MenuType = mt.type,
                                  Parent = m.parent,
                                  ParentCaption = (from p in _context.Menus where p.id == m.parent select p.caption).FirstOrDefault(),
                                  Order = m.order,
                                  Icon = m.icon,
                                  IsActive = m.isActive,
                                  Visible = m.visible,
                                  Disabled = m.disabled,
                                  CreatedBy = m.createdBy,
                                  CreatedDate = m.createdDate,
                                  LastUpdatedBy = m.lastUpdatedBy,
                                  LastUpdatedDate = m.lastUpdatedDate,
                                  IsDeleted = m.isDeleted
                              }).ToList();

                return _menus;
            }
        }

        /// <summary>
        /// Returns active Menu from the database
        /// </summary>
        /// <returns>IEnumerable MenuData</returns>
        public IEnumerable<MenuData> GetActiveMenu()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _menus = (from m in _context.Menus
                              join mt in _context.MenuTypes on m.rlkMenuTypeID equals mt.lkMenuTypeID
                              where m.isDeleted != true && m.isActive
                              orderby m.caption, m.order, m.rlkMenuTypeID
                              select new MenuData
                              {
                                  ID = m.id,
                                  CaptionType = m.caption + " (" + mt.type + ")",
                                  Caption = m.caption,
                                  MenuTypeID = m.rlkMenuTypeID,
                                  MenuType = mt.type,
                                  Parent = m.parent,
                                  ParentCaption = (from p in _context.Menus where p.id == m.parent select p.caption).FirstOrDefault(),
                                  Icon = m.icon,
                                  IsActive = m.isActive,
                                  Visible = m.visible,
                                  Disabled = m.disabled,
                                  CreatedBy = m.createdBy,
                                  CreatedDate = m.createdDate,
                                  LastUpdatedBy = m.lastUpdatedBy,
                                  LastUpdatedDate = m.lastUpdatedDate,
                                  IsDeleted = m.isDeleted
                              }).ToList();

                return _menus;
            }
        }

        /// <summary>
        /// Returns active Menus thare are not associated with a form
        /// </summary>
        /// <returns>IEnumerable MenuData</returns>
        public IEnumerable<MenuData> GetActiveUnassociatedMenu()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _menuForm = from mf in _context.Forms select mf.menuID;

                //TODO: check count and show/hide comma
                //String.Format("{0} ({1}{2}{3})", m.caption,(from p in _context.Menus where p.id == m.parent select p.caption).FirstOrDefault(), (from p in _context.Menus where p.id == m.parent select p.caption).Count()>0?", ":"", mt.type)

                var _menus = (from m in _context.Menus
                              join mt in _context.MenuTypes on m.rlkMenuTypeID equals mt.lkMenuTypeID
                              where m.isDeleted != true && m.isActive && !_menuForm.Contains(m.id)
                              orderby m.caption, m.order, m.rlkMenuTypeID
                              select new MenuData
                              {
                                  ID = m.id,
                                  CaptionType = m.caption + " (" + (from p in _context.Menus where p.id == m.parent select p.caption).FirstOrDefault() + ", " + mt.type + ")",
                                  Caption = m.caption,
                                  MenuTypeID = m.rlkMenuTypeID,
                                  MenuType = mt.type,
                                  Parent = m.parent,
                                  ParentCaption = (from p in _context.Menus where p.id == m.parent select p.caption).FirstOrDefault(),
                                  Icon = m.icon,
                                  IsActive = m.isActive,
                                  Visible = m.visible,
                                  Disabled = m.disabled,
                                  CreatedBy = m.createdBy,
                                  CreatedDate = m.createdDate,
                                  LastUpdatedBy = m.lastUpdatedBy,
                                  LastUpdatedDate = m.lastUpdatedDate,
                                  IsDeleted = m.isDeleted
                              }).ToList();

                return _menus.Count()>0 ? _menus:null;
            }
        }

        /// <summary>
        /// Returns the Navigation menu (navigation bar and/or ribbon) to show on the main screen
        /// </summary>
        /// <param name="userID">The logged in userID</param>
        /// <returns></returns>
        public IEnumerable<MenuData> GetNavigationMenu(string userID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                //the role ID of the logged in user

                var _roleID = (from u in _context.Users
                              where u.userID == userID
                              select u.roleID).FirstOrDefault();

                var _menus = (from m in _context.Menus
                              join mt in _context.MenuTypes on m.rlkMenuTypeID equals mt.lkMenuTypeID

                              //from r in m.Roles
                              where m.isDeleted != true && m.isActive && m.Roles.Any(r =>r.id== _roleID)
                              orderby m.order, m.rlkMenuTypeID ,m.caption, m.parent
                              select new MenuData
                              {
                                  ID = m.id,
                                  Caption = m.caption,
                                  CaptionType = m.caption + " (" + mt.type + ")",
                                  MenuTypeID = m.rlkMenuTypeID,
                                  MenuType = mt.type,
                                  Order = m.order,
                                  Parent = m.parent,
                                  ParentCaption = (from p in _context.Menus where p.id == m.parent select p.caption).FirstOrDefault(),
                                  Icon = m.icon,
                                  IsActive = m.isActive,
                                  Visible = m.visible,
                                  Disabled = m.disabled,

                                  CreatedBy = m.createdBy,
                                  CreatedDate = m.createdDate,
                                  LastUpdatedBy = m.lastUpdatedBy,
                                  LastUpdatedDate = m.lastUpdatedDate,
                                  IsDeleted = m.isDeleted
                              }).ToList();

                return _menus;
            }
        }

        /// <summary>
        /// Returns the form to show when a navBar item or ribbon item is selected
        /// </summary>
        /// <param name="menuCaption"></param>
        /// <returns></returns>
        public string GetFormToShow(int menuId)//string menuCaption)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _navBarItem = (from m in _context.Menus
                                   join mt in _context.MenuTypes on m.rlkMenuTypeID equals mt.lkMenuTypeID
                                   where m.id == menuId
                                   select m).ToList();

                if (_navBarItem != null)
                {
                    int _navBarItemID = 0;

                    foreach (var _bt in _navBarItem)
                    {
                        _navBarItemID = _bt.id;
                    }

                    var _result = _context.Forms.Where(f => f.Menu.id == _navBarItemID).ToList();

                    if (_result != null)
                    {
                        foreach (var _r in _result)
                            return _r.formID;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Creates MenuData to be returned to GetData call
        /// </summary>
        /// <returns>MenuData</returns>
        internal MenuData CreateMenuData(Menu source)
        {
            if (source == null)
                return null;

            MenuData _retValue = new MenuData();
            _retValue.ID = source.id;
            _retValue.Caption = source.caption;
            _retValue.Parent = source.parent;
            _retValue.MenuTypeID = source.rlkMenuTypeID;
            _retValue.Icon = source.icon;
            _retValue.IsActive = source.isActive;
            _retValue.Visible = source.visible;
            _retValue.Disabled = source.disabled;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;

            return _retValue;
        }

        /// <summary>
        /// Returns MenuType object from the MenuData provided 
        /// </summary>
        /// <param name="source">MenuData</param>
        /// <returns>Menu Type</returns>
        internal Menu CreateMenu(MenuData source)
        {
            if (source == null)
                return null;
            var _menu = new Menu()
            {
                caption = source.Caption,
                rlkMenuTypeID = source.MenuTypeID,
                icon = source.Icon,
                parent = source.Parent,
                order = source.Order,
                visible = source.Visible,
                disabled = source.Disabled,
                isActive = source.IsActive,
                createdBy = source.CreatedBy,
                createdDate = source.CreatedDate
            };

            return _menu;
        }

        /// <summary>
        /// Creates Form data
        /// </summary>
        /// <returns>FormData</returns>
        internal Form CreateForm(FormData source)
        {
            if (source == null)
                return null;

            var frm = new Form()
            {

                id = source.ID,
                menuID = source.MenuID,
                formID = source.FormID,
                formName = source.FormName
            };

            return frm;
        }

        /// <summary>
        /// returns true if the specified menu name already exists in the database
        /// </summary>
        /// <param name="type">menu name</param>
        /// <returns></returns>
        public bool Validate(string caption, int menuTypeID, int parentID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                return _context.Menus.Where(m => m.caption.Equals(caption) && m.rlkMenuTypeID.Equals(menuTypeID) && m.parent.Equals(parentID) && !(m.isDeleted ?? false)).Count() > 0 ? false : true;
            }
        }

        /// <summary>
        /// Checks whether the form is already assigned to a menu or not
        /// </summary>
        /// <returns></returns>
        public bool IsFormAlreadyAssigned(string formID)
        {
            if (formID == null)
                throw new InvalidOperationException("Invalid form name");

            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                return (_context.Forms.Where(uc => uc.formID == formID).Count() > 0) ? true : false;
            }
        }

        /// <summary>
        /// Returns form (user control) data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FormData> GetFormData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _forms = (from m in _context.Menus
                              join mt in _context.MenuTypes on m.rlkMenuTypeID equals mt.lkMenuTypeID
                              join f in _context.Forms on m.id equals f.menuID
                              orderby m.caption, mt.type, f.formName
                              select new FormData
                              {
                                  ID = f.id,
                                  MenuID = f.menuID,
                                  MenuCaption = m.caption + "(" + mt.type + ")",
                                  MenuParentCaption = (from p in _context.Menus where p.id == m.parent select p.caption).FirstOrDefault(),
                                  FormID = f.formID,
                                  FormName = f.formName
                              }).ToList();

                return _forms;
            }
        }


        /// <summary>
        /// Returns active parent menu elements
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MenuData> GetActiveParentMenu()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _excludeMenuTypeID = from mt in _context.MenuTypes where mt.type.Equals("Item") || mt.type.Equals("Menu Item") || mt.type.Equals("Button") select mt.lkMenuTypeID;
                var _menus = (from m in _context.Menus
                              join mt in _context.MenuTypes on m.rlkMenuTypeID equals mt.lkMenuTypeID
                              where !(m.isDeleted ?? false)
                                  && m.isActive && !_excludeMenuTypeID.Contains(m.rlkMenuTypeID)
                              select new MenuData
                              {
                                  ID = m.id,
                                  MenuTypeID = m.rlkMenuTypeID,
                                  CaptionType = m.caption + ((from p in _context.MenuTypes where p.lkMenuTypeID == m.parent select p.type).Count() > 0 ? " - " + (from p in _context.MenuTypes where p.lkMenuTypeID == m.parent select p.type).FirstOrDefault() : ""),
                                  Caption = m.caption,
                                  ParentCaption = (from _p in _context.Menus
                                                   join _mt in _context.MenuTypes on _p.rlkMenuTypeID equals _mt.lkMenuTypeID
                                                   where _p.id == m.parent
                                                   select new { caption = _p.caption + " (" + _mt.type + ")" }).FirstOrDefault().caption,
                              }).ToList();

                return _menus;
            }
        }

        /// <summary>
        /// Returns active parent menu elements
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MenuData> GetFilteredParentMenus(int menuTypeID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _selectedMenuTypeParent = from mt in _context.MenuTypes where mt.lkMenuTypeID == menuTypeID select mt.parent;

                var _parent = (from m in _context.Menus where !(m.isDeleted ?? false) && m.isActive && _selectedMenuTypeParent.Contains(m.rlkMenuTypeID) select m).ToList();

                List<MenuData> result = new List<MenuData>();
                foreach (var _mt in _parent)
                {
                    result.Add(CreateMenuData(_mt));
                }
                return result;
            }
        }

        /// <summary>
        /// Returns Menu Role data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MenuRoleData> GetMenuRoles(int roleID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _result = (from m in _context.Menus
                               join mt in _context.MenuTypes on m.rlkMenuTypeID equals mt.lkMenuTypeID
                               where m.isActive && !(m.isDeleted ?? false)
                               select new MenuRoleData
                               {
                                   MenuID = m.id,
                                   MenuCaption = m.caption,
                                   MenuCaptionType = m.caption + " (" + mt.type + ")",
                                   MenuType = mt.type,
                                   Applies = (_context.Roles.Where(mr => mr.Menus.Any(mrm => mrm.id == m.id) && mr.id == roleID).Count() > 0) ? true : false
                               }).ToList();
                return _result;
            }
        }

        /// <summary>
        /// This method accepts byte and returns an image
        /// </summary>
        /// <param name="_byte"></param>
        /// <returns></returns>
        public MemoryStream ByteToImage(byte[] _byte)
        {
            if (_byte == null)
                return null;

            MemoryStream image = new MemoryStream(_byte);
            return image;
        }

        #endregion
    }

    public class MenuData
    {
        public int ID { get; set; }
        public int MenuTypeID { get; set; }
        public string Caption { get; set; }
        public string CaptionType { get; set; }
        public byte[] Icon { get; set; }
        public string MenuType { get; set; }
        public int Parent { get; set; }
        public string ParentCaption { get; set; }
        public int Order { get; set; }
        public bool Visible { get; set; }
        public bool Disabled { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }

    public class MenuRoleData
    {
        public int MenuID { get; set; }
        public int RoleID { get; set; }
        public string MenuCaption { get; set; }
        public string MenuCaptionType { get; set; }
        public string MenuType { get; set; }
        public bool Applies { get; set; }
    }

    public class FormData
    {
        public int ID { get; set; }
        public int MenuID { get; set; }
        public string MenuParentCaption { get; set; }
        public string MenuCaption { get; set; }
        public string FormID { get; set; }
        public string FormName { get; set; }
    }
}
