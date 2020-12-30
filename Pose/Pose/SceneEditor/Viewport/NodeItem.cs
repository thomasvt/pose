using System.Windows.Media.Media3D;

namespace Pose.SceneEditor.Viewport
{
    /// <summary>
    /// Container with info and references of concepts that represent a single Node in the WPF SceneViewport.
    /// </summary>
    internal class NodeItem
    {
        /// <summary>
        /// Domain id of the Node.
        /// </summary>
        public ulong NodeId { get; }

        public bool IsVisible { get; set; }

        /// <summary>
        /// The graphical WPF item inside the viewport representing the Node.
        /// </summary>
        public readonly Visual3D Visual;
        /// <summary>
        /// A direct reference to the transform of the Visual in the viewport, so we can manipulate it easily.
        /// </summary>
        public readonly MatrixTransform3D Transform;

        public NodeItem(Visual3D visual, ulong nodeId, MatrixTransform3D transform)
        {
            NodeId = nodeId;
            Visual = visual;
            Transform = transform;
        }
    }
}
