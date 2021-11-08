#nullable disable

namespace MediaStreamer.Models
{
    public partial class Picture
    {
        public long PictureID { get; set; }
        public long? Xresolution { get; set; }
        public long? Yresolution { get; set; }
        public long? SizeKb { get; set; }
        public string FilePath { get; set; }
    }
}
