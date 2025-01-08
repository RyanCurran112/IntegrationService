/*
 * Author:      Ryan Curran
 * Date:        September 2019
 * Description: Class for DX Inbound Tasks
 *              Methods for Moving DX Consignment Numbers to ERP System
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
    class DeliveringExactly
    {
        public static string EDIFileName { get; set; }
        public static string ConsignmentNumber { get; set; }
        public static string Number { get; set; }

        public static string SearchPath = @"\\APP01\SalesOrders\SheffieldCourier\DX\Export";

        public static void DeliveringExactlyCourierMain()
        {
            DownloadSchemaDeliveringExactly download = new DownloadSchemaDeliveringExactly();

            try
            {
                string[] fileEntries = Directory.GetFiles(SearchPath, "*.*", SearchOption.AllDirectories);

                foreach (string fileName in fileEntries)
                {
                    List<string> list = new List<string>();

                    string filePath = fileName;
                    EDIFileName = Path.GetFileName(fileName).Replace(".csv", "");
                    string FilePath = SearchPath + "\\" + EDIFileName;

                    using (CsvReader reader = new CsvReader(filePath))
                    {
                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            if (headStart.Replace("\0","").Length > 0)
                            {

                                ConsignmentNumber = values[0].Replace("\0","");

                                SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();
                                salesDeliveryNote = download.ReadDownloadInfoDeliveringExactly(values);

                                //Check if Sales Delivery Note Exists
                                SalesDeliveryNote existingSalesDeliveryNote = new SalesDeliveryNote();
                                List<SalesDeliveryNote> existingSalesDeliveryNotes = existingSalesDeliveryNote.GetSalesDeliveryNotes("[eq]", "Number", salesDeliveryNote.Number);

                                if (existingSalesDeliveryNotes.Count > 0)
                                {
                                    salesDeliveryNote.ID = existingSalesDeliveryNotes[0].ID;
                                    salesDeliveryNote.UpdateSalesDeliveryNote();
                                }

                                string message = "Successfully added consignment - " + Number + " - " + ConsignmentNumber;
                                LogFile.WritetoLogFile(message, true);

                            }
                            
                        }
                        File.Delete(fileName);
                    }
                    string message1 = "DX-Consignment Note Details Import Complete";
                    LogFile.WritetoLogFile(message1, true);
                }

            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "DX-Cannot add consignment - " + ConsignmentNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
            Console.WriteLine("Inbound - Courier Information - DX Deliveries Complete");
        }
    }

    class DownloadSchemaDeliveringExactly : GenericClassTransactionLine
    {
        public string OrderNo { get; set; }                    //Division
        public string Channel { get; set; }          //Customer Reference (Unique Per Order Web Generated No) 
        public string AlternateReference { get; set; }
        public string PrintedDate { get; set; }
        public string CustomerName { get; set; }
        public string TrackingNumber { get; set; }                     //Customer Reference (Unique Per Order Web Generated No)
        public string PackageSize { get; set; }

        public SalesDeliveryNote ReadDownloadInfoDeliveringExactly(string[] DownloadInfo)
        {
            SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();

            try
            {
                string DeliveryNoteNumber = "";

                try
                {
                    salesDeliveryNote.Number = DownloadInfo[1].Replace("\0", "").Substring(0, 12);  //Should always be "SDN Number - Division"
                    DeliveryNoteNumber = DownloadInfo[1].Replace("\0", "").Substring(11, 1);
                }
                catch
                { 
                    salesDeliveryNote.Number = DownloadInfo[1].Replace("\0", "");
                    DeliveryNoteNumber = DownloadInfo[1].Replace("\0", "");
                }

                if (DeliveryNoteNumber != "-")
                {
                    try
                    {
                        salesDeliveryNote.Number = DownloadInfo[1].Replace("\0", "").Substring(0, 12);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    salesDeliveryNote.Number = DownloadInfo[1].Replace("\0", "").Substring(0, 11);
                }

                salesDeliveryNote.DeliveryAgentTrackingNumber = DownloadInfo[0].Replace("\0", "");
            }
            catch (Exception e)
            {
                string message = "DX-Cannot read delivery note - " + TrackingNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }

            return salesDeliveryNote;
        }
    }
}
