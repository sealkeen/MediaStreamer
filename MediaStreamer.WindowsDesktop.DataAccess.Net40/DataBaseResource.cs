using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MediaStreamer.Domain;

namespace MediaStreamer.WindowsDesktop.DataAccess.Net40
{
    public class DBAccess : IDBRepository
    {
        public IDMDBContext DB { get; set; }

        public void OnStartup()
        {
            
        }

        public long GetNewCompositionID()
        {
            if (DB.GetCompositions().Count() > 0)
                return DB.GetCompositions().Max(c => c.CompositionID) + 1;
            else
                return 0;
        }

        public long GetNewArtistID()
        {
            if (DB.GetArtists().Count() > 0)
                return (DB.GetArtists().Max(a => a.ArtistID) + 1);
            else
                return 0;
        }

        public long GetNewAlbumID()
        {
            if (DB.GetAlbums().Count() > 0)
                return (DB.GetAlbums().Max(a => a.AlbumID) + 1);
            else
                return 0;
        }

        public long GetNewModeratorID()
        {
            if (DB.GetModerators().Count() > 0)
                return (DB.GetModerators().Max(a => a.ModeratorID) + 1);
            else
                return 0;
        }

        public long GetNewAdministratorID()
        {
            if (DB.GetAdministrators().Count() > 0)
                return (DB.GetAdministrators().Max(a => a.AdministratorID) + 1);
            else
                return 0;
        }

        public void PopulateDataBase( Action<string> errorAction = null )
        {
            try
            {
                //RemoveGroupMember("Being As An Ocean", 2011, null);
                //RemoveGroupMember("Delain", 2002, null);
                //RemoveGroupMember("Fifth Dawn", 2014, null);
                //RemoveGroupMember("Saviour", 2009, null);

                //AddGroupMember("August Burns Red", new DateTime(2003, 1, 1), null);//, dB);
                //AddGroupMember("Being As An Ocean", new DateTime(2011, 1, 1), null);//, dB);
                //AddGroupMember("Delain", new DateTime(2002, 1, 1), null);//, dB);
                //AddGroupMember("Fifth Dawn", new DateTime(2014, 1, 1), null);//, dB);
                //AddGroupMember("Saviour", new DateTime(2009, 1, 1), null);//, dB);

                //AddArtistGenre("August Burns Red", "metalcore");//, dB);
                //AddArtistGenre("Being As An Ocean", "melodic hardcore");//, dB);
                //AddArtistGenre("Being As An Ocean", "post-hardcore");//, dB);
                //AddArtistGenre("Delain", "symphonic metal");//, dB);
                //AddArtistGenre("Fifth Dawn", "alternative rock");//, dB);
                //AddArtistGenre("Saviour", "melodic hardcore");//, dB);

                //AddAlbum("August Burns Red", "Found In Far Away Places", 2015, "Fearless", "Studio");
                //AddAlbum("Being As An Ocean", "Waiting For Morning To Come (Deluxe Edition)", 2018, "SharpTone", "Studio");
                //AddAlbum("Being As An Ocean", "Waiting For Morning To Come", 2017, "SharpTone", "Studio");
                //AddAlbum("Delain", "April Rain", 2009, "Sensory", "Studio");
                //AddAlbum("Fifth Dawn", "Identity", 2018, "Dreambound", "Studio");
                //AddAlbum("Saviour", "Empty Skies", 2018, "Dreambound", "Studio");

                //AddComposition("August Burns Red", "Identity", "Found In Far Away Places", 259);
                //AddComposition("August Burns Red", "Marathon", "Found In Far Away Places", 286);
                //AddComposition("August Burns Red", "Martyr", "Found In Far Away Places", 275);
                //AddComposition("Being As An Ocean", "Alone", "Waiting For Morning To Come (Deluxe Edition)", 264);
                //AddComposition("Being As An Ocean", "Black & Blue", "Waiting For Morning To Come", 256);
                //AddComposition("Being As An Ocean", "Blacktop", "Waiting For Morning To Come", 296);
                //AddComposition("Being As An Ocean", "Dissolve", "Waiting For Morning To Come", 288);
                //AddComposition("Being As An Ocean", "Glow", "Waiting For Morning To Come", 314);
                //AddComposition("Being As An Ocean", "OK", "Waiting For Morning To Come", 256);
                //AddComposition("Being As An Ocean", "Thorns", "Waiting For Morning To Come", 230);
                //AddComposition("Being As An Ocean", "Waiting for Morning to Come", "Waiting For Morning To Come", 295);
                //AddComposition("Delain", "April Rain (Album Version)", "April Rain", 276);
                //AddComposition("Fifth Dawn", "Allure", "Identity", 288);
                //AddComposition("Fifth Dawn", "Element", "Identity", 288);
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
            }
        }
        public void RemoveGroupMember(string artistName, long formationDate,
            long? dateOfDisband = null)
        {
            //var q = from artist in dB.GetArtists() where artist.ArtistName == artistName select artist;
            //if (q.Count() == 0)
            //    return;
            //var art = q.First();
            //var member = new GroupMember() {
            //    Artist = art,
            //    ArtistName = art.ArtistName,
            //    ArtistID = art.ArtistID,
            //    DateOfDisband = dateOfDisband,
            //    GroupFormationDate = formationDate
            //};
            var artists = from a in DB.GetArtists() where a.ArtistName == artistName select a;

            if ( artists == null || artists.Count() == 0 )
                return;
            long artistID = artists.First().ArtistID;

            //TODO:
            //var queue = dB.GetGroupMembers().Where(x => x.ArtistID == artistID && x.GroupFormationDate == formationDate);

            //var gM = dB.GetGroupMembers().Find(artistID, formationDate);
            //if (gM != null)
            //    dB.GetGroupMembers().Remove(gM);
        }
        Random rnd = new Random();
        public GroupMember AddGroupMember(string artistName, DateTime formationDate,
            long? dateOfDisband = null
        //, FirstFMEntities DB = null
        )
        {
            var existing = from gRM in DB.GetGroupMembers()
                           where gRM.GroupFormationDate == formationDate
                           select gRM;

            if (existing.Count() != 0)
                return existing.First();

            var artists = from artist in DB.GetArtists() where (artist.ArtistName == artistName) select artist;
            if (artists.Count() != 0)
            {
                var firstArtist = artists.First();
                var gM = new GroupMember()
                {
                    //ArtistName = artistName,
                    ArtistID = firstArtist.ArtistID,
                    GroupFormationDate = formationDate,
                    DateOfDisband = null
                };
                DB.Add(gM);
                DB.SaveChanges();
                return gM;
            }
            return new GroupMember() { Artist = AddArtist("Unknown"), GroupFormationDate = DateTime.MinValue };
        }

        public Album AddAlbum(
            string artistName,
            string albumName,
            //long artistID = -1,
            //long groupFormationDate = -1,
            long? year = null,
            string label = null,
            string type = null
        )
        {
            try
            {
                var foundAlbums = DB.GetAlbums()
                    .Where( album => 
                    album.Artist.ArtistName.ToLower() == artistName.ToLower() && 
                    album.AlbumName.ToLower() == albumName.ToLower());

                if (foundAlbums.Count() != 0)
                    return foundAlbums.First();

                var foundArtist = GetFirstArtistIfExists(artistName);
                if (foundArtist == null)
                {
                    foundArtist = AddArtist(artistName);
                    DB.SaveChanges();
                }

                var artAlbs = foundArtist.Albums;
                if (artAlbs.Count() != 0)
                {
                    var targetAlbums = artAlbs.Where(a => a.AlbumName.ToLower() == albumName.ToLower());
                    if (targetAlbums.Count() != 0)
                    {
                        DB.RemoveEntity(targetAlbums.First());
                        DB.SaveChanges();
                    }
                }

                var GMmatches = GetPossibleGroupMembers(artistName);
                GroupMember targetGM;
                if (GMmatches.Count() == 0)
                {
                    targetGM = AddGroupMember(foundArtist.ArtistName, DateTime.MinValue, null);
                }
                else
                    targetGM = GMmatches.First();

                //Album alb = new Album() { ArtistID = targetArtist.ArtistID, AlbumName = albumName,
                //    ArtistName = artistName, GroupFormationDate = groupFormationDate, Label = label,
                //    Type = type, Year = year};

                var alb = new Album()
                {
                    AlbumID = GetNewAlbumID(),
                    Artist = foundArtist,
                    ArtistID = foundArtist.ArtistID,
                    AlbumName = albumName,
                    //ArtistName = targetArtist.ArtistName,
                    GroupFormationDate = targetGM.GroupFormationDate,
                    GroupMember = targetGM,
                    Label = label,
                    Type = type,
                    Year = year
                };

                DB.Add(alb);
                DB.SaveChanges();
                return alb;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns false if does not exist.
        /// </summary>
        /// <param name="artistName"></param>
        /// <returns></returns>
        public bool ArtistExists(string artistName)
        {
            var artistMatches = GetPossibleArtists(artistName);

            if (artistMatches.Count() == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Returns null if does not exist.
        /// </summary>
        /// <param name="artistName"></param>
        /// <returns></returns>
        public Artist GetFirstArtistIfExists(string artistName)
        {
            var artistMatches = GetPossibleArtists(artistName);

            if (artistMatches.Count() == 0)
                return null;
            return artistMatches.First();
        }
        public Genre GetFirstGenreIfExists(string artistName)
        {
            var genreMatches = GetPossibleGenres(artistName);

            if (genreMatches.Count() == 0)
                return null;
            return genreMatches.First();
        }
        /// <summary>
        /// Returns null if does not exist.
        /// </summary>
        /// <param name="artistName">Possible artist name.</param>
        /// <param name="albumName">Possible album name.</param>
        /// <returns></returns>
        public Album GetFirstAlbumIfExists(string artistName, string albumName)
        {
            var albumMatches = GetPossibleAlbums(artistName, albumName);

            if (albumMatches.Count() == 0)
                return null;
            return albumMatches.First();
        }

        /// <summary>
        /// Bad function, don't use it
        /// </summary>
        /// <param name="artistName"></param>
        /// <param name="compositionName"></param>
        /// <param name="albumName"></param>
        /// <param name="duration"></param>
        /// <param name="filePath"></param>
        public void AddComposition(
            string artistName,
            string compositionName,
            string albumName,
            long? duration = null,
            string filePath = null
        )
        {
            var art = new Artist();
            var artistMatches = GetPossibleArtists(artistName);

            if (artistMatches.Count() == 0)
            {
                try
                {
                    art.ArtistName = artistName;
                    DB.Add(art);
                    DB.SaveChanges();
                    artistMatches = GetPossibleArtists(artistName);
                }
                catch
                {
                    return;
                }
            }
            var targetArtist = artistMatches.First();

            var GMmatches = GetPossibleGroupMembers(artistName);

            if (GMmatches.Count() == 0)
            {
                try
                {
                    var gM = new GroupMember();
                    gM.ArtistID = GetFirstArtistIfExists(artistName).ArtistID;
                    //gM.ArtistName = artistName;
                    DB.Add(gM);
                    DB.SaveChanges();
                    GMmatches = GetPossibleGroupMembers(artistName);
                }
                catch
                {
                    return;
                }
            }

            var targetGM = GMmatches.First();

            //Album alb = new Album() { ArtistID = targetArtist.ArtistID, AlbumName = albumName,
            //    ArtistName = artistName, GroupFormationDate = groupFormationDate, Label = label,
            //    Type = type, Year = year};

            var alb = new Album()
            {
                ArtistID = targetArtist.ArtistID,
                AlbumName = albumName,
                //ArtistName = targetArtist.ArtistName,
                GroupFormationDate = targetGM.GroupFormationDate
            };

            var cmp = new Composition()
            {
                ArtistID = targetArtist.ArtistID,
                //ArtistName = targetArtist.ArtistName,
                GroupFormationDate = targetGM.GroupFormationDate,
                CompositionName = compositionName,
                Duration = duration,
                FilePath = filePath,
                //Album = alb,
                //Artist = art,
            };

            DB.Add(cmp);
        }

        public IQueryable<Artist> GetPossibleArtists(string name)
        {
            var matches = from match in DB.GetArtists() where match.ArtistName == name select match;

            return matches;
        }

        public IQueryable<Genre> GetPossibleGenres(string name)
        {
            var matches = from match in DB.GetGenres()
                          where (match.GenreName == name)
                          select match;

            return matches;
        }

        public IQueryable<Album> GetPossibleAlbums(long artistID, string albumName)
        {
            var matches = from match in DB.GetAlbums()
                          where (match.AlbumName == albumName &&
                          match.ArtistID == artistID)
                          select match;

            return matches;
        }

        public IQueryable<Album> GetPossibleAlbums(string artistName, string albumName)
        {
            var result = from album in DB.GetAlbums()
                         join artist in DB.GetArtists()
                        on album.ArtistID equals artist.ArtistID
                         where ((artist.ArtistName == artistName) &&
                     (album.AlbumName == albumName))
                         select album;

            //var debugAlbumGenres = (from aG in dB.AlbumGenres select aG).ToList();

            return result;
        }

        public IQueryable<GroupMember> GetPossibleGroupMembers(long artistID)
        {
            var matches = from match in DB.GetGroupMembers() where match.ArtistID == artistID select match;

            return matches;
        }

        public IQueryable<GroupMember> GetPossibleGroupMembers(string artistName)
        {
            var firstArtist = GetFirstArtistIfExists(artistName);
            if (firstArtist == null)
                return null;
            long id = firstArtist.ArtistID;
            var matches = from match in DB.GetGroupMembers() where match.ArtistID == id select match;
            return matches;
        }

        public bool ContainsArtist(string artistName, List<Artist> artists)
        {
            foreach (var artist in artists)
                if (artist.ArtistName == artistName)
                    return true;
            return false;
        }

        public void AddArtistGenre(string artistName, string genreName,
            long? dateOfDisband = null,
            Action<string> errorAction = null
        )
        {
            try
            {
                if (genreName == null || genreName == string.Empty)
                {
                    errorAction?.Invoke("AddArtistGenre exception : genreName == null");
                    return;
                }
                var firstArtist = GetFirstArtistIfExists(artistName);

                if (firstArtist == null)
                {
                    errorAction?.Invoke($"AddArtistGenre exception : no matching artist exist <{artistName}>.");
                    return;
                }

                var newAGenre = new ArtistGenre() { GenreName = genreName };
                firstArtist = DB.GetArtists().First(x => x.ArtistID == firstArtist.ArtistID);
                //todo: check for changes
                //newAGenre.Genre = dB.Genres.Find(genreName);
                newAGenre.Genre = GetFirstGenreIfExists(genreName);

                if (newAGenre.Genre == null)
                    newAGenre.Genre = new Genre { GenreName = genreName };

                GetFirstArtistIfExists(firstArtist.ArtistName).ArtistGenres.Add(newAGenre);
                return;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke($"{ex.Message}");
            }
        }

        public bool ArtistHasGenre(Artist artist, string possibleGenre)
        {
            var genres = from gen in artist.ArtistGenres where gen.GenreName == possibleGenre select gen;

            if (genres.Count() == 0)
                return false;
            return true;
        }

        public void Update()
        {
            if (DB == null)
            {
                //dB.Dispose();
                //dB = null;
                DB = new DMEntities();
            }
        }

        public string ToMD5(string source)
        {
            var buffer = Encoding.Default.GetBytes(source);

            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(buffer);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public Artist AddArtist(string artistFileName,
            Action<string> errorAction = null)
        {
            try
            {
                Artist artistToAdd;
                if (artistFileName == null)
                {
                    artistToAdd = GetFirstArtistIfExists("unknown");
                    if (artistToAdd == null)
                    {
                        DB.Add(artistToAdd = new Artist() { ArtistName = "unknown", ArtistID = GetNewArtistID() });
                    }
                    return artistToAdd;
                }
                artistToAdd = GetFirstArtistIfExists(artistFileName);
                if (artistToAdd == null)
                {
                    artistToAdd = new Artist() { ArtistName = artistFileName };
                    try
                    {
                        artistToAdd.ArtistID = GetNewArtistID();
                    }
                    catch
                    {
                        errorAction?.Invoke("Aquiring new artist ID Exception raised.");
                        //return null;
                    }

                    DB.Add(artistToAdd);
                    DB.SaveChanges();
                }

                return artistToAdd;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return null;
            }
        }

        public Genre AddGenre(Artist artist, string newGenre)
        {
            var genre = GetFirstGenreIfExists("unknown");
            // TODO: Return and fix "find"
            if (genre == null)
            {
                genre = new Genre() { GenreName = "unknown" };
            }

            if (newGenre != null)
            {
                // genre tag is valid
                var foundGenre = GetFirstGenreIfExists(newGenre);

                if (foundGenre == null)
                {
                    // if genre is new
                    genre = new Genre();
                    genre.GenreName = newGenre;

                    var artG = new ArtistGenre()
                    {
                        Artist = artist,
                        ArtistID = artist.ArtistID,
                        DateOfApplication = DateTime.Now,
                        Genre = genre,
                        GenreName = genre.GenreName
                    };

                    DB.Add(artG);
                    DB.Add(genre);

                    DB.SaveChanges();
                }
                else
                {
                    genre = foundGenre;
                }
            }

            return genre;
        }

        public Album AddAlbum(
            Artist artist, Genre genre, string albumFromFile,
            string label = null, DateTime? gFD = null,
            string type = null, long? year = null)
        {
            Album albumToAdd;
            var foundAlbum = GetFirstAlbumIfExists(artist.ArtistName, albumFromFile);

            if (albumFromFile == null || albumFromFile == string.Empty)
            {
                // album tag is not recognized
                // var albumFound = dB.GetAlbums().Find("unknown");

                if (artist.ArtistName == null || artist.ArtistName == string.Empty)
                {
                    //todo:
                }
                var unknownAlbums = from unknownAlbum in DB.GetAlbums()
                                    where unknownAlbum.AlbumName == "unknown" &&
                                    unknownAlbum.Artist.ArtistID == artist.ArtistID
                                    select unknownAlbum;

                if (unknownAlbums.Count() == 0)
                {
                    albumToAdd = new Album()
                    {
                        AlbumName = "unknown",
                        AlbumID = GetNewAlbumID(),
                        Artist = artist,
                        ArtistID = artist.ArtistID,
                        Genre = genre,
                        GenreName = genre.GenreName,
                        Label = label,
                        GroupFormationDate = gFD,
                        Type = type,
                        Year = year
                    };
                    DB.Add(albumToAdd);
                }
                else
                {
                    albumToAdd = unknownAlbums.First();
                }
            }
            else
            {
                // album tag is recognized 
                if ((foundAlbum == null))
                {
                    // the album is new
                    albumToAdd = new Album()
                    { //ArtistName = artistFileName,
                        Genre = genre,
                        GenreName = genre.GenreName,
                        Artist = artist,
                        AlbumName = albumFromFile,
                        GroupFormationDate = DateTime.MinValue,
                        ArtistID = (artist.ArtistID),
                        AlbumID = GetNewAlbumID()
                    };

                    var albG = new AlbumGenre()
                    {
                        Album = albumToAdd,
                        Genre = genre,
                        Artist = artist,
                        AlbumID = albumToAdd.AlbumID,
                        ArtistID = artist.ArtistID,
                        GenreName = genre.GenreName,
                        DateOfApplication = DateTime.Now
                    };

                    DB.Add(albumToAdd);
                    DB.SaveChanges();
                    DB.Add(albG);
                    DB.SaveChanges();
                }
                else
                {
                    // album is found in DB
                    albumToAdd = foundAlbum;
                }
            }
            return albumToAdd;
        }

        /// <summary>
        /// This method removes the existing composition and add a new one for its place
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <param name="title"></param>
        /// <param name="duration"></param>
        /// <param name="fileName"></param>
        /// <param name="yearFromFile"></param>
        /// <param name="onlyReturnNoAppend"></param>
        /// <returns></returns>
        public Composition AddComposition(Artist artist, Album album,
            string title, TimeSpan duration,
            string fileName, long? yearFromFile = null,
            bool onlyReturnNoAppend = false,
            Action<string> errorAction = null)
        {
            try
            {
                var newComposition = new Composition();
                var existing = from comp in DB.GetCompositions()
                               where (comp.CompositionName == title) &&
                               (comp.Album.AlbumName == album.AlbumName) &&
                               (comp.Artist.ArtistName == artist.ArtistName)
                               select comp;

                if (existing.Count() != 0)
                {
                    var existingComp = existing.First();
                    ChangeExistingComposition(artist, album, title, duration, fileName,
                        onlyReturnNoAppend, newComposition, existingComp, errorAction);
                    return existingComp;
                }
                //cmp.AlbumName = alm.AlbumName;
                //cmp.ArtistName = art.ArtistName;

                if (title != null)
                {
                    try
                    {
                        newComposition.CompositionID = GetNewCompositionID();
                        newComposition.CompositionName = title;
                        newComposition.ArtistID = artist.ArtistID;
                        newComposition.AlbumID = album.AlbumID;
                        newComposition.Album = album;
                        newComposition.Artist = artist;
                        newComposition.GroupMember = AddGroupMember(artist.ArtistName, DateTime.Now);
                        try
                        {
                            newComposition.Duration = (long?)duration.TotalSeconds;
                            newComposition.FilePath = fileName;
                        }
                        catch
                        {
                            //leave them null
                        }

                        DB.Add(newComposition);
                        DB.SaveChanges();
                        return newComposition;
                    }
                    catch (Exception ex)
                    {
                        errorAction?.Invoke(ex.Message);
                        return null;
                    }
                }
                else
                {
                    errorAction?.Invoke("title is null");
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// This is not working because we can't change the existing data inside of a DBTable.
        /// I don't know how to fix it.
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <param name="title"></param>
        /// <param name="duration"></param>
        /// <param name="fileName"></param>
        /// <param name="onlyReturnNoAppend"></param>
        /// <param name="newComposition"></param>
        /// <param name="existingComp"></param>
        /// <returns></returns>
        public void ChangeExistingComposition(Artist artist,
            Album album, string title, TimeSpan duration, string fileName,
            bool onlyReturnNoAppend, Composition newComposition, 
            Composition existingComp, Action<string> errorAction)
        {
            try
            {
                //todo: return and complete this
                //todo: figure out why we can't change en entity

                //return existingComp;

                CopyFieldsExceptForDurationAndPath(existingComp, newComposition);

                existingComp.CompositionName = title;
                existingComp.ArtistID = artist.ArtistID;
                existingComp.AlbumID = album.AlbumID;
                existingComp.Album = album;
                existingComp.Artist = artist;

                existingComp.Duration = (long?)duration.TotalSeconds;
                existingComp.FilePath = fileName;

                //todo:

                //existingComp.Duration = (long?)duration.TotalSeconds;
                //existingComp.FilePath = fileName;
                //existingComp.CompositionID = GetNewCompositionID();
                //return existingComp;
                if (!onlyReturnNoAppend)
                    DB.SaveChanges();
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
            }
        }

        public ListenedComposition FindFirstListenedComposition(Composition composition)
        {
            var matches = from lC in DB.GetListenedCompositions()
                          where lC.CompositionID == composition.CompositionID
                          select lC;
            if (matches.Count() == 0)
                return null;
            return matches.First();
        }

        public void AddNewListenedComposition(Composition composition, User user,
            Action<string> errorAction = null)
        {
            try
            {
                var existingComps = DB.GetListenedCompositions().Where(c => c.CompositionID ==
                composition.CompositionID && user.UserID == c.UserID);
                if (existingComps != null &&
                    existingComps.Count() != 0)
                {
                    var last = existingComps.First();
                    last.CountOfPlays += 1;
                    DB.SaveChanges();
                    return;
                }
                /*public long*/
                var UserID = user.UserID;
                //*public long*/ 
                var ArtistID = composition.ArtistID;
                //*public System.DateTime*/ 
                var GroupFormationDate = composition.GroupFormationDate;
                //*public long*/ 
                var AlbumID = composition.AlbumID;
                //*public long*/ 
                var CompositionID = composition.CompositionID;
                Album album;
                var lC = new ListenedComposition()
                {
                    Album = (album = composition.Album == null ? AddAlbum(composition.Artist.ArtistName, "Unknown") : composition.Album),
                    AlbumID = album.AlbumID,
                    Artist = composition.Artist,
                    ArtistID = composition.ArtistID.Value,
                    Composition = composition,
                    CompositionID = composition.CompositionID,
                    CountOfPlays = 1,
                    GroupFormationDate = composition.GroupFormationDate == null ? DateTime.MinValue : composition.GroupFormationDate.Value,
                    GroupMember = composition.GroupMember,
                    ListenDate = DateTime.Now,
                    User = user,
                    UserID = user.UserID
                };

                DB.Add(lC);
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
            }
        }

        public void CopyFieldsExceptForDurationAndPath(Composition existingComp, Composition comp)
        {
            comp.About = existingComp.About; comp.Album = existingComp.Album;
            comp.AlbumID = existingComp.AlbumID; comp.Artist = existingComp.Artist;
            comp.ArtistID = existingComp.ArtistID; comp.CompositionName = existingComp.CompositionName;
        }

        public bool HasAdminRights(User user,
            Action<string> errorAction = null)
        {
            //var matches = from user in dB.Administrators join 
            try
            {
                var adminQuery = from u in DB.GetUsers()
                                 join a in DB.GetAdministrators()
                                 on u.UserID equals a.UserID
                                 where a.UserID == user.UserID
                                 select u;

                if (adminQuery.Count() > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
            }
            return false;
        }

        public bool HasModerRights(User user,
            Action<string> errorAction = null)
        {
            //var matches = from user in dB.Administrators join 
            try
            {
                var moderQuery = from u in DB.GetUsers()
                                 join m in DB.GetModerators()
                                 on u.UserID equals m.UserID
                                 where m.UserID == user.UserID
                                 select u;

                if (moderQuery.Count() > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
            }
            return false;
        }

        public User AddNewUser(string login, string psswd,
            string email, string bio,
            string VKLink = "null", string FaceBookLink = "null",
            Action<string> errorAction = null)
        {
            DateTime lastListenedDataModificationDate = DateTime.MinValue;
            //DateTime 

            try
            {
                var user = new User();
                long id = 0;
                try
                {
                    id = DB.GetUsers().Max(u => u.UserID) + 1;
                    // skip catching exception and leave the default value of 0
                }
                catch { }

                //user.UserID = id;
                user.UserName = login;
                user.Email = email;
                user.Password = ToMD5(psswd);
                user.DateOfSignUp = DateTime.Now;
                user.Bio = bio;

                user.VKLink = "null";
                user.FaceBookLink = "null";
                user.UserID = id;

                DB.Add(user);
                DB.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return null;
            }
        }

        public Moderator AddNewModerator(long userID,
            Action<string> errorAction = null)
        {
            try
            {
                var user = DB.GetUsers().First(u => u.UserID == userID);
                if (user == null)
                    return null;
                var moders = DB.GetModerators().Where(m => m.UserID == userID);
                if (moders.Count() != 0)
                {
                    return moders.First();
                }

                var moderator = new Moderator();
                moderator.UserID = userID;
                moderator.ModeratorID = GetNewModeratorID();

                DB.Add(moderator);
                DB.SaveChanges();
                return moderator;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return null;
            }
        }

        public Administrator AddNewAdministrator(long userID, long moderID,
            Action<string> errorAction = null)
        {
            try
            {
                var user = DB.GetUsers().First(x => x.UserID == userID);
                if (user == null)
                    return null;

                var moders = DB.GetModerators().First(x => x.ModeratorID == moderID);
                if (moders == null)
                    return null;

                var admins = DB.GetAdministrators().Where(a => a.UserID == userID);
                if (admins.Count() != 0)
                {
                    return admins.First();
                }

                var administrator = new Administrator();
                administrator.UserID = userID;
                administrator.ModeratorID = moderID;
                administrator.AdministratorID = GetNewAdministratorID();

                DB.Add(administrator);
                DB.SaveChanges();
                return administrator;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return null;
            }
        }

        public bool DeleteComposition(long ID,
            Action<string> errorAction = null)
        {
            try
            {
                DB.RemoveEntity(DB.GetCompositions().First(x => x.CompositionID == ID));
                DB?.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return false;
            }
        }

        public bool DeleteComposition(Composition composition,
            Action<string> errorAction = null)
        {
            try
            {
                DB?.RemoveEntity(DB.GetCompositions().First(x => x.CompositionID == composition.CompositionID));
                DB?.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return false;
            }
        }

        public bool DeleteListenedComposition(ListenedComposition composition,
            Action<string> errorAction = null)
        {
            try
            {
                var matches = DB.GetListenedCompositions().Where(c => c.ListenDate == composition.ListenDate && c.UserID == composition.UserID);
                if (matches.Count() != 0)
                {
                    var countOfPlays = matches.First().CountOfPlays;
                    DB.RemoveEntity(matches.First());
                    DB.SaveChanges();
                    var newMatches = DB.GetListenedCompositions().Where(c => c.ListenDate == composition.ListenDate && 
                    c.UserID == composition.UserID &&
                    composition.CompositionID == c.CompositionID 
                    );
                    if (newMatches.Count() != 0) {
                        newMatches.First().CountOfPlays+= countOfPlays;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return false;
            }
        }
        public bool DeleteAlbum(long ID, Action<string> errorAction = null) {
            try
            {
                DB.RemoveEntity(DB.GetAlbums().First(x => x.AlbumID == ID));
                DB?.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return false;
            }
        }
        public bool DeleteAlbum(Album album, Action<string> errorAction = null) {
            try
            {
                DB?.RemoveEntity(DB.GetAlbums().First(x => x.AlbumID == album.AlbumID));
                DB?.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
                return false;
            }
        }
        public ListenedArtist CreateNewListenedArtist()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ListenedComposition> GetCurrentUsersListenedCompositions(User user)
        {
            return from comp in DB.GetListenedCompositions()
                   where comp.UserID == user.UserID
                   select comp;
        }

        public IQueryable<ListenedComposition> GetCurrentUsersListenedGenres(User user)
        {
            return from comp in DB.GetListenedCompositions()
                   where comp.UserID == user.UserID
                   select comp;
        }

        public IQueryable<ListenedComposition> GetCurrentUsersListenedArtist(User user)
        {
            return from comp in DB.GetListenedCompositions()
                   where comp.UserID == user.UserID
                   select comp;
        }
    }
}
