using Pose.Domain.Animations.Messages;
using Pose.Domain.Documents.Messages;
using Pose.Domain.Editor.Messages;
using Pose.Domain.Nodes.Messages;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Panels.Hierarchy
{
    public partial class HierarchyPanelViewModel
    {
        private void RegisterMessageHandlers()
        {
            MessageBus.Default.Subscribe<DocumentLoaded>(OnDocumentLoaded);
            MessageBus.Default.Subscribe<SpriteNodeAdded>(OnSpriteNodeAdded);
            MessageBus.Default.Subscribe<BoneNodeAdded>(OnBoneNodeAdded);
            MessageBus.Default.Subscribe<NodeRemoved>(OnNodeRemoved);
            MessageBus.Default.Subscribe<NodeSelected>(OnNodeSelected);
            MessageBus.Default.Subscribe<NodeDeselected>(OnNodeDeselected);
            MessageBus.Default.Subscribe<NodeAttachedToParent>(OnNodeMoved);
            MessageBus.Default.Subscribe<NodeRenamed>(OnNodeRenamed);
                       
            MessageBus.Default.Subscribe<EditorModeChanged>(OnEditorModeChanged);
            MessageBus.Default.Subscribe<NodePropertyValueChanged>(OnNodePropertyValueChanged);
            MessageBus.Default.Subscribe<AnimationKeyValueChanged>(OnAnimationKeyValueChanged);
            MessageBus.Default.Subscribe<AnimationKeyAdded>(OnAnimationKeyAdded);
            MessageBus.Default.Subscribe<AnimationKeyRemoved>(OnAnimationKeyRemoved);
        }

        private void OnEditorModeChanged(EditorModeChanged msg)
        {
            foreach (var viewModel in _index.RightKeys)
            {
                viewModel.OnEditorModeChanged(msg.Mode);
            }
        }

        private void OnNodePropertyValueChanged(NodePropertyValueChanged msg)
        {
            if (msg.PropertyType != PropertyType.Visibility)
                return;

            if (_index.TryGet(msg.NodeId, out var viewModel))
                viewModel.UpdateNodeVisibilityButton();
        }

        private void OnAnimationKeyValueChanged(AnimationKeyValueChanged msg)
        {
            OnAnimationKeyChanged(msg.KeyId);
        }

        private void OnAnimationKeyAdded(AnimationKeyAdded msg)
        {
            OnAnimationKeyChanged(msg.KeyId);
        }

        public void OnAnimationKeyChanged(in ulong keyId)
        {
            var key = _editor.CurrentDocument.GetKey(keyId);

            OnAnimationKeyChanged(key.PropertyAnimationId, key.Frame);
        }

        private void OnAnimationKeyRemoved(AnimationKeyRemoved msg)
        {
            OnAnimationKeyChanged(msg.PropertyAnimationId, msg.Frame);
        }

        private void OnAnimationKeyChanged(ulong propertyAnimationId, int frame)
        {
            var propertyAnimation = _editor.CurrentDocument.GetPropertyAnimation(propertyAnimationId);
            if (propertyAnimation.Property != PropertyType.Visibility)
                return;
            // current animation frame?
            var currentAnimation = _editor.GetCurrentAnimation();
            if (currentAnimation.Id == propertyAnimation.AnimationId && currentAnimation.CurrentFrame == frame)
            {
                if (_index.TryGet(propertyAnimation.NodeId, out var viewModel))
                    viewModel.UpdateNodeVisibilityButton();
            }
        }

        private void OnNodeRenamed(NodeRenamed msg)
        {
            _index[msg.NodeId].Name = msg.Name;
        }

        private void OnNodeSelected(NodeSelected msg)
        {
            _index[msg.NodeId].IsSelected = true;
        }

        private void OnNodeDeselected(NodeDeselected msg)
        {
            _index[msg.NodeId].IsSelected = false;
        }

        private void OnNodeMoved(NodeAttachedToParent e)
        {
            var nodeViewModel = _index[e.NodeId];

            var sourceCollection = GetNodeCollectionOf(e.SourceParentNodeId);
            var targetCollection = GetNodeCollectionOf(e.TargetParentNodeId);

            if (sourceCollection.Contains(nodeViewModel))
                sourceCollection.Remove(nodeViewModel);
            targetCollection.Insert(e.Index, nodeViewModel);
        }

        private void OnDocumentLoaded(DocumentLoaded msg)
        {
            Clear();
            BuildNodeViewModels(RootNodes, msg.Document.GetRootNodes());
        }

        private void OnSpriteNodeAdded(SpriteNodeAdded msg)
        {
            AddNode(msg.NodeId);
        }

        private void OnBoneNodeAdded(BoneNodeAdded msg)
        {
            AddNode(msg.NodeId);
        }

        private void OnNodeRemoved(NodeRemoved msg)
        {
            var collection = GetNodeCollectionOf(msg.ParentNodeId);
            var viewModel = _index[msg.NodeId];
            collection.Remove(viewModel);
            _index.Remove(msg.NodeId);
        }
    }
}