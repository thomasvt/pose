namespace Pose.Panels.Animations
{
    public class AnimationViewModel
    : ViewModel
    {
        public AnimationViewModel(ulong animationId, string label)
        {
            AnimationId = animationId;
            Label = label;
        }

        public ulong AnimationId { get; }
        public string Label { get; }
    }
}
