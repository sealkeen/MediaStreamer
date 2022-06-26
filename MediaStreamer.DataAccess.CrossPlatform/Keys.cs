using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public static class Key
    {
        public const string ArtistName = "ArtistName";
        public const string ArtistID = "ArtistID";
        public const string GenreName = "GenreName";
        public const string GenreID = "GenreID";
        public const string AlbumID = "AlbumID";
        public const string AlbumName = "AlbumName";
        public const string Year = "Year";
        public const string Type = "Type";
        public const string Label = "Label";
        public const string CompositionID = "CompositionID";
        public const string CompositionName = "CompositionName";
        public const string Duration = "Duration";
        public const string FilePath = "FilePath";
        public const string Lyrics = "Lyrics";
        public const string About = "About";

        //ListenedComposition
        public const string ListenDate = "ListenDate";
        public const string StoppedAt = "StoppedAt";
        
        public const string UserID = "UserID";

        //PlayerState
        public const string StateID = "StateID";
        public const string StateTime = "StateTime";
        public const string VolumeLevel = "VolumeLevel";

        //DBPath
        public const string DBPathID = "DBPathID";
        public const string DataSource = "DataSource";

        public static long Parse(string str)
        {
            long lg;
            bool ok = long.TryParse(str, out lg);
            if (!ok)
                throw new InvalidCastException($"<{str}> cannot be cast to type long.");
            return lg;
        }
    }
}
