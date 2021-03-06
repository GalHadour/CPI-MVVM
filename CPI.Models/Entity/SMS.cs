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
    
    public partial class SMS :ExtensionSMS, IEntityWithID
    {
        public System.Guid ID { get; set; }
        public string UnitName { get; set; }
        public string Kc { get; set; }
        public string Rate { get; set; }
        public string TS { get; set; }
        public string SRI { get; set; }
        public string UDHI { get; set; }
        public string MMS { get; set; }
        public string PID { get; set; }
        public string DCS { get; set; }
        public string Class { get; set; }
        public string Alphabet { get; set; }
        public string Length { get; set; }
        public string TextType { get; set; }
        public string Sender { get; set; }
        public string SMSC { get; set; }
        public string CipherMode { get; set; }
        public string TMSI { get; set; }
        public string IMSI { get; set; }
        public string IMEI { get; set; }
        public Nullable<System.DateTime> TimeStamp { get; set; }
        public string Message { get; set; }
        public string Notes { get; set; }
        public Nullable<System.Guid> ARFCNID { get; set; }
    
        public virtual ARFCN ARFCN { get; set; }
    }
}
