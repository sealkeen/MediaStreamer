using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System.Collections.Generic;
using System.IO;

namespace MediaStreamer.DataAccess.CrossPlatform.Extensions
{
    public static class ArtistsExtensions
    {
        public static LinkedList<JKeyValuePair> GetPropList(this Artist artist)
        {
            var result = new LinkedList<JKeyValuePair>();
            
            //Properties
            result.AddLast(new JKeyValuePair(Key.ArtistID.ToJString(), artist.GetMediaEntityIdValue() ));
            result.AddLast(new JKeyValuePair(Key.ArtistName.ToJString(), DataBase.Coalesce(artist.ArtistName).ToJString() ));
            //Properties
            return result;
        }

        public static void SaveToFile(this IEnumerable<Artist> set, string folderPath)
        {
            Table lcTable = CrossTable.Load(folderPath, "Artists");
            foreach (var lc in set)
            {
                CrossTable.AddNewObjectToCollection(lc.GetPropList(), lcTable.Items);
            }
            lcTable.Root.ToFile(lcTable.FilePath);
        }

        public static void EnsureCreated(string folderPath)
        {
            if (!File.Exists(Path.Combine(folderPath, "Artists.json")))
            {
                Table lcTable = CrossTable.Load(folderPath, "Artists");
                lcTable.Root.ToFile(lcTable.FilePath);
            }
        }
    }
}
