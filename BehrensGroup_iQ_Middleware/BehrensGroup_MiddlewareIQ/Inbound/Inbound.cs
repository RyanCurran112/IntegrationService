using System;
using System.Collections.Generic;
using System.Text;

namespace BehrensGroup_MiddlewareIQ.Inbound
{
    class Inbound
    {
        public static void InboundMain()
        {

            //Couriers.DeliveryContactDefaultDeliveryAgents.DeliveryContactDefaultDeliveryAgentsMain();

            SalesOrders.SalesOrders.SalesOrderMain();
            Couriers.Couriers.CouriersMain();

            //Objects.ProductAssemblies.ProductAssembliesMain();
            Objects.StockBinTransferRequests.StockBinTransferRequestsMain();

            Console.WriteLine("Inbound Complete");
        }
    }
}