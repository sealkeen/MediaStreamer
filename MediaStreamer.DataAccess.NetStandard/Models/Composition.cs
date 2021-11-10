using System;
using System.Collections.Generic;
using MediaStreamer.Domain;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class Composition
    {
        public Composition()
        {
            CompositionVideos = new HashSet<CompositionVideo>();
            ListenedCompositions = new HashSet<ListenedComposition>();
        }
        public long CompositionID { get; set; }
        public string CompositionName { get; set; }
        public Nullable<long> ArtistID { get; set; }
        public Nullable<System.DateTime> GroupFormationDate { get; set; }
        public Nullable<long> AlbumID { get; set; }
        public Nullable<long> Duration { get; set; }
        public string FilePath { get; set; }
        public string Lyrics { get; set; }
        public string About { get; set; }

        public virtual Album Album { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual GroupMember GroupMember { get; set; }
        public virtual ICollection<CompositionVideo> CompositionVideos { get; set; }
        public virtual ICollection<ListenedComposition> ListenedCompositions { get; set; }
    }
}
