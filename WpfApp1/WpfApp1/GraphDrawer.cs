using BestDelivery;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GeoPoint = BestDelivery.Point;
using WpfPoint = System.Windows.Point;

namespace WpfApp1
{
	public class GraphDrawer
	{
		private readonly Canvas _canvas;
		private readonly CoordinateTransformer _transformer;

		public GraphDrawer(Canvas targetCanvas, Order[] orders)
		{
			_canvas = targetCanvas;
			_transformer = new CoordinateTransformer(orders);
		}

		public void DrawGraph(Order[] orders, int[] route)
		{
			_canvas.Children.Clear();

			var orderDict = orders.ToDictionary(o => o.ID, o => o);

			// Draw route lines
			for (int i = 0; i < route.Length - 1; i++)
			{
				var from = _transformer.Transform(orderDict[route[i]].Destination);
				var to = _transformer.Transform(orderDict[route[i + 1]].Destination);

				var line = new Line
				{
					X1 = from.X,
					Y1 = from.Y,
					X2 = to.X,
					Y2 = to.Y,
					Stroke = Brushes.Gray,
					StrokeThickness = 2
				};
				_canvas.Children.Add(line);

				// 1. Стрелка направления
				double angle = Math.Atan2(to.Y - from.Y, to.X - from.X);
				double angleDegrees = angle * 180 / Math.PI;

				double arrowLength = 18;
				double arrowWidth = 9;

				// Точка смещения — отступ назад по маршруту
				double offsetX = to.X - arrowLength * Math.Cos(angle);
				double offsetY = to.Y - arrowLength * Math.Sin(angle);

				// Вершина стрелки (нос)
				var tip = new WpfPoint(to.X, to.Y);

				// Основание стрелки (две стороны)
				var baseLeft = new WpfPoint(
					offsetX + arrowWidth * Math.Sin(angle),
					offsetY - arrowWidth * Math.Cos(angle)
				);

				var baseRight = new WpfPoint(
					offsetX - arrowWidth * Math.Sin(angle),
					offsetY + arrowWidth * Math.Cos(angle)
				);

				var arrowHead = new Polygon
				{
					Points = new PointCollection { tip, baseLeft, baseRight },
					Fill = Brushes.Black
				};
				_canvas.Children.Add(arrowHead);

				// 2. Подпись веса ребра
				var midX = (from.X + to.X) / 2;
				var midY = (from.Y + to.Y) / 2;

				double weight = RoutingTestLogic.CalculateDistance(
					orderDict[route[i]].Destination,
					orderDict[route[i + 1]].Destination
				);

				var label = new TextBlock
				{
					Text = $"{weight:F4}",
					FontSize = 18,
					Foreground = Brushes.DarkSlateGray
				};

				Canvas.SetLeft(label, midX);
				Canvas.SetTop(label, midY);
				_canvas.Children.Add(label);
			}

			// Draw points
			foreach (var order in orders)
			{
				var p = _transformer.Transform(order.Destination);

				var ellipse = new Ellipse
				{
					Width = 15,
					Height = 15,
					Fill = order.ID == -1 ? Brushes.Red : GetPriorityBrush(order.Priority)
				};

				Canvas.SetLeft(ellipse, p.X - 5);
				Canvas.SetTop(ellipse, p.Y - 5);
				_canvas.Children.Add(ellipse);

				var label = new TextBlock
				{
					Text = order.ID == -1 ? "Склад" : order.ID.ToString(),
					FontSize = 23,
					Foreground = Brushes.Black,
					FontWeight = FontWeights.SemiBold
				};
				Canvas.SetLeft(label, p.X + 25);
				Canvas.SetTop(label, p.Y - 25);
				_canvas.Children.Add(label);
			}
		}

		private Brush GetPriorityBrush(double priority)
		{
			byte r = (byte)(priority * 255);
			byte g = (byte)((1 - priority) * 255);
			return new SolidColorBrush(Color.FromRgb(r, g, 0));
		}
	}
}
