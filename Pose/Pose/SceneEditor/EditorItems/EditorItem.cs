using System;
using System.Windows;
using Pose.Domain;
using Pose.Domain.Nodes.Properties;
using Pose.SceneEditor.Gizmos;

namespace Pose.SceneEditor.EditorItems
{
    internal abstract class EditorItem : IDisposable
    {
        private readonly SceneEditorViewModel _sceneEditor;

        protected EditorItem(ulong nodeId, SceneEditorViewModel sceneEditor)
        {
            _sceneEditor = sceneEditor;
            NodeId = nodeId;
        }

        public virtual void RefreshPropertiesFromNode()
        {
            Transformation = _sceneEditor.Editor.GetNodeTransformation(NodeId);
            SetIsVisible(_sceneEditor.Editor.GetNodePropertyAsBool(NodeId, PropertyType.Visibility));
            UpdateVisuals();
        }

        public virtual void RefreshFromNode()
        {
            Transformation = _sceneEditor.Editor.GetNodeTransformation(NodeId);
            SetIsVisible(_sceneEditor.Editor.GetNodePropertyAsBool(NodeId, PropertyType.Visibility));
            UpdateVisuals();
        }

        public abstract void UpdateVisuals();

        public abstract void ShowAsSelected();

        public abstract void ShowAsNotSelected();

        public Vector GetPositionInScreenSpace()
        {
            return _sceneEditor.SceneViewport.WorldToScreenPosition(Transformation.GlobalTranslation);
        }

        public abstract void Dispose();

        public Transformation Transformation { get; private set; }
        public ulong NodeId { get; }
        public RotationGizmo RotationGizmo { get; protected set; }

        public abstract void SetIsVisible(in bool isActive);
    }
}