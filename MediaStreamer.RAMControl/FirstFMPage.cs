//using DataBaseResource;

using System.Threading.Tasks;

namespace MediaStreamer.RAMControl
{
    public class FirstFMPage : System.Windows.Controls.Page
    {
        public long? ownerID { get; set; }
        public bool ListInitialized = false;
        protected bool lastDataLoadWasPartial = false;
        protected bool _orderByDescending = true;

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
        public virtual void ListByID(long ID)
        {
            
        }
        public virtual void ListByTitle(string name)
        {
            
        }

        public virtual void ListByUserAndID(long userID, long id)
        {
            
        }

        public virtual void ClosePageResources() 
        {
            
        }
    }
}
