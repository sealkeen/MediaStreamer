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
        public List<Album> Albums { get; set; }
        public UserAlbumsPage()
        {
            try
            {
                Albums = new List<Album>();
                InitializeComponent();
                PartialListAlbums(SessionInformation.CurrentUser.UserID);
                DataContext = this;
            }
            catch (Exception ex)
            {
                Program.HandleException(ex);
            }
        }

        public void PartialListAlbums(Guid userID)
        {
            try
            {
                Albums = (from alb in Program.DBAccess.DB.GetAlbums()
                          join comps in Program.DBAccess.DB.GetCompositions()
                            on alb.ArtistID equals comps.ArtistID
                          join listComp in Program.DBAccess.DB.GetListenedCompositions()
                              on comps.CompositionID equals listComp.CompositionID
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

        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var name = this.Albums[lstItems.SelectedIndex].AlbumName;
                var artistID = this.Albums[lstItems.SelectedIndex].ArtistID;
                var albumID = this.Albums[lstItems.SelectedIndex].AlbumID;

                if (Selector.CompositionsPage == null)
                    Selector.CompositionsPage = new CompositionsPage(artistID.Value, albumID);
                else
                {
                    Session.CompositionsVM.SetLastAlbumAndArtistID(albumID, artistID.Value);
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
