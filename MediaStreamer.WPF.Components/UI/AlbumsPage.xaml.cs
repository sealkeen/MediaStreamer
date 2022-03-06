using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MediaStreamer.Domain;
using LinqExtensions;
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для ListenedAlbums.xaml
    /// </summary>
    public partial class AlbumsPage : FirstFMPage
    {
        public AlbumsPage()
        {
            Session.AlbumsVM = new AlbumsViewModel();
            InitializeComponent();
            ListAlbums();
            DataContext = Session.AlbumsVM;
        }

        public AlbumsPage(long ArtistID)
        {
            InitializeComponent();
            PartialListAlbums(ArtistID);
            DataContext = Session.AlbumsVM;
        }
        public AlbumsPage(string genreName)
        {
            InitializeComponent();
            PartialListAlbums(genreName);
            DataContext = Session.AlbumsVM;
        }
        public List<Album> GetAlbums()
        {
            try
            {
                //Program.DBAccess.Update();
                return Program.DBAccess.DB?.GetAlbums()?.ToList();
            } catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message, true);
                return new List<Album>();
            }
        }

        public void ListAlbums()
        {
            Session.AlbumsVM.Albums = GetAlbums();
            lastDataLoadWasPartial = false;
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }
        public void PartialListAlbums(long artistID)
        {
            Session.AlbumsVM.Albums = (from album in Program.DBAccess.DB.GetAlbums()
                      where album.ArtistID == artistID
                      select album).ToList();

            lastDataLoadWasPartial = true;
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public void PartialListAlbums(string genreName)
        {
            var genres = Program.DBAccess.DB.GetGenres().Where(g => g.GenreName == genreName);
            if (genres.Count() == 0)
                return;
            var artists = Program.DBAccess.DB.GetArtistGenres().Where(ag => ag.GenreID == genres.First().GenreID);
            if (artists.Count() == 0)
                return;
            var albums = Program.DBAccess.DB.GetAlbums().Where(a => (a.ArtistID == artists.First().ArtistID));
            if (albums.Count() == 0)
                return;
            Session.AlbumsVM.Albums = albums.ToList();
            lastDataLoadWasPartial = true;
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? index = lstItems?.SelectedIndex;
                if (index != null && index != -1)
                {
                    Program.DBAccess?.DeleteAlbum(Session.AlbumsVM.Albums[index.Value]);
                }
                ListAlbums();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus($"Delete composition violation: {ex.Message}", true);
            }
        }

        private void btnAddAlbum_Click(object sender, RoutedEventArgs e)
        {
            AddAlbumFromString(txtNewAlbum.Text);
            return;
            try
            {
                Program.DBAccess.AddAlbum(
                    txtArtistName.Text,
                    txtAlbumName.Text,
                    Convert.ToInt64(txtYear.Text),
                    txtLabel.Text == "" ? null : txtLabel.Text,
                    txtType.Text == "" ? null : txtLabel.Text
                    );
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }
        public static void AddAlbumFromString(string text)
        {
            List<string> str = new List<string>(text.Replace("\'", "").Split(','));
            if (str.Count() != 6)
                return;
            Program.DBAccess.AddAlbum(
                str[0],
                str[2],
                Convert.ToInt64(str[3]),
                str[4],
                str[5]
                );
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtArtistName.Text = Session.AlbumsVM.Albums[lstItems.SelectedIndex].Artist.ArtistName;
                txtAlbumName.Text = Session.AlbumsVM.Albums[lstItems.SelectedIndex].AlbumName;
                txtYear.Text = (Session.AlbumsVM.Albums[lstItems.SelectedIndex].Year == null) ? "-" : Session.AlbumsVM.Albums[lstItems.SelectedIndex].Year.ToString();
                txtLabel.Text = (Session.AlbumsVM.Albums[lstItems.SelectedIndex].Label == null) ? "-" : Session.AlbumsVM.Albums[lstItems.SelectedIndex].Label;
                txtType.Text = (Session.AlbumsVM.Albums[lstItems.SelectedIndex].Type == null) ? "-" : Session.AlbumsVM.Albums[lstItems.SelectedIndex].Type;
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var name = Session.AlbumsVM.Albums[lstItems.SelectedIndex].AlbumName;
                var artistID = Session.AlbumsVM.Albums[lstItems.SelectedIndex].ArtistID;
                var albumID = Session.AlbumsVM.Albums[lstItems.SelectedIndex].AlbumID;

                if (Selector.CompositionsPage == null)
                    Selector.CompositionsPage = new CompositionsPage(artistID.Value, albumID);
                else
                    Selector.CompositionsPage.PartialListCompositions(artistID.Value, albumID);
                Selector.MainPage.SetFrameContent(Selector.CompositionsPage);
                Selector.MainPage.UpdateFrameLayout();
                Selector.MainPage.SetStatus($"Chosen artit's <{name}> compositions listing:");
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus($"AlbumsPage exception in functions: lstItems_MouseDoubleClick(). : {ex.Message}");
            }
        }

        private void ButtonSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstItems_ColumnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _orderByDescending = !_orderByDescending;
                string columnName = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
                switch (columnName)
                {
                    case "AlbumName":
                        Session.AlbumsVM.Albums = Session.AlbumsVM.Albums.OrderByWithDirection(x => x.AlbumName, _orderByDescending).ToList();
                        break;
                    case "Artist":
                        Session.AlbumsVM.Albums = Session.AlbumsVM.Albums.OrderByWithDirection(x => x.Artist.ArtistName, _orderByDescending).ToList();
                        break;
                    case "Year":
                        Session.AlbumsVM.Albums = Session.AlbumsVM.Albums.OrderByWithDirection(x => x.Year, _orderByDescending).ToList();
                        break;
                }
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            }
        }

        private void lstItems_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
