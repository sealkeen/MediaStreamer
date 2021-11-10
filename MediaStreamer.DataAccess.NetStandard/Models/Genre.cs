using System.Collections.Generic;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class Genre
    {
        public Genre()
        {
            AlbumGenres = new HashSet<AlbumGenre>();
            Albums = new HashSet<Album>();
            ArtistGenres = new HashSet<ArtistGenre>();
            ListenedGenres = new HashSet<ListenedGenre>();
        }

        public string GenreName { get; set; }

        public virtual ICollection<AlbumGenre> AlbumGenres { get; set; }
        public virtual ICollection<Album> Albums { get; set; }
        public virtual ICollection<ArtistGenre> ArtistGenres { get; set; }
        public virtual ICollection<ListenedGenre> ListenedGenres { get; set; }
    }
}
