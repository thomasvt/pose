using System.Windows;
using System.Windows.Controls;
using Pose.Domain.Editor;

namespace Pose.SceneEditor.ToolBar
{
    /// <summary>
    /// Interaction logic for ViewportToolBar.xaml
    /// </summary>
    public partial class ViewportToolBar : UserControl
    {
        public ViewportToolBar()
        {
            InitializeComponent();
        }

        private void ModifyToolButtonClicked(object sender, RoutedEventArgs e)
        {
            Viewport.SetCurrentTool(EditorTool.Modify);
        }

        private void DrawBoneToolButtonClicked(object sender, RoutedEventArgs e)
        {
            Viewport.SetCurrentTool(EditorTool.DrawBone);
        }

        private ViewportToolBarViewModel Viewport => DataContext as ViewportToolBarViewModel;
    }
}
