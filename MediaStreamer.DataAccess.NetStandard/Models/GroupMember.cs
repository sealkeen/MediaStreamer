using System.Collections.Generic;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class GroupMember
    {
        public GroupMember()
        {
            AlbumGenres = new HashSet<AlbumGenre>();
            Albums = new HashSet<Album>();
            Compositions = new HashSet<Composition>();
            GroupRoles = new HashSet<GroupRole>();
            ListenedAlbums = new HashSet<ListenedAlbum>();
            ListenedCompositions = new HashSet<ListenedComposition>();
        }

        public long? ArtistID { get; set; }
        public System.DateTime GroupFormationDate { get; set; }
        public long? DateOfDisband { get; set; }

        public virtual Artist Artist { get; set; }
        public virtual ICollection<AlbumGenre> AlbumGenres { get; set; }
        public virtual ICollection<Album> Albums { get; set; }
        public virtual ICollection<Composition> Compositions { get; set; }
        public virtual ICollection<GroupRole> GroupRoles { get; set; }
        public virtual ICollection<ListenedAlbum> ListenedAlbums { get; set; }
        public virtual ICollection<ListenedComposition> ListenedCompositions { get; set; }
    }
}
