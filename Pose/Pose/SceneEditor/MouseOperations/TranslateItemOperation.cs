using System.Windows;
using Pose.Domain;
using Pose.SceneEditor.EditorItems;

namespace Pose.SceneEditor.MouseOperations
{
    internal class TranslateItemOperation : MouseDragEditorItemOperation
    {
        private readonly SceneEditorViewModel _sceneEditor;
        private const int MinimumTranslateThreshold = 10;
        private bool _isDragging;
        private Vector2 _currentLocalTranslation;
        private Vector _initialMouseOffset;

        public TranslateItemOperation(SceneEditorViewModel sceneEditor, EditorItem item, Vector initialMousePosition) : base(item, initialMousePosition)
        {
            _sceneEditor = sceneEditor;
        }

        public override void Begin()
        {
            _initialMouseOffset = InitialMousePosition - EditorItem.GetPositionInScreenSpace(); 
        }

        public override void UpdatePosition(Vector screenPosition)
        {
            var deltaMousePosition = screenPosition - InitialMousePosition;

            if (!_isDragging)
            {
                if (deltaMousePosition.Length < MinimumTranslateThreshold)
                    return;
                _isDragging = true; // don't drag until the intent to drag is clear.
            }

            var newScreenPosition = screenPosition - _initialMouseOffset;
            var currentWorldPosition = _sceneEditor.SceneViewport.ScreenToWorldPosition(newScreenPosition);
            _currentLocalTranslation = TransformUtils.WorldToLocalPosition(currentWorldPosition, GetParentGlobalTransform());

            _sceneEditor.Editor.SetNodeTranslationVisual(EditorItem.NodeId, _currentLocalTranslation);
        }

        public override void Cancel()
        {
            if (_isDragging)    
            {
                _sceneEditor.Editor.ResetNodeTranslationVisual(EditorItem.NodeId);
            }
        }

        public override void Finish()
        {
            if (_isDragging)
            {
                _sceneEditor.Editor.SetNodeTranslation(EditorItem.NodeId, _currentLocalTranslation);
            }
        }

        private Matrix? GetParentGlobalTransform()
        {
            var parentId = _sceneEditor.Editor.CurrentDocument.GetNode(EditorItem.NodeId).Parent?.Id;
            var parentGlobalTransform = parentId == null
                ? null
                : (Matrix?) _sceneEditor.Editor.GetNodeTransformation(parentId.Value).GlobalTransform;
            return parentGlobalTransform;
        }
    }
}
