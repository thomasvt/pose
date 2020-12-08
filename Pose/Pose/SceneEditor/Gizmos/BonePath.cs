using System.Windows.Controls;
using System.Windows.Shapes;

namespace Pose.SceneEditor.Gizmos
{
    internal class BonePath : ContentControl
    {
        public ulong? BoneNodeId { get; }

        public BonePath(ulong? boneNodeId, Path path)
        {
            BoneNodeId = boneNodeId;
            Content = path;
        }
    }
}
