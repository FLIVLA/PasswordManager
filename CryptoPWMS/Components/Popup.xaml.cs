using CryptoPWMS.Components.ModalChildControls;
using System.Windows;
using System.Windows.Controls;

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// Base pop-up modal, for hosting different form controls etc.
    /// </summary>
    public partial class Popup : Window
    {
        /// <summary>
        /// Initializes the window with the given child usercontrol.
        /// Based on the type of child control, the width and height
        /// properties of the windows are adjusted accordingly.
        /// </summary>
        /// <param name="child">Child control of any type </param>
        public Popup(UserControl child)
        {
            Owner = App.MainWin;                                // Sets the owner of the modal (to enable centerOwner startup location).
            InitializeComponent();

            HostGrid.Children.Add(child);                       // Add child UserControl to the host grid.

            if (child.GetType() == typeof(NewPasswordForm)) {   // Specific property settings for child control of type NewPasswordForm.
                Width = 600; Height = 495;                      // Set height and width properties based on specific child control type.
                var npw = child as NewPasswordForm;
                npw.Parent = this;                              // Set parent of child to this (to enable ability to close this from child)
                txt_Title.Text = "New Password";
            }

            else if (child.GetType() == typeof(NewGroupForm)) {
                Width = 500; Height = 250;
                var ngf = child as NewGroupForm;
                ngf.Parent = this;
                txt_Title.Text = "New Folder";
            }

            else if (child.GetType() == typeof(EditPasswordForm)) { // Specific property settings for child control of type EditPasswordForm
                Width = 600; Height = 495;                          // Set height and width properties based on specific child control type.
                var npw = child as EditPasswordForm;
                npw.Parent = this;                                  // Set parent of child to this (to enable ability to close this from child)
                txt_Title.Text = "Edit Password";
            }

            else if (child.GetType() == typeof(DeleteScreen)) {     // Specific property settings for child control of type DeleteScreen.
                Width = 500; Height = 250;                          // Set height and width properties based on specific child control type.
                var delScr = child as DeleteScreen;
                delScr.Parent = this;                               // Set parent of child to this (to enable ability to close this from child)
                txt_Title.Visibility = Visibility.Collapsed;
            }

            else if (child.GetType() == typeof(UnlockForm)) {   // Specific property settings for child control of type UnlockForm.
                Width = 500; Height = 260;                      // Set height and width properties based on specific child control type.
                var unlockScr = child as UnlockForm;
                unlockScr.Parent = this;                        // Set parent of child to this (to enable ability to close this from child)
            }
        }

        /// <summary>
        /// Closes the modal, and returns to the main UI of the applicaiton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            App.MainWin.blurGrid.Visibility = Visibility.Collapsed;     // Collapse blurgrid of main UI
            this.Close();                                               // closes the modal.
        }
    }
}
