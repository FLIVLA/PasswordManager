using CryptoPWMS.IO;
using CryptoPWMS.Security;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace CryptoPWMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.MainWin = this;
            App.HomeScreen = Home;
            App.MainUI = Main_ui;
        }

        /// <summary>
        /// Sets the main window state to minimized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Re-encrypts the active user's password vault (if there is an active user ID)
        /// and closes application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            if (App.Cur_User != "" && App.IsAuthenticated)                                     // User ID will be empty string no user is logged in.
            {
                if (!App.IsFileLocked(Vaults.VaultPath(Vaults.VaultState.Decrypted_Temp, App.Cur_User)))
                {
                    Crypto.EncryptVault(App.Cur_User, App.DerivedKey);
                }
                else {
                    MessageBox.Show("Temp file is blocked. wait a few seconds and try again" +
                        "\nThis is due wo sqlite not having released the file yet. " +
                        "Usually it takes max 5 seconds before it is done.");
                    return;
                }
            }
            else {
                var tempFiles = Directory.GetFiles(Vaults.TempDir);
                foreach (var file in tempFiles) {
                    File.Delete(file);
                }
            }

            this.Close();
        }

        /// <summary>
        /// Enables the user to drag move the window on screen. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dragmv_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
