using System;
using System.Collections.Generic;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard.Models
{
    public partial class User
    {
        public User()
        {
            Administrators = new HashSet<Administrator>();
            ListenedAlbums = new HashSet<ListenedAlbum>();
            ListenedArtists = new HashSet<ListenedArtist>();
            ListenedCompositions = new HashSet<ListenedComposition>();
            ListenedGenres = new HashSet<ListenedGenre>();
            Moderators = new HashSet<Moderator>();
        }

        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public System.DateTime DateOfSignUp { get; set; }
        public string VkLink { get; set; }
        public string FaceBookLink { get; set; }
        public string Bio { get; set; }
        public Nullable<System.DateTime> LastListenedCompositionChange { get; set; }
        public Nullable<System.DateTime> LastListenEntitiesUpdate { get; set; }

        public virtual ICollection<Administrator> Administrators { get; set; }
        public virtual ICollection<ListenedAlbum> ListenedAlbums { get; set; }
        public virtual ICollection<ListenedArtist> ListenedArtists { get; set; }
        public virtual ICollection<ListenedComposition> ListenedCompositions { get; set; }
        public virtual ICollection<ListenedGenre> ListenedGenres { get; set; }
        public virtual ICollection<Moderator> Moderators { get; set; }
    }
}
