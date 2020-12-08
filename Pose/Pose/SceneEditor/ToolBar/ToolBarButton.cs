using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pose.SceneEditor.ToolBar
{
    public class ToolBarButton : RadioButton
    {
        static ToolBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarButton), new FrameworkPropertyMetadata(typeof(ToolBarButton)));
        }

        public static readonly DependencyProperty IconGeometryProperty = DependencyProperty.Register(
            "IconGeometry", typeof(Geometry), typeof(ToolBarButton), new PropertyMetadata(default(Geometry)));

        public Geometry IconGeometry
        {
            get => (Geometry) GetValue(IconGeometryProperty);
            set => SetValue(IconGeometryProperty, value);
        }
    }
}
