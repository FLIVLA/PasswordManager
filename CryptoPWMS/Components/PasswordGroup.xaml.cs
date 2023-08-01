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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for PasswordGroup.xaml
    /// </summary>
    public partial class PasswordGroup : UserControl
    {
        public enum VertY2 { expand, collapse }
        private List<PasswordContainer> Passwords { get; set; }
        public PasswordGroup(string title)
        {
            Passwords = new List<PasswordContainer>();
            InitializeComponent();

            txtTitle.Text = title;
        }

        private void ExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (SubGrid.Visibility == Visibility.Visible)
                SubGrid.Visibility = Visibility.Collapsed;
            else SubGrid.Visibility = Visibility.Visible;
        }

        public void AddPw(string s)
        {
            pw_stack.Children.Add(new PasswordContainer(this));
        }
    }
}
