using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MediaStreamer.Domain;
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для UserArtistsPage.xaml
    /// </summary>
    public partial class UserArtistsPage : FirstFMPage
    {
        public bool lastDataLoadWasPartial = false;
        public List<Artist> Artists { get; set; }
        public UserArtistsPage()
        {
            //Artists = new List<Artist>();
            InitializeComponent();
            ListArtists();
            DataContext = this;
        }

        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var name = this.Artists[lstItems.SelectedIndex].ArtistName;
            var artistID = this.Artists[lstItems.SelectedIndex].ArtistID;

            if (Selector.AlbumsPage == null)
                Selector.AlbumsPage = new AlbumsPage(artistID);
            else
                Selector.AlbumsPage.ListByID(artistID);
            Selector.MainPage.SetFrameContent( Selector.AlbumsPage );
            Selector.MainPage.UpdateFrameLayout();
            Selector.MainPage.SetStatus($"You listened albums by artist <{name}> listing:");
        }

        public void ListArtists()
        {
            Artists  = (
                from lc in Program.DBAccess.DB.GetListenedCompositions()
                join c in Program.DBAccess.DB.GetCompositions()
                          on lc.CompositionID equals c.CompositionID
                join lArt in Program.DBAccess.DB.GetArtists()
                          on c.ArtistID equals lArt.ArtistID
                where (lc.UserID == SessionInformation.CurrentUser.UserID)
                       select lArt).Distinct().ToList();

            lstItems.GetBindingExpression(
                System.Windows.Controls.ListView.ItemsSourceProperty
                ).UpdateTarget();
        }

        public void PartialListArtists(long ArtistID)
        {

            Artists = (
                from lc   in Program.DBAccess.DB.GetListenedCompositions()
                join c    in Program.DBAccess.DB.GetCompositions()
                          on lc.CompositionID equals c.CompositionID
                join lArt in Program.DBAccess.DB.GetArtists()
                          on c.ArtistID equals lArt.ArtistID
                where (lc.UserID == SessionInformation.CurrentUser.UserID)
                select lArt).Where(art => art.ArtistID == ArtistID).ToList();

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
