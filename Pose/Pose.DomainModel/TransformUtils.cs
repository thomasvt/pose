using Pose.DomainModel.Nodes;

namespace Pose.DomainModel
{
    public static class TransformUtils
    {
        public static Vector2 WorldToLocalDistance(Vector2 worldDistance, Node parent)
        {
            if (parent == null)
                return worldDistance;

            // We convert a local unit vector on X and a unitvector on Y to world space using the parent's transform matrix.
            // Then, we can project the world space mouse pos onto those x and y units using dot product to know the corresponding values in local space.

            var parentTransform = parent.GetGlobalTransform();
            var xAxis = parentTransform.Transform(new Vector2(1, 0));
            var yAxis = parentTransform.Transform(new Vector2(0, 1));
            return new Vector2(worldDistance.Dot(xAxis), worldDistance.Dot(yAxis));
        }
    }
}
