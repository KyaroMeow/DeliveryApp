using BestDelivery;
using System.Linq;

namespace ConsoleApp8
{
    internal class Program
    {
        private const double PriorityWeight = 0.6; // Вес приоритета в оценке
        private const double DistanceWeight = 0.4; // Вес расстояния в оценке
        static void Main(string[] args)
        {
            Order[] orders = OrderArrays.GetOrderArray5();
            foreach (Order order in orders)
            {
                Console.WriteLine("id:" + order.ID + "Point:" + order.Destination.X + " , " + order.Destination.Y + "Priority:" + order.Priority);
            }

          
            foreach (int id in FindOptimalRoute(orders))
            {
                Console.Write(id+", ");
            }
            double routeCost;
            if (RoutingTestLogic.TestRoutingSolution(orders.First(order => order.ID == -1).Destination, orders.Where(order => order.ID != -1).ToArray(), FindOptimalRoute(orders), out routeCost))
            {
                Console.WriteLine($"Стоимость маршрута: {routeCost}");
            }

        }

        public static int[] FindOptimalRoute(Order[] orders)
        {
            // 1. Проверка наличия склада
            var warehouse = orders.FirstOrDefault(o => o.ID == -1);

            // 2. Отделяем заказы от склада
            var deliveryOrders = orders.Where(o => o.ID != -1).ToList();

            // 3. Создаем начальный маршрут: склад -> ... -> склад
            var route = new List<int> { -1 };

            // 4. Добавляем ВСЕ заказы в маршрут (пока без оптимизации)
            route.AddRange(deliveryOrders.Select(o => o.ID));

            // 5. Завершаем маршрут возвратом на склад
            route.Add(-1);

            // 6. Оптимизируем порядок заказов
            OptimizeRoute(route, orders, warehouse.Destination);

            return route.ToArray();
        }

        private static void OptimizeRoute(List<int> route, Order[] orders, Point depot)
        {
            // Оптимизируем только часть маршрута между складами
            var ordersPart = route.GetRange(1, route.Count - 2).ToList();

            // Создаем словарь для быстрого поиска ID по координатам
            var pointToIdMap = ordersPart
                .ToDictionary(
                    id => orders.First(o => o.ID == id).Destination,
                    id => id,
                    new PointComparer()); // Используем специальный компаратор

            // Добавляем точки для оптимизации (без складов)
            var pointsToOptimize = ordersPart
                .Select(id => orders.First(o => o.ID == id).Destination)
                .ToList();

            // Оптимизируем порядок точек
            var optimizedPoints = OptimizePointOrder(depot, pointsToOptimize);

            // Обновляем маршрут
            for (int i = 0; i < ordersPart.Count; i++)
            {
                route[i + 1] = pointToIdMap[optimizedPoints[i]];
            }
        }

        // Специальный компаратор для сравнения точек
        private class PointComparer : IEqualityComparer<Point>
        {
            private const double Tolerance = 0.00001;

            public bool Equals(Point a, Point b)
            {
                return Math.Abs(a.X - b.X) < Tolerance &&
                       Math.Abs(a.Y - b.Y) < Tolerance;
            }

            public int GetHashCode(Point obj)
            {
                return obj.X.GetHashCode() ^ obj.Y.GetHashCode();
            }
        }

        private static List<Point> OptimizePointOrder(Point depot, List<Point> points)
        {
            // Реализация алгоритма оптимизации (например, ближайший сосед)
            var result = new List<Point>();
            var unvisited = new HashSet<Point>(points);
            Point current = depot;

            while (unvisited.Count > 0)
            {
                var next = unvisited
                    .OrderBy(p => RoutingTestLogic.CalculateDistance(current, p))
                    .First();

                result.Add(next);
                unvisited.Remove(next);
                current = next;
            }

            return result;
        }

    }

}
