//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MediaStreamer.DataAccess.Net40.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Administrator
    {
        public long AdministratorID { get; set; }
        public Nullable<long> ModeratorID { get; set; }
        public Nullable<long> UserID { get; set; }
    
        public virtual Moderator Moderator { get; set; }
        public virtual User User { get; set; }
    }
}
