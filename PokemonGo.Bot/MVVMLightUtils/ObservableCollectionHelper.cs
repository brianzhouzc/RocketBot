using PokemonGo.Bot.MVVMLightUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaSoft.MvvmLight.Command
{
    public static class ObservableCollectionHelper
    {
        public static void UpdateWith<T>(this IList<T> source, IEnumerable<T> items)
            where T : IUpdateable<T>
        {
            //source.Clear();
            //source.AddRange(items);

            var itemsToRemove = source.Where(i => !items.Contains(i)).ToList();
            foreach (var item in itemsToRemove)
            {
                // remove old items
                source.Remove(item);
            }

            foreach (var item in items)
            {
                var indexInSource = source.IndexOf(item);

                // add new items
                if (indexInSource < 0)
                    source.Add(item);

                // update existing items
                else
                    source[indexInSource].UpdateWith(item);
            }
        }

        public static void AddOrUpdate<T>(this IList<T> source, T item)
            where T : IUpdateable<T>
        {
            var indexInSource = source.IndexOf(item);

            // add new items
            if (indexInSource < 0)
                source.Add(item);

            // update existing items
            else
                source[indexInSource].UpdateWith(item);
        }

        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }

        public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Remove(item);
            }
        }
    }
}
