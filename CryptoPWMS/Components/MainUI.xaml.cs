using CryptoPWMS.Models;
using CryptoPWMS.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
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

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for MainUI.xaml
    /// </summary>
    public partial class MainUI : UserControl
    {
        private PasswordGenerator PWG;
        private List<(FrameworkElement, FrameworkElement)> MenuPairs;

        public MainUI()
        {
            PWG = new PasswordGenerator();
            InitializeComponent();
            MenuPairs = new List<(FrameworkElement, FrameworkElement)>()
            {
                (selectionIndicator_PWLIB, PasswordLibrary_grid),
                (selectionIndicator_PWADD, AddPassword_grid),
                (selectionIndicator_PWG, rndpw_grid),
                (selectionIndicator_Settings, Settings_grid)
            };
            SetStack();
            SetPageSelection(0);
        }

        public void SetStack()
        {
            grp_stack.Children.Add(new PasswordGroup("Social Media"));
            grp_stack.Children.Add(new PasswordGroup("Email Accounts"));
            grp_stack.Children.Add(new PasswordGroup("Work & Productivity"));
            grp_stack.Children.Add(new PasswordGroup("Streaming Services"));
            grp_stack.Children.Add(new PasswordGroup("Gaming Accounts"));
            grp_stack.Children.Add(new PasswordGroup("Other"));

            foreach (PasswordGroup grp in grp_stack.Children)
            {
                grp.AddPw("");
                grp.AddPw("");
                grp.AddPw("");
                grp.AddPw("");
                grp.AddPw("");
            }
        }

        #region MENU BTNS

        private void btn_menu_passwordslib_Click(object sender, RoutedEventArgs e)
        {
            SetPageSelection(0);
        }

        private void btn_menu_addpass_Click(object sender, RoutedEventArgs e)
        {
            SetPageSelection(1);
        }

        private void btn_menu_generatepass_Click(object sender, RoutedEventArgs e)
        {
            SetPageSelection(2);
        }

        private void btn_menu_settings_Click(object sender, RoutedEventArgs e)
        {
            SetPageSelection(3);
        }

        private void btn_menu_signout_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region PW GENERATOR

        private void btn_copy_rndpw_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txt_Generatedpw.Text);
        }

        private void btn_Refresh_rndpw_Click(object sender, RoutedEventArgs e)
        {
            txt_Generatedpw.Text = PWG.Generate();
        }

        private void testgenerator()
        {
            const int testDurationInSeconds = 1;

            var uniquePasswords = new HashSet<string>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed.TotalSeconds < testDurationInSeconds)
            {
                string password = PWG.Generate();
                uniquePasswords.Add(password);
            }

            stopwatch.Stop();

            int uniquePasswordCount = uniquePasswords.Count;
            Debug.WriteLine(uniquePasswordCount);
        }

        //==================================================================
        //----------------------- GENERATOR OPTIONS ------------------------
        //==================================================================

        // LENGTH
        private void rndpwLength_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PWG.Length = (int)rndpwLength_slider.Value;
            if (txt_sliderval != null)
            {               
                txt_sliderval.Text = rndpwLength_slider.Value.ToString();
            }
            txt_Generatedpw.Text = PWG.Generate();
        }

        // CAPITAL LETTERS
        private void toggle_capital_Checked(object sender, RoutedEventArgs e)
        {
            PWG.UseCaps = true;
            txt_Generatedpw.Text = PWG.Generate();
        }

        private void toggle_capital_Unchecked(object sender, RoutedEventArgs e)
        {
            PWG.UseCaps = false;
            txt_Generatedpw.Text = PWG.Generate();
        }

        // DIGITS
        private void toggle_usedigits_Checked(object sender, RoutedEventArgs e)
        {
            PWG.UseDigits = true;
            txt_Generatedpw.Text = PWG.Generate();
        }

        private void toggle_usedigits_Unchecked(object sender, RoutedEventArgs e)
        {
            PWG.UseDigits = false;
            txt_Generatedpw.Text = PWG.Generate();
        }

        // SYMBOLS
        private void toggle_usesymbols_Checked(object sender, RoutedEventArgs e)
        {
            PWG.UseSymbols = true;
            txt_Generatedpw.Text = PWG.Generate();
        }

        private void toggle_usesymbols_Unchecked(object sender, RoutedEventArgs e)
        {
            PWG.UseSymbols = false;
            txt_Generatedpw.Text = PWG.Generate();
        }

        // GRP CHARS
        private void toggle_grpchars_Checked(object sender, RoutedEventArgs e)
        {
            PWG.GrpChars = true;
            txt_Generatedpw.Text = PWG.Generate();
        }

        private void toggle_grpchars_Unchecked(object sender, RoutedEventArgs e)
        {
            PWG.GrpChars = false;
            txt_Generatedpw.Text = PWG.Generate();
        }

        

        #endregion

        private void SetPageSelection(int p)
        {
            for (int i = 0; i < MenuPairs.Count; i++)
            {
                if (i == p)
                {
                    MenuPairs[i].Item1.Visibility = Visibility.Visible;
                    MenuPairs[i].Item2.Visibility = Visibility.Visible;
                    MenuPairs[i].Item2.IsEnabled = true;
                }
                else
                {
                    MenuPairs[i].Item1.Visibility = Visibility.Collapsed;
                    MenuPairs[i].Item2.Visibility = Visibility.Collapsed;
                    MenuPairs[i].Item2.IsEnabled = false;
                }
            }
        }
    }
}
