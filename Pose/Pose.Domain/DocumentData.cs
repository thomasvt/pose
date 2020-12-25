using System.Runtime.CompilerServices;
using Pose.Domain.Animations;
using Pose.Domain.Documents;
using Pose.Domain.Nodes;
using Pose.Framework.Messaging;


// allow Pose.Persistence.Editor to read/set the internal data directly
[assembly: InternalsVisibleTo("Pose.Persistence")]
[assembly: InternalsVisibleTo("Pose.Domain.Editor")]

namespace Pose.Domain
{
    public class DocumentData
    {
        public IMessageBus MessageBus { get; }
        public readonly Sequence IdSequence; // generates ids that are unique within the Document.

        // The physical data of which the document consists. This is the data saved/loaded from file.
        internal readonly EntityCollection<Node> Nodes;
        internal readonly EntityCollection<Animation> Animations;
        internal readonly EntityCollection<PropertyAnimation> PropertyAnimations;
        internal readonly EntityCollection<Key> Keys;
        internal readonly DrawOrder DrawOrder;
        internal readonly NodeCollection RootNodes;
        internal string AbsoluteAssetFolder;
        internal string RelativeAssetFolder;

        public DocumentData(IMessageBus messageBus, ulong highestEntityId)
        {
            MessageBus = messageBus;
            RootNodes = new NodeCollection(messageBus, null);
            DrawOrder = new DrawOrder(messageBus);
            IdSequence = new Sequence(highestEntityId);

            Nodes = new EntityCollection<Node>();
            Animations = new EntityCollection<Animation>();
            PropertyAnimations = new EntityCollection<PropertyAnimation>();
            Keys = new EntityCollection<Key>();
        }
    }
}
