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

    internal partial class ListenedArtist
    {
        public Nullable<System.DateTime> ListenDate { get; set; }
        public Nullable<long> CountOfPlays { get; set; }
        public long UserID { get; set; }
        public long ArtistID { get; set; }
    
        public virtual Artist Artist { get; set; }
        public virtual User User { get; set; }
    }
}