using System;
using System.Linq;

namespace MediaStreamer.TagEditing
{
    public static class DMTagExtractor
    {
        private static string ResolveArtistTitleConflicts(string fileName, string titleFromMetaD, string artistFromMetaD, ref string artistName, ref string compositionName)
        {
            string divider;
            if (fileName.Contains(divider = "-") || fileName.Contains(divider = "—"))
            {

                int firstPartLength = fileName.IndexOf(divider);
                int secondPartStart = fileName.IndexOf(divider) + 1;

                artistName = fileName.Substring(0, firstPartLength);
                compositionName = fileName.Substring(secondPartStart);
            }
            else
            {
                divider = null;
            }

            if (artistFromMetaD == null || artistFromMetaD.ToLower() == "unknown")
            {
                if (divider != null)
                {
                    artistName = artistName.TrimStart(divider.ToCharArray()[0]).TrimStart(' ');
                    artistName = artistName.TrimEnd(divider.ToCharArray()[0]).TrimEnd(' ');
                }
                else
                {
                    artistName = "unknown";
                }
            }
            else
            {
                artistName = artistFromMetaD;
            }

            if (titleFromMetaD == null || titleFromMetaD.ToLower() == "unknown")
            {
                if (divider != null)
                {
                    compositionName = compositionName.TrimStart(divider.ToCharArray()[0]).TrimStart(' ');
                    compositionName = compositionName.TrimEnd(divider.ToCharArray()[0]).TrimEnd(' ');
                }
                else
                {
                    compositionName = fileName;
                }
            }
            else
            {
                compositionName = titleFromMetaD;
            }

            return divider;
        }

        public static string ExcludeExtension(string withPossibleExtension)
        {
            var dotIndex = -1;
            int extensionWithDotLength = 0;
            for (int i = withPossibleExtension.Length - 1; i >= 0; i--)
            {

                extensionWithDotLength++;
                if (withPossibleExtension[i] == '.')
                {
                    if (extensionWithDotLength < 2)
                        return withPossibleExtension;
                    dotIndex = i;
                    break;
                }
                if (
                    (withPossibleExtension[i] < 'a' ||
                    withPossibleExtension[i] > 'z')
                    &&
                    (withPossibleExtension[i] > 'Z' ||
                    withPossibleExtension[i] < 'A')
                    &&
                    (withPossibleExtension[i] < '0' ||
                    withPossibleExtension[i] > '9')
                )
                    return withPossibleExtension;
            }

            int withoutExtensionLength = withPossibleExtension.Length - extensionWithDotLength;

            if (extensionWithDotLength > 5)
                return withPossibleExtension;
            //because there is no audio extension with more then 4 symbols

            return withPossibleExtension.Substring(0, withoutExtensionLength);
        }

        public static string TryGetArtistNameFromFile(TagLib.File tfile, Action<string> errorAction)
        {
            try {
                if(tfile.Tag.Performers == null || tfile.Tag.Performers.Length <= 0)
                    return "Unknown";
                return tfile.Tag.Performers[0];
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return "Unknown";
            }
        }
        public static string TryGetGenreFromFile(TagLib.File tfile, Action<string> errorAction)
        {
            try
            {
                if (tfile.Tag.Genres == null || tfile.Tag.Genres.Length <= 0)
                    return "Unknown";
                return tfile.Tag.Genres.First();
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return "Unknown";
            }
        }

        public static string TryGetTitleFromFile(TagLib.File tfile, Action<string> errorAction = null)
        {
            try {
                if (tfile.Tag.Title == null || tfile.Tag.Title.Length <= 0)
                    return "Unknown";
                return tfile.Tag.Title;
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return "Unknown";
            }
        }
        public static string TryGetAlbumFromFile(TagLib.File tfile, Action<string> errorAction = null)
        {
            try
            {
                if (tfile.Tag.Album == null || tfile.Tag.Album.Length <= 0)
                    return "unknown";
                return tfile.Tag.Album;
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return "unknown";
            }
        }
        public static long? TryGetYearFromFile(TagLib.File tfile, Action<string> errorAction = null)
        {
            try
            {
                if (tfile.Tag.Year == 0)
                    return null;
                return tfile.Tag.Year;
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return null;
            }
        }

        public static TimeSpan TryGetDurationFromFile(TagLib.File tfile, Action<string> errorAction = null)
        {
            try {
                if (tfile.Properties.Duration == TimeSpan.FromMinutes(0))
                    return TimeSpan.MinValue;
                return tfile.Properties.Duration;
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return TimeSpan.MinValue;
            }
        }
    }
}
