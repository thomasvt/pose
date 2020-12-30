using System;
using System.Collections.ObjectModel;
using Pose.Domain.Editor;
using Pose.Domain.Nodes.Properties;

namespace Pose.Panels.Hierarchy
{
    public class HierarchyNodeViewModel
    : ViewModel
    {
        private readonly Editor _editor;
        private bool _isSelected;
        private bool _isExpanded;
        private bool _isNodeVisible;
        private string _name;
        private bool _isKeyable;
        private bool? _isKeyed;
        private bool _isUpdating;

        public HierarchyNodeViewModel(Editor editor, ulong nodeId)
        {
            NodeId = nodeId;
            _editor = editor;
            Children = new ObservableCollection<HierarchyNodeViewModel>();

            UpdateNodeVisibilityButton();
        }
        
        public void OnEditorModeChanged(EditorMode mode)
        {
            IsKeyable = mode == EditorMode.Animate;
        }

        public void UpdateNodeVisibilityButton()
        {
            _isUpdating = true;
            IsNodeVisible = _editor.GetNodePropertyAsBool(NodeId, PropertyType.Visibility);
            IsKeyable = _editor.Mode == EditorMode.Animate;
            IsKeyed = GetKeyState();
            _isUpdating = false;
        }

        private bool? GetKeyState()
        {
            var animation = _editor.GetCurrentAnimation();
            var key = animation.GetKeyAnimateValueOrNull(NodeId, PropertyType.Visibility, animation.CurrentFrame);
            if (key == null)
                return false;

            var keyValue = Property.ValueToBool(key.Value);
            return keyValue == IsNodeVisible ? (bool?)true : null;
        }

        public void SetName(string name)
        {
            _isUpdating = true;
            try
            {
                Name = name;
            }
            finally
            {
                _isUpdating = false;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) 
                    return;
                _name = value;
                OnPropertyChanged();

                if (_isUpdating)
                    return;

                NameChanged?.Invoke();
            } 
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;
                if (value)
                {
                    Selected?.Invoke();
                }
                else
                {
                    Deselected?.Invoke();
                }
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value == _isExpanded) return;
                _isExpanded = value;
                OnPropertyChanged();
                IsExpandedChanged?.Invoke();
            }
        }

        public bool IsKeyable
        {
            get => _isKeyable;
            set
            {
                if (value == _isKeyable) return;
                _isKeyable = value;
                OnPropertyChanged();
            }
        }

        public bool? IsKeyed
        {
            get => _isKeyed;
            set
            {
                if (value == _isKeyed) return;
                _isKeyed = value;
                OnPropertyChanged();

                if (_isUpdating)
                    return;

                if (value.HasValue && value.Value) // we don't care about undetermined here
                {
                    Keyed?.Invoke();
                }
                else
                {
                    Unkeyed?.Invoke();
                }
            }
        }

        public bool IsNodeVisible
        {
            get => _isNodeVisible;
            set
            {
                if (value == _isNodeVisible) return;
                _isNodeVisible = value;
                OnPropertyChanged();

                if (_isUpdating)
                    return;

                _editor.SetNodeProperty(NodeId, PropertyType.Visibility, value, true);
                UpdateNodeVisibilityButton();
            }
        }

        public ulong NodeId { get; }
        public bool IsBone { get; set; }
        public bool IsSprite { get; set; }

        public ObservableCollection<HierarchyNodeViewModel> Children { get; }

        public event Action Selected;
        public event Action Deselected;
        public event Action IsExpandedChanged;
        public event Action NameChanged;
        public event Action Keyed;
        public event Action Unkeyed;

    }
}
