using System.Windows;
using System.Windows.Controls.Primitives;

namespace Pose.Controls
{
    public class KeyButton : ToggleButton
    {
        static KeyButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyButton), new FrameworkPropertyMetadata(typeof(KeyButton)));
        }

        protected override void OnToggle()
        {
            // we need to show three states, but clicking needs to toggle between two states. So, we change that behavior:

            // If IsChecked == true    --->  IsChecked = false
            // If IsChecked == false   --->  IsChecked = true
            // If IsChecked == null    --->  IsChecked = true
            IsChecked = IsChecked != true;
        }
    }
}
