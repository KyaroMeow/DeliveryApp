using BestDelivery;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Order> currentOrders = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadOrders(Order[] orders)
        {
            currentOrders = new List<Order>(orders);
            int[] route = RouteHelper.FindOptimalRoute(orders);
            GraphDrawer.Draw(MapCanvas, currentOrders, route);

            var realOrders = orders.Where(o => o.ID != -1).ToArray();
            var depot = orders.First(o => o.ID == -1).Destination;
            if (RoutingTestLogic.TestRoutingSolution(depot, realOrders, route, out double cost))
            {
                MessageBox.Show($"Маршрут построен. Стоимость: {cost:0.00}", "Инфо", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Ошибка в маршруте." + String.Join(", ", route), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Array1_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray1());
        private void Array2_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray2());
        private void Array3_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray3());
        private void Array4_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray4());
        private void Array5_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray5());
        private void Array6_Click(object sender, RoutedEventArgs e) => LoadOrders(OrderArrays.GetOrderArray6());
        private void ToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            Sidebar.Visibility = Visibility.Visible;
        }
        private void CloseSidebar_Click(object sender, RoutedEventArgs e)
        {
            Sidebar.Visibility = Visibility.Collapsed;
        }

        //private void AddOrder_Click(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(InputX.Text, out int x) &&
        //        int.TryParse(InputY.Text, out int y) &&
        //        double.TryParse(InputPriority.Text, out double priority))
        //    {
        //        int newId = currentOrders.Count;
        //        currentOrders.Add(new Order
        //        {
        //            ID = newId,
        //            Destination = new Point { X = x, Y = y },
        //            Priority = priority
        //        });
        //        LoadOrders(currentOrders.ToArray());
        //    }
        //    else
        //    {
        //        MessageBox.Show("Неверные значения координат или приоритета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}
    }
}