using System.Windows;
using System.Windows.Input;
using BestDelivery;
using GeoPoint = BestDelivery.Point;
using WpfPoint = System.Windows.Point;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private List<Order> currentOrders = new List<Order>();
        private Order[] currentOrdersMass;
        public MainWindow()
        {
            InitializeComponent();

            ZoomPanHandler.Attach(GraphContainer, ZoomTransform, PanTransform);
        }

        private void LoadOrders(Order[] orders)
        {
            currentOrdersMass = orders;
            currentOrders = orders.ToList();

            GraphContainer.Children.Clear();

            int[] route = RouteHelper.FindOptimalRoute(orders);
            var drawer = new GraphDrawer(GraphContainer, orders);
            drawer.DrawGraph(orders, route);

            var realOrders = orders.Where(o => o.ID != -1).ToArray();
            var depot = orders.First(o => o.ID == -1).Destination;
            if (RoutingTestLogic.TestRoutingSolution(depot, realOrders, route, out double cost))
            {

                RouteCostText.Text = $"{cost:0.00}";
            }
            else
            {
                MessageBox.Show("Ошибка в маршруте." + String.Join(", ", route), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            OrdersList.Items.Clear();
            foreach (var o in orders.Where(o => o.ID != -1))
            {
                OrdersList.Items.Add($"ID: {o.ID} | X:{o.Destination.X:F2}, Y:{o.Destination.Y:F2} | Приоритет: {o.Priority:F2}");
            }
        }

        private void Array1_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray1());
        private void Array2_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray2());
        private void Array3_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray3());
        private void Array4_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray4());
        private void Array5_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray5());
        private void Array6_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray6());

        private void GraphContainer_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (currentOrders != null)
            {
                WpfPoint clickPos = e.GetPosition(GraphContainer);
                CoordinateTransformer trasformer = new CoordinateTransformer(currentOrdersMass);
                GeoPoint geoPos = trasformer.InverseTransform(clickPos);

                var dialog = new AddPointWindow(geoPos.X, geoPos.Y, currentOrdersMass);
                dialog.Owner = this;

                // Converting coordinates
                var pointOnScreen = GraphContainer.PointToScreen(clickPos);
                dialog.Left = pointOnScreen.X - dialog.Width / 2;
                dialog.Top = pointOnScreen.Y - dialog.Height;

                if (dialog.ShowDialog() == true)
                {
                    currentOrders.Add(dialog.CreatedOrder);
                    LoadOrders(currentOrders.ToArray());
                }

                e.Handled = true;
            }
        }

        private void ToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            Sidebar.Visibility = Visibility.Visible;
        }
        private void CloseSidebar_Click(object sender, RoutedEventArgs e)
        {
            Sidebar.Visibility = Visibility.Collapsed;
        }
    }
}