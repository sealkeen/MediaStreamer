using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System.Collections.Generic;
using System.IO;

namespace MediaStreamer.DataAccess.CrossPlatform.Extensions
{
    public static class AlbumGenresExtensions
    {
        public static LinkedList<JKeyValuePair> GetPropList(this AlbumGenre albumGenre)
        {
            var result = new LinkedList<JKeyValuePair>();
            //Properties
            result.AddLast(new JKeyValuePair(Key.AlbumID.ToJString(), DataBase.Coalesce(albumGenre.AlbumID).ToSingleValue()));
            result.AddLast(new JKeyValuePair(Key.GenreID.ToJString(), DataBase.Coalesce(albumGenre.GenreID).ToSingleValue()));
            return result;
        }

        public static void SaveToFile(this IEnumerable<AlbumGenre> set, string folderPath)
        {
            Table lcTable = CrossTable.Load(folderPath, "AlbumGenres");
            foreach (var lc in set)
            {
                CrossTable.AddNewObjectToCollection(lc.GetPropList(), lcTable.Items);
            }
            lcTable.Root.ToFile(lcTable.FilePath);
        }

        public static void EnsureCreated(string folderPath)
        {
            if (!File.Exists(Path.Combine(folderPath, "AlbumGenres.json")))
            {
                Table lcTable = CrossTable.Load(folderPath, "AlbumGenres");
                lcTable.Root.ToFile(lcTable.FilePath);
            }
        }
    }
}
