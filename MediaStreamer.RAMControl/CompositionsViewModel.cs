using MediaStreamer.Domain;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace MediaStreamer.RAMControl
{
    public class CompositionsViewModel : ICompositionsViewModel
    {
        protected Guid _lastPartialArtistID = Guid.Empty;
        protected Guid _lastPartialAlbumID = Guid.Empty;
        protected bool _orderByDescending = false;
        public CompositionStorage CompositionsStore { get; set; }
        public bool ListInitialized = false;
        public int LastCompositionIndex = -1;
        private int _skip = 0;
        private int _take = 10;
        public CompositionsViewModel()
        {
            Session.CompositionsVM = this;
            Session.CompositionsVM.CompositionsStore = new CompositionStorage();
        }

        public bool LastDataLoadWasPartial()
        {
            return _lastPartialAlbumID != Guid.Empty;
        }

        public void ResetPartialLoad()
        {
            _lastPartialArtistID = Guid.Empty;
            _lastPartialAlbumID = Guid.Empty;
        }

        public void SetLastAlbumAndArtistID(Guid albumID, Guid artistID)
        {
            _lastPartialAlbumID = albumID;
            _lastPartialArtistID = artistID;
        }

        public async Task PartialListCompositions(Guid albumID, Guid artistID)
        {
            SetLastAlbumAndArtistID(albumID, artistID);
            ListInitialized = false;
#if !NET40
            await Task.Factory.StartNew(GetPartOfCompositions);
#else
            GetPartOfCompositions();
#endif
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

        public CompositionsViewModel(Guid ArtistID, Guid albumID)
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

        public void MovePageNext(int count)
        {
            _skip += count;
        }
    }
}
