using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System.Collections.Generic;
using System.IO;

namespace MediaStreamer.DataAccess.CrossPlatform.Extensions
{
    public static class ArtistGenresExtensions
    {
        public static LinkedList<JKeyValuePair> GetPropList(this ArtistGenre artistGenre)
        {
            var result = new LinkedList<JKeyValuePair>();
            //Properties
            result.AddLast(new JKeyValuePair(Key.ArtistID.ToJString(), DataBase.Coalesce(artistGenre.ArtistID).ToSingleValue()));
            result.AddLast(new JKeyValuePair(Key.GenreID.ToJString(), DataBase.Coalesce(artistGenre.GenreID).ToSingleValue()));
            return result;
        }

        public static void SaveToFile(this IEnumerable<ArtistGenre> set, string folderPath)
        {
            Table lcTable = CrossTable.Load(folderPath, "ArtistGenres");
            foreach (var lc in set)
            {
                CrossTable.AddNewObjectToCollection(lc.GetPropList(), lcTable.Items);
            }
            lcTable.Root.ToFile(lcTable.FilePath);
        }

        public static void EnsureCreated(string folderPath)
        {
            if (!File.Exists(Path.Combine(folderPath, "ArtistGenres.json")))
            {
                Table lcTable = CrossTable.Load(folderPath, "ArtistGenres");
                lcTable.Root.ToFile(lcTable.FilePath);
            }
        }
    }
}
