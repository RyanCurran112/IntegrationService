using System;
using System.Collections.Generic;
using System.Text;

namespace BehrensGroup_MiddlewareIQ.Outbound
{
    class Outbound
    {
        public static void OutboundMain()
        {
            StockFeeds.DipAndDozeStockFeed.StockFeedMain();
            StockFeeds.LaBeebyStockFeed.StockFeedMain();
            StockFeeds.Distributor2StockFeed.StockFeedMain();
            StockFeeds.PhoenixWorldWideStockFeed.StockFeedMain();
            Console.WriteLine("Outbound - Stock Feeds Complete");
            
            Invoices.Foodbuy.UploadInvoice();
            //Console.WriteLine("Outbound - Invoices Complete");
            
            //Couriers.RoyalMail.MoveRoyalMailFile();    
            //Couriers.DPD.MoveDPDFile();

            DeliveryNotes.DipAndDozeDeliveryNotes.MoveDipAndDozeDeliveryNoteFile();
            Console.WriteLine("Outbound - Delivery Notes Complete");

            Console.WriteLine("Outbound Complete");
        }
    }
}
