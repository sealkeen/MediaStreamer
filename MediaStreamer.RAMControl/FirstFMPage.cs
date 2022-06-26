//using DataBaseResource;

using System;
using System.Threading.Tasks;

namespace MediaStreamer.RAMControl
{
    public class FirstFMPage : System.Windows.Controls.Page
    {
        readonly public int PageID;
        public Guid? ownerID { get; set; }
        public bool ListInitialized = false;
        
        protected bool lastDataLoadWasPartial = false;
        protected bool _orderByDescending = true;
        protected static int NewPageID = 0;

        public virtual int ItemsCount()
        {
            return 0;
        }
        public FirstFMPage()
        {
            ListInitialized = false;
            //ownerID =
            //    SessionInformation.UserStatus == LogStatus.Logged ?
            //    SessionInformation.CurrentUser.UserID :
            //    -1;

            PageID = NewPageID;
            NewPageID++;
        }

        public virtual void Rerender()
        {
            
        }

        virtual public void LoadManagementElements()
        {

        }
        virtual public void ShowManagementElements()
        {

        }
        virtual public void HideManagementElements()
        {

        }
        virtual public void List()
        {
            
        }
        virtual public Task ListAsync()
        {
            return null;
        }
        public virtual void ListByID(Guid ID)
        {
            
        }
        public virtual void ListByTitle(string name)
        {
            
        }

        public virtual void ListByUserAndID(Guid userID, Guid id)
        {
            
        }

        public virtual void ClosePageResources() 
        {
            
        }
    }
}
