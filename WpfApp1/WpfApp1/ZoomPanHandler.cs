using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WpfApp1
{
    public static class ZoomPanHandler
    {
		private static Point _lastMousePosition;
		private static bool _isDragging;
		private static DispatcherTimer _inertiaTimer;
		private static Vector _velocity;
		public static void Center(Canvas canvas, TranslateTransform pan, ScaleTransform zoom)
		{
			// Центрируем граф в родительском ScrollViewer
			double centerX = (canvas.RenderSize.Width * zoom.ScaleX) / 2;
			double centerY = (canvas.RenderSize.Height * zoom.ScaleY) / 2;

			pan.X = -centerX + 600; // можно подогнать под ширину окна
			pan.Y = -centerY + 350;
		}
		private static void AnimateScale(ScaleTransform scale, double from, double to, int durationMs, bool isY = false)
		{
			var animation = new DoubleAnimation
			{
				From = from,
				To = to,
				Duration = TimeSpan.FromMilliseconds(durationMs),
				EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
			};

			if (isY)
				scale.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
			else
				scale.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
		}
		public static void Attach(Canvas canvas, ScaleTransform zoom, TranslateTransform pan)
		{
			_inertiaTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(16)
			};

			_inertiaTimer.Tick += (sender, e) =>
			{
				pan.X += _velocity.X;
				pan.Y += _velocity.Y;

				_velocity *= 0.9;

				if (_velocity.Length < 0.05)
					_inertiaTimer.Stop();
			};
			canvas.RenderTransformOrigin = new Point(0.5, 0.5);

			canvas.MouseWheel += (s, e) =>
			{
				double zoomDelta = e.Delta > 0 ? 1.1 : 0.9;

				// Получаем позицию мыши относительно графа
				var mousePos = e.GetPosition(canvas);

				// Считаем смещение от центра зума
				double absX = mousePos.X * zoom.ScaleX + pan.X;
				double absY = mousePos.Y * zoom.ScaleY + pan.Y;

				// Применяем зум
				AnimateScale(zoom, zoom.ScaleX, zoom.ScaleX * zoomDelta, 100);
				AnimateScale(zoom, zoom.ScaleY, zoom.ScaleY * zoomDelta, 100, isY: true);

				// Компенсируем сдвиг так, чтобы точка под мышкой осталась на месте
				pan.X = absX - mousePos.X * zoom.ScaleX;
				pan.Y = absY - mousePos.Y * zoom.ScaleY;
			};

			canvas.MouseLeftButtonDown += (s, e) =>
			{
				_lastMousePosition = e.GetPosition(canvas.Parent as IInputElement);
				_isDragging = true;
				_velocity = new Vector(0, 0);
				_inertiaTimer.Stop();
				canvas.CaptureMouse();
			};

			canvas.MouseLeftButtonUp += (s, e) =>
			{
				_isDragging = false;
				canvas.ReleaseMouseCapture();
			};

			canvas.MouseMove += (s, e) =>
			{
				if (!_isDragging) return;

				Point currentPosition = e.GetPosition(canvas.Parent as IInputElement);

				Vector delta = currentPosition - _lastMousePosition;
				_lastMousePosition = currentPosition;

				// Не умножай и не дели — просто двигай, если координаты стабильны
				pan.X += delta.X;
				pan.Y += delta.Y;

				_velocity = delta;
			};

			// ПКМ — сброс панорамы
			

			// Двойной клик — сброс зума
			//canvas.MouseDoubleClick += (s, e) =>
			//{
			//	zoom.ScaleX = 1;
			//	zoom.ScaleY = 1;
			//};
		}

		private static double Clamp(double value, double min, double max)
		{
			return value < min ? min : value > max ? max : value;
		}
	}
}
