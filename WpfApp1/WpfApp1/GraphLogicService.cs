using BestDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class GraphLogicService
    {
        public Order[] Orders { get; private set; }
        public int[] Route { get; private set; }
        public double RouteCost { get; private set; }

        public GraphLogicService(Order[] orders)
        {
            Orders = orders;
            CalculateRoute();
        }

        public void AddOrder(Order newOrder)
        {
            var orderList = new List<Order>(Orders);
            orderList.Add(newOrder);
            Orders = orderList.ToArray();
            CalculateRoute();
        }

        private void CalculateRoute()
        {
            Route = RouteHelper.FindOptimalRoute(Orders);

            var depot = GetDepot();
            if (RoutingTestLogic.TestRoutingSolution(depot.Destination, Orders, Route, out double cost))
            {
                RouteCost = cost;
            }
            else
            {
                RouteCost = -1;
            }
        }

        private Order GetDepot() => Array.Find(Orders, o => o.ID == -1);
    }
}
