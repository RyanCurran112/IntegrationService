/*
 * Author:      Ryan Curran
 * Date:        September 2019
 * Description: Class for Royal Mail Outbound Tasks
 *              Methods for Moving Royal Mail Consignments File to DropBox folder in correct format.
 * ToDo:        Future Work to use Royal Mail Desktop App on APP server
  */ 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Outbound.Couriers
{
    class RoyalMail
    {
        public static string OriginalPath = @"\\APP01\SalesOrders\SheffieldCourier\RoyalMail\ImportRoyalMail\";
        public static string TempPath = @"\\APP01\SalesOrders\SheffieldCourier\RoyalMail\ImportRoyalMail\DailyExportRM.csv";
        
        public static string DestinationPath;

        public static void MoveRoyalMailFile()
        {
            string computerName = Environment.MachineName;
            if (computerName == "APP01")
            {
                DestinationPath  = @"C:\Users\rcurran\Dropbox\Apps\Click and Drop\DailyExport.csv";
            }
            else
            {
                DestinationPath = @"C:\Users\rcurran.BEHRENS\Dropbox\Apps\Click and Drop\DailyExport.csv";
            }

            string[] fileEntries = Directory.GetFiles(OriginalPath);

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
                            if (headStart == "AlternateReference")
                            {
                                StringBuilder myString = new StringBuilder();

                                foreach (string i in values)
                                {
                                    myString.Append(i);
                                    myString.Append(",");
                                }
                                myString.Append("PackageType");

                                StreamWriter file = new StreamWriter(TempPath, true);
                                file.WriteLine(myString);
                                file.Close();
                            }
                            else
                            {
                                if (headStart.Length > 0)
                                {
                                    StringBuilder myString = new StringBuilder();

                                    myString.Append(values[0]);
                                    myString.Append(",");
                                    myString.Append("\"" + values[1] + "\"");
                                    myString.Append(",");
                                    myString.Append(values[2]);
                                    myString.Append(",");
                                    myString.Append(values[3]);
                                    myString.Append(",");
                                    myString.Append(values[4]);
                                    myString.Append(",");
                                    myString.Append("\"" + values[5] + "\"");
                                    myString.Append(",");
                                    myString.Append(values[6]);
                                    myString.Append(",");
                                    myString.Append(values[7]);
                                    myString.Append(",");
                                    myString.Append("\"" + values[8] + "\"");
                                    myString.Append(",");
                                    myString.Append("\"" + values[9] + "\"");
                                    myString.Append(",");
                                    myString.Append("\"" + values[10] + "\"");
                                    myString.Append(",");
                                    myString.Append("\"" + values[11] + "\"");
                                    myString.Append(",");
                                    myString.Append(values[12]);
                                    myString.Append(",");
                                    myString.Append(values[13]);
                                    myString.Append(",");
                                    myString.Append("\"" + values[14] + "\"");
                                    myString.Append(",");
                                    myString.Append(values[15]);
                                    myString.Append(",");
                                    myString.Append(Convert.ToInt32(Convert.ToDouble(values[16])));
                                    myString.Append(",");
                                    myString.Append(values[17]);
                                    myString.Append(",");
                                    myString.Append("Parcel");

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

                string message1 = "Royal Mail Feed Complete";
                LogFile.WritetoLogFile(message1, true);
            }
            catch (Exception e)
            {
                string message1 = "Royal Mail, No files to move";
                LogFile.WritetoLogFile(message1, true);
                Console.WriteLine(e.Message);
            }
        }
    }
}
