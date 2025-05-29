using System.Windows;
using BestDelivery;

namespace WpfApp1
{
    public partial class AddPointWindow : Window
    {
        private readonly double x;
        private readonly double y;
        private readonly Order[] orders;

        public Order CreatedOrder { get; private set; }

        public AddPointWindow(double x, double y, Order[] orders)
        {
            InitializeComponent();
            this.orders = orders;
            this.x = x;
            this.y = y;
            // base priority
            PrioritySlider.Value = 0.5;

            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.ShowInTaskbar = false;
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            double priority = PrioritySlider.Value;
            int maxId = orders.Any() ? orders.Max(o => o.ID) : 0;

            // Create order
            CreatedOrder = new Order
            {
                ID = maxId + 1,
                Priority = PrioritySlider.Value,
                Destination = new BestDelivery.Point
                {
                    X = x,
                    Y = y
                }
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
