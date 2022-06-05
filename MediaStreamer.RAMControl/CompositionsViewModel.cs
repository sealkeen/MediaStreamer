using MediaStreamer.Domain;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MediaStreamer.RAMControl
{
    public class CompositionsViewModel : ICompositionsPage
    {
        protected long _lastPartialArtistID = -1;
        protected long _lastPartialAlbumID = -1;
        protected bool _orderByDescending = false;
        public bool ListInitialized = false;
        public int LastCompositionIndex = -1;
        public CompositionStorage CompositionsStore { get; set; }
        public CompositionsViewModel()
        {
            Session.CompositionsVM = this;
            Session.CompositionsVM.CompositionsStore = new CompositionStorage();
        }

        public bool LastDataLoadWasPartial()
        {
            return _lastPartialAlbumID != -1;
        }

        public void ResetPartialLoad()
        {
            _lastPartialArtistID = -1;
            _lastPartialAlbumID = -1;
        }

        public void SetLastAlbumAndArtistID(long albumID, long artistID)
        {
            _lastPartialAlbumID = albumID;
            _lastPartialArtistID = artistID;
        }

        public async void PartialListCompositions(long albumID, long artistID = -1)
        {
            SetLastAlbumAndArtistID(albumID, artistID);
            ListInitialized = false;
            await Task.Factory.StartNew(GetPartOfCompositions);
        }
        public async void PartialListCompositions()
        {
            ListInitialized = false;
            await Task.Factory.StartNew(GetPartOfCompositions);
        }

        public void GetPartOfCompositions()
        {
            CompositionsStore.Compositions = (from composition in Program.DBAccess.DB.GetICompositions()
                                where composition.AlbumID == _lastPartialAlbumID
                                select composition).ToList();
        }

        public CompositionsViewModel(long ArtistID, long albumID)
        {

        }

        public void ChangeComposition(IList selectedItems)
        {
            throw new NotImplementedException();
        }

        public virtual IComposition GetCurrentComposition()
        {
            return new Composition();
        }

        public IComposition GetNextComposition()
        {
            throw new NotImplementedException();
        }

        public bool HasNextInListOrQueue()
        {
            throw new NotImplementedException();
        }

        public bool HasPreviousInList()
        {
            throw new NotImplementedException();
        }


        public virtual void PlayTarget(IComposition composition)
        {
            
        }

        public void QueueSelected(bool queueOrPush = true)
        {
            throw new NotImplementedException();
        }

        public void QueueSelected(IList selectedItems, bool queueOrPush = true)
        {
            throw new NotImplementedException();
        }

        public IList SelectedItems()
        {
            throw new NotImplementedException();
        }

        public virtual int SelectedItemsCount()
        {
            throw new NotImplementedException();
        }

        public virtual void SwitchToNextSelected()
        {
            
        }

        public void SwitchToPreviousSelected()
        {

        }
    }
}
