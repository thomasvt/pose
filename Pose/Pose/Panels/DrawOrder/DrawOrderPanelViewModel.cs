using System.Collections.ObjectModel;
using System.Linq;
using GongSolutions.Wpf.DragDrop;
using Pose.Domain;
using Pose.Domain.Documents.Messages;
using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Messages;
using Pose.Domain.Nodes.Properties;
using Pose.Framework;
using Pose.Framework.Messaging;

namespace Pose.Panels.DrawOrder
{
    public class DrawOrderPanelViewModel
    : ViewModel
    {
        private readonly Editor _editor;
        private readonly TwowayIndex<ulong, DrawOrderItemViewModel> _index;

        public DrawOrderPanelViewModel(Editor editor)
        {
            _editor = editor;
            Items = new ObservableCollection<DrawOrderItemViewModel>();
            DropTarget = new DrawOrderDropHandler(editor);
            _index = new TwowayIndex<ulong, DrawOrderItemViewModel>();

            MessageBus.Default.Subscribe<DocumentLoaded>(OnDocumentLoaded);
            MessageBus.Default.Subscribe<SpriteNodeAdded>(OnSpriteNodeAdded);
            MessageBus.Default.Subscribe<NodeRemoved>(OnNodeRemoved);
            MessageBus.Default.Subscribe<NodeSelected>(msg => OnNodeSelected(msg.NodeId));
            MessageBus.Default.Subscribe<NodeDeselected>(msg => OnNodeDeselected(msg.NodeId));
            MessageBus.Default.Subscribe<DrawOrderChanged>(DrawOrderChanged);
            MessageBus.Default.Subscribe<NodeRenamed>(OnNodeRenamed);
            MessageBus.Default.Subscribe<NodePropertyValueChanged>(OnNodePropertyValueChanged);
        }

        private void OnNodePropertyValueChanged(NodePropertyValueChanged msg)
        {
            if (msg.PropertyType == PropertyType.Visibility && _index.ContainsLeftKey(msg.NodeId))
            {
                _index[msg.NodeId].IsNodeVisible = _editor.GetNodePropertyAsBool(msg.NodeId, PropertyType.Visibility);
            }
        }

        private void OnNodeRenamed(NodeRenamed msg)
        {
            if (_editor.CurrentDocument.GetNode(msg.NodeId) is SpriteNode)
            {
                _index[msg.NodeId].Name = msg.Name;
            }
        }

        private void Clear()
        {
            _index.Clear();
            Items.Clear();
        }

        private void DrawOrderChanged(DrawOrderChanged msg)
        {
            var subject = Items.Single(vm => vm.NodeId == msg.NodeId);

            Items.MoveSafe(subject, msg.Index);
        }

        private void OnNodeSelected(ulong nodeId)
        {
            if (_editor.CurrentDocument.GetNode(nodeId) is SpriteNode)
            {
                _index[nodeId].IsSelected = true;
            }
        }

        private void OnNodeDeselected(ulong nodeId)
        {
            if (_editor.CurrentDocument.GetNode(nodeId) is SpriteNode)
            {
                _index[nodeId].IsSelected = false;
            }
        }

        private void OnSpriteNodeAdded(SpriteNodeAdded msg)
        {
            AddSpriteNode(msg.NodeId, msg.Name);
        }

        private void OnDocumentLoaded(DocumentLoaded msg)
        {
            Clear();
            foreach (var nodeId in _editor.CurrentDocument.GetNodeIdsInDrawOrder().Reverse())
            {
                AddSpriteNode(nodeId, _editor.CurrentDocument.GetNode(nodeId).Name);
            }
        }

        private void OnNodeRemoved(NodeRemoved msg)
        {
            if (!_index.TryGet(msg.NodeId, out var itemViewModel))
                return;

            Items.Remove(itemViewModel);
            _index.Remove(msg.NodeId);
        }

        public void CancelSelection()
        {
            _editor.NodeSelection.Clear();
        }

        private void AddSpriteNode(ulong nodeId, string nodeName)
        {
            var node = _editor.CurrentDocument.GetNode(nodeId);
            var itemViewModel = new DrawOrderItemViewModel
            {
                Name = nodeName,
                NodeId = nodeId,
                IsNodeVisible = _editor.GetNodePropertyAsBool(nodeId, PropertyType.Visibility)
            };
            itemViewModel.Selected += (sender, args) => _editor.SelectNodeAndChangeToModifyTool(nodeId);
            itemViewModel.Deselected += (sender, args) => _editor.NodeSelection.Remove(nodeId);
            _index.Add(nodeId, itemViewModel);
            Items.Insert(0, itemViewModel); // new sprites are always in front of all other sprites
        }

        public ObservableCollection<DrawOrderItemViewModel> Items { get; set; }
        public IDropTarget DropTarget { get; set; }
    }
}
