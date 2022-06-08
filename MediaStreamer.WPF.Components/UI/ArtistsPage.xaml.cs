using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MediaStreamer.Domain;
using LinqExtensions;
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для ArtistsPage.xaml
    /// </summary>
    public partial class ArtistsPage : FirstFMPage
    {
        public ArtistsPage()
        {
            Session.ArtistsVM = new ArtistsViewModel();
            InitializeComponent();
            List();
            DataContext = Session.ArtistsVM;
        }
        public ArtistsPage(string genreName)
        {
            Session.ArtistsVM = new ArtistsViewModel();
            InitializeComponent();
            ListByTitle(genreName);
            DataContext = Session.ArtistsVM;
        }
        public ArtistsPage(Guid userID, Guid artistID)
        {
            Session.ArtistsVM = new ArtistsViewModel();
            InitializeComponent();
            ListByUserAndID(userID, artistID);
            DataContext = Session.ArtistsVM;
        }
        public override void List()
        {
            try
            {
                Session.ArtistsVM.Artists = Program.DBAccess.DB.GetArtists().ToList();
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
            }
        }
        public override void ListByID(Guid ArtistID)
        {
            Session.ArtistsVM.Artists = Program.DBAccess.DB.GetArtists().Where(art => art.ArtistID == ArtistID).ToList();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }
        public override void ListByUserAndID(Guid userID, Guid ArtistID)
        {
            Session.ArtistsVM.Artists = (
                from art in Program.DBAccess.DB.GetArtists()
                    join comps in Program.DBAccess.DB.GetCompositions()
                        on art.ArtistID equals comps.ArtistID
                    join listComp in Program.DBAccess.DB.GetListenedCompositions()
                        on comps.CompositionID equals listComp.CompositionID 
                select art).ToList();

            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }
        public override void ListByTitle(string genreName) //TODO: optimize the method
        {
            var genres = Program.DBAccess.DB.GetGenres().Where(g => g.GenreName == genreName);
            if (genres.Count() == 0)
                return;

            var artists = Program.DBAccess.DB.GetArtistGenres().Where(ag => ag.GenreID == genres.First().GenreID);
            if (artists.Count() == 0)
                return;

            Session.ArtistsVM.Artists = (from ag in artists
                       join art in Program.DBAccess.DB.GetArtists()
                       on ag.ArtistID equals art.ArtistID
                       select art).ToList();

            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }
        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstItems.SelectedIndex < lstItems.Items.Count)
            {
                var name = Session.ArtistsVM.Artists[lstItems.SelectedIndex].ArtistName;
                var artistID = Session.ArtistsVM.Artists[lstItems.SelectedIndex].ArtistID;

                if (Selector.AlbumsPage == null)
                    Selector.AlbumsPage = new AlbumsPage(artistID);
                else
                    Selector.AlbumsPage.ListByID(artistID);
                Selector.MainPage.SetFrameContent( Selector.AlbumsPage);
                Selector.MainPage.UpdateFrameLayout();
                Selector.MainPage.SetStatus($"Chosen artit's <{name}> albums listing:");
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
                        Session.ArtistsVM.Artists = Session.ArtistsVM.Artists.OrderByWithDirection(x => x.ArtistName, _orderByDescending).ToList();
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
