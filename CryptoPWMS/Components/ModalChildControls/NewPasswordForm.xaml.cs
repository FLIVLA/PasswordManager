using CryptoPWMS.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CryptoPWMS.Components.ModalChildControls
{
    /// <summary>
    /// Interaction logic for NewPasswordForm.xaml
    /// </summary>
    public partial class NewPasswordForm : UserControl
    {
        public Window Parent { get; set; }                          // To enable the child of closing the parent modal hosting this.
        private Dictionary<string, int> GrpMap { get; set; }        // Stored key-value pairs (name, ID) of Password groups of the database.

        public NewPasswordForm()
        {
            InitializeComponent();
            var grps = Passwords.Get_PWGroups();                    // Get password groups from database.
            GrpMap = grps.ToDictionary(x => x.Name, x => x.Id);     // Create dictionary of Groups (name, id).
            
            for (int i = 0; i < grps.Count; i++)                    // Add combobox-items from group list (group names).
            {
                cbo_grp.Items.Add(grps[i].Name);
            }
        }

        /// <summary>
        /// On Click, the eventhandler calls the Insert method for the
        /// Password entity, which inserts new entry in the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            string grp = cbo_grp.SelectedItem.ToString();                   // Selected password group.
            int grpId = GrpMap[grp];                                        // Get the ID of the selected pw-group from map.
            string platform = txt_platform.Text;                            // platform name.
            string url = txt_URL.Text;                                      // platform URL.
            string un = txt_username.Text;                                  // Entered username.
            string pw = pwbx_password.Password;                             // Entered password.

            if (string.IsNullOrWhiteSpace(txt_URL.Text)) url = "";          // Set to empty string in case of whitespace chars in input field.

            Passwords.Insert(grpId, platform, url, un, pw);                 // Insert new password record in the database.
            App.MainUI.UpdatePasswordGroup(grpId);                          // Update the group container in the MainUI.
            App.MainWin.blurGrid.Visibility = Visibility.Collapsed;         
            Parent.Close();                                                 // Close the parent Modal.
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
            if (IsValidEntry())                     // IF VALID INPUT:
            {
                btn_create.IsEnabled = true;        // Enable create-button.
                btn_create.Opacity = 1;             // set Opacity to indicate button availability.
            }
            else                                    // IF INPUT IS INVALID:
            {
                btn_create.IsEnabled = false;       // Disable create-button.
                btn_create.Opacity = 0.5;           // Set Opacity property of create-button to indicate unavailable state.
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
            if (IsValidEntry())                     // IF VALID INPUT:
            {
                btn_create.IsEnabled = true;        // Enable create-button.
                btn_create.Opacity = 1;             // set Opacity to indicate button availability.
            }
            else                                    // IF INPUT IS INVALID:
            {                                           
                btn_create.IsEnabled = false;       // Disable create-button.
                btn_create.Opacity = 0.5;           // Set Opacity property of create-button to indicate unavailable state.
            }
        }

        /// <summary>
        /// Checks if the required fields have been filled out by the user.
        /// To return true, the user must have entered a username, and mathing
        /// passwords in the two password fields, and have selected a group for
        /// the new entry to be part of.
        /// </summary>
        /// <returns>Boolean value of </returns>
        private bool IsValidEntry()
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
