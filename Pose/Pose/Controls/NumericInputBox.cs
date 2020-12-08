using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pose.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class NumericInputBox : Control
    {
        private TextBox _textBox;
        private Button _decreaseButton;
        private Button _increaseButton;

        private Point _mouseDownPosition;
        private bool _wasMouseDownOnTextBox;
        private bool _isMouseDragging;
        private float _mouseDragInitialValue;

        static NumericInputBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericInputBox), new FrameworkPropertyMetadata(typeof(NumericInputBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GetControlParts();

            ConfigureMouseDragHandling();
            ConfigureTypingModeHandling();
            ConfigureArrowButtons();
        }

        private void GetControlParts()
        {
            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            _decreaseButton = GetTemplateChild("PART_DecreaseButton") as Button;
            _increaseButton = GetTemplateChild("PART_IncreaseButton") as Button;
        }

        private void ConfigureMouseDragHandling()
        {
            _textBox.PreviewMouseDown += (sender, args) => // grab mouse-down on textbox only (don't boycott the arrow buttons)
            {
                if (args.ChangedButton != MouseButton.Left || IsReadOnly)
                    return;

                _wasMouseDownOnTextBox = true;
                _isMouseDragging = false;
                _mouseDownPosition = args.GetPosition(this);
                CaptureMouse();
                args.Handled = true;
            };
            MouseMove += (sender, args) =>
            {
                if (!_wasMouseDownOnTextBox)
                    return;

                var mousePosition = args.GetPosition(this);
                var mouseMoveDistance = (_mouseDownPosition - mousePosition).Length;
                if (mouseMoveDistance > 2)
                {
                    // enter numeric mousedrag mode
                    if (!_isMouseDragging)
                    {
                        _isMouseDragging = true;
                        _mouseDragInitialValue = Value;
                    }

                    var value = _mouseDragInitialValue + (float)(mousePosition.X - _mouseDownPosition.X) * DragIncrementFactor * (Keyboard.IsKeyDown(Key.LeftShift) ? 5 : 1);
                    SetValue(_mouseDragInitialValue, AllowDecimals ? value : (int)value, true);
                }
            };
            MouseUp += (sender, args) =>
            {
                if (args.ChangedButton != MouseButton.Left)
                    return;

                _wasMouseDownOnTextBox = false;
                ReleaseMouseCapture();
                if (!_isMouseDragging) // normal click -> enter typing mode
                {
                    _textBox.Focus();
                }
                else
                {
                    SetValue(_mouseDragInitialValue, Value, false); // ensure trigger of change after drag
                }
            };
        }

        private void ConfigureTypingModeHandling()
        {
            _textBox.LostFocus += (sender, args) =>
            {
                SetTextBoxValue(Value); // in case it was a lostfocus that should cancel and revert the content (eg. mouseclick)
                _textBox.Select(0, 0);
            };
            _textBox.KeyUp += (sender, args) =>
            {
                switch (args.Key)
                {
                    case Key.Enter:
                        ProcessTypedInput();
                        _textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        break;
                    case Key.Tab:
                        ProcessTypedInput();
                        // MoveFocus is done by the system
                        break;
                    case Key.Escape:
                        SetTextBoxValue(Value);
                        Keyboard.ClearFocus();
                        break;
                }
            };
            _textBox.GotFocus += (sender, args) =>
            {                               
                _textBox.SelectAll();
            };

            SetTextBoxValue(Value);
        }

        private void ConfigureArrowButtons()
        {
            _decreaseButton.Click += (sender, args) => SetValue(Value, Value - 10 * DragIncrementFactor * (Keyboard.IsKeyDown(Key.LeftShift) ? 10 : 1), false);
            _increaseButton.Click += (sender, args) => SetValue(Value, Value + 10 * DragIncrementFactor * (Keyboard.IsKeyDown(Key.LeftShift) ? 10 : 1), false);
        }

        private void SetValue(float initialValue, float newValue, bool fromMouseDrag)
        {
            Value = newValue;
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(initialValue, newValue, fromMouseDrag));
        }

        private void ProcessTypedInput()
        {
            if (_textBox.Text == GetDisplayValue(Value))
                return; // user didn't change the text

            if (float.TryParse(_textBox.Text, out var input))
            {
                SetValue(Value, AllowDecimals ? input : (int)input, false);
            }
            else
            {
                // reset to previous value
                SetTextBoxValue(Value);
            }
        }

        private void SetTextBoxValue(float value)
        {
            if (_textBox == null)
                return;

            _textBox.Text = GetDisplayValue(value);
        }

        private string GetDisplayValue(float value)
        {
            return value.ToString(AllowDecimals ? "0.##" : "0");
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(float), typeof(NumericInputBox), new FrameworkPropertyMetadata(0f, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) =>
            {
                var doubleInputBox = o as NumericInputBox;
                var newValue = (float) args.NewValue;
                doubleInputBox.SetTextBoxValue(newValue);
            }));

        
        public float Value
        {
            get => (float) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(NumericInputBox), new PropertyMetadata(default(bool)));

        public bool IsReadOnly
        {
            get => (bool) GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly DependencyProperty DragIncrementFactorProperty = DependencyProperty.Register(
            "DragIncrementFactor", typeof(float), typeof(NumericInputBox), new PropertyMetadata(1f));

        public float DragIncrementFactor
        {
            get => (float) GetValue(DragIncrementFactorProperty);
            set => SetValue(DragIncrementFactorProperty, value);
        }

        public static readonly DependencyProperty AllowDecimalsProperty = DependencyProperty.Register(
            "AllowDecimals", typeof(bool), typeof(NumericInputBox), new PropertyMetadata(default(bool)));

        public bool AllowDecimals
        {
            get => (bool) GetValue(AllowDecimalsProperty);
            set => SetValue(AllowDecimalsProperty, value);
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
    }
}
