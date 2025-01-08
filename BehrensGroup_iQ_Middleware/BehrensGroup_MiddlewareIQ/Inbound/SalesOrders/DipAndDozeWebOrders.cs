/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for reading & uploading Dip And Doze Web Orders
 *              Orders are sent to FTP as CSV file, these are then moved, read & posted to iQ using respective API endpoints.
 */
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.SalesOrders
{
    class DipAndDozeWebOrders
    {
        public static string EDIFileName { get; set; }
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\DC01\Company\Intact iQ\BehrensFTP\SalesOrders\DipAndDoze";
        public static string path = @"\\APP01\SalesOrders\DipAndDoze";        //ProductionPath

        //public static string path = @"C:\Users\rcurran.BEHRENS\Desktop\DipAndDoze";     //TestPath

        public static void DipAndDozeOrdersMain()
        {
            DownloadSchemaDipNDoze download = new DownloadSchemaDipNDoze();
            
            try
            {
                string[] fileEntries = Directory.GetFiles(OriginalPath);
                foreach (string fileName in fileEntries)
                {
                    string CurrentDateTime = DateTime.Now.ToString();
                    CurrentDateTime = CurrentDateTime.Replace("/", "-");
                    CurrentDateTime = CurrentDateTime.Replace(":", "-");

                    string documentname = Path.GetFileNameWithoutExtension(fileName);
                    File.Move(fileName, path + @"\OrdersExport-" + CurrentDateTime + ".csv");

                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                string message = "D&D-No Files to Move";
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
                            CurrentCustomerOrderNo = values[1];

                            if (headStart.Length > 0 && headStart == "RL")
                            {
                                if (CurrentCustomerOrderNo != LastCustomerOrderNo)
                                {
                                    CashCustomer cashCustomer = new CashCustomer();
                                    Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
                                    SalesOrder salesOrder = new SalesOrder();
                                    Receipt receipt = new Receipt();

                                    //Check if Sales Order already exists
                                    SalesOrder existingSalesOrder = new SalesOrder();
                                    List<SalesOrder> existingSalesOrders = existingSalesOrder.GetSalesOrders("[eq]", "AlternateReference", CurrentCustomerOrderNo);

                                    if (existingSalesOrders.Count == 0)
                                    { 
                                    
                                        cashCustomer = download.DipAndDozeCashCustomer(values, CurrentCustomerOrderNo);
                                        customer_DeliveryContact = download.DipAndDozeCustomer_DeliveryContact(values, cashCustomer.ID, CurrentCustomerOrderNo);
                                        
                                        salesOrder = download.DipAndDozeSalesOrder(values, cashCustomer, customer_DeliveryContact.ID, CurrentCustomerOrderNo);
                                        receipt = download.DipAndDozeSalesOrderReceipt(values, CurrentCustomerOrderNo);

                                        salesOrder.SalesOrderReceipts.Add(receipt);
                                    
                                        using (CsvReader reader2 = new CsvReader(filePath))
                                        { 
                                            foreach (string[] values2 in reader2.RowEnumerator)
                                            {
                                                string headStart2 = values2[0];
                                                string CurrentCustomerOrderNo2 = values2[1];

                                                if (headStart.Length > 0 && headStart == "RL" && CurrentCustomerOrderNo == CurrentCustomerOrderNo2)
                                                {
                                                    SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
                                                    salesOrder_Line = download.DipAndDozeSalesOrder_Line(values2, CurrentCustomerOrderNo);

                                                    if (salesOrder_Line != null)
                                                    { 
                                                        salesOrder.SalesOrderLines.Add(salesOrder_Line);
                                                    }
                                                }
                                            }
                                        }

                                        salesOrder.CreateSalesOrder();
                                        //salesOrder.UpdateSalesOrder();
                                       
                                        string OrderMessage = "D&D-Successfully added order - " + salesOrder.AlternateReference + ".csv";
                                        LogFile.WritetoLogFile(OrderMessage, true);
                                    }
                                    else
                                    {
                                        string OrderMessage = "D&D-Order " + CurrentCustomerOrderNo + " already exists within iQ";
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

                    File.Move(filePath, path + @"\Complete\OrdersExport-"  + CurrentDateTime + ".csv");
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "D&D-Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
            Console.WriteLine("Inbound - Sales Orders - D&D Orders Complete");
        }
    }

    class DownloadSchemaDipNDoze : GenericClassTransactionLine
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


        //Creates or updates a cash customer within iQ
        public CashCustomer DipAndDozeCashCustomer(string[] DownloadInfo, string CustomerOrderNumber)
        {
            CashCustomer cashCustomer = new CashCustomer();
            Address address = new Address();
            
            cashCustomer.D_DivisionID = Division.Division_DAD_ID;
            cashCustomer.Branch.ID = Branch.Branch_HQ_ID;

            try
            {
                //Customer Details
                cashCustomer.FirstName = DownloadInfo[13];                            //Primary Key
                cashCustomer.FullName = DownloadInfo[6];

                //Contact Details
                cashCustomer.EmailAddress = DownloadInfo[13];

                cashCustomer.Phone = DownloadInfo[14];
                if (cashCustomer.Phone.Substring(0, 1) == "7") { cashCustomer.Phone = '0' + cashCustomer.Phone; }
                if (cashCustomer.Phone.Substring(0, 2) == "07")
                {
                    cashCustomer.Mobile = cashCustomer.Phone;
                    cashCustomer.Phone = null;
                }

                //Billing Address
                address.AddressLine1 = DownloadInfo[7];
                address.AddressLine2 = DownloadInfo[8];
                address.AddressLine3 = DownloadInfo[9];
                address.AddressLine4 = DownloadInfo[10];
                address.Country.Code = DownloadInfo[11];

                if (address.Country.Code == "GB") { address.Country.Code = "UK"; }
                address.PostCode = DownloadInfo[12];

                List<CashCustomer> cashCustomers = cashCustomer.GetCashCustomers("[eq]", "EmailAddress", cashCustomer.EmailAddress);

                if (cashCustomers.Count > 0)
                {
                    address.ID = cashCustomers[0].AddressID;
                    cashCustomer.ID = cashCustomers[0].ID;

                    if (!cashCustomer.EmailAddress.Contains("@behrens.co.uk") && !cashCustomer.EmailAddress.Contains("dipanddoze.com"))
                    {
                        address.UpdateAddress();
                        cashCustomer.UpdateCashCustomer();
                    }
                }
                else
                {
                    cashCustomer.AddressID = address.CreateAddress();
                    cashCustomer.CreateCashCustomer();
                }
                
                return cashCustomer;
            }
            catch (Exception e)
            {
                string message = "D&D-Cannot create cash customer - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        /*public Customer_Contact DipAndDozeCustomer_Contact(string[] DownloadInfo, string CustomerOrderNumber)
        {
            //Creates or updates a Customer Contact Object within iQ

            Address address = new Address();
            Customer_Contact customer_Contact = new Customer_Contact();
            Customer customer = new Customer();

            try
            {
                AlternateReference = DownloadInfo[1];

                //Customer Details
                long CustomerID = 0;
                customer.Code = DownloadInfo[4];                                                    //Should always be 60237
                List<Customer> customers = customer.GetCustomers("[eq]", "Code", customer.Code);
                foreach (Customer getCustomer in customers) { CustomerID = getCustomer.ID; }
                customer_Contact.LookupCustomer = CustomerID;

                customer_Contact.UniqueID = DownloadInfo[13].Replace(",", ".");
                customer_Contact.D_ContactName = DownloadInfo[6].Replace(",", ".").ToTitleCase();

                //Billing Address
                address.AddressLine1 = DownloadInfo[7].Replace(",", ".").ToTitleCase();
                address.AddressLine2 = DownloadInfo[8].Replace(",", ".").ToTitleCase();
                address.AddressLine3 = DownloadInfo[9].Replace(",", ".").ToTitleCase();
                address.AddressLine4 = DownloadInfo[10].Replace(",", ".").ToTitleCase();
                address.Country.Code = DownloadInfo[11];
                if (address.Country.Code == "GB") { address.Country.Code = "UK"; }
                address.PostCode = DownloadInfo[12].Replace(",", ".").ToUpper();

                //Contact Details
                customer_Contact.EmailAddress = DownloadInfo[13].Replace(",", ".");
                customer_Contact.Phone = DownloadInfo[14].Replace(",", ".");
                if (customer_Contact.Phone.Substring(0, 1) == "7") { customer_Contact.Phone = '0' + customer_Contact.Phone; }
                if (customer_Contact.Phone.Substring(0, 2) == "07")
                {
                    customer_Contact.Mobile = customer_Contact.Phone;
                    customer_Contact.Phone = null;
                }


                List<Customer_Contact> customer_Contacts = customer_Contact.GetCustomer_Contacts("[eq]", "FirstName", customer_Contact.EmailAddress);

                if (customer_Contacts.Count > 0)
                {
                    address.ID = customer_Contact.AddressID;
                    address.UpdateAddress();

                    customer_Contact.ID = customer_Contacts[0].ID;
                    customer_Contact.UpdateCustomer_Contact();
                }
                else
                {
                    customer_Contact.AddressID = address.CreateAddress();
                    customer_Contact.CreateCustomer_Contact();
                }
                return customer_Contact;
            }
            catch (Exception e)
            {
                string message = "D&D-Cannot create customer contact - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }*/

        public Customer_DeliveryContact DipAndDozeCustomer_DeliveryContact(string[] DownloadInfo, long CashCustomerID, string CustomerOrderNumber)
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
                string message = "D&D-Cannot create customer delivery contact - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public SalesOrder DipAndDozeSalesOrder(string[] DownloadInfo, CashCustomer cashCustomer, long Customer_DeliveryContactID, string CustomerOrderNumber)
        {
            SalesOrder salesOrder = new SalesOrder();
            Customer customer = new Customer();
            SalesPromotion salesPromotion = new SalesPromotion();

            //Order Information
            salesOrder.Branch.ID = Branch.Branch_HQ_ID;
            salesOrder.DespatchBranch.ID = Branch.Branch_HQ_ID;
            salesOrder.D_Division.ID = Division.Division_DAD_ID;
            salesOrder.SalesRep.ID = SalesRep.SalesRep_DAD_ID;
            salesOrder.OrderType.ID = SalesOrderType.OrderType_WebCashOrders_ID;
            salesOrder.Source.ID = SalesOrderSource.OrderSource_DAD_ID;
            salesOrder.Currency.ID = Currency.Currency_GBP_ID;

            salesOrder.PricingType = "GrossPricing";
            salesOrder.DeliveryPricingType = "GrossPricing";

            try
            {
                salesOrder.AlternateReference = DownloadInfo[1];
                salesOrder.Date = DownloadInfo[3];
                salesOrder.DueDate = DownloadInfo[25];

                //Customer Information
                customer.Code = DownloadInfo[4];                                 //Should always be 60237
                salesOrder.Customer = customer.GetCustomerCode(customer.Code);
                salesOrder.CashCustomer = cashCustomer;
                salesOrder.DeliveryContact.ID = Customer_DeliveryContactID;
                //SalesOrder.Contact.ID = Customer_ContactID;
                
                //Promotion Information
                PromoCode = DownloadInfo[5];
                if (!String.IsNullOrEmpty(PromoCode))
                {
                    if (PromoCode.Length > 9) { PromoCode = PromoCode.Substring(0, 10).ToUpper(); }
                    else { PromoCode = PromoCode.ToUpper(); }

                    if (PromoCode == "WELCOME10") { PromoCode = "DWELCOME10"; }
                    salesPromotion.GetSalesPromotionCode(PromoCode);
                    if (salesPromotion.ID == 0)
                    {
                        salesPromotion.Code = PromoCode;
                        salesPromotion.CreateSalesPromotion();
                    }
                    salesOrder.Promotion = salesPromotion;
                }

                //Delivery Information
                DeliveryService = DownloadInfo[27];
                DeliveryPrice = DownloadInfo[28];

                if (DeliveryService == "TPS48")
                {
                    salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_RM_ID;
                    salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_RM_ID;

                    salesOrder.DeliveryCostAmount = Convert.ToDecimal("2.26");
                    DeliveryPrice = DeliveryPrice;
                }
                else if (DeliveryService == "NEXT DAY")
                {
                    salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_DPD_ID;
                    salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_DPD_ID;

                    DeliveryPrice = DeliveryPrice;
                }
                salesOrder.DeliveryInstructions.Text = DownloadInfo[24];
                //SalesOrder.AdditionalDeliveryNotes = AdditionalDeliveryNotes;

                decimal DeliveryPriceGross = Convert.ToDecimal(DeliveryPrice);
                decimal DeliveryPriceNet = DeliveryPriceGross / 1.2m;

                salesOrder.DeliveryTaxRate.ID = TaxRate.TaxRate_GB01_ID;
                salesOrder.DeliveryGrossAmount = Convert.ToDecimal(Math.Round(DeliveryPriceGross, 2));

                return salesOrder;
            }
            catch (Exception e)
            {
                string message = "D&D-Cannot create Sales Order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
        
        public SalesOrder_Line DipAndDozeSalesOrder_Line(string[] DownloadInfo, string CustomerOrderNumber)
        {
            SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
            Product product = new Product();
            UnitOfMeasure sellingUnits = new UnitOfMeasure();

            salesOrder_Line.CreatedDate = DateTime.Now.ToString();

            try
            {
                salesOrder_Line.Product = product.GetProductCode(DownloadInfo[33]);
                //salesOrder_Line.SellingUnits = sellingUnits.GetUnitOfMeasure(product.Selling.SellingUnits.ID);
                sellingUnits.ID = UnitOfMeasure.SellingUnits_Each_ID;
                salesOrder_Line.SellingUnits = sellingUnits;
                //salesOrder_Line.SellingUnits = sellingUnits.GetUnitOfMeasureCode("Each");

                if (DownloadInfo[35].Length == 0) { salesOrder_Line.Quantity = 0; }
                else { salesOrder_Line.Quantity = Convert.ToDecimal(DownloadInfo[35]); }

                if (DownloadInfo[36].Length == 0) { salesOrder_Line.GrossPrice = 0; }
                else { salesOrder_Line.GrossPrice = Convert.ToDecimal(DownloadInfo[36]) / salesOrder_Line.Quantity ; }

                salesOrder_Line.TaxRate.ID = TaxRate.TaxRate_GB01_ID;
                salesOrder_Line.GrossPrice = Convert.ToDecimal(DownloadInfo[36]) / Convert.ToDecimal(salesOrder_Line.Quantity);                                  //Need to convert to decimal
                salesOrder_Line.GrossPriceDiscountAmount = Convert.ToDecimal(DownloadInfo[37]) / salesOrder_Line.Quantity;

                return salesOrder_Line;
            }
            catch (Exception e)
            {
                string message = "D&D-Cannot create sales order line - " + CustomerOrderNumber + " - " + product.Code;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public Receipt DipAndDozeSalesOrderReceipt(string[] DownloadInfo, string CustomerOrderNumber)
        {
            Receipt receipt = new Receipt();

            try
            {
                AlternateReference = DownloadInfo[1];

                if (DownloadInfo[31] == "paypal_express") { receipt.ReceiptType = Receipt.ReceiptType_Paypal_ID; }
                else if (DownloadInfo[31] == "klarna_kco") { receipt.ReceiptType = Receipt.ReceiptType_Klarna_ID; }
                receipt.Amount = Convert.ToDecimal(DownloadInfo[30]);

                return receipt;
            }
            catch (Exception e)
            {
                string message = "D&D-Cannot create sales order receipt - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
    }
}
