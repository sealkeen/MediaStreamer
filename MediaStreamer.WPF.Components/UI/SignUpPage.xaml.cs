using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using MediaStreamer.Domain;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUpPage : FirstFMPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void buttonSignUp_Click(object sender, RoutedEventArgs e)
        {
            var user = AddUser();
            AddExtended(user);
        }

        private void buttonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var user = ChangePassword();
            AddExtended(user);
        }

        private User AddUser()
        {
            try
            {
                bool valid = true;
                string email = txtEmail.Text.TrimEnd(' ').TrimStart(' ');
                string psswd = txtPassword.Password;
                string repeatedPsswd = txtRepeatPassword.Password;
                valid = IsValidEmail(email);
                if (!valid) { throw new ArgumentException("Email/Password exception."); }
                valid = IsValidPassword(psswd);
                if (!valid) { throw new ArgumentException("Email/Password exception."); }
                valid = IsReEnteredCorrectly(psswd, repeatedPsswd);
                if (!valid) { throw new ArgumentException("Reenter password exception."); }

                var user = Program.DBAccess.AddNewUser(txtLogin.Text, txtPassword.Password, txtEmail.Text, txtBio.Text);

                txtStatus.Text = "Successfully signed up!";

                Session.MainPage.mainFrame.Content = Session.SignedUpPage ??
                    (Session.SignedUpPage = new SignedUpPage($"{user.UserName ?? "Unknown"}"));
                return user;

            } catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
                return null;
            }
        }

        public User ChangePassword()
        {
            try
            {
                bool valid = true;
                string psswd = txtPassword.Password;
                string repeatedPsswd = txtRepeatPassword.Password;
                valid = IsValidPassword(psswd);
                if (!valid) { throw new Exception("Password exception."); }
                valid = IsReEnteredCorrectly(psswd, repeatedPsswd);
                if (!valid) { throw new Exception("Reenter exception."); }

                var user = Program.DBAccess.DB.GetUsers().First();
                user.Password = Program.DBAccess.ToMD5(psswd);
                Program.DBAccess.DB.SaveChanges();

                txtStatus.Text = "Successfully changed password!";

                Session.MainPage.mainFrame.Content = Session.SignedUpPage == null ?
                    Session.SignedUpPage = new SignedUpPage($"Successfully changed Password to {user.UserName}", false) :
                    Session.SignedUpPage;
                return user;
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
                return null;
            }
        }

        private void AddExtended(User user)
        {
            try
            {
                if (chkModer.IsChecked.Value) {
                    if (Program.DBAccess.DB.GetModerators().Any(m => m.UserID == user.UserID))
                        return;
                    var moder = Program.DBAccess.AddNewModerator(user.UserID);
                    if (chkAdmin.IsChecked.Value) {
                        if (Program.DBAccess.DB.GetAdministrators().Any(a => a.UserID == user.UserID))
                            return;
                        Program.DBAccess.AddNewAdministrator(user.UserID, moder.ModeratorID);
                    }
                }
            } catch (Exception ex) {
                Program.SetCurrentStatus($"SignUpPage Exception in AddExtended(): {ex.Message}");
            }
        }

        private bool IsReEnteredCorrectly(string left, string right)
        {
            bool result = (left == right);

            if (!result)
            {
                txtStatus.Text = "Passwords do not match.";
            }

            return result;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var matches = from user in Program.DBAccess.DB.GetUsers() where user.Email == email select user;

                if ((matches.Count() > 0))
                    throw new ArgumentException("Email is not valid.");

                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                txtStatus.Text = "Email address is not valid.";
                return false;
            }
        }

        bool IsValidPassword(string password)
        {
            if (password.Length < 6)
            {
                txtStatus.Text = "Password length must be more than 6 symbols.";
                return false;
            }
            return true;
        }
    }
}
