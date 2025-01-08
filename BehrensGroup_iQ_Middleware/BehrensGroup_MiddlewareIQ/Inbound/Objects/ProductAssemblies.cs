/*
* Author:      Ryan Curran
* Date:        August 2019
* Description: Class for reading & uploading Product Assemblies for Embroidery Module
*              Assemblies are created in CSV file and uploaded to iQ.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Transactions;
using BehrensGroup_MiddlewareIQ.Tools;

namespace BehrensGroup_MiddlewareIQ.Inbound.Objects
{
    class ProductAssemblies
    {
        public static string path = @"\\APP01\SalesOrders\Products\ProductAssemblies";        //ProductionPath

        //public static string path = @"C:\Users\rcurran.BEHRENS\Desktop\DipAndDoze";     //TestPath
        public static string EDIFileName;
        public static void ProductAssembliesMain()
        {
            DownloadSchemaProductAssemblies download = new DownloadSchemaProductAssemblies();

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
                        List<Product_Assembly> listProduct_Assemblies = new List<Product_Assembly>();

                        string CurrentProductCode = "";
                        string LastProductCode = "";
                        string CurrentAssemblyNumber = "";
                        string LastAssemblyNumber = "";

                        foreach (string[] values in reader.RowEnumerator)
                        {
                            CurrentProductCode = values[0];
                            CurrentAssemblyNumber = values[1];

                            if (CurrentProductCode.Length > 0 && CurrentProductCode != "Product" && CurrentProductCode != "Code" && (CurrentAssemblyNumber != LastAssemblyNumber || CurrentProductCode != LastProductCode))
                            {
                                Product_Assembly product_Assembly = new Product_Assembly();
                                product_Assembly = download.Product_Assembly(values);

                                if (product_Assembly != null)
                                {
                                    Product_AssemblyRevision product_AssemblyRevision = new Product_AssemblyRevision();
                                    product_AssemblyRevision = download.Product_AssemblyRevision(values, product_Assembly);

                                    using (CsvReader reader3 = new CsvReader(filePath))
                                    {
                                        string CurrentProductCode2 = "";
                                        string CurrentAssemblyNumber2 = "";
                                        string LastComponentProduct = "";

                                        foreach (string[] values3 in reader3.RowEnumerator)
                                        {
                                            CurrentProductCode2 = values3[0];
                                            CurrentAssemblyNumber2 = values3[1];

                                            string CurrentComponentProduct = values3[5];

                                            if (CurrentProductCode2.Length > 0 && CurrentProductCode2 == CurrentProductCode && CurrentAssemblyNumber2 == CurrentAssemblyNumber && LastComponentProduct != CurrentComponentProduct)
                                            {
                                                Product_AssemblyRevision_Component product_AssemblyRevision_Component = new Product_AssemblyRevision_Component();
                                                product_AssemblyRevision_Component = download.Product_AssemblyRevision_Component(values3, product_Assembly, product_AssemblyRevision);

                                                if (product_AssemblyRevision != null)
                                                {
                                                    product_AssemblyRevision.Components.Add(product_AssemblyRevision_Component);
                                                }
                                            }
                                            LastComponentProduct = CurrentComponentProduct;
                                        }
                                    }
                                    product_Assembly.Product_AssemblyRevisions.Add(product_AssemblyRevision);
                                    product_Assembly.UpdateProduct_AssemblyRevision();
                                }
                            }
                            LastProductCode = CurrentProductCode;
                            LastAssemblyNumber = CurrentAssemblyNumber;
                        }
                    }
                    
                    string CurrentDateTime = DateTime.Now.ToString();
                    CurrentDateTime = CurrentDateTime.Replace("/", "-");
                    CurrentDateTime = CurrentDateTime.Replace(":", "-");

                    File.Move(filePath, path + @"\Complete\ProductAssemblies-" + CurrentDateTime + ".csv");

                    string message = "ProductAssembly-Successfully added file - " + EDIFileName + "-" + CurrentDateTime + ".csv";
                    LogFile.WritetoLogFile(message, true);

                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                //Write to Log File
                string message = "ProductAssembly-Cannot read file - " + EDIFileName + ".csv";
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);
            }
        }
    }
    class DownloadSchemaProductAssemblies : GenericClassTransactionLine
    {
        public string ProductCode { get; set; }              //Customer Reference (Unique Per Order Web Generated No) 
        public string AssemblyRevisionNumber { get; set; }
        public string AssemblyRevisionDescription { get; set; }
        public string AssemblyRevisionDefaultBuildType { get; set; }

        public string AssemblyRevisionRevisionState { get; set; }

        public string AssemblyRevisionFulfillmentType { get; set; }
        public string AssemblyRevisionDefaultSubContractSupplier { get; set; }
        public string AssemblyRevisionSubContractProductCode { get; set; }
        public string AssemblyRevisionDescriptionOfService { get; set; }
        public string AssemblyRevisionSupplierCostBase { get; set; }

        public string ComponentProduct { get; set; }
        public string ComponentQuantity { get; set; }
        public string ComponentStockAdjustmentRule { get; set; }
        public string ComponentUsageValue { get; set; }

        public Product_Assembly Product_Assembly(string[] DownloadInfo)
        {
            Product Product = new Product();
            Product_Assembly Product_Assembly = new Product_Assembly();

            try
            {
                ProductCode = DownloadInfo[0];
                Product = Product.GetProductCode(ProductCode);
                Product_Assembly.ProductID = Product.ID;

                return Product_Assembly;
            }
            catch (Exception e)
            {
                string message = "ProductAssembly-Cannot update Product - " + Product_Assembly.Code;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
        public Product_AssemblyRevision Product_AssemblyRevision(string[] DownloadInfo, Product Product)
        {
            Product_AssemblyRevision Product_AssemblyRevision = new Product_AssemblyRevision() { ID = Product.ProductID };

            try
            {
                Product_AssemblyRevision.Number = DownloadInfo[1];
                Product_AssemblyRevision.Description = DownloadInfo[2];
                Product_AssemblyRevision.DefaultBuildType = DownloadInfo[3];
                Product_AssemblyRevision.RevisionState = DownloadInfo[4];
                Product_AssemblyRevision.FulfillmentType = DownloadInfo[8];
                Product_AssemblyRevision.DefaultSubContractSupplier.Code = DownloadInfo[9];
                Product_AssemblyRevision.SubContractProductCode = Product_AssemblyRevision.SubContractProductCode.GetProductCode(DownloadInfo[10]);
                Product_AssemblyRevision.DescriptionOfService = DownloadInfo[11];
                Product_AssemblyRevision.SupplierCostBase = DownloadInfo[12];

                return Product_AssemblyRevision;
            }
            catch (Exception e)
            {
                string message = "ProductAssembly-Cannot create Assembly - " + Product.Code + " - " + Product_AssemblyRevision.Number;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
        public Product_AssemblyRevision_Component Product_AssemblyRevision_Component(string[] DownloadInfo, Product Product, Product_AssemblyRevision Product_AssemblyRevision) 
        {
            Product_AssemblyRevision_Component Product_AssemblyRevision_Component = new Product_AssemblyRevision_Component();

            try
            {
                Product_AssemblyRevision_Component.Product = Product_AssemblyRevision_Component.Product.GetProductCode(DownloadInfo[5]);
                Product_AssemblyRevision_Component.Quantity = DownloadInfo[6];
                Product_AssemblyRevision_Component.StockAdjustmentRule = DownloadInfo[7];
                Product_AssemblyRevision_Component.TotalUsageValue = DownloadInfo[13];
                
                return Product_AssemblyRevision_Component;
            }
            catch (Exception e)
            {
                string message = "ProductAssembly-Cannot create Assembly Component - " + Product.Code + " - " + Product_AssemblyRevision.Number + " - " + Product_AssemblyRevision_Component.Product.Code;
                Console.WriteLine(e.Message);
                LogFile.WritetoLogFile(message, false);

                return null;
            }
        }
    }
}
