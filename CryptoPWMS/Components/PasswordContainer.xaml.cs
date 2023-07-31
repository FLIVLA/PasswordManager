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
    /// Interaction logic for PasswordContainer.xaml
    /// </summary>
    public partial class PasswordContainer : UserControl
    {
        private PasswordGroup GRP { get; set; }
        public bool IsExpanded { get; set; }

        public PasswordContainer(PasswordGroup gRP)
        {
            GRP = gRP;
            InitializeComponent();
        }

        public void BtnExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (IsExpanded)
            {
                IsExpanded = false;
                var sb = (Storyboard)this.Resources["sb_collapse"];
                sb.Begin();
                GRP.SetVertConnector_Y2(PasswordGroup.VertY2.collapse);
            }
            else
            {
                IsExpanded = true;
                GRP.SetVertConnector_Y2(PasswordGroup.VertY2.expand);
                var sb = (Storyboard)this.Resources["sb_expand"];
                sb.Begin();
            }
        }

        public PasswordGroup GetGrp()
        {
            return GRP;
        }
    }
}
