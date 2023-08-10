using CryptoPWMS.IO;
using CryptoPWMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CryptoPWMS.Components.ModalChildControls
{
    /// <summary>
    /// Interaction logic for EditPasswordForm.xaml
    /// </summary>
    public partial class EditPasswordForm : UserControl
    {
        public Window Parent { get; set; }                          // To enable the child of closing the parent modal hosting this.
        private Dictionary<string, int> GrpMap { get; set; }        // Stored key-value pairs (name, ID) of Password groups of the database.
        private PasswordItem item { get; set; }
        private int previousGrp;

        public EditPasswordForm(PasswordItem item)
        {
            this.item = item;
            previousGrp = item.Grp_Id;
            InitializeComponent();

            var grps = Passwords.Get_PWGroups();                    // Get password groups from database.
            GrpMap = grps.ToDictionary(x => x.Name, x => x.Id);     // Create dictionary of Groups (name, id).

            for (int i = 0; i < grps.Count; i++)                    // Add combobox-items from group list (group names).
            {
                cbo_grp.Items.Add(grps[i].Name);
            }

            cbo_grp.SelectedIndex = item.Grp_Id - 1;
            txt_platform.Text = item.Platform;
            txt_URL.Text = item.URL;   
        }

        /// <summary>
        /// Updates the password in the database, and updates the associated
        /// passwordgroup(s) in the main UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            string grp = cbo_grp.SelectedItem.ToString();
            int grpId = GrpMap[grp];

            item.Grp_Id = grpId;
            item.Platform = txt_platform.Text;
            item.URL = txt_URL.Text;
            
            if (string.IsNullOrWhiteSpace(txt_URL.Text)) item.URL = "";

            Passwords.Update(item, txt_username.Text, pwbx_password.Password);
            
            App.MainUI.UpdatePasswordGroup(grpId);
            if (grpId != previousGrp) {
                App.MainUI.UpdatePasswordGroup(previousGrp);
            }

            App.MainWin.blurGrid.Visibility = Visibility.Collapsed;
            Parent.Close();
        }

        /// <summary>
        /// Selection changed handler for the form combobox.
        /// Validates the fields and enables the Create Button for the user
        /// to insert the entered data in the database.         
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbo_grp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsValidInput())                     // IF VALID INPUT:
            {
                btn_update.IsEnabled = true;        // Enable update-button.
                btn_update.Opacity = 1;             // set Opacity to indicate button availability.
            }
            else                                    // IF INPUT IS INVALID:
            {
                btn_update.IsEnabled = false;       // Disable update-button.
                btn_update.Opacity = 0.5;           // Set Opacity property of update-button to indicate unavailable state.
            }
        }

        /// <summary>
        /// Content changed handler for text and password fields.
        /// Validates the fields and enables the Create Button for the user
        /// to insert the entered data in the database. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentChanged(object sender, RoutedEventArgs e)
        {
            if (IsValidInput())                     // IF VALID INPUT:
            {
                btn_update.IsEnabled = true;        // Enable update-button.
                btn_update.Opacity = 1;             // set Opacity to indicate button availability.
            }
            else                                    // IF INPUT IS INVALID:
            {
                btn_update.IsEnabled = false;       // Disable Update-button.
                btn_update.Opacity = 0.5;           // Set Opacity property of Update-button to indicate unavailable state.
            }
        }

        /// <summary>
        /// Checks if the required fields have been filled out by the user.
        /// To return true, the user must have entered a username, and mathing
        /// passwords in the two password fields, and have selected a group for
        /// the new entry to be part of.
        /// </summary>
        /// <returns>Boolean value of </returns>
        private bool IsValidInput()
        {
            bool selectedGrp = cbo_grp.SelectedItem != null;                            // User have selected password group.
            bool enteredUn = !string.IsNullOrWhiteSpace(txt_username.Text);             // User have entered a username.
            bool enteredpw = !string.IsNullOrEmpty(pwbx_password.Password);             // User have entered a password.
            bool reEnteredpw = !string.IsNullOrEmpty(pwbx_password_Re_enter.Password);  // User have re-entered the password.
            bool pwsMatch = pwbx_password.Password == pwbx_password_Re_enter.Password;  // User have entered two matching passwords.

            return selectedGrp && enteredUn && enteredpw && reEnteredpw && pwsMatch;    // Return values of all conditions combined.
        }
    }
}
