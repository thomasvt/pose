using Pose.Domain.Animations.Messages;
using Pose.Domain.Nodes;
using Pose.Framework.Messaging;

namespace Pose.Domain.Animations
{
    /// <summary>
    /// The atomic modifications that can be performed. These should only be called by Events from History.
    /// </summary>
    public partial class Animation
    {
        void IEditableAnimation.ChangeBeginFrame(in int value)
        {
            BeginFrame = value;
            MessageBus.Publish(new AnimationBeginFrameChanged(Id, value));

            if (CurrentFrame < BeginFrame)
                ChangeCurrentFrame(BeginFrame);
        }

        void IEditableAnimation.ChangeEndFrame(in int value)
        {
            EndFrame = value;
            MessageBus.Publish(new AnimationEndFrameChanged(Id, value));

            if (CurrentFrame > EndFrame)
                ChangeCurrentFrame(EndFrame);
        }

        void IEditableAnimation.AddNodeAnimationCollection(in Node node)
        {
            var nodeAnimations = new NodeAnimationCollection(MessageBus, this, node);
            AnimationsPerNode.Add(node.Id, nodeAnimations);
        }

        void IEditableAnimation.RemoveNodeAnimationCollection(in ulong nodeId)
        {
            AnimationsPerNode.Remove(nodeId);
        }

        void IEditableAnimation.AddPropertyAnimation(PropertyAnimation propertyAnimation)
        {
            var nodeAnimation = AnimationsPerNode[propertyAnimation.NodeId] as IEditableNodeAnimationCollection;
            nodeAnimation.AddPropertyAnimation(propertyAnimation);
        }

        void IEditableAnimation.RemovePropertyAnimation(PropertyAnimation propertyAnimation)
        {
            (AnimationsPerNode[propertyAnimation.NodeId] as IEditableNodeAnimationCollection).RemovePropertyAnimation(propertyAnimation.Id, propertyAnimation.Property);

        }

        void IEditableAnimation.SetIsLoop(in bool isLoop)
        {
            IsLoop = isLoop;
            MessageBus.Publish(new AnimationIsLoopChanged(Id, isLoop));
        }
    }
}
