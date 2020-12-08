namespace Pose.Domain.Documents.Messages
{
    public class CurrentDocumentFilenameChanged
    {
        public string Filename { get; }

        public CurrentDocumentFilenameChanged(string filename)
        {
            Filename = filename;
        }
    }
}
