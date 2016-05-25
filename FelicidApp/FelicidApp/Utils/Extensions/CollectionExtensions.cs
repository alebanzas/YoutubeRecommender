using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FelicidApp.Utils.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Add several items to a collection of items
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="collection">The collection of items</param>
        /// <param name="items">The items to add to the collection</param>
        public static void AddAll<T>(this Collection<T> collection, IEnumerable<T> items) => 
            items.ForEach(item => collection.Add(item));

        /// <summary>
        /// Remove several items from a collection of items
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="collection">The collection of items</param>
        /// <param name="items">The items to remove from the collection</param>
        public static void RemoveAll<T>(this Collection<T> collection, IEnumerable<T> items) =>
            items.ForEach(item => collection.Remove(item));

        /// <summary>
        /// Compares a first set with a second set of items, and gives us the intersection by returning the items to 
        /// delete from and the items to add to the first set in order to contain the same items as the second set.
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="first">The first set of items</param>
        /// <param name="second">The second set of items</param>
        /// <param name="comparer">The way to determine if two items are equal</param>
        /// <returns>A tuple in the form (items to delete, items to add)</returns>
        public static Tuple<IEnumerable<T>, IEnumerable<T>> Differences<T>(
            this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
            => Tuple.Create(
                first.Except(second, comparer).ToList() as IEnumerable<T>, 
                second.Except(first, comparer).ToList() as IEnumerable<T>);

        /// <summary>
        /// Updates a collection of items by removing the exceding items and adding the missing items 
        /// from another set of items.
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="collection">The collection to update</param>
        /// <param name="items">The set of items to compare the collection to</param>
        /// <param name="comparer">The way to determine if two items are equal</param>
        public static void Update<T>(
            this Collection<T> collection, IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            var diff = collection.Differences(items, comparer);
            collection.RemoveAll(diff.Item1);
            collection.AddAll(diff.Item2);
        }

        /// <summary>
        /// Updates a collection of items by removing some items and adding others.
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="collection">The collection to update</param>
        /// <param name="diff">A tuple with the items to remove and the items to add</param>
        public static void Update<T>(
            this Collection<T> collection, Tuple<IEnumerable<T>, IEnumerable<T>> diff)
        {
            collection.RemoveAll(diff.Item1);
            collection.AddAll(diff.Item2);
        }

        /// <summary>
        /// Executes an action for each item in a collection of items.
        /// The action cannot modify the collection itself.
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="items">The list of items</param>
        /// <param name="action">The action to execute</param>
        /// <returns>The list of items</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
            return items;
        }

        /// <summary>
        /// Executes an action for each item of a list of items
        /// </summary>
        /// <typeparam name="T">The type of the items</typeparam>
        /// <param name="items">The list of items</param>
        /// <param name="actionAsync">The action to execute</param>
        /// <returns>The list of items</returns>
        public static async Task<IEnumerable<T>> ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> actionAsync)
        {
            foreach (var item in items)
            {
                await actionAsync(item);
            }

            return items;
        }
    }
}
