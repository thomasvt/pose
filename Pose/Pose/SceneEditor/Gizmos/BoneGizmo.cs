using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Pose.Common;
using Pose.Domain;
using Pose.SceneEditor.EditorItems;
using Pose.SceneEditor.Viewport;

namespace Pose.SceneEditor.Gizmos
{
    /// <summary>
    /// The visualization of a bone
    /// </summary>
    internal class BoneGizmo
    : IGizmo
    {
        private readonly SceneEditorViewModel _sceneEditor;
        private readonly BoneNodeEditorItem _boneNodeEditorItem;
        private const double ScreenRadius = 6d;
        private const double ScreenRadiusTip = 1.5d;
        private const double BoneOpacity = 0.5d;
        private const double BoneOpacityHover = 0.9d;
        private static readonly Brush FillBrush = new SolidColorBrush(Palette.Bone);
        private static readonly Brush SelectionBrush = new SolidColorBrush(Palette.Selection);
        private Vector2 _worldPosition;
        private float _tailLengthWorld;
        private Path _path;
        private BonePath _gizmoPath;
        private float _angle;
        private float _lastZoom;
        private bool _isSelected;

        private bool _geometryIsDirty;

        public BoneGizmo(SceneEditorViewModel sceneEditor, BoneNodeEditorItem boneNodeEditorItem)
        {
            _sceneEditor = sceneEditor;
            _boneNodeEditorItem = boneNodeEditorItem;
            _lastZoom = float.MinValue;
            CreateGizmo();
        }

        private void CreateGizmo()
        {
            _path = new Path
            {
                Fill = FillBrush,
                Stroke = SelectionBrush,
                StrokeThickness = 0.5d,
                Opacity = BoneOpacity
            };
            _gizmoPath = new BonePath(_boneNodeEditorItem?.NodeId, _path) // gizmopath is container control around _path, used to detect UI events on Bones, by checking for 'BonePath'
            {
                IsHitTestVisible = true
            };

            _path.MouseEnter += (sender, args) =>
            {
                _path.Opacity = BoneOpacityHover;
            };
            _path.MouseLeave += (sender, args) =>
            {
                if (_isSelected)
                    return;
                _path.Opacity = BoneOpacity;
            };

            _sceneEditor.GizmoCanvasFront.Children.Add(_gizmoPath);
        }

        public void SetWorldPosition(Vector2 worldPosition)
        {
            if (worldPosition == _worldPosition)
                return;

            _worldPosition = worldPosition;
            _geometryIsDirty = true;
        }

        public void SetTailLength(float tailLength)
        {
            if (tailLength == _tailLengthWorld)
                return;

            _tailLengthWorld = tailLength;
            _geometryIsDirty = true;
        }

        public void SetAngle(float angle)
        {
            if (angle == _angle)
                return;

            _angle = angle;
            _geometryIsDirty = true;
        }

        public void UpdateTransform(SceneViewport sceneViewport)
        {
            var screenPosition = sceneViewport.WorldToScreenPosition(_worldPosition);

            _gizmoPath.SetValue(Canvas.LeftProperty, screenPosition.X);
            _gizmoPath.SetValue(Canvas.TopProperty, screenPosition.Y);

            UpdateGeometry();
        }

        private void UpdateGeometry()
        {
            if (!_geometryIsDirty && _sceneEditor.SceneViewport.Zoom == _lastZoom) // rebuild geometry if it has changed, or the editor's zoom had changed. We cannot zoom bones with a scale-transform because their width in screenspace is the same in any zoom)
                return;

            // todo optimization: recreating bone geometry is a top bottleneck when animating. So, we could rewrite this to use a wpf visual transform instead op recreating geometry. But animation performance is good enough for the moment.

            _lastZoom = _sceneEditor.SceneViewport.Zoom;

            if (_tailLengthWorld < 5)
            {
                _path.Data = new EllipseGeometry(new Point(0, 0), ScreenRadius, ScreenRadius);
            }
            else
            {
                var tailVectorWorld = Vector2.FromAngle(_angle, _tailLengthWorld);
                var toTailScreen = _sceneEditor.SceneViewport.WorldToScreenDistance(tailVectorWorld);
                var perpendicular = _sceneEditor.SceneViewport.WorldToScreenDistance(tailVectorWorld.GetPerpendicular());
                perpendicular.Normalize();

                var perpendicularBase = perpendicular * ScreenRadius;
                var perpendicularTip = perpendicular * ScreenRadiusTip;

                var pathFigure = new PathFigure(new Point(perpendicularBase.X, perpendicularBase.Y), new PathSegment[]
                {
                    new ArcSegment(new Point(-perpendicularBase.X, -perpendicularBase.Y), new Size(ScreenRadius, ScreenRadius), Math.PI, false, SweepDirection.Counterclockwise, true),
                    new LineSegment(new Point(toTailScreen.X - perpendicularTip.X, toTailScreen.Y - perpendicularTip.Y), true),
                    new ArcSegment(new Point(toTailScreen.X + perpendicularTip.X, toTailScreen.Y + perpendicularTip.Y), new Size(ScreenRadiusTip, ScreenRadiusTip), Math.PI, false, SweepDirection.Counterclockwise, true),
                }, true);
                _path.Data = new PathGeometry(new[] { pathFigure });
            }

            _geometryIsDirty = false;
        }

        public void ShowAsSelected()
        {
            _isSelected = true;
            _path.StrokeThickness = 2d;
            _path.Opacity = BoneOpacityHover;
        }

        public void ShowAsNotSelected()
        {
            _isSelected = false;
            _path.StrokeThickness = 0.5d;
            _path.Opacity = BoneOpacity;
        }

        public void Dispose()
        {
            _sceneEditor.GizmoCanvasFront.Children.Remove(_gizmoPath);
        }

        public void Show()
        {
            _path.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            _path.Visibility = Visibility.Collapsed;
        }
    }
}
