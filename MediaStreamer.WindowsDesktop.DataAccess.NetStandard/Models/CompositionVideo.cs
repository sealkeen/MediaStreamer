#nullable disable

namespace MediaStreamer.Models
{
    public partial class CompositionVideo
    {
        public long VideoID { get; set; }
        public long ArtistID { get; set; }
        public long AlbumID { get; set; }
        public long CompositionID { get; set; }

        public virtual Album Album { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual Composition Composition { get; set; }
        public virtual Video Video { get; set; }
    }
}
