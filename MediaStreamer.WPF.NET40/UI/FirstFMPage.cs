using DataBaseResource;

namespace DMultHandler
{
    public class FirstFMPage : System.Windows.Controls.Page
    {
        public long? ownerID { get; set; }
        public FirstFMPage()
        {
            ownerID =
                SessionInformation.UserStatus == LogStatus.Logged ?
                SessionInformation.CurrentUser.UserID :
                -1;
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
    }
}
