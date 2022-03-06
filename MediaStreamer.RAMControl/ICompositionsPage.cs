using MediaStreamer.Domain;
using System.Collections;

namespace MediaStreamer.RAMControl
{
    interface ICompositionsPage
    {
        IComposition GetCurrentComposition();
        void SwitchToNextSelected();
        void PlayTarget(IComposition composition);
        void PartialListCompositions(long ArtistID, long albumID);
        void QueueSelected(bool queueOrPush = true);
        void QueueSelected(IList selectedItems, bool queueOrPush = true);
        bool HasNextInListOrQueue();
        bool HasPreviousInList();
        void SwitchToPreviousSelected();
        void ChangeComposition(IList selectedItems);
        IList SelectedItems(); 
        IComposition GetNextComposition();
        int SelectedItemsCount();
    }
}
