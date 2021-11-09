using DataBaseResource;

namespace DMultHandler
{
    public class OwnedPage : System.Windows.Controls.Page
    {
        public long? ownerID { get; set; }
        public OwnedPage()
        {
            ownerID =
                SessionInformation.UserStatus == LogStatus.Logged ?
                SessionInformation.CurrentUser.UserID :
                -1;
        }
    }
}
