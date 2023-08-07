using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CryptoPWMS.Components
{
    /// <summary>
    /// Interaction logic for PasswordGroup.xaml
    /// </summary>
    public partial class PasswordGroup : UserControl
    {
        private Models.PasswordGroup _grp;
        public Models.PasswordGroup GRP { get => _grp; set => _grp = value; }

        public PasswordGroup(Models.PasswordGroup grp)
        {
            InitializeComponent();
            _grp = grp;
            txtTitle.Text = _grp.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (SubGrid.Visibility == Visibility.Visible)
                SubGrid.Visibility = Visibility.Collapsed;
            else SubGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Clears the group of all current passwords, then 
        /// fills the group with most recent passwords associated with
        /// the group id.
        /// </summary>
        public void Update()
        {
            pw_stack.Children.Clear();
            var pws = IO.Passwords.GetByGroup(_grp.Id);
            pws.ForEach(x => pw_stack.Children.Add(new PasswordContainer(this, x)));
        }
    }
}
