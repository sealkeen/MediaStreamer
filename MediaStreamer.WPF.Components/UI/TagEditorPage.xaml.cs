using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MediaStreamer.Domain;
using MediaStreamer.TagEditing;
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для TagEditorPage.xaml
    /// </summary>
    public partial class TagEditorPage : FirstFMPage
    {
        private DMTagEditor _dmTagEditor;

        public TagEditorPage(List<TagLib.File> tagFiles, List<IComposition> compositions)
        {
            InitializeComponent();

            _dmTagEditor = new DMTagEditor(tagFiles, compositions, Program.DBAccess, Program.SetCurrentStatus);
            DisplayAttributes(compositions?[0]);
        }

        private void DisplayAttributes(IComposition composition)
        {
            txtTitle.Text = composition?.CompositionName ?? "Unknown";
            txtAlbum.Text = composition?.Album?.AlbumName ?? "Unknown";
            txtArtist.Text = composition?.Artist?.ArtistName ?? "Unknown";
            txtYear.Text = composition?.Album?.Year?.ToString() ?? "0";
            txtPath.Text = composition?.FilePath ?? "";
            txtLyrics.Text = composition?.Lyrics ?? "";
        }

        private void buttonChangeArtist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dmTagEditor.ChangeAllArtists(txtArtist.Text, Program.DBAccess.DB, Program.SetCurrentStatus);
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }
        private void buttonChangeAlbum_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long? year = null;
                try
                {
                    year = Convert.ToInt64(txtYear.Text);
                }
                catch { }
                _dmTagEditor.AddAlbumToAll(txtArtist.Text, txtAlbum.Text, year,
                    null, null, Program.DBAccess.DB, Program.SetCurrentStatus);
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }
        private void buttonChangeTitle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dmTagEditor.AddTitleToAll(txtTitle.Text, Program.DBAccess.DB, Program.SetCurrentStatus);
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }
        private void buttonChangeYear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                uint? year = new uint?();
                try
                {
                    year = Convert.ToUInt32(txtYear.Text);
                }
                catch { }
                if (year.HasValue)
                    _dmTagEditor.AddYearToAll(year.Value, Program.DBAccess.DB);
                Program.DBAccess.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void buttonChangeLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string location = null;
                foreach (Composition comp in _dmTagEditor._compositions)
                {
                    //if (Program.IsValidURL(this.txtPath.Text))
                    //{
                        comp.FilePath = this.txtPath.Text;
                        Program.DBAccess.DB.SaveChanges();
                    //}
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void buttonPlayFromLyrics_Click(object sender, RoutedEventArgs e)
        {
            Program.OpenURL(txtPath.Text);
        }

        private void buttonChangeLyrics_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                foreach (Composition comp in _dmTagEditor._compositions)
                {
                    comp.Lyrics = txtLyrics.Text;
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void buttonSwapLyricsAndPath_Click(object sender, RoutedEventArgs e)
        {
            string path = txtPath.Text;
            txtPath.Text = txtLyrics.Text;
            txtLyrics.Text = path;
        }

        private void buttonTranslate_Click(object sender, RoutedEventArgs e)
        {
            txtPath.Text = txtPath.Text
                .Replace("https://drive.google.com/file/d/", "http://docs.google.com/uc?export=open&id=")
                .Replace("/view?usp=sharing", "");
        }
    }
}