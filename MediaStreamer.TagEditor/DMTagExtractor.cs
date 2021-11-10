using System;
using System.Linq;

namespace MediaStreamer.TagEditing
{
    public static class DMTagExtractor
    {
        public static string TryGetArtistNameFromFile(TagLib.File tfile, Action<string> errorAction)
        {
            try {
                return tfile.Tag.Performers[0];
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return "Unknown";
            }
        }
        public static string TryGetGenreFromFile(TagLib.File tfile, Action<string> errorAction)
        {
            try {
                return tfile.Tag.Genres.First();
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return "Unknown";
            }
        }

        public static string TryGetTitleFromFile(TagLib.File tfile, Action<string> errorAction = null)
        {
            try {
                return tfile.Tag.Title;
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return "Unknown";
            }
        }
        public static string TryGetAlbumFromFile(TagLib.File tfile, Action<string> errorAction = null)
        {
            try {
                return tfile.Tag.Album;
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return "Unknown";
            }
        }
        public static long? TryGetYearFromFile(TagLib.File tfile, Action<string> errorAction = null)
        {
            try {
                return tfile.Tag.Year;
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return null;
            }
        }

        public static TimeSpan TryGetDurationFromFile(TagLib.File tfile, Action<string> errorAction = null)
        {
            try {
                return tfile.Properties.Duration;
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return TimeSpan.MinValue;
            }
        }
    }
}
