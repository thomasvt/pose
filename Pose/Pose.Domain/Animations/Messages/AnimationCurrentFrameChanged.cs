namespace Pose.Domain.Animations.Messages
{
    /// <summary>
    /// Raised when the currently selected frame of a certain animation has changed.
    /// </summary>
    public class AnimationCurrentFrameChanged
    {
        public ulong AnimationId { get; }
        /// <summary>
        /// Set when UI must be updated, but not the scene. This happens when realtime animation is playing. The scene is updated according the actual WPF frametimes which are not aligned at whole integer frames of the animation.
        /// </summary>
        public bool NoSceneUpdate { get; }

        public readonly int Frame;

        public AnimationCurrentFrameChanged(ulong animationId, int frame, bool noSceneUpdate)
        {
            AnimationId = animationId;
            NoSceneUpdate = noSceneUpdate;
            Frame = frame;
        }
    }
}
