using GimjaDL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class TelephoneFaxBL
    {
        public TelephoneFaxBL()
        {
        }

        #region Methods

        /// <summary>
        /// Inserts TelephoneFax data to tblTelephoneFax table 
        /// </summary>
        public bool Add(TelephoneFaxData telephoneFaxData, bool isSync = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _telephoneFax = CreateTelephoneFaxObject(telephoneFaxData);

                    if (!isSync && _telephoneFax != null)
                    {
                        var _telephoneFaxElem = SyncTransactionBL.GetTelephoneFaxElement(_telephoneFax);
                        var _sync = new SyncTransaction()
                        {
                            id = Guid.NewGuid(),
                            action = "insert",
                            tableName = "tblTelephoneFax",
                            value = _telephoneFaxElem.ToString(),
                            branchID = "BRC-B-1"//TODO: ADD THE CURRENT BRANCH
                        };
                        _context.SyncTransactions.Add(_sync);
                    }

                    _context.TelephoneFaxes.Add(_telephoneFax);

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
        /// Checks whether the telephoneFax record exists in database
        /// </summary>
        /// <param name="telephoneFaxID">the telephoneFax ID</param>
        /// <returns></returns>
        public bool Exists(int telephoneFaxID)
        {
            if (telephoneFaxID == 0)
                return false;
            using (var db = new eDMSEntity("eDMSEntities"))
            {
                var _exists = db.TelephoneFaxes.Any(tf => tf.id == telephoneFaxID);

                return _exists;
            }
        }

        /// <summary>
        /// Creates TelephoneFaxData to be return for GetData call
        /// </summary>
        /// <returns>TelephoneFaxData</returns>
        internal TelephoneFaxData CreateTelephoneFaxData(TelephoneFax source)
        {
            if (source == null)
                return null;

            TelephoneFaxData _retValue = new TelephoneFaxData();

            _retValue.ID = source.id;
            _retValue.Type = source.type;
            _retValue.Number = source.number;
            _retValue.ParentID = source.parentID;
            _retValue.IsActive = source.isActive;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;

            return _retValue;
        }

        public static TelephoneFax CreateTelephoneFaxObject(TelephoneFaxData source)
        {
            if (source == null)
                return null;
            var _result = new TelephoneFax
            {
                id = source.ID,
                type = source.Type,
                number = source.Number,
                parentID = source.ParentID,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now,
                lastUpdatedBy = source.LastUpdatedBy,
                lastUpdatedDate = source.LastUpdatedDate,
                isActive = source.IsActive,
                isDeleted = source.IsDeleted
            };

            return _result;
        }

        /// <summary>
        /// Returns the telephone/fax details of the specified parentID
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns>TelephoneFaxData</returns>
        public List<TelephoneFaxData> GetTelephoneFax(string parentID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _telephoneFax = _context.TelephoneFaxes.Where(tf => tf.parentID == parentID && !(tf.isDeleted ?? false)).ToList();

                List<TelephoneFaxData> _result = new List<TelephoneFaxData>();
                foreach (var _r in _telephoneFax)
                {
                    _result.Add(CreateTelephoneFaxData(_r));
                }

                if (_telephoneFax != null)
                    return _result; ;

                return null;
            }
        }

        #endregion

    }

    public class TelephoneFaxData
    {
        public int ID { get; set; }
        public short Type { get; set; }
        public string Number { get; set; }
        public string ParentID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
