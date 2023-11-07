using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System.Collections.Generic;
using System.IO;

namespace MediaStreamer.DataAccess.CrossPlatform.Extensions
{
    public static class AlbumsExtensions
    {
        public static LinkedList<JKeyValuePair> GetPropList(this Album album)
        {
            var result = new LinkedList<JKeyValuePair>();
            //Properties
            result.AddLast(new JKeyValuePair(Key.AlbumID.ToJString(), album.GetMediaEntityIdValue() ));
            result.AddLast(new JKeyValuePair(Key.AlbumName, DataBase.Coalesce(album.AlbumName)));
            result.AddLast(new JKeyValuePair(Key.ArtistID.ToJString(), DataBase.Coalesce(album.ArtistID).ToJString()));
            result.AddLast(new JKeyValuePair(Key.GenreID.ToJString(), DataBase.Coalesce(album.GenreID).ToJString()));
            result.AddLast(new JKeyValuePair(Key.Year, DataBase.Coalesce(album.Year)));
            result.AddLast(new JKeyValuePair(Key.Type, DataBase.Coalesce(album.Type)));
            result.AddLast(new JKeyValuePair(Key.Label, DataBase.Coalesce(album.Label)));
            return result;
        }

        public static void SaveToFile(this IEnumerable<Album> set, string folderPath)
        {
            Table lcTable = CrossTable.Load(folderPath, "Albums");
            foreach (var lc in set)
            {
                CrossTable.AddNewObjectToCollection(lc.GetPropList(), lcTable.Items);
            }
            lcTable.Root.ToFile(lcTable.FilePath);
        }

        public static void EnsureCreated(string folderPath)
        {
            if (!File.Exists(Path.Combine(folderPath, "Albums.json")))
            {
                Table lcTable = CrossTable.Load(folderPath, "Albums");
                lcTable.Root.ToFile(lcTable.FilePath);
            }
        }
    }
}
