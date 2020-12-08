namespace Pose.Domain.Documents.Events
{
    internal class AnimationRemovedEvent
    : IEvent
    {
        public ulong AnimationId { get; }
        public string Name { get; }
        public int BeginFrame { get; }
        public int EndFrame { get; }
        public uint Fps { get; }
        public bool IsLoop { get; }

        public AnimationRemovedEvent(ulong animationId, string name, int beginFrame, int endFrame, uint fps, bool isLoop)
        {
            AnimationId = animationId;
            Name = name;
            BeginFrame = beginFrame;
            EndFrame = endFrame;
            Fps = fps;
            IsLoop = isLoop;
        }

        public void PlayForward(IEditableDocument document)
        {
            document.RemoveAnimation(AnimationId);
        }

        public void PlayBackward(IEditableDocument document)
        {
            var animation = document.AddAnimation(AnimationId, Name, BeginFrame, EndFrame, Fps, IsLoop);
        }
    }
}
