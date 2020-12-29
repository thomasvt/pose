using System.IO;
using Google.Protobuf;
using Pose.Framework.Messaging;

namespace Pose.Persistence.Editor
{

    public static class DocumentLoader
    {
        public static Domain.Documents.Document LoadFromFile(IMessageBus messageBus, string filePath)
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var codedStream = new CodedInputStream(stream);
            var doc = Document.Parser.ParseFrom(codedStream);
            return DomainDocumentBuilder.CreateDocument(messageBus, doc, filePath);
        }
    }
}
