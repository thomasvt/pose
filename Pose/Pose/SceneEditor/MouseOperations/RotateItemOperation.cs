using System;
using Pose.SceneEditor.EditorItems;
using Vector = System.Windows.Vector;

namespace Pose.SceneEditor.MouseOperations
{
    internal class RotateItemOperation
    : MouseDragEditorItemOperation
    {
        private readonly SceneEditorViewModel _sceneEditor;
        private readonly float _initialAngle;
        private readonly RotationTracker _rotationTracker;
        private readonly float _initialMouseAngle;

        public RotateItemOperation(SceneEditorViewModel sceneEditor, EditorItem editorItem, Vector initialMousePosition)
            : base(editorItem, initialMousePosition)
        {
            _sceneEditor = sceneEditor;
            var initialMouseFromCenter = initialMousePosition - editorItem.GetPositionInScreenSpace();
            _initialAngle = editorItem.Transformation.LocalRotation;
            
            _initialMouseAngle = (float)Math.Atan2(-initialMouseFromCenter.Y, initialMouseFromCenter.X);
            _rotationTracker = new RotationTracker(_initialMouseAngle, _initialAngle);
        }

        public override void UpdatePosition(Vector screenPosition)
        {
            var mouseFromCenter = screenPosition - EditorItem.GetPositionInScreenSpace();
            var angle = (float)Math.Atan2(-mouseFromCenter.Y, mouseFromCenter.X); // between -pi and +pi

            _rotationTracker.AddAngleInput(angle);

            EditorItem.RotationGizmo.ShowAngle(_initialMouseAngle, _initialMouseAngle + (_rotationTracker.Angle - _initialAngle));

            _sceneEditor.Editor.SetNodeRotationVisual(EditorItem.NodeId, _rotationTracker.Angle);
        }

        public override void Begin()
        {
            EditorItem.RotationGizmo.StartHighlight();
        }

        public override void Cancel()
        {
            EndRotationDragging();
            _sceneEditor.Editor.ResetNodeRotationVisual(EditorItem.NodeId);
        }

        public override void Finish()
        {
            EndRotationDragging();

            _sceneEditor.Editor.SetNodeRotation(EditorItem.NodeId, _rotationTracker.Angle);
        }

        private void EndRotationDragging()
        {
            EditorItem.RotationGizmo.StopHighlight();
            EditorItem.RotationGizmo.HideAngle();
        }
    }
}
