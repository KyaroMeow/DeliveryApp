using BestDelivery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp8
{
    internal class Program
    {
        private const double PriorityWeight = 0.6;
        private const double DistanceWeight = 0.4;

        static void Main(string[] args)
        {
            Order[] orders = OrderArrays.GetOrderArray4();
            PrintOrders(orders);

            int[] route = FindOptimalRoute(orders);
            PrintRoute(route);

            if (RoutingTestLogic.TestRoutingSolution(
                orders.First(o => o.ID == -1).Destination,
                orders.Where(o => o.ID != -1).ToArray(),
                route,
                out double routeCost))
            {
                Console.WriteLine($"Стоимость маршрута: {routeCost}");
            }
        }

        private static void PrintOrders(IEnumerable<Order> orders)
        {
            foreach (Order order in orders)
            {
                Console.WriteLine($"id:{order.ID} Point: X: {order.Destination.X}; Y: {order.Destination.Y} Priority:{order.Priority}");
            }
        }

        private static void PrintRoute(IEnumerable<int> route)
        {
            Console.WriteLine("Оптимальный маршрут:");
            Console.WriteLine(string.Join(" -> ", route));
        }

        public static int[] FindOptimalRoute(Order[] orders)
        {
            // 1. Проверка и подготовка данных
            var warehouse = orders.FirstOrDefault(o => o.ID == -1);

            var deliveryOrders = orders.Where(o => o.ID != -1).ToList();
            if (deliveryOrders.Count == 0) return new[] { -1, -1 };

            // 2. Строим граф всех точек
            var allPoints = orders.Select(o => o.Destination).ToList();
            var graph = BuildGraph(allPoints);

            // 3. Находим оптимальный порядок посещения с учетом приоритетов
            var optimizedOrder = DijkstraBasedOptimization(warehouse.Destination, deliveryOrders, graph);

            // 4. Формируем итоговый маршрут
            var route = new List<int> { -1 };
            route.AddRange(optimizedOrder.Select(o => o.ID));
            route.Add(-1);

            return route.ToArray();
        }

        private static Dictionary<Point, Dictionary<Point, double>> BuildGraph(List<Point> points)
        {
            var graph = new Dictionary<Point, Dictionary<Point, double>>();

            foreach (var p1 in points)
            {
                graph[p1] = new Dictionary<Point, double>();
                foreach (var p2 in points.Where(p => !p.Equals(p1)))
                {
                    graph[p1][p2] = RoutingTestLogic.CalculateDistance(p1, p2);
                }
            }

            return graph;
        }

        private static List<Order> DijkstraBasedOptimization(Point depot, List<Order> orders,
            Dictionary<Point, Dictionary<Point, double>> graph)
        {
            var remainingOrders = new List<Order>(orders);
            var optimizedOrder = new List<Order>();
            Point current = depot;

            while (remainingOrders.Count > 0)
            {
                // Находим все кратчайшие пути от текущей точки
                var paths = DijkstraShortestPaths(graph, current);

                // Выбираем следующий заказ с учетом приоритета и расстояния
                var nextOrder = SelectNextOrder(remainingOrders, paths, current);

                optimizedOrder.Add(nextOrder);
                remainingOrders.Remove(nextOrder);
                current = nextOrder.Destination;
            }

            return optimizedOrder;
        }

        private static Dictionary<Point, double> DijkstraShortestPaths(
            Dictionary<Point, Dictionary<Point, double>> graph,
            Point start)
        {
            var distances = new Dictionary<Point, double>();
            var visited = new HashSet<Point>();
            var queue = new PriorityQueue<Point, double>();

            foreach (var vertex in graph.Keys)
                distances[vertex] = vertex.Equals(start) ? 0 : double.MaxValue;

            queue.Enqueue(start, 0);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (visited.Contains(current)) continue;
                visited.Add(current);

                foreach (var neighbor in graph[current])
                {
                    double alt = distances[current] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        queue.Enqueue(neighbor.Key, alt);
                    }
                }
            }

            return distances;
        }

        private static Order SelectNextOrder(List<Order> orders,
            Dictionary<Point, double> distances,
            Point currentPoint)
        {
            // Нормализуем приоритеты и расстояния
            var maxPriority = orders.Max(o => o.Priority);
            var minPriority = orders.Min(o => o.Priority);

            var maxDistance = orders.Max(o => distances[o.Destination]);
            var minDistance = orders.Min(o => distances[o.Destination]);

            // Вычисляем рейтинг для каждого заказа
            var ratedOrders = orders.Select(o => new
            {
                Order = o,
                Rating = PriorityWeight * Normalize(o.Priority, minPriority, maxPriority, false) +
                         DistanceWeight * Normalize(distances[o.Destination], minDistance, maxDistance, true)
            });

            return ratedOrders.OrderByDescending(x => x.Rating).First().Order;
        }

        private static double Normalize(double value, double min, double max, bool reverse)
        {
            if (Math.Abs(max - min) < 0.0001) return 0.5;
            var normalized = (value - min) / (max - min);
            return reverse ? 1 - normalized : normalized;
        }
    }
}