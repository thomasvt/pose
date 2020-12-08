using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Framework.Messaging;

namespace Pose.Panels.ModeSwitching
{
    public class ModeSwitchPanelViewModel
    : ViewModel
    {
        private readonly Editor _editor;
        private bool _isDesignMode;
        private bool _isAnimateMode;
        private bool _buttonIsChecked;

        public ModeSwitchPanelViewModel(Editor editor)
        {
            _editor = editor;

            MessageBus.Default.Subscribe<EditorModeChanged>(OnEditorModeChanged);
            OnEditorModeChanged(new EditorModeChanged(_editor.Mode));
        }

        private void OnEditorModeChanged(EditorModeChanged msg)
        {
            IsDesignMode = msg.Mode == EditorMode.Design;
            ButtonIsChecked = IsAnimateMode = msg.Mode == EditorMode.Animate;
        }

        public void ActivateAnimateMode()
        {
            if (!IsAnimateMode)
                _editor.ChangeEditorMode(EditorMode.Animate);
        }

        public void ActivateDesignMode()
        {
            if (!IsDesignMode)
                _editor.ChangeEditorMode(EditorMode.Design);
        }

        public bool IsDesignMode
        {
            get => _isDesignMode;
            set
            {
                if (value == _isDesignMode)
                    return;

                _isDesignMode = value;
                OnPropertyChanged();
            }
        }

        public bool IsAnimateMode
        {
            get => _isAnimateMode;
            set
            {
                if (value == _isAnimateMode)
                    return;

                _isAnimateMode = value;
                OnPropertyChanged();
            }
        }

        public bool ButtonIsChecked
        {
            get => _buttonIsChecked;
            set
            {
                if (value == _buttonIsChecked)
                    return;

                _buttonIsChecked = value;
                OnPropertyChanged();

                _editor.ChangeEditorMode(value ? EditorMode.Animate : EditorMode.Design);
            }
        }
    }
}
