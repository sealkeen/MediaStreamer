#nullable disable

using System;

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class ListenedAlbum
    {
        public Nullable<System.DateTime> ListenDate { get; set; }
        public long? CountOfPlays { get; set; }
        public long UserID { get; set; }
        public long ArtistID { get; set; }
        public System.DateTime GroupFormationDate { get; set; }
        public long AlbumID { get; set; }

        public virtual Album Album { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual GroupMember GroupMember { get; set; }
        public virtual User User { get; set; }
    }
}
