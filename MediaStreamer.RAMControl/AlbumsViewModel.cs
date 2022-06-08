using MediaStreamer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaStreamer.RAMControl
{
    public class AlbumsViewModel
    {
        protected Guid _lastPartialGenreID = Guid.Empty;
        protected Guid _lastPartialArtistID = Guid.Empty;

        public List<Album> Albums { get; set; }
        public AlbumsViewModel()
        {
            Albums = new List<Album>();
        }

        public AlbumsViewModel(Guid artistID)
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

        public void SetLastArtistID(Guid artistID)
        {
            _lastPartialArtistID = artistID;
        }

        public bool LastDataLoadWasPartial()
        {
            return _lastPartialArtistID != Guid.Empty;
        }

        public void ResetPartialLoad()
        {
            _lastPartialArtistID = Guid.Empty;
            _lastPartialGenreID = Guid.Empty;
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
