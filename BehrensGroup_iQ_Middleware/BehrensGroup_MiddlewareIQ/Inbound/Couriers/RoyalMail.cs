/*
 * Author:      Ryan Curran
 * Date:        May 2020
 * Description: Class for Royal Mail Inbound Tasks
 *              Methods for Moving Royal Mail Consignment Numbers to ERP System using API.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.Couriers
{
    class RoyalMail
    {
        public static string EDIFileName { get; set; }
        public static string ConsignmentNumber { get; set; }
        public static string AlternateReference { get; set; }

        public static string SearchPath = @"\\APP01\SalesOrders\SheffieldCourier\RoyalMail\ImportRoyalMail\Results";

        public static void RoyalMailCourierMain()
        {
            DownloadSchemaRoyalMail download = new DownloadSchemaRoyalMail();

            try
            {
                string[] fileEntries = Directory.GetFiles(SearchPath, "*.*", SearchOption.AllDirectories);

                foreach (string fileName in fileEntries)
                {
                    List<string> list = new List<string>();

                    string filePath = fileName;
                    EDIFileName = Path.GetFileName(fileName).Replace(".csv", "");

                    string FilePath = SearchPath + "\\" + EDIFileName;

                    if (EDIFileName.Contains(".pdf"))
                    {
                        File.Delete(filePath);
                    }
                    else
                    {
                        using (CsvReader reader = new CsvReader(filePath))
                        {
                            foreach (string[] values in reader.RowEnumerator)
                            {
                                string headStart = values[0];
                                if (headStart.Length > 0 && headStart != "Order number")
                                {
                                    AlternateReference = values[2];
                                    ConsignmentNumber = values[5];

                                    SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();
                                    salesDeliveryNote = download.ReadDownloadInfoRoyalMail(values);

                                    //Check if Sales Delivery Note Exists
                                    SalesDeliveryNote existingSalesDeliveryNote = new SalesDeliveryNote();
                                    List<SalesDeliveryNote> existingSalesDeliveryNotes = existingSalesDeliveryNote.GetSalesDeliveryNotes("[eq]", "AlternateReference", salesDeliveryNote.AlternateReference);

                                    if (existingSalesDeliveryNotes.Count > 0)
                                    {
                                        salesDeliveryNote.ID = existingSalesDeliveryNotes[0].ID;
                                        salesDeliveryNote.UpdateSalesDeliveryNote();
                                    }

                                    string message = "RoyalMail-Successfully added consignment - " + AlternateReference + " - " + ConsignmentNumber;
                                    LogFile.WritetoLogFile(message, true);
                                    
                                }
                            }
                            File.Delete(filePath);
                        }
                    }
                    File.Delete(fileName);
                }
                Console.WriteLine("Inbound - Courier Information - Royal Mail Delivery Information Complete");
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "Cannot add consignment - " + ConsignmentNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
        }
    }

    class DownloadSchemaRoyalMail : GenericClassTransactionLine
    {
        public string OrderNo { get; set; }                    //Division
        public string Channel { get; set; }          //Customer Reference (Unique Per Order Web Generated No) 
        public string AlternateReference { get; set; }
        public string PrintedDate { get; set; }
        public string CustomerName { get; set; }
        public string TrackingNumber { get; set; }                     //Customer Reference (Unique Per Order Web Generated No)
        public string PackageSize { get; set; }

        public SalesDeliveryNote ReadDownloadInfoRoyalMail(string[] DownloadInfo)
        {
            SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();

            try
            {
                OrderNo = DownloadInfo[0];               //Should always be "RL"
                Channel = DownloadInfo[1];
                salesDeliveryNote.AlternateReference = DownloadInfo[2];
                salesDeliveryNote.CreatedDate = DownloadInfo[3];
                salesDeliveryNote.DeliveryContact.D_ContactName = DownloadInfo[4];
                salesDeliveryNote.DeliveryAgentTrackingNumber = DownloadInfo[5];
                PackageSize = DownloadInfo[6];
            }
            catch (Exception e)
            {
                string message = "RoyalMail-Cannot read delivery note - " + TrackingNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }

            return salesDeliveryNote;
        }
    }
}
