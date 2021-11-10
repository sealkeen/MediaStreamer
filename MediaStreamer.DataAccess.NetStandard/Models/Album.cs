using System;
using System.Collections.Generic;
using MediaStreamer.Domain;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class Album : IAlbum
    {
        public Album()
        {
            AlbumGenres = new HashSet<AlbumGenre>();
            CompositionVideos = new HashSet<CompositionVideo>();
            Compositions = new HashSet<Composition>();
            ListenedAlbums = new HashSet<ListenedAlbum>();
            ListenedCompositions = new HashSet<ListenedComposition>();
        }

        public long AlbumID { get; set; }
        public string AlbumName { get; set; }
        public long? ArtistID { get; set; }
        public Nullable<System.DateTime> GroupFormationDate { get; set; }
        public string GenreName { get; set; }
        public long? Year { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual GroupMember GroupMember { get; set; }
        public virtual ICollection<AlbumGenre> AlbumGenres { get; set; }
        public virtual ICollection<CompositionVideo> CompositionVideos { get; set; }
        public virtual ICollection<Composition> Compositions { get; set; }
        public virtual ICollection<ListenedAlbum> ListenedAlbums { get; set; }
        public virtual ICollection<ListenedComposition> ListenedCompositions { get; set; }
    }
}
