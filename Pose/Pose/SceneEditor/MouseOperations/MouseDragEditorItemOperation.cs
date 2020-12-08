using System.Windows;
using Pose.SceneEditor.EditorItems;

namespace Pose.SceneEditor.MouseOperations
{
    internal abstract class MouseDragEditorItemOperation
    : MouseDragOperation
    {
        protected readonly EditorItem EditorItem;

        protected MouseDragEditorItemOperation(EditorItem editorItem, Vector initialMousePosition)
        : base(initialMousePosition)
        {
            EditorItem = editorItem;
        }
    }
}
