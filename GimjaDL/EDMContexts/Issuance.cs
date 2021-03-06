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
    
    public partial class Issuance
    {
        public Issuance()
        {
            this.IssuedItems = new HashSet<IssuedItem>();
        }
    
        public string id { get; set; }
        public string issuedTo { get; set; }
        public System.DateTime date { get; set; }
        public string issuedBy { get; set; }
        public string approvedBy { get; set; }
        public Nullable<System.DateTime> approvedDate { get; set; }
        public string storeID { get; set; }
        public string warehouseID { get; set; }
        public string createdBy { get; set; }
        public System.DateTime createdDate { get; set; }
        public string lastUpdatedBy { get; set; }
        public Nullable<System.DateTime> lastUpdatedDate { get; set; }
        public Nullable<bool> isDeleted { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual ICollection<IssuedItem> IssuedItems { get; set; }
    }
}
