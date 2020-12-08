namespace Pose.Domain.Animations
{
    public interface IEditablePropertyAnimation
    {
        void AddKey(Key key);
        void RemoveKey(Key key);
    }
}