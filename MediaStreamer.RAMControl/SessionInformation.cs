using MediaStreamer.Domain;
using System;

namespace MediaStreamer.RAMControl
{
    public class SessionInformation
    {
        public static LogStatus UserStatus
        {
            get
            {
                if (CurrentUser == null)
                    return LogStatus.Unlogged;
                else
                    return LogStatus.Logged;
            }
        }

        public static bool Loading = false;
        public static User CurrentUser = null;

        public static bool UserListenedInfoUpToDate()
        {
            try
            {
                return
                (CurrentUser.LastListenEntitiesUpdate.HasValue &&
                CurrentUser.LastListenedCompositionChange.HasValue) ||
                (CurrentUser.LastListenEntitiesUpdate.Value ==
                CurrentUser.LastListenedCompositionChange);
            }
            catch (Exception ex)
            {
                //Program.HandleException(ex);
                return false;
            }
        } // UserListenedInfoUpToDate()
    } // public class Session Information
}
