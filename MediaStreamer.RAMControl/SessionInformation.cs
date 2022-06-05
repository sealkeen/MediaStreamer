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

    } // public class Session Information
}
