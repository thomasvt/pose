using System.Windows;
using System.Windows.Controls;

namespace Pose.Controls.CurveEditor
{
    
    public class BezierHandle : Control
    {
        static BezierHandle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BezierHandle), new FrameworkPropertyMetadata(typeof(BezierHandle)));
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(BezierHandle), new PropertyMetadata(default(bool)));

        public bool IsReadOnly
        {
            get => (bool) GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
    }
}
