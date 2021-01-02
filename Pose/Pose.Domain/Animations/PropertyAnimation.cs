using System;
using System.Collections.Generic;
using System.Linq;
using Pose.Common.Curves;
using Pose.Domain.Animations.Events;
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

        public float GetValueAt(in float frame, in int firstFrame, int lastFrame, bool isLoop)
        {
            if (!_keys.Any())
                return 0f; // no animation, nothing to tell...

            // loop over all keys in chronological order, the first key with a frame larger than the frame we're looking for is the end key of the segment we're in.
            var frameInt = (int) frame;
            Key prevKey = null;
            int prevKeyFrame = 0;
            foreach (var pair in _keys)
            {
                var key = pair.Value;
                
                if (key.Frame > frameInt)
                {
                    if (prevKey == null) // we're before the first key in the animation -> there is no key to the left
                    {
                        if (isLoop)
                        {
                            // wrap by using last key on timeline as prev key:
                            prevKey = _keys.Values[new Index(_keys.Count - 1)];
                            prevKeyFrame = firstFrame - (lastFrame - prevKey.Frame + 1); // pretend the wrapped keyframe is further to the left so we can interpolate.
                        }
                        else
                            return key.Value;
                    }

                    // we're between prevkey and key -> interpolate
                    var x = (frame - prevKeyFrame) / (key.Frame - prevKeyFrame);
                    var y = prevKey.Interpolation.CalculateY(x); // -> returns a percentage
                    return prevKey.Value * (1f - y) + key.Value * y; 
                }

                prevKey = key;
                prevKeyFrame = key.Frame;
            }

            // requested frame is higher than last key found:

            if (isLoop) // + isloop -> wrap around to the beginning
            {
                // wrap by using first key on timeline as next key.
                var nextKey = _keys.Values[new Index(0)];
                var nextKeyFrame = lastFrame + nextKey.Frame - firstFrame + 1; // pretend the wrapped keyframe is further to the right so we can intepolate
                var x = (frame - prevKey.Frame) / (nextKeyFrame - prevKey.Frame);
                var y = prevKey.Interpolation.CalculateY(x); // -> returns a percentage
                return prevKey.Value * (1f - y) + nextKey.Value * y;
            }

            // ... not a loop, just return the value of last key.
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
