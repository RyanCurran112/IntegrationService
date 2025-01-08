/*
 * Author:      Ryan Curran
 * Date:        September 2019
 * Description: Class for DPD Outbound Tasks
 *              Methods for Moving DPD Consignments File to DPD folder in correct format.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Outbound.Couriers
{
    class DPD
    {
        public static string OriginalPath = @"\\APP01\SalesOrders\SheffieldCourier\DPD\";
        public static string TempPath = @"\\APP01\SalesOrders\SheffieldCourier\DPD\DailyExportDPD.csv";
        public static string DestinationPath = @"\\APP01\SalesOrders\SheffieldCourier\DPD\ImportDPD\DailyExportDPD.csv";

        public static void MoveDPDFile()
        {
            string[] fileEntries = Directory.GetFiles(OriginalPath);

            string Division;
            string AlternateReference;
            string Number;
            string DeliveryInstructions;
            string Date;
            string TotalNetWeight;
            string DeliveryAgentService;
            string ContactName;
            string ContactPhone;
            string ContactEmail;
            string AddressLine1;
            string AddressLine2;
            string AddressLine3;
            string AddressLine4;
            string Postcode;
            string NoItems;
            string CompanyName;
            string ProfileID;

            string CurrentDateTime = Convert.ToString(DateTime.Now);
            CurrentDateTime = CurrentDateTime.Replace("/", ".");
            CurrentDateTime = CurrentDateTime.Replace(":", ".");

            try
            {
                foreach (string fileName in fileEntries)
                {
                    string filePath = fileName;

                    using (CsvReader reader = new CsvReader(filePath))
                    {
                        foreach (string[] values in reader.RowEnumerator)
                        {
                            string headStart = values[0];
                            if (headStart == "D_Division")
                            {
                                StringBuilder myString = new StringBuilder();

                                foreach (string i in values)
                                {
                                    myString.Append(i);
                                    myString.Append(",");
                                }
                                myString.Length--;

                                StreamWriter file = new StreamWriter(TempPath, true);
                                file.WriteLine(myString);
                                file.Close();
                            }
                            else
                            {
                                if (headStart.Length > 0)
                                {
                                    StringBuilder myString = new StringBuilder();

                                    Division = values[0];
                                    AlternateReference = values[1];
                                    Number = values[2];
                                    DeliveryInstructions = values[3];
                                    Date = values[4];
                                    TotalNetWeight = values[5];
                                    DeliveryAgentService = values[6];
                                    ContactName = values[7];
                                    ContactPhone = values[8];
                                    ContactEmail = values[9];
                                    AddressLine1 = values[10];
                                    AddressLine2 = values[11];
                                    AddressLine3 = values[12];
                                    AddressLine4 = values[13];
                                    Postcode = values[14];
                                    NoItems = values[15];
                                    CompanyName = values[16];
                                    ProfileID = Division;

                                    myString.Append(Division);
                                    myString.Append(",");
                                    myString.Append(AlternateReference);
                                    myString.Append(",");
                                    myString.Append(Number);
                                    myString.Append(",");
                                    myString.Append("\"" + DeliveryInstructions + "\"");
                                    myString.Append(",");
                                    myString.Append(Date);
                                    myString.Append(",");
                                    myString.Append(TotalNetWeight);
                                    myString.Append(",");
                                    myString.Append(DeliveryAgentService);
                                    myString.Append(",");
                                    myString.Append("\"" + ContactName + "\"");
                                    myString.Append(",");
                                    myString.Append(ContactPhone);
                                    myString.Append(",");
                                    myString.Append("\"" + ContactEmail + "\"");
                                    myString.Append(",");
                                    myString.Append("\"" + AddressLine1 + "\"");
                                    myString.Append(",");
                                    myString.Append("\"" + AddressLine2 + "\"");
                                    myString.Append(",");
                                    myString.Append("\"" + AddressLine3 + "\"");
                                    myString.Append(",");
                                    myString.Append("\"" + AddressLine4 + "\"");
                                    myString.Append(",");
                                    myString.Append("\"" + Postcode + "\"");
                                    myString.Append(",");

                                    if (Convert.ToInt32(Convert.ToDouble(NoItems)) == 0)
                                    {
                                        NoItems = "1";
                                    }

                                    myString.Append(NoItems);
                                    myString.Append(",");
                                    myString.Append("\"" + CompanyName + "\"");
                                    myString.Append(",");
                                    myString.Append("\"" + ProfileID + "\"");

                                    StreamWriter file = new StreamWriter(TempPath, true);
                                    file.WriteLine(myString);
                                    file.Close();
                                }
                            }
                        }
                    }
                }
                File.Move(OriginalPath + "DailyExport.csv", OriginalPath + "Complete\\DailyExport " + CurrentDateTime + ".csv");
                File.Move(TempPath, DestinationPath);

                string message1 = "DPD Feed Complete";
                LogFile.WritetoLogFile(message1, true);
            }
            catch (Exception e)
            {
                string message1 = "DPD, No files to move";
                LogFile.WritetoLogFile(message1, true);
                Console.WriteLine(e.Message);
            }
        }
    }
}
