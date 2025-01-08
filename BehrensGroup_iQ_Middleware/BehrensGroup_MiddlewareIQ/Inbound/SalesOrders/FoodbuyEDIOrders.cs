/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for reading & uploading Foodbuy EDI Orders
 *              Orders are retrieved from FTP as CSV file, these are then moved, read & posted to iQ using respective API endpoints.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.SalesOrders
{
    class FoodbuyEDIOrders
    {

        public static string EDIFileName { get; set; }
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\DC01\Company\Intact iQ\BehrensFTP\SalesOrders\Foodbuy";
        public static string path = @"\\APP01\SalesOrders\Foodbuy\Orders";        //ProductionPath

        public static void FoodbuyOrdersMain()
        {
            DownloadSchemaFoodbuy download = new DownloadSchemaFoodbuy();

            try
            {
                try
                {
                    FTPFunctions.DownloadFile();
                }
                catch (Exception e) 
                {
                    //Write to Log File
                    string message = "Foodbuy-Cannot read order - " + CustomerOrderNumber;
                    Console.WriteLine(e.Message);
                    LogFile.WritetoLogFile(message, false);
                }

               

                string[] fileEntries = Directory.GetFiles(path);

                foreach (string fileName in fileEntries)
                {
                    string filePath = fileName;
                    EDIFileName = Path.GetFileName(fileName).Replace(".csv", "");

                    string newpath = path + "\\" + EDIFileName;

                    using (CsvReader reader = new CsvReader(filePath))
                    {

                        string LastCustomerOrderNo = "";
                        string CurrentCustomerOrderNo = "";

                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            CurrentCustomerOrderNo = values[0];

                            if (headStart.Length > 0)
                            {
                                if (CurrentCustomerOrderNo != LastCustomerOrderNo && CurrentCustomerOrderNo != "Orderref")
                                {
                                    Customer_Contact customer_DeliveryContact = new Customer_Contact();
                                    SalesOrder salesOrder = new SalesOrder();

                                    //Check if Sales Order already exists
                                    SalesOrder existingSalesOrder = new SalesOrder();
                                    List<SalesOrder> existingSalesOrders = existingSalesOrder.GetSalesOrders("[eq]", "AlternateReference", CurrentCustomerOrderNo);

                                    if (existingSalesOrders.Count == 0)
                                    {
                                        
                                        salesOrder = download.FoodbuySalesOrder(values, customer_DeliveryContact.ID, CurrentCustomerOrderNo);

                                        salesOrder.PricingType = "NetPricing";
                                        salesOrder.DeliveryPricingType = "NetPricing";

                                        using (CsvReader reader2 = new CsvReader(filePath))
                                        {
                                            foreach (string[] values2 in reader2.RowEnumerator)
                                            {
                                                string headStart2 = values2[0];
                                                string CurrentCustomerOrderNo2 = values2[0];

                                                if (headStart.Length > 0 && CurrentCustomerOrderNo == CurrentCustomerOrderNo2)
                                                {
                                                    SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
                                                    salesOrder_Line = download.FoodbuySalesOrder_Line(values2, CurrentCustomerOrderNo);

                                                    if (salesOrder_Line != null)
                                                    {
                                                        salesOrder.SalesOrderLines.Add(salesOrder_Line);
                                                    }
                                                }
                                            }
                                        }
                                        salesOrder.CreateSalesOrder();
                                        //salesOrder.UpdateSalesOrder();

                                        string OrderMessage = "Foodbuy-Successfully added order - " + salesOrder.AlternateReference + "";
                                        LogFile.WritetoLogFile(OrderMessage, true);
                                    }
                                }
                                else
                                {
                                    string OrderMessage = "Foodbuy-Order " + CurrentCustomerOrderNo + " already exists within iQ";
                                    LogFile.WritetoLogFile(OrderMessage, false);
                                }
                                LastCustomerOrderNo = CurrentCustomerOrderNo;
                            }
                        }
                    }

                    string CurrentDateTime = DateTime.Now.ToString();
                    CurrentDateTime = CurrentDateTime.Replace("/", "-");
                    CurrentDateTime = CurrentDateTime.Replace(":", "-");

                    File.Move(filePath, path + @"\Complete\" + EDIFileName + ".csv");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "Foodbuy-Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
            Console.WriteLine("Inbound - Sales Orders - Foodbuy Orders Complete");
        }
    }

    class DownloadSchemaFoodbuy : GenericClassTransactionLine
    {
        public string AlternateReference { get; set; }              //Customer Reference (Unique Per Order Web Generated No) 
        public string PromoCode { get; set; }
        public string AdditionalDeliveryNotes { get; set; }
        public string DeliveryService { get; set; }
        public string DeliveryPricingType { get; set; }
        public string DeliveryPrice { get; set; }                   //Comes from Net Price on Order Line
        public string GrossPrice { get; set; }
        public string DiscountAmount { get; set; }
        public string DiscountPercentage { get; set; }              //Discount Percentage Applied - Not being imported
      
        public SalesOrder FoodbuySalesOrder(string[] DownloadInfo, long Customer_DeliveryContactID, string CustomerOrderNumber)
        {
            SalesOrder salesOrder = new SalesOrder();
            Customer customer = new Customer();

            salesOrder.Branch.ID = Branch.Branch_HQ_ID;
            salesOrder.DespatchBranch.ID = Branch.Branch_HQ_ID;
            salesOrder.D_Division.ID = Division.Division_FPC_ID;
            salesOrder.OrderType.ID = SalesOrderType.OrderType_EDI_ID;
            salesOrder.Source.ID = SalesOrderSource.OrderSource_FEDI_ID;
            salesOrder.Currency.ID = Currency.Currency_GBP_ID;

            salesOrder.DeliveryContact.ID = Customer_DeliveryContactID;

            try
            {
                salesOrder.AlternateReference = DownloadInfo[0];
                salesOrder.Customer = customer.GetCustomerCode(DownloadInfo[1]);

                salesOrder.Date = DateTime.ParseExact(DownloadInfo[2], "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                if (!String.IsNullOrEmpty(DownloadInfo[3])) { salesOrder.DueDate = DateTime.ParseExact(DownloadInfo[3], "yyyyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"); }

                salesOrder.DeliveryInstructions.Text = DownloadInfo[12];
               
                return salesOrder;
            }
            catch (Exception e)
            {
                string message = "Foodbuy-Cannot create Sales Order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public SalesOrder_Line FoodbuySalesOrder_Line(string[] DownloadInfo, string CustomerOrderNumber)
        {
            SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
            Product product = new Product();
            UnitOfMeasure sellingUnits = new UnitOfMeasure();

            salesOrder_Line.CreatedDate = DateTime.Now.ToString();

            try
            {
                salesOrder_Line.Product = product.GetProductCode(DownloadInfo[4]);

                if (DownloadInfo[6] == "Box") { sellingUnits.ID = UnitOfMeasure.SellingUnits_Box_ID; }
                else if (DownloadInfo[6] == "Carton") { sellingUnits.ID = UnitOfMeasure.SellingUnits_Box_ID; }
                else if (DownloadInfo[7] == "Pack") { sellingUnits.ID = UnitOfMeasure.SellingUnits_Pack_ID; }
                else if (DownloadInfo[6] == "Curtain") { sellingUnits.ID = UnitOfMeasure.SellingUnits_Each_ID; }

                salesOrder_Line.SellingUnits = sellingUnits;
                
                if (DownloadInfo[7].Length == 0) { salesOrder_Line.Quantity = 0; }
                else { salesOrder_Line.Quantity = Convert.ToDecimal(DownloadInfo[7]); }

                if (DownloadInfo[8].Length == 0) { salesOrder_Line.NetPrice = 0; }
                else { salesOrder_Line.NetPrice = Convert.ToDecimal(DownloadInfo[8]); }

                salesOrder_Line.TaxRate.ID = TaxRate.TaxRate_GB01_ID;
                
                return salesOrder_Line;
            }
            catch (Exception e)
            {
                string message = "Foodbuy-Cannot create sales order line - " + CustomerOrderNumber + " - " + product.Code;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
    }
}
