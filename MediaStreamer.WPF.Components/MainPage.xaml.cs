using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MainPage : StatusPage
    {
        public MainPage()
        {
            InitializeComponent();
            Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess);
            Session.MainPage = this;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            Program.mePlayer = this.mePlayer;
            Program.txtStatus = this.txtStatus;
        }
        private double volumeSliderValue = 0.025; 
        private bool userIsDraggingSlider = false;
        private bool canExecute = false;
        public event Action OnConstructing;

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mePlayer.Source != null) && mePlayer.NaturalDuration.HasTimeSpan && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = mePlayer.Position.TotalSeconds;
                PlayNextIfExists(sender);
            }
        }

        private void PlayNextIfExists(object sender)
        {
            try
            {
                if (Program.currentComposition != null &&
                    Math.Ceiling(Program.mePlayer.Position.TotalSeconds) >= Program.currentComposition?.Duration &&
                    !userIsDraggingSlider &&
                    Session.CompositionsPage.HasNextInListOrQueue() &&
                    !Program.PlayerStopped
                )
                {
                    Session.CompositionsPage?.SwitchToNextSelected();
                    Session.CompositionsPage?.PlayTarget(Session.CompositionsPage.GetNextComposition());
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void buttonCompositions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateCompositionsPage();
            }
            catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        [MTAThread]
        public async void UpdateCompositionsPage()
        {
            mainFrame.Content = Session.loadingPage;
            if (Session.CompositionsPage == null)
                Session.CompositionsPage = new CompositionsPage();
            if (!Session.CompositionsPage.ListInitialized)
            {
#if !NET40
                await Session.CompositionsPage.ListCompositionsAsync();
#else
                Session.CompositionsPage.ListCompositionsAsync().Wait();
#endif
            }
            Dispatcher.BeginInvoke(new Action(() => SetContentPageCompositions())).Wait();
            mainFrame.UpdateLayout();
            SetStatus("All compositions listing");
        }

        private void buttonAlbums_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = Session.loadingPage;
            if (Session.AlbumsPage == null)
                Session.AlbumsPage = new AlbumsPage();
            else
                Session.AlbumsPage.ListAlbums();
            mainFrame.Content = Session.AlbumsPage;
            mainFrame.UpdateLayout();
            SetStatus("All albums listing");
        }

        private void buttonArtistGenres_Click(object sender, RoutedEventArgs e)
        {
            if (Session.AGenresPage == null)
                Session.AGenresPage = new ArtistGenresPage();
            else
                Session.AGenresPage.RetrieveArtistGenres();
            mainFrame.Content = Session.AGenresPage;
            mainFrame.UpdateLayout();
            SetStatus("All artist genres listing");
        }

        private void buttonGroupMembers_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = Session.loadingPage;
            if (Session.MembersPage == null)
                Session.MembersPage = new GroupMembersPage();
            else
                Session.MembersPage.ListGroupMembers();
            mainFrame.Content = Session.MembersPage;
            mainFrame.UpdateLayout();
            SetStatus("All group members by band listing");
        }

        private void buttonArtists_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = Session.loadingPage;
            if (Session.ArtistsPage == null)
                Session.ArtistsPage = new ArtistsPage();
            else
                Session.ArtistsPage.ListArtists();
            mainFrame.Content = Session.ArtistsPage;
            mainFrame.UpdateLayout();
            SetStatus("All artists listing");
        }

        private void buttonGenres_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = Session.loadingPage;
            if (Session.GenresPage == null)
                Session.GenresPage = new GenresPage();
            else
                Session.GenresPage.ListGenres();
            mainFrame.Content = Session.GenresPage;
            mainFrame.UpdateLayout();
            SetStatus("All genres listing");
        }

        public override void SetAction(string action)
        {
            lblStatus.Content = action;
        }

        public override void SetStatus(string status)
        {
            txtStatus.Text = status;
        }

        [MTAThread]
        public void SetContentPageCompositions(FirstFMPage page = null)
        {
            mainFrame.Content = page ?? Session.CompositionsPage;
        }

        private void buttonLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogStatus logStatus = Session.Log(txtLogin.Text, txtPassword.Password);
                switch (logStatus)
                {
                    case LogStatus.Unlogged:
                        lblLogin.Content = "Signed out";
                        buttonLog.Content = "Sign in";
                        buttonSignUp.Visibility = Visibility.Visible;
                        txtLogin.Visibility = Visibility.Visible;
                        txtPassword.Visibility = Visibility.Visible;
                        DisableUserPages();
                        return;
                    case LogStatus.LoginIsMissing:
                        txtStatus.Text = "Enter your user name.";
                        return;
                    case LogStatus.PasswordIsMissing:
                        txtStatus.Text = "Enter your password.";
                        return;
                    case LogStatus.LoginPasswordPairIsIncorrect:
                        txtStatus.Text = "Username/Password combination is not found.";
                        return;
                    case LogStatus.Logged:
                        lblLogin.Content = "Logged in as: \n" +
                            SessionInformation.CurrentUser.UserName;
                        txtStatus.Text = "Successfully logged in as: " + SessionInformation.CurrentUser.UserName;
                        buttonLog.Content = "Sign out";
                        buttonSignUp.Visibility = Visibility.Hidden;
                        txtLogin.Visibility = Visibility.Hidden;
                        txtPassword.Visibility = Visibility.Hidden;
                        EnableUserPages();
                        return;
                    case LogStatus.PasswordIsIncorrect:
                        return;
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus($" MainWindow ButtonLog() Exception : {ex.Message}");
            }
        }

        private void buttonSignUp_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = Session.SignUpPage == null ?
                Session.SignUpPage = new SignUpPage() :
                Session.SignUpPage;
        }

        private void mainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        //public string ToMD5(string source)
        //{
        //    byte[] tmpSource; byte[] tmpHash;

        //    //Create a byte array from source data.
        //    tmpSource = ASCIIEncoding.ASCII.GetBytes(source);
        //    tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

        //    string hashed = tmpHash.ToString();
        //    return hashed;
        //}

        public void EnableUserPages()
        {
            userPagesStackPanel.IsEnabled = true;
        }

        public void DisableUserPages()
        {
            userPagesStackPanel.IsEnabled = false;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if(Program.DBAccess.LoadingTask != null)
            //    await Program.DBAccess.LoadingTask;

            buttonCompositions_Click(buttonCompositions, new RoutedEventArgs());
        }

        private void buttonUserCompositions_Click(object sender, RoutedEventArgs e)
        {
            if (Session.ListenedCompositionsPage == null)
                Session.ListenedCompositionsPage =
                    new MediaStreamer.WPF.Components.UserCompositionsPage();
            else
                Session.ListenedCompositionsPage.ListCompositions();
            mainFrame.Content = Session.ListenedCompositionsPage;
            mainFrame.UpdateLayout();
            SetStatus("User listened compositions listing");
        }

        private void buttonUserGenres_Click(object sender, RoutedEventArgs e)
        {
            if (SessionInformation.CurrentUser != null)
            {
                if (Session.UserGenresPage == null)
                    Session.UserGenresPage = new UserGenresPage(SessionInformation.CurrentUser.UserID);
                else
                    Session.UserGenresPage.ListGenres(SessionInformation.CurrentUser.UserID);
                mainFrame.Content = Session.UserGenresPage;
                mainFrame.UpdateLayout();
                SetStatus("Your added and listened genres listing:");
            }
            else
            {
                SetStatus("An error occured, you're not logged in.");
            }
        }

        private void buttonUserAlbums_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = Session.loadingPage;
            if (Session.ListenedAlbumsPage == null)
                Session.ListenedAlbumsPage = new UserAlbumsPage();
            else
                Session.ListenedAlbumsPage.PartialListAlbums(SessionInformation.CurrentUser.UserID);
            mainFrame.Content = Session.ListenedAlbumsPage;
            mainFrame.UpdateLayout();
            SetStatus("User albums listing");
        }

        private void buttonUserArtists_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonVideo_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = Session.loadingPage;
            if (Session.VideoPage == null)
                Session.VideoPage = new VideoPage();
            mainFrame.Content = Session.VideoPage;
            mainFrame.UpdateLayout();
            SetStatus("User albums listing");
        }

        //MEDIA PLAYER METHODS
        private void Open_Executed(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";
            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //    mePlayer.Source = new Uri(openFileDialog.FileName);
        }
        private void Play_Executed(object sender, RoutedEventArgs e)
        {
            if ((mePlayer != null) && (mePlayer.Source != null))
            {
                mePlayer.Play();
                Program.mediaPlayerIsPlaying = true;
                Program.PlayerStopped = false;
            }
        }
        private void Pause_CanExecute(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause(); Program.PlayerStopped = true;
        }
        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Pause(); Program.PlayerStopped = true;
        }

        private void Stop_CanExecute(object sender, RoutedEventArgs e)
        {
            Program.PlayerStopped = true;
            //e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Stop_Executed(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
            Program.mediaPlayerIsPlaying = false;
            Program.PlayerStopped = true;
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

        private void Play_CanExecute(object sender, RoutedEventArgs e)
        {
            canExecute = (mePlayer != null) && (mePlayer.Source != null);
            if ((mePlayer != null) && (mePlayer.Source != null) && Program.PlayerStopped)
            {
                mePlayer.Play();
                Program.mediaPlayerIsPlaying = true;
                Program.PlayerStopped = false;
                return;
            }
            int selected = Session.CompositionsPage.SelectedItemsCount();
            if (selected > 0)
            {
                Session.CompositionsPage.QueueSelected();
                Program.ShowQueue(Session.CompositionsPage.SelectedItems());
            }
            else if (selected == 0)
            {
                if (Session.CompositionsPage.lstItems.Items.Count != 0)
                {
                    Program.SetCurrentStatus("No items selected to play.");
                }
            }
        }

        private void Header_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mePlayer.Volume += (e.Delta > 0) ? volumeSliderValue : -volumeSliderValue;
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Session.Dispose();
            Program.DBAccess?.DB?.Dispose();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Session.CompositionsPage.HasNextInListOrQueue()
                )
                {
                    Session.CompositionsPage?.SwitchToNextSelected();
                    Session.CompositionsPage?.PlayTarget(Session.CompositionsPage.GetNextComposition());
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Session.CompositionsPage.HasPreviousInList()
                )
                {
                    Session.CompositionsPage?.SwitchToPreviousSelected();
                    Session.CompositionsPage?.PlayTarget(Session.CompositionsPage?.GetCurrentComposition());
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        //public event RoutedEventHandler DataBaseClick;
        //private void btnDatabase_Click(object sender, RoutedEventArgs e)
        //{
        //    DataBaseClick?.Invoke(sender, e);
        //}
    }
}
