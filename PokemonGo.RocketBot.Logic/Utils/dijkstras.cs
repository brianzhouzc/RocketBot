using System;
using System.Collections.Generic;

namespace PokemonGo.RocketBot.Logic.Utils
{
    internal class Graph
    {
        private readonly Dictionary<char, Dictionary<char, int>> vertices =
            new Dictionary<char, Dictionary<char, int>>();

        public void add_vertex(char name, Dictionary<char, int> edges)
        {
            vertices[name] = edges;
        }

        public List<char> shortest_path(char start, char finish)
        {
            var previous = new Dictionary<char, char>();
            var distances = new Dictionary<char, int>();
            var nodes = new List<char>();

            List<char> path = null;

            foreach (var vertex in vertices)
            {
                if (vertex.Key == start)
                {
                    distances[vertex.Key] = 0;
                }
                else
                {
                    distances[vertex.Key] = int.MaxValue;
                }

                nodes.Add(vertex.Key);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]);

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == finish)
                {
                    path = new List<char>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in vertices[smallest])
                {
                    var alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            return path;
        }
    }

    internal class MainClass
    {
        public static void Main(string[] args)
        {
            var g = new Graph();
            g.add_vertex('A', new Dictionary<char, int> {{'B', 7}, {'C', 8}});
            g.add_vertex('B', new Dictionary<char, int> {{'A', 7}, {'F', 2}});
            g.add_vertex('C', new Dictionary<char, int> {{'A', 8}, {'F', 6}, {'G', 4}});
            g.add_vertex('D', new Dictionary<char, int> {{'F', 8}});
            g.add_vertex('E', new Dictionary<char, int> {{'H', 1}});
            g.add_vertex('F', new Dictionary<char, int> {{'B', 2}, {'C', 6}, {'D', 8}, {'G', 9}, {'H', 3}});
            g.add_vertex('G', new Dictionary<char, int> {{'C', 4}, {'F', 9}});
            g.add_vertex('H', new Dictionary<char, int> {{'E', 1}, {'F', 3}});

            g.shortest_path('A', 'H').ForEach(x => Console.WriteLine(x));
        }
    }
}