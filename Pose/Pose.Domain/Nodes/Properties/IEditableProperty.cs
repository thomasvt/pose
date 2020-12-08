namespace Pose.Domain.Nodes.Properties
{
    internal interface IEditableProperty
    {
        void SetAnimateIncrement(float value);
        void SetDesignValue(float value);
        void LoadFromPropertyValueSet(in PropertyValueSet valueSet);
        void ResetAnimateValueToDesignPose();
    }
}
