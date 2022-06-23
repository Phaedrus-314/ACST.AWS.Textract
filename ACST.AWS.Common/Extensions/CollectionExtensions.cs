namespace ACST.AWS.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// ICollection Extension Methods
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of this ICollection instance.
        /// </summary>
        public static void AddRange<T>(this ICollection<T> collection, params T[] items)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (items == null) throw new ArgumentNullException("items");
            for (int i = 0; i < items.Length; i++)
            {
                collection.Add(items[i]);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of this ICollection instance.
        /// </summary>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (items == null) throw new ArgumentNullException("items");
            foreach (T item in items)
            {
                collection.Add(item);
            }
        }
    }
}
