using BehrensGroup_MiddlewareIQ.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace BehrensGroup_MiddlewareIQ.Outbound.Invoices
{
    public class Foodbuy
    {
        public static void UploadInvoice()
        {
            string OldPath = @"\\APP01\SalesOrders\Foodbuy\Invoices\";
            
            string EDIFileName;

            try
            {
                string[] fileEntries = Directory.GetFiles(OldPath);

                foreach (string fileName in fileEntries)
                {
                    string filePath = fileName;
                    EDIFileName = Path.GetFileName(fileName).Replace(".csv", "");
                    
                    FTPFunctions.UploadFile(EDIFileName, fileName);

                    File.Move(filePath, OldPath + @"\Complete\" + EDIFileName + ".csv");
                }
            }
            catch
            {

            }
        }
    }
}
