using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Pose.Domain;
using Pose.SceneEditor.Viewport;

namespace Pose.SceneEditor.Gizmos
{
    internal class AxesGizmo : IGizmo
    {
        private readonly GizmoCanvas _gizmoCanvas;
        private Line _lineY;
        private Line _lineX;

        public AxesGizmo(GizmoCanvas gizmoCanvas)
        {
            _gizmoCanvas = gizmoCanvas;
            CreateGizmo(gizmoCanvas);
        }

        private void CreateGizmo(GizmoCanvas gizmoCanvas)
        {
            _lineY = new Line
            {
                Stroke = new SolidColorBrush(Palette.YAxis.WithAlpha(70)),
                StrokeThickness = 1d
            };
            gizmoCanvas.Children.Add(_lineY);

            _lineX = new Line
            {
                Stroke = new SolidColorBrush(Palette.XAxis.WithAlpha(70)),
                StrokeThickness = 1d
            };
            gizmoCanvas.Children.Add(_lineX);
        }

        public void UpdateTransform(SceneViewport sceneViewport)
        {
            var originScreen = sceneViewport.WorldToScreenPosition(Vector2.Zero);
            var originCanvas = new Vector(originScreen.X, originScreen.Y);

            _lineX.X1 = 0;
            _lineX.X2 = sceneViewport.ActualWidth;
            _lineX.Y1 = _lineX.Y2 = originCanvas.Y;

            _lineY.Y1 = 0;
            _lineY.Y2 = sceneViewport.ActualHeight;
            _lineY.X1 = _lineY.X2 = originCanvas.X;
        }

        public void Dispose()
        {
            _gizmoCanvas.Children.Remove(_lineX);
            _gizmoCanvas.Children.Remove(_lineY);
        }
    }
}
