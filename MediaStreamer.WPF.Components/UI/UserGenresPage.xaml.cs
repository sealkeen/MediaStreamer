using System;
using System.Collections.Generic;
using System.Linq;
using MediaStreamer.Domain;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для UserGenresPage.xaml
    /// </summary>
    public partial class UserGenresPage : FirstFMPage
    {
        public bool lastDataLoadWasPartial = false;
        public List<Genre> Genres { get; set; }
        public UserGenresPage(long userID)
        {
            //Genres = new List<Genre>();
            InitializeComponent();
            ListGenres(userID);
            DataContext = this;
        }

        public void ListGenres(long userID)
        {
            Genres = GetListenedGenres(userID).Select(x=>x.Genre).ToList();

            if (Genres.Count() == 0)
            {
                Genres = (from gnr in Program.DBAccess.DB.GetListenedGenres()
                          join listComp in Program.DBAccess.DB.GetListenedCompositions()
                          on gnr.Genre equals listComp.Album.Genre
                          where (listComp.UserID == userID)
                          select gnr.Genre).ToList();
                lastDataLoadWasPartial = true;

                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
        }

        private static IQueryable<ListenedGenre> GetListenedGenres(long userID)
        {
            return Program.DBAccess.DB.GetListenedGenres().Where(lG => (lG.UserID == userID));

            var comps = Program.DBAccess.GetCurrentUsersListenedCompositions(SessionInformation.CurrentUser);

            var user = SessionInformation.CurrentUser;

            bool upToDate =
                (user.LastListenEntitiesUpdate.HasValue &&
                user.LastListenedCompositionChange.HasValue) ||
                (user.LastListenEntitiesUpdate.Value ==
                user.LastListenedCompositionChange);

            if (!upToDate)
            {



            }

            throw new NotImplementedException();
        }
    }
}
