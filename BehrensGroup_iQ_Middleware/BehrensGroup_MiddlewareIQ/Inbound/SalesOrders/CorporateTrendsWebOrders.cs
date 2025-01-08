/*
 * Author:      Ryan Curran
 * Date:        October 2019
 * Description: Class for reading & uploading Corporate Trends Web Orders
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
    class CorporateTrendsWebOrders
    {
        public static string EDIFileName { get; set; }
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\DC01\Company\Intact iQ\BehrensFTP\SalesOrders\CorporateTrends";
        public static string path = @"\\APP01\SalesOrders\CorporateTrends";        //ProductionPath

        //public static string path = @"C:\Users\rcurran.BEHRENS\Desktop\DipAndDoze";     //TestPath

        public static void CorporateTrendsOrdersMain()
        {
            DownloadSchemaCorporateTrends download = new DownloadSchemaCorporateTrends();

            string CurrentCustomerOrderNo = "";
            string LastCustomerOrderNo = "";
            
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
                //Write to Log File 
                string message = "CPT-There are no files to move";
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
                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            CurrentCustomerOrderNo = values[1];
                            
                            string CustomerCode = values[23];
                            CustomerCode = MiscFunctions.GetAccountCode(CustomerCode);
                            Customer Customer = new Customer();
                            Customer.GetCustomerCode(CustomerCode);

                            decimal CashAmount = 0;
                            bool CashOrder = false;
                            if (Customer.TypeID == 8645769175180) { CashOrder = false; }
                            else if (Customer.TypeID == 8645769181880) { CashOrder = true; }

                            /*if (values[47] == "0") { CashOrder = false; }
                            else { CashOrder = true; }

                            if (values[36] == "Student") { CashOrder = true; }
                            else { CashOrder = false; }
                            */

                            string Carriage = values[26];

                            if (headStart.Length > 0 && headStart == "RL" && CurrentCustomerOrderNo != LastCustomerOrderNo && (Carriage == "HBCAR" || Carriage == "CARND"))
                            {
                                CashCustomer cashCustomer = new CashCustomer();
                                //Customer_Contact customer_Contact = new Customer_Contact();
                                Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
                                SalesOrder salesOrder = new SalesOrder();
                                
                                Receipt receipt = new Receipt();

                                //Check if Sales Order already exists
                                SalesOrder existingSalesOrder = new SalesOrder();
                                List<SalesOrder> existingSalesOrders = existingSalesOrder.GetSalesOrders("[eq]", "AlternateReference", "CPT_" + CurrentCustomerOrderNo);

                                if (existingSalesOrders.Count == 0)
                                {
                                    if (String.IsNullOrEmpty(values[47])) { CashAmount += 0; }
                                    else { CashAmount = +Convert.ToDecimal(values[47]); }
                        
                                    cashCustomer.ID = 0;

                                    if (CashOrder == true)
                                    { 
                                        cashCustomer = download.CorporateTrendsCashCustomer(values, CurrentCustomerOrderNo);
                                    }
                                
                                    //customer_Contact = download.DipAndDozeCustomer_Contact(values);
                                    customer_DeliveryContact = download.CorporateTrendsCustomer_DeliveryContact(values, cashCustomer.ID, CurrentCustomerOrderNo);

                                
                                salesOrder = download.CorporateTrendsSalesOrder(values, cashCustomer.ID, customer_DeliveryContact.ID, CashOrder, CurrentCustomerOrderNo);

                                using (CsvReader reader2 = new CsvReader(filePath))
                                {
                                    foreach (string[] values2 in reader2.RowEnumerator)
                                    {
                                        string headStart2 = values2[0];
                                        string CurrentCustomerOrderNo2 = values2[1];
                                        string Carriage2 = values2[26];

                                        if (headStart2.Length > 0 && headStart2 == "RL" && CurrentCustomerOrderNo == CurrentCustomerOrderNo2 && Carriage2 != "HBCAR")
                                        {
                                            SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
                                            salesOrder_Line = download.CorporateTrendsSalesOrder_Line(values2, CurrentCustomerOrderNo);

                                            if (String.IsNullOrEmpty(values[47])) { CashAmount += 0; }
                                            else { CashAmount = +Convert.ToDecimal(values[47]); }

                                            if (salesOrder_Line != null)
                                            {
                                                salesOrder.SalesOrderLines.Add(salesOrder_Line);
                                            }
                                        }
                                    }
                                }
                                if (CashOrder)
                                {
                                    salesOrder.PricingType = "GrossPricing";
                                    salesOrder.DeliveryPricingType = "GrossPricing";
                                    receipt = download.CorporateTrendsSalesOrderReceipt(values, CurrentCustomerOrderNo);
                                    CashAmount = Convert.ToDecimal(values[48]);

                                    salesOrder.SalesOrderReceipts.Add(receipt);
                                }
                                else
                                {
                                    salesOrder.PricingType = "NetPricing";
                                    salesOrder.DeliveryPricingType = "NetPricing";
                                }

                                salesOrder.CreateSalesOrder();
                                //salesOrder.UpdateSalesOrder();

                                string OrderMessage = "CPT-Successfully added order - " + salesOrder.AlternateReference + ".csv";
                                LogFile.WritetoLogFile(OrderMessage, true);
                            }

                            else
                            {
                                string OrderMessage = "CPT-Order " + CurrentCustomerOrderNo + " already exists within iQ";
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
                string message = "CPT-Cannot read order - OrdersExport-" + "CPT_" + CurrentCustomerOrderNo;
                LogFile.WritetoLogFile(message, false);
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Inbound - Sales Orders - CPT Orders Complete");
        }
    }

    class DownloadSchemaCorporateTrends  : GenericClassTransactionLine
    {
      
        public string AlternateReference { get; set; }              //Customer Reference (Unique Per Order Web Generated No) 
    
        public string OrderDate { get; set; }                       //Date Order Placed - "dd/mm/yyyy"
        public string CustomerCode { get; set; }                     //Customer Account Code (Not related to Cash Customer)
       

        //Customer/CashCustomer Details
        public string CustomerCountry { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerMobile { get; set; }                  //PK - Cash  Customer 

        //Delivery Contact Details
        
        
        public string DeliveryCountry { get; set; }
       
        public string DeliveryPhone { get; set; }
        public string DeliveryMobile { get; set; }
        
        //Delivery Details
        public string Delivery_Agent { get; set; }
        public string DeliveryService { get; set; }
        //Payment Details
        public string DepositReference { get; set; }                //Same as Customer Reference (Unique)
        public string DepositAmount { get; set; }                   //Should be value of the order
        public string ProductCode { get; set; }
        public string TaxRateCode { get; set; }                     //VAT Code (Currently S/N but converted to iQ recognised VAT codes in Middleware)

        const long ReceiptType_Realex_ID = 19434740627053;

        public CashCustomer CorporateTrendsCashCustomer(string[] DownloadInfo, string CustomerOrderNo)
        {
            CashCustomer CashCustomer = new CashCustomer();
            Address Address = new Address();
            
            CashCustomer.Branch.ID = Branch.Branch_HQ_ID;

            try
            {
                AlternateReference = DownloadInfo[1];
                if (AlternateReference.Substring(0, 1) == "_") { CashCustomer.D_DivisionID = Division.Division_CPT_ID; }
                else { CashCustomer.D_DivisionID = Division.Division_LBY_ID; }

                //Contact Information
                CashCustomer.FirstName = DownloadInfo[12];
                CashCustomer.FullName = DownloadInfo[5].Replace(",", ".").ToTitleCase();
                CashCustomer.EmailAddress = DownloadInfo[12];
                CustomerPhone = DownloadInfo[29];
                if (String.IsNullOrEmpty(CustomerPhone)) { CustomerPhone = "01618710500"; }
                if (CustomerPhone.Replace(" ", "").Replace("#","").Substring(0, 2) == "07") { CustomerMobile = CustomerPhone; }
                CashCustomer.Phone = CustomerPhone;
                CashCustomer.Mobile = CustomerMobile;

                //Address Information
                Address.AddressLine1 = DownloadInfo[6];
                Address.AddressLine2 = DownloadInfo[7];
                Address.AddressLine3 = DownloadInfo[8];
                Address.AddressLine4 = DownloadInfo[9];
                CustomerCountry = DownloadInfo[38];
                if (CustomerCountry == "GB") { CustomerCountry = "UK"; }
                Address.Country.Code = CustomerCountry;
                Address.PostCode = DownloadInfo[10];

                List<CashCustomer> CashCustomers = CashCustomer.GetCashCustomers("[eq]", "EmailAddress", CashCustomer.EmailAddress);

                if (CashCustomers.Count > 0)
                {
                    Address.ID = CashCustomer.AddressID;
                    Address.UpdateAddress();

                    CashCustomer.ID = CashCustomers[0].ID;
                    CashCustomer.UpdateCashCustomer();
                }
                else
                {
                    CashCustomer.AddressID = Address.CreateAddress();
                    CashCustomer.CreateCashCustomer();
                }

                return CashCustomer;
            }
            catch (Exception e)
            {
                string message = "CPT-Cannot create cash customer - " + "CPT_" + CustomerOrderNo;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        /*public Customer_Contact CorporateTrendsCustomer_Contact(string[] DownloadInfo, string CustomerOrderNo)
        {
            Address Address = new Address();
            Customer_Contact Customer_Contact = new Customer_Contact();
            Customer Customer = new Customer();

            try
            {
                CustomerCode = DownloadInfo[4];
                CustomerTown = DownloadInfo[9].Replace(",", ".").ToTitleCase();
                CustomerCountry = DownloadInfo[11];
                CustomerEmail = DownloadInfo[13].Replace(",", ".");

                CustomerPhone = DownloadInfo[14].Replace(",", ".");
                if (CustomerPhone.Substring(0, 1) == "7") { CustomerPhone = '0' + CustomerPhone; }

                if (CustomerPhone.Substring(0, 2) == "07") { CustomerMobile = CustomerPhone; }

                if (CustomerCountry == "GB") { CustomerCountry = "UK"; }

                //Delivery Contact Details
                long CustomerID = 0;

                List<Customer> Customers = Customer.GetCustomers("[eq]", "Code", CustomerCode);

                foreach (Customer getCustomer in Customers)
                {
                    CustomerID = getCustomer.ID;
                }

                //Customer Contact Details
                Customer_Contact.LookupCustomer = CustomerID;

                Customer_Contact.UniqueID = CustomerEmail;
                Customer_Contact.D_ContactName = DownloadInfo[6].Replace(",", ".").ToTitleCase();
                Customer_Contact.ContactCompanyCode = CustomerCompanyName;

                Address.AddressLine1 = DownloadInfo[7].Replace(",", ".").ToTitleCase();
                Address.AddressLine2 = DownloadInfo[8].Replace(",", ".").ToTitleCase();
                Address.AddressLine3 = DownloadInfo[9].Replace(",", ".").ToTitleCase();
                Address.AddressLine4 = DownloadInfo[10].Replace(",", ".").ToTitleCase();
                Address.Country.Code = CustomerCountry;
                Address.PostCode = DownloadInfo[12].Replace(",", ".").ToUpper();

                Customer_Contact.EmailAddress = CustomerEmail;
                Customer_Contact.Phone = CustomerPhone;
                Customer_Contact.Mobile = CustomerMobile;

                List<Customer_Contact> Customer_Contacts = Customer_Contact.GetCustomer_Contacts("[eq]", "FirstName", CustomerEmail);

                if (Customer_Contacts.Count > 0)
                {
                    Address.ID = Customer_Contact.AddressID;
                    Address.UpdateAddress();

                    Customer_Contact.ID = Customer_Contacts[0].ID;
                    Customer_Contact.UpdateCustomer_Contact();
                }
                else
                {
                    Customer_Contact.AddressID = Address.CreateAddress();
                    Customer_Contact.CreateCustomer_Contact();
                }

                return Customer_Contact;
            }
            catch (Exception e)
            {
                string message = "CPT-Cannot create contact - " + "CPT_" + CustomerOrderNo;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }*/

        public Customer_DeliveryContact CorporateTrendsCustomer_DeliveryContact(string[] DownloadInfo, long CashCustomerID, string CustomerOrderNo)
        {
            Customer customer = new Customer();
            Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
            Address address = new Address();
            
            try
            {
                string ExchequerAccountCode = DownloadInfo[23];
                CustomerCode = MiscFunctions.GetAccountCode(ExchequerAccountCode);
                List<Customer> Customers = customer.GetCustomers("[eq]", "Code", CustomerCode);
                foreach (Customer getCustomer in Customers)
                {
                    customer.ID = getCustomer.ID;
                }

                customer_DeliveryContact.LookupCustomerID = customer.ID;
                customer_DeliveryContact.CashCustomerID = CashCustomerID;

                customer_DeliveryContact.UniqueID1 = DownloadInfo[12];
                customer_DeliveryContact.UniqueID2 = DownloadInfo[10];

                //Contact Information
                customer_DeliveryContact.D_ContactName = DownloadInfo[5];
                    //deliveryContact.ContactCompanyCode = DeliveryCompanyName;

                DeliveryPhone = DownloadInfo[29];
                if (String.IsNullOrEmpty(CustomerPhone)) { DeliveryPhone = "01618710500"; }
                else if (DeliveryPhone.Substring(0, 1) == "7") { DeliveryPhone = '0' + DeliveryPhone; }
                else if (DeliveryPhone.Substring(0, 2) == "07")
                {
                    DeliveryMobile = DeliveryPhone;
                    DeliveryPhone = "";
                }
                customer_DeliveryContact.Phone = DeliveryPhone;
                customer_DeliveryContact.Mobile = DeliveryMobile;
                customer_DeliveryContact.EmailAddress = DownloadInfo[12];

                //Address Information
                address.AddressLine1 = DownloadInfo[6];
                address.AddressLine2 = DownloadInfo[7];
                address.AddressLine3 = DownloadInfo[8];
                address.AddressLine4 = DownloadInfo[9];

                DeliveryCountry = DownloadInfo[38];
                if (DeliveryCountry == "GB") { DeliveryCountry = "UK"; }
                address.Country.Code = DeliveryCountry;

                address.PostCode = DownloadInfo[10];

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
                string message = "CPT-Cannot create delivery contact - " + "CPT_" + CustomerOrderNo;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public SalesOrder CorporateTrendsSalesOrder(string[] DownloadInfo, long CashCustomerID, long Customer_DeliveryContactID, bool CashOrder, string CustomerOrderNo)
        {
            SalesOrder salesOrder = new SalesOrder();
            Customer customer = new Customer();

            salesOrder.Branch.ID = Branch.Branch_HQ_ID;
            salesOrder.DespatchBranch.ID = Branch.Branch_HQ_ID;

            salesOrder.Source.ID = SalesOrderSource.OrderSource_CPT_ID;
            salesOrder.Currency.ID = Currency.Currency_GBP_ID;

            if (CashOrder == true)
            {
                salesOrder.CashCustomer.ID = CashCustomerID;
                salesOrder.OrderType.ID = SalesOrderType.OrderType_WebCashOrders_ID;
            }
            else
            {
                string staff = DownloadInfo[36];
                if (staff == "Staff") { salesOrder.OrderType.ID = SalesOrderType.OrderType_LECTURERS_ID; }
                else { salesOrder.OrderType.ID = SalesOrderType.OrderType_WebOrders_ID; }
            }
            salesOrder.DeliveryContact.ID = Customer_DeliveryContactID;

            try
            {
                AlternateReference = DownloadInfo[1];
                //salesOrder.Contact.ID = Customer_ContactID;
                string ExchequerAccountCode = DownloadInfo[23];
                CustomerCode = MiscFunctions.GetAccountCode(ExchequerAccountCode);
                salesOrder.Customer = customer.GetCustomerCode(CustomerCode);

                //Based on old system if customer account code ended in 52, 82 or 92 then it was dispatched to college not to student - This 
                //prevents delivery agent being selected & label produced
                string Bulk = ExchequerAccountCode.Substring(ExchequerAccountCode.Length - 2);
                bool BulkDeliveryCustomer;
                if (Bulk == "52" || Bulk == "82" || Bulk == "92") { BulkDeliveryCustomer = true; }
                else { BulkDeliveryCustomer = false; }

                try
                {
                    if (CustomerCode == "52113")
                    {
                        string SiteCode = AlternateReference.Substring(5, 5);
                        salesOrder.Site.Code = SiteCode;
                    }
                    else {  salesOrder.Site.Code = ""; }
                }
                catch 
                { 
                }
                
                if (AlternateReference.Substring(0, 1) == "_" && salesOrder.Customer.Code != "52113") { salesOrder.D_Division.ID = Division.Division_CPT_ID; }
                else { salesOrder.D_Division.ID = Division.Division_LBY_ID; }
         
                salesOrder.AlternateReference = "CPT_" + AlternateReference;
                salesOrder.Date = DownloadInfo[3];

                string TagNumber = DownloadInfo[22];
                if (TagNumber == "32") { TagNumber = "81"; }
                if (TagNumber == "91") { salesOrder.DueDate = (Convert.ToDateTime(DownloadInfo[24]).AddDays(14)).ToString(); }
                else {  salesOrder.DueDate = DownloadInfo[24]; }

                salesOrder.D_TagNumber = TagNumber;
                salesOrder.D_EmbroideryCode = DownloadInfo[20];

                Delivery_Agent = DownloadInfo[26];
                DeliveryService = DownloadInfo[26];
                
                if (BulkDeliveryCustomer == false)
                {
                    if (Delivery_Agent == "HBCAR")
                    {
                        salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_RM_ID;
                        salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_RM_ID;

                        salesOrder.DeliveryCostAmount = Convert.ToDecimal("2.26"); ;
                        salesOrder.DeliveryNetAmount = Convert.ToDecimal(DownloadInfo[33]);
                        salesOrder.DeliveryGrossAmount = Convert.ToDecimal(DownloadInfo[47]);
                    }
                    else if (Delivery_Agent == "CARND")
                    {
                        salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_UKM_ID;
                        salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_UKM_ID;

                        salesOrder.DeliveryNetAmount = Convert.ToDecimal(DownloadInfo[33]);
                        salesOrder.DeliveryGrossAmount = Convert.ToDecimal(DownloadInfo[47]);
                    }
                    else if (Delivery_Agent == "HBFIRST")
                    {
                        salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_RM_ID;
                        salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_RM1_ID;

                        salesOrder.DeliveryNetAmount = Convert.ToDecimal(DownloadInfo[33]);
                        salesOrder.DeliveryGrossAmount = Convert.ToDecimal(DownloadInfo[47]);
                    }
                    else if (Delivery_Agent == "GFSINT")
                    {
                        salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_GFS_ID;
                        salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_GFS_INT_ID;

                        salesOrder.DeliveryNetAmount = Convert.ToDecimal(DownloadInfo[33]);
                        salesOrder.DeliveryGrossAmount = Convert.ToDecimal(DownloadInfo[47]);
                    }
                    else
                    {
                        salesOrder.DeliveryAgent = null;
                        salesOrder.DeliveryAgentService = null;
                        salesOrder.DeliveryNetAmount = 0;
                        salesOrder.DeliveryGrossAmount = Convert.ToDecimal(DownloadInfo[47]);
                    }
                }
                else
                {
                    if (Delivery_Agent == "HBCAR" || Delivery_Agent == "CARND" || Delivery_Agent == "HBFIRST" || Delivery_Agent == "GFSINT")
                    {
                        salesOrder.DeliveryAgent.ID = DeliveryAgent.DeliveryAgent_TBC_ID;
                        salesOrder.DeliveryAgentService.ID = DeliveryAgent_Service.DeliveryAgentService_TBC_ID;

                        salesOrder.DeliveryNetAmount = Convert.ToDecimal(DownloadInfo[33]);
                        salesOrder.DeliveryGrossAmount = Convert.ToDecimal(DownloadInfo[47]);
                    }
                    else
                    {
                        salesOrder.DeliveryAgent = null;
                        salesOrder.DeliveryAgentService = null;

                        salesOrder.DeliveryNetAmount = 0;
                        salesOrder.DeliveryGrossAmount = Convert.ToDecimal(DownloadInfo[47]);
                    }
                }

                TaxRateCode = DownloadInfo[34];
                if (TaxRateCode == "S") { salesOrder.DeliveryTaxRate.ID = TaxRate.TaxRate_GB01_ID; }
                else { salesOrder.DeliveryTaxRate.ID = TaxRate.TaxRate_GB05_ID; }

                return salesOrder;
            }
            catch (Exception e)
            {
                string message = "CPT-Cannot create sales order - " + "CPT_" + CustomerOrderNo;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public SalesOrder_Line CorporateTrendsSalesOrder_Line(string[] DownloadInfo, string CustomerOrderNo)
        {
            SalesOrder_Line salesOrder_Line = new SalesOrder_Line();
            Product product = new Product();
            UnitOfMeasure sellingUnits = new UnitOfMeasure();

            try
            {
                ProductCode = DownloadInfo[31];
                if ((ProductCode.Substring(0, 3) != "BEH" && ProductCode.Substring(0, 2) != "HH" && ProductCode.Substring(0, 1) != "N" && ProductCode.Substring(0,4) != "LEIS") || ProductCode.Substring(0,3) == "NLL" || ProductCode.Substring(0,3) == "NLA") { ProductCode = "CPT-" + ProductCode; }
                salesOrder_Line.Product = product.GetProductCode(ProductCode);

                salesOrder_Line.CreatedDate = OrderDate;
                salesOrder_Line.SellingUnits = sellingUnits.GetUnitOfMeasureCode("Each");
              
                salesOrder_Line.D_EmbroideryCode = DownloadInfo[28];
                salesOrder_Line.D_Monogram = DownloadInfo[19];
                salesOrder_Line.D_ContactName = DownloadInfo[27];

                if (String.IsNullOrEmpty(DownloadInfo[32])) { salesOrder_Line.Quantity = 0; }
                else { salesOrder_Line.Quantity = Convert.ToDecimal(DownloadInfo[32]); }

                salesOrder_Line.NetPrice = Convert.ToDecimal(DownloadInfo[33]);
                salesOrder_Line.GrossPrice = Convert.ToDecimal(DownloadInfo[47]);

                if (DownloadInfo[34] == "S") { salesOrder_Line.TaxRate.ID = TaxRate.TaxRate_GB01_ID; }
                else { salesOrder_Line.TaxRate.ID = TaxRate.TaxRate_GB05_ID; }

                return salesOrder_Line;
            }
            catch (Exception e)
            {
                string message = "CPT-Cannot create sales order line - " + "CPT_" + CustomerOrderNo + " - " + ProductCode;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }

        public Receipt CorporateTrendsSalesOrderReceipt(string[] DownloadInfo, string CustomerOrderNo)
        {
            Receipt Receipt = new Receipt();

            try
            {
                DepositReference = AlternateReference;
                DepositAmount = DownloadInfo[48];

                Receipt.ReceiptType = ReceiptType_Realex_ID;

                Receipt.Amount = Convert.ToDecimal(DepositAmount);

                return Receipt;
            }
            catch (Exception e)
            {
                string message = "CPT-Cannot create sales order receipt - " + "CPT_" + CustomerOrderNo;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
    }
}
