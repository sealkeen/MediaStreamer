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
using MediaStreamer.RAMControl;
using System.Threading;

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
            Selector.MainPage.SetFrameContent( Selector.LoadingPage ); //LoadManagementElements(); //ListCompositionsAsync();
            InitializeComponent();

            Session.CompositionsVM = new CompositionsViewModel();
            DataContext = Session.CompositionsVM.CompositionsStore;
            //tsk.Wait();
            Selector.MainPage.SetFrameContent(this);
        }
        public CompositionsPage(long ArtistID, long albumID)
        {
            Session.CompositionsVM.CompositionsStore.Compositions = new List<IComposition>();
            InitializeComponent();
            LoadManagementElements();
            Session.CompositionsVM.SetLastAlbumAndArtistID(ArtistID, albumID);
            DataContext = Session.CompositionsVM;
        }

        //public IList<IComposition> Compositions { get; set; }
        //public LinkedList<Composition> Queue { get; set; }

        double _oldRoamingGroupWidth = 0.0;

        protected void ListView_OnColumnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _orderByDescending = !_orderByDescending;
                string columnName = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
                switch (columnName)
                {
                    case "Composition":
                        Session.CompositionsVM.CompositionsStore.Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.CompositionName, _orderByDescending).ToList();
                        break;
                    case "Artist":
                        Session.CompositionsVM.CompositionsStore.Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.Artist.ArtistName, _orderByDescending).ToList();
                        break;
                    case "Duration (sec)":
                        Session.CompositionsVM.CompositionsStore.Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.Duration, _orderByDescending).ToList();
                        break;
                    case "File Path":
                        Session.CompositionsVM.CompositionsStore.Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.FilePath, _orderByDescending).ToList();
                        break;
                }
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            }
        }

        public override void ListByID(long albumID)
        {
            if (!Program.DBAccess.DB.GetAlbums().Where(a => a.AlbumID == albumID).Any())
                return;

            Session.CompositionsVM.SetLastAlbumAndArtistID(albumID, -1);
            Session.CompositionsVM.GetPartOfCompositions();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public virtual List<IComposition> GetICompositions()
        {
            try {
                //DBAccess.Update();
                var result = Program.DBAccess.DB.GetICompositions().ToList();
                Dispatcher.BeginInvoke(new Action(() => Selector.MainPage.SetFrameContent(Selector.CompositionsPage)));
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

        public override async Task ListAsync()
        {
            try
            {
#if !NET40
                var listCompsTask = Task.Factory.StartNew(GetICompositions);
                //var tsk = await listCompsTask;
                listCompsTask.Wait();
                Session.CompositionsVM.CompositionsStore.Compositions = listCompsTask.Result;
#else
                Session.CompositionsVM.CompositionsStore.Compositions = GetICompositions();
#endif
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                lastDataLoadWasPartial = false;
            }
            catch {
                Session.CompositionsVM.CompositionsStore.Compositions = new List<IComposition>();
                try {
                    Session.CompositionsVM.CompositionsStore.Compositions = GetICompositions();
                } catch { 

                }
            }
        }

        [MTAThread]
        public void SetPartialCompositions(List<IComposition> compositions)
        {
            Session.CompositionsVM.CompositionsStore.Compositions = compositions;
            Session.CompositionsVM.PartialListCompositions();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
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
            return ((Session.CompositionsVM.LastCompositionIndex + 1) < Session.CompositionsVM.CompositionsStore.Compositions.Count());
        }
        public bool HasFirstElement()
        {
            return (Session.CompositionsVM.CompositionsStore.Compositions != null && Session.CompositionsVM.CompositionsStore.Compositions.Count > 1);
        }

        public bool HasNextInQueue()
        {
            return !Session.CompositionsVM.CompositionsStore.Queue.Empty();
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
                return Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex];
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
                    return Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex - 1];
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
                if (!Session.CompositionsVM.CompositionsStore.Queue.Empty())
                {
                    return Session.CompositionsVM.CompositionsStore.Queue.Dequeue();
                }
                if (HasNextInList()) {
                    return Session.CompositionsVM.CompositionsStore.Compositions[(Session.CompositionsVM.LastCompositionIndex + 1)];
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
                Session.CompositionsVM.LastCompositionIndex = lstItems.SelectedIndex;
                lstItems.SelectedIndex -= 1;
            }
        }

        public void SwitchToNextSelected()
        {
            if (HasNextInList() && Session.CompositionsVM.CompositionsStore.Queue.Empty())
            {
                if (lstItems.SelectedIndex + 1 < lstItems.Items.Count)
                {
                    Session.CompositionsVM.LastCompositionIndex = lstItems.SelectedIndex;
                    lstItems.SelectedIndex += 1;
                } else if (HasFirstElement() && Session.CompositionsVM.CompositionsStore.Queue.Empty()) {
                    Session.CompositionsVM.LastCompositionIndex = -1;
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
                    EnqueueOrPush(Session.CompositionsVM.CompositionsStore.Queue.Enqueue, 
                        Session.CompositionsVM.CompositionsStore.Queue.Push, 
                        composition.GetInstance(), queueOrPush);
                }
                if (!Program.mediaPlayerIsPlaying && HasNextInQueue())
                {
                    PlayTarget(GetNextComposition());
                }
                Program.ShowQueue(selectedItems);
                Program.AddToStatus(" Queue: { ");

                Program.AddToStatus(Program.ToString(Session.CompositionsVM.CompositionsStore.Queue));
                Program.AddToStatus(" }.");
                lstQuery.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        public void QueueSelected(bool queueOrPush = true)
        {
            QueueSelected(lstItems.SelectedItems, queueOrPush);
            lstQuery.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
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
                Selector.MainPage.SetAction($"Now playing {art ?? "Unknown"} – " + $"{comp ?? "Unknown"}");
                Program.AddToStatus(", Queued: {");
                Program.AddToStatus(Program.ToString(Session.CompositionsVM.CompositionsStore.Queue));
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
            if (lstItems.SelectedIndex < 0)
                return;

            PlayTarget(Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex]);
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
                    Selector.TagEditorPage = new TagEditorPage(files, compositions);
                    Selector.MainPage.SetFrameContent( Selector.TagEditorPage );
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
                    Program.DBAccess?.DeleteComposition(Session.CompositionsVM.CompositionsStore.Compositions[index.Value].GetInstance());
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
                ListAsync();
            }
            else
            {
                Session.CompositionsVM.PartialListCompositions();
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

        protected void ApplyToSelectedItems(Action<List<IComposition>> action)
        {
            try
            {
                if (lstItems.SelectedIndex >= 0 && lstItems.SelectedItems.Count >= 1)
                {// For every selected element
                    IComposition currentComp = Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex];
                    List<IComposition> compositions = new List<IComposition>();
                    foreach (var song in lstItems.SelectedItems)
                    {
                        currentComp = Session.CompositionsVM.CompositionsStore.Compositions[lstItems.Items.IndexOf(song)];
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
                    Session.CompositionsVM.CompositionsStore.ChangeCompositionTags(lstItems.SelectedItems);
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
                    var currentComp = Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex];

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
                txtArtistName.Text = Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex].Artist?.ArtistName;
                txtAlbumName.Text = Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex].Album?.AlbumName;
            }
            catch (Exception ex)
            {
                //Program.SetCurrentStatus(ex.Message);
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
                IComposition currentComp = Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex];
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

        private async void lstItems_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                //HandleFileOpen(files[0]);

                List<string> lstFiles = new List<string>(files);
                var tsk = await Task.Factory.StartNew( () => Program.FileManipulator.DecomposeAudioFiles(lstFiles, Program.SetCurrentStatus) );
                ReList();
            }
        }

        double _oldLstItemsWidth = 0.0;
        double _oldGridWidth = 0.0;
        double _oldWindowWidth = 0.0;
        private void GridSplitter_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            string debug = "";
            debug = "listWidth: " + lstItems.ActualWidth.ToString();
            debug += " gridWidth: " + _oldGridWidth;
            for (int i = 0; i < mainGrid.ColumnDefinitions.Count; i++)
            {
                debug += $" Col#{i}<" + mainGrid.ColumnDefinitions[i].ActualWidth + "> ";
            }
            Program.SetCurrentStatus(debug);

            _oldLstItemsWidth = lstItems.ActualWidth;
            _oldWindowWidth = pageControl.ActualWidth;
            _oldGridWidth = mainGrid.ActualWidth;
            //_oldRoamingGroupWidth = firstColumn.ActualWidth + secondColumn.ActualWidth + thirdColumn.ActualWidth;
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            double newGridWidth = mainGrid.ActualWidth;
            double newLstItemsWidth = lstItems.ActualWidth;
            if (newLstItemsWidth > _oldWindowWidth - 175)
            {
                //mainGrid.Width = _oldGridWidth;
            }

            string debug = "";
            debug += "listWidth: " + lstItems.ActualWidth.ToString();
            debug += " gridWidth: " + newGridWidth;
            for (int i = 0; i < mainGrid.ColumnDefinitions.Count; i++)
            {
                debug += $" Col#{i}<" + mainGrid.ColumnDefinitions[i].ActualWidth + "> ";
            }
            Program.SetCurrentStatus(debug);
            //double newRoamingGroupWidth = firstColumn.ActualWidth + secondColumn.ActualWidth + thirdColumn.ActualWidth;
            //if (newRoamingGroupWidth > (mainGrid.ActualWidth - 120))
            //{
            //    double changedWidth = _oldRoamingGroupWidth - newRoamingGroupWidth;
            //    if (changedWidth > 0)
            //    {
            //        //thirdColumn.
            //        thirdColumn.Width = new GridLength(thirdColumn.ActualWidth - changedWidth, GridUnitType.Auto);
            //    }
            //}
        }

        private void lstItems_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    if (e.Source != null)
            //    {
            //        List<IComposition> myList = new List<IComposition>();
            //        foreach (IComposition Item in lstItems.SelectedItems)
            //        {
            //            myList.Add(Item);
            //        }

            //        DataObject dataObject = new DataObject(myList);
            //        DragDrop.DoDragDrop(lstItems, dataObject, DragDropEffects.Move);
            //    }
            //}
        }

        private LinkedList<object> _selectedItems = new LinkedList<object>();
        protected void LstItems_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lstItems.SelectedItems.Count <= 0)
                return;

            _selectedItems.Clear();
            foreach (object i in lstItems.SelectedItems)
                _selectedItems.Enqueue(i);

            var tsk = Task.Factory.StartNew(() =>
                    lstItems_MouseLeftButtonDown(sender, e.ButtonState, e.Source, e.OriginalSource, _selectedItems)
            );
        }

        private void lstItems_MouseLeftButtonDown(object sender, 
            MouseButtonState state, 
            object source, 
            object originalSource, 
            IEnumerable SelectedItems)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(delegate
                {
                    if (null == (((FrameworkElement)originalSource).DataContext as Composition))
                        return;

                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ||
                        Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)
                    )
                    return;

                    //lstItems.SelectedItems.Add((((FrameworkElement)e.OriginalSource).DataContext as Composition));
                    if (state == MouseButtonState.Pressed)
                    {
                        if (source != null)
                        {
                            DataObject dataObject = new DataObject(SelectedItems);
                            DragDrop.DoDragDrop(lstItems, dataObject, DragDropEffects.Move);
                        }
                    }
                })
            );
        }

        private void lstQuery_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(LinkedList<object>)))
            {
                // Note that you can have more than one file.
                LinkedList<object> comps = (LinkedList<object>)e.Data.GetData(typeof(LinkedList<object>));

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                //HandleFileOpen(files[0]);
                foreach(var c in comps)
                    Session.CompositionsVM.CompositionsStore.Queue.AddLast(c as Composition);
                ReList();
            }
        }
    }
}
