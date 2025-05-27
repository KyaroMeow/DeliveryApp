using BestDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public static class RouteHelper
    {
        private const double PriorityWeight = 0.6;
        private const double DistanceWeight = 0.4;

        public static int[] FindOptimalRoute(Order[] orders)
        {
            var depot = orders.First(o => o.ID == -1);
            var deliveryOrders = orders.Where(o => o.ID != -1).ToList();
            if (deliveryOrders.Count == 0) return new[] { -1, -1 };

            var graph = BuildGraph(orders.Select(o => o.Destination).ToList());
            var optimized = OptimizeVisitOrder(depot.Destination, deliveryOrders, graph);

            var route = new List<int> { -1 };
            route.AddRange(optimized.Select(o => o.ID));
            route.Add(-1);

            return route.ToArray();
        }

        private static Dictionary<Point, Dictionary<Point, double>> BuildGraph(List<Point> points)
        {
            return points.ToDictionary(
                p1 => p1,
                p1 => points.Where(p2 => !p2.Equals(p1))
                             .ToDictionary(p2 => p2, p2 => RoutingTestLogic.CalculateDistance(p1, p2))
            );
        }

        private static List<Order> OptimizeVisitOrder(Point start, List<Order> orders,
            Dictionary<Point, Dictionary<Point, double>> graph)
        {
            var remaining = new List<Order>(orders);
            var result = new List<Order>();
            var current = start;

            while (remaining.Any())
            {
                var distances = Dijkstra(graph, current);
                var next = SelectNext(remaining, distances);

                result.Add(next);
                remaining.Remove(next);
                current = next.Destination;
            }

            return result;
        }

        private static Dictionary<Point, double> Dijkstra(Dictionary<Point, Dictionary<Point, double>> graph, Point start)
        {
            var dist = graph.Keys.ToDictionary(p => p, _ => double.MaxValue);
            dist[start] = 0;
            var visited = new HashSet<Point>();
            var queue = new PriorityQueue<Point, double>();
            queue.Enqueue(start, 0);

            while (queue.TryDequeue(out var current, out _))
            {
                if (!visited.Add(current)) continue;

                foreach (var (neighbor, cost) in graph[current])
                {
                    double alt = dist[current] + cost;
                    if (alt < dist[neighbor])
                    {
                        dist[neighbor] = alt;
                        queue.Enqueue(neighbor, alt);
                    }
                }
            }

            return dist;
        }

        private static Order SelectNext(List<Order> orders, Dictionary<Point, double> distances)
        {
            double maxP = orders.Max(o => o.Priority);
            double minP = orders.Min(o => o.Priority);
            double maxD = orders.Max(o => distances[o.Destination]);
            double minD = orders.Min(o => distances[o.Destination]);

            return orders.Select(o => new
            {
                Order = o,
                Score = PriorityWeight * Normalize(o.Priority, minP, maxP, false)
                      + DistanceWeight * Normalize(distances[o.Destination], minD, maxD, true)
            }).OrderByDescending(x => x.Score).First().Order;
        }

        private static double Normalize(double value, double min, double max, bool reverse)
        {
            if (Math.Abs(max - min) < 0.0001) return 0.5;
            var norm = (value - min) / (max - min);
            return reverse ? 1 - norm : norm;
        }
    }
}
