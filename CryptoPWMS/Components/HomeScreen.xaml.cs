using CryptoPWMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        private FrameworkElement _main;
        public FrameworkElement Main
        {
            get { return _main; }
            set { _main = value; }
        }

        public HomeScreen()
        {
            InitializeComponent();
            title_L.Text = TextTags.AppTitle();
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            UI_Transitions.FadeOut(this, _main);
        }

        private void btn_navToSignup_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
