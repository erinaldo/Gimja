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
    
    public partial class Supplier
    {
        public Supplier()
        {
            this.Receipts = new HashSet<Receipt>();
            this.TelephoneFaxes = new HashSet<TelephoneFax>();
        }
    
        public string lkSupplierID { get; set; }
        public string companyName { get; set; }
        public string contactPerson { get; set; }
        public string description { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }
        public System.DateTime createdDate { get; set; }
        public string lastUpdatedBy { get; set; }
        public Nullable<System.DateTime> lastUpdatedDate { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public Nullable<System.Guid> addressID { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual ICollection<Receipt> Receipts { get; set; }
        public virtual ICollection<TelephoneFax> TelephoneFaxes { get; set; }
    }
}
