using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pose.Controls
{
    /// <summary>
    /// This is designed to be used inside an itemtemplate of treeviews, or listviews etc. It provides the common UI pattern of list items going into text-editable mode when a second click is done on an already selected item.
    /// Note: Using EditableLabel outside an ItemsControl is not tested, and may not work as expected because we use the encapsulating itemcontainer's IsFocused property to see if a mouse click is a second or a first click.
    /// </summary>
    public class EditableLabel : Control
    {
        private TextBox _textBox;
        private string _undoText;

        static EditableLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableLabel), new FrameworkPropertyMetadata(typeof(EditableLabel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = GetTemplateChild("TextBox") as TextBox;
            _textBox.LostFocus += (sender, args) =>
            {
                if (IsEditing) 
                    FinishEditing();
            };
            _textBox.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    FinishEditing();
                else if (args.Key == Key.Escape)
                    CancelEditing();
            };
        }

        private void StartEditing()
        {
            _undoText = Text;
            IsEditing = true;
            _textBox.Focus();
            _textBox.SelectAll();
        }

        private void FinishEditing()
        {
            Text = _textBox.Text;
            IsEditing = false;
        }

        private void CancelEditing()
        {
            IsEditing = false;
            _textBox.Text = _undoText;
            Text = _undoText;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // we grab the itemcontainer around us and only go into IsEditing if that item already has focus (due to a previous click or focusing action).
            var parent = GetFocusableParent();
            if (parent.IsFocused)
            {
                StartEditing();
                e.Handled = true; // ensure the ItemContainer around doesnt receive the click and selects+focuses itself, stealing focus from our just activated textbox.
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.F2)
                StartEditing();
        }

        private UIElement GetFocusableParent()
        {
            DependencyObject current = this;
            while (current != null)
            {
                if (current is UIElement uiElement && uiElement.Focusable)
                    return uiElement;

                current = VisualTreeHelper.GetParent(current);
            }
            
            return null;
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(EditableLabel), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(EditableLabel), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(
            "IsEditing", typeof(bool), typeof(EditableLabel), new PropertyMetadata(default(bool)));

        public bool IsEditing
        {
            get => (bool) GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }
    }
}
