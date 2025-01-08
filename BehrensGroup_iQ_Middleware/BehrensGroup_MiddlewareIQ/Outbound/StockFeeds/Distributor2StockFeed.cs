/*
 * Author:      Ryan Curran
 * Date:        August 2020
 * Description: Class for Distibutor2 Outbound Tasks
 *              Methods for Moving Distibutor2 Stock Feed File to FTP folder in correct format.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Outbound.StockFeeds
{
    class Distributor2StockFeed
    {
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\APP01\SalesOrders\Behrens\CatalogueDist2StockFeed";
        public static string StockFeedPath = @"\\APP01\SalesOrders\Behrens\CatalogueDist2StockFeed\StockFeed.csv";
        public static string NewPath = @"\\DC01\Company\Intact iQ\BehrensFTP\Stock\BehrensCatalogueDist2\StockFeed.csv";

        public static void StockFeedMain()
        {
            string[] fileEntries = Directory.GetFiles(OriginalPath);

            string Product = "";                    //Product
            string ProductDescription = "";
            string ProductStyle = "";
            string ProductColour = "";
            string ProductSize = "";
            string UnallocatedQuantity = "";             //Quantity
            string SellingPrice = "";

            try
            {
                File.Delete(NewPath);
            }
            catch
            {
                string message1 = "Behrens Catalogue Dist2 Previous Stock Feed Deleted";
                LogFile.WritetoLogFile(message1, true);
            }

            try
            {
                foreach (string fileName in fileEntries)
                {
                    string filePath = fileName;

                    using (CsvReader reader = new CsvReader(filePath))
                    {
                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            if (headStart == "Code")
                            {
                                StringBuilder myString = new StringBuilder();

                                myString.Append("ProductCode");
                                myString.Append(",");
                                myString.Append("ProductDescription");
                                myString.Append(",");
                                myString.Append("ProductStyle");
                                myString.Append(",");
                                myString.Append("ProductColour");
                                myString.Append(",");
                                myString.Append("ProductSize");
                                myString.Append(",");
                                myString.Append("StockLevel");
                                myString.Append(",");
                                myString.Append("SellingPrice");

                                StreamWriter file = new StreamWriter(NewPath, true);
                                file.WriteLine(myString);
                                file.Close();
                            }
                            else
                            {
                                if (headStart.Length > 0)
                                {
                                    StringBuilder myString = new StringBuilder();

                                    Product = values[0];
                                    ProductDescription = values[1];
                                    ProductStyle = values[2];
                                    ProductColour = values[3];
                                    ProductSize = values[4];
                                    UnallocatedQuantity = values[5];
                                    SellingPrice = values[6];


                                    myString.Append(Product);
                                    myString.Append(",");
                                    myString.Append(ProductDescription);
                                    myString.Append(",");
                                    myString.Append(ProductStyle);
                                    myString.Append(",");
                                    myString.Append(ProductColour);
                                    myString.Append(",");
                                    myString.Append(ProductSize);
                                    myString.Append(",");
                                    myString.Append(Convert.ToInt32(Convert.ToDecimal(UnallocatedQuantity)));
                                    myString.Append(",");
                                    myString.Append(SellingPrice);
                                   
                                    StreamWriter file = new StreamWriter(NewPath, true);
                                    file.WriteLine(myString);
                                    file.Close();
                                }
                            }
                        }
                    }
                }

                string message1 = "Behrens Catalogue Dist2 Stock Feed Complete";
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
