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
        public static void UpdateWith<T>(this ObservableCollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (!source.Contains(item))
                    source.Add(item);
            }

            var itemsToRemove = source.Where(i => !items.Contains(i)).ToList();
            foreach (var item in itemsToRemove)
            {
                source.Remove(item);
            }
        }

        public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }

        public static void RemoveRange<T>(this ObservableCollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Remove(item);
            }
        }
    }
}
