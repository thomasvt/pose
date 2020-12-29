using System.IO;
using Google.Protobuf;

namespace Pose.Persistence.Editor
{
    public static class ProtobufSaver
    {
        // The Persistence assembly had access to the internals of the domain, so it can read the data.

        public static void SaveDocument(Domain.Documents.Document document)
        {
            var doc = ProtoDocumentBuilder.CreateProtobufDocument(document);
            Save(doc, document.Filename);
        }

        public static void Save(IMessage message, string filename)
        {
            using var stream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            using var codedStream = new CodedOutputStream(stream);
            message.WriteTo(codedStream);
        }
    }
}
