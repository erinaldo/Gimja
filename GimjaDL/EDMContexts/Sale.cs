//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GimjaDL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sale
    {
        public Sale()
        {
            this.CreditPayments = new HashSet<CreditPayment>();
            this.SalesDetails = new HashSet<SalesDetail>();
            this.SalesReturns = new HashSet<SalesReturn>();
        }
    
        public System.Guid id { get; set; }
        public string branchID { get; set; }
        public System.DateTime date { get; set; }
        public string customerID { get; set; }
        public string processedBy { get; set; }
        public string receiptID { get; set; }
        public string referenceNo { get; set; }
        public string authorizedBy { get; set; }
        public string createdBy { get; set; }
        public System.DateTime createdDate { get; set; }
        public string lastUpdatedBy { get; set; }
        public Nullable<System.DateTime> lastUpdatedDate { get; set; }
        public Nullable<bool> isVoid { get; set; }
        public bool isSalesCredit { get; set; }
        public string refNo { get; set; }
        public string reference { get; set; }
        public string fsNo { get; set; }
        public string refNote { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual ICollection<CreditPayment> CreditPayments { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ItemRequest ItemRequest { get; set; }
        public virtual ICollection<SalesDetail> SalesDetails { get; set; }
        public virtual ICollection<SalesReturn> SalesReturns { get; set; }
    }
}