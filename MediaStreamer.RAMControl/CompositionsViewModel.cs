using MediaStreamer.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.RAMControl
{
    public class CompositionsViewModel : ICompositionsPage
    {
        public CompositionsViewModel()
        {

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

        public void PartialListCompositions(long ArtistID, long albumID)
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

        public int SelectedItemsCount()
        {
            throw new NotImplementedException();
        }

        public virtual void SwitchToNextSelected()
        {
            
        }

        public void SwitchToPreviousSelected()
        {
            throw new NotImplementedException();
        }
    }
}
