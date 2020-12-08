namespace Pose.Domain
{
    public static class TransformUtils
    {
        public static Vector2 WorldToLocalDistance(Vector2 worldDistance, Matrix? parentGlobalTransform)
        {
            if (parentGlobalTransform == null)
                return worldDistance;

            // We convert a local unit vector on X and a unitvector on Y to world space using the parent's transform matrix.
            // Then, we can project the world space pos onto those x and y units using dot product to know the corresponding values in local space.

            var xAxis = parentGlobalTransform.Value.TransformDistance(new Vector2(1, 0));
            var yAxis = parentGlobalTransform.Value.TransformDistance(new Vector2(0, 1));
            return new Vector2(worldDistance.Dot(xAxis), worldDistance.Dot(yAxis));
        }

        public static Vector2 WorldToLocalPosition(Vector2 worldPosition, Matrix? parentGlobalTransform)
        {
            if (parentGlobalTransform == null)
                return worldPosition;

            // normalize to origin of parent:
            worldPosition -= parentGlobalTransform.Value.GetTranslation();

            // We convert a local unit vector on X and a unitvector on Y to world space using the parent's transform matrix.
            // Then, we can project the world space pos onto those x and y units using dot product to know the corresponding distances in local space.
            var xAxis = parentGlobalTransform.Value.TransformDistance(new Vector2(1, 0));
            var yAxis = parentGlobalTransform.Value.TransformDistance(new Vector2(0, 1));
            return new Vector2(worldPosition.Dot(xAxis), worldPosition.Dot(yAxis));
        }
    }
}
