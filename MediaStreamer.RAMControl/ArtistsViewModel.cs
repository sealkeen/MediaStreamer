using MediaStreamer.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.RAMControl
{
    public class ArtistsViewModel
    {

        public List<Artist> Artists { get; set; }

        public ArtistsViewModel()
        {
            Artists = new List<Artist>();
        }

        public ArtistsViewModel(string title)
        {
            Artists = new List<Artist>();
        }
    }
}
