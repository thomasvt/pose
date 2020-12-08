using Pose.Domain.Documents;

namespace Pose.Domain.Editor.Messages
{
    public class DocumentLoaded
    {
        public DocumentLoaded(Document document)
        {
            Document = document;
        }

        public Document Document { get; }
    }
}
