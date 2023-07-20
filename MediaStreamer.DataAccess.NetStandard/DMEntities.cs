#define netstandard2

using Microsoft.EntityFrameworkCore;
using MediaStreamer.Domain;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediaStreamer.DataAccess.NetStandard;
using MediaStreamer.Domain.Models;

#nullable disable

namespace MediaStreamer.DataAccess.NetStandard
{
    public partial class DMEntities : NetStandardContext, IPagedDMDBContext
    {
        public static string Filename { get; set; }
        public static string _localSource = @"C:/Users/Sealkeen/Documents/ГУАП Done (v)/7 Базы данных(1)/09.06.2021-2.db3";


        public DMEntities(string localSource = null)
        {
            if (localSource != null)
            {
                if (File.Exists(localSource))
                    Filename = localSource;
            }
            else
            {
                if (!File.Exists(_localSource))
                {
                    if (File.Exists(PathResolver.GetStandardDatabasePath()))
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
        public DMEntities(DbContextOptions<NetStandardContext> options)
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

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.GenreID);
                entity.Property(e => e.GenreID).IsRequired();
                entity.ToTable("Genre");
                entity.HasOne(e => e.Style)
                .WithMany(s => s.Genres)
                .HasForeignKey(e => e.StyleId);
            });

            modelBuilder.Entity<ListenedComposition>(entity =>
            {
                entity.HasKey(e => new { e.UserID, e.CompositionID });

                entity.ToTable("ListenedComposition");

                entity.Property(e => e.ListenDate).HasColumnType("DATETIME");

                entity.Property(e => e.UserID).HasColumnName("UserID");

                entity.Property(e => e.CompositionID).HasColumnName("CompositionID");

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

        public IQueryable<Administrator> GetAdministrators() { return Administrators.AsQueryable(); }
        void IDMDBContext.Add(Administrator administrator) => Administrators.Add(administrator);
        public IQueryable<Album> GetAlbums() { return Albums.AsQueryable(); }
        void IDMDBContext.Add(Album album) => Albums.Add(album);
        public IQueryable<AlbumGenre> GetAlbumGenres() { return AlbumGenres.AsQueryable(); }
        void IDMDBContext.Add(AlbumGenre albumGenre) => AlbumGenres.Add(albumGenre);
        public IQueryable<Artist> GetArtists() { return Artists.AsQueryable(); }
        void IDMDBContext.Add(Artist artist) => Artists.Add(artist);
        public IQueryable<ArtistGenre> GetArtistGenres() { return ArtistGenres.AsQueryable(); }
        void IDMDBContext.Add(ArtistGenre artistGenre) => ArtistGenres.Add(artistGenre);
        public IQueryable<Composition> GetCompositions() { return Compositions.AsQueryable().Include(c => c.Artist); }

        public async Task<List<Composition>> GetCompositionsAsync()
        {
            return await GetCompositions().ToListAsync();
        }

        public async Task<List<IComposition>> GetICompositionsAsync()
        {
            return await GetICompositions().ToListAsync();
        }

        public IQueryable<IComposition> GetICompositions() { return Compositions.AsQueryable(); }
        void IDMDBContext.Add(Composition composition) => Compositions.Add(composition);
        public IQueryable<Genre> GetGenres() { return Genres.AsQueryable(); }
        void IDMDBContext.Add(Genre genre) => Genres.Add(genre);

        public IQueryable<ListenedComposition> GetListenedCompositions() { return ListenedCompositions.AsNoTracking().AsQueryable(); }
        void IDMDBContext.Add(ListenedComposition listenedComposition) => ListenedCompositions.Add(listenedComposition);

        public IQueryable<Moderator> GetModerators() { return Moderators.AsQueryable(); }
        void IDMDBContext.Add(Moderator moderator) => Moderators.Add(moderator);

        public IQueryable<Picture> GetPictures() { return Pictures.AsQueryable(); }
        void IDMDBContext.Add(Picture picture) => Pictures.Add(picture);
        public IQueryable<User> GetUsers() { return Users.AsQueryable(); }
        void IDMDBContext.Add(User user) => Users.Add(user);
        public IQueryable<MediaStreamer.Domain.Models.Style> GetStyles() { return Styles.AsQueryable(); }
        void IDMDBContext.Add(MediaStreamer.Domain.Models.Style style) => Styles.Add(style);

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
            switch (tableName) {
                case nameof(Administrators): Administrators.RemoveRange(Administrators); break;
                case nameof(Albums): Albums.RemoveRange(Albums); break;
                case nameof(AlbumGenres): AlbumGenres.RemoveRange(AlbumGenres); break;
                case nameof(Artists): Artists.RemoveRange(Artists); break;
                case nameof(ArtistGenres): ArtistGenres.RemoveRange(ArtistGenres); break;
                case nameof(Compositions): Compositions.RemoveRange(Compositions); break;
                case nameof(Genres): Genres.RemoveRange(Genres); break;
                case nameof(ListenedCompositions): ListenedCompositions.RemoveRange(ListenedCompositions); break;
                case nameof(Moderators): Moderators.RemoveRange(Moderators); break;
                case nameof(Pictures): Pictures.RemoveRange(Pictures); break;
                case nameof(Users): Users.RemoveRange(Users); break;
            }
            SaveChanges();
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
