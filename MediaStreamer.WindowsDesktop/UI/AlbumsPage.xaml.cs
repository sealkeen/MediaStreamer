using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MediaStreamer.FileTypes;
using LinqExtensions;

namespace MediaStreamer.WindowsDesktop
{
    /// <summary>
    /// Логика взаимодействия для ListenedAlbums.xaml
    /// </summary>
    public partial class AlbumsPage : FirstFMPage
    {
        public bool lastDataLoadWasPartial = false;
        private bool _orderByDescending = true;
        public List<Album> Albums { get; set; }
        public List<Album> GetAlbums()
        {
            try
            {
                Program.DBAccess.Update();
                return Program.DBAccess.DB?.GetAlbums()?.ToList();
            } catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message, true);
                return new List<Album>();
            }
        }
        public void ListAlbums()
        {
            Albums = GetAlbums();
            lastDataLoadWasPartial = false;
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }
        public void PartialListAlbums(long artistID)
        {
            Albums = (from album in Program.DBAccess.DB.GetAlbums()
                      where album.ArtistID == artistID
                      select album).ToList();

            lastDataLoadWasPartial = true;
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public void PartialListAlbums(string genreName)
        {
            Albums = Program.DBAccess.DB.GetAlbums().Where(a => (a.GenreName == genreName)).ToList();
            lastDataLoadWasPartial = true;
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public AlbumsPage()
        {
            Albums = new List<Album>();
            InitializeComponent();
            ListAlbums();
            DataContext = this;
        }

        public AlbumsPage(long ArtistID)
        {
            Albums = new List<Album>();
            InitializeComponent();
            PartialListAlbums(ArtistID);
            DataContext = this;
        }
        public AlbumsPage(string genreName)
        {
            Albums = new List<Album>();
            InitializeComponent();
            PartialListAlbums(genreName);
            DataContext = this;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? index = lstItems?.SelectedIndex;
                if (index != null && index != -1)
                {
                    Program.DBAccess?.DeleteAlbum(Albums[index.Value]);
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
                txtArtistName.Text = Albums[lstItems.SelectedIndex].Artist.ArtistName;
                txtAlbumName.Text = Albums[lstItems.SelectedIndex].AlbumName;
                txtYear.Text = (Albums[lstItems.SelectedIndex].Year == null) ? "-" : Albums[lstItems.SelectedIndex].Year.ToString();
                txtLabel.Text = (Albums[lstItems.SelectedIndex].Label == null) ? "-" : Albums[lstItems.SelectedIndex].Label;
                txtType.Text = (Albums[lstItems.SelectedIndex].Type == null) ? "-" : Albums[lstItems.SelectedIndex].Type;
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
                var name = this.Albums[lstItems.SelectedIndex].AlbumName;
                var artistID = this.Albums[lstItems.SelectedIndex].ArtistID;
                var albumID = this.Albums[lstItems.SelectedIndex].AlbumID;

                if (Session.CompositionsPage == null)
                    Session.CompositionsPage = new CompositionsPage(artistID.Value, albumID);
                else
                    Session.CompositionsPage.PartialListCompositions(artistID.Value, albumID);
                Session.MainPage.mainFrame.Content = Session.CompositionsPage;
                Session.MainPage.mainFrame.UpdateLayout();
                Session.MainPage.SetCurrentStatus($"Chosen artit's <{name}> compositions listing:");
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
                        Albums = Albums.OrderByWithDirection(x => x.AlbumName, _orderByDescending).ToList();
                        break;
                    case "Artist":
                        Albums = Albums.OrderByWithDirection(x => x.Artist.ArtistName, _orderByDescending).ToList();
                        break;
                    case "Year":
                        Albums = Albums.OrderByWithDirection(x => x.Year, _orderByDescending).ToList();
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
