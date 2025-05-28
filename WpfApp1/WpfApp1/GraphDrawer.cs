using BestDelivery;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


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
			}

			// Draw points
			foreach (var order in orders)
			{
				var p = _transformer.Transform(order.Destination);

				var ellipse = new Ellipse
				{
					Width = 10,
					Height = 10,
					Fill = order.ID == -1 ? Brushes.Red : GetPriorityBrush(order.Priority)
				};

				Canvas.SetLeft(ellipse, p.X - 5);
				Canvas.SetTop(ellipse, p.Y - 5);
				_canvas.Children.Add(ellipse);

				var label = new TextBlock
				{
					Text = order.ID.ToString(),
					FontSize = 10,
					Foreground = Brushes.Black
				};
				Canvas.SetLeft(label, p.X + 6);
				Canvas.SetTop(label, p.Y - 6);
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
