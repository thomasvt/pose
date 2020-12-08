using System.Collections.Generic;

namespace Pose.Domain.Editor
{
    public interface ISelection<T>
    {
        /// <summary>
        /// Replaces the current selection by just the given item.
        /// </summary>
        void SelectSingle(T itemToSelect);

        void Add(T item);
        void Clear();
        List<T> ToList();
        void Remove(T item);
        /// <summary>
        /// Adds to or removes from selection to toggle its selection state.
        /// </summary>
        void ToggleSelection(T item);

        bool Contains(T item);
        int Count { get; }
        T First { get; }
    }
}