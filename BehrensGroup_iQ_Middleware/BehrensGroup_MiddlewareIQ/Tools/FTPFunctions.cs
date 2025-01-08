/*
 * Author:      Ryan Curran
 * Date:        November 2020
 * Description: Class for communicating with FTP
 *              Contains Upload File & Download Directory Functions.
 */

using FluentFTP;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace BehrensGroup_MiddlewareIQ.Tools
{
    public static class FTPFunctions
    {

        const string host = "ftp.epsys.co.uk";

        const string Inbox = @"/Inbox/";
        const string Outbox = @"/Outbox/";

        const string Username = "behrenshealthcare";
        const string Password = "KP4f{p37*He2aOn?W";


        public static void DownloadFile()
        {
            string trnsfrpth = @"\\APP01\SalesOrders\Foodbuy\Orders\";

            using (var ftp = new FtpClient(host, Username, Password))
            {
                ftp.Connect();

                // download a folder and all its files
                ftp.DownloadDirectory(trnsfrpth, Outbox, FtpFolderSyncMode.Update);

                string[] fileEntries = Directory.GetFiles(trnsfrpth);
                foreach (string fileName in fileEntries)
                {
                    ftp.DeleteFile(Outbox + Path.GetFileName(fileName));
                }
            }
        }

        public static void UploadFile(string EDIFileName, string fileName)
        {

            using (var ftp = new FtpClient(host, Username, Password))
            {
                ftp.UploadFile(fileName, Inbox + EDIFileName + ".csv");
            }

            /*
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest) WebRequest.Create(host + Inbox + @"\" + EDIFileName + ".csv");
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(Username, Password);

            // Copy the contents of the file to the request stream.
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(fileName))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse) request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
            */

        }
        public static void PhoenixUploadFile(string EDIFileName, string fileName, string host, string username, string password, string folder)
        {

            using (var ftp = new FtpClient(host, username, password))
            {
                ftp.UploadFile(fileName, folder + EDIFileName + ".csv");
            }
        }
    }
}
