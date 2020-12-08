using Pose.Domain.Animations;
using Pose.Domain.Nodes;

namespace Pose.Domain.Documents
{
    public interface IDocument
    {
        Node GetNode(in ulong nodeId);
        Animation GetAnimation(in ulong animationId);
        Key GetKey(in ulong keyId);
        PropertyAnimation GetPropertyAnimation(in ulong propertyAnimationId);
    }
}