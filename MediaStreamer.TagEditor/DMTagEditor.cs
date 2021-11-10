using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using MediaStreamer.Domain;
using TagLib;

namespace MediaStreamer.TagEditing
{
    public class DMTagEditor
    {
        public List<IComposition> _compositions = null;
        private List<TagLib.Tag> _tagsv2 = null;
        private List<TagLib.File> _files = null;
        private Action<string> _statusSetter = null;
        private IDBRepository DBAccess;

        public DMTagEditor(List<TagLib.File> tagFiles, List<IComposition> compositions, 
            IDBRepository iDBAccess, Action<string> statusSetter = null)
        {
            DBAccess = iDBAccess;
            _statusSetter = statusSetter;
            if (tagFiles == null || compositions == null)
            {
                SetCurrentStatus("An error occured. The requested composition wasn't found.");
                return;
            }

            _files = tagFiles;
            _compositions = compositions;
            _tagsv2 = new List<TagLib.Tag>();

            for (int compi = 0; compi < compositions.Count; compi++)
            {
                _tagsv2?.Add(tagFiles[compi]?.GetTag(TagLib.TagTypes.Id3v2));
            }
        }

        public static bool SourceIsVideo(string source)
        {
            if (System.IO.File.Exists(source) != true)
                return false;
            TagLib.File file = TagLib.File.Create(source);

            //if (mePlayer == null)
            foreach (ICodec codec in file.Properties.Codecs)
            {
                if (codec is TagLib.Mpeg.VideoHeader)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool SourceIsAudio(string source)
        {
            if (System.IO.File.Exists(source) != true)
                return false;
            TagLib.File file = TagLib.File.Create(source);

            //if (mePlayer == null)
            foreach (ICodec codec in file.Properties.Codecs)
            {
                if (codec is TagLib.Mpeg.AudioFile)
                {
                    return true;
                }
            }
            return false;
        }

        public static void AddTitleToCompositionsSourceFile(string title, Composition composition, Action<string> errorAction = null)
        {
            try
            {
                TagLib.File tagFile = TagLib.File.Create(composition.FilePath);
                var tagv2 = tagFile.GetTag(TagLib.TagTypes.Id3v2);
                //check if the title isn't null
                if (!string.IsNullOrEmpty(title)) {
                    string titleFromFile = tagv2.Title;
                    if (titleFromFile == null || !title.ToLower().Contains(titleFromFile.ToLower())) {
                        tagv2.Title = title;
                        tagFile.Save();
                    }
                }
            } catch (Exception ex) {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                errorAction?.Invoke(ex.Message);
            }
        }

        public static void AddArtistToCompositionsSourceFile(string artist, Composition composition, Action<string> errorAction = null)
        {
            try
            {
                TagLib.File tagFile = TagLib.File.Create(composition.FilePath);
                var tagv2 = tagFile.GetTag(TagLib.TagTypes.Id3v2);
                //check if the artist isn't null
                if (!string.IsNullOrEmpty(artist))
                {
                    string[] artistsFromFile = tagv2.Performers;

                    if (artistsFromFile == null || artistsFromFile.Count() == 0
                        || !artist.ToLower().Contains(artistsFromFile[0].ToLower()))
                    {
                        var newArtists = AddNewArtistOrReturnNull(artist, artistsFromFile);
                        tagv2.Performers = newArtists == null ? tagv2.Performers : newArtists;
                        tagFile.Save();
                    }

                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                errorAction?.Invoke(ex.Message);
            }
        }
        public static void AddYear(uint year, ref Composition composition, IDMDBContext dBContext, Action<string> errorAction = null)
        {
            try
            {
                TagLib.File tagFile = TagLib.File.Create(composition.FilePath);
                var tagv2 = tagFile.GetTag(TagLib.TagTypes.Id3v2);
                //check if the title isn't null
                    //check if the year isn't null
                if ((year < 2300) && (year > 900)) {
                    uint yearFromFile = tagv2.Year;
                    if (yearFromFile != year) {
                        tagFile.Tag.Year = year;
                        tagFile.Save();
                        if (composition.Album != null) {
                            composition.Album.Year = year;
                            dBContext.SaveChanges();
                        }
                    }
                }
            } catch (Exception ex) {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                errorAction?.Invoke(ex.Message);
            }
        }
        public void AddYearToAll(uint year, IDMDBContext dBContext, Action<string> setStatus = null)
        {
            try
            {
                for (int i = 0; i < _tagsv2.Count; i++) {
                    //check if the year isn't null
                    if (year != uint.MaxValue) {
                        uint yearFromFile = _tagsv2[i].Year;
                        if (yearFromFile != year) {
                            _tagsv2[i].Year = year;
                            _files[i].Save();
                            if (_compositions[i].Album != null) {
                                _compositions[i].Album.Year = year;
                                dBContext.SaveChanges();
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                SetCurrentStatus(ex.Message);
            }
        }

        public void AddTitleToAll(string title, IDMDBContext dBContext, Action<string> setStatus = null)
        {
            try
            { //check if the title isn't null
                if (!string.IsNullOrEmpty(title)) {
                    for (int i = 0; i < _compositions.Count; i++) {
                        string titleFromFile = _tagsv2[i].Title;
                        if (titleFromFile == null ||
                            !title.ToLower().Contains(title)) {
                            _tagsv2[i].Title = title;
                            _files[i].Save(); 
                            if (_compositions[i] != null) {
                                _compositions[i].CompositionName = title;
                                dBContext.SaveChanges();
                                if (setStatus != null)
                                    setStatus($"Title Successfully Changed: {title}");
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                setStatus(ex.Message);
            }
        }

        public void AddLyricsToAll(string lyrics, IDMDBContext dBContext, Action<string> setStatus = null)
        {
            try
            { //check if the title isn't null
                if (!string.IsNullOrEmpty(lyrics))
                {
                    for (int i = 0; i < _compositions.Count; i++)
                    {
                        string titleFromFile = _tagsv2[i].Title;
                        _tagsv2[i].Lyrics = lyrics;
                        _files[i].Save();
                        if (_compositions[i] != null)
                        {
                            _compositions[i].Lyrics= lyrics;
                            dBContext.SaveChanges();
                            if (setStatus != null)
                                setStatus($"Lyrics Successfully Added: {lyrics}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                setStatus(ex.Message);
            }
        }

        public void AddAlbumToAll(string artistName, string album,
            long? year, string label, string type, IDMDBContext dBContext, Action<string> errorAction = null)
        {
            try {
                for (int i = 0; i < _compositions.Count; i++) {
                    //step in if the album isn't null
                    if (!string.IsNullOrEmpty(album)) {
                        string albumFromFile = _tagsv2[i].Album;
                        if (albumFromFile == null || !album.ToLower().Contains(albumFromFile.ToLower() )) {
                            _tagsv2[i].Album = album;
                            _files[i].Save();
                        }
                        if (_compositions[i] != null) {
                            if (artistName != null) {
                                Artist foundArtist = DBAccess.GetFirstArtistIfExists(_compositions[0].Artist.ArtistName);
                                if (foundArtist == null) {
                                    foundArtist = DBAccess.AddArtist(artistName);
                                }
                                Album foundAlbum = DBAccess.GetFirstAlbumIfExists(artistName, album);
                                if (foundAlbum == null) {
                                    foundAlbum = DBAccess.AddAlbum(foundArtist.ArtistName, album, year, label, type);
                                }
                                _compositions[i].Album = foundAlbum;
                                _compositions[i].AlbumID = foundAlbum.AlbumID;
                                dBContext.SaveChanges();
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                errorAction?.Invoke(ex.Message);
            }
        }
        public void RemoveArtists(string artist)
        {
            //_tagsv2[i].Performers
            throw new NotImplementedException();
        }
        public void ChangeAllArtists(string artist, IDMDBContext dBContext, Action<string> errorAction = null)
        {
            for (int i = 0; i < _compositions.Count; i++) {
                try {
                    _files[i].GetTag(TagLib.TagTypes.Id3v2).Performers = GetNewArtistList(artist);
                    _files[i].Save();
                    if (_compositions[i] != null) {
                        Artist foundArtist = DBAccess.AddArtist(artist);

                        if (foundArtist != null) {
                            _compositions[i].Artist = foundArtist;
                            _compositions[i].ArtistID = foundArtist.ArtistID;
                            dBContext.SaveChanges();
                        }
                    }
                } catch (Exception ex) {
                    //System.Diagnostics.Debug.WriteLine(ex.Message);
                    errorAction?.Invoke(ex.Message);
                }
            }
        }

        public string[] GetNewArtistList(string artist)
        {
            return new string[] { artist };
        }

        public static string[] AddNewArtistOrReturnNull(string artist, string[] artistsFromFile, Action<string> errorAction = null)
        {
            try
            {
                string[] newArtists = new string[artistsFromFile.Length + 1];
                newArtists[0] = artist;
                artistsFromFile.CopyTo(newArtists, 1);
                return newArtists;
            } catch (Exception ex) {
                errorAction?.Invoke(ex.Message);
                return null;
            }
        }
        public void SetCurrentStatus(string status, Action<string> statusSetter = null)
        {
            try
            {
                if (statusSetter != null)
                    statusSetter(status);
            } catch (Exception ex) {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                statusSetter?.Invoke(ex.Message);
            }
        }
    }
}