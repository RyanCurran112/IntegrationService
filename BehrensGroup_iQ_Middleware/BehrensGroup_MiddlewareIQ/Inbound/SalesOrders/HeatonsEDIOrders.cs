/*
 * Author:      Ryan Curran
 * Date:        February 2021
 * Description: Class for reading & uploading Heatons EDI Orders
 *              Orders are retrieved from FTP as XML file, these are then moved, read & posted to iQ using respective API endpoints.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.SalesOrders
{
    class HeatonsEDIOrders
    {
        public static string EDIFileName { get; set; }
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\DC01\Company\Intact iQ\BehrensFTP\SalesOrders\Heatons";
        public static string path = @"\\APP01\SalesOrders\Heatons\Orders";        //ProductionPath


        public static void HeatonsOrdersMain()
        {
             
            DownloadSchemaHeatons download = new DownloadSchemaHeatons();

            try
            {
                string[] fileEntries = Directory.GetFiles(OriginalPath);
                foreach (string fileName in fileEntries)
                {
                    string CurrentDateTime = DateTime.Now.ToString();
                    CurrentDateTime = CurrentDateTime.Replace("/", "-");
                    CurrentDateTime = CurrentDateTime.Replace(":", "-");

                    string documentname = Path.GetFileNameWithoutExtension(fileName);
                    File.Move(fileName, path + @"\OrdersExport-" + CurrentDateTime + ".xml");

                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                string message = "Heatons-No Files to Move";
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, true);
            }

            try
            {
                string[] fileEntries = Directory.GetFiles(path);

                foreach (string fileName in fileEntries)
                {
                    string filePath = fileName;
                    EDIFileName = Path.GetFileName(fileName).Replace(".xml", "");

                    string newpath = path + "\\" + EDIFileName;

                    XmlTextReader reader = null;
                    reader = new XmlTextReader(filePath);
                    reader.WhitespaceHandling = WhitespaceHandling.None;

                    reader.MoveToAttribute("BuyersOrderNumber");
                    Console.WriteLine(reader.GetValueAsync());

                    /*
                    if (headStart.Length > 0 && headStart == "RL")
                            {
                                if (CurrentCustomerOrderNo != LastCustomerOrderNo)
                                {
                                    Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
                                    SalesOrder salesOrder = new SalesOrder();

                                    //Check if Sales Order already exists
                                    SalesOrder existingSalesOrder = new SalesOrder();
                                    List<SalesOrder> existingSalesOrders = existingSalesOrder.GetSalesOrders("[eq]", "AlternateReference", CurrentCustomerOrderNo);

                                    if (existingSalesOrders.Count == 0)
                                    {

                                        customer_DeliveryContact = download.HeatonsCustomer_DeliveryContact(values, CurrentCustomerOrderNo);

                                        salesOrder = download.HeatonsSalesOrder(values, customer_DeliveryContact.ID, CurrentCustomerOrderNo);

                                     
                                        using (CsvReader reader2 = new CsvReader(filePath))
                                        {
                                            foreach (string[] values2 in reader2.RowEnumerator)
                                            {
                                                string headStart2 = values2[0];
                                                string CurrentCustomerOrderNo2 = values2[1];

                                                if (headStart.Length > 0 && headStart == "RL" && CurrentCustomerOrderNo == CurrentCustomerOrderNo2)
                                                {
                                                    SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
                                                    salesOrder_Line = download.HeatonsSalesOrder_Line(values2, CurrentCustomerOrderNo);

                                                    if (salesOrder_Line != null)
                                                    {
                                                        salesOrder.SalesOrderLines.Add(salesOrder_Line);
                                                    }
                                                }
                                            }
                                        }

                                        salesOrder.CreateSalesOrder();

                                        string OrderMessage = "Heatons-Successfully added order - " + salesOrder.AlternateReference + ".csv";
                                        LogFile.WritetoLogFile(OrderMessage, true);
                                    }
                                    else
                                    {
                                        string OrderMessage = "Heatons-Order " + CurrentCustomerOrderNo + " already exists within iQ";
                                        LogFile.WritetoLogFile(OrderMessage, false);
                                    }

                                }
                                LastCustomerOrderNo = CurrentCustomerOrderNo;
                            }
                        }
                    }
                    */

                    string CurrentDateTime = DateTime.Now.ToString();
                    CurrentDateTime = CurrentDateTime.Replace("/", "-");
                    CurrentDateTime = CurrentDateTime.Replace(":", "-");

                    File.Move(filePath, path + @"\Complete\OrdersExport-" + CurrentDateTime + ".xml");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "Heatons-Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
            Console.WriteLine("Inbound - Sales Orders - Heatons Orders Complete");
        }
    }
    class DownloadSchemaHeatons : GenericClassTransactionLine
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

        public Customer_DeliveryContact HeatonsCustomer_DeliveryContact(string[] DownloadInfo, long CashCustomerID, string CustomerOrderNumber)
        {
            Customer customer = new Customer();
            Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
            Address address = new Address();

            try
            {

                //Delivery Customer Details
                customer.Code = DownloadInfo[4];                                                    //Should always be 60237
                long CustomerID = 0;
                List<Customer> customers = customer.GetCustomers("[eq]", "Code", customer.Code);
                foreach (Customer getCustomer in customers) { CustomerID = getCustomer.ID; }

                customer_DeliveryContact.LookupCustomerID = CustomerID;
                customer_DeliveryContact.CashCustomerID = CashCustomerID;

                customer_DeliveryContact.UniqueID1 = DownloadInfo[22];
                customer_DeliveryContact.UniqueID2 = DownloadInfo[21];

                //Delivery Contact Details
                customer_DeliveryContact.D_ContactName = DownloadInfo[15];
                customer_DeliveryContact.EmailAddress = DownloadInfo[22];
                customer_DeliveryContact.Phone = DownloadInfo[23];
                if (customer_DeliveryContact.Phone.Substring(0, 1) == "7") { customer_DeliveryContact.Phone = '0' + customer_DeliveryContact.Phone; }
                if (customer_DeliveryContact.Phone.Substring(0, 2) == "07")
                {
                    customer_DeliveryContact.Mobile = customer_DeliveryContact.Phone;
                    customer_DeliveryContact.Phone = null;
                }

                //Delivery Address
                address.AddressLine1 = DownloadInfo[16];
                address.AddressLine2 = DownloadInfo[17];
                address.AddressLine3 = DownloadInfo[18];
                address.AddressLine4 = DownloadInfo[19];
                address.Country.Code = DownloadInfo[20];
                if (address.Country.Code == "GB") { address.Country.Code = "UK"; }
                address.PostCode = DownloadInfo[21];


                BehrensGroup_ClassLibrary.Functions.RESTFilter filterstring = new BehrensGroup_ClassLibrary.Functions.RESTFilter();
                BehrensGroup_ClassLibrary.Functions.RESTFilter filterstring2 = new BehrensGroup_ClassLibrary.Functions.RESTFilter();

                filterstring.htmlAttribute = "firstname";
                filterstring.htmlOperator = "[eq]";
                filterstring.htmlParameter = customer_DeliveryContact.EmailAddress;

                filterstring2.htmlAttribute = "lastname";
                filterstring2.htmlOperator = "[eq]";
                filterstring2.htmlParameter = address.PostCode.Replace(" ", "");


                List<BehrensGroup_ClassLibrary.Functions.RESTFilter> filterstrings = new List<BehrensGroup_ClassLibrary.Functions.RESTFilter>
                {
                    filterstring,
                    filterstring2
                };

                Customer_DeliveryContact searchDelivery_Contact = new Customer_DeliveryContact();
                List<Customer_DeliveryContact> Customer_DeliveryContacts = searchDelivery_Contact.GetCustomer_DeliveryContacts(filterstrings);

                if (Customer_DeliveryContacts.Count > 0)
                {
                    bool found = false;
                    foreach (Customer_DeliveryContact iQCustomer_DeliveryContact in Customer_DeliveryContacts)
                    {
                        if (iQCustomer_DeliveryContact.LookupCustomer.ID == customer_DeliveryContact.LookupCustomerID)
                        {
                            address.ID = iQCustomer_DeliveryContact.Address.ID;
                            address.UpdateAddress();

                            customer_DeliveryContact.ID = iQCustomer_DeliveryContact.ID;
                            customer_DeliveryContact.AddressID = iQCustomer_DeliveryContact.Address.ID;
                            customer_DeliveryContact.LookupCustomerID = iQCustomer_DeliveryContact.LookupCustomer.ID;

                            customer_DeliveryContact.UpdateCashCustomer_DeliveryContact();
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        customer_DeliveryContact.AddressID = address.CreateAddress();
                        customer_DeliveryContact.CreateCashCustomer_DeliveryContact();
                    }
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
                string message = "Heatons-Cannot create customer delivery contact - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
    }
}
