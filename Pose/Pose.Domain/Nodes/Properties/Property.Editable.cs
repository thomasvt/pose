namespace Pose.Domain.Nodes.Properties
{
    public partial class Property
    {
        void IEditableProperty.SetAnimateIncrement(float value)
        {
            AnimateIncrement = value;
            SetAnimateVisualValue(GetAnimateNetValue(value));
        }

        void IEditableProperty.SetDesignValue(float value)
        {
            DesignValue = value;
            SetDesignVisualValue(value);
        }

        void IEditableProperty.LoadFromPropertyValueSet(in PropertyValueSet valueSet)
        {
            AnimateIncrement = valueSet.AnimateIncrement;
            ((IEditableProperty)this).SetDesignValue(valueSet.BaseValue);
        }

        void IEditableProperty.ResetAnimateValueToDesignPose()
        {
            if (_isIncrementalValueProperty)
            {
                // set the increment to 0, so that net value == designvalue
                ((IEditableProperty)this).SetAnimateIncrement(0f);
            }
            else
            {
                // increment IS the net value, so set it to the designvalue.
                ((IEditableProperty)this).SetAnimateIncrement(DesignValue);
            }
        }
    }
}