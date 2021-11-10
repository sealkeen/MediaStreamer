using System;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class AlbumGenre
    {
        public string GenreName { get; set; }
        public long ArtistID { get; set; }
        public System.DateTime GroupFormationDate { get; set; }
        public long AlbumID { get; set; }
        public Nullable<System.DateTime> DateOfApplication { get; set; }

        public virtual Album Album { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual GroupMember GroupMember { get; set; }
    }
}
