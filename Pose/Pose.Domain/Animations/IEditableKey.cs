namespace Pose.Domain.Animations
{
    internal interface IEditableKey
    {
        void ChangeValue(float value);
        void ChangeInterpolationData(InterpolationData data);
    }
}