using System.Windows;
using Pose.Common;
using Pose.Domain;
using Pose.SceneEditor.Gizmos;

namespace Pose.SceneEditor.MouseOperations
{
    internal class DrawBoneOperation
    : MouseDragOperation
    {
        private readonly SceneEditorViewModel _sceneEditor;
        private BoneGizmo _gizmo;
        private readonly Vector2 _positionWorld;
        private Vector2 _positionTailWorld;

        public DrawBoneOperation(SceneEditorViewModel sceneEditor, Vector initialMousePosition) : base(initialMousePosition)
        {
            _sceneEditor = sceneEditor;
            _positionWorld = _sceneEditor.SceneViewport.ScreenToWorldPosition(initialMousePosition);
            CreateBoneGizmo(_positionWorld);
        }

        public override void UpdatePosition(Vector screenPosition)
        {
            _positionTailWorld = _sceneEditor.SceneViewport.ScreenToWorldPosition(screenPosition);
            var tail = _positionTailWorld - _positionWorld;
            _gizmo.SetTailLength(tail.Magnitude);
            _gizmo.SetAngle(tail.GetAngle());
            _gizmo.UpdateTransform(_sceneEditor.SceneViewport);
        }

        public override void Finish()
        {
            var tail = _positionTailWorld - _positionWorld;
            _sceneEditor.Editor.AddBoneNode(_positionWorld, tail.GetAngle(), tail.Magnitude);
            _gizmo?.Dispose();
            _sceneEditor.RemoveGizmo(_gizmo);
        }

        public override void Cancel()
        {
            _gizmo?.Dispose();
            _sceneEditor.RemoveGizmo(_gizmo);
        }

        private void CreateBoneGizmo(Vector2 positionWorld)
        {
            _gizmo = new BoneGizmo(_sceneEditor, null);
            
            _gizmo.SetWorldPosition(positionWorld);
            _gizmo.SetTailLength(0f);
            _gizmo.UpdateTransform(_sceneEditor.SceneViewport);
            _sceneEditor.AddGizmo(_gizmo);
        }
    }
}
