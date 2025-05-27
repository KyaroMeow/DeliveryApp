using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1
{
    public static class ZoomPanHandler
    {
        private static Point _lastMousePosition;
        private static bool _isDragging;

        public static void Attach(Canvas canvas, ScaleTransform zoom, TranslateTransform pan)
        {
            canvas.MouseWheel += (s, e) =>
            {
                double zoomDelta = e.Delta > 0 ? 1.1 : 0.9;
                zoom.ScaleX *= zoomDelta;
                zoom.ScaleY *= zoomDelta;
            };

            canvas.MouseLeftButtonDown += (s, e) =>
            {
                _lastMousePosition = e.GetPosition(canvas);
                _isDragging = true;
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

                Point currentPosition = e.GetPosition(canvas);
                Vector delta = currentPosition - _lastMousePosition;

                pan.X += delta.X;
                pan.Y += delta.Y;

                _lastMousePosition = currentPosition;
            };
        }
    }
}
