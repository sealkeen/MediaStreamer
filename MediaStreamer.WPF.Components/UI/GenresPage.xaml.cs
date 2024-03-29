﻿using MediaStreamer.RAMControl;
using System;
using System.Linq;
using System.Windows.Input;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для GenresPage.xaml
    /// </summary>
    public partial class GenresPage : FirstFMPage
    {

        public void ListGenres()
        {
            //DBAccess.Update();
            try
            {
                Session.GenresVM.Genres = Program.DBAccess.DB.GetGenres().ToList();
            } catch (Exception ex) {
                Program.SetCurrentStatus("Database was not loaded correctly.");
            }
        }
        public GenresPage()
        {
            Session.GenresVM = new GenresViewModel();
            InitializeComponent();

            ListGenres();
            DataContext = Session.GenresVM;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var genreName = Session.GenresVM.Genres[lstItems.SelectedIndex].GenreName;

            if (Selector.ArtistsPage == null)
                Selector.ArtistsPage = new ArtistsPage(genreName);
            else
                Selector.ArtistsPage.ListByTitle(genreName);
            Selector.MainPage.SetFrameContent( Selector.ArtistsPage );
            Selector.MainPage.UpdateFrameLayout();
            Selector.MainPage.SetStatus($"Chosen genres's <{genreName}> artist listing:");
        }
    }
}
