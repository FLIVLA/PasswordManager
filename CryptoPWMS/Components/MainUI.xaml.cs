using CryptoPWMS.Models;
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

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for MainUI.xaml
    /// </summary>
    public partial class MainUI : UserControl
    {
        private PasswordGenerator PWG;

        public MainUI()
        {
            PWG = new PasswordGenerator();
            InitializeComponent();  
            //pwg_tag.Text = TextTags.PasswordGenerator();
        }

        #region MENU BTNS

        private void btn_menu_passwordslib_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_menu_addpass_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_menu_generatepass_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_menu_settings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_menu_signout_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region PW GENERATOR

        private void btn_copy_rndpw_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Refresh_rndpw_Click(object sender, RoutedEventArgs e)
        {
            txt_Generatedpw.Text = PWG.Generate();
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
    }
}
