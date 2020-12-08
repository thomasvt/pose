using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Pose.Domain.Animations.Messages;
using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Messages;
using Pose.Framework.Messaging;
using Pose.Panels.Properties.SubPanels;

namespace Pose.Panels.Properties
{
    public class PropertiesPanelViewModel
    : ViewModel
    {
        
        private readonly Editor _editor;
        private EntityType _entityType;
        private ulong? _entityId;
        private TranslateSubPanelViewModel _translateSubPanel;
        private RotateSubPanelViewModel _rotateSubPanel;
        private BoneSubPanelViewModel _boneSubPanel;
        private KeySubPanelViewModel _keySubPanel;
        private string _title;
        private VerticalAlignment _verticalAlignment;
        
        public PropertiesPanelViewModel(Editor editor)
        {
            _editor = editor;
            SubPanels = new ObservableCollection<SubPanelViewModel>();

            MessageBus.Default.Subscribe<NodeSelected>(OnNodeSelected);
            MessageBus.Default.Subscribe<NodeDeselected>(OnNodeDeselected);
            MessageBus.Default.Subscribe<NodeTransformChanged>(OnNodeTransformChanged);
            MessageBus.Default.Subscribe<NodePropertyValueChanged>(OnNodePropertyValueChanged);
            MessageBus.Default.Subscribe<AnimationCurrentFrameChanged>(OnCurrentAnimationFrameChanged);
            MessageBus.Default.Subscribe<AnimationKeyRemoved>(OnAnimationKeyRemoved);
            MessageBus.Default.Subscribe<AnimationKeyAdded>(OnAnimationKeyAdded);
            MessageBus.Default.Subscribe<AnimationKeyValueChanged>(OnAnimationKeyUpdated);
            MessageBus.Default.Subscribe<EditorModeChanged>(OnEditorModeChanged);
            MessageBus.Default.Subscribe<KeySelected>(OnKeySelected);
            MessageBus.Default.Subscribe<KeyDeselected>(OnKeyDeselected);

            CreateSubPanels();
        }

        private void OnEditorModeChanged(EditorModeChanged obj)
        {
            RefreshAll();
        }

        private void OnNodeTransformChanged(NodeTransformChanged msg)
        {
            if (msg.NodeId == _entityId)
                RefreshAll();
        }

        private void OnNodePropertyValueChanged(NodePropertyValueChanged msg)
        {
            if (msg.NodeId == _entityId)
            {
                foreach (var panel in SubPanels.OfType<NodeSubPanelViewModel>())
                {
                    panel.RefreshPropertyAndKeyButton(_editor, msg.PropertyType);
                }
            }
        }

        private void OnCurrentAnimationFrameChanged(AnimationCurrentFrameChanged msg)
        {
            if (_editor.IsCurrentAnimation(msg.AnimationId))
            {
                foreach (var panel in SubPanels.OfType<NodeSubPanelViewModel>())
                {
                    panel.Refresh();
                }
            }
        }

        private void CreateSubPanels()
        {
            _translateSubPanel = new TranslateSubPanelViewModel(_editor);
            _rotateSubPanel = new RotateSubPanelViewModel(_editor);
            _boneSubPanel = new BoneSubPanelViewModel(_editor);
            _keySubPanel = new KeySubPanelViewModel(_editor);
        }

        private void OnAnimationKeyRemoved(AnimationKeyRemoved msg)
        {
            if (!_entityId.HasValue)
                return;

            if (msg.KeyId == _entityId)
            {
                Clear();
            }
            else if (_editor.GetCurrentAnimation().CurrentFrame == msg.Frame)
            {
                RefreshKeyButtonsOnly();
            }
        }

        private void OnAnimationKeyAdded(AnimationKeyAdded e)
        {
            if (!_entityId.HasValue)
                return;

            if (_editor.GetCurrentAnimation().CurrentFrame == e.Frame)
                RefreshKeyButtonsOnly();
        }

        private void OnAnimationKeyUpdated(AnimationKeyValueChanged e)
        {
            if (!_entityId.HasValue)
                return;

            if (IsKeyOfCurrentFrame(e.KeyId) || _entityId == e.KeyId)
                RefreshAll();
        }

        /// <summary>
        /// Checks if the key is at the current frame of the current animation.
        /// </summary>
        public bool IsKeyOfCurrentFrame(ulong keyId)
        {
            var key = _editor.CurrentDocument.GetKey(keyId);
            var propertyAnimation = _editor.CurrentDocument.GetPropertyAnimation(key.PropertyAnimationId);
            return _editor.GetCurrentAnimation().Id == propertyAnimation.AnimationId && _editor.CurrentDocument.GetAnimation(propertyAnimation.AnimationId).CurrentFrame == key.Frame;
        }

        private void RefreshAll()
        {
            SetEntityTitle();
            foreach (var panel in SubPanels)
            {
                panel.Refresh();
            }
        }

        private void SetEntityTitle()
        {
            Title = _entityType switch
            {
                EntityType.Node => _editor.CurrentDocument.GetNode(_entityId.Value).ToString(),
                EntityType.AnimationKey => "Animation Key",
                _ => string.Empty
            };
        }

        private void RefreshKeyButtonsOnly()
        {
            if (!_entityId.HasValue)
                return;

            foreach (var panel in SubPanels.OfType<NodeSubPanelViewModel>())
            {
                panel.RefreshKeyButtons();
            }
        }

        private void OnNodeDeselected(NodeDeselected msg)
        {
            if (msg.NodeId != _entityId)
                return;

            Clear();
        }

        private void Clear()
        {
            Title = string.Empty;
            _entityType = EntityType.None;
            _entityId = null;
            SubPanels.Clear();
        }

        private void OnNodeSelected(NodeSelected msg)
        {
            _entityType = EntityType.Node;
            _entityId = msg.NodeId;
            VerticalAlignment = VerticalAlignment.Top;
            var node = _editor.CurrentDocument.GetNode(msg.NodeId);
            SubPanels.Clear();

            SubPanels.Add(_translateSubPanel);
            SubPanels.Add(_rotateSubPanel);

            if (node is BoneNode)
            {
                SubPanels.Add(_boneSubPanel);
            }

            SetPanelsNodeId(msg.NodeId);
            RefreshAll();
        }

        private void OnKeySelected(KeySelected msg)
        {
            _entityType = EntityType.AnimationKey;
            _entityId = msg.KeyId;
            VerticalAlignment = VerticalAlignment.Bottom;
            SubPanels.Clear();

            SubPanels.Add(_keySubPanel);
            _keySubPanel.KeyId = msg.KeyId;

            RefreshAll();
        }

        private void OnKeyDeselected(KeyDeselected msg)
        {
            if (msg.KeyId != _entityId)
                return;

            Clear();
        }

        private void SetPanelsNodeId(ulong nodeId)
        {
            foreach (var panel in SubPanels.OfType<NodeSubPanelViewModel>())
            {
                panel.SetNodeId(nodeId);
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                    return;

                _title = value;
                OnPropertyChanged();
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                if (_verticalAlignment == value)
                    return;

                _verticalAlignment = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SubPanelViewModel> SubPanels { get; }
    }
}
