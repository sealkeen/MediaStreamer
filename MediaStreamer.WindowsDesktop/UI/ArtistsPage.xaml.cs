using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MediaStreamer.FileTypes;
using LinqExtensions;

namespace MediaStreamer.WindowsDesktop
{
    /// <summary>
    /// Логика взаимодействия для ArtistsPage.xaml
    /// </summary>
    public partial class ArtistsPage : FirstFMPage
    {
        public bool lastDataLoadWasPartial = false;
        public bool _orderByDescending = true;
        public List<Artist> Artists { get; set; }

        public void ListArtists()
        {
            try
            {
                Artists = Program.DBAccess.DB.GetArtists().ToList();
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
            }
        }
        public void PartialListArtists(long ArtistID)
        {
            Artists = Program.DBAccess.DB.GetArtists().Where(art => art.ArtistID == ArtistID).ToList();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public void PartialListArtists(long userID, long ArtistID)
        {
            Artists = (from art in Program.DBAccess.DB.GetArtists()
                       join listComp in Program.DBAccess.DB.GetListenedCompositions()
                       on art.ArtistID equals listComp.ArtistID
                       select art).ToList();

            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public void PartialListArtists(string genreName)
        {
            Artists = Program.DBAccess.DB.GetAlbums().Where(a => (a.GenreName == genreName)).Select(album => album.Artist).ToList();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public ArtistsPage()
        {
            //Artists = new List<Artist>();
            InitializeComponent();
            ListArtists();
            DataContext = this;
        }
        public ArtistsPage(string genreName)
        {
            //Artists = new List<Artist>();
            InitializeComponent();
            PartialListArtists(genreName);
            DataContext = this;
        }
        public ArtistsPage(long userID, long artistID)
        {
            //Artists = new List<Artist>();
            InitializeComponent();
            PartialListArtists(userID, artistID);
            DataContext = this;
        }

        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstItems.SelectedIndex < lstItems.Items.Count)
            {
                var name = this.Artists[lstItems.SelectedIndex].ArtistName;
                var artistID = this.Artists[lstItems.SelectedIndex].ArtistID;

                if (Session.AlbumsPage == null)
                    Session.AlbumsPage = new AlbumsPage(artistID);
                else
                    Session.AlbumsPage.PartialListAlbums(artistID);
                Session.MainPage.mainFrame.Content = Session.AlbumsPage;
                Session.MainPage.mainFrame.UpdateLayout();
                Session.MainPage.SetCurrentStatus($"Chosen artit's <{name}> albums listing:");
            }
        }

        private void lstItems_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lstItems_ColumnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                _orderByDescending = !_orderByDescending;
                string columnName = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
                switch (columnName)
                {
                    case "Artist Name":
                        Artists = Artists.OrderByWithDirection(x => x.ArtistName, _orderByDescending).ToList();
                        break;
                }
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            }
        }
    }
}
