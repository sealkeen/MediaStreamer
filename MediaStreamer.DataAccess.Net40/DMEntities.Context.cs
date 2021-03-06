//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MediaStreamer.DataAccess.Net40.SQLite
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using MediaStreamer.Domain;

    public partial class DMEntities : DbContext, IDMDBContext//, MediaStreamer.Domain.IDMDBContext
    {
        public DMEntities()
            : base("name=DMEntities")
        {
            
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
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

        //Custom Methods
        public void Clear()
        {
            Database.Delete();
            Database.Create();
        }
        public void AddEntity<T>(T entity) where T : class
        {
            if (typeof(T) == typeof(Composition))
            
                Compositions.Add(entity as Composition);
            else if (typeof(T) == typeof(Album)) 
                Albums.Add(entity as Album);
            else if (typeof(T) == typeof(ListenedComposition))
                ListenedCompositions.Add(entity as ListenedComposition);
        }
        public void RemoveEntity<T>(T entity) where T : class
        {
            if (typeof(T) == typeof(Composition))
                Compositions.Remove(entity as Composition);
            else if (typeof(T) == typeof(Album))
                Albums.Remove(entity as Album);
            else if (typeof(T) == typeof(ListenedComposition))
                ListenedCompositions.Remove(entity as ListenedComposition);
        }

        public Task<IQueryable<Composition>> GetCompositionsAsync()
        {
            return Task.Factory.StartNew(GetCompositions);
        }
        public IQueryable<Administrator> GetAdministrators() { return Administrators; }
        public void Add(Administrator administrator) => Administrators.Add(administrator);
        public IQueryable<Album> GetAlbums() { return Albums; }
        public void Add(Album album) => Albums.Add(album);
        public IQueryable<AlbumGenre> GetAlbumGenres(){ return AlbumGenres; }
        public void Add(AlbumGenre albumGenre) => AlbumGenres.Add(albumGenre);
        public IQueryable<Artist> GetArtists(){ return Artists; }
        public void Add(Artist artist) => Artists.Add(artist);
        public IQueryable<ArtistGenre> GetArtistGenres(){ return ArtistGenres; }
        public void Add(ArtistGenre artistGenre) => ArtistGenres.Add(artistGenre);
        public IQueryable<Composition> GetCompositions(){ return Compositions; }
        public IQueryable<IComposition> GetICompositions() { return Compositions; }
        public void Add(Composition composition) => Compositions.Add(composition);
        public IQueryable<CompositionVideo> GetCompositionVideos(){ return CompositionVideos; }
        public void Add(CompositionVideo compositionVideo) => CompositionVideos.Add(compositionVideo);
        public IQueryable<Genre> GetGenres(){ return Genres; }
        public void Add(Genre genre) => Genres.Add(genre);
        public IQueryable<GroupMember> GetGroupMembers(){ return GroupMembers; }
        public void Add(GroupMember groupMember) => GroupMembers.Add(groupMember);
        public IQueryable<GroupRole> GetGroupRoles(){ return GroupRoles; }
        public void Add(GroupRole groupRole) => GroupRoles.Add(groupRole);
        public IQueryable<ListenedAlbum> GetListenedAlbums(){ return ListenedAlbums; }
        public void Add(ListenedAlbum listenedAlbum) => ListenedAlbums .Add(listenedAlbum);
        public IQueryable<ListenedArtist> GetListenedArtists(){ return ListenedArtists; }
        public void Add(ListenedArtist listenedArtist) => ListenedArtists.Add(listenedArtist);
        public IQueryable<ListenedComposition> GetListenedCompositions(){ return ListenedCompositions; }
        public void Add(ListenedComposition listenedComposition) => ListenedCompositions.Add(listenedComposition);
        public IQueryable<ListenedGenre> GetListenedGenres(){ return ListenedGenres; }
        public void Add(ListenedGenre listenedGenre) => ListenedGenres.Add(listenedGenre);
        public IQueryable<Moderator> GetModerators(){ return Moderators; }
        public void Add(Moderator moderator) => Moderators.Add(moderator);
        public IQueryable<Musician> GetMusicians(){ return Musicians; }
        public void Add(Musician musician) => Musicians.Add(musician);
        public IQueryable<MusicianRole> GetMusicianRoles(){ return MusicianRoles; }
        public void Add(MusicianRole musicianRole) => MusicianRoles.Add(musicianRole);
        public IQueryable<Picture> GetPictures(){ return Pictures; }
        public void Add(Picture picture) => Pictures.Add(picture);
        public IQueryable<User> GetUsers(){ return Users; }
        public void Add(User user) => Users.Add(user);
        public IQueryable<Video> GetVideos(){ return Videos; }
        public void Add(Video video) => Videos.Add(video);


        public void UpdateAndSaveChanges<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void EnsureCreated()
        {
            throw new NotImplementedException();
        }
        public void DisableLazyLoading()
        {

        }
        //IQueryable<MediaStreamer.Domain.Administrator> MediaStreamer.Domain.IDMDBContext.GetAdministrators()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.Administrator administrator)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.Album> MediaStreamer.Domain.IDMDBContext.GetAlbums()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.Album administrator)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.AlbumGenre> MediaStreamer.Domain.IDMDBContext.GetAlbumGenres()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.AlbumGenre albumGenre)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.Artist> MediaStreamer.Domain.IDMDBContext.GetArtists()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.Artist artist)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.ArtistGenre> MediaStreamer.Domain.IDMDBContext.GetArtistGenres()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.ArtistGenre artistGenre)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.Composition> MediaStreamer.Domain.IDMDBContext.GetCompositions()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IQueryable<MediaStreamer.Domain.Composition>> GetCompositionsAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.IComposition> MediaStreamer.Domain.IDMDBContext.GetICompositions()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.Composition composition)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.CompositionVideo> MediaStreamer.Domain.IDMDBContext.GetCompositionVideos()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.CompositionVideo compositionVideo)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.Genre> MediaStreamer.Domain.IDMDBContext.GetGenres()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.Genre genre)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.GroupMember> MediaStreamer.Domain.IDMDBContext.GetGroupMembers()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.GroupMember groupMember)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.GroupRole> MediaStreamer.Domain.IDMDBContext.GetGroupRoles()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.GroupRole groupRole)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.ListenedAlbum> MediaStreamer.Domain.IDMDBContext.GetListenedAlbums()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.ListenedAlbum listenedAlbum)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.ListenedArtist> MediaStreamer.Domain.IDMDBContext.GetListenedArtists()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.ListenedArtist listenedArtist)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.ListenedComposition> MediaStreamer.Domain.IDMDBContext.GetListenedCompositions()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.ListenedComposition listenedComposition)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.ListenedGenre> MediaStreamer.Domain.IDMDBContext.GetListenedGenres()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.ListenedGenre listenedGenre)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.Moderator> MediaStreamer.Domain.IDMDBContext.GetModerators()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.Moderator moderator)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.Musician> MediaStreamer.Domain.IDMDBContext.GetMusicians()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.Musician musician)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.MusicianRole> MediaStreamer.Domain.IDMDBContext.GetMusicianRoles()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.MusicianRole musicianRole)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.Picture> MediaStreamer.Domain.IDMDBContext.GetPictures()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.Picture picture)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.User> MediaStreamer.Domain.IDMDBContext.GetUsers()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.User user)
        //{
        //    throw new NotImplementedException();
        //}

        //IQueryable<MediaStreamer.Domain.Video> MediaStreamer.Domain.IDMDBContext.GetVideos()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(MediaStreamer.Domain.Video video)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
