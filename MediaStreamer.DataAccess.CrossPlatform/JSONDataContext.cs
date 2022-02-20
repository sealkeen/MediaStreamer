using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MediaStreamer.Domain;
using EPAM.CSCourse2016.JSONParser.Library;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class JSONDataContext : IDMDBContext
    {
        public virtual List<Administrator> Administrators { get; set; }
        public virtual List<Album> Albums { get; set; }
        public virtual List<AlbumGenre> AlbumGenres { get; set; }
        public virtual List<Artist> Artists { get; set; }
        public virtual List<ArtistGenre> ArtistGenres { get; set; }
        public virtual List<Composition> Compositions { get; set; }
        public virtual List<CompositionVideo> CompositionVideos { get; set; }
        public virtual List<Genre> Genres { get; set; }
        public virtual List<GroupMember> GroupMembers { get; set; }
        public virtual List<GroupRole> GroupRoles { get; set; }
        public virtual List<ListenedAlbum> ListenedAlbums { get; set; }
        public virtual List<ListenedArtist> ListenedArtists { get; set; }
        public virtual List<ListenedComposition> ListenedCompositions { get; set; }
        public virtual List<ListenedGenre> ListenedGenres { get; set; }
        public virtual List<Moderator> Moderators { get; set; }
        public virtual List<Musician> Musicians { get; set; }
        public virtual List<MusicianRole> MusicianRoles { get; set; }
        public virtual List<Picture> Pictures { get; set; }
        public virtual List<User> Users { get; set; }
        public virtual List<Video> Videos { get; set; }

        public string FolderName { get; set; } = "Compositions";
        
        public void Add(Administrator administrator)
        {
            throw new NotImplementedException();
        }

        public void Add(Album album)
        {
            string AlbumsDB = Path.Combine(FolderName, "Albums");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Albums");
            
            List<JKeyValuePair> list = new List<JKeyValuePair>( );
            list.Add(new JKeyValuePair("ArtistID", album.ArtistID.ToString()));
            list.Add(new JKeyValuePair("AlbumName", album.AlbumName));

            if (root.HasThesePairsRecursive(list) != null)
                return;

            JObject jAlbum = new JObject(root);

            jAlbum.Add(new JKeyValuePair("ID", DataBase.Coalesce(album.AlbumID), jAlbum));
            jAlbum.Add(new JKeyValuePair("AlbumName", DataBase.Coalesce(album.AlbumName), jAlbum));
            jAlbum.Add(new JKeyValuePair("ArtistID", DataBase.Coalesce(album.ArtistID), jAlbum));
            jAlbum.Add(new JKeyValuePair("GenreID", DataBase.Coalesce(album.GenreID), jAlbum));
            jAlbum.Add(new JKeyValuePair("Year", DataBase.Coalesce(album.Year), jAlbum));
            jAlbum.Add(new JKeyValuePair("Type", DataBase.Coalesce(album.Type), jAlbum));
            jAlbum.Add(new JKeyValuePair("Label", DataBase.Coalesce(album.Label), jAlbum));

            root.Add(jAlbum);
            root.ToFile(AlbumsDB);
        }

        public void Add(AlbumGenre albumGenre)
        {
            throw new NotImplementedException();
        }

        public void Add(Artist artist)
        {
            throw new NotImplementedException();
        }

        public void Add(ArtistGenre artistGenre)
        {
            throw new NotImplementedException();
        }

        public void Add(Composition composition)
        {
            throw new NotImplementedException();
        }

        public void Add(CompositionVideo compositionVideo)
        {
            throw new NotImplementedException();
        }

        public void Add(Genre genre)
        {
            throw new NotImplementedException();
        }

        public void Add(GroupMember groupMember)
        {
            throw new NotImplementedException();
        }

        public void Add(GroupRole groupRole)
        {
            throw new NotImplementedException();
        }

        public void Add(ListenedAlbum listenedAlbum)
        {
            throw new NotImplementedException();
        }

        public void Add(ListenedArtist listenedArtist)
        {
            throw new NotImplementedException();
        }

        public void Add(ListenedComposition listenedComposition)
        {
            throw new NotImplementedException();
        }

        public void Add(ListenedGenre listenedGenre)
        {
            throw new NotImplementedException();
        }

        public void Add(Moderator moderator)
        {
            throw new NotImplementedException();
        }

        public void Add(Musician musician)
        {
            throw new NotImplementedException();
        }

        public void Add(MusicianRole musicianRole)
        {
            throw new NotImplementedException();
        }

        public void Add(Picture picture)
        {
            throw new NotImplementedException();
        }

        public void Add(User user)
        {
            throw new NotImplementedException();
        }

        public void Add(Video video)
        {
            throw new NotImplementedException();
        }

        public void AddEntity<T>(T o) where T : class
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            //throw new NotImplementedException();
        }

        public void DisableLazyLoading()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void EnsureCreated()
        {
            var genres = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Genres");
            var artists = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Artists");
            var albums = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Albums");
            var compositions = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Compositions");
        }

        public IQueryable<Administrator> GetAdministrators()
        {
            throw new NotImplementedException();
        }

        public IQueryable<AlbumGenre> GetAlbumGenres()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Album> GetAlbums()
        {
            string AlbumsDB = Path.Combine(FolderName, "Albums");

            if (!File.Exists(AlbumsDB))
                return (new List<Album>()).AsQueryable();

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Albums");

            var jAlbums = root.Descendants();


            List<JKeyValuePair> list = new List<JKeyValuePair>();

            if (root.HasThesePairsRecursive(list) != null)
                return;
        }

        public IQueryable<ArtistGenre> GetArtistGenres()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Artist> GetArtists()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Composition> GetCompositions()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Composition>> GetCompositionsAsync()
        {
            throw new NotImplementedException();
        }

        public IQueryable<CompositionVideo> GetCompositionVideos()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Genre> GetGenres()
        {
            throw new NotImplementedException();
        }

        public IQueryable<GroupMember> GetGroupMembers()
        {
            throw new NotImplementedException();
        }

        public IQueryable<GroupRole> GetGroupRoles()
        {
            throw new NotImplementedException();
        }

        public IQueryable<IComposition> GetICompositions()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ListenedAlbum> GetListenedAlbums()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ListenedArtist> GetListenedArtists()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ListenedComposition> GetListenedCompositions()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ListenedGenre> GetListenedGenres()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Moderator> GetModerators()
        {
            throw new NotImplementedException();
        }

        public IQueryable<MusicianRole> GetMusicianRoles()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Musician> GetMusicians()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Picture> GetPictures()
        {
            throw new NotImplementedException();
        }

        public IQueryable<User> GetUsers()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Video> GetVideos()
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity<T>(T o) where T : class
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void UpdateAndSaveChanges<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
