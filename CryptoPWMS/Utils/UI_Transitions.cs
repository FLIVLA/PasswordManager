using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace CryptoPWMS.Utils
{
    public static class UI_Transitions
    {
        public static void Fade(FrameworkElement eOut, FrameworkElement eIn)
        {
            if (eOut == null || eIn == null)
                throw new ArgumentNullException("Both fadeOutElement and fadeInElement must not be null.");

            var duration = TimeSpan.FromSeconds(0.5);

            // Set the initial opacity of the fadeIn element to 0
            eIn.Opacity = 0;

            DoubleAnimation fadeInAnimation = new DoubleAnimation(1, duration);
            fadeInAnimation.Completed += (s, e) =>
            {
                eIn.IsEnabled = true;
            };

            // Create a DoubleAnimation to fade out the current element
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, duration);
            fadeOutAnimation.Completed += (s, e) =>
            {
                eOut.Visibility = Visibility.Collapsed;
                eOut.IsEnabled = false;
                eIn.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                eIn.Visibility = Visibility.Visible;
            };

            // Apply the animations to the Opacity property of the elements
            eOut.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }
    }
}
