﻿using System;
using System.Diagnostics;
using System.Windows.Controls;
using MediaStreamer.Domain;
using StringExtensions;
using System.Collections;
using System.Collections.Generic;
using LinqExtensions;
using System.Linq;

namespace MediaStreamer.RAMControl
{
    public class Program
    {
        //TODO: Connect with RAMControl (Exclude Program, Session, SessionInformation, FirstFMPage)
        //TODO: Move CompositionStorage to RAMControl
        //TODO: Connect with RAMControl (Exclude Program, Session, SessionInformation, FirstFMPage)
        //TODO: Move CompositionStorage to RAMControl

        public static IDBRepository DBAccess;
        public static bool mediaPlayerIsPlaying = false;
        public static MediaElement mePlayer;
        public static System.Windows.Controls.TextBlock txtStatus;
        public static IComposition currentComposition;
        public static MediaStreamer.IO.FileManipulator FileManipulator;
        public static bool PlayerStopped = false;
        public static TimeSpan NewPosition;
        public static bool startupFromCommandLine = false;

        #region AutoPlay Closing / Opening
        public static void OnClosing()
        {
            if (currentComposition == null)
                return;
            double position = mePlayer.Position.TotalMilliseconds;
            var ts = TimeSpan.FromMilliseconds(position);

            var comp = currentComposition;
            ListenedComposition lC = new ListenedComposition();
            lC.CompositionID = comp.CompositionID;
            lC.UserID = SessionInformation.CurrentUser == null ? 0 : SessionInformation.CurrentUser.UserID;
            lC.StoppedAt = position;
            lC.ListenDate = DateTime.Now;

            DBAccess.ClearListenedCompositions();
            DBAccess.DB.Add(lC);
        }

        public static IList OnOpen()
        {
            var query = DBAccess.DB.GetListenedCompositions();
            if (query.Count() == 0)
                return new List<Composition>();
            var lcomp = DBAccess.DB.GetListenedCompositions().First();
            var comp = DBAccess.DB.GetCompositions().Where(c => c.CompositionID == lcomp.CompositionID).ToList();
            NewPosition = TimeSpan.FromMilliseconds(lcomp.StoppedAt);
            return comp;
        }
        #endregion

        public static void SetPlayerPositionToZero()
        {
            NewPosition = TimeSpan.FromMilliseconds(0.0);
            mePlayer.Position = TimeSpan.FromMilliseconds(0.0);
        }

        public static void HandleException(Exception ex)
        {
            SetCurrentStatus("Error to double click add composition");
            SetCurrentStatus(ex.Message);
        }

        public static void OpenURL(string URL)
        {
            if (URL.IsValidURL())
            {
                mePlayer.Source = new Uri(URL);
                mePlayer.Play();
                Program.mediaPlayerIsPlaying = true;
                Program.PlayerStopped = false;
            }
        }

        public static void OpenSource(string source)
        {
            if (!(source.FileExistsOrValidURL()))
                return;
            //TagLib.File file = TagLib.File.Create(source);

            //if (mePlayer == null)
            //    mePlayer = new MediaElement();

            //foreach (ICodec codec in file.Properties.Codecs)
            //{
            //    if (codec is TagLib.Mpeg.AudioFile)
            //    {
            mePlayer.Source = new Uri(source);
            mePlayer.Play();
            Program.mediaPlayerIsPlaying = true;
            Program.PlayerStopped = false;
            //return;
            //    }
            //}
        }

        [MTAThread]
        public static void SetTxtStatusContents(string status)
        {
            if (Session.MainPage != null && Session.MainPage.ListInitialized && status != null)
            {
                Session.MainPage.Dispatcher.BeginInvoke(new Action(delegate
                {
                    Session.MainPage.SetStatus(status);
                }));
            }
        }
        [MTAThread]
        public static void AddToStatus(string addition)
        {
            if (Session.MainPage != null && Session.MainPage.ListInitialized && addition != null)
            {
                Session.MainPage.Dispatcher.BeginInvoke(new Action(delegate
                {
                    Session.MainPage.AddToStatus(addition);
                }));
            }
        }

        [MTAThread]
        public static void SetCurrentAction(string action)
        {
            if (Session.MainPage != null && Session.MainPage.ListInitialized && action != null)
            {
                Session.MainPage.Dispatcher.BeginInvoke(new Action(delegate
                {
                    Session.MainPage.SetAction(action);
                }));
            }
        }
        [MTAThread]
        public static void SetCurrentStatus(string status)
        {
            if (Session.MainPage != null && Session.MainPage.ListInitialized && status != null)
            {
                SetTxtStatusContents(status);
            }
        }

        public static string ToString(IEnumerable comps)
        {
            string result = "";
            foreach (ICompositionInstance cmp in comps)
            {
                result += $"{ cmp?.GetInstance()?.Artist?.ArtistName ?? "Unknown"} – " +
                $"" + $"{cmp?.GetInstance()?.CompositionName ?? "Unknown"}" + ", ";
            }
            result = result.TrimEnd(new char[] { ',', ' ' });
            result += ".";
            return result;
        }

        public static string ReturnEmptyIfZero(int value) => value == 0 ? "" : " [" + value.ToString() + "]";

        public static string ToMergedString(IEnumerable comps)
        {
            Queue<string> result = new Queue<string>(); int repeated = 0; Composition previous = null;
            foreach (Composition current in comps)
            {
                if (current.CompareTo(previous) == 0)
                {
                    result.Dequeue();
                    repeated++;
                }
                result.Enqueue($"{ current?.Artist.ArtistName ?? "Unknown"} – " +
                $"" + $"{current?.CompositionName ?? "Unknown"}" + $"{ReturnEmptyIfZero(repeated)}, ");

                previous = current;
            }
            var last = result.Peek();
            last = last?.TrimEnd(new char[] { ',', ' ' });
            last += ".";
            return result.ToLine();
        }

        public static void ShowQueue(IEnumerable compositions, string action = "Now playing")
        {
            SetCurrentStatus($"Queued: {ToString(compositions)}");
        }

        public static void SetCurrentStatus(string status, bool error)
        {
            txtStatus.Text = status;
            if (error)
                Debug.WriteLine(status);
        }
    } // public class Program
}  // namespace MediaStreamer.RAMControl
