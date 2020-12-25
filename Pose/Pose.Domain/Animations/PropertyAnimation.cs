using System;
using System.Collections.Generic;
using System.Linq;
using Pose.Domain.Animations.Events;
using Pose.Domain.Curves;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Animations
{
    public partial class PropertyAnimation
    : Entity, IEditablePropertyAnimation
    {
        private readonly SortedList<int, Key> _keys;

        public PropertyAnimation(IMessageBus messageBus, ulong id, ulong animationId, ulong nodeId, PropertyType property, uint vertex = 0) // vertex is reserved for future per corner animations, 0 means apply to all vertices of quad
        : base(messageBus, id)
        {
            Vertex = vertex;
            AnimationId = animationId;
            NodeId = nodeId;
            Property = property;
            _keys = new SortedList<int, Key>();
        }

        internal void AddOrUpdateKey(IUnitOfWork uow, in int frame, float value)
        {
            if (_keys.TryGetValue(frame, out var key))
            {
                key.ChangeValue(uow, value);
            }
            else
            {
                var interpolation = GetDefaultInterpolation();
                uow.Execute(new AnimationKeyAddedEvent(Id, uow.GetNewEntityId(), frame, value, interpolation));
            }
        }

        private InterpolationData GetDefaultInterpolation()
        {
            return Property == PropertyType.Visibility 
                ? new InterpolationData(CurveType.Hold) 
                : InterpolationData.EasingLow;
        }

        internal void Clear(IUnitOfWork uow)
        {
            while (_keys.Any())
                RemoveKey(uow, _keys.Values.First());
        }

        internal void RemoveKey(IUnitOfWork uow, int frame)
        {
            if (!_keys.ContainsKey(frame))
                throw new Exception($"Cannot delete Key: PropertyAnimation has no key at frame {frame}.");

            var key = _keys[frame];
            RemoveKey(uow, key);
        }

        private void RemoveKey(IUnitOfWork uow, Key key)
        {
            uow.Execute(new AnimationKeyRemovedEvent(Id, key.Id, key.Frame, key.Value, key.Interpolation));
        }

        internal void RemoveKeys(IUnitOfWork uow, HashSet<ulong> keyIds)
        {
            foreach (var key in _keys.Values.ToList())
            {
                if (keyIds.Contains(key.Id))
                    RemoveKey(uow, key);
            }
        }

        public float GetValueAt(in float frame)
        {
            var frameInt = (int) frame;
            Key prevKey = null;
            foreach (var pair in _keys)
            {
                var nextKey = pair.Value;

                if (nextKey.Frame == frameInt)
                    return pair.Value.Value;
                
                if (nextKey.Frame > frameInt)
                {
                    if (prevKey == null) // we're before the first key = no left side key to interpolate with: just return the right side key's value.
                        return nextKey.Value;

                    // interpolate
                    var x = (frame - prevKey.Frame) / (nextKey.Frame - prevKey.Frame);
                    var y = prevKey.Interpolation.CalculateY(x); // -> percentages
                    return prevKey.Value * (1f - y) + nextKey.Value * y;
                }

                prevKey = nextKey;
            }

            // requested frame is higher than last key found, so return value of that last key
            return prevKey?.Value ?? 0f;
        }

        public Key GetKeyAt(in int frame)
        {
            _keys.TryGetValue(frame, out var key);
            return key;
        }

        internal void InternalAdd(Key key)
        {
            _keys.Add(key.Frame, key);
        }

        public bool IsEmpty() => !_keys.Any();
        public ulong AnimationId { get; }
        public ulong NodeId { get; }
        public IEnumerable<Key> Keys => _keys.Values;
        public uint Vertex { get; }
        public PropertyType Property { get; }
    }
}
