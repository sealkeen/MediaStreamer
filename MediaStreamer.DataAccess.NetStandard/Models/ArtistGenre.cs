using System;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class ArtistGenre
    {
        public long ArtistID { get; set; }
        public string GenreName { get; set; }
        public Nullable<System.DateTime> DateOfApplication { get; set; }

        public virtual Artist Artist { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
