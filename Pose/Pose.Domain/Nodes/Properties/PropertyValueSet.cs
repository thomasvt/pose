namespace Pose.Domain.Nodes.Properties
{
    public readonly struct PropertyValueSet
    {
        public PropertyType PropertyType { get; }
        public readonly float BaseValue, AnimateIncrement;

        public PropertyValueSet(PropertyType propertyType, float baseValue, float animateIncrement)
        {
            PropertyType = propertyType;
            AnimateIncrement = animateIncrement;
            BaseValue = baseValue;
        }
    }
}
