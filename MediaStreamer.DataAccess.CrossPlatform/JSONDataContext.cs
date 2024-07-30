using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using Sealkeen.Linq.Extensions;
using System.Collections.Concurrent;
using MediaStreamer.Domain.Models;
using MediaStreamer.DataAccess.CrossPlatform.Extensions;
using System.Reflection.Emit;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class JSONDataContext : IPagedDMDBContext
    {
        public JSONDataContext() : this (null) {  }
        public JSONDataContext(Action<string> log = null)
        {
            if (log != null) _log = log;
            else _log = MediaStreamer.Logging.Extensions.LogInConsoleAndDebug;
            FolderName = PathResolver.GetStandardDatabasePath();
            TableInfo = new ConcurrentDictionary<string, DateTime>();

            Genres = new List<Genre>();
            Artists = new List<Artist>();
            Albums = new List<Album>();
            Compositions = new List<Composition>();
            ArtistGenres = new List<ArtistGenre>();
            AlbumGenres = new List<AlbumGenre>();
            ListenedCompositions = new List<ListenedComposition>();
            SaveDelayed = false;
        }
        private Action<string> _log;
        
        public bool SaveDelayed { get; set; }

        public JSONDataContext(Action<string> log = null, string dbPath = null) : this(log)
        {
            FolderName = dbPath;
        }

        public virtual ConcurrentDictionary<string, DateTime> TableInfo { get; set; }
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
                TableInfo[pair.Key] = CrossTable.GetTableUpdateTime(pair.Value);
            }
        }

        public void Add(Administrator administrator)
        {
            throw new NotImplementedException();
        }

        public void Add(Album album)
        {
            if (SaveDelayed) { 
                Albums.Add(album); return;
            }
            if (Albums.Where(c =>
                    c.AlbumName == album.AlbumName &&
                    c.ArtistID == album.ArtistID).Any())
                return;

            var alTable = CrossTable.Load(FolderName, "Albums");
            CrossTable.AddNewObjectToCollection(album.GetPropList(), alTable.Items);
            alTable.Root.ToFile(Path.Combine(FolderName, "Albums.json"));
        }

        public void Add(AlbumGenre albumGenre)
        {
            if (SaveDelayed) {
                AlbumGenres.Add(albumGenre);
                return;
            }
            if (AlbumGenres.Where(c =>
                    c.AlbumID == albumGenre.AlbumID &&
                    c.GenreID == albumGenre.GenreID).Any())
                return;

            var agTable = CrossTable.Load(FolderName, "AlbumGenres");
            CrossTable.AddNewObjectToCollection(albumGenre.GetPropList(), agTable.Items);
            agTable.Root.ToFile(agTable.FilePath);
        }

        public void Add(Artist artist)
        {
            if (SaveDelayed) {
                Artists.Add(artist); return;
            }
            if (Artists.Where(
                    c => c.ArtistName == artist.ArtistName).Count() != 0)
                return;

            var arTable = CrossTable.Load(FolderName, "Artists");
            CrossTable.AddNewObjectToCollection(artist.GetPropList(), arTable.Items);
            arTable.Root.ToFile(arTable.FilePath);
        }

        public void Add(ArtistGenre artistGenre)
        {
            if (SaveDelayed) {
                ArtistGenres.Add(artistGenre); return;
            }
            if (ArtistGenres.Where(
                    c => c.ArtistID == artistGenre.ArtistID &&
                    c.GenreID == artistGenre.GenreID
                    ).Count() != 0)
                return;

            var agTable = CrossTable.Load(FolderName, "ArtistGenres");
            CrossTable.AddNewObjectToCollection(artistGenre.GetPropList(), agTable.Items);
            agTable.Root.ToFile(agTable.FilePath);
        }

        public void Add(Composition composition)
        {
            if (SaveDelayed) {
                Compositions.Add(composition); return;    
            }
            if (Compositions.Where(
                    c =>
                    c.CompositionName == composition.CompositionName &&
                    c.FilePath == composition.FilePath).Count() != 0)
                return;

            var cTable = CrossTable.Load(FolderName, "Compositions");
            CrossTable.AddNewObjectToCollection(composition.GetPropList(), cTable.Items);
            cTable.Root.ToFile(cTable.FilePath);
        }

        public void Add(CompositionVideo compositionVideo)
        {
            throw new NotImplementedException();
        }

        public void Add(Genre genre)
        {
            if (SaveDelayed) {
                Genres.Add(genre); return;
            }
            if (Genres.Where(
                    c => c.GenreName == genre.GenreName).Count() != 0)
                return;

            var genresDB = CrossTable.Load(FolderName, "Genres");
            CrossTable.AddNewObjectToCollection(genre.GetPropList(), genresDB.Items);
            genresDB.Root.ToFile(genresDB.FilePath);
        }

        public void Add(ListenedComposition listenedComposition)
        {
            if (SaveDelayed) {
                ListenedCompositions.Add(listenedComposition); return;
            }
            var listenedDB = CrossTable.Load(FolderName, "ListenedCompositions");

            CrossTable.AddNewObjectToCollection(listenedComposition.GetPropList(), listenedDB.Items);
            listenedDB.Root.ToFile(listenedDB.FilePath);
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

        public void AddEntity<T>(T entity) where T : MediaEntity
        {
            if (typeof(T) == typeof(Composition)) {
                //var id = DataBase.GetMaxID<Composition, Guid>(Compositions.AsQueryable(), "CompositionID") + 1;
                //id++;
                //Property.SetProperty(entity, "CompositionID", id);
                Add(entity as Composition);
            } else if (typeof(T) == typeof(Artist)) {
                //var id = DataBase.GetMaxID<Artist, Guid>(Artists.AsQueryable(), "ArtistID");
                //id++;
                //Property.SetProperty(entity, "ArtistID", id);
                Add(entity as Artist);
            } else if (typeof(T) == typeof(Album)) {
                //var id = DataBase.GetMaxID<Album, Guid>(Albums.AsQueryable(), "AlbumID");
                //id++;
                //Property.SetProperty(entity, "AlbumID", id);
                Add(entity as Album);
            } else if (typeof(T) == typeof(Genre)) {
                //var id = DataBase.GetMaxID<Genre, Guid>(Genres.AsQueryable(), "GenreID");
                //id++;
                //Property.SetProperty(entity, "GenreID", id);
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
            GenresExtensions.EnsureCreated(FolderName);
            ArtistsExtensions.EnsureCreated(FolderName);
            AlbumsExtensions.EnsureCreated(FolderName);
            CompositionsExtensions.EnsureCreated(FolderName);
            ArtistGenresExtensions.EnsureCreated(FolderName);
            AlbumGenresExtensions.EnsureCreated(FolderName);
            ListenedCompositionsExtensions.EnsureCreated(FolderName);

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
            if (!CrossTable.UpdateOccured(TableInfo, FolderName, nameof(AlbumGenres)))
                return AlbumGenres.AsQueryable();

            var jAlbums = CrossTable.LoadAllEntities(FolderName, "AlbumGenres.json");

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
                            Reflection.MapValue(received, Key.AlbumID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.GenreID:
                            Reflection.MapValue(received, Key.GenreID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                    }
                }
                AlbumGenres.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(AlbumGenres)] = CrossTable.GetTableUpdateTime(Path.Combine(FolderName, "AlbumGenres.json")));
            return AlbumGenres.AsQueryable();
        }

        public IQueryable<Album> GetAlbums()
        {
            if (!CrossTable.UpdateOccured(TableInfo, FolderName, nameof(Albums)))
                return Albums.AsQueryable();

            var jAlbums = CrossTable.LoadAllEntities(FolderName, "Albums.json");

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
                            Reflection.MapValue(received, Key.AlbumID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.AlbumName:
                            Reflection.MapValue(received, Key.AlbumName, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.ArtistID:
                            Reflection.MapValue(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.GenreID:
                            Reflection.MapValue(received, Key.GenreID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.Year:
                            Reflection.MapValue(received, Key.Year, kv.GetIntegerValueOrReturnNull());
                            break;
                        case Key.Type:
                            Reflection.MapValue(received, Key.Type, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.Label:
                            Reflection.MapValue(received, Key.Label, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                if (Artists.Count == 0)
                    GetArtists();
                received.Artist = CrossTable.GetLinkedEntity(received.ArtistID, Artists, "ArtistID");
                Albums.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Albums)] = CrossTable.GetTableUpdateTime(Path.Combine(FolderName, "Albums.json")));
            return Albums.AsQueryable();
        }

        public IQueryable<ArtistGenre> GetArtistGenres()
        {
            if (!CrossTable.UpdateOccured(TableInfo, FolderName, nameof(ArtistGenres)))
                return ArtistGenres.AsQueryable();

            var jArtists = CrossTable.LoadAllEntities(FolderName, "ArtistGenres.json");

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
                            Reflection.MapValue(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.GenreID:
                            Reflection.MapValue(received, Key.GenreID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                    }
                }
                ArtistGenres.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(ArtistGenres)] = CrossTable.GetTableUpdateTime(Path.Combine(FolderName, "ArtistGenres.json")));
            return ArtistGenres.AsQueryable();
        }

        public IQueryable<Artist> GetArtists()
        {
            if (!CrossTable.UpdateOccured(TableInfo, FolderName, nameof(Artists)))
                return Artists.AsQueryable();

            var jArtists = CrossTable.LoadAllEntities(FolderName, "Artists.json");

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
                            Reflection.MapValue(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.ArtistName:
                            Reflection.MapValue(received, Key.ArtistName, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                Artists.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Artists)] = CrossTable.GetTableUpdateTime(Path.Combine(FolderName, "Artists.json")));
            return Artists.AsQueryable();
        }

        public IQueryable<Composition> GetCompositions()
        {
            if (!CrossTable.UpdateOccured(TableInfo, FolderName, nameof(Compositions)))
                return Compositions.AsQueryable();

            var jCompositions = CrossTable.LoadAllEntities(FolderName, "Compositions.json");

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
                            Reflection.MapValue(received, Key.CompositionID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.CompositionName:
                            Reflection.MapValue(received, Key.CompositionName, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.ArtistID:
                            Reflection.MapValue(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.AlbumID:
                            Reflection.MapValue(received, Key.AlbumID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.Duration:
                            Reflection.MapValue(received, Key.Duration, kv.GetIntegerValueOrReturnNull());
                            break;
                        case Key.FilePath:
                            Reflection.MapValue(received, Key.FilePath, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.Lyrics:
                            Reflection.MapValue(received, Key.Lyrics, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.About:
                            Reflection.MapValue(received, Key.About, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                if (Artists.Count == 0)
                    GetArtists();
                received.Artist = CrossTable.GetLinkedEntity(received.ArtistID, Artists, "ArtistID");
                Compositions.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Compositions)] = 
                CrossTable.GetTableUpdateTime(Path.Combine(FolderName, "Compositions.json")));
            return Compositions.AsQueryable();
        }
        public async Task<List<Composition>> GetCompositionsAsync()
        { 
            return GetCompositions().ToList();
        }
        public async Task<List<IComposition>> GetICompositionsAsync()
        { 
            return GetICompositions().ToList();
        }

        public IQueryable<CompositionVideo> GetCompositionVideos()
        {
            return new List<CompositionVideo>().AsQueryable();
        }

        public IQueryable<Genre> GetGenres()
        {
            if (!CrossTable.UpdateOccured(TableInfo, FolderName, nameof(Genres)))
                return Genres.AsQueryable();

            var jGenres = CrossTable.LoadAllEntities(FolderName, "Genres.json");

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
                            Reflection.MapValue(received, Key.GenreID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.GenreName:
                            Reflection.MapValue(received, Key.GenreName, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                Genres.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Genres)] = CrossTable.GetTableUpdateTime(Path.Combine(FolderName, "Genres.json")));
            return Genres.AsQueryable();
        }

        public IQueryable<IComposition> GetICompositions()
        {
            return GetCompositions().AsQueryable();//.Include(c => c.Artist); 
        }

        public IQueryable<ListenedComposition> GetListenedCompositions()
        {
            if (!CrossTable.UpdateOccured(TableInfo, FolderName, nameof(ListenedCompositions)))
                return ListenedCompositions.AsQueryable();

            var jCompositions = CrossTable.LoadAllEntities(FolderName, "ListenedCompositions.json");

            ListenedCompositions = new List<ListenedComposition>();
            foreach (var jComposition in jCompositions)
            {
                ListenedComposition received = new ListenedComposition();
                var fields = jComposition.DescendantPairs();
                foreach (var kv in fields)
                {
                    var key = kv.Key.ToString().Trim('\"');
                    var value = kv.Value.AsUnquoted();
                    switch (key)
                    {
                        case Key.ListenDate:
                            Reflection.MapValue(received, Key.ListenDate,
                                DateTime.ParseExact(kv.Value.AsUnquoted().ToString(), "dd.MM.yyyy H:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                            break;
                        case Key.CompositionID:
                            Guid compositionIDValue;
                            if (Guid.TryParse(value, out compositionIDValue)) {
                                Reflection.MapValue(received, Key.CompositionID, compositionIDValue);
                            } else {
                                // TODO: handle the case where the value could not be parsed as a Guid
                            }
                            break;
                        case Key.UserID:
                            Guid userIDValue;
                            if (Guid.TryParse(value, out userIDValue)) {
                                Reflection.MapValue(received, Key.UserID, userIDValue);
                            } else {
                                // TODO: handle the case where the value could not be parsed as a Guid
                            }
                            break;
                        case Key.StoppedAt:
                            double stoppedAtValue;
                            if (double.TryParse(value, out stoppedAtValue)) {
                                Reflection.MapValue(received, Key.StoppedAt, stoppedAtValue);
                            } else {
                                // TODO: handle the case where the value could not be parsed as a double
                            }
                            break;
                    }
                }
                ListenedCompositions.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(ListenedCompositions)] = CrossTable.GetTableUpdateTime(Path.Combine(FolderName, "ListenedCompositions.json")));
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

        public void RemoveEntity<T>(T o, bool saveDelayed) where T : MediaEntity
        {
            var tp = o.GetType();
            if (tp == typeof(Composition))
            {
                if (saveDelayed)
                    Compositions.Remove(o as Composition);
                else
                    RemoveComposition(o);
            }
            else if (tp == typeof(Album))
            {
                if (saveDelayed)
                    Albums.Remove(o as Album);
                else
                    RemoveAlbum(o);
            }
            else if (tp == typeof(Artist))
            {
                if (saveDelayed)
                    Artists.Remove(o as Artist);
                else
                    RemoveArtist(o);
            }
            else if (tp == typeof(ArtistGenre))
            {
                if (saveDelayed)
                    ArtistGenres.Remove(o as ArtistGenre);
                else
                    RemoveArtistGenre(o);
            }
        }
        public void RemoveRange<T>(T o) where T : IEnumerable<MediaEntity>
        {
            var tp = o.GetType();
            throw new NotImplementedException();
        }

        private void RemoveArtist<T>(T o) where T : MediaEntity
        {
            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Artists.json");
            JItem itemsCollection = (root == null) ? new JArray(root)
            : root.FindPairByKey("Artists".ToJString()).GetPairedValue(); // Get array from JSON
            List<JKeyValuePair> lst = new List<JKeyValuePair> {
                new JKeyValuePair(Key.AlbumID.ToJString(), new JString(o.GetId()))
            };
            var pair = itemsCollection.HasThesePairsRecursive(lst);
            TryRemoveNodeFromArrayAndSaveToFile(Path.Combine(FolderName, "Artists.json"), pair, root);
        }

        private void RemoveArtistGenre<T>(T o) where T : MediaEntity
        {
            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "ArtistsGenres.json");
            JItem itemsCollection = (root == null) ? new JArray(root)
            : root.FindPairByKey("ArtistsGenres".ToJString()).GetPairedValue(); // Get array from JSON
            List<JKeyValuePair> lst = new List<JKeyValuePair> {
                new JKeyValuePair(Key.AlbumID.ToJString(), new JString(o.GetId()))
            };
            var pair = itemsCollection.HasThesePairsRecursive(lst);
            TryRemoveNodeFromArrayAndSaveToFile(Path.Combine(FolderName, "ArtistsGenres.json"), pair, root);
        }

        private void RemoveAlbum<T>(T o) where T : MediaEntity
        {
            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Albums.json");
            JItem itemsCollection = (root == null) ? new JArray(root)
            : root.FindPairByKey("Albums".ToJString()).GetPairedValue(); // Get array from JSON
            List<JKeyValuePair> lst = new List<JKeyValuePair> {
                new JKeyValuePair(Key.AlbumID.ToJString(), new JString(o.GetId()))
            };
            var pair = itemsCollection.HasThesePairsRecursive(lst);
            TryRemoveNodeFromArrayAndSaveToFile(Path.Combine(FolderName, "Albums.json"), pair, root);
        }

        private void TryRemoveNodeFromArrayAndSaveToFile(string tableName, JItem pair, JItem root) {
            if (pair != null && root != null) {
                pair.Parent.Parent.Items.Remove(pair.Parent);
                root.ToFile(tableName);
            }
        }

        private void RemoveComposition<T>(T o) where T : class
        {
            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Compositions.json");
            JItem itemsCollection = (root == null) ? new JArray(root)
            : root.FindPairByKey("Compositions".ToJString()).GetPairedValue();
            List<JKeyValuePair> lst = new List<JKeyValuePair> {
                    new JKeyValuePair(Key.CompositionID.ToJString(), new JSingleValue((o as Composition).CompositionID.ToString()))
                };
            var pair = itemsCollection.HasThesePairsRecursive(lst);
            TryRemoveNodeFromArrayAndSaveToFile(Path.Combine(FolderName, "Compositions.json"), pair, root);
        }

        public int SaveChanges()
        {
            if (SaveDelayed)
            {
                AlbumsExtensions.SaveToFile(Albums, FolderName);
                GenresExtensions.SaveToFile(Genres, FolderName);
                ArtistsExtensions.SaveToFile(Artists, FolderName);
                CompositionsExtensions.SaveToFile(Compositions, FolderName);
                ArtistGenresExtensions.SaveToFile(ArtistGenres, FolderName);
                AlbumGenresExtensions.SaveToFile(AlbumGenres, FolderName);
                ListenedCompositionsExtensions.SaveToFile(ListenedCompositions, FolderName);
            }

            return 1;
        }

        public int SaveChangesTo(string tableName)
        {
            switch (tableName) {
                case "Albums":
                    AlbumsExtensions.SaveToFile(Albums, FolderName);
                break;
                case "Genres":
                    GenresExtensions.SaveToFile(Genres, FolderName);
                break;
                case "Artists":
                    ArtistsExtensions.SaveToFile(Artists, FolderName);
                break;
                case "Compositions":
                    CompositionsExtensions.SaveToFile(Compositions, FolderName);
                break;
                case "ArtistGenres":
                    ArtistGenresExtensions.SaveToFile(ArtistGenres, FolderName);
                break;
                case "AlbumGenres":
                    AlbumGenresExtensions.SaveToFile(AlbumGenres, FolderName);
                break;
                case "ListenedCompositions":
                    ListenedCompositionsExtensions.SaveToFile(ListenedCompositions, FolderName);
                break;
            }

            return 1;
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
            if (!CrossTable.UpdateOccured(TableInfo, FolderName, nameof(Compositions)))
                return Task.Factory.StartNew(() => Compositions.Skip(skip).Take(take).ToList());

            var jCompositions = CrossTable.LoadAllEntities(FolderName, "Compositions.json");

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
                            Reflection.MapValue(received, Key.CompositionID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.CompositionName:
                            Reflection.MapValue(received, Key.CompositionName, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.ArtistID:
                            Reflection.MapValue(received, Key.ArtistID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.AlbumID:
                            Reflection.MapValue(received, Key.AlbumID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.Duration:
                            Reflection.MapValue(received, Key.Duration, kv.GetIntegerValueOrReturnNull());
                            break;
                        case Key.FilePath:
                            Reflection.MapValue(received, Key.FilePath, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.Lyrics:
                            Reflection.MapValue(received, Key.Lyrics, kv.GetPairedValue().AsUnquoted());
                            break;
                        case Key.About:
                            Reflection.MapValue(received, Key.About, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                if (Artists.Count == 0)
                    GetArtists();
                received.Artist = CrossTable.GetLinkedEntity(received.ArtistID, Artists, "ArtistID");
                Compositions.Add(received);
            }

            Task.Factory.StartNew(() => TableInfo[nameof(Compositions)] = CrossTable.GetTableUpdateTime(Path.Combine(FolderName, "Compositions.json")));

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

        public IQueryable<ListenedComposition> GetListenedCompositions(bool includeCompositions)
        {
            if (CrossTable.UpdateOccured(TableInfo, FolderName, nameof(ListenedCompositions)))
                ListenedCompositions = (List<ListenedComposition>)GetListenedCompositions();

            var lComps = GetListenedCompositions();
            if ( Compositions.Any() && lComps.Any() ) {
                foreach (var lComp in lComps)
                {
                    lComp.Composition = Compositions
                        .FirstOrDefault(c => c.CompositionID == lComp.CompositionID);
                }
            }
            return lComps;
        }
    }
}
