/*
* Author:      Ryan Curran
* Date:        August 2019
* Description: Class for reading & uploading Stock Bin Transfer Requests for Warehouse
*              Requests are created in CSV file via SQL and uploaded to iQ.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.Objects
{
    public class StockBinTransferRequests
    {
        public static string EDIFileName { get; set; }

        public static string SearchPath = @"\\APP01\SalesOrders\Warehouse\ReplenishmentReport";


        public static void StockBinTransferRequestsMain()
        {
            try
            {
                string[] fileEntries = Directory.GetFiles(SearchPath);

                foreach (string fileName in fileEntries)
                {
                    string filePath = fileName;
                    EDIFileName = Path.GetFileName(fileName).Replace(".csv", "");

                    string FilePath = SearchPath + "\\" + EDIFileName;

                    using (CsvReader reader = new CsvReader(filePath))
                    {
                        string previousproductcode = "";

                        decimal requiredstock;
                        decimal remainderrequired = 1;
                        decimal fromstockbinlevel;
                        decimal boxQuantity;
                        
                        StockBinTransferRequest stockBinTransferRequest = new StockBinTransferRequest();
                        stockBinTransferRequest.RequestedBy = "SYSADMIN";

                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            if (headStart.Length > 0 && values[0] != "Product")
                            {
                                StockBinTransferRequest_Line stockBinTransferRequest_Line = new StockBinTransferRequest_Line();

                                Product product = new Product();
                                stockBinTransferRequest_Line.Product = product.GetProductCode(values[0]);

                                if (stockBinTransferRequest_Line.Product.Code != previousproductcode)
                                {
                                    remainderrequired = 0;
                                    requiredstock = Convert.ToDecimal(values[1]);
                                }
                                else
                                {
                                    requiredstock = remainderrequired;
                                }

                                if (values[2] != "NULL" && values[2] != "")
                                {
                                    stockBinTransferRequest_Line.ProductBatch.ID = Convert.ToInt64(values[2]);
                                }

                                stockBinTransferRequest_Line.FromStockBin.ID = Convert.ToInt64(values[5]);
                                stockBinTransferRequest_Line.ToStockBin.ID = Convert.ToInt64(values[8]);
                         
                                fromstockbinlevel = Convert.ToDecimal(values[7]);

                                if (fromstockbinlevel >= requiredstock)
                                {
                                    stockBinTransferRequest_Line.Quantity = requiredstock;
                                    remainderrequired = 0;
                                }
                                else
                                {
                                    stockBinTransferRequest_Line.Quantity = fromstockbinlevel;
                                    remainderrequired = requiredstock - fromstockbinlevel;
                                }

                                if (stockBinTransferRequest_Line.Quantity > 0)
                                {
                                    stockBinTransferRequest.StockBinTransferRequestLines.Add(stockBinTransferRequest_Line);
                                }

                                previousproductcode = stockBinTransferRequest_Line.Product.Code;
                            }
                        }
                        stockBinTransferRequest.CreateStockBinTransferRequest();
                        string message = "StockBinTransferRequest-Successfully added request - " + stockBinTransferRequest.Number;
                        LogFile.WritetoLogFile(message, true);
                    }
                    File.Move(fileName, SearchPath + @"\Complete\" + EDIFileName + ".csv");
                    Console.WriteLine("Inbound - Warehouse - Stock Bin Transfer Request Complete");
                }
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "Cannot add SBTR";
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
        }

    }
}
