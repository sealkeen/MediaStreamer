using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DataBaseResource
{
    public static class DBAccess
    {
        public static DMEntities dB;

        public static long GetNewCompositionID()
        {
            if (dB.Compositions.Count() > 0)
                return (dB.Compositions.Max(c => c.CompositionID) + 1);
            else
                return 0;
        }

        public static long GetNewArtistID()
        {
            if (DBAccess.dB.Artists.Count() > 0)
                return (DBAccess.dB.Artists.Max(a => a.ArtistID) + 1);
            else
                return 0;
        }

        public static long GetNewAlbumID()
        {
            if (DBAccess.dB.Albums.Count() > 0)
                return (DBAccess.dB.Albums.Max(a => a.AlbumID) + 1);
            else
                return 0;
        }

        public static long GetNewModeratorID()
        {
            if (DBAccess.dB.Moderators.Count() > 0)
                return (DBAccess.dB.Moderators.Max(a => a.ModeratorID) + 1);
            else
                return 0;
        }

        public static long GetNewAdministratorId()
        {
            if (DBAccess.dB.Administrators.Count() > 0)
                return (DBAccess.dB.Administrators.Max(a => a.AdministratorID) + 1);
            else
                return 0;
        }

        public static void PopulateDataBase()
        {
            try
            {
                //using () {

                RemoveGroupMember("Being As An Ocean", 2011, null);
                RemoveGroupMember("Delain", 2002, null);
                RemoveGroupMember("Fifth Dawn", 2014, null);
                RemoveGroupMember("Saviour", 2009, null);

                AddGroupMember("August Burns Red", new DateTime(2003, 1, 1), null);//, dB);
                AddGroupMember("Being As An Ocean", new DateTime(2011, 1, 1), null);//, dB);
                AddGroupMember("Delain", new DateTime(2002, 1, 1), null);//, dB);
                AddGroupMember("Fifth Dawn", new DateTime(2014, 1, 1), null);//, dB);
                AddGroupMember("Saviour", new DateTime(2009, 1, 1), null);//, dB);

                AddArtistGenre("August Burns Red", "metalcore");//, dB);
                AddArtistGenre("Being As An Ocean", "melodic hardcore");//, dB);
                AddArtistGenre("Being As An Ocean", "post-hardcore");//, dB);
                AddArtistGenre("Delain", "symphonic metal");//, dB);
                AddArtistGenre("Fifth Dawn", "alternative rock");//, dB);
                AddArtistGenre("Saviour", "melodic hardcore");//, dB);

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
                dB.SaveChanges();
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public static void RemoveGroupMember(string artistName, long formationDate,
            long? dateOfDisband = null)
        {
            //var q = from artist in dB.Artists where artist.ArtistName == artistName select artist;
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
            var artists = from a in dB.Artists where a.ArtistName == artistName select a;

            if (artists == null || artists.Count() == 0)
                return;
            long artistID = artists.First().ArtistID;

            var gM = dB.GroupMembers.Find(artistID, formationDate);
            if (gM != null)
                dB.GroupMembers.Remove(gM);
        }

        public static void AddGroupMember(string artistName, DateTime formationDate,
            long? dateOfDisband = null
        //, FirstFMEntities dB = null
        )
        {
            var existing = from gRM in dB.GroupMembers
                           where gRM.GroupFormationDate == formationDate
                           select gRM;

            if (existing.Count() != 0)
                return;

            var artists = from artist in dB.Artists where (artist.ArtistName == artistName) select artist;
            if (artists.Count() != 0)
            {
                var firstArtist = artists.First();
                GroupMember gM = new GroupMember()
                {
                    //ArtistName = artistName,
                    ArtistID = firstArtist.ArtistID,
                    GroupFormationDate = formationDate,
                    DateOfDisband = null
                };
                dB.GroupMembers.Add(gM);
            }
        }

        public static Album AddAlbum(
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
                Artist foundArtist = GetFirstArtistIfExists(artistName);
                if (foundArtist == null)
                {
                    foundArtist = AddArtist(artistName);
                    dB.SaveChanges();
                }

                var GMmatches = GetPossibleGroupMembers(artistName);

                if (GMmatches.Count() == 0)
                    return null;

                GroupMember targetGM = GMmatches.First();

                //Album alb = new Album() { ArtistID = targetArtist.ArtistID, AlbumName = albumName,
                //    ArtistName = artistName, GroupFormationDate = groupFormationDate, Label = label,
                //    Type = type, Year = year};

                Album alb = new Album()
                {
                    ArtistID = foundArtist.ArtistID,
                    AlbumName = albumName,
                    //ArtistName = targetArtist.ArtistName,
                    GroupFormationDate = targetGM.GroupFormationDate,
                    Label = label,
                    Type = type,
                    Year = year,
                };

                dB.Albums.Add(alb);
                dB.SaveChanges();
                return alb;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns false if does not exist.
        /// </summary>
        /// <param name="artistName"></param>
        /// <returns></returns>
        public static bool ArtistExists(string artistName)
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
        public static Artist GetFirstArtistIfExists(string artistName)
        {
            var artistMatches = GetPossibleArtists(artistName);

            if (artistMatches.Count() == 0)
                return null;
            return artistMatches.First();
        }
        public static Genre GetFirstGenreIfExists(string artistName)
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
        public static Album GetFirstAlbumIfExists(string artistName, string albumName)
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
        public static void AddComposition(
            string artistName,
            string compositionName,
            string albumName,
            long? duration = null,
            string filePath = null
        )
        {
            Artist art = new Artist();
            var artistMatches = GetPossibleArtists(artistName);

            if (artistMatches.Count() == 0)
            {
                try
                {
                    art.ArtistName = artistName;
                    dB.Artists.Add(art);
                    dB.SaveChanges();
                    artistMatches = GetPossibleArtists(artistName);
                }
                catch
                {
                    return;
                }
            }
            Artist targetArtist = artistMatches.First();

            var GMmatches = GetPossibleGroupMembers(artistName);

            if (GMmatches.Count() == 0)
            {
                try
                {
                    GroupMember gM = new GroupMember();
                    gM.ArtistID = GetFirstArtistIfExists(artistName).ArtistID;
                    //gM.ArtistName = artistName;
                    dB.GroupMembers.Add(gM);
                    dB.SaveChanges();
                    GMmatches = GetPossibleGroupMembers(artistName);
                }
                catch
                {
                    return;
                }
            }

            GroupMember targetGM = GMmatches.First();

            //Album alb = new Album() { ArtistID = targetArtist.ArtistID, AlbumName = albumName,
            //    ArtistName = artistName, GroupFormationDate = groupFormationDate, Label = label,
            //    Type = type, Year = year};

            Album alb = new Album()
            {
                ArtistID = targetArtist.ArtistID,
                AlbumName = albumName,
                //ArtistName = targetArtist.ArtistName,
                GroupFormationDate = targetGM.GroupFormationDate
            };

            Composition cmp = new Composition()
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

            dB.Compositions.Add(cmp);
        }

        public static IQueryable<Artist> GetPossibleArtists(string name)
        {
            var matches = from match in dB.Artists where match.ArtistName == name select match;

            return matches;
        }

        public static IQueryable<Genre> GetPossibleGenres(string name)
        {
            var matches = from match in dB.Genres
                          where (match.GenreName == name)
                          select match;

            return matches;
        }

        public static IQueryable<Album> GetPossibleAlbums(long artistID, string albumName)
        {
            var matches = from match in dB.Albums
                          where (match.AlbumName == albumName &&
                          match.ArtistID == artistID)
                          select match;

            return matches;
        }

        public static IQueryable<Album> GetPossibleAlbums(string artistName, string albumName)
        {
            var result = from album in dB.Albums
                         join artist in dB.Artists
 on album.ArtistID equals artist.ArtistID
                         where ((artist.ArtistName == artistName) &&
                      (album.AlbumName == albumName))
                         select album;

            //var debugAlbumGenres = (from aG in dB.AlbumGenres select aG).ToList();

            return result;
        }

        public static IQueryable<GroupMember> GetPossibleGroupMembers(long artistID)
        {
            var matches = from match in dB.GroupMembers where match.ArtistID == artistID select match;

            return matches;
        }

        public static IQueryable<GroupMember> GetPossibleGroupMembers(string artistName)
        {
            var firstArtist = GetFirstArtistIfExists(artistName);
            if (firstArtist == null)
                return null;
            long id = firstArtist.ArtistID;
            var matches = from match in dB.GroupMembers where match.ArtistID == id select match;
            return matches;
        }

        public static bool ContainsArtist(string artistName, List<Artist> artists)
        {
            foreach (Artist artist in artists)
                if (artist.ArtistName == artistName)
                    return true;
            return false;
        }

        public static void AddArtistGenre(string artistName, string genreName,
            long? dateOfDisband = null //, FirstFMEntities dB = null
        )
        {
            try
            {
                if (genreName == null || genreName == string.Empty)
                {
                    Debug.WriteLine("AddArtistGenre exception : genreName == null");
                    return;
                }
                Artist firstArtist = GetFirstArtistIfExists(artistName);

                if (firstArtist == null)
                {
                    Debug.WriteLine($"AddArtistGenre exception : no matching artist exist <{artistName}>.");
                    return;
                }

                ArtistGenre newAGenre = new ArtistGenre() { GenreName = genreName };
                firstArtist = dB.Artists.Find(firstArtist.ArtistID);
                //todo: check for changes
                //newAGenre.Genre = dB.Genres.Find(genreName);
                newAGenre.Genre = DBAccess.GetFirstGenreIfExists(genreName);

                if (newAGenre.Genre == null)
                    newAGenre.Genre = new Genre { GenreName = genreName };

                GetFirstArtistIfExists(firstArtist.ArtistName).ArtistGenres.Add(newAGenre);
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        public static bool ArtistHasGenre(Artist artist, string possibleGenre)
        {
            var genres = from gen in artist.ArtistGenres where gen.GenreName == possibleGenre select gen;

            if (genres.Count() == 0)
                return false;
            return true;
        }

        public static void Update()
        {
            if (dB == null)
            {
                //dB.Dispose();
                //dB = null;
                dB = new DMEntities();
            }
        }

        public static string ToMD5(string source)
        {
            byte[] tmpSource; byte[] tmpHash;

            //Create a byte array from source data.
            tmpSource = ASCIIEncoding.ASCII.GetBytes(source);
            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            string hashed = System.Text.Encoding.Default.GetString(tmpHash);
            return hashed;
        }

        public static Artist AddArtist(string artistFileName)
        {
            try
            {
                Artist artistToAdd;
                if (artistFileName == null)
                {
                    artistToAdd = GetFirstArtistIfExists("unknown");
                    if (artistToAdd == null)
                    {
                        dB.Artists.Add(artistToAdd = new Artist() { ArtistName = "unknown", ArtistID = GetNewArtistID() });
                    }
                    return artistToAdd;
                }
                artistToAdd = DBAccess.GetFirstArtistIfExists(artistFileName);
                if (artistToAdd == null)
                {
                    artistToAdd = new Artist() { ArtistName = artistFileName };
                    try
                    {
                        artistToAdd.ArtistID = DBAccess.GetNewArtistID();
                    }
                    catch
                    {
                        Debug.WriteLine("Aquiring new artist ID Exception raised.");
                        //return null;
                    }

                    DBAccess.dB.Artists.Add(artistToAdd);
                    DBAccess.dB.SaveChanges();
                }

                return artistToAdd;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static Genre AddGenre(Artist artist, string newGenre)
        {
            Genre genre = DBAccess.GetFirstGenreIfExists("unknown");
            // TODO: Return and fix "find"
            if (genre == null)
            {
                genre = new Genre() { GenreName = "unknown" };
            }

            if (newGenre != null)
            {
                // genre tag is valid
                Genre foundGenre = DBAccess.GetFirstGenreIfExists(newGenre);

                if (foundGenre == null)
                {
                    // if genre is new
                    genre = new Genre();
                    genre.GenreName = newGenre;

                    ArtistGenre artG = new ArtistGenre()
                    {
                        Artist = artist,
                        ArtistID = artist.ArtistID,
                        DateOfApplication = DateTime.Now,
                        Genre = genre,
                        GenreName = genre.GenreName
                    };

                    DBAccess.dB.ArtistGenres.Add(artG);
                    DBAccess.dB.Genres.Add(genre);

                    DBAccess.dB.SaveChanges();
                }
                else
                {
                    genre = foundGenre;
                }
            }

            return genre;
        }

        public static Album AddAlbum(
            Artist artist, Genre genre, string albumFromFile,
            string label = null, DateTime? gFD = null,
            string type = null, long? year = null)
        {
            Album albumToAdd;
            Album foundAlbum = GetFirstAlbumIfExists(artist.ArtistName, albumFromFile);

            if (albumFromFile == null || albumFromFile == string.Empty)
            {
                // album tag is not recognized
                // var albumFound = dB.Albums.Find("unknown");

                if (artist.ArtistName == null || artist.ArtistName == string.Empty)
                {
                    //todo:
                }
                var unknownAlbums = from unknownAlbum in dB.Albums
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
                    dB.Albums.Add(albumToAdd);
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
                        AlbumID = DBAccess.GetNewAlbumID()
                    };

                    AlbumGenre albG = new AlbumGenre()
                    {
                        Album = albumToAdd,
                        Genre = genre,
                        Artist = artist,
                        AlbumID = albumToAdd.AlbumID,
                        ArtistID = artist.ArtistID,
                        GenreName = genre.GenreName,
                        DateOfApplication = DateTime.Now
                    };

                    dB.Albums.Add(albumToAdd);
                    dB.SaveChanges();
                    dB.AlbumGenres.Add(albG);
                    dB.SaveChanges();
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
        public static Composition AddComposition(Artist artist, Album album,
            string title, TimeSpan duration,
            string fileName, long? yearFromFile = null,
            bool onlyReturnNoAppend = false)
        {
            try
            {
                Composition newComposition = new Composition();
                var existing = from comp in dB.Compositions
                               where (comp.CompositionName == title) &&
                               (comp.Album.AlbumName == album.AlbumName) &&
                               (comp.Artist.ArtistName == artist.ArtistName)
                               select comp;

                if (existing.Count() != 0)
                {
                    var existingComp = existing.First();
                    dB.Compositions.Remove(existingComp);

                    //return ChangeExistingComposition(artist, album, title, duration, fileName, onlyReturnNoAppend, newComposition, existingComp);
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

                        try
                        {
                            newComposition.Duration = (long?)duration.TotalSeconds;
                            newComposition.FilePath = fileName;
                        }
                        catch
                        {
                            //leave them null
                        }

                        dB.Compositions.Add(newComposition);
                        dB.SaveChanges();
                        return newComposition;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return null;
                    }
                }
                else
                {
                    // title is null
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
        private static Composition ChangeExistingComposition(Artist artist, Album album, string title, TimeSpan duration, string fileName, bool onlyReturnNoAppend, Composition newComposition, Composition existingComp)
        {
            //todo: return and complete this
            //todo: figure out why we can't change en entity

            //return existingComp;

            CopyFieldsExceptForDurationAndPath(existingComp, newComposition);

            newComposition.CompositionName = title;
            newComposition.ArtistID = artist.ArtistID;
            newComposition.AlbumID = album.AlbumID;
            newComposition.Album = album;
            newComposition.Artist = artist;

            newComposition.Duration = (long?)duration.TotalSeconds;
            newComposition.FilePath = fileName;
            newComposition.CompositionID = GetNewCompositionID();
            dB.Compositions.Add(newComposition);

            //existingComp.Duration = (long?)duration.TotalSeconds;
            //existingComp.FilePath = fileName;
            //existingComp.CompositionID = GetNewCompositionID();
            //return existingComp;
            if (!onlyReturnNoAppend)
                dB.SaveChanges();
            return newComposition;
        }

        public static ListenedComposition FindFirstListenedComposition(Composition composition)
        {
            var matches = from lC in dB.ListenedCompositions
                          where lC.CompositionID == composition.CompositionID
                          select lC;
            if (matches.Count() == 0)
                return null;
            return matches.First();
        }

        public static void AddNewListenedComposition(Composition composition)
        {
            try
            {
                /*public long*/
                var UserID = SessionInformation.CurrentUser.UserID;
                //*public long*/ 
                var ArtistID = composition.ArtistID;
                //*public System.DateTime*/ 
                var GroupFormationDate = composition.GroupFormationDate;
                //*public long*/ 
                var AlbumID = composition.AlbumID;
                //*public long*/ 
                var CompositionID = composition.CompositionID;

                ListenedComposition lC = new ListenedComposition()
                {
                    Album = composition.Album,
                    AlbumID = composition.AlbumID.Value,
                    Artist = composition.Artist,
                    ArtistID = composition.ArtistID.Value,
                    Composition = composition,
                    CompositionID = composition.CompositionID,
                    CountOfPlays = 1,
                    GroupFormationDate = composition.GroupFormationDate == null ? DateTime.MinValue : composition.GroupFormationDate.Value,
                    GroupMember = composition.GroupMember,
                    ListenDate = DateTime.Now,
                    User = SessionInformation.CurrentUser,
                    UserID = SessionInformation.CurrentUser.UserID
                };

                dB.ListenedCompositions.Add(lC);
                dB.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void CopyFieldsExceptForDurationAndPath(Composition existingComp, Composition comp)
        {
            comp.About = existingComp.About; comp.Album = existingComp.Album;
            comp.AlbumID = existingComp.AlbumID; comp.Artist = existingComp.Artist;
            comp.ArtistID = existingComp.ArtistID; comp.CompositionName = existingComp.CompositionName;
        }

        public static bool HasAdminRights(User user)
        {
            //var matches = from user in dB.Administrators join 
            try
            {
                var adminQuery = from u in dB.Users
                                 join a in dB.Administrators
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
                Debug.WriteLine(ex.Message);
            }
            return false;
        }

        public static bool HasModerRights(User user)
        {
            //var matches = from user in dB.Administrators join 
            try
            {
                var moderQuery = from u in dB.Users
                                 join m in dB.Moderators
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
                Debug.WriteLine(ex.Message);
            }
            return false;
        }

        public static User AddNewUser(string login, string psswd,
            string email, string bio,
            string VKLink = "null", string FaceBookLink = "null")
        {
            DateTime lastListenedDataModificationDate = DateTime.MinValue;
            //DateTime 

            try
            {
                User user = new User();
                long id = 0;
                try
                {
                    id = DBAccess.dB.Users.Max(u => u.UserID) + 1;
                    // skip catching exception and leave the default value of 0
                }
                catch { }

                //user.UserID = id;
                user.UserName = login;
                user.Email = email;
                user.Password = DBAccess.ToMD5(psswd);
                user.DateOfSignUp = DateTime.Now;
                user.Bio = bio;

                user.VKLink = "null";
                user.FaceBookLink = "null";
                user.UserID = id;

                DBAccess.dB.Users.Add(user);
                DBAccess.dB.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static Moderator AddNewModerator(long userID)
        {
            try
            {
                var user = dB.Users.Find(userID);
                if (user == null)
                    return null;
                var moders = dB.Moderators.Where(m => m.UserID == userID);
                if (moders.Count() != 0)
                {
                    return moders.First();
                }

                Moderator moderator = new Moderator();
                moderator.UserID = userID;
                moderator.ModeratorID = GetNewModeratorID();

                dB.Moderators.Add(moderator);
                dB.SaveChanges();
                return moderator;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static Administrator AddNewAdministrator(long userID, long moderID)
        {
            try
            {
                var user = dB.Users.Find(userID);
                if (user == null)
                    return null;

                var moders = dB.Moderators.Find(moderID);
                if (moders == null)
                    return null;

                var admins = dB.Administrators.Where(a => a.UserID == userID);
                if (admins.Count() != 0)
                {
                    return admins.First();
                }

                Administrator administrator = new Administrator();
                administrator.UserID = userID;
                administrator.ModeratorID = moderID;
                administrator.AdministratorID = GetNewAdministratorId();

                dB.Administrators.Add(administrator);
                dB.SaveChanges();
                return administrator;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static bool DeleteComposition(long ID)
        {
            try
            {
                dB.Compositions.Remove(dB.Compositions.Find(ID));
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool DeleteComposition(Composition composition)
        {
            try
            {
                dB.Compositions.Remove(dB.Compositions.Find(composition.CompositionID));
                dB.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool DeleteListenedComposition(ListenedComposition composition)
        {
            try
            {
                var matches = dB.ListenedCompositions.Where(c => c.ListenDate == composition.ListenDate && c.UserID == composition.UserID);
                if (matches.Count() != 0)
                {
                    dB.ListenedCompositions.Remove(matches.First());
                    dB.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static ListenedArtist CreateNewListenedArtist()
        {
            throw new NotImplementedException();
        }

        public static IQueryable<ListenedComposition> GetCurrentUsersListenedCompositions()
        {
            return from comp in DBAccess.dB.ListenedCompositions
                   where comp.UserID == SessionInformation.CurrentUser.UserID
                   select comp;
        }

        public static IQueryable<ListenedComposition> GetCurrentUsersListenedGenres()
        {
            return from comp in DBAccess.dB.ListenedCompositions
                   where comp.UserID == SessionInformation.CurrentUser.UserID
                   select comp;
        }

        public static IQueryable<ListenedComposition> GetCurrentUsersListenedArtist()
        {
            return from comp in DBAccess.dB.ListenedCompositions
                   where comp.UserID == SessionInformation.CurrentUser.UserID
                   select comp;
        }
    }
}
