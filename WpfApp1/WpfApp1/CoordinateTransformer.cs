using BestDelivery;
using GeoPoint = BestDelivery.Point;
using WpfPoint = System.Windows.Point;

namespace WpfApp1
{
    public class CoordinateTransformer
    {
        private double minX, maxX, minY, maxY;
        private double scaleX, scaleY;

        public CoordinateTransformer(Order[] orders, double targetWidth = 1000, double targetHeight = 1000)
        {
            minX = orders.Min(o => o.Destination.X);
            maxX = orders.Max(o => o.Destination.X);
            minY = orders.Min(o => o.Destination.Y);
            maxY = orders.Max(o => o.Destination.Y);

            double rangeX = maxX - minX;
            double rangeY = maxY - minY;

            scaleX = targetWidth / rangeX;
            scaleY = targetHeight / rangeY;
        }

        public WpfPoint Transform(GeoPoint geoPoint)
        {
            double x = (geoPoint.X - minX) * scaleX;
            double y = (geoPoint.Y - minY) * scaleY;

            return new WpfPoint(x, y);
        }
        public GeoPoint InverseTransform(WpfPoint wpfPoint)
        {
            double geoX = (wpfPoint.X / scaleX) + minX;
            double geoY = (wpfPoint.Y / scaleY) + minY;

            return new GeoPoint { X = geoX, Y = geoY };
        }
    }
}
