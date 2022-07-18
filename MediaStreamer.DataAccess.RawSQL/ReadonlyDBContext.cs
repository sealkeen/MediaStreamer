using MediaStreamer.Domain;
using Sealkeen.Abstractions;
using StringExtensions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using LinqExtensions;

namespace MediaStreamer.DataAccess.RawSQL
{
    public class ReadonlyDBContext : IDMDBContext
    {
        private readonly ILogger _logger;

        public ReadonlyDBContext(string filename, ILogger logger)
        {
            Filename = filename;
            this._logger = logger;
            _logger?.LogTrace(nameof(ReadonlyDBContext) + " constructed.");
            ConString = $"DataSource={Filename}";
            
            Genres = new List<Genre>();
            Artists = new List<Artist>();
            Albums = new List<Album>();
            Compositions = new List<Composition>();
        }

        public virtual List<Genre> Genres { get; set; }
        public virtual List<Album> Albums { get; set; }
        public virtual List<Artist> Artists { get; set; }
        public virtual List<Composition> Compositions { get; set; }

        public virtual System.Data.IDbCommand DbCommand { get; set; }

        public string Filename { get; }
        public string ConString { get; }

        public void Add(Administrator administrator)
        {
            
        }

        public void Add(Album administrator)
        {
            
        }

        public void Add(AlbumGenre albumGenre)
        {
            
        }

        public void Add(Artist artist)
        {
            
        }

        public void Add(ArtistGenre artistGenre)
        {
            
        }

        public void Add(Composition composition)
        {
            
        }

        public void Add(CompositionVideo compositionVideo)
        {
            
        }

        public void Add(Genre genre)
        {
            
        }

        public void Add(ListenedComposition listenedComposition)
        {
            
        }

        public void Add(Moderator moderator)
        {
            
        }

        public void Add(Picture picture)
        {
            
        }

        public void Add(User user)
        {
            
        }

        public void Add(Video video)
        {
            
        }

        public void AddEntity<T>(T o) where T : class
        {
            
        }

        public void Clear()
        {
            
        }

        public bool ClearTable(string tableName)
        {
            return false;
        }

        public void DisableLazyLoading()
        {
            
        }

        public void EnsureCreated()
        {
            
        }

        public IQueryable<Administrator> GetAdministrators()
        {
            return new List<Administrator>().AsQueryable();
        }

        public IQueryable<AlbumGenre> GetAlbumGenres()
        {
            return new List<AlbumGenre>().AsQueryable();
        }

        public IQueryable<Album> GetAlbums()
        {
            return Albums.AsQueryable();
        }

        public IQueryable<ArtistGenre> GetArtistGenres()
        {
            return new List<ArtistGenre>().AsQueryable();
        }

        public IQueryable<Artist> GetArtists()
        {
            return Artists.AsQueryable();
        }

        public void TryToReplaceOldPaths()
        {

            SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(ConString);
            SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand("SELECT REPLACE([strUrl], '/myoldurl/', '/mynewurl/') FROM [UrlRewrite]"
                );
            cmd.ExecuteNonQuery();
        }

        public IQueryable<Composition> GetCompositions()
        {
            //TryToReplaceOldPaths();

            SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(ConString);
            SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(
                "select * from Composition c " + 
                "left join Artist art on c.ArtistID = art.ArtistID " +
                "left join Album  alb on c.AlbumID = alb.AlbumID"
                );
            cmd.Connection = conn;

            conn.Open();
            cmd.ExecuteScalar();
            SQLiteDataReader dr = cmd.ExecuteReader();
            Composition cmp = new Composition();
            Dictionary<long, Guid> artIDs = new Dictionary<long, Guid>();
            Dictionary<long, Guid> albIDs = new Dictionary<long, Guid>();
            Dictionary<string, Guid> genIDs = new Dictionary<string, Guid>();

            Compositions.Clear();
            Albums.Clear();
            Artists.Clear();

            while (dr.Read())
            {
                cmp = new Composition();
                cmp.CompositionID = Guid.NewGuid();
                cmp.CompositionName = dr["CompositionName"] as string;
                cmp.FilePath = dr["FilePath"] as string;
                try
                {
                    Guid albumID = GetIDOrReturnNew(albIDs, (dr["AlbumID"] as long?).Value);
                    Guid artistID = GetIDOrReturnNew(artIDs, (dr["ArtistID"] as long?).Value);
                    //Guid genID = GetIDOrReturnNew<string>(genIDs, dr["GenreName"] as string);
                    string artistName = dr["ArtistName"] as string;
                    string albumName = dr["AlbumName"] as string;
                    //string genreName = dr["GenreName"] as string;

                    //Genre gen = new Genre() { GenreID = albumID, GenreName = albumName };
                    Artist art = new Artist() { ArtistID = artistID, ArtistName = artistName };
                    Album alb = new Album() { AlbumID = albumID, AlbumName = albumName };
                    Artists.Add(art);
                    Albums.Add(alb);
                    //Genres.Add(gen);
                    cmp.AlbumID = alb.AlbumID;
                    cmp.ArtistID = art.ArtistID;
                    cmp.Artist = art;
                    cmp.Album = alb;
                }
                catch { 
                
                }

                Compositions.Add(cmp);
            }


            return Compositions.AsQueryable();
        }

        public async Task<List<Composition>> GetCompositionsAsync()
        {
            return await GetCompositions().CreateListAsync();
        }

        public async Task<List<IComposition>> GetICompositionsAsync()
        {
            return await GetICompositions().CreateListAsync();
        }


        public Guid GetIDOrReturnNew<T>(Dictionary<T, Guid> dict, T key)
        {
            if (dict.Keys.Contains(key))
            {
                return dict.Where(k => k.Key.Equals(key)).First().Value;
            }
            var guid = Guid.NewGuid();
            dict.Add(key, guid);
            return guid;
        }

        public IQueryable<CompositionVideo> GetCompositionVideos()
        {
            return new List<CompositionVideo>().AsQueryable();
        }

        public string GetContainingFolderPath()
        {
            return Filename.GetDirectoryOf();
        }

        public IQueryable<Genre> GetGenres()
        {
            return Genres.AsQueryable();
        }

        public IQueryable<IComposition> GetICompositions()
        {
            if (Compositions == null || Compositions.Count() <= 0)
                GetCompositions();
            return Compositions.AsQueryable();
        }

        public IQueryable<ListenedComposition> GetListenedCompositions()
        {
            return new List<ListenedComposition>().AsQueryable();
        }

        public IQueryable<Moderator> GetModerators()
        {
            return new List<Moderator>().AsQueryable();
        }

        public IQueryable<Picture> GetPictures()
        {
            return new List<Picture>().AsQueryable();
        }

        public IQueryable<User> GetUsers()
        {
            return new List<User>().AsQueryable();
        }

        public IQueryable<Video> GetVideos()
        {
            return new List<Video>().AsQueryable();
        }

        public void RemoveEntity<T>(T o) where T : class
        {
            
        }

        public void UpdateAndSaveChanges<TEntity>(TEntity entity) where TEntity : class
        {
            
        }

        public int SaveChanges()
        {
            return 0;
        }

        public void Dispose()
        {
            
        }

        private bool TableExists(string table)
        {
            string command = $"select * from sys.tables";
            using (SQLiteConnection con = new SQLiteConnection(Filename))
            using (SQLiteCommand com = new SQLiteCommand(command, con))
            {
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(0).ToLower() == table.ToLower())
                        return true;
                }
                reader.Close();
            }
            return false;
        }
    }
}
