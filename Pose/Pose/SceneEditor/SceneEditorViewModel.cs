using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Pose.Domain;
using Pose.Domain.Editor;
using Pose.Domain.Nodes;
using Pose.SceneEditor.EditorItems;
using Pose.SceneEditor.Gizmos;
using Pose.SceneEditor.MouseOperations;
using Pose.SceneEditor.ToolBar;
using Pose.SceneEditor.Tools;
using Pose.SceneEditor.Viewport;

namespace Pose.SceneEditor
{
    public partial class SceneEditorViewModel
        : ViewModel
    {
        /// The SceneEditor abandons the MVVM pattern for the Viewport and Gizmo layers for performance reasons.
        /// It renders the spritenodes of the document as textured meshes with a global WPF transform applied.
        /// We don't build the skeleton in a WPF hierarchy. Instead, all nodes - including childnodes - are ROOT-visuals in the WPF Viewport.
        /// The editor sets their global transform based on Pose domain logic.
        /// This guarantees that we see exactly the same animations in the editor as in the runtimes, because we use the same math and ideas.

        internal readonly Editor Editor;
        internal SceneViewport SceneViewport;
        internal GizmoCanvas GizmoCanvasFront;
        internal GizmoCanvas GizmoCanvasBack;

        private readonly HashSet<IGizmo> _gizmos;
        private readonly SpriteBitmapStore _spriteBitmapStore;
        private MouseDragOperation _currentLeftMouseDragOperation; // the current mouse drag operation, started by mousetools, active while the left mouse button is down.
        private MouseDragOperation _currentMiddleMouseDragOperation; // the current mouse drag operation, started by mousetools, active while the left mouse button is down.
        private IMouseTool _currentMouseTool; // the tool represented by the selected button in the toolbar
        private readonly Dictionary<ulong, EditorItem> _items;
        private bool _isToolBarVisible;
        private Cursor _editorMouseCursor;

        public SceneEditorViewModel(Editor editor, ViewportToolBarViewModel viewportToolBarViewModel, SpriteBitmapStore spriteBitmapStore)
        {
            Editor = editor;
            _spriteBitmapStore = spriteBitmapStore;
            _items = new Dictionary<ulong, EditorItem>();
            _gizmos = new HashSet<IGizmo>();

            ViewportToolBar = viewportToolBarViewModel;

            ConfigureMessageHandling();
        }

        public void ViewLoaded(SceneViewport viewport, GizmoCanvas gizmoCanvasFront, GizmoCanvas gizmoCanvasBack)
        {
            SceneViewport = viewport;
            GizmoCanvasFront = gizmoCanvasFront;
            GizmoCanvasBack = gizmoCanvasBack;

            Clear();
            LoadCurrentDocument();
            UpdateAllGizmoTransforms();
        }

        internal void AddGizmo(IGizmo gizmo)
        {
            _gizmos.Add(gizmo);
        }

        internal void RemoveGizmo(IGizmo gizmo)
        {
            _gizmos.Remove(gizmo);
        }

        public void PanTo(Vector worldDistance)
        {
            SceneViewport.PanTo(worldDistance);
            UpdateAllGizmoTransforms();
        }

        private void LoadCurrentDocument()
        {
            if (!Editor.HasDocument)
                return;

            Clear();
            foreach (var node in Editor.CurrentDocument.GetAllNodes())
            {
                switch (node)
                {
                    case SpriteNode spriteNode:
                        AddSpriteNode(node.Id, spriteNode.SpriteRef);
                        break;
                    case BoneNode boneNode:
                        AddBoneNode(node.Id, boneNode.Name);
                        break;
                }
            }

            ReloadDrawOrder();
        }

        private void Clear()
        {
            SceneViewport.Clear();
            _spriteBitmapStore.Clear();
            _items.Clear();
            foreach (var gizmo in _gizmos)
            {
                gizmo.Dispose();
            }
            _gizmos.Clear();
            AddGizmo(new AxesGizmo(GizmoCanvasBack));
            UpdateAllGizmoTransforms(); // the axes gizmo
        }

        private void AddSpriteNode(ulong nodeId, SpriteReference spriteRef)
        {
            var spriteBitmap = _spriteBitmapStore.Get(spriteRef.RelativePath);
            var item = new SpriteNodeEditorItem(nodeId, this, spriteBitmap);
            _items.Add(nodeId, item);
        }

        private void AddBoneNode(in ulong nodeId, string name)
        {
            var item = new BoneNodeEditorItem(nodeId, this);
            _items.Add(nodeId, item);
        }

        private void ReloadDrawOrder()
        {
            var spriteNodeIdsInOrder = Editor.CurrentDocument.GetNodeIdsInDrawOrder();
            SceneViewport.SortVisuals(spriteNodeIdsInOrder); 
        }

        public void OnRenderSizeChanged()
        {
            UpdateAllGizmoTransforms();
        }

        internal void UpdateAllGizmoTransforms()
        {
            foreach (var gizmo in _gizmos)
            {
                gizmo.UpdateTransform(SceneViewport);
            }
        }

        private void UpdateTransformations()
        {
            foreach (var item in _items.Values)
            {
                item.RefreshTransformationFromNode();
            }
        }

        internal void StartMouseDragOperation(MouseDragOperation mouseDragOperation)
        {
            _currentLeftMouseDragOperation = mouseDragOperation;
            _currentLeftMouseDragOperation.Begin();
        }

        private void StartMouseTool(EditorTool tool)
        {
            switch (tool)
            {
                case EditorTool.Modify:
                    _currentMouseTool = new ModifyTool(this);
                    break;
                case EditorTool.DrawBone:
                    _currentMouseTool = new DrawBoneTool(this);
                    break;
                case EditorTool.Pose:
                    _currentMouseTool = new PoseTool(this);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported Editor Tool: {tool}");
            }

            EditorMouseCursor = _currentMouseTool.MouseCursor;
        }

        private void ZoomIn(Vector mouseFromCenter)
        {
            SceneViewport.ZoomIn();

            var panDistance = SceneViewport.ScreenToWorldDistance(mouseFromCenter * (SceneViewport.ZoomFactor - 1f));
            var position = new Vector(SceneViewport.SceneCamera.Position.X + panDistance.X,
                SceneViewport.SceneCamera.Position.Y + panDistance.Y);
            PanTo(position);
        }

        private void ZoomOut(Vector mouseFromCenter)
        {
            SceneViewport.ZoomOut();

            var panDistance =
                SceneViewport.ScreenToWorldDistance(mouseFromCenter / SceneViewport.ZoomFactor - mouseFromCenter);
            var position = new Vector(SceneViewport.SceneCamera.Position.X + panDistance.X,
                SceneViewport.SceneCamera.Position.Y + panDistance.Y);
            PanTo(position);
        }

        internal EditorItem GetEditorItem(ulong nodeId)
        {
            return _items[nodeId];
        }

        public bool IsToolBarVisible
        {
            get => _isToolBarVisible;
            set
            {
                if (_isToolBarVisible == value)
                    return;

                _isToolBarVisible = value;
                OnPropertyChanged();
            }
        }

        public Cursor EditorMouseCursor
        {
            get => _editorMouseCursor;
            set
            {
                if (_editorMouseCursor == value)
                    return;

                _editorMouseCursor = value;
                OnPropertyChanged();
            }
        }

        public ulong? GetTopmostNodeIdAt(Point mousePosition, ulong? afterNodeId)
        {
            var nodeId = GizmoCanvasFront.GetTopmostNodeIdAt(mousePosition);
            if (nodeId.HasValue)
                return nodeId;
            return SceneViewport.PickSpriteNodeAfter(mousePosition, afterNodeId);
        }

        public ViewportToolBarViewModel ViewportToolBar { get; set; }
    }
}