using System.Windows.Media.Media3D;

namespace Pose.SceneEditor.Viewport
{
    internal class SpriteNodeItem
    : NodeItem
    {
        /// <summary>
        /// A direct reference to the WPF resource containing the sprite that is shown by this Node.
        /// </summary>
        public readonly SpriteBitmap SpriteBitmap;

        public SpriteNodeItem(Visual3D visual, ulong nodeId, MatrixTransform3D transform, SpriteBitmap spriteBitmap)
        : base(visual, nodeId, transform)
        {
            SpriteBitmap = spriteBitmap;
        }
    }
}
