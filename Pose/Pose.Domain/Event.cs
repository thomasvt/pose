using Pose.Domain.Documents;

namespace Pose.Domain
{
    internal interface IEvent
    {
        void PlayForward(IEditableDocument document);
        void PlayBackward(IEditableDocument document);
    }
}
