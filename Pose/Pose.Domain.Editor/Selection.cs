using System;
using System.Collections.Generic;
using System.Linq;

namespace Pose.Domain.Editor
{
    public class Selection<T> : ISelection<T>
    {
        private readonly HashSet<T> _selectedItems;

        internal Selection()
        {
            _selectedItems = new HashSet<T>();
        }

        /// <summary>
        /// Replaces the current selection by just the given item.
        /// </summary>
        public void SelectSingle(T itemToSelect)
        {
            var alreadySelected = false;
            foreach (var item in _selectedItems.ToList())
            {
                if (!item.Equals(itemToSelect))
                {
                    Remove(item);
                }
                else
                {
                    alreadySelected = true;
                }
            }

            if (!alreadySelected)
            {
                Add(itemToSelect);
            }
        }

        public void Add(T item)
        {
            if (_selectedItems.Contains(item))
                return;

            _selectedItems.Add(item);
            SelectAction?.Invoke(item);
        }

        public void Remove(T item)
        {
            if (!_selectedItems.Contains(item))
                return;

            _selectedItems.Remove(item);
            DeselectAction?.Invoke(item);
        }

        public void ToggleSelection(T item)
        {
            if (_selectedItems.Contains(item))
            {
                Remove(item);
            }
            else
            {
                Add(item);
            }
        }

        public bool Contains(T item)
        {
            return _selectedItems.Contains(item);
        }

        public void Clear()
        {
            while (_selectedItems.Any())
            {
                Remove(_selectedItems.First());
            }
        }

        public List<T> ToList()
        {
            return _selectedItems.ToList();
        }

        public Action<T> SelectAction { get; set; }

        public Action<T> DeselectAction { get; set; }

        public int Count => _selectedItems.Count;
        public T First => _selectedItems.First();
        public T FirstOrDefault(T defaultValue) => _selectedItems.Any() ? First : defaultValue;
    }
}
