using CryptoPWMS.Components.ModalChildControls;
using CryptoPWMS.Models;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for PasswordContainer.xaml
    /// </summary>
    public partial class PasswordContainer : UserControl
    {
        private PasswordGroup grp;
        private PasswordItem item;
        private DispatcherTimer timer;
        private TimeSpan rt;

        public bool IsExpanded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grp"></param>
        public PasswordContainer(PasswordGroup grp, PasswordItem item)
        {
            this.grp = grp;
            this.item = item;

            InitializeComponent();

            txt_platform.Text = item.Platform;
            txt_lastupdated.Text = item.LastUpdated;
            
            if (!string.IsNullOrEmpty(item.URL)) {
                btn_url.IsEnabled = true;
                btn_url.Content = item.URL;
            }

            rt = TimeSpan.FromSeconds(10);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Expands/collapses the containers expandable section based
        /// on its current expanded state. On click the expanded state 
        /// is set, and the appropriate storyboard of the controls resources
        /// will be executed (animation of UI elements). 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_unlockPass_Click(object sender, RoutedEventArgs e)
        {
            rt = TimeSpan.FromSeconds(10);
            timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PasswordGroup GetGrp()
        {
            return grp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            //var p = new Popup(new NewPasswordForm());
            //App.MainWin.blurGrid.Visibility = Visibility.Visible;
            //p.ShowDialog();
        }

        /// <summary>
        /// Opens modal containing cancel and final deletion options
        /// to ensure that the user does not accidentally delete records.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var p = new Popup(new DeleteScreen(item));
            App.MainWin.blurGrid.Visibility = Visibility.Visible;
            p.ShowDialog();
        }

        /// <summary>
        /// Opens the URL of the password item in the default browser
        /// of the current device used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_url_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(item.URL) && !string.IsNullOrWhiteSpace(item.URL))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = item.URL,
                        UseShellExecute = true
                    };

                    Process.Start(psi);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
    }
}
