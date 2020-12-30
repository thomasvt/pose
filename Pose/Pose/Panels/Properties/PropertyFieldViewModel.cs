using System;
using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Panels.Properties
{
    public class PropertyFieldViewModel
    : ViewModel
    {
        internal readonly PropertyType PropertyType;
        private float _value;
        private float _dragFactor;
        private bool? _isKeyed;
        private bool _isUpdating;
        private bool _isKeyButtonVisible;

        public PropertyFieldViewModel(PropertyType propertyType, string label)
        {
            Label = label;
            PropertyType = propertyType;
            IsKeyButtonVisible = false;
            DragFactor = 1f;
            DisplayValueFactor = 1f;
            IsKeyingEnabled = true;

            MessageBus.Default.Subscribe<EditorModeChanged>(OnEditorModeChanged);
        }

        private void OnEditorModeChanged(EditorModeChanged msg)
        {
            IsKeyButtonVisible = IsKeyingEnabled && msg.Mode == EditorMode.Animate;
        }

        public void BeginUpdate()
        {
            _isUpdating = true;
        }

        public void EndUpdate()
        {
            _isUpdating = false;
        }

        public float Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Multiplies the mousedrag value-changes to this field.
        /// </summary>
        public float DragFactor
        {
            get => _dragFactor;
            set
            {
                if (value == _dragFactor) return;
                _dragFactor = value;
                OnPropertyChanged();
            }
        }

        public bool IsKeyButtonVisible
        {
            get => _isKeyButtonVisible;
            set
            {
                if (value == _isKeyButtonVisible) return;
                _isKeyButtonVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsKeyingEnabled { get; set; }

        public string Label { get; }

        /// <summary>
        /// The internal value is multiplied by this when being displayed in the field.
        /// </summary>
        public float DisplayValueFactor { get; set; }

        public void RefreshPropertyValueAndKeyButton(Editor editor, in ulong nodeId)
        {
            Value = editor.GetNodeProperty(nodeId, PropertyType) * DisplayValueFactor;
            RefreshKeyButtonState(editor, nodeId);
        }

        public void RefreshKeyButtonState(Editor editor, ulong nodeId)
        {
            IsKeyed = GetKeyState(editor, nodeId, Value / DisplayValueFactor);
        }

        private bool? GetKeyState(Editor editor, ulong nodeId, in float currentPropertyValue)
        {
            var animation = editor.GetCurrentAnimation();
            var key = animation.GetKeyAnimateValueOrNull(nodeId, PropertyType, animation.CurrentFrame);
            if (key == null)
                return false;
            return Math.Abs(key.Value - currentPropertyValue) < 0.001 ? (bool?)true : null; // compensate for rounding errors while roundtripping, visual values can be different from internally stored values.
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
                    PropertyKeyed?.Invoke(PropertyType);
                }
                else
                {
                    PropertyUnkeyed?.Invoke(PropertyType);
                }
            }
        }

        public void OnChanged(in float newValue, in bool isTransient)
        {
            PropertyValueChanged?.Invoke(this, new PropertyValueChanged(PropertyType, newValue / DisplayValueFactor, isTransient));
        }

        public event Action<PropertyType> PropertyKeyed;

        public event Action<PropertyType> PropertyUnkeyed;

        public event EventHandler<PropertyValueChanged> PropertyValueChanged;
    }
}
