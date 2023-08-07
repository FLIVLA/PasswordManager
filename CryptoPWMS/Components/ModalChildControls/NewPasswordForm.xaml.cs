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
        public Window Parent { get; set; }
        private Dictionary<string, int> GrpMap { get; set; }

        public NewPasswordForm()
        {
            InitializeComponent();
            var grps = Passwords.Get_PWGroups();                    // Get password groups from database.
            GrpMap = grps.ToDictionary(x => x.Name, x => x.Id);     // Create dictionary of Groups (name, id).
            
            for (int i = 0; i < grps.Count; i++)                    // Add combobox items from group list.
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
            var grp = cbo_grp.SelectedItem.ToString();
            var grpId = GrpMap[grp];
            var platform = txt_platform.Text;
            var url = txt_URL.Text;
            var un = txt_username.Text;
            var pw = pwbx_password.Password;                            

            if (string.IsNullOrWhiteSpace(txt_URL.Text)) url = "";          // Set to empty string in case of whitespace chars in input field.

            Passwords.Insert(App.Cur_Uid, grpId, platform, url, un, pw);    // Insert new password record in the database.
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
            if (IsValidEntry())
            {
                btn_create.IsEnabled = true;
                btn_create.Opacity = 1;
            }
            else
            {
                btn_create.IsEnabled = false;
                btn_create.Opacity = 0.5;
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
            if (IsValidEntry())
            {
                btn_create.IsEnabled = true;
                btn_create.Opacity = 1;
            }
            else
            {
                btn_create.IsEnabled = false;
                btn_create.Opacity = 0.5;
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
            bool selectedGrp = cbo_grp.SelectedItem != null;
            bool enteredUn = !string.IsNullOrWhiteSpace(txt_username.Text);
            bool enteredpw = !string.IsNullOrEmpty(pwbx_password.Password);
            bool reEnteredpw = !string.IsNullOrEmpty(pwbx_password_Re_enter.Password);
            bool pwsMatch = pwbx_password.Password == pwbx_password_Re_enter.Password;

            return selectedGrp && enteredUn && enteredpw && reEnteredpw && pwsMatch;
        }
    }
}
