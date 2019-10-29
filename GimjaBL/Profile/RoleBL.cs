using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class RoleBL
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

        public RoleBL()
        {
            HasData();
        }

        #region Methods

        /// <summary>
        /// Inserts Role data to tblRole
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(RoleData roleData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    //int _priority = _context.Roles.Where(r => r.priority == roleData.Priority && !(r.isDeleted ?? false)).GroupBy(g => g.id).Count();

                    //if (_priority > 0 )
                    //    roleData.Priority = (short)(_priority + 1);
                    //else
                    //    roleData.Priority = _context.Roles.Where(r=>!(r.isDeleted ?? false)).Count()>0 ? (short)_context.Roles.Where(r=>!(r.isDeleted ?? false)).Count(): (short)1;

                    var _role = CreateRole(roleData);
                    _context.Roles.Add(_role);

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
        /// Updates Role data on tblRole
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(RoleData roleData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _role = _context.Roles.Single(i => i.id == roleData.ID);

                if (_role == null)
                    throw new InvalidOperationException("Role detail could not be found.");

                //sets the new role data
                _role.roleName = roleData.RoleName;
                _role.description = roleData.Description;
                _role.isActive = roleData.IsActive;

                _role.lastUpdatedBy = Singleton.Instance.UserID;
                _role.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Deletes Role data from tblRole table
        /// </summary>
        /// <param name="roleID">The current role's ID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(int roleID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _role = _context.Roles.SingleOrDefault(c => c.id == roleID);

                if (_role == null)
                    throw new InvalidOperationException("The role you are trying to delete doesn't exist");

                _role.isDeleted = true;
                _role.lastUpdatedBy = Singleton.Instance.UserID;
                _role.lastUpdatedDate = DateTime.Now;

                //delete the user
                var _user = _context.Users.SingleOrDefault(u => u.roleID == roleID);
                _context.Users.Remove(_user);

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Returns Role data from tblRole table
        /// </summary>
        /// <returns>IEnumerable RoleData</returns>
        public IEnumerable<RoleData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _role;

                if (isActive)
                {
                    _role = _context.Roles.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).ToList();
                }
                else
                {
                    _role = _context.Roles.Where(b => !(b.isDeleted ?? false)).ToList();
                }

                List<RoleData> _result = new List<RoleData>();
                foreach (var _r in _role)
                {
                    _result.Add(CreateRoleData(_r));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates RoleData to be return for GetData call
        /// </summary>
        /// <returns>RoleData</returns>
        internal RoleData CreateRoleData(Role source)
        {
            if (source == null)
                return null;

            RoleData _retValue = new RoleData();

            _retValue.ID = source.id;
            _retValue.RoleName = source.roleName;
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
        /// Returns Role object from the RoleData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Role CreateRole(RoleData source)
        {
            if (source == null)
                return null;
            var _role = new Role()
            {
                id = source.ID,
                roleName =  source.RoleName,
                description = source.Description,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            return _role;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.Roles.Count(i => !(i.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        public static IEnumerable<RoleData> GetActiveRoles()
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _roles = (from c in db.Roles
                                  where c.isActive && !(c.isDeleted ?? false)
                                  select c).ToList();

                List<RoleData> _result = new List<RoleData>();
                _roles.ForEach(c => _result.Add(CreateRoleObject(c)));

                return _result;
            }
        }

        private static RoleData CreateRoleObject(Role c)
        {
            return new RoleData
            {
                ID = c.id,
                RoleName = c.roleName,
                Description = c.description,
                IsActive = c.isActive
            };
        }

        /// <summary>
        /// Checks if the new role name already exists
        /// </summary>
        /// <param name="roleName">The new role name to be added</param>
        /// <returns>true if the new role name does not exist. False, otherwise</returns>
        public bool IsValid(string roleName)
        {
            if (!String.IsNullOrWhiteSpace(roleName))
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    return (_context.Roles.Where(r=>r.roleName.Equals(roleName) && !(r.isDeleted ?? false)).Count()>0 ? false : true);
                }
            }

            return false;
        }

        /// <summary>
        ///// Moves the priority of the role one step up/down
        ///// </summary>
        ///// <param name="roleData">role to be updated</param>
        ///// <param name="priority">1 for Up and -1 for Down
        ///// <returns></returns>
        //public bool UpdateRolePriority(RoleData roleData, int priority)
        //{
        //    if (roleData != null)
        //    {
        //        using (var _context = new eDMSEntity("eDMSEntities"))
        //        {
        //            //id and priority of the role to be changed
        //            int _roleID = 0;
        //            short _priority = 0;
 
        //            //sets the new order value based on priority value (+1 or -1)
        //            short _newPriorityValue = (roleData.Priority + priority) < 0 ? (short)1 : (short)(roleData.Priority + priority);
        //            if (_newPriorityValue >= 1)
        //            {
        //                var _prevOrNexRole = (from r in _context.Roles
        //                                      where //r.priority == roleData.Priority && 
        //                                          //priority == -1 ? (r.priority <= _newPriorityValue) : (r.priority >= _newPriorityValue)
        //                                          r.priority == _newPriorityValue
        //                                          && !(r.isDeleted ?? false)
        //                                      group r by r.id into g
        //                                      orderby g.Key ascending
        //                                      select new RoleData { ID = g.Select(r => r.id).FirstOrDefault(), Priority = g.Select(r => r.priority).FirstOrDefault(), RoleName = g.Select(r => r.roleName).FirstOrDefault(), Description = g.Select(r => r.description).FirstOrDefault() });

        //                if (_prevOrNexRole == null || _prevOrNexRole.Count() == 0)
        //                    roleData.Priority = _newPriorityValue;
        //                else
        //                {
        //                    foreach (var _pnRole in _prevOrNexRole)
        //                    {
        //                        _priority = roleData.Priority;
        //                        roleData.Priority = _newPriorityValue;

        //                        _roleID = _pnRole.ID;
        //                    }
        //                }

        //                var _role = _context.Roles.Where(r => r.id == roleData.ID || r.id== _roleID).ToList();
        //                foreach (var _r in _role)
        //                {
        //                    if (_r.id == roleData.ID)
        //                        _r.priority = roleData.Priority;
        //                    else
        //                        _r.priority = _priority;
        //                }

        //                int _row = _context.SaveChanges();
        //                return _row > 0;
        //            }
        //            else
        //                return false;
        //        }
        //    }
        //    else
        //        return false;
        //}

        /// <summary>
        /// Returns the number of user accounts having the specified user role
        /// </summary>
        /// <param name="roleID">roleID to delete</param>
        /// <returns>the number of user accounts</returns>
        public int UserAccountsWithRole(int roleID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                return _context.Users.Where(u => u.roleID == roleID).Count();
            }
        }
        
        #endregion
    }
    

    public class RoleData
    {
        public int ID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
