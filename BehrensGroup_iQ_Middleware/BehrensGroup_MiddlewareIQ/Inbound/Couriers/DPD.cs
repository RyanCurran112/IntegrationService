
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.Couriers
{
    class DPD
    {
        public static string EDIFileName { get; set; }
        public static string ConsignmentNumber { get; set; }
        public static string AlternateReference { get; set; }
        public static string SalesDeliveryNoteNo { get; set; }

        public static string SearchPath = @"\\APP01\SalesOrders\SheffieldCourier\DPD\ImportDPD\Complete";

        public static void DPDCourierMain()
        {
            DownloadSchemaDPD download = new DownloadSchemaDPD();

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
                            if (headStart.Length > 0)
                            {
                                SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();
                                salesDeliveryNote = download.ReadDownloadInfoDPD(values);

                                //Check if Sales Delivery Note Exists
                                SalesDeliveryNote existingSalesDeliveryNote = new SalesDeliveryNote();
                                List<SalesDeliveryNote> existingSalesDeliveryNotes = existingSalesDeliveryNote.GetSalesDeliveryNotes("[eq]", "Number", salesDeliveryNote.Number);

                                if (existingSalesDeliveryNotes.Count > 0)
                                {
                                    salesDeliveryNote.ID = existingSalesDeliveryNotes[0].ID;
                                    salesDeliveryNote.UpdateSalesDeliveryNote();
                                }

                                string message = "DPD-Successfully added consignment - " + salesDeliveryNote.Number + " - " + ConsignmentNumber;
                                LogFile.WritetoLogFile(message, true);

                            }
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

            Console.WriteLine("Inbound - Courier Information - DPD Delivery Infomation Complete");
        }
    }

    class DownloadSchemaDPD : GenericClassTransactionLine
    {
        public string Division { get; set; }                    //Division
        public string Dunno { get; set; }                   
        public string Dunno2 { get; set; }
        public string PrintedDate { get; set; }
        public string CustomerName { get; set; }
        public string TrackingNumber { get; set; }                     //Customer Reference (Unique Per Order Web Generated No)
        
        public SalesDeliveryNote ReadDownloadInfoDPD(string[] DownloadInfo)
        {
            SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();

            try
            {
                Division = DownloadInfo[0];               
                Dunno = DownloadInfo[1];
                salesDeliveryNote.Number = DownloadInfo[2];
                salesDeliveryNote.DeliveryAgentTrackingNumber = DownloadInfo[3];
            }
            catch (Exception e)
            {
                string message = "DPD-Cannot read delivery note - " + DownloadInfo[3];
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }

            return salesDeliveryNote;
        }
    }
}
