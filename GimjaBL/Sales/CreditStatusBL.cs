using GimjaDL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GimjaBL
{
    public class CreditStatusBL
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

        //public short ID { get; set; }
        //public string Name { get; set; }
        //public string Description { get; set; }
        //public bool IsActive { get; set; }

        public CreditStatusBL()
        {
            HasData();
        }

        public static IList<CreditStatusData> GetCreditStatusList(bool isActive = false)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _status = (from s in db.CreditStatus
                               where !(s.isDeleted ?? false)
                               select new CreditStatusData //CreditStatusBL()
                               {
                                   CreditStatusID = s.lkCreditStatusID,
                                   Name = s.name,
                                   Description = s.description,
                                   IsActive = s.isActive
                               }).ToList();
                if (isActive)
                    _status = _status.Where(x => x.IsActive).ToList();

                return _status;
            }
        }

        public static CreditStatusData GetCreditStatus(short id)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _status = (from s in db.CreditStatus
                               where !(s.isDeleted ?? false) && s.lkCreditStatusID == id
                               select new CreditStatusData//CreditStatusBL()
                               {
                                   CreditStatusID = s.lkCreditStatusID,
                                   Name = s.name,
                                   Description = s.description,
                                   IsActive = s.isActive
                               }).SingleOrDefault();

                return _status;
            }
        }

        public static CreditStatusData GetCreditStatus(string name)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _status = (from s in db.CreditStatus
                               where !(s.isDeleted ?? false) && s.name.ToLower().Equals(name.ToLower())
                               select new CreditStatusData//CreditStatusBL()
                               {
                                   CreditStatusID = s.lkCreditStatusID,
                                   Name = s.name,
                                   Description = s.description,
                                   IsActive = s.isActive
                               }).SingleOrDefault();

                return _status;
            }
        }

        public static CreditStatusData GetPendingCreditStatus()
        {
            var _status = GetCreditStatus("pending");//1
            return _status;
        }
        public static CreditStatusData GetPaidCreditStatus()
        {
            var _status = GetCreditStatus("paid");//2
            return _status;
        }
        public static CreditStatusData GetWrittenOffCreditStatus()
        {
            var _status = GetCreditStatus("written off");//3
            return _status;
        }
        public static CreditStatusData GetOverdueCreditStatus()
        {
            var _status = GetCreditStatus("over due");//4
            return _status;
        }
        public static CreditStatusData GetPartiallyPaidCreditStatus()
        {
            var _status = GetCreditStatus("partially paid");//5
            return _status;
        }

        public List<SalesCreditStatusData> GetList()
        {
            using (var db = new eDMSEntity())
            {
                var _salesList = from s in db.Sales.Include("Customer")
                                 join sd in db.SalesDetails.Include("Item") on s.id equals sd.salesID
                                 join cp in db.CreditPayments.Include("CreditStatus") on s.id equals cp.salesID
                                 //let pmts = s.CreditPayments.Where(cp => cp.rlkCreditStatusID == 1)
                                 where s.isSalesCredit && cp.rlkCreditStatusID == 1// pmts.Any(p => p.salesID == s.id)//(s.CreditPayments.rlkCreditStatusID == 1)//only pending credit sales
                                 select new SalesCreditStatusData
                                 {
                                     ReferenceNo = s.referenceNo,
                                     CustomerName = s.Customer.name ?? s.Customer.companyName,
                                     CreditStatus = cp.CreditStatus.name,//s.CreditPayment.CreditStatus.name,
                                     CreditStatusID = cp.rlkCreditStatusID,//s.CreditPayment.rlkCreditStatusID,
                                     SalesDate = s.date,
                                     Item = sd.Item.itemID,
                                     TotalPrice = (sd.Item.unitPrice - (sd.discount ?? 0d)) * sd.quantity
                                 };

                return _salesList.ToList();
            }
        }

        public static SalesCreditStatusData GetCreditStatus(string refNo, string itemId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _salesCreditStatus = (from s in db.Sales.Include("Customer")
                                          join sd in db.SalesDetails.Include("Item") on s.id equals sd.salesID
                                          join cp in db.CreditPayments.Include("CreditStatus") on s.id equals cp.salesID
                                          where s.isSalesCredit && s.referenceNo == refNo && sd.itemID.Equals(itemId)
                                          select new SalesCreditStatusData
                                          {
                                              ReferenceNo = s.referenceNo,
                                              CustomerName = s.Customer.name ?? s.Customer.companyName,
                                              CreditStatus = cp.CreditStatus.name,//s.CreditPayment.CreditStatus.name,
                                              CreditStatusID = cp.rlkCreditStatusID,//s.CreditPayment.rlkCreditStatusID,
                                              SalesDate = s.date,
                                              Item = sd.Item.itemID,
                                              TotalPrice = (sd.Item.unitPrice - (sd.discount ?? 0d)) * sd.quantity
                                          }).SingleOrDefault();

                return _salesCreditStatus;
            }
        }

        public static IList<SalesCreditStatusData> GetCreditPayments(Guid salesId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _salesCreditStatusList = (from s in db.Sales.Include("Customer")
                                              join sd in db.SalesDetails.Include("Item") on s.id equals sd.salesID
                                              join cp in db.CreditPayments.Include("CreditStatus") on s.id equals cp.salesID
                                              where s.id == salesId
                                              select new SalesCreditStatusData
                                              {
                                                  ReferenceNo = s.referenceNo,
                                                  CustomerName = s.Customer.name ?? s.Customer.companyName,
                                                  CreditStatus = cp.CreditStatus.name,//s.CreditPayment.CreditStatus.name,
                                                  CreditStatusID = cp.rlkCreditStatusID,//s.CreditPayment.rlkCreditStatusID,
                                                  SalesDate = s.date,
                                                  Item = sd.itemID,
                                                  TotalPrice = (sd.unitPrice - (sd.discount ?? 0d)) * sd.quantity
                                              }).ToList();

                return _salesCreditStatusList;
            }
        }

        /// <summary>
        /// Inserts CreditStatus data to lkCreditStatus
        /// </summary>
        /// <returns>True if data is successfully added. False, otherwise</returns>
        public bool Add(CreditStatusData creditStatusData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                try
                {
                    var _creditStatusData = CreateCreditStatus(creditStatusData);
                    _context.CreditStatus.Add(_creditStatusData);

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
        /// Updates CreditStatus data on lkCreditStatus
        /// </summary>
        /// <returns>Returns true if data is successfully updated. False, otherwise</returns>
        public bool Update(CreditStatusData creditStatusData)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _creditStatusData = _context.CreditStatus.Single(c => c.lkCreditStatusID == creditStatusData.CreditStatusID);

                if (_creditStatusData == null)
                    throw new InvalidOperationException("Credit Status detail could not be found.");

                //sets the new credit status data
                _creditStatusData.name = creditStatusData.Name;
                _creditStatusData.description = creditStatusData.Description;
                _creditStatusData.isActive = creditStatusData.IsActive;

                _creditStatusData.lastUpdatedBy = Singleton.Instance.UserID;
                _creditStatusData.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Deletes CreditStatus data from lkCreditStatus table
        /// </summary>
        /// <param name="creditStatusID">The current credit status's CreditStatusID</param>
        /// <returns>true if delete is successfull. False, otherwise</returns>
        public bool Delete(int creditStatusID)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                var _creditStatusData = _context.CreditStatus.SingleOrDefault(c => c.lkCreditStatusID == creditStatusID);

                if (_creditStatusData == null)
                    throw new InvalidOperationException("The credit status you are trying to delete doesn't exist");

                _creditStatusData.isDeleted = true;
                _creditStatusData.lastUpdatedBy = Singleton.Instance.UserID;
                _creditStatusData.lastUpdatedDate = DateTime.Now;

                int _row = _context.SaveChanges();
                return _row > 0;
            }
        }

        /// <summary>
        /// Returns CreditStatus data from lkCreditStatus table
        /// </summary>
        /// <returns>IEnumerable CreditStatusData</returns>
        public IEnumerable<CreditStatusData> GetData(bool isActive = false)
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                dynamic _creditStatusData;

                if (isActive)
                {
                    _creditStatusData = _context.CreditStatus.Where(b => !(b.isDeleted ?? false) && b.isActive.Equals(isActive)).OrderBy(r => r.name).ToList();
                }
                else
                {
                    _creditStatusData = _context.CreditStatus.Where(b => !(b.isDeleted ?? false)).OrderBy(r => r.name).ToList();
                }

                List<CreditStatusData> _result = new List<CreditStatusData>();
                foreach (var _r in _creditStatusData)
                {
                    _result.Add(CreateCreditStatusData(_r));
                }

                return _result;
            }
        }

        /// <summary>
        /// Creates CreditStatusData to be return for GetData call
        /// </summary>
        /// <returns>CreditStatusData</returns>
        internal CreditStatusData CreateCreditStatusData(CreditStatus source)
        {
            if (source == null)
                return null;

            CreditStatusData _retValue = new CreditStatusData();

            _retValue.CreditStatusID = source.lkCreditStatusID;
            _retValue.Name = source.name;
            _retValue.IsActive = source.isActive;
            _retValue.Description = source.description;
            _retValue.CreatedBy = source.createdBy;
            _retValue.CreatedDate = source.createdDate;
            _retValue.LastUpdatedBy = source.lastUpdatedBy;
            _retValue.LastUpdatedDate = source.lastUpdatedDate;
            _retValue.IsDeleted = source.isDeleted;
            return _retValue;
        }

        /// <summary>
        /// Returns CreditStatus object from the CreditStatusData provided 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static CreditStatus CreateCreditStatus(CreditStatusData source)
        {
            if (source == null)
                return null;
            var _creditStatusData = new CreditStatus()
            {
                lkCreditStatusID = source.CreditStatusID,
                name = source.Name,
                description = source.Description,
                isActive = source.IsActive,
                createdBy = Singleton.Instance.UserID,
                createdDate = DateTime.Now
            };

            return _creditStatusData;
        }

        /// <summary>
        /// Sets IsDataAvailable to true if there is data, false otherwise
        /// </summary>
        public void HasData()
        {
            using (var _context = new eDMSEntity("eDMSEntities"))
            {
                IsDataAvailable = (_context.CreditStatus.Count(c => !(c.isDeleted ?? false)) == 0 ? false : true);
            }
        }

        /// <summary>
        /// Checks if the new credit status name already exists
        /// </summary>
        /// <param name="name">The new credit status name to be added</param>
        /// <returns>true if the new credit status name does not exist. False, otherwise</returns>
        public bool IsValid(string name)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                using (var _context = new eDMSEntity("eDMSEntities"))
                {
                    return (_context.CreditStatus.Where(r => r.name.Equals(name) && !(r.isDeleted ?? false)).Count() > 0 ? false : true);
                }
            }

            return false;
        }

    }

    public class CustomerLedgerBL
    {
        /// <summary>
        /// Gets the list of customer ledger info
        /// </summary>
        /// <param name="pendingOnly">dertermines whether pending only customer ledger records are needed</param>
        /// <returns></returns>
        public List<CustomerLedgerData> GetCustomerLedgers(bool pendingOnly = true)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _custLedger = (from c in db.CustomerLedgers
                                   where c.isActive
                                   select c);
                if (pendingOnly)
                    _custLedger = _custLedger.Where(c => c.lkCreditStatusID == 1);//TODO: ENSURE THE PENDING CREDIT STATUS HAS ID 1

                var _custLedgerList = _custLedger.ToList();
                var _retValue = new List<CustomerLedgerData>();
                _custLedgerList.ForEach(c => _retValue.Add(CreateCustomerLedgerObject(c)));

                return _retValue;
            }
        }
        /// <summary>
        /// Gets the list of customer ledger objects for new entry
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        public List<CustomerLedgerData> GetNewCustomerLedgers(string branch = "")
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                if (!string.IsNullOrEmpty(branch))
                {
                    var _existingLedgers = (from cl in db.CustomerLedgers
                                            join s in db.Sales on cl.referenceNo equals s.referenceNo
                                            where cl.isActive && s.branchID.Equals(branch)
                                            group cl by cl.referenceNo into RefLedgers
                                            let f = RefLedgers.FirstOrDefault()
                                            where RefLedgers.Any(r => r.lkCreditStatusID == 1) &&
                                            f != null && RefLedgers.Sum(r => r.amountPaid) < f.receivable//the pending status
                                            select new CustomerLedgerData()
                                            {
                                                ID = f.id,
                                                CustomerID = f.customerID,
                                                ReferenceNo = f.referenceNo,
                                                Date = DateTime.Today,
                                                Receivable = f.receivable,
                                                AmountPaid = 0,
                                                Remaining = (f.receivable - RefLedgers.Sum(r => r.amountPaid)),
                                                CreditStatusID = 1
                                            }).ToList();
                    return _existingLedgers;
                }
                else
                {
                    var _existingLedgers = (from cl in db.CustomerLedgers
                                            where cl.isActive
                                            group cl by cl.referenceNo into RefLedgers
                                            let f = RefLedgers.FirstOrDefault()
                                            where RefLedgers.Any(r => r.lkCreditStatusID == 1) &&
                                            f != null && RefLedgers.Sum(r => r.amountPaid) < f.receivable//the pending status
                                            select new CustomerLedgerData()
                                            {
                                                ID = f.id,
                                                CustomerID = f.customerID,
                                                ReferenceNo = f.referenceNo,
                                                Date = DateTime.Today,
                                                Receivable = f.receivable,
                                                AmountPaid = 0,
                                                Remaining = (f.receivable - RefLedgers.Sum(r => r.amountPaid)),
                                                CreditStatusID = 1
                                            }).ToList();
                    return _existingLedgers;
                }
            }
        }

        private static CustomerLedgerData CreateCustomerLedgerObject(CustomerLedger c)
        {
            return new CustomerLedgerData()
            {
                ID = c.id,
                CustomerID = c.customerID,
                ReferenceNo = c.referenceNo,
                Date = c.date,
                Receivable = c.receivable,
                AmountPaid = c.amountPaid,
                CreditStatusID = c.lkCreditStatusID
            };
        }
        /// <summary>
        /// Gets the customer ledger records by the specified sales reference number
        /// </summary>
        /// <param name="referenceNo">the ref # of the sales</param>
        /// <returns></returns>
        public List<CustomerLedgerData> GetCustomerLedgers(string referenceNo)
        {
            if (string.IsNullOrWhiteSpace(referenceNo))
            {
                return null;
            }
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _ledgers = (from c in db.CustomerLedgers
                                where c.isActive && c.referenceNo.Equals(referenceNo)
                                select c).ToList();
                if (_ledgers == null)
                    return null;
                var _retValue = new List<CustomerLedgerData>();
                _ledgers.ForEach(c => _retValue.Add(CreateCustomerLedgerObject(c)));

                return _retValue;
            }
        }

        public bool IsCompleted(string _refNo)
        {
            if (string.IsNullOrEmpty(_refNo))
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _isCompleted = (from cl in db.CustomerLedgers
                                    where cl.isActive && cl.referenceNo.Equals(_refNo)
                                    //let receivable = cl.receivable
                                    group cl by cl.referenceNo into RefLedgers
                                    let f = RefLedgers.FirstOrDefault()
                                    select f != null && RefLedgers.Sum(r => r.amountPaid) >= f.receivable
                                    ).FirstOrDefault();

                return _isCompleted;
            }
        }

        public bool IsCompleted(int id)
        {
            if (id == 0)
                return false;
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                var _isCompleted = (from cl in db.CustomerLedgers
                                    where cl.isActive && cl.id == id
                                    //let receivable = cl.receivable
                                    group cl by cl.referenceNo into RefLedgers
                                    let f = RefLedgers.FirstOrDefault()
                                    select f != null && RefLedgers.Sum(r => r.amountPaid) >= f.receivable
                                    ).FirstOrDefault();

                return _isCompleted;
            }
        }

        public bool InsertLedgers(List<CustomerLedgerData> _modifiedLedgers)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                //create customer ledger record object from params
                var _newLedgers = new List<CustomerLedger>();
                _modifiedLedgers.ForEach(l => _newLedgers.Add(new CustomerLedger()
                {
                    customerID = l.CustomerID,
                    lkCreditStatusID = l.CreditStatusID,
                    referenceNo = l.ReferenceNo,
                    date = l.Date,
                    receivable = l.Receivable,
                    amountPaid = l.AmountPaid
                }));
                //add them to database collection
                db.CustomerLedgers.AddRange(_newLedgers);
                //save to database
                int rows = db.SaveChanges();
                return rows > 0;
            }
        }

        public static bool SaveWriteOff(WriteOffData _writeOff, int ledgerId)
        {
            using (var db = new eDMSEntity("eDMSEntity"))
            {
                CustomerLedgerBL custLogic = new CustomerLedgerBL();
                if (!custLogic.IsCompleted(ledgerId))
                {
                    var _newWriteOff = new WriteOff()
                    {
                        id = _writeOff.ID,
                        customerID = _writeOff.CustomerID,
                        amount = _writeOff.Amount,
                        createdBy = _writeOff.CreatedBy,
                        createdDate = _writeOff.CreatedDate,
                        lastUpdatedBy = _writeOff.LastUpdatedBy,
                        lastUpdatedDate = _writeOff.LastUpdatedDate,
                        isDeleted = _writeOff.IsDeleted
                    };
                    db.WriteOffs.Add(_newWriteOff);
                    //change the customer ledger data
                    var _ledger = db.CustomerLedgers.Where(l => l.id == ledgerId).SingleOrDefault();
                    if (_ledger == null)
                        throw new ArgumentNullException("The customer ledger object cannot be found.");
                    var _writtenOffStatus = CreditStatusBL.GetCreditStatus("written off");
                    if (_writtenOffStatus == null)
                        throw new ArgumentNullException("The Written Off Status is not available.");
                    _ledger.lkCreditStatusID = _writtenOffStatus.CreditStatusID;
                    //save to the database
                    int rows = db.SaveChanges();
                    return rows > 0;
                }
                else
                {
                    throw new InvalidOperationException("The customer ledger is completely paid, no need to write off.");
                }
            }
        }
    }

    public class CustomerLedgerData
    {
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public string ReferenceNo { get; set; }
        public System.DateTime Date { get; set; }
        public decimal Receivable { get; set; }
        public decimal AmountPaid { get; set; }
        public short CreditStatusID { get; set; }
        public bool IsActive { get; set; }

        public string CustomerName { get; set; }
        /// <summary>
        /// Get the remaining amount of credit
        /// </summary>
        public decimal Remaining { get; set; }
    }

    public class SalesCreditStatusData
    {
        public string ReferenceNo { get; set; }
        public string CustomerName { get; set; }
        public string CreditStatus { get; set; }
        public short CreditStatusID { get; set; }
        public DateTime SalesDate { get; set; }
        public string Item { get; set; }
        public double TotalPrice { get; set; }
    }

    public class CreditStatusData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public short CreditStatusID { get; set; }        
    }

    public class WriteOffData
    {
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public decimal Amount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
