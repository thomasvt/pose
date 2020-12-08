using System.Collections;
using System.Collections.Generic;

namespace Pose.Domain
{
    public class EntityCollection<TEntity> : IEnumerable<TEntity>
        where TEntity : Entity
    {
        private readonly Dictionary<ulong, TEntity> _entities;

        public EntityCollection()
        {
            _entities = new Dictionary<ulong, TEntity>();
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity.Id, entity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _entities.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(in ulong id)
        {
            _entities.Remove(id);
        }

        public TEntity this[ulong id] => _entities[id];

        public int Count => _entities.Count;
    }
}
