using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGo.RocketAPI.Window
{
    /* Copied from here, with modifications to make more generic
     * http://stackoverflow.com/questions/2927469/traveling-salesman-problem-2-opt-algorithm-c-sharp-implementation
     There's probably a much better implementation of TSP's solution */
    public static class TSP<T> 
    {
        private static Func<T, T, double> distance;
        public static IEnumerable<T> getMinimumTour(IEnumerable<T> nodes, Func<T,T,double> distance)
        {
            TSP<T>.distance = distance;
            //create an initial tour out of nearest neighbors
            var stops = nodes
                        .Select(i => new Stop(i))
                        .NearestNeighbors()
                        .ToList();

            //create next pointers between them
            stops.Connect(true);

            //wrap in a tour object
            Tour startingTour = new Tour(stops);

            //the actual algorithm
            while (true)
            {
                var newTour = startingTour.GenerateMutations()
                                            .MinBy(tour => tour.Cost());
                if (newTour.Cost() < startingTour.Cost()) startingTour = newTour;
                else break;
            }
            return startingTour.Cycle().Select(s => s.City);
        }

        internal class Stop
        {
            public Stop(T city)
            {
                City = city;
            }


            public Stop Next { get; set; }

            public T City { get; set; }


            public Stop Clone()
            {
                return new Stop(City);
            }


            public static double Distance(Stop first, Stop other)
            {
                return TSP<T>.distance(first.City, other.City);
            }


            //list of nodes, including this one, that we can get to
            public IEnumerable<Stop> CanGetTo()
            {
                var current = this;
                while (true)
                {
                    yield return current;
                    current = current.Next;
                    if (current == this) break;
                }
            }


            public override bool Equals(object obj)
            {
                return EqualityComparer<T>.Default.Equals(City,((Stop)obj).City);
            }


            public override int GetHashCode()
            {
                return City.GetHashCode();
            }

        }


        internal class Tour
        {
            public Tour(IEnumerable<Stop> stops)
            {
                Anchor = stops.First();
            }


            //the set of tours we can make with 2-opt out of this one
            public IEnumerable<Tour> GenerateMutations()
            {
                for (Stop stop = Anchor; stop.Next != Anchor; stop = stop.Next)
                {
                    //skip the next one, since you can't swap with that
                    Stop current = stop.Next.Next;
                    while (current != Anchor)
                    {
                        yield return CloneWithSwap(stop.City, current.City);
                        current = current.Next;
                    }
                }
            }


            public Stop Anchor { get; set; }


            public Tour CloneWithSwap(T firstCity, T secondCity)
            {
                Stop firstFrom = null, secondFrom = null;
                var stops = UnconnectedClones();
                stops.Connect(true);

                foreach (Stop stop in stops)
                {
                    if (EqualityComparer<T>.Default.Equals(stop.City, firstCity)) firstFrom = stop;

                    if (EqualityComparer<T>.Default.Equals(stop.City, secondCity)) secondFrom = stop;
                }

                //the swap part
                var firstTo = firstFrom.Next;
                var secondTo = secondFrom.Next;

                //reverse all of the links between the swaps
                firstTo.CanGetTo()
                        .TakeWhile(stop => stop != secondTo)
                        .Reverse()
                        .Connect(false);

                firstTo.Next = secondTo;
                firstFrom.Next = secondFrom;

                var tour = new Tour(stops);
                return tour;
            }


            public IList<Stop> UnconnectedClones()
            {
                return Cycle().Select(stop => stop.Clone()).ToList();
            }


            public double Cost()
            {
                return Cycle().Aggregate(
                    0.0,
                    (sum, stop) =>
                    sum + Stop.Distance(stop, stop.Next));
            }


            public IEnumerable<Stop> Cycle()
            {
                return Anchor.CanGetTo();
            }


            public override string ToString()
            {
                string path = String.Join(
                    "->",
                    Cycle().Select(stop => stop.ToString()).ToArray());
                return String.Format("Cost: {0}, Path:{1}", Cost(), path);
            }

        }
    }
    public static class ExtensionMethods
    {

        //take an ordered list of nodes and set their next properties
        internal static void Connect<T>(this IEnumerable<TSP<T>.Stop> stops, bool loop)
        
        {
            TSP<T>.Stop prev = null, first = null;
            foreach (var stop in stops)
            {
                if (first == null) first = stop;
                if (prev != null) prev.Next = stop;
                prev = stop;
            }

            if (loop)
            {
                prev.Next = first;
            }
        }


        //T with the smallest func(T)
        internal static T MinBy<T, TComparable> (
            this IEnumerable<T> xs,
            Func<T, TComparable> func) 
            where TComparable : IComparable<TComparable>
        {
            return xs.DefaultIfEmpty().Aggregate(
                (maxSoFar, elem) =>
                func(elem).CompareTo(func(maxSoFar)) > 0 ? maxSoFar : elem);
        }


        //return an ordered nearest neighbor set
        internal static IEnumerable<TSP<T>.Stop> NearestNeighbors<T>(this IEnumerable<TSP<T>.Stop> stops)
        {
            var stopsLeft = stops.ToList();
            for (var stop = stopsLeft.First();
                    stop != null;
                    stop = stopsLeft.MinBy(s => TSP<T>.Stop.Distance(stop, s)))
            {
                stopsLeft.Remove(stop);
                yield return stop;
            }
        }
    }
}
