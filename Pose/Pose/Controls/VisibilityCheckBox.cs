using System.Windows;
using System.Windows.Controls;

namespace Pose.Controls
{
    
    public class VisibilityCheckBox : CheckBox
    {
        static VisibilityCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VisibilityCheckBox), new FrameworkPropertyMetadata(typeof(VisibilityCheckBox)));
        }
    }
}
