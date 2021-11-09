

namespace DMultHandler
{
    /// <summary>
    /// Логика взаимодействия для SignedUpPage.xaml
    /// </summary>
    public partial class SignedUpPage : FirstFMPage
    {
        public SignedUpPage()
        {
            InitializeComponent();
        }

        public SignedUpPage(string signInfo, bool append = true) : this()
        {
            if (append == true)
                txtLogin.Text = $"Successfully signed up as: {signInfo}";
            else
                txtLogin.Text = signInfo;
        }
    }
}
