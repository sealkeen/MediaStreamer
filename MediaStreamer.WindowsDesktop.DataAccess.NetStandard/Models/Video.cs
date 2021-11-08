using System.Collections.Generic;

#nullable disable

namespace MediaStreamer.Models
{
    public partial class Video
    {
        public Video()
        {
            CompositionVideos = new HashSet<CompositionVideo>();
        }

        public long VideoID { get; set; }
        public long? Xresolution { get; set; }
        public long? Yresolution { get; set; }
        public double? Fps { get; set; }
        public byte[] VariableFps { get; set; }
        public long? SizeKb { get; set; }
        public string FilePath { get; set; }

        public virtual ICollection<CompositionVideo> CompositionVideos { get; set; }
    }
}
