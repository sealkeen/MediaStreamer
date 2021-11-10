#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class ListenedGenre
    {
        public long? CountOfPlays { get; set; }
        public long UserID { get; set; }
        public string GenreName { get; set; }

        public virtual Genre GenreNameNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
