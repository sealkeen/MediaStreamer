using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MediaStreamer.Domain;
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для GenresPage.xaml
    /// </summary>
    public partial class GenresPage : FirstFMPage
    {
        public bool lastDataLoadWasPartial = false;
        public List<Genre> Genres { get; set; }

        public void ListGenres()
        {
            //DBAccess.Update();
            try
            {
                Genres = Program.DBAccess.DB.GetGenres().ToList();
            } catch (Exception ex) {
                Program.SetCurrentStatus("Database was not loaded correctly.");
            }
        }
        public GenresPage()
        {
            //Genres = new List<Genre>();
            InitializeComponent();
            ListGenres();
            DataContext = this;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var genreName = this.Genres[lstItems.SelectedIndex].GenreName;

            if (Session.ArtistsPage == null)
                Session.ArtistsPage = new ArtistsPage(genreName);
            else
                Session.ArtistsPage.PartialListArtists(genreName);
            Session.MainPage.SetFrameContent( Session.ArtistsPage );
            Session.MainPage.UpdateFrameLayout();
            Session.MainPage.SetStatus($"Chosen genres's <{genreName}> artist listing:");
        }
    }
}
