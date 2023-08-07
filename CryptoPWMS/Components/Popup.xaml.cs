using CryptoPWMS.Components.ModalChildControls;
using System.Windows;
using System.Windows.Controls;

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        public Popup(UserControl child)
        {
            Owner = App.MainWin;
            InitializeComponent();

            HostGrid.Children.Add(child);

            if (child.GetType() == typeof(NewPasswordForm)) {
                Width = 600; Height = 495;
                var npw = child as NewPasswordForm;
                npw.Parent = this;
            }

            else if (child.GetType() == typeof(DeleteScreen)) {
                Width = 500; Height = 250;
                var delScr = child as DeleteScreen;
                delScr.Parent = this;
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            App.MainWin.blurGrid.Visibility = Visibility.Collapsed;
            this.Close();
        }
    }
}
