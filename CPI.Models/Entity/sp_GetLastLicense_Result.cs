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
    
    public partial class sp_GetLastLicense_Result
    {
        public System.Guid ID { get; set; }
        public int Count { get; set; }
        public string SerialNumber { get; set; }
        public string AuthorizationKey { get; set; }
        public string RegistrationNumber { get; set; }
        public Nullable<int> Period { get; set; }
        public Nullable<System.DateTime> Start { get; set; }
        public Nullable<System.DateTime> End { get; set; }
        public Nullable<System.DateTime> LastSession { get; set; }
        public string Summary { get; set; }
        public string Hash { get; set; }
        public string Directory { get; set; }
        public string Spare1 { get; set; }
        public string Spare2 { get; set; }
    }
}
