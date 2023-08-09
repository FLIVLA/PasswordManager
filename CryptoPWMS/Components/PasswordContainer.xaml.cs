using CryptoPWMS.Components.ModalChildControls;
using CryptoPWMS.Models;
using CryptoPWMS.Security;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for PasswordContainer.xaml
    /// </summary>
    public partial class PasswordContainer : UserControl
    {
        private PasswordGroup grp;               // Parent container (used when having to update the parent on edit/delete)
        private PasswordItem item;               // Data model object representing a password record in the database.
        private DispatcherTimer timer;           // Timer, for timing clearing any visible decrypted values from the container UI.
        private TimeSpan timespan;               // Timespan for the Dispatcher timer. Initialized to 10 seconds in the constructor.

        public bool IsExpanded { get; set; }     // Represents the expanded state of the expandable section of the container.

        /// <summary>
        /// Initializes the container control with the parent group container
        /// and the PasswordItem DataModel object. Initializes a new Dispatcher Timer
        /// instance (for timing clearing any visible decrypted values).
        /// Based on whether the PasswordItem object has an URL stored, the url button
        /// will initialized as available for quick browser access to the platform url.
        /// </summary>
        /// <param name="grp">Parent group container of the password record.</param>
        /// <param name="item">The password record to be stored in the container.</param>
        public PasswordContainer(PasswordGroup grp, PasswordItem item)
        {
            this.grp = grp;                                 // store argument values in field variables.
            this.item = item;

            InitializeComponent();

            txt_platform.Text = item.Platform;              // Set row text field values.
            txt_lastupdated.Text = item.LastUpdated;
            
            if (!string.IsNullOrEmpty(item.URL)) {          // Set availability and button content of the URL button based on data.
                btn_url.IsEnabled = true;
                btn_url.Content = item.URL;
            }

            timespan = TimeSpan.FromSeconds(10);            // Timespan of 10 seconds (will give the user 10 seconds for viewing temporarilt descrypted values).
            timer = new DispatcherTimer();                  
            timer.Interval = TimeSpan.FromSeconds(1);       // Set timer interval (ticks every 1 seconds).
            timer.Tick += Timer_Tick;                       // Add event listener for the dispatcher timer tick.
        }

        #region ============================= ROW SECTION ==============================

        /// <summary>
        /// Expands/collapses the containers expandable section based
        /// on its current expanded state. On click the expanded state 
        /// is set, and the appropriate storyboard of the controls resources
        /// will be executed (animation of UI elements). 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BtnExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (IsExpanded)
            {
                IsExpanded = false;                                     
                var sb = (Storyboard)this.Resources["sb_collapse"];
                sb.Begin();
            }
            else
            {
                IsExpanded = true;
                var sb = (Storyboard)this.Resources["sb_expand"];
                sb.Begin();
            }
        }

        /// <summary>
        /// Opens the URL of the password item in the default browser
        /// of the current device used. (BONUS FEATURE)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_url_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(item.URL) && !string.IsNullOrWhiteSpace(item.URL))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = item.URL,
                        UseShellExecute = true
                    };

                    Process.Start(psi);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        #endregion

        #region ========================== EXPANDABLE SECTION ==========================

        /// <summary>
        /// Will prompt the user to enter masterpassword for eventually
        /// temporarily decrypting the stored username and password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_unlockPass_Click(object sender, RoutedEventArgs e)
        {
            var p = new Popup(new UnlockForm(this));
            App.MainWin.blurGrid.Visibility = Visibility.Visible;
            p.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var p = new Popup(new NewPasswordForm());
            App.MainWin.blurGrid.Visibility = Visibility.Visible;
            p.ShowDialog();
        }

        /// <summary>
        /// Opens modal containing cancel and final deletion options
        /// to ensure that the user does not accidentally delete records.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var p = new Popup(new DeleteScreen(item));
            App.MainWin.blurGrid.Visibility = Visibility.Visible;
            p.ShowDialog();
        }

        /// <summary>
        /// Copies the temporarily decrypted value of the password
        /// present in the password field to clipboard.
        /// Only available when temporarily unlocked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(pwbx_password.Password);
        }

        /// <summary>
        /// Copies the temporarily decrypted value of the username
        /// present in the username field to clipboard.
        /// Only available when temporarily unlocked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CopyUsername_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txt_username.Text);
        }

        /// <summary>
        /// Will temporarily reveal the password in the password field.
        /// Only available when temporaily unlocked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RevealPassword_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                txt_password.Text = pwbx_password.Password;
                pwbx_password.Visibility = Visibility.Collapsed;
                txt_password.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Will hide the revealed password value in the password field.
        /// Only available when temporarily unlocked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RevealPassword_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                txt_password.Text = string.Empty;
                txt_password.Visibility = Visibility.Collapsed;
                pwbx_password.Visibility = Visibility.Visible;
            }
        }

        #endregion

        /// <summary>
        /// Returs the parent group container.
        /// </summary>
        /// <returns></returns>
        public PasswordGroup GetGrp()
        {
            return grp;
        }

        /// <summary>
        /// On tick the timer will subtract an interval value from
        /// timespan. If it hits zero, it will clear the container of
        /// any decrypted values and reset various states of UI elements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            timespan = timespan.Subtract(timer.Interval);

            if (timespan <= TimeSpan.Zero)
            {
                timer.Stop();
                timespan = TimeSpan.Zero;

                // Set element visibility:
                txt_username_placeholder.Visibility = Visibility = Visibility.Visible;
                txt_password_placeholder.Visibility = Visibility.Visible;                
                btn_RevealPassword.Visibility = Visibility.Collapsed;               
                txt_password.Visibility = Visibility.Collapsed;
                pwbx_password.Visibility = Visibility.Visible;

                btn_CopyUsername.Opacity = 0.8;
                btn_CopyPassword.Opacity = 0.8;

                // Set element availability.
                btn_RevealPassword.IsEnabled = false;
                btn_CopyPassword.IsEnabled = false;
                btn_CopyUsername.IsEnabled = false;

                // Empty fields.
                txt_username.Text = string.Empty;
                pwbx_password.Password = string.Empty;
                txt_password.Text = string.Empty;
            }
        }

        /// <summary>
        /// Temporarily decrypts username and password
        /// </summary>
        /// <param name="key"></param>
        public void Unlock()
        {
            try
            {
                byte[] decryptedSalt = Crypto.Decrypt_AES(item.Salt, App.DerivedKey, App.Salt, item.IV);
                byte[] decryptedKey = Crypto.Decrypt_AES(item.Key, App.DerivedKey, App.Salt, item.IV);

                byte[] encryptedUsername = item.Username;
                byte[] encryptedPassword = item.Password;

                byte[] decryptedUsername = Crypto.Decrypt_AES(encryptedUsername, decryptedKey, decryptedSalt, item.IV);
                byte[] decryptedPassword = Crypto.Decrypt_AES(encryptedPassword, decryptedKey, decryptedSalt, item.IV);

                string username = Encoding.UTF8.GetString(decryptedUsername);
                string password = Encoding.UTF8.GetString(decryptedPassword);

                txt_username.Text = username;
                pwbx_password.Password = password;

                txt_username_placeholder.Visibility = Visibility.Collapsed;
                txt_password_placeholder.Visibility = Visibility.Collapsed;
                btn_RevealPassword.Visibility = Visibility.Visible;

                btn_CopyUsername.Opacity = 1.0;
                btn_CopyPassword.Opacity = 1.0;

                btn_RevealPassword.IsEnabled = true;              
                btn_CopyPassword.IsEnabled = true;
                btn_CopyUsername.IsEnabled = true;

                var sb_exp = FindResource("sb_timerExpiration") as Storyboard;
                timespan = TimeSpan.FromSeconds(10);
                timer.Start();
                sb_exp.Begin();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
