using System.Collections.Generic;
using Pose.Domain.Animations;
using Pose.Domain.Animations.Messages;
using Pose.Domain.Documents.Messages;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Messages;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Documents
{
    /// <summary>
    /// The atomic modifications that can be performed. These should only be called by Events from History.
    /// </summary>
    public partial class Document
    {
        void IEditableDocument.MoveNode(ulong nodeId, ulong? parentId, int destinationIndex)
        {
            var node = GetNode(nodeId);
            var sourceCollection = GetOwningCollection(node);
            var sourceIndex = sourceCollection.IndexOf(node);
            var parentNode = parentId.HasValue ? GetNode(parentId.Value) : null;
            var destinationCollection = parentNode?.Nodes ?? Data.RootNodes;

            if (sourceCollection == destinationCollection && sourceIndex < destinationIndex)
            {
                destinationIndex--;
            }

            ((IEditableNodeCollection)sourceCollection).Detach(node);
            ((IEditableNodeCollection)destinationCollection).Attach(destinationIndex, node);
        }

        Animation IEditableDocument.AddAnimation(in ulong animationId, string name, int beginFrame, int endFrame, uint framesPerSecond, bool isLoop)
        {
            var animation = new Animation(_messageBus, animationId, name)
            {
                BeginFrame = beginFrame, 
                EndFrame = endFrame, 
                FramesPerSecond = framesPerSecond, 
                IsLoop = isLoop
            };
            Data.Animations.Add(animation);
            _messageBus.Publish(new AnimationAdded(animationId, name));
            return animation;
        }

        void IEditableDocument.AddNodeAnimationCollection(in ulong animationId, ulong nodeId)
        {
            var animation = GetAnimation(animationId) as IEditableAnimation;
            var node = GetNode(nodeId);
            animation.AddNodeAnimationCollection(node);
        }

        void IEditableDocument.RemoveNodeAnimationCollection(in ulong animationId, in ulong nodeId)
        {
            var animation = GetAnimation(animationId) as IEditableAnimation;
            animation.RemoveNodeAnimationCollection(nodeId);
        }

        void IEditableDocument.MoveSpriteInFrontOf(in ulong nodeId, int index)
        {
            (Data.DrawOrder as IEditableDrawOrder).Move(nodeId, index);
        }

        Key IEditableDocument.AddKey(ulong keyId, ulong propertyAnimationId, int frame, float value, InterpolationData interpolation)
        {
            var key = new Key(_messageBus, keyId, propertyAnimationId, frame, value, interpolation);
            var propertyAnimation = Data.PropertyAnimations[propertyAnimationId] as IEditablePropertyAnimation;
            Data.Keys.Add(key);
            propertyAnimation.AddKey(key);
            return key;
        }

        PropertyAnimation IEditableDocument.AddPropertyAnimation(in ulong propertyAnimationId, ulong animationId, ulong nodeId, PropertyType property)
        {
            var propertyAnimation = new PropertyAnimation(_messageBus, propertyAnimationId, animationId, nodeId, property);
            var animation = GetAnimation(animationId) as IEditableAnimation;
            animation.AddPropertyAnimation(propertyAnimation);
            Data.PropertyAnimations.Add(propertyAnimation);
            return propertyAnimation;
        }

        SpriteNode IEditableDocument.AddSpriteNode(ulong nodeId, string name, SpriteReference spriteRef, ulong? parentNodeId, int index, List<PropertyValueSet> propertyValues)
        {
            var node = new SpriteNode(_messageBus, nodeId, name, spriteRef);
            Data.Nodes.Add(node);
            _messageBus.Publish(new SpriteNodeAdded(nodeId, name, spriteRef)); // before attaching, so UI gets messages in sensible order

            (GetChildNodeCollection(parentNodeId) as IEditableNodeCollection).Attach(index, node);

            if (propertyValues != null)
                ((IEditableNode)node).SetPropertyValues(propertyValues);

            (Data.DrawOrder as IEditableDrawOrder).AddInFront(nodeId);

            return node;
        }

        BoneNode IEditableDocument.AddBoneNode(in ulong nodeId, string name, ulong? parentNodeId, in int index, List<PropertyValueSet> propertyValues)
        {
            var node = new BoneNode(_messageBus, nodeId, name);
            Data.Nodes.Add(node);
            _messageBus.Publish(new BoneNodeAdded(nodeId, name)); // before attaching, so UI gets messages in sensible order

            (GetChildNodeCollection(parentNodeId) as IEditableNodeCollection).Attach(index, node);

            if (propertyValues != null)
                ((IEditableNode)node).SetPropertyValues(propertyValues);

            return node;
        }

        void IEditableDocument.RemoveAnimation(in ulong id)
        {
            _messageBus.Publish(new AnimationKeyRemoving(id));
            Data.Animations.Remove(id);
            _messageBus.Publish(new AnimationRemoved(id));
        }

        void IEditableDocument.RemoveKey(in ulong keyId)
        {
            var key = GetKey(keyId);
            var propertyAnimation = GetPropertyAnimation(key.PropertyAnimationId) as IEditablePropertyAnimation;
            propertyAnimation.RemoveKey(key);
            Data.Keys.Remove(keyId);
        }

        void IEditableDocument.RemoveNode(ulong nodeId)
        {
            _messageBus.Publish(new NodeRemoving(nodeId));

            var node = Data.Nodes[nodeId];
            Data.Nodes.Remove(nodeId);
            (GetOwningCollection(node) as IEditableNodeCollection).Detach(node);
            (Data.DrawOrder as IEditableDrawOrder).Remove(nodeId);
            _messageBus.Publish(new NodeRemoved(nodeId, node.Parent?.Id));
        }

        void IEditableDocument.RemovePropertyAnimation(in ulong propertyAnimationId)
        {
            var propertyAnimation = GetPropertyAnimation(propertyAnimationId);
            var animation = GetAnimation(propertyAnimation.AnimationId) as IEditableAnimation;
            animation.RemovePropertyAnimation(propertyAnimation);
            Data.PropertyAnimations.Remove(propertyAnimationId);
        }

        void IEditableDocument.MarkDirty()
        {
            IsModified = true;
            Modified?.Invoke();
        }

        void IEditableDocument.SetAssetFolder(string path)
        {
            Data.AssetFolder = path;
            _messageBus.Publish(new AssetFolderChanged(path));
        }

        void IEditableDocument.SetMetaDataValue(string key, object value)
        {
            _messageBus.Publish(new MetaDataChanged(key, value));
        }
    }
}
