using MediaStreamer.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.DataAccess.NetStandard
{
    public class NetStandardContext : DbContext
    {
        public NetStandardContext() { }
        public NetStandardContext(DbContextOptions<NetStandardContext> options)
            : base(options)
        {

        }
        public virtual DbSet<Administrator> Administrators { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<AlbumGenre> AlbumGenres { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<ArtistGenre> ArtistGenres { get; set; }
        public virtual DbSet<Composition> Compositions { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<ListenedComposition> ListenedCompositions { get; set; }
        public virtual DbSet<Moderator> Moderators { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }
        public virtual DbSet<PlayerState> PlayerStates { get; set; }
        public virtual DbSet<MediaStreamer.Domain.Models.Style> Styles { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
