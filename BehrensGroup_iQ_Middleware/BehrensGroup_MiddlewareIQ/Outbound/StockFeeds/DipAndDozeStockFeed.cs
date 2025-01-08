/*
 * Author:      Ryan Curran
 * Date:        September 2019
 * Description: Class for D&D Stock Feed Outbound Tasks
 *              Methods for Moving Dip And Doze Stock Feed File to FTP folder in correct format.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Outbound.StockFeeds
{
    class DipAndDozeStockFeed
    {
        public static string EDIFileName { get; set; }
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\APP01\SalesOrders\DipAndDoze\StockFeed";
        public static string StockFeedPath = @"\\APP01\SalesOrders\DipAndDoze\StockFeed\AllProducts.csv";
        public static string NewPath = @"\\DC01\Company\Intact iQ\BehrensFTP\Stock\DipAndDoze\StockFeed.csv";

        public static string BundlePath = @"\\APP01\SalesOrders\DipAndDoze\StockFeed\BundleProducts.csv";
        public static string StandardPath = @"\\APP01\SalesOrders\DipAndDoze\StockFeed\StandardProducts.csv";

        public static void StockFeedMain()
        {
            string[] fileEntries = Directory.GetFiles(OriginalPath);

            string ProductType;                     //Kit or Standard Product
            string Product = "";                    //Product
            string LastProduct = "";
            string ComponentProduct;                //Component Product

            string UnallocatedQuantity;             //Quantity
            string LastUnallocatedQuantity = "0";

            string PackSize;

            try
            {
                File.Delete(StockFeedPath);
            }
            catch
            {
                string message1 = "Dip & Doze Previous Stock Feed Deleted";
                LogFile.WritetoLogFile(message1, true);
            }

            try
            {
                foreach (string fileName in fileEntries)
                {
                    string filePath = fileName;

                    if (filePath == BundlePath)
                    {
                        ProductType = "Bundle";
                    }
                    else if (filePath == StandardPath)
                    {
                        ProductType = "Standard";
                    }
                    else
                    {
                        ProductType = "";
                    }

                    using (CsvReader reader = new CsvReader(filePath))
                    {
                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            if (headStart == "Owner")
                            {
                                StringBuilder myString = new StringBuilder();

                                myString.Append("Product");
                                myString.Append(",");
                                myString.Append("UnallocatedQuantity");

                                StreamWriter file = new StreamWriter(StockFeedPath, true);
                                file.WriteLine(myString);
                                file.Close();
                            }
                            else if (headStart == "Product")
                            {

                            }
                            else
                            {
                                if (headStart.Length > 0)
                                {
                                    StringBuilder myString = new StringBuilder();

                                    if (ProductType == "Standard")
                                    {
                                        Product = values[0];
                                        UnallocatedQuantity = values[1];
                                        PackSize = values[2];

                                        myString.Append(Product);
                                        myString.Append(",");
                                        myString.Append(Convert.ToDecimal(UnallocatedQuantity) / Convert.ToDecimal(PackSize));
                                    }
                                    else if (ProductType == "Bundle")
                                    {
                                        Product = values[0];
                                        ComponentProduct = values[1];
                                        UnallocatedQuantity = values[2];
                                        PackSize = values[3];

                                        if (Product == LastProduct)
                                        {
                                            if (Convert.ToDecimal(LastUnallocatedQuantity) < Convert.ToDecimal(UnallocatedQuantity))
                                            {
                                                UnallocatedQuantity = LastUnallocatedQuantity;
                                            }
                                        }

                                        myString.Append(Product);
                                        myString.Append(",");
                                        myString.Append(Convert.ToDecimal(UnallocatedQuantity) / Convert.ToDecimal(PackSize));

                                        LastProduct = Product;
                                        LastUnallocatedQuantity = (Convert.ToDecimal(UnallocatedQuantity) / Convert.ToDecimal(PackSize)).ToString();
                                    }

                                    StreamWriter file = new StreamWriter(StockFeedPath, true);
                                    file.WriteLine(myString);
                                    file.Close();
                                }
                            }
                        }
                    }
                }

                File.Delete(NewPath);
                File.Move(StockFeedPath, NewPath);
                //File.Delete(StockFeedPath);

                string message1 = "Dip & Doze Stock Feed Complete";
                LogFile.WritetoLogFile(message1, true);
            }
            catch (Exception e)
            {
                //Write to Log File
                //string message = "D&D SF - Cannot read product - " + Product;
                //Console.WriteLine(e.Message);
                //LogFile.WritetoLogFile(message, false);
            }
        }
    }
}
