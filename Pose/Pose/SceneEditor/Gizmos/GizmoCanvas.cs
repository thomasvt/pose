using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pose.SceneEditor.Gizmos
{
    public class GizmoCanvas : Canvas
    {
        public ulong? GetTopmostNodeIdAt(Point mousePosition)
        {
            ulong? selectedNodeId = null;

            // this is not the most robust code, but it's an isolated detail.
            // We find a BoneGizmo (which represents a BoneNode) by finding the Path that is inside it and checking for its parent being a BoneGizmo.

            VisualTreeHelper.HitTest(this,
                target => selectedNodeId == null ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.Stop, r =>
                {
                    if (!((r.VisualHit as FrameworkElement)?.Parent is BonePath bonePath))
                        return HitTestResultBehavior.Continue;

                    selectedNodeId = bonePath.BoneNodeId;
                    return HitTestResultBehavior.Stop;
                },
                new PointHitTestParameters(mousePosition));

            return selectedNodeId;
        }
    }
}
