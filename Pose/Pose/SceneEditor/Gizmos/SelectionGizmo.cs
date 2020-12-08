using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Pose.SceneEditor.Viewport;

namespace Pose.SceneEditor.Gizmos
{
    internal class SelectionGizmo
    : IGizmo
    {
        private Rectangle _rectangle;
        private readonly SceneEditorViewModel _sceneEditor;
        private readonly ulong _nodeId;

        public SelectionGizmo(SceneEditorViewModel sceneEditor, ulong nodeId)
        {
            _sceneEditor = sceneEditor;
            _nodeId = nodeId;
            CreateGizmo();
        }

        private void CreateGizmo()
        {
            _rectangle = new Rectangle
            {
                Stroke = new SolidColorBrush(Palette.Selection),
                StrokeThickness = 1d,
                Visibility = Visibility.Hidden,
            };
            _sceneEditor.GizmoCanvasFront.Children.Add(_rectangle);
        }

        public void UpdateTransform(SceneViewport sceneViewport)
        {
            // todo replace with rectangle following transform

            var bounds = sceneViewport.GetBoundingBox(_nodeId);

            if (bounds.IsEmpty)
            {
                Canvas.SetLeft(_rectangle, -1);
                Canvas.SetTop(_rectangle, -1);

                _rectangle.Width = 0;
                _rectangle.Height = 0;
                return;
            }

            _rectangle.Width = bounds.Width;
            _rectangle.Height = bounds.Height;

            Canvas.SetLeft(_rectangle, bounds.X);
            Canvas.SetTop(_rectangle, bounds.Y);
        }

        public void Show()
        {
            UpdateTransform(_sceneEditor.SceneViewport);
            _rectangle.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            _rectangle.Visibility = Visibility.Hidden;
        }

        public void SetRectangle(Rect rect)
        {
            Canvas.SetLeft(_rectangle, rect.X);
            Canvas.SetTop(_rectangle, rect.Y);

            _rectangle.Width = rect.Width;
            _rectangle.Height = rect.Height;
        }

        public void Dispose()
        {
            if (_rectangle == null)
                return;
            _sceneEditor.GizmoCanvasFront.Children.Remove(_rectangle);
        }

        public bool IsVisible => _rectangle.Visibility == Visibility.Visible;
    }
}
