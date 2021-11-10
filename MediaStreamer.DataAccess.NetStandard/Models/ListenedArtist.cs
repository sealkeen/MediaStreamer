#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class ListenedArtist
    {
        public System.DateTime ListenDate { get; set; }
        public long? CountOfPlays { get; set; }
        public long UserID { get; set; }
        public long ArtistID { get; set; }

        public virtual Artist Artist { get; set; }
        public virtual User User { get; set; }
    }
}
