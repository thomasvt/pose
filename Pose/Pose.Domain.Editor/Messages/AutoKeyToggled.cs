namespace Pose.Domain.Editor.Messages
{
    public class AutoKeyToggled
    {
        public readonly bool IsAutoKeying;

        public AutoKeyToggled(bool isAutoKeying)
        {
            IsAutoKeying = isAutoKeying;
        }
    }
}
