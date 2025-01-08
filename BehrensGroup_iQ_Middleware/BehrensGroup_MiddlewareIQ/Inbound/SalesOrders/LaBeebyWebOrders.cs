/*
 * Author:      Ryan Curran
 * Date:        August 2019
 * Description: Class for reading & uploading LaBeeby Web Orders
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
    class LaBeebyWebOrders
    {
        public static string EDIFileName { get; set; }
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\DC01\Company\Intact iQ\BehrensFTP\SalesOrders\LaBeeby";
        public static string path = @"\\APP01\SalesOrders\LaBeeby";        //ProductionPath

        //public static string path = @"C:\Users\rcurran.BEHRENS\Desktop\LaBeeby";     //TestPath

        public static void LaBeebyOrdersMain()
        {
            DownloadSchemaLaBeeby download = new DownloadSchemaLaBeeby();

            try
            {
                string[] fileEntries = Directory.GetFiles(OriginalPath);
                foreach (string fileName in fileEntries)
                {
                    string CurrentDateTime = DateTime.Now.ToString();
                    CurrentDateTime = CurrentDateTime.Replace("/", "-");
                    CurrentDateTime = CurrentDateTime.Replace(":", "-");

                    File.Move(fileName, path + @"\OrdersExport-" + CurrentDateTime + ".csv");

                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                string message = "LBY-No Files to Move";
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
                        string LastCustomerOrderNo = "";
                        string CurrentCustomerOrderNo = "";

                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            CurrentCustomerOrderNo = values[1];
                            string Carriage = values[34]; 

                            if (headStart.Length > 0 && headStart == "RL" && CurrentCustomerOrderNo != LastCustomerOrderNo && Carriage == "Carriage")
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
                                    cashCustomer = download.LaBeebyCashCustomer(values, CurrentCustomerOrderNo);
                                    customer_DeliveryContact = download.LaBeebyCustomer_DeliveryContact(values, cashCustomer.ID, CurrentCustomerOrderNo);
                                    
                                    
                                    salesOrder = download.LaBeebySalesOrder(values, cashCustomer.ID, customer_DeliveryContact.ID, CurrentCustomerOrderNo);

                                    CustomerOrderNumber = salesOrder.AlternateReference;

                                    receipt = download.LaBeebySalesOrderReceipt(values, CurrentCustomerOrderNo);

                                    salesOrder.SalesOrderReceipts.Add(receipt);
                                    salesOrder.PricingType = "GrossPricing";
                                    salesOrder.DeliveryPricingType = "GrossPricing";

                                    using (CsvReader reader2 = new CsvReader(filePath))
                                    {
                                        foreach (string[] values2 in reader2.RowEnumerator)
                                        {
                                            string headStart2 = values2[0];
                                            string CurrentCustomerOrderNo2 = values2[1];
                                            string Carriage2 = values2[34];

                                            if (headStart.Length > 0 && headStart == "RL" && CurrentCustomerOrderNo == CurrentCustomerOrderNo2 && Carriage2 != "Carriage")
                                            {
                                                SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
                                                salesOrder_Line = download.LaBeebySalesOrder_Line(values2, CurrentCustomerOrderNo);

                                                if (salesOrder_Line != null)
                                                {
                                                    salesOrder.SalesOrderLines.Add(salesOrder_Line);
                                                }
                                            }
                                        }
                                    }

                                    salesOrder.CreateSalesOrder();

                                    string OrderMessage = "LBY-Successfully added order - " + salesOrder.AlternateReference + ".csv";
                                    LogFile.WritetoLogFile(OrderMessage, true);
                                }
                                else
                                {
                                    string OrderMessage = "LBY-Order " + CurrentCustomerOrderNo + " already exists within iQ";
                                    LogFile.WritetoLogFile(OrderMessage, false);
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
                string message = "LBY-Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
            Console.WriteLine("Inbound - Sales Orders - LBY Orders Complete");
        }
    }

    class DownloadSchemaLaBeeby : GenericClassTransactionLine
    {
        public string AlternateReference { get; set; }              //Customer Reference (Unique Per Order Web Generated No) 
        public string CustomerCode { get; set; }                     //Customer Account Code (Not related to Cash Customer)
        public string PromoCode { get; set; }

        public string AdditionalDeliveryNotes { get; set; }

        //Delivery Details
        public string Delivery_Agent { get; set; }
        public string DeliveryService { get; set; }

        public string ProductCode { get; set; }

        

        public CashCustomer LaBeebyCashCustomer(string[] DownloadInfo, string CustomerOrderNumber)
        {
            CashCustomer cashCustomer = new CashCustomer();
            Address address = new Address();

            cashCustomer.D_DivisionID = Division.Division_LBY_ID;
            cashCustomer.Branch.ID = Branch.Branch_HQ_ID;

            try
            {
                AlternateReference = DownloadInfo[1];
                
                //Customer Details
                cashCustomer.FirstName = DownloadInfo[14];
                cashCustomer.FullName = DownloadInfo[5] + " " + DownloadInfo[6];
                cashCustomer.CompanyName = DownloadInfo[7];

                //Billing Address
                address.AddressLine1 = DownloadInfo[8];
                address.AddressLine2 = DownloadInfo[9];
                address.AddressLine3 = DownloadInfo[10];
                address.AddressLine4 = DownloadInfo[11];
                address.Country.Code = DownloadInfo[12];
                if (address.Country.Code == "GB") { address.Country.Code = "UK"; }
                address.PostCode = DownloadInfo[13];

                cashCustomer.EmailAddress = DownloadInfo[14];
                cashCustomer.Phone = DownloadInfo[15];
                if (cashCustomer.Phone.Substring(0, 1) == "7") { cashCustomer.Phone = '0' + cashCustomer.Phone; }
                if (cashCustomer.Phone.Substring(0, 2) == "07")
                {
                    cashCustomer.Mobile = cashCustomer.Phone;
                    cashCustomer.Phone = "";
                }

                List<CashCustomer> CashCustomers = cashCustomer.GetCashCustomers("[eq]", "EmailAddress", cashCustomer.EmailAddress);

                if (CashCustomers.Count > 0)
                {
                    address.ID = cashCustomer.AddressID;
                    address.UpdateAddress();

                    cashCustomer.ID = CashCustomers[0].ID;
                    cashCustomer.UpdateCashCustomer();
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
                string message = "LBY-Cannot create cash customer - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        /*public Customer_Contact LaBeebyCustomer_Contact(string[] DownloadInfo, string CustomerOrderNumber)
        {
            Address address = new Address();
            Customer_Contact customer_Contact = new Customer_Contact();
            Customer customer = new Customer();

            try
            {
                AlternateReference = DownloadInfo[1];

                //Customer Details
                long CustomerID = 0;
                customer.Code = DownloadInfo[4];          //Should always be 60237
                List<Customer> Customers = customer.GetCustomers("[eq]", "Code", CustomerCode);
                foreach (Customer getCustomer in Customers) { CustomerID = getCustomer.ID; }
                customer_Contact.LookupCustomer = CustomerID;
                
                customer_Contact.UniqueID = DownloadInfo[14].Replace(",", ".");
                customer_Contact.D_ContactName = DownloadInfo[5].Replace(",", ".").ToTitleCase() + " " + DownloadInfo[6].Replace(",", ".").ToTitleCase();
                customer_Contact.ContactCompanyCode = DownloadInfo[7].Replace(",", ".").ToTitleCase();

                //Billing Address
                address.AddressLine1 = DownloadInfo[8].Replace(",", ".").ToTitleCase();
                address.AddressLine2 = DownloadInfo[9].Replace(",", ".").ToTitleCase();
                address.AddressLine3 = DownloadInfo[10].Replace(",", ".").ToTitleCase();
                address.AddressLine4 = DownloadInfo[11].Replace(",", ".").ToTitleCase();
                address.Country.Code = DownloadInfo[12];
                if (address.Country.Code == "GB") { address.Country.Code = "UK"; }
                address.PostCode = DownloadInfo[13].Replace(",", ".").ToUpper();

                //Contact Details
                customer_Contact.EmailAddress = DownloadInfo[14].Replace(",", ".");
                customer_Contact.Phone = DownloadInfo[15].Replace(",", ".");
                if (customer_Contact.Phone.Substring(0, 1) == "7") { customer_Contact.Phone = '0' + customer_Contact.Phone; }
                if (customer_Contact.Phone.Substring(0, 2) == "07")
                {
                    customer_Contact.Mobile = customer_Contact.Phone;
                    customer_Contact.Phone = null;
                }
                

                List<Customer_Contact> Customer_Contacts = customer_Contact.GetCustomer_Contacts("[eq]", "FirstName", customer_Contact.EmailAddress);

                if (Customer_Contacts.Count > 0)
                {
                    address.ID = customer_Contact.AddressID;
                    address.UpdateAddress();

                    customer_Contact.ID = Customer_Contacts[0].ID;
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
                string message = "LBY-Cannot create customer contact - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }*/

        public Customer_DeliveryContact LaBeebyCustomer_DeliveryContact(string[] DownloadInfo, long CashCustomerID, string CustomerOrderNumber)
        {
            Address address = new Address();
            Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
            Customer customer = new Customer();

            try
            {
                //Customer Details
                customer.Code = DownloadInfo[4];          //Should always be 60237
                long CustomerID = 0;
                List<Customer> Customers = customer.GetCustomers("[eq]", "Code", customer.Code);
                foreach (Customer getCustomer in Customers) { CustomerID = getCustomer.ID; }
                customer_DeliveryContact.LookupCustomerID = CustomerID;
                
                customer_DeliveryContact.CashCustomerID = CashCustomerID;

                //Delivery Contact Details
                customer_DeliveryContact.UniqueID1 = DownloadInfo[25];
                customer_DeliveryContact.UniqueID2 = DownloadInfo[24];
                customer_DeliveryContact.D_ContactName = DownloadInfo[16] + " "  + DownloadInfo[17];
                customer_DeliveryContact.CompanyName = DownloadInfo[18];

                //Delivery Address
                address.AddressLine1 = DownloadInfo[19];
                address.AddressLine2 = DownloadInfo[20];
                address.AddressLine3 = DownloadInfo[21];
                address.AddressLine4 = DownloadInfo[22];
                address.Country.Code = DownloadInfo[23];
                if (address.Country.Code == "GB") { address.Country.Code = "UK"; }
                address.PostCode = DownloadInfo[24];

                //Delivery Contact Details
                customer_DeliveryContact.EmailAddress = DownloadInfo[25];
                customer_DeliveryContact.Phone = DownloadInfo[15];
                if (customer_DeliveryContact.Phone.Substring(0, 1) == "7") { customer_DeliveryContact.Phone = '0' + customer_DeliveryContact.Phone; }
                if (customer_DeliveryContact.Phone.Substring(0, 2) == "07")
                {
                    customer_DeliveryContact.Mobile = customer_DeliveryContact.Phone;
                    customer_DeliveryContact.Phone = null;
                }

                BehrensGroup_ClassLibrary.Functions.RESTFilter filterstring = new BehrensGroup_ClassLibrary.Functions.RESTFilter();
                BehrensGroup_ClassLibrary.Functions.RESTFilter filterstring2 = new BehrensGroup_ClassLibrary.Functions.RESTFilter();
                BehrensGroup_ClassLibrary.Functions.RESTFilter filterstring3 = new BehrensGroup_ClassLibrary.Functions.RESTFilter();

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
                string message = "LBY-Cannot create customer delivery contact - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public SalesOrder LaBeebySalesOrder(string[] DownloadInfo, long CashCustomerID, long Customer_DeliveryContactID, string CustomerOrderNumber)
        {
            SalesOrder salesOrder = new SalesOrder();
            Customer customer = new Customer();
            //SalesPromotion salesPromotion = new SalesPromotion();

            salesOrder.Branch.ID = Branch.Branch_HQ_ID;
            salesOrder.DespatchBranch.ID = Branch.Branch_HQ_ID;
            salesOrder.D_Division.ID = Division.Division_LBY_ID;
            salesOrder.SalesRep.ID = SalesRep.SalesRep_LBY_ID;
            salesOrder.OrderType.ID = SalesOrderType.OrderType_WebCashOrders_ID;
            salesOrder.Source.ID = SalesOrderSource.OrderSource_LBY_ID;
            salesOrder.Currency.ID = Currency.Currency_GBP_ID;

            try
            {
                salesOrder.AlternateReference = DownloadInfo[1];
                salesOrder.Date = DownloadInfo[3];

                salesOrder.DeliveryContact.ID = Customer_DeliveryContactID;
                salesOrder.CashCustomer.ID = CashCustomerID;

                CustomerCode = DownloadInfo[4];                                 //Should always be 
                salesOrder.Customer = customer.GetCustomerCode(CustomerCode);

                SalesPromotion salesPromotion = new SalesPromotion();
                //Promotion Code
                PromoCode = DownloadInfo[45];
                if (!String.IsNullOrEmpty(PromoCode))
                {
                    if (PromoCode.Length > 7) { PromoCode = "L_" + PromoCode.Substring(0, 8).ToUpper(); }
                    else { PromoCode = "L_" + PromoCode.ToUpper(); }

                    salesPromotion.GetSalesPromotionCode(PromoCode);
                    if (salesPromotion.ID == 0)
                    {
                        salesPromotion.Code = PromoCode;
                        salesPromotion.CreateSalesPromotion();
                    }
                    salesOrder.Promotion = salesPromotion;
                }

                salesOrder.D_TagNumber = "81";

                if (!String.IsNullOrEmpty(DownloadInfo[27])) { salesOrder.DueDate = DownloadInfo[27]; }

                Delivery_Agent = DownloadInfo[29];
                DeliveryService = DownloadInfo[29];

                if (DeliveryService == "CARND")
                {
                    salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_UKM_ID;
                    salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_UKM_ID;
                }
                else if (DeliveryService == "HBCAR")
                {
                    salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_RM_ID;
                    salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_RM_ID;

                    salesOrder.DeliveryCostAmount = Convert.ToDecimal("2.26");
                }
                else if (DeliveryService == "GFSINT")
                {
                    salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_GFS_ID;
                    salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_GFS_INT_ID;
                }

               
                salesOrder.DeliveryInstructions.Text = DownloadInfo[26];

                if (DownloadInfo[41] == "GB01") 
                { 
                    salesOrder.DeliveryTaxRate.ID = TaxRate.TaxRate_GB01_ID;
                    salesOrder.DeliveryGrossAmount = Math.Round(Convert.ToDecimal(DownloadInfo[30]) * 1.2m, 2);                 //Convert to decimal
                }
                else if (DownloadInfo[41] == "GB05") 
                { 
                    salesOrder.DeliveryTaxRate.ID = TaxRate.TaxRate_GB05_ID;
                    salesOrder.DeliveryGrossAmount = Math.Round(Convert.ToDecimal(DownloadInfo[30]), 2);                      //Convert to decimal
                }

                return salesOrder;
            }
            catch (Exception e)
            {
                string message = "LBY-Cannot create sales order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public SalesOrder_Line LaBeebySalesOrder_Line(string[] DownloadInfo, string CustomerOrderNumber)
        {
            SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
            Product product = new Product();
            UnitOfMeasure sellingUnits = new UnitOfMeasure();

            try
            {
                salesOrder_Line.CreatedDate = DateTime.Now.ToString();

                salesOrder_Line.Product = product.GetProductCode(DownloadInfo[33]);
                if (salesOrder_Line.Product.Code == "BEH-FACEMASK-EN14683") { salesOrder_Line.SellingUnits = sellingUnits.GetUnitOfMeasureCode("Pack"); }
                else { salesOrder_Line.SellingUnits = sellingUnits.GetUnitOfMeasureCode("Each"); }

                salesOrder_Line.D_ContactName = DownloadInfo[16] + " " + DownloadInfo[17];

                if (String.IsNullOrEmpty(DownloadInfo[35])) { salesOrder_Line.Quantity = 0; }
                else { salesOrder_Line.Quantity = Convert.ToDecimal(DownloadInfo[35]); }

                decimal NetPrice = Convert.ToDecimal(DownloadInfo[36]);
                decimal NetPriceSub = Convert.ToDecimal(DownloadInfo[37]);
                decimal DiscountAmount = Decimal.Divide(NetPriceSub, salesOrder_Line.Quantity) - NetPrice;
                decimal VATAmount = Convert.ToDecimal(DownloadInfo[40]);

                decimal GrossAmount = (NetPriceSub + VATAmount) / salesOrder_Line.Quantity;

                salesOrder_Line.GrossPrice = GrossAmount;
                
                if (DownloadInfo[41] == "GB01") 
                {
                    salesOrder_Line.GrossPriceDiscountAmount = Decimal.Round(DiscountAmount * 1.2m, 2);
                    salesOrder_Line.TaxRate.ID = TaxRate.TaxRate_GB01_ID; 
                }
                else if (DownloadInfo[41] == "GB05") 
                {
                    salesOrder_Line.GrossPriceDiscountAmount = Decimal.Round(DiscountAmount, 2);
                    if (salesOrder_Line.Product.Code == "BEH-FACEMASK-EN14683") { salesOrder_Line.TaxRate.ID = TaxRate.TaxRate_GB04_ID; }
                    else { salesOrder_Line.TaxRate.ID = TaxRate.TaxRate_GB05_ID; }
                }

                //else { salesOrder_Line.TaxRate.ID = TaxRate_GB05_ID; }
                //salesOrder_Line.NetPrice = Convert.ToDecimal(salesOrder_Line.NetPrice) / Convert.ToDecimal(salesOrder_Line.Quantity);
                //salesOrder_Line.Dis = salesOrderLine.NetPrice - Convert.ToDecimal(NetPriceLessDiscount);
                //salesOrderLine.DiscountAmount = salesOrderLine.DiscountAmount * salesOrderLine.Quantity;

                if (salesOrder_Line.Quantity != 0) { salesOrder_Line.TaxAmount = Convert.ToDecimal(DownloadInfo[39]) / salesOrder_Line.Quantity; }; 

                //salesOrder_Line.GrossPrice = salesOrder_Line.NetPrice + salesOrder_Line.TaxAmount;
                //salesOrder_Line.GrossPriceDiscountAmount = Conve rt.ToDecimal(DownloadInfo[38]) * 1.2m / salesOrder_Line.Quantity;
                                
                 return salesOrder_Line;
            }
            catch (Exception e)
            {
                string message = "LBY-Cannot create sales order line - " + CustomerOrderNumber + " - " + ProductCode;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public Receipt LaBeebySalesOrderReceipt(string[] DownloadInfo, string CustomerOrderNumber)
        {
            Receipt receipt = new Receipt();

            try
            {
                if (DownloadInfo[32] == "paypal") { receipt.ReceiptType = Receipt.ReceiptType_Paypal_ID; }
                else if (DownloadInfo[32] == "realex_redirect") { receipt.ReceiptType = Receipt.ReceiptType_Realex_ID; }
                else if (DownloadInfo[32] == "kco") { receipt.ReceiptType = Receipt.ReceiptType_Klarna_ID;  }
                receipt.Amount = Convert.ToDecimal(DownloadInfo[31]);

                return receipt;
            }
            catch (Exception e)
            {
                string message = "LBY-Cannot create sales order receipt - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
    }
}
