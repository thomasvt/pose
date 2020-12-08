using Pose.Framework.Messaging;

namespace Pose.Domain
{
    public abstract class Entity
    {
        public ulong Id { get; }

        protected Entity(IMessageBus messageBus, ulong id)
        {
            MessageBus = messageBus;
            Id = id;
        }

        protected IMessageBus MessageBus;
    }
}
