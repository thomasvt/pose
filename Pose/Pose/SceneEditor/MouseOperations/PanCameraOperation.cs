using System.Windows;
using System.Windows.Media.Media3D;

namespace Pose.SceneEditor.MouseOperations
{
    internal class PanCameraOperation : MouseDragOperation
    {
        private readonly SceneEditorViewModel _sceneEditor;
        private Point3D _initialCameraPosition;

        public PanCameraOperation(SceneEditorViewModel sceneEditor, Vector initialMousePosition) : base(initialMousePosition)
        {
            _sceneEditor = sceneEditor;
            _initialCameraPosition = sceneEditor.SceneViewport.SceneCamera.Position;
        }

        public override void UpdatePosition(Vector screenPosition)
        {
            var deltaMousePosition = screenPosition - InitialMousePosition;

            deltaMousePosition = new Vector(deltaMousePosition.X, -deltaMousePosition.Y);
            var worldDistance = _sceneEditor.SceneViewport.ScreenToWorldDistance(deltaMousePosition);

            _sceneEditor.PanTo(new Vector(_initialCameraPosition.X - worldDistance.X, _initialCameraPosition.Y - worldDistance.Y));
        }
    }
}
