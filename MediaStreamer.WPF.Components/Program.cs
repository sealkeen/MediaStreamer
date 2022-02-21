﻿using System;
using System.Diagnostics;
using System.Windows.Controls;
using MediaStreamer.Domain;
using MediaStreamer.IO;
using System.Collections;
using System.Collections.Generic;
using LinqExtensions;

namespace MediaStreamer.WPF.Components
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
            Session.MainPage.Dispatcher.BeginInvoke(new Action(delegate
            {
                Session.MainPage.txtStatus.Text = status;
            }));
        }
        [MTAThread]
        public static void AddToStatus(string addition)
        {
            Session.MainPage.Dispatcher.BeginInvoke(new Action(delegate
            {
                txtStatus.Text += addition;
            }));
        }

        [MTAThread]
        public static void SetCurrentAction(string action)
        {
            Session.MainPage.Dispatcher.BeginInvoke(new Action(delegate
            {
                Session.MainPage.lblStatus.Content = action;
            }));
        }
        [MTAThread]
        public static void SetCurrentStatus(string status)
        {
            SetTxtStatusContents(status);
        }

        public static string ToString(IEnumerable comps)
        {
            string result = "";
            foreach (ICompositionInstance cmp in comps)
            {
                result += $"{ cmp?.GetInstance()?.Artist.ArtistName ?? "Unknown"} – " +
                $"" + $"{cmp?.GetInstance()?.CompositionName ?? "Unknown"}" + ", ";
            }
            result = result.TrimEnd(new char[] { ',', ' ' });
            result += ".";
            return result;
        }

        public void Merge(LinkedList<string> list)
        {

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
} // namespace MediaStreamer.WPF.Components
