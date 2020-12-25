using Pose.Domain.Nodes;

namespace Pose.Domain.Animations
{
    internal interface IEditableAnimation
    {
        void ChangeBeginFrame(in int value);
        void ChangeEndFrame(in int value);
        void AddNodeAnimationCollection(in Node node);
        void RemoveNodeAnimationCollection(in ulong nodeId);
        void AddPropertyAnimation(PropertyAnimation propertyAnimation);
        void RemovePropertyAnimation(PropertyAnimation propertyAnimation);
        void SetIsLoop(in bool isLoop);
        void ChangeName(string name);
    }
}