using System;
using System.Collections.Generic;

namespace Pose.Framework.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly Dictionary<Type, List<object>> _actions;

        public MessageBus()
        {
            _actions = new Dictionary<Type, List<object>>();
        }

        public void Subscribe<T>(Action<T> handler)
        {
            if (!_actions.TryGetValue(typeof(T), out var list))
            {
                list = new List<object>();
                _actions.Add(typeof(T), list);
            }
            list.Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            if (!_actions.TryGetValue(typeof(T), out var list))
                return;

            list.Remove(handler);
        }

        public void Publish<T>(T message)
        {
            if (!_actions.TryGetValue(message.GetType(), out var list))
                return;

            foreach (Action<T> action in list)
            {
                action.Invoke(message);
            }
        }

        public static IMessageBus Default = new MessageBus();
    }
}
