using System;
using System.Collections.Generic;

namespace Pose.Framework
{
    public static class ListExtensions
    {
        /// <summary>
        /// Inserts the item at a position in the list so that the list remains sorted as defined by the compareFunc. Currently this is a naive algorithm that iterates all items in search of the correct position. Don't use this on performance critical subjects.
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

        /// <summary>
        /// Inserts the item at a position in the list so that the list remains sorted according to the string (case insensitive) returned by the selector. Currently this is a naive algorithm that iterates all items in search of the correct position. Don't use this on performance critical subjects.
        /// </summary>
        /// <param name="item">The item to insert in the list</param>
        /// <param name="selector">Must return the string to use for case insensitive ordering in the list.</param>
        public static void SortedInsertByString<T>(this IList<T> list, T item, Func<T, string> selector)
        {
            SortedInsert(list, item, (a, b) => string.Compare(selector(a), selector(b), StringComparison.OrdinalIgnoreCase));
        }
    }
}
