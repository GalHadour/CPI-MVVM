//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CPI.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class ARFCN: ExtensionARFCN, IEntityWithID
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ARFCN()
        {
            this.Calls = new HashSet<Call>();
            this.SMS = new HashSet<SMS>();
            this.Receivers = new HashSet<Receiver>();
        }
    
        public System.Guid ID { get; set; }
        public Nullable<int> Chanel { get; set; }
        public string Band { get; set; }
        public string Frequency { get; set; }
        public Nullable<int> MCC { get; set; }
        public Nullable<int> MNC { get; set; }
        public Nullable<int> LAC { get; set; }
        public Nullable<int> CI { get; set; }
        public Nullable<int> Power { get; set; }
        public Nullable<int> ScramblingCode { get; set; }
        public string System { get; set; }
        public string Configuration { get; set; }
        public string CellARFCNs { get; set; }
        public string NeighbourCells { get; set; }
        public string Notes { get; set; }
        public Nullable<int> ProviderID { get; set; }
        public Nullable<System.Guid> Session_ID { get; set; }
    
        public virtual Provider Provider { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Call> Calls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SMS> SMS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Receiver> Receivers { get; set; }
        public virtual Session Session { get; set; }
    }
}
