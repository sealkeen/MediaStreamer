using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DataBaseResource;
using MediaStreamer.FileTypes;

namespace DMultHandler
{
    /// <summary>
    /// Логика взаимодействия для ArtistGenres.xaml
    /// </summary>
    public partial class ArtistGenresPage : FirstFMPage
    {
        public bool lastDataLoadWasPartial = false;
        public List<ArtistGenre> ArtistGenres { get; set; }
        public void RetrieveArtistGenres()
        {
            ArtistGenres = Program.DBAccess.DB.GetArtistGenres().ToList();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public ArtistGenresPage()
        {
            InitializeComponent();
            ArtistGenres = new List<ArtistGenre>();
            RetrieveArtistGenres();
            DataContext = this;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
