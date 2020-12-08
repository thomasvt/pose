using System.Windows;

namespace Pose.SceneEditor.ToolBar
{
    public class PoseToolBar : System.Windows.Controls.ToolBar
    {
        static PoseToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PoseToolBar), new FrameworkPropertyMetadata(typeof(PoseToolBar)));
        }
    }
}
