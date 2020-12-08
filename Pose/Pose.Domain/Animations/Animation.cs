using System;
using System.Collections.Generic;
using System.Linq;
using Pose.Domain.Animations.Events;
using Pose.Domain.Animations.Messages;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Animations
{
    public partial class Animation
    : Entity, IEditableAnimation
    {
        public Animation(IMessageBus messageBus, ulong id, string name)
        : base(messageBus, id)
        {
            Name = name;
            AnimationsPerNode = new Dictionary<ulong, NodeAnimationCollection>();
            CurrentFrame = 1;
        }

        internal Dictionary<ulong, NodeAnimationCollection> AnimationsPerNode { get; }

        /// <summary>
        /// Stores the current value of the <see cref="PropertyType"/> in the <see cref="Key"/> at the current frame in the animation.
        /// </summary>
        public void KeyCurrentPropertyValue(IUnitOfWork uow, ulong nodeId, PropertyType property)
        {
            if (!AnimationsPerNode.ContainsKey(nodeId))
            {
                uow.Execute(new NodeAnimationCollectionAddedEvent(Id, nodeId));
            }

            var nodeAnimations = AnimationsPerNode[nodeId];
            nodeAnimations.AddOrUpdateKey(uow, CurrentFrame, property);
        }

        public void RemoveKeyAtCurrentFrame(IUnitOfWork uow, ulong nodeId, PropertyType property)
        {
            if (!AnimationsPerNode.TryGetValue(nodeId, out var nodeAnimations))
                throw new Exception($"That Node has no animated properties in this animation.");

            nodeAnimations.RemoveKey(uow, CurrentFrame, property);

            CleanUpIfEmpty(uow, nodeAnimations);
        }

        private void CleanUpIfEmpty(IUnitOfWork uow, NodeAnimationCollection nodeAnimationCollection)
        {
            if (nodeAnimationCollection.IsEmpty())
            {
                uow.Execute(new NodeAnimationCollectionRemovedEvent(Id, nodeAnimationCollection.Node.Id));
            }
        }

        /// <summary>
        /// Returns the animate value of the stored key at the given frame, or null if no key is there.
        /// </summary>
        public float? GetKeyAnimateValueOrNull(in ulong nodeId, PropertyType propertyType, in int frame)
        {
            if (AnimationsPerNode.TryGetValue(nodeId, out var nodeAnimations))
            {
                var key = nodeAnimations.GetKeyOrNull(propertyType, frame);
                if (key == null)
                    return null;
                return nodeAnimations.Node.GetProperty(propertyType).GetAnimateNetValue(key.Value);
            }
            return null;
        }

        internal void RemoveAllAnimationsOfNode(IUnitOfWork uow, ulong nodeId)
        {
            if (!AnimationsPerNode.TryGetValue(nodeId, out var nodeAnimationCollection))
                return;

            RemoveNodeAnimationCollection(uow, nodeAnimationCollection);
        }

        private void RemoveNodeAnimationCollection(IUnitOfWork uow, NodeAnimationCollection nodeAnimationCollection)
        {
            nodeAnimationCollection.RemoveAllAnimations(uow);
            uow.Execute(new NodeAnimationCollectionRemovedEvent(Id, nodeAnimationCollection.Node.Id));
        }

        public void RemoveKeys(IUnitOfWork uow, HashSet<ulong> keyIds)
        {
            foreach (var nodeAnimationCollection in AnimationsPerNode.Values)
            {
                nodeAnimationCollection.RemoveKeys(uow, keyIds);
                CleanUpIfEmpty(uow, nodeAnimationCollection);
            }
        }

        internal void RemoveAll(IUnitOfWork uow)
        {
            while (AnimationsPerNode.Any())
            {
                RemoveNodeAnimationCollection(uow, AnimationsPerNode.Values.First());
            }
        }
        
        public void ChangeCurrentFrame(int currentFrame, bool noSceneUpdate = false)
        {
            if (currentFrame == CurrentFrame)
                return;

            if (currentFrame < BeginFrame)
                currentFrame = BeginFrame;
            if (currentFrame > EndFrame)
                currentFrame = EndFrame;

            CurrentFrame = currentFrame;
            MessageBus.Publish(new AnimationCurrentFrameChanged(Id, currentFrame, noSceneUpdate));
        }

        public void ChangeBeginFrameTransient(in int beginFrame)
        {
            if (beginFrame == BeginFrame)
                return;

            (this as IEditableAnimation).ChangeBeginFrame(beginFrame);
        }

        public void ChangeBeginFrame(IUnitOfWork uow, in int undoValue, in int value)
        {
            uow.Execute(new AnimationBeginFrameChangedEvent(Id, undoValue, value));
        }

        public void ChangeEndFrameTransient(in int endFrame)
        {
            if (endFrame == EndFrame)
                return;

            (this as IEditableAnimation).ChangeEndFrame(endFrame);
        }

        public void ChangeEndFrame(IUnitOfWork uow, in int undoValue, in int value)
        {
            uow.Execute(new AnimationEndFrameChangedEvent(Id, undoValue, value));
        }

        internal void InternalAdd(Node node, PropertyAnimation propertyAnimation)
        {
            if (!AnimationsPerNode.TryGetValue(node.Id, out var nodeAnimationCollection))
            {
                nodeAnimationCollection = new NodeAnimationCollection(MessageBus, this, node);
                AnimationsPerNode.Add(node.Id, nodeAnimationCollection);
            }
            nodeAnimationCollection.PropertyAnimations.Add(propertyAnimation.Property, propertyAnimation);
        }

        /// <summary>
        /// Pushes the net animation values of all animated properties in this <see cref="Animation"/> to the scene's state.
        /// </summary>
        internal void ApplyCurrentFrameToScene()
        {
            foreach (var nodeAnimations in AnimationsPerNode.Values)
            {
                nodeAnimations.ApplyToScene(CurrentFrame);
            }
        }

        internal void ApplyFrameToScene(float frame)
        {
            foreach (var nodeAnimations in AnimationsPerNode.Values)
            {
                nodeAnimations.ApplyToScene(frame);
            }
        }

        public void ToggleIsLoop(IUnitOfWork uow)
        {
            uow.Execute(new AnimationIsLoopChangedEvent(Id, IsLoop, !IsLoop));
        }

        public override string ToString()
        {
            return $"Animation [{Name}]";
        }

        public string Name { get; }

        public int CurrentFrame { get; private set; }

        public int BeginFrame { get; internal set; }

        public int EndFrame { get; internal set; }

        public uint FramesPerSecond { get; internal set; }

        public bool IsLoop { get; internal set; }

        public IEnumerable<PropertyAnimation> PropertyAnimations => AnimationsPerNode.Values.SelectMany(apn => apn.PropertyAnimations.Values);
    }
}
