using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CryptoPWMS.Components.ModalChildControls
{
    /// <summary>
    /// Interaction logic for UnlockForm.xaml
    /// </summary>
    public partial class UnlockForm : UserControl
    {
        public Window Parent { get; set; }                          // The parent modal hosting the form instance. Set to enable closing the modal instance from the control.
        private PasswordContainer passwordContainer { get; set; }   // Pointer to the password of which the unlock is attempted.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pwc"></param>
        public UnlockForm(PasswordContainer pwc)
        {
            passwordContainer = pwc;
            InitializeComponent();
        }

        /// <summary>
        /// Calls the unlock method of the PasswordContainer instance which
        /// the form was initialized with. If Successful, this will temporarily
        /// decrypt the passworditem credentials making them available to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_unlock_Click(object sender, RoutedEventArgs e)
        {
            passwordContainer.Unlock();
            App.MainWin.blurGrid.Visibility = Visibility.Collapsed;     // Collapse blurgrid of main UI
            Parent.Close();                                             // closes the modal.
        }

        /// <summary>
        /// Collapses placeholder element on GotFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pwbx_MasterKey_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholder_mk.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Set visibility property of placeholder element to
        /// visible on LostFocus, if the input is not empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pwbx_MasterKey_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pwbx_MasterKey.Password))
                placeholder_mk.Visibility = Visibility.Visible;
        }
    }
}
