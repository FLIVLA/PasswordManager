using CryptoPWMS.IO;
using CryptoPWMS.Models;
using System.Windows;
using System.Windows.Controls;

namespace CryptoPWMS.Components.ModalChildControls
{
    /// <summary>
    /// Interaction logic for DeleteScreen.xaml
    /// </summary>
    public partial class DeleteScreen : UserControl
    {
        public Window Parent { get; set; }                  // To enable the child of closing the parent modal hosting this.
        private PasswordItem PasswordItem { get; set; }     // The password item to be deleted.

        public DeleteScreen(PasswordItem item)
        {
            InitializeComponent();
            PasswordItem = item;                                                                    // store passed item in field variable.
            txt_msg.Text = $"Are you sure you want to delete the password for {item.Platform}? " +  // set text message value.
                           $"\nAll data for this record will be permanently deleted.";
        }

        /// <summary>
        /// Cancels deletion of the password item, and returns to the main UI
        /// of the application. Collapses the main window blurgrid,
        /// and closes the parent modal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            App.MainWin.blurGrid.Visibility = Visibility.Collapsed;     // collapses the blurgrid of the main UI.
            Parent.Close();                                             // closes the parent modal, to return to the main UI.
        }

        /// <summary>
        /// Deletes the password item from the database, updates the list of
        /// password items in the groupcontainer associated with the
        /// Deleted item. Closes parent modal, and returns to main UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            Passwords.Delete(PasswordItem);                             // Deletes the password from the database.
            App.MainUI.UpdatePasswordGroup(PasswordItem.Grp_Id);        // Updates contents of the groupcontainer of the deleted item.
            App.MainWin.blurGrid.Visibility = Visibility.Collapsed;     // collapses the blurgrid of the main UI.
            Parent.Close();                                             // closes the parent modal, to return to the main UI.
        }
    }
}
