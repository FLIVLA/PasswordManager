using CryptoPWMS.IO;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CryptoPWMS.Components.ModalChildControls
{
    /// <summary>
    /// Interaction logic for NewGroupForm.xaml
    /// </summary>
    public partial class NewGroupForm : UserControl
    {
        public Window Parent { get; set; }  // To enable the child of closing the parent modal hosting this.

        public NewGroupForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates new group in the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_grpname.Text) && !string.IsNullOrWhiteSpace(txt_grpname.Text))
                {
                    Passwords.InsertGroup(txt_grpname.Text);
                    App.MainUI.FillPasswordData();
                    App.MainWin.blurGrid.Visibility = Visibility.Collapsed;     // Collapse blurgrid of main UI
                    Parent.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Ensures that no create attempt is made if input is empty or whitespace chars.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_grpname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_grpname.Text) && !string.IsNullOrWhiteSpace(txt_grpname.Text))
            {
                btn_create.IsEnabled = true;
                btn_create.Opacity = 1.0;
            }
            else
            {
                btn_create.IsEnabled = false;
                btn_create.Opacity = 0.5;
            }
        }
    }
}
