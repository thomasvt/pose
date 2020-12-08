using System;

namespace Pose.Framework.Messaging
{
    public interface IMessageBus
    {
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Publish<T>(T message);
    }
}