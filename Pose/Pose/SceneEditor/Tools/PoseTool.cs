using System.Windows;
using System.Windows.Input;
using Pose.SceneEditor.MouseOperations;

namespace Pose.SceneEditor.Tools
{
    internal class PoseTool
    : IMouseTool
    {
        private readonly SceneEditorViewModel _sceneEditor;

        public PoseTool(SceneEditorViewModel sceneEditor)
        {
            _sceneEditor = sceneEditor;
        }

        public void MouseLeftDown(Point mousePosition)
        {
            var nodeId = _sceneEditor.GetTopmostNodeIdAt(mousePosition, null);

            if (nodeId != null)
            {
                _sceneEditor.Editor.NodeSelection.SelectSingle(nodeId.Value);
                var sceneEditorItem = _sceneEditor.GetEditorItem(nodeId.Value);
                _sceneEditor.StartMouseDragOperation(new TranslateItemOperation(_sceneEditor, sceneEditorItem, mousePosition.ToVector()));
            }
            else
            {
                _sceneEditor.Editor.NodeSelection.Clear();
            }
        }

        public void MouseDoubleClick(Point mousePosition)
        {
            
        }

        public void MouseLeftUp(Point mousePosition)
        {
            
        }

        public Cursor MouseCursor => Cursors.Arrow;
    }
}
