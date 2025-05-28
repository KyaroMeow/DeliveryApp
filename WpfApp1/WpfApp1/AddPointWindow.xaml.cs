using BestDelivery;
using System.Windows;
using System.Windows.Controls;
using GeoPoint = BestDelivery.Point;
using WpfPoint = System.Windows.Point;

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
			// Устанавливаем начальное значение приоритета
			PrioritySlider.Value = 1.0;

			// Делаем окно модальным
			this.WindowStartupLocation = WindowStartupLocation.Manual;
			this.ShowInTaskbar = false;
			this.ResizeMode = ResizeMode.NoResize;
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			double priority = PrioritySlider.Value;
            int maxId = orders.Any() ? orders.Max(o => o.ID) : 0;

            // Создаем новый заказ с ID на 1 больше максимального
            CreatedOrder = new Order
            {
                ID = maxId + 1, // Новый ID
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
