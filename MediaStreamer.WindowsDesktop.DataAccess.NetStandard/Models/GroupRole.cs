using System;

#nullable disable

namespace MediaStreamer.Models
{
    public partial class GroupRole
    {
        public long MusicianID { get; set; }
        public long ArtistID { get; set; }
        public System.DateTime GroupFormationDate { get; set; }
        public string MusicianRoleName { get; set; }
        public Nullable<System.DateTime> GroupJoinDate { get; set; }

        public virtual Artist Artist { get; set; }
        public virtual GroupMember GroupMembers { get; set; }
        public virtual Musician Musician { get; set; }
        public virtual MusicianRole MusicianRoles { get; set; }
    }
}
