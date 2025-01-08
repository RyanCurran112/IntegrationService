/*
 * Author:      Ryan Curran
 * Date:        October 2019
 * Description: Class for reading & uploading NetEDI Orders
 *              Orders are sent to App Server as CSV file, these are then moved, read & posted to iQ using respective API endpoints.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Transactions;

using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.SalesOrders
{
    class NetEDIOrders
    {
        public static string EDIFileName { get; set; }
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\APP01\NeTIX\Inbound";
        public static string path = @"\\APP01\SalesOrders\NetEDI";        //ProductionPath

        public static void NetEDIOrdersMain()
        {
            DownloadSchemaNetEDI download = new DownloadSchemaNetEDI();

            try
            {
                string[] fileEntries = Directory.GetFiles(OriginalPath);
                foreach (string fileName in fileEntries)
                {
                    string CurrentDateTime = DateTime.Now.ToString();
                    CurrentDateTime = CurrentDateTime.Replace("/", "-");
                    CurrentDateTime = CurrentDateTime.Replace(":", "-");

                    string documentname = Path.GetFileNameWithoutExtension(fileName);
                    File.Move(fileName, path + @"\" + documentname + "-" + CurrentDateTime + ".csv");
                }
            }
            catch (Exception e)
            {
                string message = "NeTIX-No Files to Move";
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, true);
            }

            try
            {
                string[] fileEntries = Directory.GetFiles(path);

                foreach (string fileName in fileEntries)
                {
                    string filePath = fileName;
                    EDIFileName = Path.GetFileName(fileName).Replace(".csv", "");

                    string newpath = path + "\\" + EDIFileName;

                    using (CsvReader reader = new CsvReader(filePath))
                    {
                        List<SalesOrder> listSalesOrders = new List<SalesOrder>();

                        string LastCustomerOrderNo = "";
                        string CurrentCustomerOrderNo = "";

                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            
                            if (headStart.Length > 0 && headStart == "HEAD")
                            {
                                //Check if Sales Order already exists
                                Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
                                SalesOrder salesOrder = new SalesOrder();

                                CurrentCustomerOrderNo = values[4];
                                SalesOrder existingSalesOrder = new SalesOrder();
                                List<SalesOrder> existingSalesOrders = existingSalesOrder.GetSalesOrders("[eq]", "AlternateReference", CurrentCustomerOrderNo);

                                if (existingSalesOrders.Count == 0)
                                {
                                    customer_DeliveryContact = download.NetEDICustomer_DeliveryContact(values, CurrentCustomerOrderNo);

                                    salesOrder = download.NetEDISalesOrder(values, customer_DeliveryContact.ID, CurrentCustomerOrderNo);
                                    salesOrder.PricingType = "NetPricing";
                                    salesOrder.DeliveryPricingType = "NetPricing";

                                    using (CsvReader reader2 = new CsvReader(filePath))
                                    {
                                        string CurrentCustomerOrderNo2 = CurrentCustomerOrderNo;

                                        foreach (string[] values2 in reader2.RowEnumerator)
                                        {
                                            string headStart2 = values2[0];

                                            if (headStart2.Length > 0 && headStart2 == "HEAD")
                                            {
                                                CurrentCustomerOrderNo2 = values2[4];
                                            }
                                            else if (headStart2.Length > 0 && headStart2 == "LINE" && CurrentCustomerOrderNo == CurrentCustomerOrderNo2)
                                            {
                                                SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
                                                salesOrder_Line = download.NetEDISalesOrder_Line(values2, salesOrder, CurrentCustomerOrderNo);

                                                if (salesOrder_Line != null)
                                                {
                                                    salesOrder.SalesOrderLines.Add(salesOrder_Line);
                                                }
                                            }
                                            else if (headStart2.Length > 0 && headStart2 == "TEXT" && CurrentCustomerOrderNo == CurrentCustomerOrderNo2)
                                            {
                                                salesOrder.Particulars = values2[2];
                                            }
                                            else if (headStart2.Length > 0 && headStart2 == "RECON" && CurrentCustomerOrderNo == CurrentCustomerOrderNo2)
                                            {

                                            }
                                        }
                                    }

                                    salesOrder.CreateSalesOrder();
                                    //salesOrder.UpdateSalesOrder();

                                    string OrderMessage = "NeTIX-Successfully added order - " + salesOrder.AlternateReference + ".csv";
                                    LogFile.WritetoLogFile(OrderMessage, true);
                                }
                                else
                                {
                                    string OrderMessage = "NeTIX-Order " + CurrentCustomerOrderNo + " already exists within iQ";
                                    LogFile.WritetoLogFile(OrderMessage, false);
                                }
                            }
                            LastCustomerOrderNo = CurrentCustomerOrderNo;
                        }
                    }

                    string CurrentDateTime = DateTime.Now.ToString();
                    CurrentDateTime = CurrentDateTime.Replace("/", "-");
                    CurrentDateTime = CurrentDateTime.Replace(":", "-");

                    File.Move(filePath, path + @"\Complete\OrdersExport-" + CurrentDateTime + ".csv");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "NeTIX-Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }

            Console.WriteLine("Inbound - Sales Orders - NetEDI Orders Complete");
        }
    }

    class DownloadSchemaNetEDI : GenericClassTransactionLine
    {
        public Customer_DeliveryContact NetEDICustomer_DeliveryContact(string[] DownloadInfo, string CustomerOrderNo)
        {
            Address address = new Address();
            Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
            Customer customer = new Customer();
            Division division = new Division();

            try
            {
                //Delivery Customer Details
                customer.Code = DownloadInfo[2];                                                     

                long CustomerID = 0;
                List<Customer> customers = customer.GetCustomers("[eq]", "Code", customer.Code);
                foreach (Customer getCustomer in customers) { CustomerID = getCustomer.ID; }
                customer_DeliveryContact.LookupCustomerID = CustomerID;

                //Delivery Contact Details
                if (customer.Code == "19044")
                {
                    division.ID = Division.Division_BHT_ID;

                    customer_DeliveryContact.UniqueID1 = DownloadInfo[23];

                    if (DownloadInfo[4].Substring(0, 2) == "UK") { address.Country.Code = "UK"; }
                    else { address.Country.Code = "DE"; }

                    customer_DeliveryContact.EmailAddress = "hometextiles@behrens.co.uk";
                    
                }
                else if (customer.Code == "61832" || customer.Code == "61832" || customer.Code == "40010")
                {
                    division.ID = Division.Division_BHT_ID;

                    customer_DeliveryContact.UniqueID1 = DownloadInfo[22];
                    address.Country.Code = "UK";

                    customer_DeliveryContact.EmailAddress = "hometextiles@behrens.co.uk";
                    customer_DeliveryContact.Phone = DownloadInfo[23];
                }        
                else if (customer.Code == "31858")
                {
                    division.ID = Division.Division_FPC_ID;

                    customer_DeliveryContact.UniqueID1 = DownloadInfo[24];
                    address.Country.Code = "UK";

                    customer_DeliveryContact.EmailAddress = "healthcare@behrens.co.uk";
                    customer_DeliveryContact.Phone = DownloadInfo[23];
                }
                else if (customer.Code == "31129" || customer.Code == "31553")
                {
                    division.ID = Division.Division_FPC_ID;

                    customer_DeliveryContact.UniqueID1 = DownloadInfo[24];
                    address.Country.Code = "UK";

                    customer_DeliveryContact.EmailAddress = "healthcare@behrens.co.uk";
                    customer_DeliveryContact.Phone = DownloadInfo[23];
                }
                else if (customer.Code == "30516" || customer.Code == "60215" || customer.Code == "29178")
                {
                    division.ID = Division.Division_NDV_ID;

                    customer_DeliveryContact.UniqueID1 = DownloadInfo[23];
                    address.Country.Code = "UK";

                    customer_DeliveryContact.EmailAddress = "healthcare@behrens.co.uk";
                    customer_DeliveryContact.Phone = "01618721444";
                }

                customer_DeliveryContact.UniqueID2 = DownloadInfo[21];
                customer_DeliveryContact.D_ContactName = DownloadInfo[16];

                //Delivery Address
                address.AddressLine1 = DownloadInfo[17];
                address.AddressLine2 = DownloadInfo[18];
                address.AddressLine3 = DownloadInfo[19];
                address.AddressLine4 = DownloadInfo[20];
                
                address.PostCode = DownloadInfo[21];


                //Delivery Contact Details
                customer_DeliveryContact.Phone = "01618721444";

                BehrensGroup_ClassLibrary.Functions.RESTFilter filterstring = new BehrensGroup_ClassLibrary.Functions.RESTFilter();
                BehrensGroup_ClassLibrary.Functions.RESTFilter filterstring2 = new BehrensGroup_ClassLibrary.Functions.RESTFilter();

                filterstring.htmlAttribute = "firstname";
                filterstring.htmlOperator = "[eq]";
                filterstring.htmlParameter = customer_DeliveryContact.UniqueID1;

                filterstring2.htmlAttribute = "lastname";
                filterstring2.htmlOperator = "[eq]";
                filterstring2.htmlParameter = address.PostCode.Replace(" ", "");

                List<BehrensGroup_ClassLibrary.Functions.RESTFilter> filterstrings = new List<BehrensGroup_ClassLibrary.Functions.RESTFilter>
                {
                    filterstring,
                    filterstring2
                };

                List<Customer_DeliveryContact> Customer_DeliveryContacts = customer_DeliveryContact.GetCustomer_DeliveryContacts(filterstrings);

                if (Customer_DeliveryContacts.Count > 0)
                {
                    address.ID = customer_DeliveryContact.AddressID;
                    if (customer_DeliveryContact.LookupCustomer.Code != "31858")
                    {
                        address.UpdateAddress();
                    }
                    
                    customer_DeliveryContact.ID = Customer_DeliveryContacts[0].ID;
                    customer_DeliveryContact.UpdateCashCustomer_DeliveryContact();
                }
                else
                {
                    customer_DeliveryContact.AddressID = address.CreateAddress();
                    customer_DeliveryContact.CreateCashCustomer_DeliveryContact();
                }
                return customer_DeliveryContact;
            }
            catch (Exception e)
            {
                string message = "Cannot read order - " + CustomerOrderNo;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public SalesOrder NetEDISalesOrder(string[] DownloadInfo, long Customer_DeliveryContactID, string CustomerOrderNo)
        {
            SalesOrder salesOrder = new SalesOrder();
            Customer customer = new Customer();

            //Order Details
            salesOrder.Branch.ID = Branch.Branch_HQ_ID;
            salesOrder.DespatchBranch.ID = Branch.Branch_HQ_ID;
            salesOrder.OrderType.ID = SalesOrderType.OrderType_EDI_ID;
            salesOrder.Source.ID = SalesOrderSource.OrderSource_NEDI_ID;
            salesOrder.Currency.ID = Currency.Currency_GBP_ID;

            try
            {
                //Alternate Reference 
                salesOrder.AlternateReference = DownloadInfo[4];

                if (!String.IsNullOrEmpty(DownloadInfo[5])) { salesOrder.Date = DownloadInfo[5]; }
                else { salesOrder.Date = DateTime.Now.ToString(); }

                if (!String.IsNullOrEmpty(DownloadInfo[6])) { salesOrder.DueDate = DownloadInfo[6]; }

                //Customer Details
                salesOrder.Customer = customer.GetCustomerCode(DownloadInfo[2]);              
          
                if (salesOrder.Customer.Code == "19044" || salesOrder.Customer.Code == "61832" || salesOrder.Customer.Code == "61832" || salesOrder.Customer.Code == "40010")
                {
                    salesOrder.D_Division.ID = Division.Division_BHT_ID;
                }
                else if (salesOrder.Customer.Code == "31129" || salesOrder.Customer.Code == "31553" )
                {
                    salesOrder.D_Division.ID = Division.Division_FPC_ID;
                }
                else if (salesOrder.Customer.Code == "31858")
                {
                    salesOrder.D_Division.ID = Division.Division_FPC_ID;
                    salesOrder.D_ReqPt1 = DownloadInfo[9];
                    salesOrder.D_ReqPt2 = DownloadInfo[12];

                    salesOrder.D_ReqNo = DownloadInfo[10];
                }
                else if (salesOrder.Customer.Code == "30516" || salesOrder.Customer.Code == "60215" || salesOrder.Customer.Code == "31269" || salesOrder.Customer.Code == "29178")
                {
                    salesOrder.D_Division.ID = Division.Division_NDV_ID;
                }


                salesOrder.DeliveryContact.ID = Customer_DeliveryContactID;

                return salesOrder;
            }
            catch (Exception e)
            {
                string message = "NeTIX-Cannot create Sales Order - " + CustomerOrderNo;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }

        }

        public SalesOrder_Line NetEDISalesOrder_Line(string[] DownloadInfo, SalesOrder salesOrder, string CustomerOrderNo)
        {
            SalesOrder_Line salesOrder_Line = new SalesOrder_Line();

            Product product = new Product();
            Customer_ProductProperties customer_ProductProperties = new Customer_ProductProperties();
            UnitOfMeasure sellingUnits = new UnitOfMeasure();

            try
            {
                salesOrder_Line.CreatedDate = DateTime.Now.ToString();

                if (DownloadInfo[2].Length == 0) { salesOrder_Line.Product = product.GetProductCode(DownloadInfo[6]); }
                else { salesOrder_Line.Product = product.GetProductCode(DownloadInfo[2]); }

                if (salesOrder_Line.Product == null)
                {
                    List<Customer_ProductProperties> CustomerProductPropertiess = new List<Customer_ProductProperties>();
                    CustomerProductPropertiess = customer_ProductProperties.GetCustomer_ProductProperties(product.Code);

                    if (DownloadInfo[2].Length == 0) { CustomerProductPropertiess = customer_ProductProperties.GetCustomer_ProductProperties(DownloadInfo[6]); }
                    else { CustomerProductPropertiess = customer_ProductProperties.GetCustomer_ProductProperties(DownloadInfo[2]); }

                    foreach (Customer_ProductProperties getCustomerProductProperties in CustomerProductPropertiess)
                    {
                        if (getCustomerProductProperties._Owner_.Code == salesOrder.Customer.Code)
                        {
                            salesOrder_Line.Product = getCustomerProductProperties.Product;
                        }
                    }
                }

                if (salesOrder.Customer.Code == "31858" || DownloadInfo[26] == "BX")
                {
                    salesOrder_Line.SellingUnits = sellingUnits.GetUnitOfMeasureCode("Box");
                }
                else if (salesOrder.Customer.Code == "61832" || DownloadInfo[26] == "EA") 
                { 
                    salesOrder_Line.SellingUnits = sellingUnits.GetUnitOfMeasureCode("Carton");
                }
                else 
                { 
                    salesOrder_Line.SellingUnits = sellingUnits.GetUnitOfMeasureCode("Each");
                }

                if (DownloadInfo[25].Length == 0) { salesOrder_Line.Quantity = 0; }
                else { salesOrder_Line.Quantity = Convert.ToDecimal(DownloadInfo[25]); }

                if (DownloadInfo[9].Length == 0) { salesOrder_Line.NetPrice = 0; }
                else { salesOrder_Line.NetPrice = Convert.ToDecimal(DownloadInfo[9]); }

                salesOrder_Line.TaxRate.ID = TaxRate.TaxRate_GB01_ID;
                
                return salesOrder_Line;
            }
            catch (Exception e)
            {
                string message = "NeTIX-Cannot create sales order line - " + CustomerOrderNo + " - " + product.Code;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
    }
}
