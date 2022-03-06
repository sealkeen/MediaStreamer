using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using MediaStreamer.TagEditing;
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для LoadingPage.xaml
    /// </summary>
    public partial class VideoPage : FirstFMPage
    {
        MediaElement videoPlayer;
        public static bool videoPlayerIsPlaying = false;
        public static bool videoLoaded = false;
        private bool userIsDraggingSlider = false;
        private bool canExecute = false;
        public VideoPage()
        {
            videoPlayer = mePlayer;
            mePlayer = Program.mePlayer;
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mePlayer.Source != null) && mePlayer.NaturalDuration.HasTimeSpan && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = mePlayer.Position.TotalSeconds;
            }
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                OpenSource(openFileDialog.FileName);
        }
        private void Open_Executed(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                mePlayer.Source = new Uri(openFileDialog.FileName);
            }
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((mePlayer != null) && (mePlayer.Source != null))
            {
                mePlayer.Play();
                Program.mediaPlayerIsPlaying = true;
            }
        }
        private void Play_Executed(object sender, RoutedEventArgs e)
        {
            if ((mePlayer != null) && (mePlayer.Source != null))
            {
                mePlayer.Play();
                Program.mediaPlayerIsPlaying = true;
            }
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Program.mediaPlayerIsPlaying;
        }
        private void Pause_CanExecute(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Program.mediaPlayerIsPlaying;
        }
        private void Stop_CanExecute(object sender, RoutedEventArgs e)
        {
            //e.CanExecute = mediaPlayerIsPlaying;
            mePlayer.Stop();
            mePlayer.Source = mePlayer.Source;
        }
        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Stop();
            Program.mediaPlayerIsPlaying = false;
        }
        private void Stop_Executed(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
            Program.mediaPlayerIsPlaying = false;
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Play_CanExecute(object sender, RoutedEventArgs e)
        {
            canExecute = (mePlayer != null) && (mePlayer.Source != null);
            if (canExecute)
            {
                mePlayer.Play();
                Program.mediaPlayerIsPlaying = true;
            }
        }

        private void OpenSource(string source)
        {
            if (System.IO.File.Exists(source) != true)
                return;
            TagLib.File file = TagLib.File.Create(source);

            if (mePlayer == null)
                mePlayer = new MediaElement();
            if (DMTagEditor.SourceIsVideo(source))
            {
                mePlayer = videoPlayer;
            }
            else if (DMTagEditor.SourceIsAudio(source))
            {
                mePlayer = Program.mePlayer;
            }
            else
            {
                return;
            }
            mePlayer.Source = new Uri(source);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (videoLoaded)
            {
                try
                {
                    if (videoPlayerIsPlaying)
                    {
                        mePlayer.Stop();
                        videoPlayerIsPlaying = false;
                    }
                    else
                    {
                        mePlayer.Play();
                        videoPlayerIsPlaying = true;
                    }
                }
                catch (Exception ex)
                {
                    Program.txtStatus.Text = ex.Message;
                }
            }
        }
    } //public partial class VideoPage : FirstFMPage
} //namespace MediaStreamer.WPF.Components

