using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MediaStreamer.Domain;
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для UserAlbumsPage.xaml
    /// </summary>
    public partial class UserAlbumsPage : FirstFMPage
    {
        public List<ListenedAlbum> Albums { get; set; }
        public UserAlbumsPage()
        {
            try
            {
                Albums = new List<ListenedAlbum>();
                InitializeComponent();
                PartialListAlbums(SessionInformation.CurrentUser.UserID);
                DataContext = this;
            }
            catch (Exception ex)
            {
                Program.HandleException(ex);
            }
        }

        public void PartialListAlbums(long userID)
        {
            try
            {
                Albums = (from alb in Program.DBAccess.DB.GetListenedAlbums()
                           join listComp in Program.DBAccess.DB.GetListenedCompositions()
                           on alb.AlbumID equals listComp.AlbumID
                           where (listComp.UserID == userID)
                           select alb).Distinct().ToList();
                lastDataLoadWasPartial = true;
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex)
            {
                Program.HandleException(ex);
            }
        }

        public ListenedAlbum NewListenedAlbum(
            Album album,
            User user, long? countOfPlays = null)
        {
            try
            {
                var lA = new ListenedAlbum();
                lA.Album = album;
                lA.AlbumID = album.AlbumID;
                lA.Artist = album.Artist;
                lA.ArtistID = album.ArtistID.Value;
                lA.CountOfPlays = countOfPlays;
                lA.GroupFormationDate = album.GroupFormationDate.Value;
                lA.GroupMember = album.GroupMember;
                lA.ListenDate = null;
                lA.User = user;
                lA.UserID = user.UserID;

                Program.DBAccess.DB.Add(lA);

                return lA;

            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus($"ButtonLogLickException : {ex.Message}");
                return null;
            }
        }

        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var name = this.Albums[lstItems.SelectedIndex].Album.AlbumName;
                var artistID = this.Albums[lstItems.SelectedIndex].ArtistID;
                var albumID = this.Albums[lstItems.SelectedIndex].AlbumID;

                if (Selector.CompositionsPage == null)
                    Selector.CompositionsPage = new CompositionsPage(artistID, albumID);
                else
                {
                    Session.CompositionsVM.SetLastAlbumAndArtistID(albumID, artistID);
                    Session.CompositionsVM.PartialListCompositions();
                }
                Selector.MainPage.SetFrameContent( Selector.CompositionsPage );
                Selector.MainPage.UpdateFrameLayout();
                Selector.MainPage.SetStatus($"Chosen artit's <{name}> compositions listing:");
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus($"AlbumsPage exception in functions: lstItems_MouseDoubleClick(): {ex.Message}");
            }
        }

        private void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAddAlbum_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
