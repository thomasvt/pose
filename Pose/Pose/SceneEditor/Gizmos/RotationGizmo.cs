using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Pose.SceneEditor.EditorItems;
using Pose.SceneEditor.MouseOperations;
using Pose.SceneEditor.Viewport;

namespace Pose.SceneEditor.Gizmos
{
    internal class RotationGizmo
    : IGizmo
    {
        private readonly SceneEditorViewModel _sceneEditor;
        private readonly EditorItem _item;

        private Ellipse _circle, _sensor;

        private const double Diameter = 140f;
        private const double Radius = Diameter * 0.5f;
        private const double SensorThickness = 20f;

        private static readonly Brush StrokeBrush = new SolidColorBrush(Palette.RotationGizmo.WithAlpha(100));
        private static readonly Brush StrokeBrushHighlight = new SolidColorBrush(Palette.RotationGizmo.WithAlpha(200));
        private Path _pieSlice;
        private Line _startLine;
        private Line _endLine;

        public RotationGizmo(SceneEditorViewModel sceneEditor, EditorItem item)
        {
            _sceneEditor = sceneEditor;
            _item = item;
            CreateGizmo();
        }

        private void CreateGizmo()
        {
            _circle = new Ellipse
            {
                StrokeThickness = 2,
                Stroke = StrokeBrush,
                Width = Diameter,
                Height = Diameter,
                Visibility = Visibility.Hidden
            };

            _sensor = new Ellipse
            {
                StrokeThickness = SensorThickness,
                Stroke = Brushes.Transparent,
                Width = Diameter + SensorThickness * 0.6667,
                Height = Diameter + SensorThickness * 0.6667,
                Visibility = Visibility.Hidden
            };

            _pieSlice = new Path
            {
                Fill = StrokeBrush,
                StrokeThickness = 0,
                Width = Diameter,
                Height = Diameter,
                Visibility = Visibility.Hidden
            };

            _startLine = new Line
            {
                Stroke = StrokeBrushHighlight,
                StrokeThickness = 1,
                Visibility = Visibility.Hidden
            };

            _endLine = new Line
            {
                Stroke = StrokeBrushHighlight,
                StrokeThickness = 1,
                Visibility = Visibility.Hidden
            };

            _sensor.MouseEnter += (sender, args) =>
            {
                if (!IsHighlighting) _circle.Stroke = StrokeBrushHighlight;
            };
            _sensor.MouseLeave += (sender, args) =>
            {
                 if (!IsHighlighting) _circle.Stroke = StrokeBrush;
            };
            _sensor.MouseDown += OnMouseDown;

            _sceneEditor.GizmoCanvasFront.Children.Add(_circle);
            _sceneEditor.GizmoCanvasFront.Children.Add(_sensor);
            _sceneEditor.GizmoCanvasFront.Children.Add(_pieSlice);
            _sceneEditor.GizmoCanvasFront.Children.Add(_startLine);
            _sceneEditor.GizmoCanvasFront.Children.Add(_endLine);
        }

        public void Dispose()
        {
            _sceneEditor.GizmoCanvasFront.Children.Remove(_circle);
            _sceneEditor.GizmoCanvasFront.Children.Remove(_sensor);
            _sceneEditor.GizmoCanvasFront.Children.Remove(_pieSlice);
            _sceneEditor.GizmoCanvasFront.Children.Remove(_startLine);
            _sceneEditor.GizmoCanvasFront.Children.Remove(_endLine);
        }

        /// <summary>
        /// Shows a visible pieslice on top of the gizmo going from one angle to another.
        /// </summary>
        public void ShowAngle(in double fromAngle, in double toAngle)
        {
            _pieSlice.Visibility = Visibility.Visible;
            _pieSlice.Data = CreatePieSlice(fromAngle, toAngle);

            var start = GetPointOnCircle(fromAngle, Radius);
            _startLine.X1 = Radius;
            _startLine.Y1 = Radius;
            _startLine.X2 = start.X;
            _startLine.Y2 = start.Y;
            _startLine.Visibility = Visibility.Visible;

            var end = GetPointOnCircle(toAngle, Radius);
            _endLine.X1 = Radius;
            _endLine.Y1 = Radius;
            _endLine.X2 = end.X;
            _endLine.Y2 = end.Y;
            _endLine.Visibility = Visibility.Visible;
        }

        private static Geometry CreatePieSlice(in double fromAngle, double toAngle)
        {
            //              --b\ - toAngle
            //          ---     \
            //      ---         |
            //    c-------------a  - fromAngle
            //

            var diff = (toAngle > fromAngle ? toAngle - fromAngle : fromAngle - toAngle);
            if (diff > MathF.PI * 2)
            {
                // if more than a full circle, just keep showing a full circle
                diff = MathF.PI * 2;
                toAngle = fromAngle + diff - 0.001f;
            }

            var c = new Point(Radius, Radius);

            var a = GetPointOnCircle(fromAngle, Radius);
            var ca = new LineSegment(a, false);

            var b = GetPointOnCircle(toAngle, Radius);

            var ab = new ArcSegment
            {
                Point = b,
                Size = new Size(Radius, Radius),
                SweepDirection = toAngle > fromAngle ? SweepDirection.Counterclockwise : SweepDirection.Clockwise,
                IsLargeArc = diff > MathF.PI
            };

            var figure = new PathFigure
            {
                StartPoint = c,
                IsClosed = true,
                Segments = { ca, ab }
            };

            return new PathGeometry
            {
                Figures = { figure }
            };
        }

        private static Point GetPointOnCircle(double fromAngle, double r)
        {
            return new Point(r + Math.Cos(fromAngle) * r, r - Math.Sin(fromAngle) * r);
        }

        public void HideAngle()
        {
            _pieSlice.Visibility = Visibility.Hidden;
            _startLine.Visibility = Visibility.Hidden;
            _endLine.Visibility = Visibility.Hidden;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(_sceneEditor.SceneViewport).ToVector();
            _sceneEditor.StartMouseDragOperation(new RotateItemOperation(_sceneEditor, _item, mousePosition));
            e.Handled = true; // prevent other reaction
        }

        public void Show()
        {
            UpdateTransform(_sceneEditor.SceneViewport);

            _circle.Visibility = Visibility.Visible;
            _sensor.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            _circle.Visibility = Visibility.Hidden;
            _sensor.Visibility = Visibility.Hidden;
        }

        public void UpdateTransform(SceneViewport sceneViewport)
        {
            var offset = _item.GetPositionInScreenSpace();

            _circle.SetValue(Canvas.LeftProperty, offset.X - Radius);
            _circle.SetValue(Canvas.TopProperty, offset.Y - Radius);

            _sensor.SetValue(Canvas.LeftProperty, offset.X - Radius - SensorThickness / 3);
            _sensor.SetValue(Canvas.TopProperty, offset.Y - Radius - SensorThickness / 3);

            _pieSlice.SetValue(Canvas.LeftProperty, offset.X - Radius);
            _pieSlice.SetValue(Canvas.TopProperty, offset.Y - Radius);

            _startLine.SetValue(Canvas.LeftProperty, offset.X - Radius);
            _startLine.SetValue(Canvas.TopProperty, offset.Y - Radius);

            _endLine.SetValue(Canvas.LeftProperty, offset.X - Radius);
            _endLine.SetValue(Canvas.TopProperty, offset.Y - Radius);
        }

        public void StopHighlight()
        {
            _circle.Stroke = StrokeBrush;
            IsHighlighting = false;
        }

        public void StartHighlight()
        {
            _circle.Stroke = StrokeBrushHighlight;
            IsHighlighting = true;
        }

        public bool IsHighlighting { get; private set; }

        public bool IsVisible => _circle.Visibility == Visibility.Visible;
    }
}
