using MediaStreamer.RAMControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MediaStreamer.WPF.Components
{
    public static class Selector
    {
        public static Window MainWindow { get; set; }
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


        public static Style BlankStyle { get; private set; }
        private static List<FirstFMPage> lst = new List<FirstFMPage>() { CompositionsPage, AlbumsPage, ArtistsPage };
        public static void Rerender()
        {
            //Type type = typeof(Selector); // MyClass is static class with static properties
            //var cnt = type.GetFields().Count();
            
            foreach (var p in lst )//type.GetFields())
            {
                //var o_page = p.GetValue(null); // static classes cannot be instanced, so use null...
                                          //do something with v

                //if (o_page is FirstFMPage)
                {
                    var page = p;//o_page as FirstFMPage;
                    //if (page.Style != BlankStyle)
                    //{
                    //    if (BlankStyle == null)
                    //        BlankStyle = new Style
                    //        {
                    //            TargetType = typeof(Page)
                    //        };

                    //    page.Style = BlankStyle;
                    //}
                    //else
                    //{
                    //    page.Style = (Style)MainWindow.FindResource("MainWindowStyle");
                    //}
                    if(page != null)
                        page.Rerender();
                }
            }
        }
    }
}
