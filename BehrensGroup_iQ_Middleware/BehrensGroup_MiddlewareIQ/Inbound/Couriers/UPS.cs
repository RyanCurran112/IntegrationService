
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.Couriers
{
    class UPS
    {
        public static string EDIFileName { get; set; }
        public static string ConsignmentNumber { get; set; }
        public static string Number { get; set; }

        public static string SearchPath = @"\\APP01\SalesOrders\SheffieldCourier\UPS\Export\";
        public static string fileName = @"\\APP01\SalesOrders\SheffieldCourier\UPS\Export\UPS.csv";

        public static void UPSCourierMain()
        {
            DownloadSchemaUPS download = new DownloadSchemaUPS();

            try
            {
                string[] fileEntries = Directory.GetFiles(SearchPath, "*.*", SearchOption.AllDirectories);

                EDIFileName = Path.GetFileName(fileName).Replace(".csv", "");

                string FilePath = SearchPath + "\\" + EDIFileName;

                using (CsvReader reader = new CsvReader(fileName))
                {
                    foreach (string[] values in reader.RowEnumerator)
                    {
                        string headStart = values[0];
                        if (headStart.Length > 0)
                        {
                            Number = values[1]; 
                            ConsignmentNumber = values[2];

                            SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();
                            salesDeliveryNote = download.UPSTrackingInformation(values);

                            //Check if Sales Delivery Note Exists
                            SalesDeliveryNote existingSalesDeliveryNote = new SalesDeliveryNote();
                            List<SalesDeliveryNote> existingSalesDeliveryNotes = existingSalesDeliveryNote.GetSalesDeliveryNotes("[eq]", "Number", salesDeliveryNote.Number);

                            if (existingSalesDeliveryNotes.Count > 0)
                            {
                                salesDeliveryNote.ID = existingSalesDeliveryNotes[0].ID;
                                salesDeliveryNote.UpdateSalesDeliveryNote();
                            }

                            string message = "UPS-Successfully added consignment - " + Number + " - " + ConsignmentNumber;
                            LogFile.WritetoLogFile(message, true);
                        }
                    }
                }

                File.Delete(fileName);
                File.Create(fileName);

                StringBuilder sb = new StringBuilder();
                sb.Append("Success,Number,TrackingNumber");

                StreamWriter file = new StreamWriter(fileName, true);
                file.WriteLine(sb);
                file.Close();
            }
            catch (Exception e)
            {
                //Write to Log File
                //string message = "UPS-Cannot add consignment - " + Number + " - " + ConsignmentNumber;
                Console.WriteLine(e.Message);
                //LogFile.WritetoLogFile(message, false);
            }
            Console.WriteLine("Inbound - Courier Information - UPS Delivery Information Complete");
        }
    }

    class DownloadSchemaUPS : GenericClassTransactionLine
    {
        public string Success { get; set; }                    //Division
        public string TrackingNumber { get; set; }                     //Customer Reference (Unique Per Order Web Generated No)

        public SalesDeliveryNote UPSTrackingInformation(string[] DownloadInfo)
        {
            SalesDeliveryNote salesDeliveryNote = new SalesDeliveryNote();

            try
            {
                salesDeliveryNote.Number = DownloadInfo[1]; 
                salesDeliveryNote.DeliveryAgentTrackingNumber = DownloadInfo[2]; 
            }
            catch (Exception e)
            {
                string message = "UPS-Cannot read delivery note - " + TrackingNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }

            return salesDeliveryNote;
        }
    }
}
