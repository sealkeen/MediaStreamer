using System.Collections.Generic;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class Musician
    {
        public Musician()
        {
            GroupRoles = new HashSet<GroupRole>();
        }

        public long MusicianID { get; set; }
        public string MusicianName { get; set; }
        public virtual ICollection<GroupRole> GroupRoles { get; set; }
    }
}
