using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace MediaStreamer.DataAccess.CrossPlatform.Extensions
{
    public static class ListenedCompositionsExtensions
    {
        public static LinkedList<JKeyValuePair> GetPropList(this ListenedComposition listenedComposition)
        {
            var result = new LinkedList<JKeyValuePair>();
            result.AddLast(new JKeyValuePair(Key.ListenDate.ToString(), listenedComposition.ListenDate.ToString("dd.MM.yyyy H:mm:ss")));
            result.AddLast(new JKeyValuePair(Key.CompositionID.ToString(), listenedComposition.CompositionID.ToString()));
            result.AddLast(new JKeyValuePair(Key.UserID.ToString(), listenedComposition.UserID.ToString()));
            result.AddLast(new JKeyValuePair(Key.StoppedAt.ToString(), listenedComposition.StoppedAt.ToString()));
            return result;
        }

        public static void SaveToFile(this IEnumerable<ListenedComposition> set, string folderPath)
        {
            Table lcTable = CrossTable.Load(folderPath, "ListenedCompositions");
            foreach (var lc in set)
            {
                CrossTable.AddNewObjectToCollection(lc.GetPropList(), lcTable.Items);
            }
            lcTable.Root.ToFile(lcTable.FilePath);
        }

        public static void EnsureCreated(string folderPath)
        {
            if (!File.Exists(Path.Combine(folderPath, "ListenedCompositions.json")))
            {
                Table lcTable = CrossTable.Load(folderPath, "ListenedCompositions");
                lcTable.Root.ToFile(lcTable.FilePath);
            }
        }
    }
}
