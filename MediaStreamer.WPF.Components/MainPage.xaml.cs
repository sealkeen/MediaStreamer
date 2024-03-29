﻿using MediaStreamer.Domain;
using MediaStreamer.RAMControl;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace MediaStreamer.WPF.Components
{
    public partial class MainPage : StatusPage
    {
        public MainPage()
        {
            Session.MainPageVM = new MainPageViewModel();
            Session.MainPageVM.UpdateBindingExpression = this.UpdateBindingExpression;
            //DataContext = Session.MainPageVM;

            InitializeComponent();
            
            InitializeData().Wait();

            lblPager.DataContext = Session.MainPageVM;
            //LoggerPage.GetInstance();
        }

        public async Task InitializeData()
        {
            try
            {
                Program._logger?.LogTrace($"The new position is : {Program.NewPosition}");
                Program._logger?.LogTrace("Creating MainPage()");
                if (Program.FileManipulator == null)
                    Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess, Program._logger);

                Selector.MainPage = this;
                Session.MainPage = this;

                Program._logger?.LogTrace("Creating Dispatcher Timer");
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
                Program.mePlayer = this.mePlayer;
                Program.txtStatus = this.txtStatus;
                ListInitialized = true;
            } catch (Exception ex) {
                Program._logger?.LogError("in MainWindow.InitializeData(): " + ex.ToString() + ex.Message);
            }
        }

        private double volumeKeyPercent = 5;
        private bool userIsDraggingSlider = false;
        private bool canExecute = false;

        public override Frame GetFrame()
        {
            return mainFrame;
        }

        /// <summary> Load Page Event </summary>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if(Program.DBAccess.LoadingTask != null) await Program.DBAccess.LoadingTask; 
            buttonCompositions_Click(buttonCompositions, new RoutedEventArgs());

            if (!Program.startupFromCommandLine)
            {
                var savedQuery = Program.OnOpen();
                Selector.CompositionsPage.QueueSelected(savedQuery);
                Program.mePlayer.Position = Program.NewPosition;
                if (savedQuery != null && savedQuery.Count > 0) {
                    Program.currentComposition = savedQuery[0] as IComposition;
                    Selector.CompositionsPage.lstItems_TryToSelectItem(Program.currentComposition);
                }
            } else {
                Program.SetPlayerPositionToZero();
                Selector.CompositionsPage?.SwitchToNextSelected();
                Selector.CompositionsPage?.PlayTarget(Selector.CompositionsPage.GetNextComposition());
                Program.startupFromCommandLine = false;
            }
        }
        private void Header_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var shiftIncrease = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            if (e.Delta > 0)
            {
                VolumeUp("Mouse Wheel", shiftIncrease);
            }
            else
            {
                VolumeDown("Mouse Wheel", shiftIncrease);
            }
        }
        private void StatusPage_HandleArrows(object sender, KeyEventArgs e)
        {
            if (!Selector.CompositionsPage.lstItems_OwnsFocus())
            {
                var shiftIncrease = Keyboard.Modifiers == ModifierKeys.Shift;
                if (e.Key == Key.Up) VolumeUp("Up Arrow", shiftIncrease);
                if (e.Key == Key.Down) VolumeDown("Down Arrow", shiftIncrease);
            }
        }
        private void VolumeDown(string key, bool shiftIncrease)
        {
            double volumeAdjustment = mePlayer.Volume * volumeKeyPercent / 100.0;
            if (shiftIncrease) { volumeAdjustment *= 5; }
            mePlayer.Volume -= volumeAdjustment;
            Program.SetCurrentStatus($"{key}: volume down.");
        }
        private void VolumeUp(string key, bool shiftIncrease)
        {
            double volumeAdjustment = mePlayer.Volume * volumeKeyPercent / 100.0;
            if (shiftIncrease) { volumeAdjustment *= 5; }
            mePlayer.Volume += volumeAdjustment;
            Program.SetCurrentStatus($"{key}: volume up.");
        }

        public void StatusPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key) {
                case (Key.Space): HandleSpacePressed(sender, e); break;
                case (Key.Down): StatusPage_HandleArrows(sender, e); break;
                case (Key.Up): StatusPage_HandleArrows(sender, e); break;
                case (Key.Enter): 
                    if(Session.MainPage.GetFrame().Content is CompositionsPage &&
                        Selector.CompositionsPage.lstItems.SelectedIndex >= 0)
                        Selector.CompositionsPage.PlaySelectedTarget(); break;
            }
        }

        private void HandleSpacePressed(object sender, KeyEventArgs e)
        {
            var ok = mainFrame.Focus();
            if (Program.mePlayer.Source == null && Selector.CompositionsPage.HasNextInListOrQueue())
            {
                Selector.CompositionsPage?.SwitchToNextSelected();
                Selector.CompositionsPage?.PlayTarget(Selector.CompositionsPage.GetNextComposition());
                Program.SetCurrentStatus("Space key pressed, playback started");
                return;
            }
            if (Program.mediaPlayerIsPlaying)
            {
                Pause_CanExecute(this, new RoutedEventArgs());
                Program.SetCurrentStatus("Space key pressed, playback paused");
            }
            else
            {
                Play_Executed(this, new RoutedEventArgs());
                Program.SetCurrentStatus("Space key pressed, playback started");
            }
        }

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
                    Selector.CompositionsPage.HasNextInListOrQueue() &&
                    !Program.PlayerStopped
                )
                {
                    Selector.CompositionsPage?.SwitchToNextSelected();
                    Selector.CompositionsPage?.PlayTarget(Selector.CompositionsPage.GetNextComposition());
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus("PlayNextIfExists: " + ex.Message);
            }
        }
        private async void buttonCompositions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Program._logger?.LogTrace("Started loading from context ");
                await ListAsync();
            }
            catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        public override void SetFrameContent(object o)
        {
            mainFrame.Content = o;
            mainFrame.UpdateLayout();
        }

        [MTAThread]
        public void SetContentPageCompositions(FirstFMPage page = null)
        {
            mainFrame.Content = page ?? Selector.CompositionsPage;
        }
        [MTAThread]
        public override async Task ListAsync()
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            if (Selector.CompositionsPage == null)
                Selector.CompositionsPage = new CompositionsPage();
            if (Session.CompositionsVM.LastDataLoadWasPartial() || !Selector.CompositionsPage.ListInitialized)
            {
#if !NET40
                await Selector.CompositionsPage.ListAsync();
#else
                Selector.CompositionsPage.ListAsync().Wait();
#endif
                Session.CompositionsVM.ResetPartialLoad();
            }
            Dispatcher.BeginInvoke(new Action(() => SetContentPageCompositions())).Wait();
            mainFrame.UpdateLayout();
            SetStatus("All compositions listing");
        }

        private void buttonAlbums_Click(object sender, RoutedEventArgs e)
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            if (Selector.AlbumsPage == null)
                Selector.AlbumsPage = new AlbumsPage();
            else
                Selector.AlbumsPage.List();
            mainFrame.Content = Selector.AlbumsPage;
            mainFrame.UpdateLayout();
            SetStatus("All albums listing");
        }

        private void buttonArtistGenres_Click(object sender, RoutedEventArgs e)
        {
            if (Selector.AGenresPage == null)
                Selector.AGenresPage = new ArtistGenresPage();
            else
                Selector.AGenresPage.List();
            mainFrame.Content = Selector.AGenresPage;
            mainFrame.UpdateLayout();
            SetStatus("All artist genres listing");
        }


        private void buttonArtists_Click(object sender, RoutedEventArgs e)
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            if (Selector.ArtistsPage == null)
                Selector.ArtistsPage = new ArtistsPage();
            else
                Selector.ArtistsPage.List();
            mainFrame.Content = Selector.ArtistsPage;
            mainFrame.UpdateLayout();
            SetStatus("All artists listing");
        }

        private void buttonGenres_Click(object sender, RoutedEventArgs e)
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            if (Selector.GenresPage == null)
                Selector.GenresPage = new GenresPage();
            else
                Selector.GenresPage.List();
            mainFrame.Content = Selector.GenresPage;
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
                        txtStatus.Text = "Username/Password combination was not found.";
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
                        txtStatus.Text = "Username/Password combination was not found.";
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
            mainFrame.Content = Selector.SignUpPage == null ?
                Selector.SignUpPage = new SignUpPage() :
                Selector.SignUpPage;
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


        private void buttonUserCompositions_Click(object sender, RoutedEventArgs e)
        {
            if (Selector.ListenedCompositionsPage == null)
                Selector.ListenedCompositionsPage =
                    new UserCompositionsPage();
            else
                Selector.ListenedCompositionsPage.List();
            mainFrame.Content = Selector.ListenedCompositionsPage;
            mainFrame.UpdateLayout();
            SetStatus("User listened compositions listing");
        }

        private void buttonUserGenres_Click(object sender, RoutedEventArgs e)
        {
            if (SessionInformation.CurrentUser != null)
            {
                if (Selector.UserGenresPage == null)
                    Selector.UserGenresPage = new UserGenresPage(SessionInformation.CurrentUser.UserID);
                else
                    Selector.UserGenresPage.ListByID(SessionInformation.CurrentUser.UserID);
                mainFrame.Content = Selector.UserGenresPage;
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
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            if (Selector.ListenedAlbumsPage == null)
                Selector.ListenedAlbumsPage = new UserAlbumsPage();
            else
                Selector.ListenedAlbumsPage.ListByID(SessionInformation.CurrentUser.UserID);
            mainFrame.Content = Selector.ListenedAlbumsPage;
            mainFrame.UpdateLayout();
            SetStatus("User albums listing");
        }

        private void buttonUserArtists_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonVideo_Click(object sender, RoutedEventArgs e)
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            if (Selector.VideoPage == null)
                Selector.VideoPage = new VideoPage();
            mainFrame.Content = Selector.VideoPage;
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
            mePlayer.Pause(); Program.PlayerStopped = true; Program.mediaPlayerIsPlaying = false;
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
            int selected = Selector.CompositionsPage.SelectedItemsCount();
            if (selected > 0)
            {
                Selector.CompositionsPage.QueueSelected();
                Program.ShowQueue(Selector.CompositionsPage.SelectedItems());
            }
            else if (selected == 0)
            {
                if (Selector.CompositionsPage.ItemsCount() != 0)
                {
                    Program.SetCurrentStatus("No items selected to play.");
                }
            }
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Selector.Dispose();
            Program.DBAccess?.DB?.Dispose();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Selector.CompositionsPage.HasNextInListOrQueue()
                )
                {
                    Selector.CompositionsPage?.SwitchToNextSelected();
                    Selector.CompositionsPage?.PlayTarget(Selector.CompositionsPage.GetNextComposition());
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus("MainPage.btnNext_Click" + ex.Message);
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Selector.CompositionsPage.HasPreviousInList()
                )
                {
                    Selector.CompositionsPage?.SwitchToPreviousSelected();
                    Selector.CompositionsPage?.PlayTarget(Selector.CompositionsPage?.GetCurrentComposition());
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus("btnPrevious_Click" + ex.Message);
            }
        }

        public void UpdateBindingExpression()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                BindingOperations.GetBindingExpression(lblPager, Label.ContentProperty).UpdateTarget())
            );
        }
    }
}
