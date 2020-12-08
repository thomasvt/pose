using Pose.Domain.Nodes.Properties;

namespace Pose.Panels.Properties
{
    public class PropertyValueChanged
    {
        public PropertyType PropertyType { get; }
        public float NewValue { get; }
        /// <summary>
        /// The value change is from a mousedrag and is temporary (until mousebutton released and a non transient PropertyValueChanged is published)
        /// </summary>
        public bool IsTransient { get; }

        public PropertyValueChanged(PropertyType propertyType, float newValue, bool isTransient)
        {
            PropertyType = propertyType;
            NewValue = newValue;
            IsTransient = isTransient;
        }
    }
}
