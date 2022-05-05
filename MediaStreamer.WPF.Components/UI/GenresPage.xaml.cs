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

            if (Selector.ArtistsPage == null)
                Selector.ArtistsPage = new ArtistsPage(genreName);
            else
                Selector.ArtistsPage.ListByTitle(genreName);
            Selector.MainPage.SetFrameContent( Selector.ArtistsPage);
            Selector.MainPage.UpdateFrameLayout();
            Selector.MainPage.SetStatus($"Chosen genres's <{genreName}> artist listing:");
        }
    }
}
