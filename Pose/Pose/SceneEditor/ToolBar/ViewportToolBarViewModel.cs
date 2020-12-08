using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Framework.Messaging;

namespace Pose.SceneEditor.ToolBar
{
    public class ViewportToolBarViewModel
    : ViewModel
    {
        private readonly Editor _editor;
        private EditorTool _currentTool;

        public ViewportToolBarViewModel(Editor editor)
        {
            _editor = editor;
            MessageBus.Default.Subscribe<EditorToolChanged>(OnEditorToolChanged);
        }

        private void OnEditorToolChanged(EditorToolChanged msg)
        {
            CurrentTool = msg.Tool;
        }

        public void SetCurrentTool(EditorTool tool)
        {
            _editor.ChangeEditorTool(tool);
        }

        public EditorTool CurrentTool
        {
            get => _currentTool;
            set
            {
                if (_currentTool == value)
                    return;

                _currentTool = value;
                OnPropertyChanged();
            }
        }
    }
}
