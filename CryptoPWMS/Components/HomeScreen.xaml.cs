using CryptoPWMS.IO;
using CryptoPWMS.Security;
using CryptoPWMS.Utils;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Xml.Linq;

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
            string username = txt_username.Text;
            string password = pwbx_password.Password;

            string un = Users.GetUsername(username, password);
            if (un != "")
            {
                App.Cur_User = un;
                UI_Transitions.Fade(login_credentialsForm, login_masterkeyForm);    // Navigate to masterpassword form.
            }

            txt_username.Text = string.Empty;
            pwbx_password.Password = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DecryptVault_Click(object sender, RoutedEventArgs e)
        {
            string mp = pwbx_masterkey.Password;
            byte[] derivedKey = Crypto.DeriveKey(mp, App.Salt);
            App.DerivedKey = derivedKey;

            try
            {
                Crypto.DecryptVault(
                    Path.Combine(Vaults.BaseDir, $"{App.Cur_User}.db.cryptovault"), 
                    App.DerivedKey
                );

                App.MainUI.FillPasswordData();
                UI_Transitions.Fade(this, App.MainUI);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }
        }

        /// <summary>
        /// Creates new user in the database, using the entered credentials
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_signup_Click(object sender, RoutedEventArgs e)
        {
            string username = txt_newUsername.Text;
            string password = pwbx_newPassword.Password;

            Users.Insert(username, password);                   // Insert new user in the user database.
            string un = Users.GetUsername(username, password);
            if (un != "")
            {
                App.Cur_User = un;                              // Set current user_Id in Application.
                Vaults.Create($"{un}.db");                      // Create new vault instance for the user.
                
                UI_Transitions.Fade(                            // Navigate to masterkey form.
                    createAccount_credentialsForm, 
                    createAccount_masterkeyForm);
            }

            txt_newUsername.Text = string.Empty;                // Empty the user input (username and password).
            pwbx_newPassword.Password = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_chooseNewMasterKey_Click(object sender, RoutedEventArgs e)
        {
            string mp = pwbx_newMasterkey.Password;
            byte[] derivedKey = Crypto.DeriveKey(mp, App.Salt);
            App.DerivedKey = derivedKey;

            Crypto.EncryptVault(
                Path.Combine(Vaults.BaseDir, $"{App.Cur_User}.db"), 
                App.DerivedKey
            );

            var nav_sb = FindResource("sb_navto_login") as Storyboard;
            nav_sb.Begin();

            txt_username.IsEnabled = true;
            pwbx_password.IsEnabled = true;

            pwbx_newMasterkey.Password = string.Empty;

            App.DerivedKey = new byte[0];
            App.Salt = new byte[0];
            App.Cur_User = "";
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

            txt_newUsername.IsEnabled = true;
            pwbx_newPassword.IsEnabled = true;

            txt_username.Text = string.Empty;
            pwbx_password.Password = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_navToLogin_Click(object sender, RoutedEventArgs e)
        {
            txt_username.IsEnabled = true;
            pwbx_password.IsEnabled = true;

            txt_newUsername.IsEnabled = false;
            pwbx_newPassword.IsEnabled = false;

            txt_newUsername.Text = string.Empty;
            pwbx_newPassword.Password = string.Empty;
        }

        public void ResetLoginSection()
        {
            txt_username.IsEnabled = true;
            pwbx_password.IsEnabled = true;

            txt_newUsername.IsEnabled = false;
            pwbx_newPassword.IsEnabled = false;

            txt_newUsername.Text = string.Empty;
            pwbx_newPassword.Password = string.Empty;
        }
    }
}
