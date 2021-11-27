using Microsoft.EntityFrameworkCore;
using MediaStreamer.Domain;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using MediaStreamer.IO;

#nullable disable

namespace MediaStreamer
{
    public partial class DMEntitiesContext : DbContext, IDMDBContext
    {
        public static string Filename { get; set; }
        public static string _localSource = @"C:/Users/Sealkeen/Documents/ГУАП Done (v)/7 Базы данных(1)/09.06.2021-2.db3";

        public DMEntitiesContext(string localSource = null)
        {
            if (localSource != null)
            {
                if (File.Exists(localSource))
                    Filename = localSource;
            }
            else
            {

                if (!_localSource.FileExists())
                {
                    if (File.Exists("O:/DB/26.10.2021-3.db3"))
                        Filename = "O:/DB/26.10.2021-3.db3";
                    else
                    {
                        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "xmsdb.db3");
                        bool exists = File.Exists(fileName);
                        Filename = fileName;
                    }
                    Database.EnsureCreated();
                }
                else
                {
                    Filename = _localSource;
                }
            }
        }

        public void Clear()
        {
            this.ChangeTracker
               .Entries()
               .ToList()
               .ForEach(e => e.State = EntityState.Detached);

            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        //public async Task ClearAsync()
        //{
        //    await Database.EnsureDeleted();
        //    await Database.EnsureCreated();
        //}
        public DMEntitiesContext(DbContextOptions<DMEntitiesContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    //optionsBuilder.UseSqlite($"DataSource=http://docs.google.com/uc?export=open&id=1TqCBUjhXeglQogUaIGaegu7TUf4-iiXA");
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkID=723263.
                    optionsBuilder.UseSqlite(@$"DataSource={Filename}");
                }
                else
                {
                    optionsBuilder.UseSqlite($"DataSource={Filename}");
                } 
            }
        }

        public virtual DbSet<Administrator> Administrators { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<AlbumGenre> AlbumGenres { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<ArtistGenre> ArtistGenres { get; set; }
        public virtual DbSet<Composition> Compositions { get; set; }
        public virtual DbSet<CompositionVideo> CompositionVideos { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<GroupRole> GroupRoles { get; set; }
        public virtual DbSet<ListenedAlbum> ListenedAlbums { get; set; }
        public virtual DbSet<ListenedArtist> ListenedArtists { get; set; }
        public virtual DbSet<ListenedComposition> ListenedCompositions { get; set; }
        public virtual DbSet<ListenedGenre> ListenedGenres { get; set; }
        public virtual DbSet<Moderator> Moderators { get; set; }
        public virtual DbSet<Musician> Musicians { get; set; }
        public virtual DbSet<MusicianRole> MusicianRoles { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Video> Videos { get; set; }

        public string DBPath { get; set; } = "";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.ToTable("Administrator");

                entity.Property(e => e.AdministratorID)
                    .ValueGeneratedNever()
                    .HasColumnName("AdministratorID");

                entity.Property(e => e.ModeratorID).HasColumnName("ModeratorID");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.HasOne(d => d.Moderator)
                    .WithMany(p => p.Administrators)
                    .HasForeignKey(d => d.ModeratorID);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Administrators)
                    .HasForeignKey(d => d.UserID);
            });

            modelBuilder.Entity<Album>(entity =>
            {
                entity.ToTable("Album");

                entity.Property(e => e.AlbumID)
                    .ValueGeneratedNever()
                    .HasColumnName("AlbumID");

                entity.Property(e => e.AlbumName).IsRequired();

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.Property(e => e.GroupFormationDate).HasColumnType("DATE");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.ArtistID);

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.GenreName);

                entity.HasOne(d => d.GroupMember)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.GroupFormationDate);
            });

            modelBuilder.Entity<AlbumGenre>(entity =>
            {
                entity.HasKey(e => new { e.GenreName, e.ArtistID, e.GroupFormationDate, e.AlbumID });

                entity.ToTable("AlbumGenre");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.Property(e => e.GroupFormationDate).HasColumnType("DATE");

                entity.Property(e => e.AlbumID).HasColumnName("AlbumID");

                entity.Property(e => e.DateOfApplication).HasColumnType("DATE");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.AlbumGenres)
                    .HasForeignKey(d => d.AlbumID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.AlbumGenres)
                    .HasForeignKey(d => d.GenreName)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.GroupMember)
                    .WithMany(p => p.AlbumGenres)
                    .HasForeignKey(d => d.GroupFormationDate)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Artist>(entity =>
            {
                entity.ToTable("Artist");

                entity.Property(e => e.ArtistID)
                    .ValueGeneratedNever()
                    .HasColumnName("ArtistID");

                entity.Property(e => e.ArtistName).IsRequired();
            });

            modelBuilder.Entity<ArtistGenre>(entity =>
            {
                entity.HasKey(e => new { e.ArtistID, e.GenreName });

                entity.ToTable("ArtistGenre");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.Property(e => e.DateOfApplication).HasColumnType("DATE");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.ArtistGenres)
                    .HasForeignKey(d => d.ArtistID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.ArtistGenres)
                    .HasForeignKey(d => d.GenreName)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Composition>(entity =>
            {
                entity.ToTable("Composition");

                entity.Property(e => e.CompositionID)
                    .ValueGeneratedNever()
                    .HasColumnName("CompositionID");

                entity.Property(e => e.AlbumID).HasColumnName("AlbumID");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.Property(e => e.CompositionName).IsRequired();

                entity.Property(e => e.GroupFormationDate).HasColumnType("DATE");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.Compositions)
                    .HasForeignKey(d => d.AlbumID);

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Compositions)
                    .HasForeignKey(d => d.ArtistID);

                entity.HasOne(d => d.GroupMember)
                    .WithMany(p => p.Compositions)
                    .HasForeignKey(d => d.GroupFormationDate);
            });

            modelBuilder.Entity<CompositionVideo>(entity =>
            {
                entity.HasKey(e => new { e.VideoID, e.ArtistID, e.AlbumID, e.CompositionID });

                entity.ToTable("CompositionVideo");

                entity.Property(e => e.VideoID).HasColumnName("VideoID");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.Property(e => e.AlbumID).HasColumnName("AlbumID");

                entity.Property(e => e.CompositionID).HasColumnName("CompositionID");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.CompositionVideos)
                    .HasForeignKey(d => d.AlbumID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.CompositionVideos)
                    .HasForeignKey(d => d.ArtistID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Composition)
                    .WithMany(p => p.CompositionVideos)
                    .HasForeignKey(d => d.CompositionID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Video)
                    .WithMany(p => p.CompositionVideos)
                    .HasForeignKey(d => d.VideoID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.GenreName);

                entity.ToTable("Genre");
            });

            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.HasKey(e => e.GroupFormationDate);

                entity.Property(e => e.GroupFormationDate).HasColumnType("DATE");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.GroupMembers)
                    .HasForeignKey(d => d.ArtistID);
            });

            modelBuilder.Entity<GroupRole>(entity =>
            {
                entity.HasKey(e => new { e.MusicianID, e.ArtistID, e.GroupFormationDate, e.MusicianRoleName });

                entity.ToTable("GroupRole");

                entity.Property(e => e.MusicianID).HasColumnName("MusicianID");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.Property(e => e.GroupFormationDate).HasColumnType("DATE");

                entity.Property(e => e.GroupJoinDate).HasColumnType("DATETIME");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.GroupRoles)
                    .HasForeignKey(d => d.ArtistID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.GroupMember)
                    .WithMany(p => p.GroupRoles)
                    .HasForeignKey(d => d.GroupFormationDate)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Musician)
                    .WithMany(p => p.GroupRoles)
                    .HasForeignKey(d => d.MusicianID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.MusicianRole)
                    .WithMany(p => p.GroupRoles)
                    .HasForeignKey(d => d.MusicianRoleName)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ListenedAlbum>(entity =>
            {
                entity.HasKey(e => new { e.UserID, e.ArtistID, e.GroupFormationDate, e.AlbumID });

                entity.ToTable("ListenedAlbum");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.Property(e => e.GroupFormationDate).HasColumnType("DATE");

                entity.Property(e => e.AlbumID).HasColumnName("AlbumID");

                entity.Property(e => e.ListenDate).HasColumnType("DATETIME");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.ListenedAlbums)
                    .HasForeignKey(d => d.AlbumID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.ListenedAlbums)
                    .HasForeignKey(d => d.ArtistID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.GroupMember)
                    .WithMany(p => p.ListenedAlbums)
                    .HasForeignKey(d => d.GroupFormationDate)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ListenedAlbums)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ListenedArtist>(entity =>
            {
                entity.HasKey(e => new { e.UserID, e.ArtistID });

                entity.ToTable("ListenedArtist");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.Property(e => e.ListenDate).HasColumnType("DATETIME");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.ListenedArtists)
                    .HasForeignKey(d => d.ArtistID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ListenedArtists)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ListenedComposition>(entity =>
            {
                entity.HasKey(e => new { e.ListenDate, e.UserID, e.ArtistID, e.GroupFormationDate, e.AlbumID, e.CompositionID });

                entity.ToTable("ListenedComposition");

                entity.Property(e => e.ListenDate).HasColumnType("DATETIME");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.Property(e => e.GroupFormationDate).HasColumnType("DATE");

                entity.Property(e => e.AlbumID).HasColumnName("AlbumID");

                entity.Property(e => e.CompositionID).HasColumnName("CompositionID");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.ListenedCompositions)
                    .HasForeignKey(d => d.AlbumID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.ListenedCompositions)
                    .HasForeignKey(d => d.ArtistID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Composition)
                    .WithMany(p => p.ListenedCompositions)
                    .HasForeignKey(d => d.CompositionID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.GroupMember)
                    .WithMany(p => p.ListenedCompositions)
                    .HasForeignKey(d => d.GroupFormationDate)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ListenedCompositions)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ListenedGenre>(entity =>
            {
                entity.HasKey(e => new { e.UserID, e.GenreName });

                entity.ToTable("ListenedGenre");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.GenreName).HasColumnType("Text");

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.ListenedGenres)
                    .HasForeignKey(d => d.GenreName)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ListenedGenres)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Moderator>(entity =>
            {
                entity.ToTable("Moderator");

                entity.Property(e => e.ModeratorID)
                    .ValueGeneratedNever()
                    .HasColumnName("ModeratorID");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Moderators)
                    .HasForeignKey(d => d.UserID);
            });

            modelBuilder.Entity<Musician>(entity =>
            {
                entity.ToTable("Musician");

                entity.Property(e => e.MusicianID)
                    .ValueGeneratedNever()
                    .HasColumnName("MusicianID");
            });

            modelBuilder.Entity<MusicianRole>(entity =>
            {
                entity.HasKey(e => e.MusicianRoleName);

                entity.ToTable("MusicianRole");
            });

            modelBuilder.Entity<Picture>(entity =>
            {
                entity.ToTable("Picture");

                entity.Property(e => e.PictureID)
                    .ValueGeneratedNever()
                    .HasColumnName("PictureID");

                entity.Property(e => e.XResolution).HasColumnName("XResolution");

                entity.Property(e => e.YResolution).HasColumnName("YResolution");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "IX_User_Email")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "IX_User_UserName")
                    .IsUnique();

                entity.Property(e => e.UserID)
                    .ValueGeneratedNever()
                    .HasColumnName("UserID");

                entity.Property(e => e.DateOfSignUp)
                    .IsRequired()
                    .HasColumnType("DATETIME");

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.LastListenEntitiesUpdate).HasColumnType("DATETIME");

                entity.Property(e => e.LastListenedCompositionChange).HasColumnType("DATETIME");

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.UserName).IsRequired();

                entity.Property(e => e.VKLink).HasColumnName("VKLink");
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.ToTable("Video");

                entity.Property(e => e.VideoID)
                    .ValueGeneratedNever()
                    .HasColumnName("VideoID");

                entity.Property(e => e.FPS).HasColumnName("FPS");

                entity.Property(e => e.VariableFPS)
                    .HasColumnType("BOOLEAN")
                    .HasColumnName("VariableFPS");

                entity.Property(e => e.XResolution).HasColumnName("XResolution");

                entity.Property(e => e.YResolution).HasColumnName("YResolution");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public void AddEntity<T>(T item) where T : class
        {
            Add(item);
        }
        public void RemoveEntity<T>(T item) where T : class
        {
            Remove(item);
        }

        public IQueryable<Administrator> GetAdministrators() { return Administrators; }
        void IDMDBContext.Add(Administrator administrator) => Administrators.Add(administrator);
        public IQueryable<Album> GetAlbums() { return Albums; }
        void IDMDBContext.Add(Album album) => Albums.Add(album);
        public IQueryable<AlbumGenre> GetAlbumGenres() { return AlbumGenres; }
        void IDMDBContext.Add(AlbumGenre albumGenre) => AlbumGenres.Add(albumGenre);
        public IQueryable<Artist> GetArtists() { return Artists; }
        void IDMDBContext.Add(Artist artist) => Artists.Add(artist);
        public IQueryable<ArtistGenre> GetArtistGenres() { return ArtistGenres; }
        void IDMDBContext.Add(ArtistGenre artistGenre) => ArtistGenres.Add(artistGenre);
        public IQueryable<Composition> GetCompositions() { return Compositions.Include(c => c.Artist); }

        public Task<IQueryable<Composition>> GetCompositionsAsync() 
        { 
            return Task.Factory.StartNew(GetCompositions); 
        }
        public IQueryable<IComposition> GetICompositions() { return Compositions; }
        void IDMDBContext.Add(Composition composition) => Compositions.Add(composition);
        public IQueryable<CompositionVideo> GetCompositionVideos() { return CompositionVideos; }
        void IDMDBContext.Add(CompositionVideo compositionVideo) => CompositionVideos.Add(compositionVideo);
        public IQueryable<Genre> GetGenres() { return Genres; }
        void IDMDBContext.Add(Genre genre) => Genres.Add(genre);
        public IQueryable<GroupMember> GetGroupMembers() { return GroupMembers; }
        void IDMDBContext.Add(GroupMember groupMember) => GroupMembers.Add(groupMember);
        public IQueryable<GroupRole> GetGroupRoles() { return GroupRoles; }
        void IDMDBContext.Add(GroupRole groupRole) => GroupRoles.Add(groupRole);
        public IQueryable<ListenedAlbum> GetListenedAlbums() { return ListenedAlbums; }
        void IDMDBContext.Add(ListenedAlbum listenedAlbum) => ListenedAlbums.Add(listenedAlbum);
        public IQueryable<ListenedArtist> GetListenedArtists() { return ListenedArtists; }
        void IDMDBContext.Add(ListenedArtist listenedArtist) => ListenedArtists.Add(listenedArtist);
        public IQueryable<ListenedComposition> GetListenedCompositions() { return ListenedCompositions; }
        void IDMDBContext.Add(ListenedComposition listenedComposition) => ListenedCompositions.Add(listenedComposition);
        public IQueryable<ListenedGenre> GetListenedGenres() { return ListenedGenres; }
        void IDMDBContext.Add(ListenedGenre listenedGenre) => ListenedGenres.Add(listenedGenre);
        public IQueryable<Moderator> GetModerators() { return Moderators; }
        void IDMDBContext.Add(Moderator moderator) => Moderators.Add(moderator);
        public IQueryable<Musician> GetMusicians() { return Musicians; }
        void IDMDBContext.Add(Musician musician) => Musicians.Add(musician);
        public IQueryable<MusicianRole> GetMusicianRoles() { return MusicianRoles; }
        void IDMDBContext.Add(MusicianRole musicianRole) => MusicianRoles.Add(musicianRole);
        public IQueryable<Picture> GetPictures() { return Pictures; }
        void IDMDBContext.Add(Picture picture) => Pictures.Add(picture);
        public IQueryable<User> GetUsers() { return Users; }
        void IDMDBContext.Add(User user) => Users.Add(user);
        public IQueryable<Video> GetVideos() { return Videos; }
        void IDMDBContext.Add(Video video) => Videos.Add(video);

        void IDMDBContext.UpdateAndSaveChanges<TEntity>(TEntity entity)
        {
            Update(entity);
            SaveChanges();
        }
    }
}
