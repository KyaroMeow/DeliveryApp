using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BestDelivery;

namespace WpfApp1
{
        public static class GraphDrawer
        {
            private const double NodeSize = 10;
            private const double Scale = 5; // масштаб для визуализации

            public static void Draw(Canvas canvas, List<Order> orders, int[] route)
            {
                canvas.Children.Clear();

                // рисуем рёбра маршрута
                for (int i = 0; i < route.Length - 1; i++)
                {
                    Point from = GetPointById(orders, route[i]);
                    Point to = GetPointById(orders, route[i + 1]);

                    var line = new Line
                    {
                        X1 = from.X * Scale,
                        Y1 = from.Y * Scale,
                        X2 = to.X * Scale,
                        Y2 = to.Y * Scale,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    canvas.Children.Add(line);
                }

                // рисуем вершины
                foreach (var order in orders)
                {
                    var ellipse = new Ellipse
                    {
                        Width = NodeSize,
                        Height = NodeSize,
                        Fill = order.ID == -1 ? Brushes.Red : GetColorByPriority(order.Priority)
                    };
                    Canvas.SetLeft(ellipse, order.Destination.X * Scale - NodeSize / 2);
                    Canvas.SetTop(ellipse, order.Destination.Y * Scale - NodeSize / 2);
                    canvas.Children.Add(ellipse);

                    var label = new TextBlock
                    {
                        Text = order.ID.ToString(),
                        Foreground = Brushes.Black,
                        FontSize = 10
                    };
                    Canvas.SetLeft(label, order.Destination.X * Scale + 5);
                    Canvas.SetTop(label, order.Destination.Y * Scale - 5);
                    canvas.Children.Add(label);
                }
            }

            private static Point GetPointById(List<Order> orders, int id) =>
                orders.First(o => o.ID == id).Destination;

            private static Brush GetColorByPriority(double priority)
            {
                // чем выше приоритет, тем ближе к красному
                byte red = (byte)(255 * priority);
                byte green = (byte)(255 * (1 - priority));
                return new SolidColorBrush(Color.FromRgb(red, green, 0));
            }
        }
}
