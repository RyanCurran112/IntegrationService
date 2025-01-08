/*
 * Author:      Ryan Curran
 * Date:        Feb 2021
 * Description: Class for Delivery Contacts Inbound Tasks
 *              Methods for Updating Default Delivery Agents & Services in ERP System using API.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.Couriers
{
    class DeliveryContactDefaultDeliveryAgents
    {
        public static string EDIFileName { get; set; }
        public static string ConsignmentNumber { get; set; }
        public static string AlternateReference { get; set; }

        public static string SearchPath = @"\\APP01\SalesOrders\SheffieldCourier\DeliveryContacts\";

        public static void DeliveryContactDefaultDeliveryAgentsMain()
        {
            DownloadSchemaDeliveryContacts download = new DownloadSchemaDeliveryContacts();

            try
            {
                string[] fileEntries = Directory.GetFiles(SearchPath, "*.*", SearchOption.AllDirectories);

                foreach (string fileName in fileEntries)
                {
                    string filePath = fileName;
                    EDIFileName = Path.GetFileName(fileName).Replace(".csv", "");

                    string FilePath = SearchPath + "\\" + EDIFileName;

                    using (CsvReader reader = new CsvReader(filePath))
                    {
                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            if (headStart.Length > 0 && headStart != "DeliveryContactID")
                            {

                                Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();
                                customer_DeliveryContact = download.ReadDownloadInfoDeliveryContacts(values);

                                customer_DeliveryContact.UpdateCashCustomer_DeliveryContact();

                                // string message = "Customer Delivery Contact-Successfully updated DeliveryContact - " + AlternateReference + " - " + ConsignmentNumber;
                                //LogFile.WritetoLogFile(message, true);

                            }
                        }
                        File.Delete(filePath);
                    }
                    File.Delete(fileName);
                }
                //Console.WriteLine("Inbound - Courier Information - Royal Mail Delivery Information Complete");
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "Cannot update delivery contact";
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
        }
    }

    class DownloadSchemaDeliveryContacts : GenericClassTransactionLine
    {
        public string DeliveryContactID { get; set; } 
        public string DeliveryAgentID { get; set; }          
        public string DeliveryAgentServiceID { get; set; }

        public Customer_DeliveryContact ReadDownloadInfoDeliveryContacts(string[] DownloadInfo)
        {
            Customer_DeliveryContact customer_DeliveryContact = new Customer_DeliveryContact();

            try
            {
                DeliveryContactID = DownloadInfo[0];

                customer_DeliveryContact.ID = long.Parse(DownloadInfo[0]);
                customer_DeliveryContact.DefaultDeliveryAgentID = long.Parse(DownloadInfo[1]);
                customer_DeliveryContact.DefaultDeliveryAgentServiceID = long.Parse(DownloadInfo[2]);

            }
            catch (Exception e)
            {
                string message = "CustomerDeliveryContact-Cannot read delivery contact - " + DeliveryContactID;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }

            return customer_DeliveryContact;
        }
    }
}
