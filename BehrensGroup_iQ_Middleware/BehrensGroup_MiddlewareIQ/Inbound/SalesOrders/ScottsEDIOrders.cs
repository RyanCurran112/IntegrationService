/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for reading & uploading Scotts Orders
 *              Orders are downloaded from Portal as CSV file, these are then moved, read & posted to iQ using respective API endpoints.
 *              -- Further amendments to have full integration between us and portal --
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
    class ScottsWebOrders
    {
        public static string EDIFileName { get; set; }
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\DC01\Company\Intact iQ\BehrensFTP\SalesOrders\DipAndDoze";
        public static string path = @"\\APP01\SalesOrders\ScottsEDI";        //ProductionPath

        //public static string path = @"C:\Users\rcurran.BEHRENS\Desktop\DipAndDoze";     //TestPath

        public static void ScottsOrdersMain()
        {
            DownloadSchemaScotts download = new DownloadSchemaScotts();

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
                            CurrentCustomerOrderNo = values[0];
                            CustomerOrderNumber = values[0];

                            if (headStart.Length > 0 && headStart.Substring(0,3) == "DLN")
                            {
                                if (CurrentCustomerOrderNo != LastCustomerOrderNo)
                                {
                                    //Check if Sales Order already exists
                                    Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
                                    SalesOrder salesOrder = new SalesOrder();

                                    SalesOrder existingSalesOrder = new SalesOrder();
                                    List<SalesOrder> existingSalesOrders = existingSalesOrder.GetSalesOrders("[eq]", "AlternateReference", CustomerOrderNumber);

                                    if (existingSalesOrders.Count == 0)
                                    {
                                        customer_DeliveryContact = download.ScottsCustomer_DeliveryContact(values, CustomerOrderNumber);
                                        salesOrder = download.ScottsSalesOrder(values, customer_DeliveryContact.ID, CustomerOrderNumber);

                                        using (CsvReader reader2 = new CsvReader(filePath))
                                        {
                                            foreach (string[] values2 in reader2.RowEnumerator)
                                            {
                                                string headStart2 = values2[0];
                                                string CurrentCustomerOrderNo2 = values2[0];
                                                if (headStart2.Length > 0 && headStart2.Substring(0, 3) == "DLN" && CurrentCustomerOrderNo == CurrentCustomerOrderNo2)
                                                {
                                                    SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
                                                    salesOrder_Line = download.ScottsSalesOrder_Line(values2, CustomerOrderNumber);

                                                    if (salesOrder_Line != null)
                                                    {
                                                        salesOrder.SalesOrderLines.Add(salesOrder_Line);
                                                    }
                                                }
                                            }
                                        }

                                        salesOrder.PricingType = "NetPricing";
                                        salesOrder.DeliveryPricingType = "NetPricing";

                                        salesOrder.CreateSalesOrder();

                                        string OrderMessage = "BHT-Successfully added order - " + salesOrder.AlternateReference + ".csv";
                                        LogFile.WritetoLogFile(OrderMessage, true);
                                    }
                                    else
                                    {
                                        string OrderMessage = "BHT-Order " + CustomerOrderNumber + " already exists within iQ";
                                        LogFile.WritetoLogFile(OrderMessage, false);
                                    }
                                }
                                LastCustomerOrderNo = CurrentCustomerOrderNo;
                            }
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
                string message = "BHT-Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
            Console.WriteLine("Inbound - Sales Orders - Scotts Orders Complete");
        }
    }

    class DownloadSchemaScotts : GenericClassTransactionLine
    {
        public string CustomerCode { get; set; }                     //Customer Account Code (Not related to Cash Customer)

        public string DeliveryPhone { get; set; }
        public string DeliveryMobile { get; set; }
        public string AdditionalDeliveryNotes { get; set; }

        //Delivery Details
        public string DeliveryPricingType { get; set; }
        public string DeliveryPrice { get; set; }                   //Comes from Net Price on Order Line
        public string ProductCode { get; set; }
        public string Quantity { get; set; }
        public string PricingType { get; set; }
        public string NetPrice { get; set; }

        public Customer_DeliveryContact ScottsCustomer_DeliveryContact(string[] DownloadInfo, string CustomerOrderNumber)
        {
            try
            {
                Address Address = new Address();
                Customer_DeliveryContact Customer_DeliveryContact = new Customer_DeliveryContact();
                Customer Customer = new Customer();

                CustomerCode = "19241";

                //Delivery Contact Details
                long CustomerID = 0;
                List<Customer> Customers = Customer.GetCustomers("[eq]", "Code", CustomerCode);
                foreach (Customer getCustomer in Customers)
                {
                    CustomerID = getCustomer.ID;
                }
                Customer_DeliveryContact.LookupCustomerID = CustomerID;

                Customer_DeliveryContact.UniqueID1 = DownloadInfo[9];
                if (Customer_DeliveryContact.UniqueID1.Contains("@"))
                {
                    Customer_DeliveryContact.UniqueID1 = DownloadInfo[9];
                }
                else
                {
                    Customer_DeliveryContact.UniqueID1 = "ordersurge@scottsandco.com";
                }

                Customer_DeliveryContact.UniqueID2 = DownloadInfo[20];

                Customer_DeliveryContact.D_ContactName = DownloadInfo[8];
                Address.AddressLine1 = DownloadInfo[13];
                Address.AddressLine3 = DownloadInfo[14];
                Address.AddressLine4 = DownloadInfo[15];

                Address.Country.Code = DownloadInfo[24];
                if (Address.Country.Code == "GB") { Address.Country.Code = "UK"; }

                Address.PostCode = DownloadInfo[20];

                Customer_DeliveryContact.EmailAddress = Customer_DeliveryContact.UniqueID1;

                if (DeliveryPhone != null)
                { 
                    DeliveryPhone = DownloadInfo[26].Replace("#", "");
                    if (DeliveryPhone.Substring(0, 1) == "7") { DeliveryPhone = '0' + DeliveryPhone; }
                    if (DeliveryPhone.Substring(0, 2) == "07")
                    {
                        DeliveryMobile = DeliveryPhone;
                        DeliveryPhone = "";
                    }
                }
                Customer_DeliveryContact.Phone = DeliveryPhone;
                Customer_DeliveryContact.Mobile = DeliveryMobile;

                //Customer_DeliveryContact.CashCustomerID = CashCustomerID;

                BehrensGroup_ClassLibrary.Functions.RESTFilter filterstring = new BehrensGroup_ClassLibrary.Functions.RESTFilter();
                BehrensGroup_ClassLibrary.Functions.RESTFilter filterstring2 = new BehrensGroup_ClassLibrary.Functions.RESTFilter();

                filterstring.htmlAttribute = "firstname";
                filterstring.htmlOperator = "[eq]";
                filterstring.htmlParameter = Customer_DeliveryContact.EmailAddress;

                filterstring2.htmlAttribute = "lastname";
                filterstring2.htmlOperator = "[eq]";
                filterstring2.htmlParameter = Address.PostCode.Replace(" ", "");

                List<BehrensGroup_ClassLibrary.Functions.RESTFilter> filterstrings = new List<BehrensGroup_ClassLibrary.Functions.RESTFilter>
                {
                    filterstring,
                    filterstring2
                };

                List<Customer_DeliveryContact> Customer_DeliveryContacts = Customer_DeliveryContact.GetCustomer_DeliveryContacts(filterstrings);

                if (Customer_DeliveryContacts.Count > 0)
                {
                    Address.ID = Customer_DeliveryContact.AddressID;
                    Address.UpdateAddress();

                    Customer_DeliveryContact.ID = Customer_DeliveryContacts[0].ID;
                    Customer_DeliveryContact.UpdateCashCustomer_DeliveryContact();
                }
                else
                {
                    Customer_DeliveryContact.AddressID = Address.CreateAddress();
                    Customer_DeliveryContact.CreateCashCustomer_DeliveryContact();
                }
                return Customer_DeliveryContact;
            }
            catch (Exception e)
            {
                string message = "BHT-Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public SalesOrder ScottsSalesOrder(string[] DownloadInfo, long Customer_DeliveryContactID, string CustomerOrderNumber)
        {
            SalesOrder salesOrder = new SalesOrder();
            Customer customer = new Customer();

            try
            {
                salesOrder.Branch.ID = Branch.Branch_HQ_ID;
                salesOrder.DespatchBranch.ID = Branch.Branch_HQ_ID;
                salesOrder.D_Division.ID = Division.Division_BHT_ID;
                //Get from Customer
                //salesOrder.SalesRep.ID = SalesRep_DB_ID;

                salesOrder.OrderType.ID = SalesOrderType.OrderType_EDI_ID;
                salesOrder.Source.ID = SalesOrderSource.OrderSource_SEDI_ID;
                salesOrder.Currency.ID = Currency.Currency_GBP_ID;
                salesOrder.AlternateReference = DownloadInfo[0];
                salesOrder.Date = DownloadInfo[1];

                CustomerCode = "19241";                                         //Should always be 19241
                salesOrder.Customer = customer.GetCustomerCode(CustomerCode);

                //AdditionalDeliveryNotes = DownloadInfo[24].Replace(",", "."); ;

                
                salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_CC_ID;
                salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_CC_ID;
                DeliveryPrice = "0";

                NetPrice = DownloadInfo[45];
                if (NetPrice.Length == 0)
                {
                    NetPrice = "0";
                }

                salesOrder.DeliveryContact.ID = Customer_DeliveryContactID;

                salesOrder.DeliveryInstructions.Text = AdditionalDeliveryNotes;
                //SalesOrder.AdditionalDeliveryNotes = AdditionalDeliveryNotes;

                salesOrder.DeliveryPricingType = DeliveryPricingType;

                decimal DeliveryPriceNet = Convert.ToDecimal(DeliveryPrice);
                DeliveryPriceNet = Math.Round(DeliveryPriceNet, 3);

                salesOrder.DeliveryNetAmount = Convert.ToDecimal(DeliveryPriceNet);                      //Convert to decimal
                salesOrder.DeliveryTaxRate.ID = TaxRate.TaxRate_GB01_ID;

                return salesOrder;
            }
            catch (Exception e)
            {
                string message = "BHT-Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }

        }

        public SalesOrder_Line ScottsSalesOrder_Line(string[] DownloadInfo, string CustomerOrderNumber)
        {
            SalesOrder_Line salesOrder_Line = new SalesOrder_Line();

            Product product = new Product();
            Customer_ProductProperties customer_ProductProperties = new Customer_ProductProperties();
            UnitOfMeasure sellingUnits = new UnitOfMeasure();

            try
            {
                string CustomerCode = "19241";
                salesOrder_Line.CreatedDate = DownloadInfo[1];

                ProductCode = DownloadInfo[42];
                salesOrder_Line.Product = product.GetProductCode(ProductCode);

                if (salesOrder_Line.Product == null)
                {
                    ProductCode = DownloadInfo[63];
                    List<Customer_ProductProperties> CustomerProductPropertiess = new List<Customer_ProductProperties>();
                    CustomerProductPropertiess = customer_ProductProperties.GetCustomer_ProductProperties(ProductCode);

                    foreach (Customer_ProductProperties getCustomerProductProperties in CustomerProductPropertiess)
                    {
                        if(getCustomerProductProperties._Owner_.Code == CustomerCode)
                        {
                            salesOrder_Line.Product = getCustomerProductProperties.Product;
                        }
                    }
                }

                salesOrder_Line.SellingUnits = sellingUnits.GetUnitOfMeasureCode("Each");

                Quantity = DownloadInfo[47];
                if (Quantity.Length == 0) { salesOrder_Line.Quantity = 0; }
                else { salesOrder_Line.Quantity = Convert.ToDecimal(Quantity); }

                
                NetPrice = DownloadInfo[45];
                if (NetPrice.Length == 0) { NetPrice = "0"; }
                salesOrder_Line.PricingType = PricingType;  
                salesOrder_Line.NetPrice = Convert.ToDecimal(NetPrice);
                salesOrder_Line.TaxRate.ID = TaxRate.TaxRate_GB01_ID;

                return salesOrder_Line;
            }
            catch (Exception e)
            {
                string message = "BHT-Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
    }
}
