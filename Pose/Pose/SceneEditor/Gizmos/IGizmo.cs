using System;
using Pose.SceneEditor.Viewport;

namespace Pose.SceneEditor.Gizmos
{
    /// <summary>
    /// Gizmos are visual constructs in the scene. Often handles for the mouse to manipulate things, or other visual aids eg. the rotation ring, a selection marker around a node, bones.
    /// </summary>
    internal interface IGizmo : IDisposable
    {
        void UpdateTransform(SceneViewport sceneViewport);
    }
}