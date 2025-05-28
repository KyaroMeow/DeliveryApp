using BestDelivery;
using System.Windows;

namespace WpfApp1
{
	public partial class AddPointWindow : Window
	{
		private readonly double x;
		private readonly double y;

		public Order CreatedOrder { get; private set; }

		public AddPointWindow(double x, double y)
		{
			InitializeComponent();
			this.x = x;
			this.y = y;
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			double priority = PrioritySlider.Value;
			CreatedOrder = new Order
			{
				ID = -999, // будет заменён позже
				Priority = priority,
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
