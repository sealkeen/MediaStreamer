using MediaStreamer.Domain;
using System.Collections.Generic;

namespace MediaStreamer.RAMControl
{
    public class AlbumsViewModel
    {
        public List<Album> Albums { get; set; }
        public AlbumsViewModel()
        {
            Albums = new List<Album>();
        }

        public AlbumsViewModel(long artistID)
        {
            Albums = new List<Album>();
        }

        public AlbumsViewModel(string genreName)
        {
            Albums = new List<Album>();
        }
    }
}
