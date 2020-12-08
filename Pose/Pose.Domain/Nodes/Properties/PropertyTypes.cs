namespace Pose.Domain.Nodes.Properties
{
    public static class PropertyTypes
    {
        public static bool IsTransformProperty(PropertyType propertyType)
        {
            return propertyType == PropertyType.RotationAngle || propertyType == PropertyType.TranslationX || propertyType == PropertyType.TranslationY;
        }
    }
}
