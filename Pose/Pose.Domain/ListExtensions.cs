using System;
using System.Collections.Generic;

namespace Pose.Domain
{
    public static class ListExtensions
    {
        /// <summary>
        /// Moves an item that's already in the list to another index. (with the index as it is before the move has started)
        /// </summary>
        public static void MoveSafe<T>(this IList<T> list, T item, int destinationIndex)
        {
            var currentIndex = list.IndexOf(item);
            if (currentIndex == -1)
                throw new Exception("The list doesn't contain that item.");

            if (destinationIndex == currentIndex || destinationIndex == currentIndex + 1)
                return;
            
            list.RemoveAt(currentIndex);
            list.Insert(currentIndex < destinationIndex ? destinationIndex - 1 : destinationIndex, item);
        }
    }
}
