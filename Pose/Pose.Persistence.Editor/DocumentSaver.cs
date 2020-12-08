using System.IO;
using Google.Protobuf;

namespace Pose.Persistence.Editor
{
    public class DocumentSaver
    {
        // The Persistence assembly had access to the internals of the domain, so it can read the data.

        public void SaveToFile(Domain.Documents.Document document)
        {
            var doc = ProtoModelBuilder.CreateProtobufDocument(document);
            using var stream = File.Open(document.Filename, FileMode.Create, FileAccess.Write, FileShare.None);
            using var codedStream = new CodedOutputStream(stream);
            doc.WriteTo(codedStream);
        }

        
    }
}
