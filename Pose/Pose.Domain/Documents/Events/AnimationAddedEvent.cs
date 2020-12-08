namespace Pose.Domain.Documents.Events
{
    internal class AnimationAddedEvent
    : IEvent
    {
        public ulong AnimationId { get; }
        public string Name { get; }
        public int BeginFrame { get; }
        public int EndFrame { get; }
        public uint FramesPerSecond { get; }
        public bool IsLoop { get; }

        public AnimationAddedEvent(ulong animationId, string name, int beginFrame, int endFrame, uint framesPerSecond, bool isLoop)
        {
            AnimationId = animationId;
            Name = name;
            BeginFrame = beginFrame;
            EndFrame = endFrame;
            FramesPerSecond = framesPerSecond;
            IsLoop = isLoop;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.AddAnimation(AnimationId, Name, BeginFrame, EndFrame, FramesPerSecond, IsLoop);
        }

        public void PlayBackward(IEditableDocument document)
        {
            document.RemoveAnimation(AnimationId);
        }
    }
}
