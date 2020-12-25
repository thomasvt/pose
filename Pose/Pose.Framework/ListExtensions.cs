using System;
using System.Collections.Generic;

namespace Pose.Framework
{
    public static class ListExtensions
    {
        /// <summary>
        /// Simple insertion at the correct position in the list according to the order defined by the compareFunc. Currently this is a naive algorithm that iterated the items in search of the correct position. Don't use this on performance critical subjects.
        /// </summary>
        /// <param name="row">The item to insert</param>
        /// <param name="compareFunc">Should return -1, 0 or 1 to indicate the first argument is less, equal or greater (respectively) than the second argument.</param>
        public static void SortedInsert<T>(this IList<T> list, T item, Func<T, T, int> compareFunc)
        {
            var i = 0;
            while (i < list.Count && compareFunc(list[i], item) < 0)
                i++;

            list.Insert(i, item);
        }
    }
}
