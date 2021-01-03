using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Pose.Common;
using Pose.Domain.Nodes;
using Matrix = System.Windows.Media.Matrix;
using Point = System.Windows.Point;
using Vector = System.Windows.Vector;

namespace Pose.SceneEditor.Viewport
{
    /// <summary>
    /// Abstracts internal 3D geometric concepts into an API about 2D <see cref="Node"/>s.
    /// </summary>
    public class SceneViewport : Viewport3D
    {
        internal const float ZoomFactor = 1.15f;

        /// <summary>
        /// Lookup of NodeItem from WPF Visual3D
        /// </summary>
        private readonly Dictionary<Visual3D, NodeItem> _nodeItemFromVisual;

        /// <summary>
        /// Lookup of NodeItem from domain id.
        /// </summary>
        private readonly Dictionary<ulong, NodeItem> _nodeItemFromId;

        /// <summary>
        /// The orthographic camera of the scene.
        /// </summary>
        public OrthographicCamera SceneCamera { get; }

        /// <summary>
        /// // the distance in px from top left to center of viewport
        /// </summary>
        private Vector _screenCenterPx;

        private ModelVisual3D _lightVisual;

        public SceneViewport()
        {
            _nodeItemFromVisual = new Dictionary<Visual3D, NodeItem>();
            _nodeItemFromId = new Dictionary<ulong, NodeItem>();

            SceneCamera = new OrthographicCamera
            {
                Width = ActualWidth,
                Position = new Point3D(0, 0, 5),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0)
            };
            Clear();
        }

        public void SetZoom(float zoom)
        {
            Zoom = zoom;
            ApplyZoomToCamera();
        }

        public void ZoomIn()
        {
            Zoom *= ZoomFactor;
            ApplyZoomToCamera();
        }

        public void ZoomOut()
        {
            Zoom /= ZoomFactor;
            ApplyZoomToCamera();
        }

        public override void EndInit()
        {
            base.EndInit();

            Camera = SceneCamera;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ApplyZoomToCamera(finalSize.Width);
            _screenCenterPx = new Vector(finalSize.Width * 0.5, finalSize.Height * 0.5);
            return base.ArrangeOverride(finalSize);
        }

        internal void ApplyZoomToCamera(double? width = null)
        {
            SceneCamera.Width = (width ?? ActualWidth) / DeviceDependentZoom;
        }

        public Rect GetBoundingBox(ulong nodeId)
        {
            var visual = _nodeItemFromId[nodeId].Visual;
            var bounds3D = VisualTreeHelper.GetDescendantBounds(visual);

            var transform = visual.TransformToAncestor(this);
            return transform?.TransformBounds(bounds3D) ?? Rect.Empty;
        }

        /// <summary>
        /// Returns the center of the node in pixels from the viewport's top-left.
        /// </summary>
        public Vector GetNodeCenterInScreenSpace(ulong nodeId)
        {
            var transform = _nodeItemFromId[nodeId].Transform;
            return WorldToScreenPosition(new Vector2((float)transform.Matrix.OffsetX, (float)transform.Matrix.OffsetY));
        }

        /// <summary>
        /// Returns the spritenode of which the given location (in screenspace pixels) is over its visual. If multple sprites are on top of each other, it returns the first hit sprite behind 'afterSpriteNodeId'.
        /// </summary>
        public ulong? PickSpriteNodeAfter(Point locationPx, ulong? afterSpriteNodeId)
        {
            // note: the WPF HitTest's hit order was originally used, but sometimes the order didn't agree with the visual order. So now we just get all hit sprites,
            // sort them by render order and then pick the next selection. 
            var hitSprites = GetAllHitSprites(locationPx).OrderByDescending(spriteNodeItem => Children.IndexOf(spriteNodeItem.Visual)); // sort by render order, front == first

            if (!hitSprites.Any())
                return null; // no hit

            if (afterSpriteNodeId.HasValue)
            {
                var returnNext = false;
                foreach (var hitSprite in hitSprites)
                {
                    if (returnNext)
                        return hitSprite.NodeId;

                    if (hitSprite.NodeId == afterSpriteNodeId.Value)
                        returnNext = true;
                }
            }

            // just return the most frontal hit if no afterSpriteNodeId given, or it wasn't found.
            return hitSprites.First().NodeId;
        }

        private List<NodeItem> GetAllHitSprites(Point locationPx)
        {
            var list = new List<NodeItem>();
            VisualTreeHelper.HitTest(this,
                target => HitTestFilterBehavior.Continue, r =>
                {
                    if (!(r is RayMeshGeometry3DHitTestResult result))
                        return HitTestResultBehavior.Continue;

                    if (!(result.VisualHit is ModelVisual3D modelVisual))
                        return HitTestResultBehavior.Continue;

                    var nodeItem = _nodeItemFromVisual[modelVisual];
                    if (nodeItem is SpriteNodeItem spriteNodeItem)
                    {
                        var textureHitLocation = GetHitUv(result);
                        var pixel = spriteNodeItem.SpriteBitmap.GetPixelAtUv(textureHitLocation);
                        if (pixel.Alpha != 0)
                            list.Add(nodeItem);
                    }

                    return HitTestResultBehavior.Continue;
                },
                new PointHitTestParameters(locationPx));
            return list;
        }

        private static Vector GetHitUv(RayMeshGeometry3DHitTestResult result)
        {
            var uv1 = result.MeshHit.TextureCoordinates[result.VertexIndex1];
            var uv2 = result.MeshHit.TextureCoordinates[result.VertexIndex2];
            var uv3 = result.MeshHit.TextureCoordinates[result.VertexIndex3];

            // barycentric to Cartesian:
            return new Vector(uv1.X, uv1.Y) * result.VertexWeight1 + new Vector(uv2.X, uv2.Y) * result.VertexWeight2 + new Vector(uv3.X, uv3.Y) * result.VertexWeight3;
        }

        /// <summary>
        /// Creates a new node in the scene with a sprite.
        /// </summary>
        internal void AddSpriteNode(ulong nodeId, SpriteBitmap spriteBitmap)
        {
            var transform = new MatrixTransform3D(Matrix3D.Identity);
            var visual = new ModelVisual3D
            {
                Content = Model3DBuilder.MakeSpriteQuad(spriteBitmap.BitmapImage),
                Transform = transform
            };
            spriteBitmap.ResourcesReloaded += () => visual.Content = Model3DBuilder.MakeSpriteQuad(spriteBitmap.BitmapImage);

            Children.Add(visual);
            var nodeItem = new SpriteNodeItem(visual, nodeId, transform, spriteBitmap);
            _nodeItemFromVisual.Add(visual, nodeItem);
            _nodeItemFromId.Add(nodeId, nodeItem);
        }

        public void RemoveSpriteNode(ulong nodeId)
        {
            var visual = _nodeItemFromId[nodeId].Visual;
            _nodeItemFromId.Remove(nodeId);
            _nodeItemFromVisual.Remove(visual);
            Children.Remove(visual);
        }

        /// <summary>
        /// Changes the transform of the visual representation of the node.
        /// </summary>
        public void SetTransform(ulong nodeId, Matrix transform)
        {
            var matrix3D = Matrix3D.Identity;
            matrix3D.M11 = transform.M11;
            matrix3D.M12 = transform.M12;
            matrix3D.M21 = transform.M21;
            matrix3D.M22 = transform.M22;
            matrix3D.OffsetX = transform.OffsetX;
            matrix3D.OffsetY = transform.OffsetY;
            _nodeItemFromId[nodeId].Transform.Matrix = matrix3D;
        }

        public void Clear()
        {
            Zoom = 1f;
            PanTo(new Vector(0, 0));
            Children.Clear();
            _lightVisual = new ModelVisual3D
            {
                Content = new AmbientLight(Colors.White)
            };
            Children.Add(_lightVisual);
            _nodeItemFromId.Clear();
            _nodeItemFromVisual.Clear();
        }


        /// <summary>
        /// Returns WPF's multiplier from device pixel to device independent pixel. (eg. 1.5 when Windows desktop is set to 150%)
        /// </summary>
        private float GetDevicePixelScale()
        {
            var source = PresentationSource.FromVisual(this);
            return (float) source.CompositionTarget.TransformToDevice.M11;
        }

        public void Hide(in ulong nodeId)
        {
            var nodeItem = _nodeItemFromId[nodeId];
            nodeItem.IsVisible = false;
            if (Children.Contains(nodeItem.Visual)) // this is fast (checks parent)
                Children.Remove(nodeItem.Visual);
        }

        public void Show(in ulong nodeId)
        {
            var nodeItem = _nodeItemFromId[nodeId];
            nodeItem.IsVisible = true;
            if (!Children.Contains(nodeItem.Visual)) // this is fast (checks parent)
                Children.Add(nodeItem.Visual);
        }

        public void SortVisuals(IList<ulong> spriteIdsInDrawOrder)
        {
            // note: rendering order is opposite of domain draw order. (rendering: last in list is in front of others)

            // Rebuilding the list of Children is quite heavy, and very often the order is unchanged. So let's check for differences first.
            if (DrawOrderIsDifferent(spriteIdsInDrawOrder)) return;

            // the render order follows the physical order of the children, so that order must correspond to the DrawOrder of the scene. Using Z coords to force drawing order does not work, because the physical child order would still cause transparency issues,
            // like you would in DirectX when render order does not match depth sorting.
            // (The depth buffer would make the gpu skip pixels of a sprite that's behind a transparent part of another sprite that was rendered earlier => render order for transparent geometry must always be back to front)
            Children.Clear();
            Children.Add(_lightVisual);
            for (var index = spriteIdsInDrawOrder.Count - 1; index >= 0; index--)
            {
                var nodeId = spriteIdsInDrawOrder[index];
                var node = _nodeItemFromId[nodeId];
                if (node.IsVisible)
                    Children.Add(node.Visual);
            }
        }

        private bool DrawOrderIsDifferent(IList<ulong> spriteIdsInDrawOrder)
        {
            var j = 1; // skip the first (camera)
            for (var i = 1; i <= spriteIdsInDrawOrder.Count; i++)
            {
                var nodeId = spriteIdsInDrawOrder[^i]; // index from end, ^1 is the last item
                var node = _nodeItemFromId[nodeId];
                if (!node.IsVisible)
                    continue;

                if (Children[j] != node.Visual)
                {
                    return false;
                }

                j++;
            }

            return true;
        }

        /// <summary>
        /// Converts a screen distance in pixels to the equivalent distance in world space.
        /// </summary>
        public Vector2 ScreenToWorldDistance(Vector distance)
        {
            return distance.ToVector2() / DeviceDependentZoom;
        }

        /// <summary>
        /// Converts a world position to the 2D position in pixels from the top-left of the viewport.
        /// </summary>
        public Vector WorldToScreenPosition(Vector2 position)
        {
            var cameraPosition = GetCameraWorldPosition();
            var temp = (position - cameraPosition) * DeviceDependentZoom;
            return new Vector(_screenCenterPx.X + temp.X, _screenCenterPx.Y - temp.Y);
        }

        public Vector WorldToScreenDistance(Vector2 distance)
        {
            distance = distance * DeviceDependentZoom;
            return new Vector(distance.X, -distance.Y);
        }

        /// <summary>
        /// Converts a world length to screen length.
        /// </summary>
        public float WorldToScreenLength(float length)
        {
            return length * DeviceDependentZoom;
        }

        /// <summary>
        /// Converts a screen position in pixels to world position.
        /// </summary>
        public Vector2 ScreenToWorldPosition(Vector position)
        {
            var cameraPosition = GetCameraWorldPosition();
            var temp = new Vector(position.X - _screenCenterPx.X, _screenCenterPx.Y - position.Y).ToVector2();
            return temp / DeviceDependentZoom + cameraPosition;
        }

        public Vector2 GetCameraWorldPosition()
        {
            return new Vector2((float)SceneCamera.Position.X, (float)SceneCamera.Position.Y);
        }

        /// <summary>
        /// Moves the camera to a certain position of the scene.
        /// </summary>
        public void PanTo(Vector position)
        {
            SceneCamera.Position = new Point3D(position.X, position.Y, SceneCamera.Position.Z);
        }

        /// <summary>
        /// The zoom scale of the camera.
        /// </summary>
        public float Zoom { get; private set; }

        /// <summary>
        /// WPF uses Windows Desktop zoom percentage to convert its device independent units into device specific pixel sized.
        /// To get a crisp 1-to-1 sprite-to-screen pixel ratio, we need to reverse that zoom, because WPF also does this with the viewport content.
        /// </summary>
        private float DeviceDependentZoom => Zoom / GetDevicePixelScale();
    }
}
