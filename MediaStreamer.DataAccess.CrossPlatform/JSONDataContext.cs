using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
            JObject jAlbum = new JObject(root);

            List<JKeyValuePair> list = new List<JKeyValuePair>( );

            list.Add(new JKeyValuePair(Key.AlbumID, DataBase.Coalesce(album.AlbumID), jAlbum));
            list.Add(new JKeyValuePair(Key.AlbumName, DataBase.Coalesce(album.AlbumName), jAlbum));
            list.Add(new JKeyValuePair(Key.ArtistID, DataBase.Coalesce(album.ArtistID), jAlbum));
            list.Add(new JKeyValuePair(Key.GenreID, DataBase.Coalesce(album.GenreID), jAlbum));
            list.Add(new JKeyValuePair(Key.Year, DataBase.Coalesce(album.Year), jAlbum));
            list.Add(new JKeyValuePair(Key.Type, DataBase.Coalesce(album.Type), jAlbum));
            list.Add(new JKeyValuePair(Key.Label, DataBase.Coalesce(album.Label), jAlbum));

            // Already Exists, return 
            if (root.HasThesePairsRecursive(list) != null)
                return;

            jAlbum.AddPairs(list);

            root.Add(jAlbum);
            root.ToFile(AlbumsDB);
        }

        public void Add(AlbumGenre albumGenre)
        {
            string AlbumGenresDB = Path.Combine(FolderName, "AlbumGenres");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "AlbumGenres");
            JObject jAlbum = new JObject(root);

            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.ArtistID, DataBase.Coalesce(albumGenre.ArtistID), jAlbum));
            list.Add(new JKeyValuePair(Key.AlbumID, DataBase.Coalesce(albumGenre.AlbumID), jAlbum));

            // Already Exists, return 
            if (root.HasThesePairsRecursive(list) != null)
                return;

            jAlbum.AddPairs(list);

            root.Add(jAlbum);
            root.ToFile(AlbumGenresDB);
        }

        public void Add(Artist artist)
        {
            string AlbumGenresDB = Path.Combine(FolderName, "Artists");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Artists");
            JObject jArtist = new JObject(root);

            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.ArtistID, DataBase.Coalesce(artist.ArtistID), jArtist));
            list.Add(new JKeyValuePair(Key.ArtistName, DataBase.Coalesce(artist.ArtistName), jArtist));

            // Already Exists, return 
            if (root.HasThesePairsRecursive(list) != null)
                return;

            jArtist.AddPairs(list); 

            root.Add(jArtist);
            root.ToFile(AlbumGenresDB);
        }

        public void Add(ArtistGenre artistGenre)
        {
            string ArtistGenresDB = Path.Combine(FolderName, "ArtistGenres");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "ArtistGenres");
            JObject jAG = new JObject(root);

            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.ArtistID, DataBase.Coalesce(artistGenre.ArtistID), jAG));
            list.Add(new JKeyValuePair(Key.GenreID, DataBase.Coalesce(artistGenre.GenreID), jAG));

            // Already Exists, return 
            if (root.HasThesePairsRecursive(list) != null)
                return;

            jAG.AddPairs(list);

            root.Add(jAG);
            root.ToFile(ArtistGenresDB);
        }

        public void Add(Composition composition)
        {
            string CompositionsDB = Path.Combine(FolderName, "Compositions");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Compositions");
            JObject jComposition = new JObject(root);

            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.CompositionID, DataBase.Coalesce(composition.CompositionID), jComposition));
            list.Add(new JKeyValuePair(Key.CompositionName, DataBase.Coalesce(composition.CompositionName), jComposition));
            list.Add(new JKeyValuePair(Key.ArtistID, DataBase.Coalesce(composition.ArtistID), jComposition));
            list.Add(new JKeyValuePair(Key.AlbumID, DataBase.Coalesce(composition.ArtistID), jComposition));
            list.Add(new JKeyValuePair(Key.Duration, DataBase.Coalesce(composition.Duration), jComposition));
            list.Add(new JKeyValuePair(Key.FilePath, DataBase.Coalesce(composition.FilePath), jComposition));
            list.Add(new JKeyValuePair(Key.Lyrics, DataBase.Coalesce(composition.Lyrics), jComposition));
            list.Add(new JKeyValuePair(Key.About, DataBase.Coalesce(composition.About), jComposition));

            // Already Exists, return 
            if (root.HasThesePairsRecursive(list) != null)
                return;

            jComposition.AddPairs(list);

            root.Add(jComposition);
            root.ToFile(CompositionsDB);
        }

        public void Add(CompositionVideo compositionVideo)
        {
            throw new NotImplementedException();
        }

        public void Add(Genre genre)
        {
            string genresDB = Path.Combine(FolderName, "Genres");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Genres");
            JObject jAG = new JObject(root);

            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.GenreID, DataBase.Coalesce(genre.GenreID), jAG));
            list.Add(new JKeyValuePair(Key.GenreName, DataBase.Coalesce(genre.GenreName), jAG));

            // Already Exists, return 
            if (root.HasThesePairsRecursive(list) != null)
                return;

            jAG.AddPairs(list);

            root.Add(jAG);
            root.ToFile(genresDB);
        }

        public void Add(GroupMember groupMember)
        {
            //throw new NotImplementedException();
        }

        public void Add(GroupRole groupRole)
        {
            //throw new NotImplementedException();
        }

        public void Add(ListenedAlbum listenedAlbum)
        {
            //throw new NotImplementedException();
        }

        public void Add(ListenedArtist listenedArtist)
        {
            //throw new NotImplementedException();
        }

        public void Add(ListenedComposition listenedComposition)
        {
            //throw new NotImplementedException();
        }

        public void Add(ListenedGenre listenedGenre)
        {
            //throw new NotImplementedException();
        }

        public void Add(Moderator moderator)
        {
            //throw new NotImplementedException();
        }

        public void Add(Musician musician)
        {
            //throw new NotImplementedException();
        }

        public void Add(MusicianRole musicianRole)
        {
            //throw new NotImplementedException();
        }

        public void Add(Picture picture)
        {
            //throw new NotImplementedException();
        }

        public void Add(User user)
        {
            //throw new NotImplementedException();
        }

        public void Add(Video video)
        {
            //throw new NotImplementedException();
        }

        public void AddEntity<T>(T entity) where T : class
        {
            if (typeof(T) == typeof(Composition))
                Compositions.Add(entity as Composition);
            else if (typeof(T) == typeof(Album))
                Albums.Add(entity as Album);
            else if (typeof(T) == typeof(ListenedComposition))
                ListenedCompositions.Add(entity as ListenedComposition);
            else if (typeof(T) == typeof(Genre))
                Genres.Add(entity as Genre);
            else if (typeof(T) == typeof(AlbumGenre))
                AlbumGenres.Add(entity as AlbumGenre);
            else if (typeof(T) == typeof(ArtistGenre))
                ArtistGenres.Add(entity as ArtistGenre);
        }

        public void Clear()
        {
            DataBase.DeleteTable(FolderName, "Genres");
            DataBase.DeleteTable(FolderName, "Artists");
            DataBase.DeleteTable(FolderName, "Albums");
            DataBase.DeleteTable(FolderName, "Compositions");
            DataBase.DeleteTable(FolderName, "ArtistGenres");
            DataBase.DeleteTable(FolderName, "AlbumGenres");

            EnsureCreated();
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
            var artistGenres = DataBase.LoadFromFileOrCreateRootObject(FolderName, "ArtistGenres");
            var albumGenres = DataBase.LoadFromFileOrCreateRootObject(FolderName, "AlbumGenres");
            var compositions = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Compositions");
        }

        public IQueryable<Administrator> GetAdministrators()
        {
            throw new NotImplementedException();
        }

        public IQueryable<AlbumGenre> GetAlbumGenres()
        {            
            string AlbumsDB = Path.Combine(FolderName, "AlbumGenres");

            if (!File.Exists(AlbumsDB))
                return (new List<AlbumGenre>()).AsQueryable();

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Albums");
            var jAlbums = root.Descendants();

            List<AlbumGenre> result = new List<AlbumGenre>();
            foreach (var jAlbum in jAlbums)
            {
                AlbumGenre received = new AlbumGenre();
                var fields = jAlbum.DescendantPairs();
                foreach (var kv in fields) 
                {
                    switch (kv.Key.ToString())
                    {
                        case Key.AlbumID:
                            DataBase.SetProperty(received, Key.AlbumID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.ArtistID:
                            DataBase.SetProperty(received, Key.ArtistID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.GenreID:
                            DataBase.SetProperty(received, Key.GenreID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                    }
                    result.Add(received);
                }
            }

            return result.AsQueryable();
        }

        public IQueryable<Album> GetAlbums()
        {
            string AlbumsDB = Path.Combine(FolderName, "Albums");

            if (!File.Exists(AlbumsDB))
                return (new List<Album>()).AsQueryable();

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Albums");
            var jAlbums = root.Descendants();

            List<Album> result = new List<Album>();
            foreach (var jAlbum in jAlbums)
            {
                Album received = new Album();
                var fields = jAlbum.DescendantPairs();
                foreach (var kv in fields) 
                {
                    switch (kv.Key.ToString())
                    {
                        case Key.AlbumID:
                            DataBase.SetProperty(received, Key.AlbumID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.AlbumName:
                            DataBase.SetProperty(received, Key.AlbumName, kv.GetPairedValue());
                            break;
                        case Key.ArtistID:
                            DataBase.SetProperty(received, Key.ArtistID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.GenreID:
                            DataBase.SetProperty(received, Key.GenreID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.Year:
                            DataBase.SetProperty(received, Key.Year, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.Type:
                            DataBase.SetProperty(received, Key.Type, kv.GetPairedValue());
                            break;
                        case Key.Label:
                            DataBase.SetProperty(received, Key.Label, kv.GetPairedValue());
                            break;
                    }
                    result.Add(received);
                }
            }

            return result.AsQueryable();
        }

        public IQueryable<ArtistGenre> GetArtistGenres()
        {
            string ArtistsDB = Path.Combine(FolderName, "ArtistGenres");

            if (!File.Exists(ArtistsDB))
                return (new List<ArtistGenre>()).AsQueryable();

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Artists");
            var jArtists = root.Descendants();

            List<ArtistGenre> result = new List<ArtistGenre>();
            foreach (var jArtist in jArtists)
            {
                ArtistGenre received = new ArtistGenre();
                var fields = jArtist.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString())
                    {
                        case Key.ArtistID:
                            DataBase.SetProperty(received, Key.ArtistID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.GenreID:
                            DataBase.SetProperty(received, Key.GenreID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                    }
                    result.Add(received);
                }
            }
            return result.AsQueryable();
        }

        public IQueryable<Artist> GetArtists()
        {
            string ArtistsDB = Path.Combine(FolderName, "Artists");

            if (!File.Exists(ArtistsDB))
                return (new List<Artist>()).AsQueryable();

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Artists");
            var jArtists = root.Descendants();

            List<Artist> result = new List<Artist>();
            foreach (var jArtist in jArtists)
            {
                Artist received = new Artist();
                var fields = jArtist.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString())
                    {
                        case Key.ArtistID:
                            DataBase.SetProperty(received, Key.ArtistID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.ArtistName:
                            DataBase.SetProperty(received, Key.ArtistName, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                    }
                    result.Add(received);
                }
            }

            return result.AsQueryable();
        }

        public IQueryable<Composition> GetCompositions()
        {
            string CompositionsDB = Path.Combine(FolderName, "Compositions");

            if (!File.Exists(CompositionsDB))
                return (new List<Composition>()).AsQueryable();

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Compositions");
            var jCompositions = root.Descendants();

            List<Composition> result = new List<Composition>();
            foreach (var jComposition in jCompositions)
            {
                Composition received = new Composition();
                var fields = jComposition.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString())
                    {
                        case Key.CompositionID:
                            DataBase.SetProperty(received, Key.CompositionID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.CompositionName:
                            DataBase.SetProperty(received, Key.CompositionName, kv.GetPairedValue());
                            break;
                        case Key.ArtistID:
                            DataBase.SetProperty(received, Key.ArtistID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.AlbumID:
                            DataBase.SetProperty(received, Key.AlbumID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.Duration:
                            DataBase.SetProperty(received, Key.Duration, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.FilePath:
                            DataBase.SetProperty(received, Key.FilePath, kv.GetPairedValue());
                            break;
                        case Key.Lyrics:
                            DataBase.SetProperty(received, Key.Lyrics, kv.GetPairedValue());
                            break;
                        case Key.About:
                            DataBase.SetProperty(received, Key.About, kv.GetPairedValue());
                            break;
                    }
                    result.Add(received);
                }
            }

            return result.AsQueryable();
        }

        public Task<IQueryable<Composition>> GetCompositionsAsync()
        {
            return Task.Factory.StartNew(() => GetCompositions());
        }

        public IQueryable<CompositionVideo> GetCompositionVideos()
        {
            return new List<CompositionVideo>().AsQueryable();
        }

        public IQueryable<Genre> GetGenres()
        {
            string GenresDB = Path.Combine(FolderName, "Genres");

            if (!File.Exists(GenresDB))
                return (new List<Genre>()).AsQueryable();

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Genres");
            var jGenres = root.Descendants();

            List<Genre> result = new List<Genre>();
            foreach (var jGenre in jGenres)
            {
                Genre received = new Genre();
                var fields = jGenre.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString())
                    {
                        case Key.GenreID:
                            DataBase.SetProperty(received, Key.GenreID, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                        case Key.GenreName:
                            DataBase.SetProperty(received, Key.GenreName, DataBase.TryParseInt(kv.GetPairedValue()));
                            break;
                    }
                    result.Add(received);
                }
            }

            return result.AsQueryable();
        }

        public IQueryable<GroupMember> GetGroupMembers()
        {
            return new List<GroupMember>().AsQueryable();
        }

        public IQueryable<GroupRole> GetGroupRoles()
        {
            return new List<GroupRole>().AsQueryable();
        }

        public IQueryable<IComposition> GetICompositions()
        {
            return GetCompositions().AsQueryable();//.Include(c => c.Artist); 
        }

        public IQueryable<ListenedAlbum> GetListenedAlbums()
        {
            return new List<ListenedAlbum>().AsQueryable();
        }

        public IQueryable<ListenedArtist> GetListenedArtists()
        {
            return new List<ListenedArtist>().AsQueryable();
        }

        public IQueryable<ListenedComposition> GetListenedCompositions()
        {
            return new List<ListenedComposition>().AsQueryable();
        }

        public IQueryable<ListenedGenre> GetListenedGenres()
        {
            return new List<ListenedGenre>().AsQueryable();
        }

        public IQueryable<Moderator> GetModerators()
        {
            return new List<Moderator>().AsQueryable();
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
            return new List<User>().AsQueryable();
        }

        public IQueryable<Video> GetVideos()
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity<T>(T o) where T : class
        {
            //throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            return 1;
            //throw new NotImplementedException();
        }

        public void UpdateAndSaveChanges<TEntity>(TEntity entity) where TEntity : class
        {
            //throw new NotImplementedException();
        }
    }
}
