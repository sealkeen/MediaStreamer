//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataBaseResource
{
    using System;
    using System.Collections.Generic;
    
    public partial class ArtistGenre
    {
        public long ArtistID { get; set; }
        public string GenreName { get; set; }
        public Nullable<System.DateTime> DateOfApplication { get; set; }
    
        public virtual Artist Artist { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
