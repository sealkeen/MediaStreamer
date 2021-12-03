//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MediaStreamer.DataAccess.Net40.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Album
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Album()
        {
            this.AlbumGenres = new HashSet<AlbumGenre>();
            this.Compositions = new HashSet<Composition>();
            this.CompositionVideos = new HashSet<CompositionVideo>();
            this.ListenedAlbums = new HashSet<ListenedAlbum>();
            this.ListenedCompositions = new HashSet<ListenedComposition>();
        }
    
        public long AlbumID { get; set; }
        public string AlbumName { get; set; }
        public Nullable<long> ArtistID { get; set; }
        public Nullable<System.DateTime> GroupFormationDate { get; set; }
        public long GenreID { get; set; }
        public string GenreName { get; set; }
        public Nullable<long> Year { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
    
        public virtual Artist Artist { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual GroupMember GroupMember { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlbumGenre> AlbumGenres { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Composition> Compositions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompositionVideo> CompositionVideos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ListenedAlbum> ListenedAlbums { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ListenedComposition> ListenedCompositions { get; set; }
    }
}
