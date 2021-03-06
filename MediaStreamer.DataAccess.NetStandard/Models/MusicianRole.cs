using System.Collections.Generic;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class MusicianRole
    {
        public MusicianRole()
        {
            GroupRoles = new HashSet<GroupRole>();
        }

        public string MusicianRoleName { get; set; }

        public virtual ICollection<GroupRole> GroupRoles { get; set; }
    }
}
