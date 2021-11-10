using System.Collections.Generic;
using MediaStreamer.Domain;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class Artist
    {
        public Artist()
        {
            AlbumGenres = new HashSet<AlbumGenre>();
            Albums = new HashSet<Album>();
            ArtistGenres = new HashSet<ArtistGenre>();
            CompositionVideos = new HashSet<CompositionVideo>();
            Compositions = new HashSet<Composition>();
            GroupMembers = new HashSet<GroupMember>();
            GroupRoles = new HashSet<GroupRole>();
            ListenedAlbums = new HashSet<ListenedAlbum>();
            ListenedArtists = new HashSet<ListenedArtist>();
            ListenedCompositions = new HashSet<ListenedComposition>();
        }

        public long ArtistID { get; set; }
        public string ArtistName { get; set; }

        public virtual ICollection<AlbumGenre> AlbumGenres { get; set; }
        public virtual ICollection<Album> Albums { get; set; }
        public virtual ICollection<ArtistGenre> ArtistGenres { get; set; }
        public virtual ICollection<CompositionVideo> CompositionVideos { get; set; }
        public virtual ICollection<Composition> Compositions { get; set; }
        public virtual ICollection<GroupMember> GroupMembers { get; set; }
        public virtual ICollection<GroupRole> GroupRoles { get; set; }
        public virtual ICollection<ListenedAlbum> ListenedAlbums { get; set; }
        public virtual ICollection<ListenedArtist> ListenedArtists { get; set; }
        public virtual ICollection<ListenedComposition> ListenedCompositions { get; set; }
    }
}
