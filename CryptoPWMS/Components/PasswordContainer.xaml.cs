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
using System.Windows.Threading;

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for PasswordContainer.xaml
    /// </summary>
    public partial class PasswordContainer : UserControl
    {
        private PasswordGroup grp;
        private DispatcherTimer timer;
        private TimeSpan rt;

        public bool IsExpanded { get; set; }


        public PasswordContainer(PasswordGroup grp)
        {
            this.grp = grp;
            InitializeComponent();

            rt = TimeSpan.FromSeconds(10);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10); // Update the timer every 10 milliseconds
            timer.Tick += Timer_Tick;
        }


        public void BtnExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (IsExpanded)
            {
                IsExpanded = false;
                var sb = (Storyboard)this.Resources["sb_collapse"];
                sb.Begin();
            }
            else
            {
                IsExpanded = true;
                var sb = (Storyboard)this.Resources["sb_expand"];
                sb.Begin();
            }
        }

        private void btn_unlockPass_Click(object sender, RoutedEventArgs e)
        {
            rt = TimeSpan.FromSeconds(10);
            timer.Start();
        }



        public PasswordGroup GetGrp()
        {
            return grp;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            rt = rt.Subtract(timer.Interval);

            if (rt <= TimeSpan.Zero)
            {
                timer.Stop();
                rt = TimeSpan.Zero;
            }
            txt_stopwatch.Text = $"{rt:ss\\:ff}";
        }
    }
}
