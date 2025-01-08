
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.Couriers
{
    class GFS
    {
        public static string EDIFileName { get; set; }
        public static string ConsignmentNumber { get; set; }

        public static string path = @"\\APP01\SalesOrders\SheffieldCourier\GFS\Export";

        public static void GFSCourierMain()
        {
            DownloadSchemaGFS download = new DownloadSchemaGFS();

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
                            if (headStart.Length > 0)
                            {
                                SalesDeliveryNote salesDeliveryNoteHeader = new SalesDeliveryNote();

                                download.ReadDownloadInfoGFS(values, salesDeliveryNoteHeader);

                                string SalesDeliveryNotePath = newpath + " - ConsignmentDetails.csv";

                                //salesDeliveryNoteHeader.WriteSalesDeliveryNoteHeader(SalesDeliveryNotePath);
                            }
                        }
                    }

                    File.Move(filePath, path + @"\Complete\" + EDIFileName);

                    File.Move(newpath + " - ConsignmentDetails.csv", @"\\APP01\SalesOrders\SheffieldCourier\TrackingNumbers\" + EDIFileName + " - ConsignmentDetails.csv");

                    string message = "Successfully added consignment - " + ConsignmentNumber;
                    LogFile.WritetoLogFile(message, true);
                }
                string message1 = "Consignment Note Details Import Complete";
                LogFile.WritetoLogFile(message1, true);
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "Cannot read consignment - " + ConsignmentNumber;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
            Console.WriteLine("Inbound - Courier Information - GFS Delivery Infomation Complete");
        }
    }

    class DownloadSchemaGFS : GenericClassTransactionLine
    {
        public string RecordType { get; set; }                  //Order Line "RL"
        public string ConsignmentNo { get; set; }          //Customer Reference (Unique Per Order Web Generated No) 
        public string ContractNo { get; set; }                   //Web Order "WEB"
        public string Carrier { get; set; }                   //Date Order Placed - "dd/mm/yyyy"
        public string Service { get; set; }                 //Customer Reference (Unique Per Order Web Generated No)
        public string PrintedDate { get; set; }
        public string DespatchDate { get; set; }
        public string Packs { get; set; }

        //Customer/CashCustomer Details
        public string DeliveryFirstName { get; set; }
        public string DeliveryLastName { get; set; }
        public string DeliveryFullName { get; set; }
        public string DeliveryCompanyName { get; set; }
        public string DeliveryStreet1 { get; set; }
        public string DeliveryStreet2 { get; set; }
        public string DeliveryTown { get; set; }
        public string DeliveryCounty { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryPostcode { get; set; }

        public string ShipmentReference { get; set; }
        public string ConsignmentReference { get; set; }
        public string Weight { get; set; }
        public string Station { get; set; }

        public void ReadDownloadInfoGFS(string[] DownloadInfo, SalesDeliveryNote salesDeliveryNoteHeader)
        {
            RecordType = DownloadInfo[0];               //Should always be "RL"
            ConsignmentNo = DownloadInfo[1];
            ContractNo = DownloadInfo[2];
            Carrier = DownloadInfo[3];
            Service = DownloadInfo[4];
            PrintedDate = DownloadInfo[5];
            DespatchDate = DownloadInfo[6];
            Packs = DownloadInfo[7];

            DeliveryFullName = DownloadInfo[8].Replace(",", ".");
            DeliveryCompanyName = DownloadInfo[9].Replace(",", ".").ToTitleCase();                     //Need to get field for 
            DeliveryStreet1 = DownloadInfo[10].Replace(",", ".").ToTitleCase();
            DeliveryStreet2 = DownloadInfo[11].Replace(",", ".").ToTitleCase();
            DeliveryTown = DownloadInfo[12].Replace(",", ".").ToTitleCase();
            DeliveryCounty = DownloadInfo[13].Replace(",", ".").ToTitleCase();
            DeliveryCountry = DownloadInfo[15];
            DeliveryPostcode = DownloadInfo[14].Replace(",", ".").ToUpper();

            ShipmentReference = DownloadInfo[16];
            ConsignmentReference = DownloadInfo[17];

            //Cash Customer Details
            salesDeliveryNoteHeader.Number = ConsignmentReference;
            salesDeliveryNoteHeader.ConsignmentNo = ConsignmentNo;

        }
    }
}
