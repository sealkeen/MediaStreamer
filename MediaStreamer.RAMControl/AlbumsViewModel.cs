using MediaStreamer.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaStreamer.RAMControl
{
    public class AlbumsViewModel
    {
        protected long _lastPartialGenreID = -1;
        protected long _lastPartialArtistID = -1;

        public List<Album> Albums { get; set; }
        public AlbumsViewModel()
        {
            Albums = new List<Album>();
        }

        public AlbumsViewModel(long artistID)
        {
            _lastPartialArtistID = artistID;
            Albums = (from album in Program.DBAccess.DB.GetAlbums()
                      where album.ArtistID == artistID
                      select album).ToList();
        }

        public AlbumsViewModel(string genreName)
        {
            Albums = new List<Album>();
        }

        public void SetLastArtistID(long artistID)
        {
            _lastPartialArtistID = artistID;
        }

        public bool LastDataLoadWasPartial()
        {
            return _lastPartialArtistID != -1;
        }

        public void ResetPartialLoad()
        {
            _lastPartialArtistID = -1;
            _lastPartialGenreID = -1;
        }

        public async void PartialListAlbums()
        {
            await Task.Factory.StartNew(GetPartOfAlbums);
        }

        public void GetPartOfAlbums()
        {
            Albums = (from composition in Program.DBAccess.DB.GetAlbums()
                                              where composition.ArtistID == _lastPartialArtistID
                                              select composition).ToList();
        }
    }
}
