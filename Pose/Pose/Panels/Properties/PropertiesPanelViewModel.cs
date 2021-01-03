using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        private string _topTitle;
        private SubPanelViewModel _bottomSubPanel;
        private string _bottomTitle;

        public PropertiesPanelViewModel(Editor editor)
        {
            _editor = editor;
            TopSubPanels = new ObservableCollection<SubPanelViewModel>();

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
            RefreshAllTop();
        }

        private void OnNodeTransformChanged(NodeTransformChanged msg)
        {
            if (msg.NodeId == _entityId)
                RefreshAllTop();
        }

        private void OnNodePropertyValueChanged(NodePropertyValueChanged msg)
        {
            if (msg.NodeId == _entityId)
            {
                foreach (var panel in TopSubPanels.OfType<NodeSubPanelViewModel>())
                {
                    panel.RefreshPropertyAndKeyButton(_editor, msg.PropertyType);
                }
            }
        }

        private void OnCurrentAnimationFrameChanged(AnimationCurrentFrameChanged msg)
        {
            if (_editor.IsCurrentAnimation(msg.AnimationId))
            {
                foreach (var panel in TopSubPanels.OfType<NodeSubPanelViewModel>())
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
                ClearTop();
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
                RefreshAllTop();
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

        private void RefreshAllTop()
        {
            UpdateTopTitle();
            foreach (var panel in TopSubPanels)
            {
                panel.Refresh();
            }
        }

        private void UpdateTopTitle()
        {
            TopTitle = _entityType switch
            {
                EntityType.Node => _editor.CurrentDocument.GetNode(_entityId.Value).ToString(),
                _ => string.Empty
            };
        }

        private void RefreshKeyButtonsOnly()
        {
            if (!_entityId.HasValue)
                return;

            foreach (var panel in TopSubPanels.OfType<NodeSubPanelViewModel>())
            {
                panel.RefreshKeyButtons();
            }
        }

        private void OnNodeDeselected(NodeDeselected msg)
        {
            if (msg.NodeId != _entityId)
                return;

            ClearTop();
        }

        private void ClearTop()
        {
            TopTitle = string.Empty;
            _entityType = EntityType.None;
            _entityId = null;
            TopSubPanels.Clear();
        }

        private void OnNodeSelected(NodeSelected msg)
        {
            _entityType = EntityType.Node;
            _entityId = msg.NodeId;
            var node = _editor.CurrentDocument.GetNode(msg.NodeId);
            TopSubPanels.Clear();

            TopSubPanels.Add(_translateSubPanel);
            TopSubPanels.Add(_rotateSubPanel);

            if (node is BoneNode)
            {
                TopSubPanels.Add(_boneSubPanel);
            }

            SetTopPanelsNodeId(msg.NodeId);
            RefreshAllTop();
        }

        private void OnKeySelected(KeySelected msg)
        {
            _keySubPanel.KeyId = msg.KeyId;
            _keySubPanel.Refresh();
            BottomTitle = "Animation Key";
            BottomSubPanel = _keySubPanel;
        }

        private void OnKeyDeselected(KeyDeselected msg)
        {
            if (msg.KeyId != _keySubPanel.KeyId)
                return;

            BottomSubPanel = null;
        }

        private void SetTopPanelsNodeId(ulong nodeId)
        {
            foreach (var panel in TopSubPanels.OfType<NodeSubPanelViewModel>())
            {
                panel.SetNodeId(nodeId);
            }
        }

        public string TopTitle
        {
            get => _topTitle;
            set
            {
                if (_topTitle == value)
                    return;

                _topTitle = value;
                OnPropertyChanged();
            }
        }

        public string BottomTitle
        {
            get => _bottomTitle;
            set
            {
                if (value == _bottomTitle) return;
                _bottomTitle = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SubPanelViewModel> TopSubPanels { get; }

        public SubPanelViewModel BottomSubPanel
        {
            get => _bottomSubPanel;
            set
            {
                if (Equals(value, _bottomSubPanel)) return;
                _bottomSubPanel = value;
                OnPropertyChanged();
            }
        }
    }
}
