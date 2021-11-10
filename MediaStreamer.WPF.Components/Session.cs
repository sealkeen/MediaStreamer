using System;
using System.Collections;
using System.Linq;
using System.Windows.Controls;
using MediaStreamer.Domain;

namespace MediaStreamer.WPF.Components
{
    public class Session : IEnumerable
    {
        public static IDBRepository DBAccess;
        public static CompositionsPage CompositionsPage { get; set; }
        public static AlbumsPage AlbumsPage { get; set; }
        public static ArtistGenresPage AGenresPage { get; set; }
        public static GroupMembersPage MembersPage { get; set; }
        public static ArtistsPage ArtistsPage { get; set; }
        public static GenresPage GenresPage { get; set; }
        public static SignUpPage SignUpPage { get; set; }
        public static MainPage MainPage { get; set; }
        public static UserCompositionsPage ListenedCompositionsPage { get; set; }
        public static UserAlbumsPage ListenedAlbumsPage { get; set; }
        public static UserGenresPage UserGenresPage { get; set; }
        public static SignedUpPage SignedUpPage { get; set; }
        public static TagEditorPage TagEditorPage { get; set; }
        public static VideoPage VideoPage { get; set; }

        public static LoadingPage loadingPage = new LoadingPage();

        private static Session _session = new Session();
        private static System.Reflection.PropertyInfo[] _propertyCollection = _session.GetType().GetProperties();

        public static void UpdatePages()
        {
            //CompositionsPage = new CompositionsPage();
            if ((MainPage.mainFrame.Content as FirstFMPage) is CompositionsPage)
            {
                MainPage.mainFrame.Content = CompositionsPage;
            }
        }

        public static void TryLog(string login, string password)
        {

        }

        public static LogStatus Log(string login, string password)
        {
            if (SessionInformation.UserStatus == LogStatus.Logged)
            {
                return Unlog();
            }
            if (login == string.Empty)
            {
                return LogStatus.LoginIsMissing;
            }
            if (password == string.Empty)
            {
                return LogStatus.PasswordIsMissing;
            }

            var users = from u in Program.DBAccess.DB.GetUsers()
                        where u.UserName == login
                        select u;
            if (users.Count() == 0)
            {
                //we don't reveal that it's incorrect username to keep
                //the existing logins secret 
                //(user could enter both an existing login + incorrect password
                //or inexisting login with a made up password) 
                return LogStatus.LoginPasswordPairIsIncorrect;
            }
            var user = users?.First();
            //byte[] tmpSource; byte[] tmpHash;
            ////Create a byte array from source data.
            //tmpSource = ASCIIEncoding.ASCII.GetBytes(password);
            //tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            string hashedPassword = Program.DBAccess.ToMD5(password);
            if (user?.Password == hashedPassword)
            {
                SessionInformation.CurrentUser = user;
                return LogStatus.Logged;
            }
            else
            {
                return LogStatus.PasswordIsIncorrect;
            }
        }

        public static LogStatus Unlog()
        {
            try
            {
                if (SessionInformation.CurrentUser != null)
                {
                    SessionInformation.CurrentUser = null;
                }
                return LogStatus.Unlogged;
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
                return LogStatus.Error;
            }
        }

        public static void Dispose()
        {
            foreach (System.Reflection.PropertyInfo property in _propertyCollection)
            {
                Page page = new Page();
                property.GetValue(page, null);
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (System.Reflection.PropertyInfo property in _propertyCollection)
            {
                Page page = new Page();
                property.GetValue(page, null);

                yield return page;
            }
        }
    }
    
} //namespace FirstFMCourse