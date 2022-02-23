using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MediaStreamer.Domain;
using MediaStreamer.IO;
using LinqExtensions;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для Compositions.xaml
    /// </summary>
    public partial class CompositionsPage : FirstFMPage
    {
        public CompositionsPage()
        {
            ListInitialized = false;
            //Session.MainPage.mainFrame.Content = Session.loadingPage; //LoadManagementElements(); //ListCompositionsAsync();
            Compositions = new List<IComposition>();
            Queue = new LinkedList<Composition>();
            InitializeComponent();
            DataContext = this;
            //tsk.Wait(); //Session.MainPage.mainFrame.Content = this;
        }
        public IList<IComposition> Compositions { get; set; }
        public LinkedList<Composition> Queue { get; set; }
        public bool lastDataLoadWasPartial = false;
        protected long _lastPartialArtistID = -1;
        protected long _lastPartialAlbumID = -1;
        protected bool _orderByDescending = false;
        public bool ListInitialized = false;
        public int LastCompositionIndex = -1;


        protected void ListView_OnColumnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _orderByDescending = !_orderByDescending;
                string columnName = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
                switch (columnName)
                {
                    case "Composition":
                        Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.CompositionName, _orderByDescending).ToList();
                        break;
                    case "Artist":
                        Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.Artist.ArtistName, _orderByDescending).ToList();
                        break;
                    case "Duration (sec)":
                        Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.Duration, _orderByDescending).ToList();
                        break;
                    case "File Path":
                        Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.FilePath, _orderByDescending).ToList();
                        break;
                }
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            }
        }

        public virtual List<IComposition> GetICompositions()
        {
            try {
                //DBAccess.Update();
                var result = Program.DBAccess.DB.GetICompositions().ToList();
                Dispatcher.BeginInvoke(new Action(() => Session.MainPage.SetContentPageCompositions()));
                ListInitialized = true;
                return result;
            } catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
                return new List<IComposition>();
            }
        }

        public override void LoadManagementElements()
        {
            if (ownerID == null)
                return;
            var existingAdmins = Program.DBAccess.DB.GetAdministrators().Select(adm => adm).Where(a => a.UserID == ownerID);
            if (existingAdmins.Count() > 0)
            {
                ShowManagementElements();
            }
            else
            {
                HideManagementElements();
            }
        }

        public override void ShowManagementElements()
        {
            buttonDelete.Visibility = Visibility.Visible;
            buttonDelete.IsEnabled = true;
        }

        public override void HideManagementElements()
        {
            buttonDelete.Visibility = Visibility.Hidden;
            buttonDelete.IsEnabled = false;
        }

        public async Task ListCompositionsAsync()
        {
            try
            {
#if !NET40
                var listCompsTask = Task.Factory.StartNew(GetICompositions);
                //var tsk = await listCompsTask;
                listCompsTask.Wait();
                Compositions = listCompsTask.Result;
#else
                Compositions = GetICompositions();
#endif
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                lastDataLoadWasPartial = false;
            }
            catch {
                Compositions = new List<IComposition>();
                try {
                    Compositions = GetICompositions();
                } catch { 

                }
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
        }

        public CompositionsPage(long ArtistID, long albumID)
        {
            Compositions = new List<IComposition>();
            InitializeComponent();
            LoadManagementElements();
            PartialListCompositions(ArtistID, albumID);
            DataContext = this;
        }

        [MTAThread]
        public void SetPartialCompositions(List<IComposition> compositions)
        {
            Compositions = compositions;
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public void GetPartOfCompositions()
        {
            var compositions = (from composition in Program.DBAccess.DB.GetICompositions()
                                where composition.AlbumID == _lastPartialAlbumID &&
                                composition.ArtistID == _lastPartialArtistID
                                select composition).ToList();
            Dispatcher.BeginInvoke(new Action(() => SetPartialCompositions(compositions)));
        }

        public async void PartialListCompositions(long ArtistID, long albumID)
        {
            ListInitialized = false;
            _lastPartialArtistID = ArtistID;
            _lastPartialAlbumID = albumID;
            lastDataLoadWasPartial = true;
            await Task.Factory.StartNew(GetPartOfCompositions);
        }

        protected async void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            var tsk = await Program.FileManipulator.OpenAudioFileCrossPlatform();

            Program.FileManipulator.DecomposeAudioFile(tsk, Program.SetCurrentStatus);
            ReList();
        }

        protected void buttonNewRange_Click(object sender, RoutedEventArgs e)
        {
            Program.FileManipulator.DecomposeAudioFiles(Program.FileManipulator.OpenAudioFilesCrossPlatform(), Program.SetCurrentStatus);
            ReList();
        }

        public bool HasNextInList()
        {
            return ((LastCompositionIndex + 1) < Compositions.Count());
        }
        public bool HasFirstElement()
        {
            return (Compositions != null && Compositions.Count > 1);
        }

        public bool HasNextInQueue()
        {
            return !Queue.Empty();
        }

        public bool HasPreviousInList()
        {
            int? selectedIndex = lstItems?.SelectedIndex;
            bool moreThenOne = selectedIndex >= 1;
                //&& 
            bool lessThanLast = (lstItems?.SelectedIndex < lstItems?.Items.Count);
            return moreThenOne && lessThanLast;
        }

        public bool HasNextInListOrQueue()
        {
            return HasNextInQueue() || HasNextInList();
        }

        public IComposition GetCurrentComposition()
        {
            try
            {
                return Compositions[lstItems.SelectedIndex];
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
                return null;
            }
        }

        public IComposition GetPreviousComposition()
        {
            try
            {
                if (HasPreviousInList())
                {
                    return Compositions[lstItems.SelectedIndex - 1];
                }
                throw new ArgumentOutOfRangeException("End of compositions list.");
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
                return null;
            }
        }

        public IComposition GetNextComposition()
        {
            try
            {
                if (!Queue.Empty())
                {
                    return Queue.Dequeue();
                }
                if (HasNextInList()) {
                    return Compositions[(LastCompositionIndex + 1)];
                }
                throw new ArgumentOutOfRangeException("End of compositions list.");
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
                return null;
            }
        }

        protected void buttonListen_Click(object sender, RoutedEventArgs e)
        {

        }

        public void SwitchToPreviousSelected()
        {
            if (HasPreviousInList())
            {
                LastCompositionIndex = lstItems.SelectedIndex;
                lstItems.SelectedIndex -= 1;
            }
        }

        public void SwitchToNextSelected()
        {
            if (HasNextInList() && Queue.Empty())
            {
                if (lstItems.SelectedIndex + 1 < lstItems.Items.Count)
                {
                    LastCompositionIndex = lstItems.SelectedIndex;
                    lstItems.SelectedIndex += 1;
                } else if (HasFirstElement() && Queue.Empty()) {
                    LastCompositionIndex = -1;
                    lstItems.SelectedIndex = 0;
                }
            }
        }

        public int SelectedItemsCount()
        {
            return lstItems.SelectedItems.Count;
        }

        public IList SelectedItems()
        {
            return lstItems.SelectedItems;
        }

        public delegate void AppendHandler(Composition composition);
        public static void EnqueueOrPush(AppendHandler enquque, AppendHandler push, Composition comp, bool enqueue)
        {
            if (enqueue)
                enquque(comp);
            else
                push(comp);
        }

        public void QueueSelected(IList selectedItems, bool queueOrPush = true)
        {
            try
            {
                foreach (ICompositionInstance composition in selectedItems)
                {
                    EnqueueOrPush(Queue.Enqueue, Queue.Push, composition.GetInstance(), queueOrPush);
                }
                if (!Program.mediaPlayerIsPlaying && HasNextInQueue())
                {
                    PlayTarget(GetNextComposition());
                }
                Program.ShowQueue(selectedItems);
                Program.AddToStatus(" Queue: { ");
                Program.AddToStatus(Program.ToString(Queue));
                Program.AddToStatus(" }.");
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        public void QueueSelected(bool queueOrPush = true)
        {
            QueueSelected(lstItems.SelectedItems, queueOrPush);
        }

        public void PlayTarget(IComposition target)
        {
            try
            {
                if (target == null || string.IsNullOrEmpty(target.FilePath))
                    return;
                Program.OpenSource(target?.FilePath);

                var art = target?.Artist?.ArtistName;
                var comp = target?.CompositionName;
                Program.SetCurrentStatus($"Played: [{art ?? "Unknown"} – " + $"{comp ?? "Unknown"}]");
                Session.MainPage.SetCurrentAction($"Now playing {art ?? "Unknown"} – " + $"{comp ?? "Unknown"}");
                Program.AddToStatus(", Queued: {");
                Program.AddToStatus(Program.ToString(Queue));
                Program.AddToStatus(" }.");

                Program.currentComposition = target;

                if (SessionInformation.CurrentUser != null)
                {
                    long? id = Program.currentComposition?.CompositionID;

                    if (target != null)
                        Program.DBAccess?.AddNewListenedComposition(target.GetInstance(), SessionInformation.CurrentUser);
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus($"Error to double click PlayTarget(): {ex?.Message}");
            }
        }

        protected void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if ((Program.mePlayer != null) && (Program.mePlayer.Source != null)) {
            if (null == (((FrameworkElement)e.OriginalSource).DataContext as Composition))
            {
                return;
            }

            PlayTarget(Compositions[lstItems.SelectedIndex]);
        }

        //TODO: Remove TagEditing import from WPF.Components
        public void ChangeComposition(IList selectedItems)
        {
            try
            {
                if (selectedItems.Count >= 0)
                {
                    ICompositionInstance currentComp;
                    var files = new List<TagLib.File>() { };
                    var compositions = new List<IComposition>();
                    
                    // For every selected element
                    foreach (var song in selectedItems)
                    {
                        currentComp = (ICompositionInstance)song;
                        string filePath = currentComp?.GetInstance().FilePath;
                        if (filePath != null)
                        {
                            compositions?.Add(currentComp.GetInstance());
                            var file = filePath.FileExists() ? TagLib.File.Create(filePath) : null;
                            files.Add(file);
                        }
                    }
                    Session.TagEditorPage = new TagEditorPage(files, compositions);
                    Session.MainPage.mainFrame.Content = Session.TagEditorPage;
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, error: true);
            }
        }

        protected void CmiChangeComposition_Click(object sender, RoutedEventArgs e)
        {
            ChangeComposition(lstItems.SelectedItems);
        }

        protected void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? index = lstItems?.SelectedIndex;
                if (index != null && index != -1)
                {
                    Program.DBAccess?.DeleteComposition(Compositions[index.Value].GetInstance());
                }
                ReList();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus($"Delete composition violation: {ex.Message}", true);
            }
        }

        protected void ReList()
        {
            if (lastDataLoadWasPartial != true)
            {
                ListCompositionsAsync();
            }
            else
            {
                PartialListCompositions(_lastPartialArtistID, _lastPartialAlbumID);
            }
        }

        protected void CmiPlaySeveral_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Program.FileManipulator.PlaySeveralSongs(lstItems?.SelectedItems, typeof(Composition));
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            }
        }

        protected void LstItems_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (null == (((FrameworkElement)e.OriginalSource).DataContext as Composition))
            //    return;
            //ApplyToSelectedItems(RenameCompositionFiles);
        }

        protected void ApplyToSelectedItems(Action<List<IComposition>> action)
        {
            try
            {
                if (lstItems.SelectedIndex >= 0 && lstItems.SelectedItems.Count >= 1)
                {// For every selected element
                    IComposition currentComp = Compositions[lstItems.SelectedIndex];
                    List<IComposition> compositions = new List<IComposition>();
                    foreach (var song in lstItems.SelectedItems)
                    {
                        currentComp = Compositions[lstItems.Items.IndexOf(song)];
                        if (!string.IsNullOrEmpty(currentComp.FilePath))
                        {
                            compositions.Add(currentComp);
                        }
                    }
                    action(compositions);
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
                UnselectItems();
            }
        }

        protected void UnselectItems()
        {
            if (lstItems != null)
                lstItems.SelectedIndex = -1;
        }

        /// <summary>
        /// TODO:Fix "Rename to Standard" check menu item button -> enable renaming the file to match pattern "Artist – Title (Year if exists)".
        /// </summary>
        /// <param name="compositions"></param>
        protected static void RenameCompositionFiles(List<IComposition> compositions)
        {
            FileInfo fileInfo;
            foreach (var comp in compositions)
            {
                fileInfo = new FileInfo(comp.FilePath);
                Artist artist = comp.Artist;
                if (comp.Artist == null)
                {
                    artist = Program.DBAccess.DB.GetICompositions().Where(x => x.ArtistID == comp.ArtistID).First().Artist;
                }
                string newName = fileInfo.DirectoryName + "\\" +
                    artist.ArtistName + " – " + comp.CompositionName;
                if (comp.Album.Year != null)
                {
                    if (comp.Album.Year < 2100 && comp.Album.Year > 1900)
                        newName += $" ({comp.Album.Year})";
                }
                newName += fileInfo.Extension;
                System.IO.File.Move(fileInfo.FullName, newName);
                comp.FilePath = newName;
                Program.DBAccess.DB.SaveChanges();

                //TimeSpan tS;
                //if (comp.Duration != null)
                //    tS = TimeSpan.FromSeconds(comp.Duration.Value);
                //else
                //    tS = TimeSpan.FromSeconds(int.MinValue);

                Program.DBAccess.AddComposition(artist.ArtistName,
                    comp.CompositionName, comp.Album.AlbumName, comp.Duration.Value, comp.FilePath);
            }
        }

        /// <summary>
        /// TODO:Fix "Rename to Standard" check menu item button -> enable renaming the file to match pattern "Artist – Title (Year if exists)".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CmiRename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstItems.SelectedIndex >= 0 && lstItems.SelectedItems.Count >= 1)
                {// For every selected element
                    CompositionStorage.ChangeCompositionTags(lstItems.SelectedItems);
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            } finally {
                ReList();
            }
        }

        protected void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstItems.SelectedIndex >= 0 && lstItems.SelectedItems.Count >= 1)
                {// For every selected element
                    int currentIndex = lstItems.SelectedIndex/* + i*/;
                    var currentComp = Compositions[lstItems.SelectedIndex];

                    var fileInfo = new FileInfo(currentComp.FilePath);
                    string newName = fileInfo.DirectoryName + "\\" +
                        currentComp.Artist.ArtistName + " – " + currentComp.CompositionName;
                    if (currentComp.Album.Year != null)
                    {
                        if (currentComp.Album.Year < 2100 && currentComp.Album.Year > 1900)
                            newName += $" ({currentComp.Album.Year})";
                    }
                    newName += ".mp3";
                    System.IO.File.Move(fileInfo.FullName, newName);
                    currentComp.FilePath = newName;
                    TimeSpan tS = GetDurationFrom(currentComp);

                    Program.DBAccess.AddComposition(currentComp.Artist, currentComp.Album,
                        currentComp.CompositionName, tS, currentComp.FilePath, null, false, Program.SetCurrentStatus);
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            }
        }

        protected static TimeSpan GetDurationFrom(IComposition currentComp)
        {
            TimeSpan tS;
            if (currentComp.Duration != null)
                tS = TimeSpan.FromSeconds(currentComp.Duration.Value);
            else
                tS = TimeSpan.FromSeconds(int.MinValue);
            return tS;
        }

        protected void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtArtistName.Text = Compositions[lstItems.SelectedIndex].Artist?.ArtistName;
                txtAlbumName.Text = Compositions[lstItems.SelectedIndex].Album?.AlbumName;
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        protected void cmiEnQueue_Click(object sender, RoutedEventArgs e)
        {
            QueueSelected();
        }

        protected void cmiOpenInWinamp_Click(object sender, RoutedEventArgs e)
        {
            CmiPlaySeveral_Click(sender, e);
        }

        protected void cmiOpenLocation_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = lstItems.SelectedIndex/* + i*/;
            if (currentIndex != -1)
            {
                IComposition currentComp = Compositions[lstItems.SelectedIndex];
                if (currentComp.FilePath.FileExists())
                {
                    currentComp.FilePath.ShowFileInExplorer();
                }
            }
        }

        protected void cmiPush_Click(object sender, RoutedEventArgs e)
        {
            QueueSelected(false);
        }

        protected void cmiUpdate_Click(object sender, RoutedEventArgs e)
        {
            ReList();
        }
    }
}
