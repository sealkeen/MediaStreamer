using Microsoft.EntityFrameworkCore;
using MediaStreamer.Domain;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediaStreamer.Domain.Models;

namespace MediaStreamer.DataAccess.NetStandard
{
    public partial class DMEntitiesContext : NetStandardContext, IPagedDMDBContext
    {
        public static bool UseSQLServer = false;
        public static string Filename { get; set; }
        public static string LocalSource { get; set; } = @"O:/DB/09.06.2021-2.db3";

        private int _skipComps = -1;
        private int _takeComps = -1;
        public DMEntitiesContext()
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DMEntitiesContext(bool useSQLServer)
        {
            UseSQLServer = useSQLServer;
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

        public DMEntitiesContext(DbContextOptions<NetStandardContext> options)
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
                    entity.HasOne(e => e.Style)
                    .WithMany(s => s.Genres)
                    .HasForeignKey(e => e.StyleId);
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
                entity.HasKey(e => new { e.UserID, e.CompositionID });

                entity.ToTable("ListenedComposition");

                entity.Property(e => e.ListenDate).HasColumnType("DATETIME");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.CompositionID).HasColumnName("CompositionID");

                entity.Property(e => e.StoppedAt).HasColumnName("StoppedAt");

                entity.Property(e => e.CountOfPlays).HasColumnName("CountOfPlays");

                entity.HasOne(d => d.Composition)
                    .WithMany(p => p.ListenedCompositions)
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
                entity.Property(e => e.AspNetUserId).HasColumnName("AspNetUserId");
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

            modelBuilder.Entity<Style>(entity =>
            {
                entity.ToTable("Style");

                entity.Property(e => e.StyleId)
                    .HasColumnName("StyleId");

                entity.HasMany(s => s.Genres)
                .WithOne(g => g.Style);

                entity.Property(e => e.StyleName).IsRequired();
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

        public async Task<List<Composition>> GetCompositionsAsync()
        {
            if(_skipComps != -1 && _takeComps != -1)
                return await GetCompositions().ToListAsync();
            else
                return await GetCompositionsAsync(_skipComps, _takeComps);
        }

        public async Task<List<IComposition>> GetICompositionsAsync()
        {
            if (_skipComps != -1 && _takeComps != -1)
                return await GetICompositions().ToListAsync();
            else
                return await GetICompositionsAsync(_skipComps, _takeComps);
        }

        public IQueryable<IComposition> GetICompositions() { DisableLazyLoading(); return Compositions.Include(c => c.Artist); }
        void IDMDBContext.Add(Composition composition) => Compositions.Add(composition);
        public IQueryable<Genre> GetGenres() { return Genres; }
        void IDMDBContext.Add(Genre genre) => Genres.Add(genre);
        public IQueryable<ListenedComposition> GetListenedCompositions() { return ListenedCompositions; }
        void IDMDBContext.Add(ListenedComposition listenedComposition) => ListenedCompositions.Add(listenedComposition);
        public IQueryable<Moderator> GetModerators() { return Moderators; }
        void IDMDBContext.Add(Moderator moderator) => Moderators.Add(moderator);
        public IQueryable<Picture> GetPictures() { return Pictures; }
        void IDMDBContext.Add(Picture picture) => Pictures.Add(picture);
        public IQueryable<User> GetUsers() { return Users; }
        void IDMDBContext.Add(User user) => Users.Add(user);
        public IQueryable<Style> GetStyles() { return Styles; }
        void IDMDBContext.Add(Style style) => Styles.Add(style);

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
                    case nameof(Genres): if ((cnt = Genres.Count()) > 0) Genres.RemoveRange(Genres); break;
                    case nameof(ListenedCompositions):
                        if ((cnt = ListenedCompositions.Count()) > 0) ListenedCompositions.RemoveRange(ListenedCompositions); break;
                    case nameof(Moderators): if ((cnt = Moderators.Count()) > 0) Moderators.RemoveRange(Moderators); break;
                    case nameof(Pictures): if ((cnt = Pictures.Count()) > 0) Pictures.RemoveRange(Pictures); break;
                    case nameof(Users): if ((cnt = Users.Count()) > 0) Users.RemoveRange(Users); break;
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

        public Task<List<Composition>> GetCompositionsAsync(int skip, int take)
        {
            return Task.Factory.StartNew(() => Compositions.Skip(skip).Take(take).ToList());
        }

        public Task<List<IComposition>> GetICompositionsAsync(int skip, int take)
        {
            return Task.Factory
                .StartNew(() => Compositions
                .Skip(skip)
                .Take(take)
                .Select(c => c as IComposition)
                .ToList());
        }
    }
}
