/*
 * Author:      Ryan Curran
 * Date:        Jan 2021
 * Description: Class for Object StockBinTranferRequestLine
 *              Methods for Retreiving, Updating & Adding Stock Bin Transfer Request via REST API
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using Newtonsoft.Json;
using RestSharp;

using BehrensGroup_ClassLibrary.BaseClasses;
using BehrensGroup_ClassLibrary.Object;
using BehrensGroup_ClassLibrary.Functions;

namespace BehrensGroup_ClassLibrary.Transactions
{
    public class StockBinTransferRequest_Line : GenericClassTransactionLine
    {
        //ID                    - GenericClassID
        //Number                - GenericClassTransactionNumber
        //CreatedDate           - GenericClassTransactionNumber
        //LineNo                - GenericClassTransactionLine

        public Product Product = new Product();
        public long ProductID { get; set; }
        public ProductBatch ProductBatch = new ProductBatch();

        public StockBin FromStockBin = new StockBin();
        public StockBin ToStockBin = new StockBin();
        public decimal Quantity { get; set; }
    }
}
