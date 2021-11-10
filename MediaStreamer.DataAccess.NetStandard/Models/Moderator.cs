using System.Collections.Generic;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class Moderator
    {
        public Moderator()
        {
            Administrators = new HashSet<Administrator>();
        }

        public long ModeratorID { get; set; }
        public long? UserID { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Administrator> Administrators { get; set; }
    }
}
