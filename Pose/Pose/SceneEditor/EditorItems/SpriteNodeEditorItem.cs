using Pose.SceneEditor.Gizmos;

namespace Pose.SceneEditor.EditorItems
{
    internal sealed class SpriteNodeEditorItem : EditorItem
    {
        private readonly SceneEditorViewModel _sceneEditor;
        private readonly SpriteBitmap _spriteBitmap;

        public SpriteNodeEditorItem(ulong nodeId, SceneEditorViewModel sceneEditor, SpriteBitmap spriteBitmap)
        : base(nodeId, sceneEditor)
        {
            _sceneEditor = sceneEditor;
            _spriteBitmap = spriteBitmap;

            CreateSubItems();
            RefreshPropertiesFromNode();
        }

        private void CreateSubItems()
        {
            _sceneEditor.SceneViewport.AddSpriteNode(NodeId, _spriteBitmap);
            _spriteBitmap.ResourcesReloaded += UpdateGizmoTransforms;
            SelectionGizmo = new SelectionGizmo(_sceneEditor, NodeId);
            RotationGizmo = new RotationGizmo(_sceneEditor, this);
            _sceneEditor.AddGizmo(SelectionGizmo);
            _sceneEditor.AddGizmo(RotationGizmo);
        }

        public override void Dispose()
        {
            _sceneEditor.SceneViewport.RemoveSpriteNode(NodeId);

            _sceneEditor.RemoveGizmo(SelectionGizmo);
            _sceneEditor.RemoveGizmo(RotationGizmo);

            SelectionGizmo.Dispose();
            RotationGizmo.Dispose();
        }

        public override void SetIsVisible(in bool isActive)
        {
            if (isActive)
            {
                _sceneEditor.SceneViewport.Show(NodeId);
            }
            else
            {
                _sceneEditor.SceneViewport.Hide(NodeId);
            }
        }

        public void UpdateGizmoTransforms()
        {
            if (SelectionGizmo.IsVisible)
                SelectionGizmo.UpdateTransform(_sceneEditor.SceneViewport);
            if (RotationGizmo.IsVisible)
                RotationGizmo.UpdateTransform(_sceneEditor.SceneViewport);
        }


        public override void UpdateVisuals()
        {
            _sceneEditor.SceneViewport.SetTransform(NodeId, Transformation.GlobalTransform.ToWpfMatrix());
            UpdateGizmoTransforms();
        }

        public override void ShowAsSelected()
        {
            SelectionGizmo.Show();
            RotationGizmo.Show();
        }

        public override void ShowAsNotSelected()
        {
            SelectionGizmo.Hide();
            RotationGizmo.Hide();
        }

        public SelectionGizmo SelectionGizmo { get; private set; }
    }
}
