//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PantrTest.Models.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_Address
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_Address()
        {
            this.tbl_User = new HashSet<tbl_User>();
        }
    
        public int PK_Address { get; set; }
        public Nullable<int> FK_City { get; set; }
        public string Address { get; set; }
    
        public virtual tbl_City tbl_City { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_User> tbl_User { get; set; }
    }
}
