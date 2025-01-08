using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehrensGroup_MiddlewareIQ.Inbound.SalesOrders
{
    public class SalesOrders
    {
        public static void SalesOrderMain()
        {
            //HeatonsEDIOrders.HeatonsOrdersMain();

            NetEDIOrders.NetEDIOrdersMain();
            FoodbuyEDIOrders.FoodbuyOrdersMain();
            ScottsWebOrders.ScottsOrdersMain();
            DipAndDozeWebOrders.DipAndDozeOrdersMain();
            LaBeebyWebOrders.LaBeebyOrdersMain();
            
            CorporateTrendsWebOrders.CorporateTrendsOrdersMain();

            Console.WriteLine("Inbound - Sales Orders Complete");
        }
    }
}
