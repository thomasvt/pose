using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;
using Pose.SceneEditor.Gizmos;

namespace Pose.SceneEditor.EditorItems
{
    internal sealed class BoneNodeEditorItem : EditorItem
    {
        private readonly SceneEditorViewModel _sceneEditor;

        public BoneNodeEditorItem(ulong nodeId, SceneEditorViewModel sceneEditor)
        : base(nodeId, sceneEditor)
        {
            _sceneEditor = sceneEditor;

            CreateSubItems();
            RefreshTransformationFromNode();
        }

        private void CreateSubItems()
        {
            BoneGizmo = new BoneGizmo(_sceneEditor, this);
            RotationGizmo = new RotationGizmo(_sceneEditor, this);

            _sceneEditor.AddGizmo(BoneGizmo);
            _sceneEditor.AddGizmo(RotationGizmo);
        }

        private void UpdateGizmoTransforms()
        {
            BoneGizmo.UpdateTransform(_sceneEditor.SceneViewport);
            if (RotationGizmo.IsVisible)
                RotationGizmo.UpdateTransform(_sceneEditor.SceneViewport);
        }

        public override void UpdateVisuals()
        {
            var node = _sceneEditor.Editor.CurrentDocument.GetNode(NodeId) as BoneNode;
            BoneGizmo.SetWorldPosition(Transformation.GlobalTranslation);
            BoneGizmo.SetAngle(Transformation.GlobalRotation);
            BoneGizmo.SetTailLength(node.GetProperty(PropertyType.BoneLength).DesignVisualValue);

            UpdateGizmoTransforms();
        }

        public override void ShowAsSelected()
        {
            BoneGizmo.ShowAsSelected();
            RotationGizmo.Show();
        }

        public override void ShowAsNotSelected()
        {
            BoneGizmo.ShowAsNotSelected();
            RotationGizmo.Hide();
        }

        public override void Dispose()
        {
            _sceneEditor.RemoveGizmo(BoneGizmo);
            _sceneEditor.RemoveGizmo(RotationGizmo);
            BoneGizmo.Dispose();
            RotationGizmo.Dispose();
        }

        public override void SetIsVisible(in bool isActive)
        {
            if (isActive)
            {
                BoneGizmo.Show();
            }
            else
            {
                BoneGizmo.Hide();
            }
        }

        public BoneGizmo BoneGizmo { get; private set; }
    }
}
