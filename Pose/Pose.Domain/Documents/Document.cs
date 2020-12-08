using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pose.Domain.Animations;
using Pose.Domain.Documents.Events;
using Pose.Domain.Documents.Messages;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Documents
{
    /// <summary>
    /// This is the entrypoint into the POCO based domain. It translated Id's into OO instances and allows to manipulate the domain while notifying History so Undo is possible.
    /// </summary>
    public partial class Document : IEditableDocument
    {
        private readonly IMessageBus _messageBus;
        internal readonly DocumentData Data;

        private readonly History.History _history;

        private Document(IMessageBus messageBus, DocumentData data)
        {
            _messageBus = messageBus;
            Data = data;
            _history = new History.History(messageBus, this);
        }

        public static Document CreateNew(IMessageBus messageBus)
        {
            var data = new DocumentData(messageBus, 0);
            var document = new Document(messageBus, data);
            AddDefaultData(messageBus, document);
            document.IsModified = false;
            return document;
        }

        private static void AddDefaultData(IMessageBus messageBus, Document document)
        {
            // kick start the doc with default data: (without adding it to History or sending messages to UI)
            var animation = new Animation(messageBus, document.GetNextEntityId(), "Rest")
            {
                BeginFrame = 1,
                EndFrame = 60,
                FramesPerSecond = 60,
                IsLoop = true
            };
            document.Data.Animations.Add(animation);
        }

        internal static Document CreateFrom(IMessageBus messageBus, DocumentData data)
        {
            var document = new Document(messageBus, data);
            return document;
        }

        public void SetAssetFolder(IUnitOfWork uow, string path)
        {
            uow.Execute(new AssetFolderChangedEvent(Data.AssetFolder, path));
        }

        #region Nodes

        public IEnumerable<Node> GetRootNodes()
        {
            return Data.RootNodes;
        }

        public IEnumerable<Node> GetAllNodes()
        {
            return Data.Nodes;
        }

        public Node GetNode(in ulong nodeId)
        {
            return Data.Nodes[nodeId];
        }

        public void MoveNodeInHierarchy(IUnitOfWork uow, ulong nodeId, ulong? targetNodeId, InsertPosition insertPosition)
        {
            var node = GetNode(nodeId);
            if (insertPosition == InsertPosition.Child && node.Parent?.Id == targetNodeId)
                return; // moved to be a child of its current parent, ignore

            var targetNode = targetNodeId.HasValue ? GetNode(targetNodeId.Value) : null;
            var sourceParentId = node.Parent?.Id;
            var destinationParentId = insertPosition == InsertPosition.Child ? targetNodeId : targetNode.Parent?.Id;
            var sourceCollection = GetOwningCollection(node);
            var sourceIndex = sourceCollection.IndexOf(node);
            var targetCollection = insertPosition == InsertPosition.Child ? targetNode.Nodes : GetOwningCollection(targetNode);
            int destinationIndex;

            if (insertPosition != InsertPosition.Child)
            {
                destinationIndex = targetCollection.IndexOf(targetNode);

                if (insertPosition == InsertPosition.After)
                    destinationIndex++; // convert InsertPosition.After/Before to more to just the exact destination index where to put the item.

                if (sourceCollection == targetCollection && (destinationIndex == sourceIndex || destinationIndex == sourceIndex + 1))
                    return; // node is moved to be a neighbour of itself, don't do anything.
            }
            else
            {
                // insert as first child
                destinationIndex = 0;
            }

            var undoIndex = (sourceCollection == targetCollection && sourceIndex > destinationIndex) ? sourceIndex+1 : sourceIndex;

            var globalTransform = node.GetDesignTransformation().GlobalTransform;
            uow.Execute(new NodeChangedParentEvent(node.Id, sourceParentId, undoIndex, destinationParentId, destinationIndex));
            node.UpdatePropertiesDesignValuesForGlobalTransform(uow, globalTransform);
        }

        internal NodeCollection GetOwningCollection(Node node)
        {
            return node.Parent == null ? Data.RootNodes : node.Parent.Nodes;
        }

        /// <summary>
        /// Adjusts the draw order of a sprite by moving it to another position.
        /// </summary>
        public void MoveSpriteInDrawOrder(IUnitOfWork uow, in ulong nodeId, int destinationIndex)
        {
            var node = GetNode(nodeId);
            if (!(node is SpriteNode))
                throw new Exception($"Node {nodeId} is not a SpriteNode.");

            var undoIndex = Data.DrawOrder.IndexOf(nodeId);
            if (destinationIndex < undoIndex)
                undoIndex++;

            uow.Execute(new DrawOrderChangedEvent(nodeId, undoIndex, destinationIndex));
        }

        /// <summary>
        /// Adds a new SpriteNode in the root-level, at the end.
        /// </summary>
        public ulong AddSpriteNode(IUnitOfWork uow, string name, SpriteReference spriteRef)
        {
            var id = uow.GetNewEntityId();
            uow.Execute(new SpriteNodeAddedEvent(id, name, spriteRef, null, Data.RootNodes.Count));
            return id;
        }

        /// <summary>
        /// Adds a new BoneNode.
        /// </summary>
        public ulong AddBoneNode(IUnitOfWork uow, string name, Vector2 position, float angle, float boneLength)
        {
            var nodeId = uow.GetNewEntityId();
            uow.Execute(new BoneNodeAddedEvent(nodeId, name, null, Data.RootNodes.Count));
            var node = GetNode(nodeId);
            node.GetProperty(PropertyType.TranslationX).SetDesignValue(uow, position.X);
            node.GetProperty(PropertyType.TranslationY).SetDesignValue(uow, position.Y);
            node.GetProperty(PropertyType.RotationAngle).SetDesignValue(uow, angle);
            node.GetProperty(PropertyType.BoneLength).SetDesignValue(uow, boneLength);
            return nodeId;
        }

        public void RemoveNode(IUnitOfWork uow, ulong nodeId)
        {
            var node = GetNode(nodeId);
            RemoveNodeAndChildren(uow, node);
        }

        private void RemoveNodeAndChildren(IUnitOfWork uow, Node node)
        {
            while (node.Nodes.Any())
            {
                RemoveNodeAndChildren(uow, node.Nodes.First());
            }

            // remove all animation keys linked to node.
            foreach (var animation in Data.Animations)
            {
                animation.RemoveAllAnimationsOfNode(uow, node.Id);
            }

            var index = node.Parent?.Nodes.IndexOf(node) ?? Data.RootNodes.IndexOf(node);
            switch (node)
            {
                case SpriteNode spriteNode:
                {
                    uow.Execute(new SpriteNodeRemovedEvent(node.Id, node.Name, spriteNode.SpriteRef, node.Parent?.Id, index, node.GetPropertyValueSets()));
                    break;
                }
                case BoneNode boneNode:
                {
                    uow.Execute(new BoneNodeRemovedEvent(node.Id, node.Name, node.Parent?.Id, index, node.GetPropertyValueSets()));
                    break;
                }
            }
        }

        #endregion

        #region Animation

        public Animation GetAnimation(in ulong animationId)
        {
            return Data.Animations[animationId];
        }

        public ulong AddAnimation(IUnitOfWork uow, string name, int beginFrame, int endFrame, uint framesPerSecond, bool isLoop)
        {
            var animationId = uow.GetNewEntityId();
            uow.Execute(new AnimationAddedEvent(animationId, name, beginFrame, endFrame, framesPerSecond, isLoop));
            return animationId;
        }

        public void RemoveAnimation(IUnitOfWork uow, ulong animationId)
        {
            var animation = Data.Animations[animationId];
            animation.RemoveAll(uow);
            uow.Execute(new AnimationRemovedEvent(animationId, animation.Name, animation.BeginFrame, animation.EndFrame, animation.FramesPerSecond, animation.IsLoop));
        }

        public ulong GetFirstAnimationId()
        {
            return Data.Animations.First().Id;
        }

        public void UpdateSceneForAnimationCurrentFrame(in ulong animationId)
        {
            GetAnimation(animationId).ApplyCurrentFrameToScene();
        }

        public Key GetKey(in ulong keyId)
        {
            return Data.Keys[keyId];
        }

        public PropertyAnimation GetPropertyAnimation(in ulong propertyAnimationId)
        {
            return Data.PropertyAnimations[propertyAnimationId];
        }

        #endregion

        public IUnitOfWork StartUnitOfWork(string operationName)
        {
            return _history.StartUnitOfWork(operationName);
        }

        private NodeCollection GetChildNodeCollection(ulong? parentNodeId)
        {
            if (parentNodeId.HasValue)
            {
                var parentNode = GetNode(parentNodeId.Value);
                return parentNode.Nodes;
            }

            return Data.RootNodes;
        }

        public void SetFilename(string filename)
        {
            Filename = filename;
            (this as IEditableDocument).MarkDirty();
        }

        internal void InternalSetFilename(string filePath)
        {
            Filename = filePath;
        }

        public void MarkSaved()
        {
            IsModified = false;
        }

        #region History

        public bool CanUndo()
        {
            return _history.CanUndo();
        }

        public void Undo()
        {
            _history.Undo();
        }

        public bool CanRedo()
        {
            return _history.CanRedo();
        }

        public void Redo()
        {
            _history.Redo();
        }

        public void NavigateHistoryTo(in ulong version)
        {
            _history.JumpToVersion(version);
        }

        #endregion

        /// <summary>
        /// Returns an id for a new entity that's unique within the document.
        /// </summary>
        public ulong GetNextEntityId()
        {
            return Data.IdSequence.GetNext();
        }

        public void SetMetaData(IUnitOfWork uow, string key, object undoValue, object value)
        {
            uow.Execute(new MetaDataChangedEvent(key, undoValue, value));
        }

        /// <summary>
        /// Node ids in draworder, front to back.
        /// </summary>
        public IList<ulong> GetNodeIdsInDrawOrder() => Data.DrawOrder.GetNodeIdsInOrder();

        public string Filename { get; private set; }
        /// <summary>
        /// This contains the full file path to which Pose last saved this document. It's value is saved inside the file and loaded into this property. It helps finding the original AssetFolder if the pose file was moved.
        /// </summary>
        public string PreviousSaveFilename { get; internal set; }

        public bool IsModified { get; private set; }

        public string AssetFolder => Data.AssetFolder;
        public string RelativeAssetFolder => Data.RelativeAssetFolder;

        public event Action Modified;

        public bool HasFilename => Filename != null;
        public int AnimationCount => Data.Animations.Count;
        

        public string GetUniqueBoneName()
        {
            var i = 0;
            while (Data.Nodes.Any(n => n.Name == "Bone " + ++i))
            {
            }

            return $"Bone " + i;
        }

        public void ApplyCurrentAnimationFrameToScene(in ulong animationId)
        {
            BeginUpdateScene();
            // animation will only update the node properties that have keys in the animation, so reset all animation increments to 0 to have all non animated properties at their Design value.
            foreach (var node in Data.Nodes)
            {
                node.ResetAnimateIncrementValues();
            }
            var animation = GetAnimation(animationId);
            animation.ApplyCurrentFrameToScene();
            EndUpdateScene();
        }

        private void BeginUpdateScene()
        {
            foreach (var rootNode in Data.RootNodes)
            {
                rootNode.BeginUpdate();
            }
        }

        private void EndUpdateScene()
        {
            foreach (var rootNode in Data.RootNodes)
            {
                rootNode.EndUpdate();
            }
            _messageBus.Publish(new BulkSceneUpdateEnded());
        }

        /// <summary>
        /// Applies a non-integer frame of the animation to the scene. This applies the exact property values belonging to that non-integer time of the animation.
        /// </summary>
        /// <param name="isFirstFrame">When you're playing the animation realtime, set this to False on all non-first-of-play-sequence frames.</param>
        public void ApplyAnimationToScene(in ulong animationId, float frame, bool isFirstFrame = true)
        {
            BeginUpdateScene();
            if (isFirstFrame)
            {
                foreach (var node in Data.Nodes)
                {
                    node.ResetAnimateIncrementValues();
                }
            }
            var animation = GetAnimation(animationId);
            animation.ApplyFrameToScene(frame);
            EndUpdateScene();
        }

        public IEnumerable<Animation> GetAnimations()
        {
            return Data.Animations;
        }

        public bool AnimationExists(string name)
        {
            return Data.Animations.Any(a => a.Name == name);
        }

        public string GetAbsoluteAssetPath(string path)
        {
            return Path.Combine(AssetFolder, path);
        }
    }
}
