using System.Windows;
using System.Windows.Input;
using Pose.SceneEditor.MouseOperations;

namespace Pose.SceneEditor.Tools
{
    public class DrawBoneTool
    : IMouseTool
    {
        private readonly SceneEditorViewModel _sceneEditor;

        public DrawBoneTool(SceneEditorViewModel sceneEditor)
        {
            _sceneEditor = sceneEditor;
        }

        public void MouseLeftDown(Point mousePosition)
        {
            _sceneEditor.StartMouseDragOperation(new DrawBoneOperation(_sceneEditor, mousePosition.ToVector()));
        }

        public void MouseDoubleClick(Point mousePosition)
        {
            
        }

        public void MouseLeftUp(Point mousePosition)
        {
            
        }

        public Cursor MouseCursor => Cursors.Cross;
    }
}
