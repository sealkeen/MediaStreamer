#nullable disable

namespace MediaStreamer.Models
{
    public partial class Administrator
    {
        public long AdministratorID { get; set; }
        public long? ModeratorID { get; set; }
        public long? UserID { get; set; }

        public virtual Moderator Moderator { get; set; }
        public virtual User User { get; set; }
    }
}
