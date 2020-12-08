using Pose.Domain;
using Pose.Domain.Animations.Messages;
using Pose.Domain.Curves;
using Pose.Domain.Editor;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Panels.Properties.SubPanels
{
    public class KeySubPanelViewModel
    : SubPanelViewModel
    {
        private readonly Editor _editor;
        private double _keyValue;
        private string _nodeName;
        private string _propertyName;
        private BezierCurve _bezierCurve;
        private CurveType _curveType;
        private bool _isLoading;
        private bool _curveIsReadOnly;
        private bool _isValueNumeric;
        private bool _isValueBoolean;
        private bool _keyValueBool;

        public KeySubPanelViewModel(Editor editor)
        {
            _editor = editor;
            BezierCurve = new BezierCurve(Vector2.Zero, Vector2.Zero, Vector2.One, Vector2.One);

            MessageBus.Default.Subscribe<AnimationKeyInterpolationDataChanged>(OnKeyInterpolationDataChanged);
        }

        private void OnKeyInterpolationDataChanged(AnimationKeyInterpolationDataChanged msg)
        {
            if (msg.KeyId == KeyId)
                Refresh();
        }

        public void OnValueChanged(in float value, in bool isTransient)
        {
            if (_isLoading || !IsValueNumeric)
                return;

            if (!isTransient)
            {
                _editor.ChangeKeyValue(KeyId, value);
            }
        }

        public void OnValueChanged(in bool value)
        {
            if (_isLoading || !IsValueBoolean)
                return;

            _editor.ChangeKeyValue(KeyId, value ? Property.TrueValue : Property.FalseValue);
        }

        public override void Refresh()
        {
            _isLoading = true;
            try
            {
                var key = _editor.CurrentDocument.GetKey(KeyId);
                var propertyAnimation = _editor.CurrentDocument.GetPropertyAnimation(key.PropertyAnimationId);
                CurveIsReadOnly = propertyAnimation.Property == PropertyType.Visibility;
                PropertyName = propertyAnimation.Property.ToString();
                NodeName = _editor.CurrentDocument.GetNode(propertyAnimation.NodeId).ToString();
                IsValueBoolean = propertyAnimation.Property == PropertyType.Visibility;
                IsValueNumeric = !IsValueBoolean;
                if (IsValueBoolean)
                {
                    KeyValueBool = Property.ValueToBool(key.Value);
                }
                else
                {
                    KeyValue = key.Value;
                }
                BezierCurve = key.Interpolation.BezierCurve ?? new BezierCurve(Vector2.Zero, new Vector2(0.25f, 0), new Vector2(0.75f, 1), Vector2.One);
                CurveType = key.Interpolation.Type;
            }
            finally
            {
                _isLoading = false;
            }

        }

        public void OnBezierCurveChanged()
        {
            _editor.ChangeKeyInterpolation(KeyId, CurveType, BezierCurve);
        }

        private void OnCurveTypeChanged()
        {
            _editor.ChangeKeyInterpolation(KeyId, CurveType, CurveType == CurveType.Bezier ? (BezierCurve?)BezierCurve : null);
        }

        public void SetCurve(CurveType type, BezierCurve? bezierCurve = null)
        {
            CurveType = type;
            BezierCurve = bezierCurve.GetValueOrDefault();
            OnCurveTypeChanged();
        }

        public string NodeName
        {
            get => _nodeName;
            set
            {
                if (_nodeName == value)
                    return;

                _nodeName = value;
                OnPropertyChanged();
            }
        }

        public string PropertyName
        {
            get => _propertyName;
            set
            {
                if (_propertyName == value)
                    return;

                _propertyName = value;
                OnPropertyChanged();
            }
        }

        public double KeyValue
        {
            get => _keyValue;
            set
            {
                if (_keyValue == value)
                    return;

                _keyValue = value;
                OnPropertyChanged();
            }
        }

        public bool KeyValueBool
        {
            get => _keyValueBool;
            set
            {
                if (value == _keyValueBool) return;
                _keyValueBool = value;
                OnPropertyChanged();
            }
        }

        public BezierCurve BezierCurve
        {
            get => _bezierCurve;
            set
            {
                if (Equals(_bezierCurve, value))
                    return;

                _bezierCurve = value;
                OnPropertyChanged();
            }
        }

        public CurveType CurveType
        {
            get => _curveType;
            set
            {
                if (_curveType == value)
                    return;

                _curveType = value;
                OnPropertyChanged();

                if (_isLoading)
                    return;
                OnCurveTypeChanged();
            }
        }

        public bool CurveIsReadOnly
        {
            get => _curveIsReadOnly;
            set
            {
                if (value == _curveIsReadOnly) return;
                _curveIsReadOnly = value;
                OnPropertyChanged();
            }
        }

        public bool IsValueNumeric
        {
            get => _isValueNumeric;
            set
            {
                if (value == _isValueNumeric) return;
                _isValueNumeric = value;
                OnPropertyChanged();
            }
        }

        public bool IsValueBoolean
        {
            get => _isValueBoolean;
            set
            {
                if (value == _isValueBoolean) return;
                _isValueBoolean = value;
                OnPropertyChanged();
            }
        }

        public ulong KeyId { get; internal set; }

        
    }
}
