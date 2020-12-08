using System.Collections.Generic;

namespace Pose.Framework
{
    public class TwowayIndex<TA, TB>
    {
        private readonly Dictionary<TA, TB> _ab;
        private readonly Dictionary<TB, TA> _ba;

        public TwowayIndex()
        {
            _ab = new Dictionary<TA, TB>();
            _ba = new Dictionary<TB, TA>();
        }

        public IEnumerable<TB> RightKeys => _ab.Values;
        public IEnumerable<TA> LeftKeys => _ab.Keys;

        public void Add(TA a, TB b)
        {
            _ab.Add(a, b);
            _ba.Add(b, a);
        }

        public void Remove(TA a)
        {
            var b = this[a];
            _ab.Remove(a);
            _ba.Remove(b);
        }

        public bool TryGet(in TA a, out TB b)
        {
            return _ab.TryGetValue(a, out b);
        }

        public bool TryGet(in TB b, out TA a)
        {
            return _ba.TryGetValue(b, out a);
        }

        public bool ContainsLeftKey(in TA leftKey)
        {
            return _ab.ContainsKey(leftKey);
        }

        public bool ContainsRightKey(in TB rightKey)
        {
            return _ba.ContainsKey(rightKey);
        }

        public void Clear()
        {
            _ab.Clear();
            _ba.Clear();
        }

        public TB this[TA a] => _ab[a];

        public TA this[TB b] => _ba[b];
    }
}
