using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System.Collections.Generic;
using System.IO;

namespace MediaStreamer.DataAccess.CrossPlatform.Extensions
{
    public static class GenresExtensions
    {
        public static LinkedList<JKeyValuePair> GetPropList(this Genre genre)
        {
            var result = new LinkedList<JKeyValuePair>();
            result.AddLast(new JKeyValuePair(Key.GenreID.ToJString(), genre.GetMediaEntityIdValue()));
            result.AddLast(new JKeyValuePair(Key.GenreName, DataBase.Coalesce(genre.GenreName)));
            return result;
        }

        public static void SaveToFile(this IEnumerable<Genre> set, string folderPath)
        {
            var path = Path.Combine(folderPath, "Genres.json");
            var movePath = path + $"{System.DateTime.Today.ToString("yyyy-MM-dd__HH.mm.ss")}";
            if (File.Exists(path)) {
                File.Move(path, movePath);
            }
            Table lcTable = CrossTable.Load(folderPath, "Genres");
            foreach (var lc in set)
            {
                CrossTable.AddNewObjectToCollection(lc.GetPropList(), lcTable.Items);
            }
            lcTable.Root.ToFile(lcTable.FilePath);
            File.Delete(movePath);
        }

        public static void EnsureCreated(string folderPath)
        {
            if (!File.Exists(Path.Combine(folderPath, "Genres.json")))
            {
                Table lcTable = CrossTable.Load(folderPath, "Genres");
                lcTable.Root.ToFile(lcTable.FilePath);
            }
        }
    }
}
