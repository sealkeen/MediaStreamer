#nullable disable

using System;

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class ListenedComposition
    {
        public System.DateTime ListenDate { get; set; }
        public Nullable<long> CountOfPlays { get; set; }
        public long UserID { get; set; }
        public Nullable<long> ArtistID { get; set; }
        public System.DateTime GroupFormationDate { get; set; }
        public Nullable<long> AlbumID { get; set; }
        public long CompositionID { get; set; }

        public virtual Album Album { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual Composition Composition { get; set; }
        public virtual GroupMember GroupMember { get; set; }
        public virtual User User { get; set; }
    }
}
