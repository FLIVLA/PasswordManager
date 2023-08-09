using CryptoPWMS.Components.ModalChildControls;
using CryptoPWMS.IO;
using CryptoPWMS.Security;
using CryptoPWMS.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            SetPageSelection(0);
        }

        #region ====================== EVENT HANDLERS ======================

        /// <summary>
        /// Shows a form on screen for creating new password records
        /// in the database. The shown form will temporarily disable
        /// the main UI of the application, until the new record have
        /// been created, or the users aborts by closing the modal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            var p = new Popup(new NewPasswordForm());               // Construct new modal and child control.
            App.MainWin.blurGrid.Visibility = Visibility.Visible;   // Set visibility of "blur grid".
            p.ShowDialog();                                         // Show modal as dialog.
        }

        #endregion

        #region ========================== METHODS ==========================

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
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

        public void ClearMain()
        {
            grp_stack.Children.Clear();
            txt_Generatedpw.Text = string.Empty;
        }

        /// <summary>
        /// Creates container tree from password records associated with 
        /// the user of the current active user id.
        /// </summary>
        public void FillPasswordData()
        {
            var grps = Passwords.Get_PWGroups();                                // Get groups from database
            var pws = Passwords.GetAll();                                       // Get password records from database (by iser id).

            grps.ForEach(x => grp_stack.Children.Add(new PasswordGroup(x)));    // Add password group containers to vertical stack.            
            foreach (PasswordGroup grp in grp_stack.Children)                   // Add Passwords to groupcontainers
            {
                pws.Where(x => x.Grp_Id == grp.GRP.Id).OrderBy(x => x.Platform).ToList()                                             
                    .ForEach(pw => grp.pw_stack.Children.Add(new PasswordContainer(grp, pw))); 
            }
        }

        #endregion

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
            Crypto.EncryptVault(
                Path.Combine(Vaults.BaseDir, $"{App.Cur_User}.db"),
                App.DerivedKey
            );

            App.Cur_User = "";
            App.DerivedKey = new byte[0];
            App.Salt = new byte[0];

            App.HomeScreen.ResetLoginSection();

            UI_Transitions.Fade(this, App.HomeScreen);
            ClearMain();
        }

        #endregion

        #region PW GENERATOR

        /// <summary>
        /// Copies current generated password to the clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_copy_rndpw_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txt_Generatedpw.Text);
        }

        /// <summary>
        /// Generates new random password with the current settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Refresh_rndpw_Click(object sender, RoutedEventArgs e)
        {
            txt_Generatedpw.Text = PWG.Generate();
        }

        //==================================================================
        //----------------------- GENERATOR OPTIONS ------------------------
        //==================================================================

        private void rndpwLength_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PWG.Length = (int)rndpwLength_slider.Value;
            if (txt_sliderval != null)
            {               
                txt_sliderval.Text = rndpwLength_slider.Value.ToString();
            }
            txt_Generatedpw.Text = PWG.Generate();
        }

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

        public void UpdatePasswordGroup(int grpId)
        {
            foreach (PasswordGroup grp in grp_stack.Children)
            {
                if (grp.GRP.Id == grpId) grp.Update();   
            }
        }
    }
}
