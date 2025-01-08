
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
    class CashCustomerOutput
    {
        public static string EDIFileName { get; set; }
        public static string CustomerOrderNumber { get; set; }

        public static string OriginalPath = @"\\APP01\SalesOrders\CashCustomers\iQCashCustomerContacts";
        public static string path = @"\\APP01\SalesOrders\CashCustomers";

        public static void CashCustomerDetailsMain()
        {

            DownloadSchemaCashCustomerDetails download = new DownloadSchemaCashCustomerDetails();

            string CurrentDateTime = DateTime.Now.ToString();
            CurrentDateTime = CurrentDateTime.Replace("/", "-");
            CurrentDateTime = CurrentDateTime.Replace(":", "-");

            try
            {
                string[] fileEntries = Directory.GetFiles(OriginalPath);
                int i = 1;
                foreach (string fileName in fileEntries)
                {
                    File.Move(fileName, path + @"\cashcustomer-export-" + CurrentDateTime + i + ".csv");
                    i++;
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.ToString());
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
                            if (headStart.Length > 0 && headStart == "ORDER")
                            {
                                Customer_DeliveryContact deliveryContact = new Customer_DeliveryContact();
                                Customer_Contact customerContact = new Customer_Contact();
                                SalesOrder salesOrderHeader = new SalesOrder();
                                SalesOrder_Line salesOrderLine = new SalesOrder_Line();

                                download.ReadCashCustomerDetails(values, deliveryContact, customerContact, salesOrderHeader);

                                string DeliveryContactPath = newpath + " - DeliveryContact.csv";
                                string CustomerContactPath = newpath + " - CustomerContact.csv";
                                string SalesOrderHeaderPath = path + @"\iQCashCustomerContacts\OrderDetails\" + EDIFileName + " - SalesOrder.csv";

                                //deliveryContact.WriteDeliveryContactSchema(DeliveryContactPath);
                                //customerContact.WriteCustomerContactSchema(CustomerContactPath);
                                //salesOrderHeader.WriteSalesOrderHeaderSchema(salesOrderHeader, SalesOrderHeaderPath);
                            }
                        }
                    }

                    File.Move(filePath, path + @"\Complete\" + EDIFileName + ".csv");

                    File.Move(newpath + " - DeliveryContact.csv", path + @"\DeliveryContacts\" + EDIFileName + " - DeliveryContact.csv");
                    File.Move(newpath + " - CustomerContact.csv", path + @"\Contacts\" + EDIFileName + " - CustomerContact.csv");
                    //File.Move(newpath + ".csv", path + @"\iQCashCustomerContacts\OrderDetails\" + EDIFileName + " - SalesOrder.csv");

                    string message = "Successfully added order - " + CustomerOrderNumber;
                    LogFile.WritetoLogFile(message, true);
                }
                string message1 = "CashCustomer File Import Complete";
                LogFile.WritetoLogFile(message1, true);
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "Cannot read order - " + CustomerOrderNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
        }
    }

    class DownloadSchemaCashCustomerDetails : GenericClassTransactionLine
    {
        public string ImportType { get; set; }                  //Order Line "ORDER"
        public string OrderNumber { get; set; }                 //Customer Reference (Unique Per Order Web Generated No) 
        public string AccountCode { get; set; }                 //Web Order "WEB"
        public string Branch { get; set; }                      //Date Order Placed - "dd/mm/yyyy"

        //Customer/CashCustomer Details
        public string CustomerFullName { get; set; }            //Customer Reference (Unique Per Order Web Generated No)
        public string CustomerCompanyName { get; set; }         //Customer Name (First and Last names not captured seperately)
        public string CustomerStreet1 { get; set; }             //DeliveryAddressLine1
        public string CustomerStreet2 { get; set; }             //DeliveryAddressLine2
        public string CustomerTown { get; set; }                //DeliveryAddressLine3
        public string CustomerCounty { get; set; }              //DeliveryAddressLine4
        public string CustomerCountry { get; set; }
        public string CustomerPostcode { get; set; }            //DeliveryPostcode          (Currently Delivery & Customer Address are not passed seperately)
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerMobile { get; set; }

        public void ReadCashCustomerDetails(string[] DownloadInfo, Customer_DeliveryContact deliveryContact, Customer_Contact customerContact, SalesOrder salesOrderHeader)
        {
            Customer_Contact customer_Contact = new Customer_Contact();
            Address address = new Address();

            try
            {
                ImportType = DownloadInfo[0];                           //Should always be "ORDER"
                OrderNumber = DownloadInfo[1].Replace(" ", "");

                AccountCode = DownloadInfo[2].Replace(" ", "");

                CustomerCompanyName = DownloadInfo[3].Replace(",", ".").ToTitleCase();
                Branch = DownloadInfo[4];
                CustomerFullName = DownloadInfo[5].Replace(",", ".").ToTitleCase();

                CustomerStreet1 = DownloadInfo[6].Replace(",", ".").ToTitleCase();
                CustomerStreet2 = DownloadInfo[7].Replace(",", ".").ToTitleCase();
                CustomerTown = DownloadInfo[8].Replace(",", ".").ToTitleCase();
                CustomerCounty = DownloadInfo[9].Replace(",", ".").ToTitleCase();
                CustomerCountry = DownloadInfo[10];
                CustomerPostcode = DownloadInfo[11].Replace(",", ".").ToUpper();
                CustomerEmail = DownloadInfo[12].Replace(",", ".");

                CustomerPhone = DownloadInfo[13].Replace("#", "");
                if (CustomerPhone.Substring(0, 1) == "7")
                {
                    CustomerPhone = '0' + CustomerPhone;
                }

                if (CustomerPhone.Substring(0, 2) == "07")
                {
                    CustomerMobile = CustomerPhone;
                }

                //Customer Contact Details
                customer_Contact.Customer.Code = AccountCode;

                customer_Contact.FirstName = CustomerEmail;
                customer_Contact.D_ContactName = CustomerFullName;
                customer_Contact.ContactCompanyCode = CustomerCompanyName;

                address.AddressLine1 = CustomerStreet1;
                address.AddressLine2 = CustomerStreet2;
                address.AddressLine3 = CustomerTown;
                address.AddressLine4 = CustomerCounty;
                address.Country.Code = CustomerCountry;
                address.PostCode = CustomerPostcode;

                customerContact.EmailAddress = CustomerEmail;
                customerContact.Phone = CustomerPhone;
                customerContact.Mobile = CustomerMobile;
                customerContact.CustomerContactRoleCode = "Professional";
                customerContact.EmailMarketingAllowed = "No";

                //Delivery Contact Details
                deliveryContact.Customer.Code = AccountCode;
                deliveryContact.UniqueID1 = CustomerEmail;
                deliveryContact.UniqueID2 = CustomerPostcode.Replace(" ", "");
                deliveryContact.FullName = CustomerFullName;
                deliveryContact.Customer.Code = CustomerCompanyName;

                address.AddressLine1 = CustomerStreet1;
                address.AddressLine2 = CustomerStreet2;
                address.AddressLine3 = CustomerTown;
                address.AddressLine4 = CustomerCounty;
                address.Country.Code = CustomerCountry;
                address.PostCode = CustomerPostcode;

                deliveryContact.EmailAddress = CustomerEmail;
                deliveryContact.Phone = CustomerPhone;
                deliveryContact.Mobile = CustomerMobile;
                //deliveryContact.Role = "Professional";

                //Sales Order Header Details

                salesOrderHeader.Number = OrderNumber;

                ///salesOrderHeader.OrderDeliveryContactUniqueID1 = deliveryContact.UniqueID1;
                //salesOrderHeader.OrderDeliveryContactUniqueID2 = deliveryContact.UniqueID2;
                //salesOrderHeader.OrderContactUniqueID1 = customerContact.UniqueID1;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
