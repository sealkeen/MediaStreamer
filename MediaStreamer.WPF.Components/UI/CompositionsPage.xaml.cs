using MediaStreamer.Domain;
using MediaStreamer.Logging;
using MediaStreamer.RAMControl;
using Sealkeen.Linq.Extensions;
using StringExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для Compositions.xaml
    /// </summary>
    public partial class CompositionsPage : FirstFMPage
    {
        protected Control CurrentListView { get; set; }
        public CompositionsPage()
        {
            try
            {
                Program._logger?.LogTrace($"The new position is : {Program.NewPosition}");
                Program._logger?.LogTrace($"Creating CompositionsPage(), Program.startupFromCommandLine = {Program.startupFromCommandLine}");
                Program._logger?.LogTrace($"Current composition is not nuLL = {Program.currentComposition != null}" + Environment.NewLine +
                    $"Current composition path is not nuLL = {Program.currentComposition?.FilePath != null}");
                //cmd mode play from cmd parameter file
                if (Program.startupFromCommandLine)
                {
                    Program.SetPlayerPositionToZero();
                    Program.mePlayer.Source = new Uri(Program.currentComposition.FilePath);
                    Program._logger?.LogTrace($"Setting Program.mePlayer.Source = {Program.currentComposition.FilePath}, " +
                        $"File valid = {File.Exists(Program.currentComposition.FilePath)}");
                }
            }
            catch (Exception ex) 
            { 
                Program.SetCurrentStatus("CompositionsPage(): " + ex.Message); 
            }

            if (Session.CompositionsVM == null)
                Session.CompositionsVM = new CompositionsViewModel();

            ListInitialized = false;

            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage())); //LoadManagementElements(); //ListCompositionsAsync();
            InitializeComponent();

            Program._logger?.LogTrace($"Setting valid DataContext = {Session.CompositionsVM.CompositionsStore != null}");

            DataContext = Session.CompositionsVM.CompositionsStore;
            //tsk.Wait();
            Selector.MainPage.SetFrameContent(this);

            Program._logger?.LogTrace($"The new position is : {Program.NewPosition}");
        }

        public CompositionsPage(Guid ArtistID, Guid albumID)
        {
            Session.CompositionsVM.CompositionsStore.Compositions = new List<IComposition>();
            InitializeComponent();
            LoadManagementElements();
            Session.CompositionsVM.SetLastAlbumAndArtistID(ArtistID, albumID);
            DataContext = Session.CompositionsVM;
        }

        public async override void ListByID(Guid albumID)
        {
            if (!Program.DBAccess.DB.GetCompositions().Where(a => a.AlbumID == albumID).Any())
                return;
#if !NET40
            await Session.CompositionsVM.PartialListCompositions(albumID, Guid.Empty);
#else
            Session.CompositionsVM.PartialListCompositions(albumID, Guid.Empty).Wait();
#endif
            //Session.CompositionsVM.GetPartOfCompositions();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public async virtual Task<List<IComposition>> GetICompositions()
        {
            try {
                //DBAccess.Update();
                Dispatcher.BeginInvoke(new Action(() => Selector.MainPage.SetFrameContent(Selector.CompositionsPage)));
                var result =
                    await Program.DBAccess.DB.GetICompositionsAsync(Session.MainPageVM.GetSkip(), 10);
                ListInitialized = true;
                return result;
            } catch (Exception ex) {
                Program.SetCurrentStatus("GetICompositions: " + ex.Message);
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
            try {
                Session.CompositionsVM.CompositionsStore.Compositions = await GetICompositions();
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                lastDataLoadWasPartial = false;
            } catch {
                Session.CompositionsVM.CompositionsStore.Compositions = new List<IComposition>();
                try {
                    await GetICompositions();
                    lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                    lastDataLoadWasPartial = false;
                }
                catch (Exception ex) {
                    Program.SetCurrentStatus("ListAsync: " + ex.Message);
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
                Program.SetCurrentStatus("GetCurrentComposition(): " + ex.Message, true);
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
                Program.SetCurrentStatus("GetPreviousComposition(): " + ex.Message, true);
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
                Program.SetCurrentStatus("GetNextComposition(): " + ex.Message, true);
                return null;
            }
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
                if (selectedItems == null || selectedItems.Count == 0)
                    return;
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

                if (SessionInformation.CurrentUser == null)
                {
                    SessionInformation.CurrentUser = new User() { UserID = Guid.Empty };
                }
                //Program.DBAccess?.ClearListenedCompositions();
                Program.DBAccess?.AddNewListenedComposition(target.GetInstance(), SessionInformation.CurrentUser);
                if (Selector.ListenedCompositionsPage != null)
                {
                    Task.Factory.StartNew( () => Selector.ListenedCompositionsPage.List() );
                }
                if (lstItems.SelectedIndex < 0 || lstItems.SelectedItem.GetHashCode() != target.GetHashCode())
                {
                    lstItems_TryToSelectItem(target);
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus($"Error to double click PlayTarget(): {ex?.Message}");
            }
        }

        public void PlaySelectedTarget()
        {
            if (lstItems.SelectedIndex < 0)
                return;
            if (Session.CompositionsVM.CompositionsStore.Compositions != null &&
                Session.CompositionsVM.CompositionsStore.Compositions.Count > lstItems.SelectedIndex)
                PlayTarget(Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex]);
            else
            {       
                Program.SetCurrentStatus("ViewModel for Compositions was not initialized correctly.");
                return;
            }
            Program.currentComposition = Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex];
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
            try
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
            } catch (Exception ex) {
                Program._logger?.LogError("RenameCompositionFiles" + ex.Message);
            }
        }

        // GridSplitter -->
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
        // <-- GridSplitter 
        public override void Rerender()
        {
            this.InitializeComponent();
        }

    }
}
