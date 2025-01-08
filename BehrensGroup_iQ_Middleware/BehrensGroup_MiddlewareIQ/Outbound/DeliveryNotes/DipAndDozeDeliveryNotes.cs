/*
 * Author:      Ryan Curran
 * Date:        September 2019
 * Description: Class for Dip And Doze Outbound Tasks
 *              Methods for Moving Dip And Doze Delivery Note File to FTP folder in correct format.
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Outbound.DeliveryNotes
{
    class DipAndDozeDeliveryNotes
    {
        public static string OriginalPath = @"\\APP01\SalesOrders\DipAndDoze\DeliveryNotes\";
        public static void MoveDipAndDozeDeliveryNoteFile()
        {
            string[] fileEntries = Directory.GetFiles(OriginalPath);

            try
            {
                foreach (string fileName in fileEntries)
                {
                    File.Delete(@"\\DC01\Company\Intact IQ\BehrensFTP\DeliveryNotes\DipAndDoze\DeliveryNotes.csv");
                    File.Move(@"\\APP01\SalesOrders\DipAndDoze\DeliveryNotes\DeliveryNotes.csv", @"\\DC01\Company\Intact IQ\BehrensFTP\DeliveryNotes\DipAndDoze\DeliveryNotes.csv");

                    string message1 = "Dip & Doze Delivery Notes Moved";
                    LogFile.WritetoLogFile(message1, true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                string message1 = "There were no files to move";
                LogFile.WritetoLogFile(message1, true);
            }

        }
    }
}
