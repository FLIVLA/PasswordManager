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
        public Window Parent { get; set; }
        private PasswordItem PasswordItem { get; set; }

        public DeleteScreen(PasswordItem item)
        {
            InitializeComponent();

            PasswordItem = item;
            txt_msg.Text = $"Are you sure you want to delete the password for {item.Platform}? " +
                           $"\nAll data for this record will be permanently deleted.";

        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            App.MainWin.blurGrid.Visibility = Visibility.Collapsed;
            Parent.Close();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            Passwords.Delete(PasswordItem);
            App.MainUI.UpdatePasswordGroup(PasswordItem.Grp_Id);
            App.MainWin.blurGrid.Visibility = Visibility.Collapsed;
            Parent.Close();
        }
    }
}
