using System.Windows;
using System.Windows.Input;
using Pose.SceneEditor.MouseOperations;

namespace Pose.SceneEditor
{
    public partial class SceneEditorViewModel
    {
        internal void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(SceneViewport);

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                {
                    _currentMouseTool.MouseLeftDown(mousePosition);
                    break;
                }
                case MouseButton.Middle:
                    _currentMiddleMouseDragOperation = new PanCameraOperation(this, mousePosition.ToVector());
                    break;
            }
        }

        internal void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(SceneViewport);

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                {
                    _currentMouseTool.MouseDoubleClick(mousePosition);
                    break;
                }
            }
        }

        internal void OnMouseMove(object sender, MouseEventArgs e)
        {
            // update the mousedrag operation.
            var position = e.GetPosition(SceneViewport).ToVector();
            if (_currentMiddleMouseDragOperation != null)
            {
                // middle mouse is always pan camera, give this priority while its active, so user can pan while doing a leftbutton drag.
                _currentMiddleMouseDragOperation?.UpdatePosition(position);
            }
            else
            {
                _currentLeftMouseDragOperation?.UpdatePosition(position);
            }
        }

        internal void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            // end any mousedrag operation
            if (e.ChangedButton == MouseButton.Left)
            {
                _currentLeftMouseDragOperation?.Finish();
                _currentLeftMouseDragOperation = null;
            }

            if (e.ChangedButton == MouseButton.Middle)
            {
                _currentMiddleMouseDragOperation?.Finish();
                _currentMiddleMouseDragOperation = null;
            }

            var mousePosition = e.GetPosition(SceneViewport);
            _currentMouseTool.MouseLeftUp(mousePosition);
        }

        internal void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta == 0)
                return;

            var mousePosition = e.GetPosition(SceneViewport);
            var mouseFromCenter = new Vector(mousePosition.X - SceneViewport.ActualWidth * 0.5f,
                -(mousePosition.Y - SceneViewport.ActualHeight * 0.5f));
            if (e.Delta > 0)
                ZoomIn(mouseFromCenter);
            else
                ZoomOut(mouseFromCenter);

            UpdateAllGizmoTransforms();
        }

        internal void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    if (_currentLeftMouseDragOperation != null || _currentMiddleMouseDragOperation != null)
                        return;
                    Editor.RemoveSelectedNodes();
                    break;
                case Key.Escape:
                    if (_currentLeftMouseDragOperation != null)
                    {
                        _currentLeftMouseDragOperation?.Cancel();
                        _currentLeftMouseDragOperation = null;
                    }
                    else if (!Editor.IsInDefaultEditorTool)
                    {
                        Editor.ChangeToDefaultEditorTool();
                    }
                    else
                    {
                        Editor.NodeSelection.Clear();
                    }

                    break;
            }
        }
    }
}