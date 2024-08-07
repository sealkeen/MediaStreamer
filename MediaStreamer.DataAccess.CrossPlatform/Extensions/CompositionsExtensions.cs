﻿using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System.Collections.Generic;
using System.IO;

namespace MediaStreamer.DataAccess.CrossPlatform.Extensions
{
    public static class CompositionsExtensions
    {
        public static LinkedList<JKeyValuePair> GetPropList(this Composition composition)
        {
            var result = new LinkedList<JKeyValuePair>();
            result.AddLast( new JKeyValuePair(Key.CompositionID.ToJString(), composition.GetMediaEntityIdValue()) );
            result.AddLast(new JKeyValuePair(Key.CompositionName, DataBase.Coalesce(composition.CompositionName)));
            result.AddLast(new JKeyValuePair(Key.ArtistID.ToJString(), DataBase.Coalesce(composition.ArtistID).ToJString() ));
            result.AddLast(new JKeyValuePair(Key.AlbumID.ToJString(), DataBase.Coalesce(composition.AlbumID).ToJString()));
            result.AddLast(new JKeyValuePair(Key.Duration.ToJString(), DataBase.Coalesce(composition.Duration).ToSingleValue()));
            result.AddLast(new JKeyValuePair(Key.FilePath, DataBase.Coalesce(composition.FilePath)));
            result.AddLast(new JKeyValuePair(Key.Lyrics, DataBase.Coalesce(composition.Lyrics)));
            result.AddLast(new JKeyValuePair(Key.About, DataBase.Coalesce(composition.About)));
            return result;
        }

        public static void SaveToFile(this IEnumerable<Composition> set, string folderPath)
        {
            var path = Path.Combine(folderPath, "Compositions.json");
            var movePath = path + $"{System.DateTime.Today.ToString("yyyy-MM-dd__HH.mm.ss")}";
            if (File.Exists(path)) {
                File.Move(path, movePath);
            }
            Table lcTable = CrossTable.Load(folderPath, "Compositions");
            foreach (var lc in set)
            {
                CrossTable.AddNewObjectToCollection(lc.GetPropList(), lcTable.Items);
            }
            lcTable.Root.ToFile(lcTable.FilePath);
            File.Delete(movePath);
        }

        public static void EnsureCreated(string folderPath)
        {
            if (!File.Exists(Path.Combine(folderPath, "Compositions.json")))
            {
                Table lcTable = CrossTable.Load(folderPath, "Compositions");
                lcTable.Root.ToFile(lcTable.FilePath);
            }
        }
    }
}
