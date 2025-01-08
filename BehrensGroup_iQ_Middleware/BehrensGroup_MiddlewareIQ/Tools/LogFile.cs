using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BehrensGroup_MiddlewareIQ.Tools
{
    class LogFile
    {
        public static void WritetoLogFile(string message, bool success)
        {
            string path = @"\\APP01\SalesOrders";
            string LogFilePath = "";

            StringBuilder myString = new StringBuilder();

            if (success == true)
            {
                myString.Append("SUCCESS: " + message + " - " + DateTime.Now);
                LogFilePath = path + @"\LogFile-Success.txt";
            }
            else
            {
                myString.Append("ERROR: " + message + " - " + DateTime.Now);
                LogFilePath = path + @"\LogFile-Error.txt";
            }

            StreamWriter file = new StreamWriter(LogFilePath, true);
            file.WriteLine(myString);
            file.Close();
        }
    }
}
