/*
 * Author:      Ryan Curran
 * Date:        August 2020
 * Description: Class for Parcelforce Inbound Tasks
 *              Methods for Moving Parcelforce Consignment Numbers to ERP System using API.
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
    class Parcelforce
    {
        public static string EDIFileName { get; set; }
        public static string ConsignmentNumber { get; set; }
        public static string Number { get; set; }

        public static string SearchPath = @"\\APP01\SalesOrders\SheffieldCourier\Parcelforce\Export";

        public static void ParcelforceCourierMain()
        {
            DownloadSchemaParcelforce download = new DownloadSchemaParcelforce();

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

                                    SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();
                                    salesDeliveryNote = download.ReadDownloadInfoParcelforce(values);

                                    //Check if Sales Delivery Note Exists
                                    SalesDeliveryNote existingSalesDeliveryNote = new SalesDeliveryNote();
                                    List<SalesDeliveryNote> existingSalesDeliveryNotes = existingSalesDeliveryNote.GetSalesDeliveryNotes("[eq]", "Number", salesDeliveryNote.Number);

                                    if (existingSalesDeliveryNotes.Count > 0)
                                    {
                                        salesDeliveryNote.ID = existingSalesDeliveryNotes[0].ID;

                                        if (String.IsNullOrEmpty(existingSalesDeliveryNote.DeliveryAgentTrackingNumber))
                                        {    
                                            salesDeliveryNote.UpdateSalesDeliveryNote();
                                        }
                                        
                                    }

                                    string message = "Parcelforce-Successfully added consignment - " + Number + " - " + ConsignmentNumber;
                                    LogFile.WritetoLogFile(message, true);

                                }
                            }
                            File.Delete(filePath);
                        }
                    }
                    File.Delete(fileName);
                }
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "Cannot add consignment - " + ConsignmentNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
            Console.WriteLine("Inbound - Courier Information - Parcelforce Delivery Infomation Complete");
        }
    }

    class DownloadSchemaParcelforce : GenericClassTransactionLine
    {
        public string AddressLine1 { get; set; }                    //Division
        public string AddressLine2 { get; set; }                    //Division
        public string AddressLine3 { get; set; }                    //Division
        public string AddressLine4 { get; set; }                    //Division
        public string City { get; set; }                            //Division
        public string Postcode { get; set; }                        //Division

        public string Service { get; set; }                         //Customer Reference (Unique Per Order Web Generated No) 
        public string TrackingNumber { get; set; }                  //Customer Reference (Unique Per Order Web Generated No)

        public string ParcelCount { get; set; }
        public string AlternateReference { get; set; }              //Customer Reference (Unique Per Order Web Generated No)

        public SalesDeliveryNote ReadDownloadInfoParcelforce(string[] DownloadInfo)
        {
            SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();

            try
            {
                salesDeliveryNote.Number = DownloadInfo[12];
                salesDeliveryNote.DeliveryAgentTrackingNumber = DownloadInfo[15];
            }
            catch (Exception e)
            {
                string message = "Parcelforce-Cannot read delivery note - " + TrackingNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                salesDeliveryNote.Number = "";
               
                return salesDeliveryNote;
            }

            return salesDeliveryNote;
        }
    }
}
