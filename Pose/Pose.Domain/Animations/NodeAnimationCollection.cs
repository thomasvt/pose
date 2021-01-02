using System;
using System.Collections.Generic;
using System.Linq;
using Pose.Domain.Animations.Events;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Animations
{
    public partial class NodeAnimationCollection : IEditableNodeAnimationCollection
    {
        private readonly IMessageBus _messageBus;
        public readonly Animation Animation;
        public readonly Node Node;
        public Dictionary<PropertyType, PropertyAnimation> PropertyAnimations;

        public NodeAnimationCollection(IMessageBus messageBus, Animation animation, Node node)
        {
            _messageBus = messageBus;
            Animation = animation;
            Node = node;
            PropertyAnimations = new Dictionary<PropertyType, PropertyAnimation>();
        }

        internal void AddOrUpdateKey(in IUnitOfWork uow, in int frame, PropertyType propertyType)
        {
            if (!PropertyAnimations.ContainsKey(propertyType))
            {
                uow.Execute(new PropertyAnimationAddedEvent(uow.GetNewEntityId(), Animation.Id, Node.Id, propertyType));
            }

            var propertyAnimation = PropertyAnimations[propertyType];
            var property = Node.GetProperty(propertyType);
            propertyAnimation.AddOrUpdateKey(uow, frame, property.AnimateIncrement);
        }

        internal void RemoveKey(IUnitOfWork uow, in int frame, PropertyType propertyType)
        {
            if (!PropertyAnimations.TryGetValue(propertyType, out var propertyAnimation))
                throw new Exception($"Node \"{Node.Name}\" has no animation for property {propertyType}.");

            propertyAnimation.RemoveKey(uow, frame);
            CleanUpIfEmpty(uow, propertyAnimation);
        }

        private void CleanUpIfEmpty(IUnitOfWork uow, PropertyAnimation propertyAnimation)
        {
            if (!propertyAnimation.IsEmpty()) 
                return;

            uow.Execute(new PropertyAnimationRemovedEvent(Animation.Id, propertyAnimation.Id, propertyAnimation.NodeId, propertyAnimation.Property));
        }

        internal bool IsEmpty()
        {
            return !PropertyAnimations.Any();
        }

        internal Key GetKeyOrNull(PropertyType propertyType, in int frame)
        {
            return PropertyAnimations.TryGetValue(propertyType, out var propertyAnimation) 
                ? propertyAnimation.GetKeyAt(frame) 
                : null;
        }

        internal void RemoveAllAnimations(IUnitOfWork uow)
        {
            while (PropertyAnimations.Any())
            {
                var propertyAnimation = PropertyAnimations.Values.First();
                propertyAnimation.Clear(uow);
                uow.Execute(new PropertyAnimationRemovedEvent(Animation.Id, propertyAnimation.Id, propertyAnimation.NodeId, propertyAnimation.Property));
            }
        }

        public void ApplyToScene(in float frame, int firstFrame, int lastFrame, bool isLoop)
        {
            foreach (var (propertyType, propertyAnimation) in PropertyAnimations)
            {
                var value = propertyAnimation.GetValueAt(frame, firstFrame, lastFrame, isLoop);
                var property = Node.GetProperty(propertyType) as IEditableProperty;
                property.SetAnimateIncrement(value);
            }
        }

        internal void RemoveKeys(IUnitOfWork uow, HashSet<ulong> keyIds)
        {
            foreach (var propertyAnimation in PropertyAnimations.Values)
            {
                propertyAnimation.RemoveKeys(uow, keyIds);
                CleanUpIfEmpty(uow, propertyAnimation);
            }
        }
    }
}
