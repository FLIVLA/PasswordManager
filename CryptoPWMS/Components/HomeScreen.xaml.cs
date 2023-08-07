using CryptoPWMS.IO;
using CryptoPWMS.Utils;
using System.Windows;
using System.Windows.Controls;

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        public HomeScreen()
        {
            InitializeComponent();
            title_L.Text = TextTags.AppTitle();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            var un = txt_username.Text;
            var pw = pwbx_password.Password;
            var uid = Users.Uid(un, pw);
            if (uid != -1)
            {
                App.Cur_Uid = uid;
                App.MainUI.FillPasswordData();
                UI_Transitions.Fade(this, App.MainUI);               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_navToSignup_Click(object sender, RoutedEventArgs e)
        {
            txt_username.IsEnabled = false;
            pwbx_password.IsEnabled = false;
        }

        private void btn_navToLogin_Click(object sender, RoutedEventArgs e)
        {
            txt_username.IsEnabled = true;
            pwbx_password.IsEnabled = true;
        }

        /// <summary>
        /// Creates new user in the database, using the entered credentials
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_signup_Click(object sender, RoutedEventArgs e)
        {
            Users.Insert(txt_newUsername.Text, pwbx_newPassword.Password);
        }
    }
}
