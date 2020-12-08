using System.IO;
using System.Text;
using Pose.Domain.Documents;

namespace Pose.Domain.Editor.Serialization
{
    public static class DocumentSerializer
    {
        private const int DocumentVersion = 1;

        public static void Serialize(BinaryWriter writer, Document document)
        {
            writer.Write(Encoding.ASCII.GetBytes("pose"));
            writer.Write(DocumentVersion);
        }

        public static Document Deserialize(BinaryReader reader)
        {
            return null;
        }
    }
}
