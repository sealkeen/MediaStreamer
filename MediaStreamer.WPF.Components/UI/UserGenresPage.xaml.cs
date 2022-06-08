using System;
using System.Collections.Generic;
using System.Linq;
using MediaStreamer.Domain;
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для UserGenresPage.xaml
    /// </summary>
    public partial class UserGenresPage : FirstFMPage
    {
        public bool lastDataLoadWasPartial = false;
        public List<Genre> Genres { get; set; }
        public UserGenresPage(Guid userID)
        {
            //Genres = new List<Genre>();
            InitializeComponent();
            ListGenres(userID);
            DataContext = this;
        }

        public void ListGenres(Guid userID)
        {
            Genres = GetListenedGenres(userID).ToList();

            if (Genres.Count() == 0)
            {
                Genres = (from lc in Program.DBAccess.DB.GetListenedCompositions() 
                          join comp in Program.DBAccess.DB.GetCompositions()
                          on lc.CompositionID equals comp.CompositionID
                          join alb in Program.DBAccess.DB.GetAlbums()
                          on comp.AlbumID equals alb.AlbumID
                          join gnr in Program.DBAccess.DB.GetGenres()
                          on alb.GenreID equals gnr.GenreID
                          where (lc.UserID == userID)
                          select gnr).ToList();

                lastDataLoadWasPartial = true;

                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
        }

        private static IQueryable<Genre> GetListenedGenres(Guid userID)
        {
            return (from lc in Program.DBAccess.DB.GetListenedCompositions()
                        join comp in Program.DBAccess.DB.GetCompositions()
                            on lc.CompositionID equals comp.CompositionID
                        join alb in Program.DBAccess.DB.GetAlbums()
                            on comp.AlbumID equals alb.AlbumID
                        join gnr in Program.DBAccess.DB.GetGenres()
                            on alb.GenreID equals gnr.GenreID
                    where (lc.UserID == userID)
                    select gnr);

            throw new NotImplementedException();
        }
    }
}
