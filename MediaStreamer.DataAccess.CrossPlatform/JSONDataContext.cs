using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System.Collections.Concurrent;
using MediaStreamer.Domain.Models;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class JSONDataContext : IPagedDMDBContext
    {
        Action<string> _log;

        public JSONDataContext() : this (null) {  }

        public JSONDataContext(Action<string> log = null)
        {
            if (log != null)
                _log = log;
            else
                _log = Console.WriteLine;
            FolderName = PathResolver.GetStandardDatabasePath();
            TableInfo = new ConcurrentDictionary<string, long>();

            Genres = new List<Genre>();
            Artists = new List<Artist>();
            Albums = new List<Album>();
            Compositions = new List<Composition>();
            ArtistGenres = new List<ArtistGenre>();
            AlbumGenres = new List<AlbumGenre>();
            ListenedCompositions = new List<ListenedComposition>();
        }

        public JSONDataContext(Action<string> log = null, string dbPath = null) : this(log)
        {
            FolderName = dbPath;
        }

        public virtual ConcurrentDictionary<string, long> TableInfo { get; set; }
        public virtual List<Album> Albums { get; set; }
        public virtual List<AlbumGenre> AlbumGenres { get; set; }
        public virtual List<Artist> Artists { get; set; }
        public virtual List<ArtistGenre> ArtistGenres { get; set; }
        public virtual List<Composition> Compositions { get; set; }
        public virtual List<Administrator> Administrators { get; set; }
        public virtual List<Moderator> Moderators { get; set; }
        public virtual List<Style> Styles { get; set; }
        public virtual List<User> Users { get; set; }
        public virtual List<Genre> Genres { get; set; }
        public virtual List<ListenedComposition> ListenedCompositions { get; set; }


        public string FolderName { get; set; } = "Compositions";
        public string GetContainingFolderPath() => FolderName;

        public void InitializeTableInfo()
        {
            Dictionary<string, string> paths = new Dictionary<string, string>() {
                { nameof(ListenedCompositions), Path.Combine(FolderName, "ListenedCompositions.json") },
                { nameof(Compositions), Path.Combine(FolderName, "Compositions.json") },
                { nameof(ArtistGenres), Path.Combine(FolderName, "ArtistGenres.json") },
                { nameof(AlbumGenres), Path.Combine(FolderName, "AlbumGenres.json") },
                { nameof(Artists), Path.Combine(FolderName, "Artists.json") },
                { nameof(Albums), Path.Combine(FolderName, "Albums.json") },
                { nameof(Genres), Path.Combine(FolderName, "Genres.json") },
            };

            foreach (var pair in paths)
            { 
                TableInfo[pair.Key] = Table.GetTableSize(pair.Value);
            }
        }

        public void Add(Administrator administrator)
        {
            throw new NotImplementedException();
        }

        public void Add(Album album)
        {
            string AlbumsDB = Path.Combine(FolderName, "Albums.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Albums.json");
            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("Albums".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);

            JObject jAlbum = new JObject(itemsCollection);

            List<JKeyValuePair> list = new List<JKeyValuePair>();

            //Properties
            list.Add(new JKeyValuePair(Key.AlbumID.ToJString(), DataBase.Coalesce(album.AlbumID).ToSingleValue(), jAlbum));
            list.Add(new JKeyValuePair(Key.AlbumName, DataBase.Coalesce(album.AlbumName), jAlbum));
            list.Add(new JKeyValuePair(Key.ArtistID.ToJString(), DataBase.Coalesce(album.ArtistID).ToSingleValue(), jAlbum));
            list.Add(new JKeyValuePair(Key.GenreID.ToJString(), DataBase.Coalesce(album.GenreID).ToSingleValue(), jAlbum));
            list.Add(new JKeyValuePair(Key.Year, DataBase.Coalesce(album.Year), jAlbum));
            list.Add(new JKeyValuePair(Key.Type, DataBase.Coalesce(album.Type), jAlbum));
            list.Add(new JKeyValuePair(Key.Label, DataBase.Coalesce(album.Label), jAlbum));

            // Already Exists, return 
            if (Albums.Where(
                    c =>
                    c.AlbumName == album.AlbumName &&
                    c.ArtistID == album.ArtistID).Count() != 0)
                return;

            jAlbum.AddPairs(list);
            itemsCollection.Add(jAlbum);
            root.ToFile(AlbumsDB);
        }

        public void Add(AlbumGenre albumGenre)
        {
            string AlbumGenresDB = Path.Combine(FolderName, "AlbumGenres.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "AlbumGenres.json");
            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("AlbumGenres".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);
            JObject jAlbum = new JObject(itemsCollection);

            //Properties
            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.AlbumID.ToJString(), DataBase.Coalesce(albumGenre.AlbumID).ToSingleValue(), jAlbum));
            list.Add(new JKeyValuePair(Key.GenreID.ToJString(), DataBase.Coalesce(albumGenre.GenreID).ToSingleValue(), jAlbum));

            // Already Exists, return 
            if (AlbumGenres.Where(
                    c =>
                    c.AlbumID == albumGenre.AlbumID &&
                    c.GenreID == albumGenre.GenreID).Count() != 0)
                return;

            jAlbum.AddPairs(list);

            itemsCollection.Add(jAlbum);
            root.ToFile(AlbumGenresDB);
        }

        public void Add(Artist artist)
        {
            string AlbumGenresDB = Path.Combine(FolderName, "Artists.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Artists.json");
            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("Artists".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);

            JObject jArtist = new JObject(itemsCollection);

            //Properties
            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.ArtistID.ToJString(), DataBase.Coalesce(artist.ArtistID).ToSingleValue(), jArtist));
            list.Add(new JKeyValuePair(Key.ArtistName, DataBase.Coalesce(artist.ArtistName), jArtist));

            // Already Exists, return 
            if (Artists.Where(
                    c => c.ArtistName == artist.ArtistName).Count() != 0)
                return;

            jArtist.AddPairs(list);
            itemsCollection.Add(jArtist);
            root.ToFile(AlbumGenresDB);
        }

        public void Add(ArtistGenre artistGenre)
        {
            string ArtistGenresDB = Path.Combine(FolderName, "ArtistGenres.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "ArtistGenres.json");
            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("ArtistGenres".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);

            JObject jAG = new JObject(itemsCollection);

            //Properties
            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.ArtistID.ToJString(), DataBase.Coalesce(artistGenre.ArtistID).ToSingleValue(), jAG));
            list.Add(new JKeyValuePair(Key.GenreID.ToJString(), DataBase.Coalesce(artistGenre.GenreID).ToSingleValue(), jAG));

            // Already Exists, return 
            if (ArtistGenres.Where(
                    c => c.ArtistID == artistGenre.ArtistID &&
                    c.GenreID == artistGenre.GenreID
                    ).Count() != 0)
                return;

            jAG.AddPairs(list);
            itemsCollection.Add(jAG);
            root.ToFile(ArtistGenresDB);
        }

        public void Add(Composition composition)
        {
            if (composition == null)
                return;
            string CompositionsDB = Path.Combine(FolderName, "Compositions.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Compositions.json");
            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("Compositions".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);

            JObject jComposition = new JObject(itemsCollection);

            //Properties
            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.CompositionID.ToJString(), DataBase.Coalesce(composition.CompositionID).ToSingleValue(), jComposition));
            list.Add(new JKeyValuePair(Key.CompositionName, DataBase.Coalesce(composition.CompositionName), jComposition));
            list.Add(new JKeyValuePair(Key.ArtistID.ToJString(), DataBase.Coalesce(composition.ArtistID).ToSingleValue(), jComposition));
            list.Add(new JKeyValuePair(Key.AlbumID.ToJString(), DataBase.Coalesce(composition.AlbumID).ToSingleValue(), jComposition));
            list.Add(new JKeyValuePair(Key.Duration.ToJString(), DataBase.Coalesce(composition.Duration).ToSingleValue(), jComposition));
            list.Add(new JKeyValuePair(Key.FilePath, DataBase.Coalesce(composition.FilePath), jComposition));
            list.Add(new JKeyValuePair(Key.Lyrics, DataBase.Coalesce(composition.Lyrics), jComposition));
            list.Add(new JKeyValuePair(Key.About, DataBase.Coalesce(composition.About), jComposition));

            // Already Exists, return 
            if (Compositions.Where(
                    c =>
                    c.CompositionName == composition.CompositionName &&
                    c.FilePath == composition.FilePath).Count() != 0)
                return;

            jComposition.AddPairs(list);
            itemsCollection.Add(jComposition);
            root.ToFile(CompositionsDB);
        }

        public void AddRange(IEnumerable<Composition> newCompositions)
        {
            string CompositionsDB = Path.Combine(FolderName, "Compositions.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Compositions.json");
            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("Compositions".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);
            foreach (var comp in newCompositions)
            {
                if (comp == null)
                    continue;
                JObject jComposition = new JObject(itemsCollection);
                //Properties
                List<JKeyValuePair> list = new List<JKeyValuePair>();
                list.Add(new JKeyValuePair(Key.CompositionID.ToJString(), DataBase.Coalesce(comp.CompositionID).ToSingleValue(), jComposition));
                list.Add(new JKeyValuePair(Key.CompositionName, DataBase.Coalesce(comp.CompositionName), jComposition));
                list.Add(new JKeyValuePair(Key.ArtistID.ToJString(), DataBase.Coalesce(comp.ArtistID).ToSingleValue(), jComposition));
                list.Add(new JKeyValuePair(Key.AlbumID.ToJString(), DataBase.Coalesce(comp.AlbumID).ToSingleValue(), jComposition));
                list.Add(new JKeyValuePair(Key.Duration.ToJString(), DataBase.Coalesce(comp.Duration).ToSingleValue(), jComposition));
                list.Add(new JKeyValuePair(Key.FilePath, DataBase.Coalesce(comp.FilePath), jComposition));
                list.Add(new JKeyValuePair(Key.Lyrics, DataBase.Coalesce(comp.Lyrics), jComposition));
                list.Add(new JKeyValuePair(Key.About, DataBase.Coalesce(comp.About), jComposition));

                // Already Exists, return 
                if (Compositions.Where(
                        c =>
                        c.CompositionName == comp.CompositionName &&
                        c.FilePath == comp.FilePath).Count() != 0)
                    continue;

                jComposition.AddPairs(list);
                itemsCollection.Add(jComposition);
                Compositions.Add(comp);
            }
            root.ToFile(CompositionsDB);
        }

        public void Add(CompositionVideo compositionVideo)
        {
            throw new NotImplementedException();
        }


        public void Add(Genre genre)
        {
            string genresDB = Path.Combine(FolderName, "Genres.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Genres.json");
            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("Genres".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);

            JObject jAG = new JObject(itemsCollection);

            //Properties
            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.GenreID.ToJString(), DataBase.Coalesce(genre.GenreID).ToSingleValue(), jAG));
            list.Add(new JKeyValuePair(Key.GenreName, DataBase.Coalesce(genre.GenreName), jAG));

            // Already Exists, return 
            if (Genres.Where(
                    c => c.GenreName == genre.GenreName).Count() != 0)
                return;

            jAG.AddPairs(list);
            itemsCollection.Add(jAG);
            root.ToFile(genresDB);
        }

        public void Add(ListenedComposition listenedComposition)
        {
            string listenedDB = Path.Combine(FolderName, "ListenedCompositions.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "ListenedCompositions.json");

            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("ListenedCompositions".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);

            JObject jLS = new JObject(itemsCollection);

            //Properties
            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.ListenDate.ToString(), listenedComposition.ListenDate.ToString(), jLS));
            list.Add(new JKeyValuePair(Key.CompositionID.ToString(), listenedComposition.CompositionID.ToString(), jLS));
            list.Add(new JKeyValuePair(Key.UserID.ToString(), listenedComposition.UserID.ToString(), jLS));
            list.Add(new JKeyValuePair(Key.StoppedAt.ToString(), listenedComposition.StoppedAt.ToString(), jLS));

            jLS.AddPairs(list);
            itemsCollection.Add(jLS);
            root.ToFile(listenedDB);
        }


        public void Add(Moderator moderator)
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
            if (typeof(T) == typeof(Composition)) {
                //var id = DataBase.GetMaxID<Composition, Guid>(Compositions.AsQueryable(), "CompositionID") + 1;
                //id++;
                //Table.SetProperty(entity, "CompositionID", id);
                Add(entity as Composition);
            } else if (typeof(T) == typeof(Artist)) {
                //var id = DataBase.GetMaxID<Artist, Guid>(Artists.AsQueryable(), "ArtistID");
                //id++;
                //Table.SetProperty(entity, "ArtistID", id);
                Add(entity as Artist);
            } else if (typeof(T) == typeof(Album)) {
                //var id = DataBase.GetMaxID<Album, Guid>(Albums.AsQueryable(), "AlbumID");
                //id++;
                //Table.SetProperty(entity, "AlbumID", id);
                Add(entity as Album);
            } else if (typeof(T) == typeof(Genre)) {
                //var id = DataBase.GetMaxID<Genre, Guid>(Genres.AsQueryable(), "GenreID");
                //id++;
                //Table.SetProperty(entity, "GenreID", id);
                Add(entity as Genre);
            } else if (typeof(T) == typeof(ListenedComposition))
                Add(entity as ListenedComposition);
            else if (typeof(T) == typeof(AlbumGenre))
                Add(entity as AlbumGenre);
            else if (typeof(T) == typeof(ArtistGenre))
                Add(entity as ArtistGenre);
        }

        public void Clear()
        {
            DataBase.DeleteTable(FolderName, "Genres.json");
            DataBase.DeleteTable(FolderName, "Artists.json");
            DataBase.DeleteTable(FolderName, "Albums.json");
            DataBase.DeleteTable(FolderName, "Compositions.json");
            DataBase.DeleteTable(FolderName, "ArtistGenres.json");
            DataBase.DeleteTable(FolderName, "AlbumGenres.json");
            DataBase.DeleteTable(FolderName, "ListenedCompositions.json");

            EnsureCreated();
        }

        public bool ClearTable(string tableName)
        {
            string certainTable = Path.Combine(FolderName, tableName + ".json");
            try {
                File.Delete(certainTable);
                return true;
            }
            catch {
                return false;
            }
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
            Genres = GetGenres().ToList();
            Artists = GetArtists().ToList();
            Albums = GetAlbums().ToList();
            Compositions = GetCompositions().ToList();
            ArtistGenres = GetArtistGenres().ToList();
            AlbumGenres = GetAlbumGenres().ToList();
            ListenedCompositions = GetListenedCompositions().ToList();
        }

        public IQueryable<Administrator> GetAdministrators()
        {
            throw new NotImplementedException();
        }

        public IQueryable<AlbumGenre> GetAlbumGenres()
        {
            if (!Table.SizeChanged(TableInfo, FolderName, nameof(AlbumGenres)))
                return AlbumGenres.AsQueryable();

            var jAlbums = Table.LoadInMemory(FolderName, "AlbumGenres.json");

            AlbumGenres = new List<AlbumGenre>();
            foreach (var jAlbum in jAlbums)
            {
                AlbumGenre received = new AlbumGenre();
                var fields = jAlbum.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.AlbumID:
                            Table.SetProperty(received, Key.AlbumID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.GenreID:
                            Table.SetProperty(received, Key.GenreID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                    }
                }
                AlbumGenres.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(AlbumGenres)] = Table.GetTableSize(Path.Combine(FolderName, "AlbumGenres.json")));
            return AlbumGenres.AsQueryable();
        }

        public IQueryable<Album> GetAlbums()
        {
            if (!Table.SizeChanged(TableInfo, FolderName, nameof(Albums)))
                return Albums.AsQueryable();

            var jAlbums = Table.LoadInMemory(FolderName, "Albums.json");

            Albums = new List<Album>();
            foreach (var jAlbum in jAlbums)
            {
                Album received = new Album();
                var fields = jAlbum.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.AlbumID:
                            Table.SetProperty(received, Key.AlbumID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.AlbumName:
                            Table.SetProperty(received, Key.AlbumName, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.ArtistID:
                            Table.SetProperty(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.GenreID:
                            Table.SetProperty(received, Key.GenreID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.Year:
                            Table.SetProperty(received, Key.Year, kv.GetIntegerValueOrReturnNull());
                            break;
                        case Key.Type:
                            Table.SetProperty(received, Key.Type, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.Label:
                            Table.SetProperty(received, Key.Label, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                if (Artists.Count == 0)
                    GetArtists();
                received.Artist = Table.GetLinkedEntity(received.ArtistID, Artists, "ArtistID");
                Albums.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Albums)] = Table.GetTableSize(Path.Combine(FolderName, "Albums.json")));
            return Albums.AsQueryable();
        }

        public IQueryable<ArtistGenre> GetArtistGenres()
        {
            if (!Table.SizeChanged(TableInfo, FolderName, nameof(ArtistGenres)))
                return ArtistGenres.AsQueryable();

            var jArtists = Table.LoadInMemory(FolderName, "ArtistGenres.json");

            ArtistGenres = new List<ArtistGenre>();
            foreach (var jArtist in jArtists)
            {
                ArtistGenre received = new ArtistGenre();
                var fields = jArtist.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.ArtistID:
                            Table.SetProperty(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.GenreID:
                            Table.SetProperty(received, Key.GenreID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                    }
                }
                ArtistGenres.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(ArtistGenres)] = Table.GetTableSize(Path.Combine(FolderName, "ArtistGenres.json")));
            return ArtistGenres.AsQueryable();
        }

        public IQueryable<Artist> GetArtists()
        {
            if (!Table.SizeChanged(TableInfo, FolderName, nameof(Artists)))
                return Artists.AsQueryable();

            var jArtists = Table.LoadInMemory(FolderName, "Artists.json");

            Artists = new List<Artist>();
            foreach (var jArtist in jArtists)
            {
                Artist received = new Artist();
                var fields = jArtist.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.ArtistID:
                            Table.SetProperty(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.ArtistName:
                            Table.SetProperty(received, Key.ArtistName, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                Artists.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Artists)] = Table.GetTableSize(Path.Combine(FolderName, "Artists.json")));
            return Artists.AsQueryable();
        }

        public IQueryable<Composition> GetCompositions()
        {
            if (!Table.SizeChanged(TableInfo, FolderName, nameof(Compositions)))
                return Compositions.AsQueryable();

            var jCompositions = Table.LoadInMemory(FolderName, "Compositions.json");

            Compositions = new List<Composition>();
            foreach (var jComposition in jCompositions)
            {
                Composition received = new Composition();
                var fields = jComposition.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.CompositionID:
                            Table.SetProperty(received, Key.CompositionID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.CompositionName:
                            Table.SetProperty(received, Key.CompositionName, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.ArtistID:
                            Table.SetProperty(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.AlbumID:
                            Table.SetProperty(received, Key.AlbumID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.Duration:
                            Table.SetProperty(received, Key.Duration, kv.GetIntegerValueOrReturnNull());
                            break;
                        case Key.FilePath:
                            Table.SetProperty(received, Key.FilePath, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.Lyrics:
                            Table.SetProperty(received, Key.Lyrics, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.About:
                            Table.SetProperty(received, Key.About, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                if (Artists.Count == 0)
                    GetArtists();
                received.Artist = Table.GetLinkedEntity(received.ArtistID, Artists, "ArtistID");
                Compositions.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Compositions)] = Table.GetTableSize(Path.Combine(FolderName, "Compositions.json")));
            return Compositions.AsQueryable();
        }
#if !NET40
        public async Task<List<Composition>> GetCompositionsAsync() => await Task.Run(() => GetCompositions().ToList());
        public async Task<List<IComposition>> GetICompositionsAsync() => await Task.Run(() => GetICompositions().ToList());
#else //Net Framework 4.0 doesn't support <await> until 4.5
        public async Task<List<Composition>> GetCompositionsAsync()
        { 
            return GetCompositions().ToList();
        }
        public async Task<List<IComposition>> GetICompositionsAsync()
        { 
            return GetICompositions().ToList();
        }
#endif

        public IQueryable<CompositionVideo> GetCompositionVideos()
        {
            return new List<CompositionVideo>().AsQueryable();
        }

        public IQueryable<Genre> GetGenres()
        {
            if (!Table.SizeChanged(TableInfo, FolderName, nameof(Genres)))
                return Genres.AsQueryable();

            var jGenres = Table.LoadInMemory(FolderName, "Genres.json");

            Genres = new List<Genre>();
            foreach (var jGenre in jGenres)
            {
                Genre received = new Genre();
                var fields = jGenre.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.GenreID:
                            Table.SetProperty(received, Key.GenreID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.GenreName:
                            Table.SetProperty(received, Key.GenreName, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                Genres.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Genres)] = Table.GetTableSize(Path.Combine(FolderName, "Genres.json")));
            return Genres.AsQueryable();
        }

        public IQueryable<IComposition> GetICompositions()
        {
            return GetCompositions().AsQueryable();//.Include(c => c.Artist); 
        }

        public IQueryable<ListenedComposition> GetListenedCompositions()
        {
            if (!Table.SizeChanged(TableInfo, FolderName, nameof(ListenedCompositions)))
                return ListenedCompositions.AsQueryable();

            var jCompositions = Table.LoadInMemory(FolderName, "ListenedCompositions.json");

            ListenedCompositions = new List<ListenedComposition>();
            foreach (var jComposition in jCompositions)
            {
                ListenedComposition received = new ListenedComposition();
                var fields = jComposition.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.ListenDate:
                            Table.SetProperty(received, Key.ListenDate, DateTime.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.CompositionID:
                            Table.SetProperty(received, Key.CompositionID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.UserID:
                            Table.SetProperty(received, Key.UserID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.StoppedAt:
                            Table.SetProperty(received, Key.StoppedAt, double.Parse(kv.Value.AsUnquoted()));
                            break;
                    }
                }
                ListenedCompositions.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(ListenedCompositions)] = Table.GetTableSize(Path.Combine(FolderName, "ListenedCompositions.json")));
            return ListenedCompositions.AsQueryable();
        }



        public IQueryable<Moderator> GetModerators()
        {
            return new List<Moderator>().AsQueryable();
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
            var tp = o.GetType();
            if (tp == typeof(Composition))
            {
                string CompositionsDB = Path.Combine(FolderName, "Compositions.json");

                var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Compositions.json");
                JItem itemsCollection = null;
                if (root != null)
                    itemsCollection = root.FindPairByKey("Compositions".ToJString()).GetPairedValue();
                else
                    itemsCollection = new JArray(root);

                List<JKeyValuePair> lst = new List<JKeyValuePair>();
                lst.Add(new JKeyValuePair(Key.CompositionID.ToJString(), new JSingleValue((o as Composition).CompositionID.ToString())));

                var pair = itemsCollection.HasThesePairsRecursive(lst);
                if (pair != null)
                {
                    pair.Parent.Parent.Items.Remove(pair.Parent);

                    root.ToFile(CompositionsDB);
                }
            }
        }

        public int SaveChanges()
        {
            return 1;
            //throw new NotImplementedException();
        }

        public void UpdateAndSaveChanges<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity is ListenedComposition)
            {
                //ListenedCompositions.
            }
        }

        public IQueryable<Style> GetStyles()
        {
            throw new NotImplementedException();
        }

        public Task<List<Composition>> GetCompositionsAsync(int skip, int take)
        {
            if (!Table.SizeChanged(TableInfo, FolderName, nameof(Compositions)))
                return Task.Factory.StartNew(() => Compositions.Skip(skip).Take(take).ToList());

            var jCompositions = Table.LoadInMemory(FolderName, "Compositions.json");

            Compositions = new List<Composition>();
            foreach (var jComposition in jCompositions)
            {
                Composition received = new Composition();
                var fields = jComposition.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.CompositionID:
                            Table.SetProperty(received, Key.CompositionID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.CompositionName:
                            Table.SetProperty(received, Key.CompositionName, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.ArtistID:
                            Table.SetProperty(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.AlbumID:
                            Table.SetProperty(received, Key.AlbumID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.Duration:
                            Table.SetProperty(received, Key.Duration, kv.GetIntegerValueOrReturnNull());
                            break;
                        case Key.FilePath:
                            Table.SetProperty(received, Key.FilePath, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.Lyrics:
                            Table.SetProperty(received, Key.Lyrics, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.About:
                            Table.SetProperty(received, Key.About, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                if (Artists.Count == 0)
                    GetArtists();
                received.Artist = Table.GetLinkedEntity(received.ArtistID, Artists, "ArtistID");
                Compositions.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Compositions)] = Table.GetTableSize(Path.Combine(FolderName, "Compositions.json")));

            return Task.Factory.StartNew(() => Compositions.Skip(skip).Take(take).ToList());
        }

        public async Task<List<IComposition>> GetICompositionsAsync(int skip, int take)
        {
#if !NET40
            var result =
                (await GetCompositionsAsync(skip, take));
            return 
                result
                .Select(c => c as IComposition).ToList();
#else
    throw new NotImplementedException();
#endif
        }

        public void Add(Style style)
        {
            throw new NotImplementedException();
        }
    }
}
