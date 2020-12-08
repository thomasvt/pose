using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Pose.SceneEditor.Viewport;

namespace Pose.SceneEditor
{
    /// <summary>
    /// The scene editor processing user input, and showing gizmos on top of the <see cref="SceneViewport"/>. 
    /// </summary>
    public partial class SceneEditor : UserControl
    {
        public SceneEditor()
        {
            InitializeComponent();
        }

        private void SceneViewport_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ViewLoaded(Viewport, GizmoCanvasFront, GizmoCanvasBack);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            ViewModel?.OnRenderSizeChanged();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            ViewModel.OnKeyUp(sender, e);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.OnMouseDown(sender, e);
        }

        private void SceneEditor_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewModel.OnMouseDoubleClick(sender, e);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.OnMouseUp(sender, e);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            ViewModel.OnMouseMove(sender, e);
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ViewModel.OnMouseWheel(sender, e);
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // set keyboard focus. Put this in PreviewMouseDown because child controls (Gizmos) may catch and eat normal MouseDown, causing SceneEditor not to focus -> no keyup detection.
            Focus();
        }

        private SceneEditorViewModel ViewModel => DataContext as SceneEditorViewModel;
    }
}
