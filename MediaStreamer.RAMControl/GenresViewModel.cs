using MediaStreamer.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.RAMControl
{
    public class GenresViewModel
    {
        public GenresViewModel()
        {
            Genres = new List<Genre>();
        }
        public List<Genre> Genres { get; set; }
    }
}
