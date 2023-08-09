using CryptoPWMS.IO;
using CryptoPWMS.Security;
using CryptoPWMS.Utils;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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
                Crypto.DecryptVault(App.Cur_User, App.DerivedKey);
                App.IsAuthenticated = true;
                App.MainUI.FillPasswordData();
                UI_Transitions.Fade(this, App.MainUI);
                ResetLoginSection();
            }
            catch { 
                MessageBox.Show("Invalid key!");
                var tempFiles = Directory.GetFiles(Vaults.TempDir);
                foreach (var file in tempFiles)
                {
                    File.Delete(file);
                }
                if (App.HomeScreen.Visibility == Visibility.Visible
                    && App.HomeScreen.form_login.Visibility == Visibility.Visible)
                {
                    App.HomeScreen.ResetLoginSection();
                }
                return;
            }
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

            bool res = Users.Insert(username, password);            // Insert new user in the user database.
            if (res)
            {
                string un = Users.GetUsername(username, password);
                if (un != "")
                {
                    App.Cur_User = un;                              // Set current user_Id in Application.
                    Vaults.Create(un);                              // Create new vault instance for the user.

                    UI_Transitions.Fade(                            // Navigate to masterkey form.
                        createAccount_credentialsForm,
                        createAccount_masterkeyForm);
                }

                txt_newUsername.Text = string.Empty;                // Empty the user input (username and password).
                pwbx_newPassword.Password = string.Empty;
            }
            else ResetCreateAccountForm();
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

            Crypto.EncryptVault(App.Cur_User, App.DerivedKey);

            ResetLoginSection();
            var nav_sb = FindResource("sb_navto_login") as Storyboard;
            nav_sb.Begin();
            ResetCreateAccountForm();

            txt_username.IsEnabled = true;
            pwbx_password.IsEnabled = true;

            pwbx_newMasterkey.Password = string.Empty;

            App.DerivedKey = new byte[0];
            App.Salt = new byte[0];
            App.Cur_User = "";
        }

        /// <summary>
        /// Navigates to the signup section of the homescreen.
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

            placeholder_username.Visibility = Visibility.Visible;
            placeholder_pw.Visibility = Visibility.Visible;
            placeholder_mk.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Navigates to the login section of the homescreen.
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

            placeholder_newusername.Visibility = Visibility.Visible;
            placeholder_newpw.Visibility = Visibility.Visible;
            placeholder_newmk.Visibility = Visibility.Visible;
        }

        public void ResetLoginSection()
        {
            UI_Transitions.Fade(login_masterkeyForm, login_credentialsForm);
            txt_username.Text = string.Empty;
            pwbx_password.Password = string.Empty;
            pwbx_masterkey.Password = string.Empty;
            placeholder_username.Visibility = Visibility.Visible;
            placeholder_pw.Visibility = Visibility.Visible;
            placeholder_mk.Visibility = Visibility.Visible;
        }

        public void ResetCreateAccountForm()
        {
            UI_Transitions.Fade(createAccount_masterkeyForm, createAccount_credentialsForm);
            txt_newUsername.Text = string.Empty;
            pwbx_newPassword.Password = string.Empty;
            pwbx_newMasterkey.Password = string.Empty;
            placeholder_newusername.Visibility = Visibility.Visible;
            placeholder_newpw.Visibility = Visibility.Visible;
            placeholder_newmk.Visibility = Visibility.Visible;
        }

        private bool ValidateMasterKey(string mk)
        {
            string upperLowerPattern = @"^(?=.*[a-z])(?=.*[A-Z])";
            string digitPattern = @"(?=.*\d)";
            string symbolPattern = @"(?=.*[$@|^#?+])";
            string lengthPattern = @".{10,}"; // Minimum length of 10 characters
            string combinedPattern = upperLowerPattern + digitPattern + symbolPattern + lengthPattern;

            return Regex.IsMatch(mk, combinedPattern);
        }

        #region ============================ FOCUS ==========================

        /* GotFocus and LostFocus event handlers for all text fields of the 
         * homescreen. handles visibility state of placeholder elements based
         * on the active control and its current value.
         */

        private void pwbx_password_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholder_pw.Visibility= Visibility.Collapsed;
        }

        private void pwbx_password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pwbx_password.Password) || string.IsNullOrWhiteSpace(pwbx_password.Password))
            {
                placeholder_pw.Visibility = Visibility.Visible;
            }
        }

        private void txt_username_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholder_username.Visibility = Visibility.Collapsed;
        }

        private void txt_username_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_username.Text) || string.IsNullOrWhiteSpace(txt_username.Text))
            {
                placeholder_username.Visibility = Visibility.Visible;
            }
        }

        private void pwbx_newPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholder_newpw.Visibility = Visibility.Collapsed;
        }

        private void pwbx_newPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pwbx_newPassword.Password) || string.IsNullOrWhiteSpace(pwbx_newPassword.Password))
            {
                placeholder_newpw.Visibility = Visibility.Visible;
            }
        }

        private void txt_newUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholder_newusername.Visibility = Visibility.Collapsed;
        }

        private void txt_newUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_newUsername.Text) || string.IsNullOrWhiteSpace(txt_newUsername.Text))
            {
                placeholder_newusername.Visibility = Visibility.Visible;
            }
        }

        private void pwbx_newMasterkey_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholder_newmk.Visibility = Visibility.Collapsed;
        }

        private void pwbx_newMasterkey_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pwbx_newMasterkey.Password) || string.IsNullOrWhiteSpace(pwbx_newMasterkey.Password))
            {
                placeholder_newmk.Visibility = Visibility.Visible;
            }
        }

        private void pwbx_masterkey_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholder_mk.Visibility = Visibility.Collapsed;
        }

        private void pwbx_masterkey_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pwbx_masterkey.Password) || string.IsNullOrWhiteSpace(pwbx_masterkey.Password))
            {
                placeholder_mk.Visibility = Visibility.Visible;
            }
        }


        #endregion

        private void pwbx_newMasterkey_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ValidateMasterKey(pwbx_newMasterkey.Password))
            {
                btn_chooseNewMasterKey.IsEnabled = true;
                btn_chooseNewMasterKey.Opacity = 1.0;
            }
            else
            {
                btn_chooseNewMasterKey.IsEnabled = false;
                btn_chooseNewMasterKey.Opacity = 0.7;
            }
        }
    }
}
