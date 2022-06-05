using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.WPF.Components
{
    public static class Selector
    {
        public static MainPage MainPage { get; set; }
        public static CompositionsPage CompositionsPage { get; set; }
        public static AlbumsPage AlbumsPage { get; set; }
        public static ArtistsPage ArtistsPage { get; set; }
        public static ArtistGenresPage AGenresPage { get; set; }
        public static GenresPage GenresPage { get; set; }
        public static SignUpPage SignUpPage { get; set; }
        public static UserCompositionsPage ListenedCompositionsPage { get; set; }
        public static UserAlbumsPage ListenedAlbumsPage { get; set; }
        public static UserGenresPage UserGenresPage { get; set; }
        public static SignedUpPage SignedUpPage { get; set; }
        public static TagEditorPage TagEditorPage { get; set; }
        public static VideoPage VideoPage { get; set; }
        public static LoadingPage LoadingPage { get; set; }

        internal static void Dispose()
        {
            
        }
    }
}
