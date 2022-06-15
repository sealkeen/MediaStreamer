using Microsoft.EntityFrameworkCore;
using MediaStreamer.Domain;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;


namespace MediaStreamer.DataAccess.NetStandard
{
    public partial class DMEntitiesContext : DbContext, IDMDBContext
    {
        public static bool UseSQLServer = false;
        public string DBPath { get; set; } = "";
        public static string Filename { get; set; }
        public static string LocalSource { get; set; } = @"O:/DB/09.06.2021-2.db3";

        public DMEntitiesContext()
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public void EnsureCreated()
        {
            Database.EnsureCreated();
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
                    if (UseSQLServer) {
                        optionsBuilder.UseSqlServer(@"Server=.\SQLExpress;initial catalog=CompositionsDB;user id=sys_admin;password=s0m3P4ssw0rdT3xt;MultipleActiveResultSets=True;");
                    }
                    else
                        optionsBuilder.UseSqlite(@$"DataSource={PathResolver.GetStandardDatabasePath()}");
                }
                else
                {
                    optionsBuilder.UseSqlite($"DataSource={PathResolver.GetStandardDatabasePath()}");
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
        public virtual DbSet<ListenedComposition> ListenedCompositions { get; set; }
        public virtual DbSet<Moderator> Moderators { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<PlayerState> PlayerStates { get; set; }

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

            });

            modelBuilder.Entity<Album>(entity =>
            {
                entity.ToTable("Album");

                entity.Property(e => e.AlbumID)
                    .ValueGeneratedNever()
                    .HasColumnName("AlbumID");

                entity.Property(e => e.AlbumName).IsRequired();

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");


                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.ArtistID);

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.Albums)
                    .HasForeignKey(d => d.GenreID);
            });

            modelBuilder.Entity<AlbumGenre>(entity =>
            {
                entity.HasKey(e => new { e.AlbumID, e.GenreID });

                entity.ToTable("AlbumGenre");

                entity.Property(e => e.AlbumID).HasColumnName("AlbumID");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.AlbumGenres)
                    .HasForeignKey(d => d.AlbumID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.AlbumGenres)
                    .HasForeignKey(d => d.GenreID)
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
                entity.HasKey(e => new { e.ArtistID, e.GenreID });

                entity.ToTable("ArtistGenre");

                entity.Property(e => e.ArtistID).HasColumnName("ArtistID");

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.ArtistGenres)
                    .HasForeignKey(d => d.ArtistID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.ArtistGenres)
                    .HasForeignKey(d => d.GenreID)
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

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.Compositions)
                    .HasForeignKey(d => d.AlbumID);

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Compositions)
                    .HasForeignKey(d => d.ArtistID);

            });

            modelBuilder.Entity<CompositionVideo>(entity =>
            {
                entity.HasKey(e => new { e.VideoID, e.CompositionID });

                entity.ToTable("CompositionVideo");

                entity.Property(e => e.VideoID).HasColumnName("VideoID");


                entity.Property(e => e.CompositionID).HasColumnName("CompositionID");

                entity.HasOne(d => d.Composition)
                    .WithMany(p => p.CompositionVideos)
                    .HasForeignKey(d => d.CompositionID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

            });

            if (UseSQLServer)
            {
                modelBuilder.Entity<Genre>(entity =>
                {
                    entity.HasKey(e => e.GenreID);
                    entity.Property(e => e.GenreID).IsRequired();
                    entity.ToTable("Genre");
                });
            }
            else
            {
                modelBuilder.Entity<Genre>(entity =>
                {
                    entity.HasKey(e => e.GenreID);
                    entity.ToTable("Genre");
                });
            }

            modelBuilder.Entity<ListenedComposition>(entity =>
            {
                entity.HasKey(e => new { e.ListenedCompositionID });

                entity.ToTable("ListenedComposition");

                entity.Property(e => e.ListenDate).HasColumnType("DATETIME");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.CompositionID).HasColumnName("CompositionID");


                entity.HasOne(d => d.Composition)
                    .WithMany(p => p.ListenedCompositions)
                    .HasForeignKey(d => d.CompositionID)
                    .OnDelete(DeleteBehavior.ClientSetNull);


                entity.HasOne(d => d.User)
                    .WithMany(p => p.ListenedCompositions)
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
                    .HasColumnType("BIT")
                    .HasColumnName("VariableFPS");

                entity.Property(e => e.XResolution).HasColumnName("XResolution");

                entity.Property(e => e.YResolution).HasColumnName("YResolution");
            });

            modelBuilder.Entity<PlayerState>(entity =>
            {
                entity.HasKey(e => e.StateID);

                entity.Property(e => e.StateTime)
                .IsRequired()
                .HasColumnType("DATETIME");

                entity.Property(e => e.VolumeLevel)
                .IsRequired()
                .HasColumnType("NUMERIC");
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
        public IQueryable<Album> GetAlbums() { return Albums.Include(a => a.Artist); }
        void IDMDBContext.Add(Album album) => Albums.Add(album);
        public IQueryable<AlbumGenre> GetAlbumGenres() { return AlbumGenres; }
        void IDMDBContext.Add(AlbumGenre albumGenre) => AlbumGenres.Add(albumGenre);
        public IQueryable<Artist> GetArtists() { return Artists; }
        void IDMDBContext.Add(Artist artist) => Artists.Add(artist);
        public IQueryable<ArtistGenre> GetArtistGenres() { return ArtistGenres; }
        void IDMDBContext.Add(ArtistGenre artistGenre) => ArtistGenres.Add(artistGenre);
        public IQueryable<Composition> GetCompositions() { DisableLazyLoading(); return Compositions.Include(c => c.Artist); }

        public Task<IQueryable<Composition>> GetCompositionsAsync() 
        { 
            return Task.Factory.StartNew(GetCompositions); 
        }
        public IQueryable<IComposition> GetICompositions() { DisableLazyLoading(); return Compositions.Include(c => c.Artist); }
        void IDMDBContext.Add(Composition composition) => Compositions.Add(composition);
        public IQueryable<CompositionVideo> GetCompositionVideos() { return CompositionVideos; }
        void IDMDBContext.Add(CompositionVideo compositionVideo) => CompositionVideos.Add(compositionVideo);
        public IQueryable<Genre> GetGenres() { return Genres; }
        void IDMDBContext.Add(Genre genre) => Genres.Add(genre);
        public IQueryable<ListenedComposition> GetListenedCompositions() { return ListenedCompositions.Include(c => c.Composition); }
        void IDMDBContext.Add(ListenedComposition listenedComposition) => ListenedCompositions.Add(listenedComposition);
        public IQueryable<Moderator> GetModerators() { return Moderators; }
        void IDMDBContext.Add(Moderator moderator) => Moderators.Add(moderator);
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

        public void DisableLazyLoading()
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public bool ClearTable(string tableName)
        {
            try
            {
                int cnt = 0;
                switch (tableName)
                {
                    case nameof(Administrators): if ((cnt = Administrators.Count()) > 0) Administrators.RemoveRange(Administrators); break;
                    case nameof(Albums): if ((cnt = Albums.Count()) > 0) Albums.RemoveRange(Albums); break;
                    case nameof(AlbumGenres): if ((cnt = AlbumGenres.Count()) > 0) AlbumGenres.RemoveRange(AlbumGenres); break;
                    case nameof(Artists): if ((cnt = Artists.Count()) > 0) Artists.RemoveRange(Artists); break;
                    case nameof(ArtistGenres): if ((cnt = ArtistGenres.Count()) > 0) ArtistGenres.RemoveRange(ArtistGenres); break;
                    case nameof(Compositions): if ((cnt = Compositions.Count()) > 0) Compositions.RemoveRange(Compositions); break;
                    case nameof(CompositionVideos):
                        if ((cnt = CompositionVideos.Count()) > 0) CompositionVideos.RemoveRange(CompositionVideos); break;
                    case nameof(Genres): if ((cnt = Genres.Count()) > 0) Genres.RemoveRange(Genres); break;
                    case nameof(ListenedCompositions):
                        if ((cnt = ListenedCompositions.Count()) > 0) ListenedCompositions.RemoveRange(ListenedCompositions); break;
                    case nameof(Moderators): if ((cnt = Moderators.Count()) > 0) Moderators.RemoveRange(Moderators); break;
                    case nameof(Pictures): if ((cnt = Pictures.Count()) > 0) Pictures.RemoveRange(Pictures); break;
                    case nameof(Users): if ((cnt = Users.Count()) > 0) Users.RemoveRange(Users); break;
                    case nameof(Videos): if ((cnt = Videos.Count()) > 0) Videos.RemoveRange(Videos); break;
                    case nameof(PlayerStates): if ((cnt = PlayerStates.Count()) > 0) PlayerStates.RemoveRange(PlayerStates); break;
                }
                if (cnt > 0)
                    SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ue) { 
            }
            return false;
        }

        public string GetContainingFolderPath()
        {
            return Filename;
        }
    }
}
