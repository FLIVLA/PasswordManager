using CryptoPWMS.Components;
using CryptoPWMS.IO;
using CryptoPWMS.Utils;
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

        private void btn_minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dragmv_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
