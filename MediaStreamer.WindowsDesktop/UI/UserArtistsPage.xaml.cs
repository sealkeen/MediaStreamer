using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MediaStreamer.Domain;

namespace MediaStreamer.WindowsDesktop
{
    /// <summary>
    /// Логика взаимодействия для UserArtistsPage.xaml
    /// </summary>
    public partial class UserArtistsPage : FirstFMPage
    {
        public bool lastDataLoadWasPartial = false;
        public List<ListenedArtist> Artists { get; set; }
        public UserArtistsPage()
        {
            //Artists = new List<Artist>();
            InitializeComponent();
            ListArtists();
            DataContext = this;
        }

        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var name = this.Artists[lstItems.SelectedIndex].Artist.ArtistName;
            var artistID = this.Artists[lstItems.SelectedIndex].ArtistID;

            if (Session.AlbumsPage == null)
                Session.AlbumsPage = new AlbumsPage(artistID);
            else
                Session.AlbumsPage.PartialListAlbums(artistID);
            Session.MainPage.mainFrame.Content = Session.AlbumsPage;
            Session.MainPage.mainFrame.UpdateLayout();
            Session.MainPage.SetCurrentStatus($"You listened albums by artist <{name}> listing:");
        }

        public void ListArtists()
        {
            Artists  = (from lArt
                       in Program.DBAccess.DB.GetListenedArtists()
                       where (lArt.UserID == SessionInformation.CurrentUser.UserID)
                       select lArt).Distinct().ToList();


            lstItems.GetBindingExpression(
                System.Windows.Controls.ListView.ItemsSourceProperty
                ).UpdateTarget();
        }

        public void PartialListArtists(long ArtistID)
        {

            Artists = Program.DBAccess.DB.GetListenedArtists().Where(art => art.ArtistID == ArtistID).ToList();
            lastDataLoadWasPartial = true;

            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        //public bool DisableUserInterfaceIfNotLogged()
        //{
        //    if (SessionInformation.UserStatus == LogStatus.Unlogged) {
        //        return true;
        //    }
        //    return false;
        //} 
    }
}
