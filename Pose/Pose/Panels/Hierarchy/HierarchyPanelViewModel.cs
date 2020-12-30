using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pose.Domain.Editor;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;
using Pose.Framework;

namespace Pose.Panels.Hierarchy
{
    public partial class HierarchyPanelViewModel
    : ViewModel
    {
        private readonly Editor _editor;
        private readonly TwowayIndex<ulong, HierarchyNodeViewModel> _index;
        private ObservableCollection<HierarchyNodeViewModel> _rootNodes;

        public HierarchyPanelViewModel(Editor editor)
        {
            _editor = editor;
            _index = new TwowayIndex<ulong, HierarchyNodeViewModel>();
            RootNodes = new ObservableCollection<HierarchyNodeViewModel>();

            DropHandler = new HierarchyDropHandler(editor);

            RegisterMessageHandlers();

            Clear();
        }

        private IList<HierarchyNodeViewModel> GetNodeCollectionOf(ulong? parentNodeId)
        {
            if (parentNodeId.HasValue)
            {
                var parentViewModel = _index[parentNodeId.Value];
                return parentViewModel.Children;
            }

            // it's a rootnode
            return RootNodes;
        }

        private void Clear()
        {
            _index.Clear();
            RootNodes.Clear();
        }

        private void BuildNodeViewModels(ICollection<HierarchyNodeViewModel> nodeViewModelCollection, IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var nodeViewModel = CreateNode(node.Id, node.Name, _editor.GetNodePropertyAsBool(node.Id, PropertyType.Visibility));
                nodeViewModelCollection.Add(nodeViewModel);
                BuildNodeViewModels(nodeViewModel.Children, node.Nodes);
            }
        }

        private HierarchyNodeViewModel CreateNode(ulong nodeId, string name, bool isActive)
        {
            var node = _editor.CurrentDocument.GetNode(nodeId);
            var nodeViewModel = new HierarchyNodeViewModel(_editor, nodeId)
            {
                IsNodeVisible = isActive,
                IsExpanded = false,
                IsBone = node is BoneNode,
                IsSprite = node is SpriteNode
            };
            nodeViewModel.SetName(name);
            nodeViewModel.NameChanged += () => _editor.RenameNode(nodeId, nodeViewModel.Name);
            nodeViewModel.Selected += () => _editor.SelectNodeAndChangeToModifyTool(nodeId); ;
            nodeViewModel.Deselected += () => _editor.NodeSelection.Remove(nodeId);
            nodeViewModel.Keyed += () => _editor.AddOrUpdateKeyAtCurrentFrame(nodeId, PropertyType.Visibility);
            nodeViewModel.Unkeyed += () => _editor.RemoveKeyAtCurrentFrame(nodeId, PropertyType.Visibility);
            _index.Add(nodeId, nodeViewModel);
            return nodeViewModel;
        }

        private void AddNode(ulong nodeId)
        {
            var node = _editor.CurrentDocument.GetNode(nodeId);
            CreateNode(nodeId, node.Name, _editor.GetNodePropertyAsBool(nodeId, PropertyType.Visibility));
            // no attaching to parent, that comes in NodeAttachedToParent message.
        }

        public void RemoveSelectedNodes()
        {
            _editor.RemoveSelectedNodes();
        }

        public void CancelSelection()
        {
            _editor.NodeSelection.Clear();
        }

        public ObservableCollection<HierarchyNodeViewModel> RootNodes
        {
            get => _rootNodes;
            private set
            {
                if (_rootNodes == value)
                    return;

                _rootNodes = value;
                OnPropertyChanged();
            }
        }

        public HierarchyDropHandler DropHandler { get; }
    }
}
