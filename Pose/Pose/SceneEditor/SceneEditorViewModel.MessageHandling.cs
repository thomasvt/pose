using Pose.Domain.Documents.Messages;
using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Domain.Nodes.Messages;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.SceneEditor
{
    public partial class SceneEditorViewModel
    {
        private void ConfigureMessageHandling()
        {
            MessageBus.Default.Subscribe<DocumentLoaded>(OnDocumentLoaded);
            MessageBus.Default.Subscribe<EditorToolChanged>(OnEditorToolChanged);
            MessageBus.Default.Subscribe<SpriteNodeAdded>(OnSpriteNodeAdded);
            MessageBus.Default.Subscribe<BoneNodeAdded>(OnBoneNodeAdded);
            MessageBus.Default.Subscribe<NodeRemoved>(OnNodeRemoved);
            MessageBus.Default.Subscribe<NodeSelected>(OnNodeSelected);
            MessageBus.Default.Subscribe<NodeDeselected>(OnNodeDeselected);
            MessageBus.Default.Subscribe<DrawOrderChanged>(OnDrawOrderChanged);
            MessageBus.Default.Subscribe<NodeTransformChanged>(OnNodeTransformChanged);
            MessageBus.Default.Subscribe<NodePropertyValueChanged>(OnNodePropertyValueChanged);
            MessageBus.Default.Subscribe<EditorModeChanged>(OnEditorModeChanged);
            MessageBus.Default.Subscribe<AssetFolderChanged>(OnAssetFolderChanged);
            MessageBus.Default.Subscribe<BulkSceneUpdateEnded>(OnBulkSceneUpdateEnded);
        }

        private void OnAssetFolderChanged(AssetFolderChanged msg)
        {
            _spriteBitmapStore.ChangeAssetFolder(msg.Path);
        }

        private void OnEditorModeChanged(EditorModeChanged msg)
        {
            IsToolBarVisible = msg.Mode == EditorMode.Design;
            UpdateTransformations();
        }

        private void OnEditorToolChanged(EditorToolChanged obj)
        {
            StartMouseTool(obj.Tool);
        }

        private void OnDocumentLoaded(DocumentLoaded msg)
        {
            if (Editor == null)
                return;

            LoadCurrentDocument();
        }

        private void OnNodeTransformChanged(NodeTransformChanged msg)
        {
            if (msg.IsBulkUpdate)
                return; // ignore individual changes and wait for BulkSceneUpdateEnded.

            var item = _items[msg.NodeId];
            item.RefreshPropertiesFromNode();
        }

        private void OnBulkSceneUpdateEnded(BulkSceneUpdateEnded msg)
        {
            foreach (var item in _items.Values)
            {
                item.RefreshFromNode();
            }
            ReloadDrawOrder();
        }

        private void OnNodePropertyValueChanged(NodePropertyValueChanged msg)
        {
            if (msg.IsBulkUpdate)
                return;

            if (msg.PropertyType == PropertyType.Visibility)
            {
                var item = _items[msg.NodeId];
                var isVisible = Editor.GetNodePropertyAsBool(msg.NodeId, PropertyType.Visibility);
                item.SetIsVisible(isVisible);
                if (isVisible)
                {
                    ReloadDrawOrder();
                }
                if (isVisible && Editor.NodeSelection.Contains(item.NodeId))
                {
                    item.ShowAsSelected();
                }
            }
            else
            {
                var item = _items[msg.NodeId];
                item.UpdateVisuals();
            }
        }

        private void OnSpriteNodeAdded(SpriteNodeAdded msg)
        {
            AddSpriteNode(msg.NodeId, msg.SpriteRef);
        }

        private void OnBoneNodeAdded(BoneNodeAdded msg)
        {
            AddBoneNode(msg.NodeId, msg.Name);
        }

        private void OnNodeRemoved(NodeRemoved msg)
        {
            _items[msg.NodeId].Dispose();
            _items.Remove(msg.NodeId);
        }

        private void OnDrawOrderChanged(DrawOrderChanged msg)
        {
            ReloadDrawOrder();
        }

        private void OnNodeSelected(NodeSelected msg)
        {
            var item = _items[msg.NodeId];
            if (Editor.GetNodePropertyAsBool(msg.NodeId, PropertyType.Visibility))
            {
                item.ShowAsSelected();
            }
        }

        private void OnNodeDeselected(NodeDeselected msg)
        {
            var item = _items[msg.NodeId];
            item.ShowAsNotSelected();
        }
    }
}
