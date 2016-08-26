using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace PokemonGo.WPF.Behaviors
{
    public static class CollectionViewSourceBehavior
    {
        public static readonly DependencyProperty SortPropertyProperty =
            DependencyProperty.RegisterAttached(
                "SortProperty",
                typeof(string),
                typeof(CollectionViewSourceBehavior),
                new UIPropertyMetadata("", OnSortPropertyChanged));

        public static object GetSortProperty(DependencyObject element)
        {
            return element.GetValue(SortPropertyProperty);
        }

        public static void SetSortProperty(DependencyObject element, object value)
        {
            element.SetValue(SortPropertyProperty, value);
        }

        public static void OnSortPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var collectionViewSource = dependencyObject as CollectionViewSource;
                if (collectionViewSource == null)
                {
                    return;
                }

                var SortProperty = e.NewValue.ToString();
                var newSortDescription = new SortDescription
                {
                    PropertyName = SortProperty
                };
                collectionViewSource.SortDescriptions.Clear();
                collectionViewSource.SortDescriptions.Add(newSortDescription);
            }
        }
    }
}